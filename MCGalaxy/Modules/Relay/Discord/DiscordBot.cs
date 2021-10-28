/*
    Copyright 2015 MCGalaxy
        
    Dual-licensed under the Educational Community License, Version 2.0 and
    the GNU General Public License, Version 3 (the "Licenses"); you may
    not use this file except in compliance with the Licenses. You may
    obtain a copy of the Licenses at
    
    http://www.opensource.org/licenses/ecl2.php
    http://www.gnu.org/licenses/gpl-3.0.html
    
    Unless required by applicable law or agreed to in writing,
    software distributed under the Licenses are distributed on an "AS IS"
    BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express
    or implied. See the Licenses for the specific language governing
    permissions and limitations under the Licenses.
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using MCGalaxy.Config;
using MCGalaxy.Events.GroupEvents;
using MCGalaxy.Events.PlayerEvents;
using MCGalaxy.Events.ServerEvents;
using MCGalaxy.Util;

namespace MCGalaxy.Modules.Relay.Discord 
{
    public sealed class DiscordBot : RelayBot 
    {
        DiscordApiClient api;
        DiscordWebsocket socket;
        DiscordSession session;
        string botUserID;
        
        Dictionary<string, bool> isDMChannel = new Dictionary<string, bool>();
        List<string> filter_triggers = new List<string>();
        List<string> filter_replacements = new List<string>();
        JsonArray allowed;

        public override string RelayName { get { return "Discord"; } }
        public override bool Enabled     { get { return Config.Enabled; } }
        public override string UserID    { get { return botUserID; } }
        public DiscordConfig Config;
        
        TextFile replacementsFile = new TextFile("text/discord/replacements.txt",
                                        "// This file is used to replace words/phrases sent to discord",
                                        "// Lines starting with // are ignored",
                                        "// Lines should be formatted like this:",
                                        "// example:http://example.org",
                                        "// That would replace 'example' in messages sent with 'http://example.org'");
        
        
        protected override bool CanReconnect {
            get { return canReconnect && (socket == null || socket.CanReconnect); }
        }
        
        protected override void DoConnect() {
            socket = new DiscordWebsocket();
            socket.Session   = session;
            socket.Token     = Config.BotToken;
            socket.Presence  = Config.PresenceEnabled;
            socket.Status    = Config.Status;
            socket.Activity  = Config.Activity;
            socket.GetStatus = GetStatusMessage;
            
            socket.OnReady         = HandleReadyEvent;
            socket.OnResumed       = HandleResumedEvent;
            socket.OnMessageCreate = HandleMessageEvent;
            socket.OnChannelCreate = HandleChannelEvent;
            socket.Connect();
        }
                
        // mono wraps exceptions from reading in an AggregateException, e.g:
        //   * AggregateException - One or more errors occurred.
        //      * ObjectDisposedException - Cannot access a disposed object.
        // .NET sometimes wraps exceptions from reading in an IOException, e.g.:
        //   * IOException - The read operation failed, see inner exception.
        //      * ObjectDisposedException - Cannot access a disposed object.
        static Exception UnpackError(Exception ex) {
            if (ex.InnerException is ObjectDisposedException)
                return ex.InnerException;
            if (ex.InnerException is IOException)
                return ex.InnerException;
            
            // TODO can we ever get an IOException wrapping an IOException?
            return null;
        }
        
        protected override void DoReadLoop() {
            try {
                socket.ReadLoop();
            } catch (Exception ex) {
                Exception unpacked = UnpackError(ex);
                // throw a more specific exception if possible
                if (unpacked != null) throw unpacked;
                
                // rethrow original exception otherwise
                throw;
            }
        }
        
        protected override void DoDisconnect(string reason) {
            try {
                socket.Disconnect();
            } catch {
                // no point logging disconnect failures
            }
        }
        
        
        public override void ReloadConfig() {
            Config.Load();
            base.ReloadConfig();
            LoadReplacements();
        }
        
        protected override void UpdateConfig() {
            Channels     = Config.Channels.SplitComma();
            OpChannels   = Config.OpChannels.SplitComma();
            IgnoredUsers = Config.IgnoredUsers.SplitComma();
            
            UpdateAllowed();
            LoadBannedCommands();
        }
        
        void UpdateAllowed() {
            JsonArray mentions = new JsonArray();
            if (Config.CanMentionUsers) mentions.Add("users");
            if (Config.CanMentionRoles) mentions.Add("roles");
            if (Config.CanMentionHere)  mentions.Add("everyone");
            allowed = mentions;
        }
        
        void LoadReplacements() {
            replacementsFile.EnsureExists();            
            string[] lines = replacementsFile.GetText();
            
            filter_triggers.Clear();
            filter_replacements.Clear();
            
            ChatTokens.LoadTokens(lines, (phrase, replacement) => 
                                  {
                                      filter_triggers.Add(phrase);
                                      filter_replacements.Add(MarkdownToSpecial(replacement));
                                  });
        }
        
        public override void LoadControllers() {
            Controllers = PlayerList.Load("text/discord/controllers.txt");
        }
        
        
        string GetNick(JsonObject data) {
            if (!Config.UseNicks) return null;
            object raw;
            if (!data.TryGetValue("member", out raw)) return null;
            
            // Make sure this is really a member object first
            JsonObject member = raw as JsonObject;
            if (member == null) return null;
            
            member.TryGetValue("nick", out raw);
            return raw as string;
        }
        
        RelayUser ExtractUser(JsonObject data) {
            JsonObject author = (JsonObject)data["author"];
            string channel    = (string)data["channel_id"];
            string message    = (string)data["content"];
            
            RelayUser user = new RelayUser();
            user.Nick = GetNick(data) ?? (string)author["username"];
            user.ID   =                  (string)author["id"];
            return user;
        }

        
        void HandleReadyEvent(JsonObject data) {
            JsonObject user = (JsonObject)data["user"];
            botUserID       = (string)user["id"];
            HandleResumedEvent(data);
        }
        
        void HandleResumedEvent(JsonObject data) {
            // May not be null when reconnecting
            if (api == null) {
                api = new DiscordApiClient();
                api.Token = Config.BotToken;
                api.RunAsync();
            }
            OnReady();
        }
        
        void PrintAttachments(JsonObject data, string channel) {
            object raw;
            if (!data.TryGetValue("attachments", out raw)) return;
            
            JsonArray list = raw as JsonArray;
            if (list == null) return;
            RelayUser user = ExtractUser(data);
            
            foreach (object entry in list) {
                JsonObject attachment = entry as JsonObject;
                if (attachment == null) continue;
                
                string url = (string)attachment["url"];
                HandleChannelMessage(user, channel, url);
            }
        }
        
        void HandleMessageEvent(JsonObject data) {
            RelayUser user = ExtractUser(data);
            // ignore messages from self
            if (user.ID == botUserID) return;
            
            string channel = (string)data["channel_id"];
            string message = (string)data["content"];
            bool isDM;
            
            if (isDMChannel.TryGetValue(channel, out isDM)) {
                HandleDirectMessage(user, channel, message);
            } else {
                HandleChannelMessage(user, channel, message);
                PrintAttachments(data, channel);
            }
        }
        
        void HandleChannelEvent(JsonObject data) {
            string channel = (string)data["id"];
            string type    = (string)data["type"];
            
            if (type == "1") isDMChannel[channel] = true;
        }
        
        
        static bool IsEscaped(char c) {
            // To match Discord: \a --> \a, \* --> *
            return (c >  ' ' && c <= '/') || (c >= ':' && c <= '@') 
                || (c >= '[' && c <= '`') || (c >= '{' && c <= '~');
        }        
        protected override string ParseMessage(string input) {
            StringBuilder sb = new StringBuilder(input);
            SimplifyCharacters(sb);
            
            // remove variant selector character used with some emotes
            sb.Replace("\uFE0F", "");
            
            // unescape \ escaped characters
            //  -1 in case message ends with a \
            int length = sb.Length - 1;
            for (int i = 0; i < length; i++) 
            {
                if (sb[i] != '\\') continue;
                if (!IsEscaped(sb[i + 1])) continue;
                
                sb.Remove(i, 1); length--;
            }
            return sb.ToString();
        }
        
        string GetStatusMessage() {
            string online = PlayerInfo.NonHiddenCount().ToString();
            return Config.StatusMessage.Replace("{PLAYERS}", online);
        }
        
        void UpdateDiscordStatus() {
            try { socket.UpdateStatus(); } catch { }
        }
        
        
        protected override void OnStart() {
            session = new DiscordSession();
            base.OnStart();
            
            OnPlayerConnectEvent.Register(HandlePlayerConnect, Priority.Low);
            OnPlayerDisconnectEvent.Register(HandlePlayerDisconnect, Priority.Low);
            OnPlayerActionEvent.Register(HandlePlayerAction, Priority.Low);
        }
        
        protected override void OnStop() {
            socket = null;
            if (api != null) {
                api.StopAsync();
                api = null;
            }
            base.OnStop();
            
            OnPlayerConnectEvent.Unregister(HandlePlayerConnect);
            OnPlayerDisconnectEvent.Unregister(HandlePlayerDisconnect);
            OnPlayerActionEvent.Unregister(HandlePlayerAction);
        }
        
        void HandlePlayerConnect(Player p) { UpdateDiscordStatus(); }
        void HandlePlayerDisconnect(Player p, string reason) { UpdateDiscordStatus(); }
        
        void HandlePlayerAction(Player p, PlayerAction action, string message, bool stealth) {
            if (action != PlayerAction.Hide && action != PlayerAction.Unhide) return;
            UpdateDiscordStatus();
        }
        
        
        /// <summary> Asynchronously sends a message to the discord API </summary>
        public void Send(DiscordApiMessage msg) {
            // can be null in gap between initial connection and ready event received
            if (api != null) api.SendAsync(msg);
        }
        
        protected override void DoSendMessage(string channel, string message) {
            ChannelSendMessage msg = new ChannelSendMessage(channel, message);
            msg.Allowed = allowed;
            Send(msg);
        }
        
        protected override string ConvertMessage(string message) {
            message = base.ConvertMessage(message);
            message = Colors.StripUsed(message);
            message = EscapeMarkdown(message);
            message = SpecialToMarkdown(message);
            return message;
        }
        
        static readonly string[] markdown_special = {  @"\",  @"*",  @"_",  @"~",  @"`",  @"|" };
        static readonly string[] markdown_escaped = { @"\\", @"\*", @"\_", @"\~", @"\`", @"\|" };
        static string EscapeMarkdown(string message) {
            // don't let user use bold/italic etc markdown
            for (int i = 0; i < markdown_special.Length; i++) 
            {
                message = message.Replace(markdown_special[i], markdown_escaped[i]);
            }
            return message;
        }
        
        protected override string PrepareMessage(string message) {
            // allow uses to do things like replacing '+' with ':green_square:'
            for (int i = 0; i < filter_triggers.Count; i++) 
            {
                message = message.Replace(filter_triggers[i], filter_replacements[i]);
            }
            return message;
        }
        
        
        // all users are already verified by Discord
        protected override bool CheckController(string userID, ref string error) { return true; }
        
        protected override string UnescapeFull(Player p) {
            return BOLD + base.UnescapeFull(p) + BOLD;
        }        
        protected override string UnescapeNick(Player p) {
            return BOLD + base.UnescapeNick(p) + BOLD;
        }
        
        
        static string FormatRank(OnlineListEntry e) {
            return string.Format(UNDERLINE + "{0}" + UNDERLINE + " (" + CODE + "{1}" + CODE + ")",
                                 e.group.GetFormattedName(), e.players.Count);
        }

        static string FormatNick(Player p, Player pl) {
            string flags  = OnlineListEntry.GetFlags(pl);
            string format;
            
            if (flags.Length > 0) {
                format = BOLD + "{0}" + BOLD + ITALIC + "{2}" + ITALIC + " (" + CODE + "{1}" + CODE + ")";
            } else {
                format = BOLD + "{0}" + BOLD                           + " (" + CODE + "{1}" + CODE + ")";
            }
            return string.Format(format, p.FormatNick(pl), 
                                 // level name must not have _ escaped as the level name is in a code block -
                                 //  otherwise the escaped "\_" actually shows as "\_" instead of "_" 
                                 pl.level.name.Replace('_', UNDERSCORE),
                                 flags);
        }
        
        static string FormatPlayers(Player p, OnlineListEntry e) {
            return e.players.Join(pl => FormatNick(p, pl), ", ");
        }
        
        protected override void MessagePlayers(RelayPlayer p) {
            ChannelSendEmbed embed = new ChannelSendEmbed(p.ChannelID);
            int total;
            List<OnlineListEntry> entries = PlayerInfo.GetOnlineList(p, p.Rank, out total);
            
            embed.Color = Config.EmbedColor;
            embed.Title = string.Format("{0} player{1} currently online",
                                        total, total.Plural());
            
            foreach (OnlineListEntry e in entries) 
            {
                if (e.players.Count == 0) continue;
                
                embed.Fields.Add(
                    ConvertMessage(FormatRank(e)),
                    ConvertMessage(FormatPlayers(p, e))
                );
            }
            Send(embed);
        }
        
        
        // these characters are chosen specifically to lie within the unspecified unicode range
        //  https://en.wikipedia.org/wiki/Private_Use_Areas
        const char UNDERSCORE = '\uEDC1'; // _
        const char TILDE      = '\uEDC2'; // ~
        const char STAR       = '\uEDC3'; // *
        const char GRAVE      = '\uEDC4'; // `
        const char BAR        = '\uEDC5'; // |
        
        public const string UNDERLINE     = "\uEDC1\uEDC1"; // __
        public const string BOLD          = "\uEDC3\uEDC3"; // **
        public const string ITALIC        = "\uEDC1"; // _
        public const string CODE          = "\uEDC4"; // `
        public const string SPOILER       = "\uEDC5\uEDC5"; // ||
        public const string STRIKETHROUGH = "\uEDC2\uEDC2"; // ~~
        
        static string MarkdownToSpecial(string input) {
            return input
                .Replace('_', UNDERSCORE)
                .Replace('~', TILDE)
                .Replace('*', STAR)
                .Replace('`', GRAVE)
                .Replace('|', BAR);
        }
        
        static string SpecialToMarkdown(string input) {
            return input
                .Replace(UNDERSCORE, '_')
                .Replace(TILDE,      '~')
                .Replace(STAR,       '*')
                .Replace(GRAVE,      '`')
                .Replace(BAR,        '|');
        }
    }
}
