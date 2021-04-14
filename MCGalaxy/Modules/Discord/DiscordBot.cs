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
using System.Threading;
using MCGalaxy.Config;
using MCGalaxy.Events.GroupEvents;
using MCGalaxy.Events.PlayerEvents;
using MCGalaxy.Events.ServerEvents;

namespace MCGalaxy.Modules.Discord {

    public sealed class DiscordBot {
        public bool Disconnected;
        
        DiscordApiClient api;
        DiscordWebsocket socket;
        DiscordConfig config;
        Thread thread;

        void HandleEvent(JsonObject obj) {
            // actually handle the event
            string eventName = (string)obj["t"];
            if (eventName == "MESSAGE_CREATE") HandleMessageEvent(obj);
        }
        
        void HandleMessageEvent(JsonObject obj) {
            JsonObject data   = (JsonObject)obj["d"];
            JsonObject author = (JsonObject)data["author"];
            string message    = (string)data["content"];
            
            string user = (string)author["username"];
            string msg  = "&I(Discord) " + user + ": &f" + message;
            Logger.Log(LogType.IRCChat, msg);
            Chat.Message(ChatScope.Global, msg, null, null);
        }
        
        string GetStatus() {
            string online = PlayerInfo.NonHiddenCount().ToString();
            return config.Status.Replace("{PLAYERS}", online);
        }        
        
        readonly Player ircDefault = new Player("Discord");
        void OnReady() {
            ircDefault.group = Group.DefaultRank;
            
            api = new DiscordApiClient();
            api.Token = config.BotToken;
            RegisterEvents();
        }
        
        
        void RegisterEvents() {
            OnPlayerConnectEvent.Register(HandlePlayerConnect, Priority.Low);
            OnPlayerDisconnectEvent.Register(HandlePlayerDisconnect, Priority.Low);
            OnGroupLoadEvent.Register(HandleGroupLoad, Priority.Low);
            
            OnChatEvent.Register(HandleChat, Priority.Low);
            OnChatSysEvent.Register(HandleChatSys, Priority.Low);
            OnChatFromEvent.Register(HandleChatFrom, Priority.Low);
        }
        
        void UnregisterEvents() {
            OnPlayerConnectEvent.Unregister(HandlePlayerConnect);
            OnPlayerDisconnectEvent.Unregister(HandlePlayerDisconnect);
            OnGroupLoadEvent.Unregister(HandleGroupLoad);
            
            OnChatEvent.Unregister(HandleChat);
            OnChatSysEvent.Unregister(HandleChatSys);
            OnChatFromEvent.Unregister(HandleChatFrom);
        }
        
        string Unescape(Player p, string msg) {
            string full = Server.Config.IRCShowPlayerTitles ? p.FullName : p.group.Prefix + p.ColoredName;
            return msg.Replace("λFULL", full).Replace("λNICK", p.ColoredName);
        }
        
        void HandlePlayerConnect(Player p) { socket.SendUpdateStatus(); }
        void HandlePlayerDisconnect(Player p, string reason) { socket.SendUpdateStatus(); }
        void HandleGroupLoad() { ircDefault.group = Group.DefaultRank; }
        
        // TODO to much overlap with IRCHandlers
        void MessageToIRC(ChatScope scope, string msg, object arg, ChatMessageFilter filter) {
            ChatMessageFilter scopeFilter = Chat.scopeFilters[(int)scope];
            
            if (scopeFilter(ircDefault, arg) && (filter == null || filter(ircDefault, arg))) {
            	api.SendMessage("678784831164776454", msg);
            }
        }
        
        void HandleChatSys(ChatScope scope, string msg, object arg,
                           ref ChatMessageFilter filter, bool irc) {
            if (irc) MessageToIRC(scope, msg, arg, filter);
        }
        
        void HandleChatFrom(ChatScope scope, Player source, string msg,
                            object arg, ref ChatMessageFilter filter, bool irc) {
            if (irc) MessageToIRC(scope, Unescape(source, msg), arg, filter);
        }
        
        void HandleChat(ChatScope scope, Player source, string msg,
                        object arg, ref ChatMessageFilter filter, bool irc) {
            if (irc) MessageToIRC(scope, Unescape(source, msg), arg, filter);
        }
        
        
        public void RunAsync(DiscordConfig conf) {
            config = conf;
            socket = new DiscordWebsocket();
            
            socket.Token     = config.BotToken;
            socket.Handler   = HandleEvent;
            socket.GetStatus = GetStatus;
            socket.OnReady   = OnReady;
                
            thread      = new Thread(IOThread);
            thread.Name = "DiscordRelayBot";
            thread.IsBackground = true;
            thread.Start();
        } // TODO hide / unhide
        
        void IOThread() {
            try {
                socket.Connect();
                socket.ReadLoop();
            } catch (Exception ex) {
                Logger.LogError("Discord relay error", ex);
            }
        }
        
        volatile bool disconnecting;
        public void Stop() {
            if (disconnecting) return;
            disconnecting = true;
            
            try {
                socket.Disconnect();
            } finally {
                Disconnected = true;
                UnregisterEvents();
            }
        }
    }
}
