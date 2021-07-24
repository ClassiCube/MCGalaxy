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

namespace MCGalaxy.Modules.Relay.Discord {

    public sealed class DiscordBot : RelayBot {
        DiscordApiClient api;
        DiscordWebsocket socket;
        string botUserID;
        
        Dictionary<string, bool> isDMChannel = new Dictionary<string, bool>();
        List<string> filter_triggers = new List<string>();
        List<string> filter_replacements = new List<string>();

        public override string RelayName { get { return "Discord"; } }
        public override bool Enabled     { get { return Config.Enabled; } }
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
            socket.Token     = Config.BotToken;
            socket.Presence  = Config.PresenceEnabled;
            socket.Status    = Config.Status;
            socket.Activity  = Config.Activity;
            socket.GetStatus = GetStatusMessage;
            
            socket.OnReady         = HandleReadyEvent;
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
            LoadBannedCommands();
        }
        
        void LoadReplacements() {
            replacementsFile.EnsureExists();            
            string[] lines = replacementsFile.GetText();
            
            filter_triggers.Clear();
            filter_replacements.Clear();
            
            ChatTokens.LoadTokens(lines, (phrase, replacement) => 
                                  {
                                      filter_triggers.Add(phrase);
                                      filter_replacements.Add(replacement);
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
        
        
        protected override string ParseMessage(string input) {
            StringBuilder sb = new StringBuilder(input);
            SimplifyCharacters(sb);
            
            // remove variant selector character used with some emotes
            sb.Replace("\uFE0F", "");
            return sb.ToString();
        }
        
        string GetStatusMessage() {
            string online = PlayerInfo.NonHiddenCount().ToString();
            return Config.StatusMessage.Replace("{PLAYERS}", online);
        }
        
        void UpdateDiscordStatus() {
            try { socket.SendUpdateStatus(); } catch { }
        }
        
        
        protected override void OnStart() {
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
            Send(new ChannelSendMessage(channel, message));
        }
        
        protected override string ConvertMessage(string message) {
            message = base.ConvertMessage(message);
            message = Colors.StripUsed(message);
            return message;
        }
        
        static readonly string[] markdown_special = {  @"\",  @"*",  @"_",  @"~",  @"`",  @"|" };
        static readonly string[] markdown_escaped = { @"\\", @"\*", @"\_", @"\~", @"\`", @"\|" };
        static string StripMarkdown(string message) {
             // don't let user use bold/italic etc markdown
            for (int i = 0; i < markdown_special.Length; i++) {
                message = message.Replace(markdown_special[i], markdown_escaped[i]);
            }
             return message;
        }
        
        protected override string PrepareMessage(string message) {
            message = StripMarkdown(message);
            // allow uses to do things like replacing '+' with ':green_square:'
            for (int i = 0; i < filter_triggers.Count; i++) {
                message = message.Replace(filter_triggers[i], filter_replacements[i]);
            }
            return message;
        }
        
        
        // all users are already verified by Discord
        protected override bool CheckController(string userID, ref string error) { return true; }
        
        protected override string UnescapeFull(Player p) {
            return "**" + StripMarkdown(base.UnescapeFull(p)) + "**";
        }
        
        protected override string UnescapeNick(Player p) {
            return "**" + StripMarkdown(base.UnescapeNick(p)) + "**";
        }
        
        
        static string FormatRank(OnlineListEntry e) {
            return string.Format("__{0}__ (`{1}`)",
                                 e.group.GetFormattedName(), e.players.Count);
        }

        static string FormatNick(Player p, Player pl) {
            string flags  = OnlineListEntry.GetFlags(pl);
            string format = flags.Length > 0 ? "**{0}**_{2}_ (`{1}`)" : "**{0}** (`{1}`)";            
            return string.Format(format, p.FormatNick(pl), pl.level.name, flags);
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
            foreach (OnlineListEntry e in entries) {
                if (e.players.Count == 0) continue;
                
                embed.Fields.Add(
                    ConvertMessage(FormatRank(e)),
                    ConvertMessage(FormatPlayers(p, e))
                );
            }
            Send(embed);
        }
    }
}
