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
using MCGalaxy.Config;
using MCGalaxy.Events.PlayerEvents;
using MCGalaxy.Games;
using MCGalaxy.Tasks;
using MCGalaxy.Util;

namespace MCGalaxy.Modules.Relay.Discord 
{
    public sealed class DiscordBot : RelayBot 
    {
        DiscordApiClient api;
        DiscordWebsocket socket;
        DiscordSession session;
        string botUserID;
        
        Dictionary<string, byte> channelTypes = new Dictionary<string, byte>();
        const byte CHANNEL_DIRECT = 0;
        const byte CHANNEL_TEXT   = 1;

        List<string> filter_triggers = new List<string>();
        List<string> filter_replacements = new List<string>();
        JsonArray allowed;

        public override string RelayName { get { return "Discord"; } }
        public override bool Enabled     { get { return Config.Enabled; } }
        public override string UserID    { get { return botUserID; } }
        public DiscordConfig Config;
        
        TextFile replacementsFile = new TextFile("text/discord/replacements.txt",
                                        "// This file is used to replace words/phrases sent to Discord",
                                        "// Lines starting with // are ignored",
                                        "// Lines should be formatted like this:",
                                        "// example:http://example.org",
                                        "// That would replace 'example' in messages sent to Discord with 'http://example.org'");
        
        
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
            
            if (!Config.CanMentionHere) return;
            Logger.Log(LogType.Warning, "can-mention-everyone option is enabled in {0}, " +
                       "which allows pinging all users on Discord from in-game. " +
                       "It is recommended that this option be disabled.", DiscordConfig.PROPS_PATH);
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
            byte type;

            // Working out whether a channel is a direct message channel
            //  or not without querying the Discord API is a bit of a pain
            // In v6 api, a CHANNEL_CREATE event was always emitted for
            //  direct message channels - hence the relatively simple
            //  solution was to treat every other channels as text channels
            // However, in v8 api changelog the following entry is noted:
            //  "Bots no longer receive Channel Create Gateway Event for DMs"
            // Therefore the code is now forced to instead calculate which
            //  channels are probably text channels, and which aren't        
            if (!channelTypes.TryGetValue(channel, out type))
            {
                type = GuessChannelType(data);
                // channel is definitely a text/normal channel
                if (type == CHANNEL_TEXT) channelTypes[channel] = type;
            }
            
            if (type == CHANNEL_DIRECT) {
                HandleDirectMessage(user, channel, message);
            } else {
                HandleChannelMessage(user, channel, message);
                PrintAttachments(data, channel);
            }
        }
        
        void HandleChannelEvent(JsonObject data) {
            string channel = (string)data["id"];
            string type    = (string)data["type"];

            // 1 = direct/private message channel type
            if (type == "1") channelTypes[channel] = CHANNEL_DIRECT;
        }

        byte GuessChannelType(JsonObject data) {
            // As per discord's documentation:
            //  "The member object exists in MESSAGE_CREATE and MESSAGE_UPDATE
            //   events from text-based guild channels, provided that the
            //   author of the message is not a webhook"
            if (data.ContainsKey("member")) return CHANNEL_TEXT;

            // As per discord's documentation
            //  "You can tell if a message is generated by a webhook by
            //   checking for the webhook_id on the message object."
            if (data.ContainsKey("webhook_id")) return CHANNEL_TEXT;

            // TODO are there any other cases to consider?
            return CHANNEL_DIRECT; // unknown
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
            
            StripMarkdown(sb);
            return sb.ToString();
        }
        
        static void StripMarkdown(StringBuilder sb) {
            // TODO proper markdown parsing
            sb.Replace("**", "");
        }


        readonly object updateLocker = new object();
        volatile bool updateScheduled;
        DateTime nextUpdate;

        public void UpdateDiscordStatus() {
            TimeSpan delay = default(TimeSpan);
            DateTime now   = DateTime.UtcNow;

            // websocket gets disconnected with code 4008 if try to send too many updates too quickly
            lock (updateLocker) {
                // status update already pending?
                if (updateScheduled) return;
                updateScheduled = true;

                // slowdown if sending too many status updates
                if (nextUpdate > now) delay = nextUpdate - now;
            }
            
            Server.MainScheduler.QueueOnce(DoUpdateStatus, null, delay);
        }

        void DoUpdateStatus(SchedulerTask task) {
            DateTime now = DateTime.UtcNow;
            // OK to queue next status update now
            lock (updateLocker) {
                updateScheduled = false;
                nextUpdate      = now.AddSeconds(0.5);
                // ensures status update can't be sent more than once every 0.5 seconds
            }

            DiscordWebsocket s = socket;
            // websocket gets disconnected with code 4003 if tries to send data before identifying
            //  https://discord.com/developers/docs/topics/opcodes-and-status-codes
            if (s == null || !s.SentIdentify) return;

            try { s.UpdateStatus(); } catch { }
        }

        string GetStatusMessage() {
            fakeGuest.group     = Group.DefaultRank;
            List<Player> online = PlayerInfo.GetOnlineCanSee(fakeGuest, fakeGuest.Rank); 

            string numOnline = online.Count.ToString();
            return Config.StatusMessage.Replace("{PLAYERS}", numOnline);
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
            if (api != null) api.QueueAsync(msg);
        }
        
        protected override void DoSendMessage(string channel, string message) {
            message = ConvertMessage(message);
            const int MAX_MSG_LEN = 2000;
            
            // Discord doesn't allow more than 2000 characters in a single message,
            //  so break up message into multiple parts for this extremely rare case
            //  https://discord.com/developers/docs/resources/channel#create-message
            for (int offset = 0; offset < message.Length; offset += MAX_MSG_LEN)
            {
                int partLen = Math.Min(message.Length - offset, MAX_MSG_LEN);
                string part = message.Substring(offset, partLen);
                
                ChannelSendMessage msg = new ChannelSendMessage(channel, part);
                msg.Allowed = allowed;
                Send(msg);
            }
        }
        
        /// <summary> Formats a message for displaying on Discord </summary>
        /// <example> Escapes markdown characters such as _ and * </example>
        string ConvertMessage(string message) {
            message = ConvertMessageCommon(message);
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
        
        protected override void MessagePlayers(RelayPlayer p) {
            ChannelSendEmbed embed = new ChannelSendEmbed(p.ChannelID);
            int total;
            List<OnlineListEntry> entries = PlayerInfo.GetOnlineList(p, p.Rank, out total);
            
            embed.Color  = Config.EmbedColor;
            embed.Title  = string.Format("{0} player{1} currently online",
                                        total, total.Plural());
            
            foreach (OnlineListEntry e in entries) 
            {
                if (e.players.Count == 0) continue;
                
                embed.Fields.Add(
                    ConvertMessage(FormatRank(e)),
                    ConvertMessage(FormatPlayers(p, e))
                );
            }
            AddGameStatus(embed);
            Send(embed);
        }
        
        static string FormatPlayers(Player p, OnlineListEntry e) {
            return e.players.Join(pl => FormatNick(p, pl), ", ");
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
        
        void AddGameStatus(ChannelSendEmbed embed) {
            if (!Config.EmbedGameStatuses) return;
            
            StringBuilder sb = new StringBuilder();
            IGame[] games    = IGame.RunningGames.Items;
            
            foreach (IGame game in games)
            {
                Level lvl = game.Map;
                if (!game.Running || lvl == null) continue;
                sb.Append(BOLD + game.GameName + BOLD + " is running on " + lvl.name + "\n");
            }
            
            if (sb.Length == 0) return;
            embed.Fields.Add("Running games", ConvertMessage(sb.ToString()));
        }
        
        
        // these characters are chosen specifically to lie within the unspecified unicode range,
        //  as those characters are "application defined" (EDCX = Escaped Discord Character #X)
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
