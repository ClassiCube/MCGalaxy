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
using System.Text;
using System.Threading;
using MCGalaxy.Config;
using MCGalaxy.Events.GroupEvents;
using MCGalaxy.Events.PlayerEvents;
using MCGalaxy.Events.ServerEvents;

namespace MCGalaxy.Modules.Relay.Discord {

    public sealed class DiscordBot : RelayBot {
        bool disconnected, disconnecting;
        DiscordApiClient api;
        DiscordWebsocket socket;
        string botUserID;
        Dictionary<string, bool> isDMChannel = new Dictionary<string, bool>();

        public override string RelayName { get { return "Discord"; } }
        public override bool Enabled     { get { return Config.Enabled; } }
        public override bool Connected   { get { return socket != null && !disconnected; } }
        public DiscordConfig Config;
        
        public override void LoadControllers() {
            Controllers = PlayerList.Load("ranks/Discord_Controllers.txt");
        }
        
        void TryReconnect() {
            try {
                Disconnect("Attempting reconnect");
                AutoReconnect();
            } catch (Exception ex) {
                Logger.LogError("Error reconnecting Discord relay", ex);
            }
        }
        
        void IOThread() {
            try {
                socket.Connect();
                socket.ReadLoop();
            } catch (Exception ex) {
                Logger.LogError("Discord relay error", ex);
                if (disconnecting || !socket.CanReconnect) return;
                
                // try to recover from dropped connection
                TryReconnect();
            }
        }
        
        protected override void DoConnect() {
            // TODO implement properly
            socket = new DiscordWebsocket();
            disconnecting = false;
            disconnected  = false;
            
            Channels   = Config.Channels.SplitComma();
            OpChannels = Config.OpChannels.SplitComma();
            
            socket.Token     = Config.BotToken;
            socket.GetStatus = GetStatus;
            
            socket.OnReady         = HandleReadyEvent;
            socket.OnMessageCreate = HandleMessageEvent;
            socket.OnChannelCreate = HandleChannelEvent;
            
            Thread worker = new Thread(IOThread);
            worker.Name   = "DiscordRelayBot";
            worker.IsBackground = true;
            worker.Start();
        }
        
        protected override void DoDisconnect(string reason) {
            disconnecting = true;
            try {
                if (api != null) api.StopAsync();
                socket.Disconnect();
            } finally {
                disconnected = true;
                UnregisterEvents();
            }
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
            
            api = new DiscordApiClient();
            api.Token = Config.BotToken;
            
            api.RunAsync();
            RegisterEvents();
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
        
        string GetStatus() {
            string online = PlayerInfo.NonHiddenCount().ToString();
            return Config.Status.Replace("{PLAYERS}", online);
        }
        
        
        void RegisterEvents() {
            OnPlayerConnectEvent.Register(HandlePlayerConnect, Priority.Low);
            OnPlayerDisconnectEvent.Register(HandlePlayerDisconnect, Priority.Low);
            OnPlayerActionEvent.Register(HandlePlayerAction, Priority.Low);
            HookEvents();
        }
        
        void UnregisterEvents() {
            OnPlayerConnectEvent.Unregister(HandlePlayerConnect);
            OnPlayerDisconnectEvent.Unregister(HandlePlayerDisconnect);
            OnPlayerActionEvent.Unregister(HandlePlayerAction);
            UnhookEvents();
        }
        
        void HandlePlayerConnect(Player p) { socket.SendUpdateStatus(); }
        void HandlePlayerDisconnect(Player p, string reason) { socket.SendUpdateStatus(); }
        
        
        protected override void DoSendMessage(string channel, string message) {
            if (api == null) return;
            api.SendMessageAsync(channel, message);
        }
                       
        protected override string ConvertMessage(string message) {
            message = EmotesHandler.Replace(message);
            message = ChatTokens.ApplyCustom(message);
            message = Colors.StripUsed(message);
            return message;
        }        
        
        void HandlePlayerAction(Player p, PlayerAction action, string message, bool stealth) {
            if (action != PlayerAction.Hide && action != PlayerAction.Unhide) return;
            socket.SendUpdateStatus();
        }
        
        
        // all users are already verified by Discord
        protected override bool CheckController(string userID, ref string error) { return true; }
        
        protected override string UnescapeFull(Player p) {
            return "**" + base.UnescapeFull(p) + "**";
        }
        
        protected override string UnescapeNick(Player p) {
            return "**" + base.UnescapeNick(p) + "**";
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
            
            embed.Title = string.Format("{0} player{1} currently online",
                                        total, total.Plural());
            foreach (OnlineListEntry e in entries) {
                if (e.players.Count == 0) continue;
                
                embed.Fields.Add(
                    ConvertMessage(FormatRank(e)),
                    ConvertMessage(FormatPlayers(p, e))
                );
            }
            api.SendAsync(embed);
        }
    }
}
