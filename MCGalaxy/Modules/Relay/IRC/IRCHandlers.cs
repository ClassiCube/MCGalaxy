/*
    Copyright 2011 MCForge
        
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
using MCGalaxy.Commands;
using MCGalaxy.Events;
using MCGalaxy.Events.GroupEvents;
using MCGalaxy.Events.PlayerEvents;
using MCGalaxy.Events.ServerEvents;
using MCGalaxy.Modules.Relay;
using MCGalaxy.DB;
using Sharkbite.Irc;

namespace MCGalaxy.Network {
    
    /// <summary> Handles messages and other events from IRC.</summary>
    public sealed class IRCHandlers {
        
        readonly IRCBot bot;
        public IRCHandlers(IRCBot bot) { this.bot = bot; }
        
        public volatile bool hookedEvents = false;
        Dictionary<string, List<string>> userMap = new Dictionary<string, List<string>>();

        static void MessageInGame(string srcNick, string message) {
            RelayBot.MessageInGame(srcNick, message);
        }
        
        /// <summary> Hooks IRC events so they are handled. </summary>
        public void Hook() {
            if (hookedEvents) return;
            hookedEvents = true;

            OnPlayerActionEvent.Register(HandlePlayerAction, Priority.Low);
            OnShuttingDownEvent.Register(HandleShutdown, Priority.Low);

            // Regster events for incoming
            bot.connection.Listener.OnNick += Listener_OnNick;
            bot.connection.Listener.OnRegistered += Listener_OnRegistered;
            bot.connection.Listener.OnAction += Listener_OnAction;
            bot.connection.Listener.OnPublic += Listener_OnPublic;
            bot.connection.Listener.OnPrivate += Listener_OnPrivate;
            bot.connection.Listener.OnError += Listener_OnError;
            bot.connection.Listener.OnQuit += Listener_OnQuit;
            bot.connection.Listener.OnJoin += Listener_OnJoin;
            bot.connection.Listener.OnPart += Listener_OnPart;
            bot.connection.Listener.OnDisconnected += Listener_OnDisconnected;
            bot.connection.Listener.OnChannelModeChange += Listener_OnChannelModeChange;
            bot.connection.Listener.OnNames += Listener_OnNames;
            bot.connection.Listener.OnKick += Listener_OnKick;
            bot.connection.Listener.OnKill += Listener_OnKill;
            bot.connection.Listener.OnPrivateNotice += Listener_OnPrivateNotice;
        }

        /// <summary> Unhooks IRC events so they are no longer handled. </summary>
        public void Unhook() {
            if (!hookedEvents) return;
            hookedEvents = false;
            userMap.Clear();
            
            OnPlayerActionEvent.Unregister(HandlePlayerAction);
            OnShuttingDownEvent.Unregister(HandleShutdown);
            
            // Regster events for incoming
            bot.connection.Listener.OnNick -= Listener_OnNick;
            bot.connection.Listener.OnRegistered -= Listener_OnRegistered;
            bot.connection.Listener.OnAction -= Listener_OnAction;
            bot.connection.Listener.OnPublic -= Listener_OnPublic;
            bot.connection.Listener.OnPrivate -= Listener_OnPrivate;
            bot.connection.Listener.OnError -= Listener_OnError;
            bot.connection.Listener.OnQuit -= Listener_OnQuit;
            bot.connection.Listener.OnJoin -= Listener_OnJoin;
            bot.connection.Listener.OnPart -= Listener_OnPart;
            bot.connection.Listener.OnDisconnected -= Listener_OnDisconnected;
            bot.connection.Listener.OnChannelModeChange -= Listener_OnChannelModeChange;
            bot.connection.Listener.OnNames -= Listener_OnNames;
            bot.connection.Listener.OnKick -= Listener_OnKick;
            bot.connection.Listener.OnKill -= Listener_OnKill;
            bot.connection.Listener.OnPrivateNotice -= Listener_OnPrivateNotice;
        }

        
        void HandlePlayerAction(Player p, PlayerAction action, string message, bool stealth) {
            if (action  != PlayerAction.Me) return;
            if (p.level != null && !p.level.SeesServerWideChat) return;
            bot.Say("*" + p.DisplayName + " " + message, stealth);
        }
        
        void HandleShutdown(bool restarting, string message) {
            bot.Disconnect(restarting ? "Server is restarting." : "Server is shutting down.");
        }
        
        
        void Listener_OnAction(UserInfo user, string channel, string description) {
            MessageInGame(user.Nick, string.Format("&I(IRC) * {0} {1}", user.Nick, description));
        }
        
        void Listener_OnJoin(UserInfo user, string channel) {
            bot.connection.Sender.Names(channel);
            DoJoinLeaveMessage(user.Nick, "joined", channel);
        }
        
        void Listener_OnPart(UserInfo user, string channel, string reason) {
            List<string> chanNicks = GetNicks(channel);
            RemoveNick(user.Nick, chanNicks);
            if (user.Nick == bot.nick) return;
            DoJoinLeaveMessage(user.Nick, "left", channel);
        }

        void DoJoinLeaveMessage(string nick, string verb, string channel) {
            Logger.Log(LogType.RelayActivity, "{0} {1} channel {2}", nick, verb, channel);
            string which = bot.opchannels.CaselessContains(channel) ? " operator" : "";
            MessageInGame(nick, string.Format("&I(IRC) {0} {1} the{2} channel", nick, verb, which));
        }

        void Listener_OnQuit(UserInfo user, string reason) {
            // Old bot was disconnected, try to reclaim it.
            if (user.Nick == bot.nick) bot.connection.Sender.Nick(bot.nick);
            
            foreach (var chans in userMap) {
                RemoveNick(user.Nick, chans.Value);
            }
            
            if (user.Nick == bot.nick) return;
            Logger.Log(LogType.RelayActivity, user.Nick + " left IRC");
            MessageInGame(user.Nick, "&I(IRC) " + user.Nick + " left");
        }

        void Listener_OnError(ReplyCode code, string message) {
            Logger.Log(LogType.RelayActivity, "IRC Error: " + message);
        }

        void Listener_OnPrivate(UserInfo user, string message) {
            message = IRCBot.ParseMessage(message);
            bot.HandlePrivate(user.Nick, message);
        }        

        void Listener_OnPublic(UserInfo user, string channel, string message) {
            message = message.TrimEnd();
            if (message.Length == 0) return;
            bool opchat = bot.opchannels.CaselessContains(channel);
            
            message = IRCBot.ParseMessage(message);
            bot.HandlePublic(user.Nick, channel, message, opchat);
        }

        public bool CheckIRCCommand(string nick, string cmdName, out string error) {
            error = null;
            if (!Server.ircControllers.Contains(nick)) return false;
            
            bool foundAtAll = false;
            foreach (string chan in bot.channels) {
                if (VerifyNick(chan, nick, ref error, ref foundAtAll)) return true;
            }
            foreach (string chan in bot.opchannels) {
                if (VerifyNick(chan, nick, ref error, ref foundAtAll)) return true;
            }
            
            if (!foundAtAll) {
                error = "You are not on the bot's list of users for some reason, please leave and rejoin."; return false;
            }
            if (bot.BannedCommands.CaselessContains(cmdName)) {
                error = "You are not allowed to use this command from IRC.";
            }
            return false;
        }
        
        
        void Listener_OnRegistered() {
            Logger.Log(LogType.RelayActivity, "Connected to IRC!");
            bot.resetting = false;
            bot.retries = 0;
            
            Authenticate();
            Logger.Log(LogType.RelayActivity, "Joining channels...");
            JoinChannels();
        }
        
        void JoinChannels() {
            foreach (string chan in bot.channels) {
                bot.Join(chan);
            }
            foreach (string chan in bot.opchannels) {
                bot.Join(chan);
            }
        }
        
        void Listener_OnPrivateNotice(UserInfo user, string notice) {
            if (!notice.CaselessStarts("You are now identified")) return;
            Logger.Log(LogType.RelayActivity, "Joining channels...");
            JoinChannels();
        }
        
        void Authenticate() {
            string nickServ = Server.Config.IRCNickServName;
            if (nickServ.Length == 0) return;
            
            if (Server.Config.IRCIdentify && Server.Config.IRCPassword.Length > 0) {
                Logger.Log(LogType.RelayActivity, "Identifying with " + nickServ);
                bot.connection.Sender.PrivateMessage(nickServ, "IDENTIFY " + Server.Config.IRCPassword);
            }
        }

        void Listener_OnDisconnected() {
            if (!bot.resetting && bot.retries < 3) { bot.retries++; bot.Connect(); }
        }

        void Listener_OnNick(UserInfo user, string newNick) {
            //Chat.MessageGlobal(Server.IRCColor + "(IRC) " + user.Nick + " changed nick to " + newNick);
            // We have successfully reclaimed our nick, so try to sign in again.
            if (newNick == bot.nick) Authenticate();

            if (newNick.Trim().Length == 0) return;
            
            foreach (var chans in userMap) {
                int index = GetNickIndex(user.Nick, chans.Value);
                if (index >= 0) {
                    string prefix = GetPrefix(chans.Value[index]);
                    chans.Value[index] = prefix + newNick;
                } else {
                    // should never happen, but just in case.
                    bot.connection.Sender.Names(chans.Key);
                }
            }

            MessageInGame(user.Nick, "&I(IRC) " + user.Nick + " &Sis now known as &I" + newNick);
        }
        
        void Listener_OnNames(string channel, string[] nicks, bool last) {
            List<string> chanNicks = GetNicks(channel);
            foreach (string n in nicks)
                UpdateNick(n, chanNicks);
        }
        
        void Listener_OnChannelModeChange(UserInfo who, string channel) {
            bot.connection.Sender.Names(channel);
        }
        
        void Listener_OnKick(UserInfo user, string channel, string kickee, string reason) {
            List<string> chanNicks = GetNicks(channel);
            RemoveNick(user.Nick, chanNicks);
            
            if (reason.Length > 0) reason = " (" + reason + ")";
            Logger.Log(LogType.RelayActivity, "{0} kicked {1} from IRC{2}", user.Nick, kickee, user.Nick);
            MessageInGame(user.Nick, "&I(IRC) " + user.Nick + " kicked " + kickee + reason);
        }
        
        void Listener_OnKill(UserInfo user, string nick, string reason) {
            foreach (var chans in userMap)
                RemoveNick(user.Nick, chans.Value);
        }
        
        List<string> GetNicks(string channel) {
            foreach (var chan in userMap) {
                if (chan.Key.CaselessEq(channel)) return chan.Value;
            }
            
            List<string> nicks = new List<string>();
            userMap[channel] = nicks;
            return nicks;
        }
        
        void UpdateNick(string n, List<string> chanNicks) {
            string unprefixNick = Unprefix(n);
            for (int i = 0; i < chanNicks.Count; i++ ) {
                if (unprefixNick == Unprefix(chanNicks[i])) {
                    chanNicks[i] = n; return;
                }
            }
            chanNicks.Add(n);
        }
        
        void RemoveNick(string n, List<string> chanNicks) {
            int index = GetNickIndex(n, chanNicks);
            if (index >= 0) chanNicks.RemoveAt(index);
        }
        
        int GetNickIndex(string n, List<string> chanNicks) {
            if (chanNicks == null) return -1;
            string unprefixNick = Unprefix(n);
            
            for (int i = 0; i < chanNicks.Count; i++ ) {
                if (unprefixNick == Unprefix(chanNicks[i]))
                    return i;
            }
            return -1;
        }
        
        string Unprefix(string nick) {
            return nick.Substring(GetPrefixLength(nick));
        }
        
        string GetPrefix(string nick) {
            return nick.Substring(0, GetPrefixLength(nick));
        }
        
        int GetPrefixLength(string nick) {
            int prefixChars = 0;
            for (int i = 0; i < nick.Length; i++) {
                if (!IsNickChar(nick[i]))
                    prefixChars++;
                else
                    return prefixChars;
            }
            return prefixChars;
        }
        
        bool IsNickChar(char c) {
            return (c >= '0' && c <= '9') || (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') ||
                c == '[' || c == ']' || c == '{' || c == '}' || c == '^' || c == '`' || c == '_' || c == '|';
        }
        
        bool VerifyNick(string channel, string userNick, ref string error, ref bool foundAtAll) {
            List<string> chanNicks = GetNicks(channel);
            if (chanNicks.Count == 0) return false;
            
            int index = GetNickIndex(userNick, chanNicks);
            if (index == -1) return false;
            foundAtAll = true;
            
            IRCControllerVerify verify = Server.Config.IRCVerify;
            if (verify == IRCControllerVerify.None) return true;
            
            if (verify == IRCControllerVerify.HalfOp) {
                string prefix = GetPrefix(chanNicks[index]);
                if (prefix.Length == 0 || prefix == "+") {
                    error = "You must be at least a half-op on the channel to use commands from IRC."; return false;
                }
                return true;
            } else {
                foreach (string chan in bot.opchannels) {
                    chanNicks = GetNicks(chan);
                    if (chanNicks.Count == 0) continue;
                    
                    index = GetNickIndex(userNick, chanNicks);
                    if (index != -1) return true;
                }
                error = "You must have joined the opchannel to use commands from IRC."; return false;
            }
        }
    }
}
