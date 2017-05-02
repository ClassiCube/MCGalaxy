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
using MCGalaxy.DB;
using Sharkbite.Irc;

namespace MCGalaxy.Network {
    
    /// <summary> Handles messages and other events from IRC.</summary>
    public sealed class IRCHandlers {
        
        readonly IRCBot bot;
        public IRCHandlers(IRCBot bot) { this.bot = bot; }
        
        volatile bool hookedEvents = false;
        Dictionary<string, List<string>> userMap = new Dictionary<string, List<string>>();
        DateTime lastWho, lastOpWho;
        static char[] trimChars = new char[] { ' ' };
        
        /// <summary> Hooks IRC events so they are handled. </summary>
        public void Hook() {
            if (hookedEvents) return;
            hookedEvents = true;
            
            // Register events for outgoing
            Player.PlayerChat += Player_PlayerChat;
            Player.PlayerConnect += Player_PlayerConnect;
            Player.PlayerDisconnect += Player_PlayerDisconnect;
            Player.DoPlayerAction += Player_PlayerAction;

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

            // Register events for outgoing
            Player.PlayerChat -= Player_PlayerChat;
            Player.PlayerConnect -= Player_PlayerConnect;
            Player.PlayerDisconnect -= Player_PlayerDisconnect;
            Player.DoPlayerAction -= Player_PlayerAction;
            
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
        

        #region In-game event handlers
        
        void Player_PlayerAction(Player p, PlayerAction action,
                                 string message, bool stealth) {
            if (!bot.Enabled) return;
            string msg = null;
            
            if (action == PlayerAction.AFK && !p.hidden)
                msg = p.ColoredName + " %Sis AFK " + message;
            else if (action == PlayerAction.UnAFK && !p.hidden)
                msg = p.ColoredName + " %Sis no longer AFK";
            else if (action == PlayerAction.Joker)
                msg = p.ColoredName + " %Sis now a &aJ&bo&ck&5e&9r%S";
            else if (action == PlayerAction.Unjoker)
                msg = p.ColoredName + " %Sis no longer a &aJ&bo&ck&5e&9r%S";
            else if (action == PlayerAction.Me)
                msg = "*" + p.DisplayName + " " + message;
            else if (action == PlayerAction.Review)
                msg = p.ColoredName + " %Sis requesting a review.";
            else if (action == PlayerAction.JoinWorld && Server.ircShowWorldChanges && !p.hidden)
                msg = p.ColoredName + " %Swent to &8" + message;
            
            if (msg != null) bot.Say(msg, stealth);
        }
        
        void Player_PlayerDisconnect(Player p, string reason) {
            if (!bot.Enabled || p.hidden) return;
            if (!Server.guestLeaveNotify && p.Rank <= LevelPermission.Guest) return;
            
            bot.Say(p.DisplayName + " %Sleft the game (" + reason + "%S)", false);
        }

        void Player_PlayerConnect(Player p) {
            if (!bot.Enabled || p.hidden) return;
            if (!Server.guestJoinNotify && p.Rank <= LevelPermission.Guest) return;
            
            bot.Say(p.DisplayName + " %Sjoined the game", false);
        }
        
        void Player_PlayerChat(Player p, string message) {
            if (!bot.Enabled) return;
            if (message.Trim(trimChars) == "") return;
            
            string name = Server.ircPlayerTitles ? p.FullName : p.group.prefix + p.ColoredName;
            bot.Say(name + "%S: " + message, p.opchat);
        }
        #endregion
        
        
        void Listener_OnAction(UserInfo user, string channel, string description) {
            Player.GlobalIRCMessage(String.Format("%I(IRC) * {0} {1}", user.Nick, description));
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

        void DoJoinLeaveMessage(string who, string verb, string channel) {
            Server.s.Log(String.Format("{0} {1} channel {2}", who, verb, channel));
            string which = channel.CaselessEq(bot.opchannel) ? " operator" : "";
            Player.GlobalIRCMessage(String.Format("%I(IRC) {0} {1} the{2} channel", who, verb, which));
        }

        void Listener_OnQuit(UserInfo user, string reason) {
            // Old bot was disconnected, try to reclaim it.
            if (user.Nick == bot.nick) bot.connection.Sender.Nick(bot.nick);
            
            foreach (var chans in userMap) {
                RemoveNick(user.Nick, chans.Value);
            }
            
            if (user.Nick == bot.nick) return;
            Server.s.Log(user.Nick + " left IRC");
            Player.GlobalIRCMessage("%I(IRC) " + user.Nick + " left");
        }

        void Listener_OnError(ReplyCode code, string message) {
            Server.s.Log("IRC Error: " + message);
        }

        void Listener_OnPrivate(UserInfo user, string message) {
            message = Colors.IrcToMinecraftColors(message);
            string[] parts = message.SplitSpaces(2);
            string cmdName = parts[0].ToLower();
            string cmdArgs = parts.Length > 1 ? parts[1] : "";
            
            if (HandleWhoCommand(user, null, cmdName, false)) return;
            Command.Search(ref cmdName, ref cmdArgs);
            
            string error;
            string chan = String.IsNullOrEmpty(bot.channel) ? bot.opchannel : bot.channel;
            if (!CheckIRCCommand(user, cmdName, chan, out error)) {
                if (error != null) bot.Pm(user.Nick, error);
                return;
            }
            
            HandleIRCCommand(user, null, cmdName, cmdArgs);
        }
        

        void Listener_OnPublic(UserInfo user, string channel, string message) {
            message = message.TrimEnd();
            if (message.Length == 0) return;
            bool opchat = channel.CaselessEq(bot.opchannel);
            
            message = Colors.IrcToMinecraftColors(message);
            string[] parts = message.SplitSpaces(3);
            string ircCmd = parts[0].ToLower();
            if (HandleWhoCommand(user, channel, ircCmd, opchat)) return;
            
            if (ircCmd == Server.ircCommandPrefix && !HandleChannelcommand(user, channel, message, parts)) return;

            if (channel.CaselessEq(bot.opchannel)) {
                Server.s.Log(String.Format("(OPs): (IRC) {0}: {1}", user.Nick, message));
                Chat.MessageOps(String.Format("To Ops &f-%I(IRC) {0}&f- {1}", user.Nick,
                                              Server.profanityFilter ? ProfanityFilter.Parse(message) : message));
            } else {
                Server.s.Log(String.Format("(IRC) {0}: {1}", user.Nick, message));
                Player.GlobalIRCMessage(String.Format("%I(IRC) {0}: &f{1}", user.Nick,
                                                      Server.profanityFilter ? ProfanityFilter.Parse(message) : message));
            }
        }
        
        bool HandleChannelcommand(UserInfo user, string channel, string message, string[] parts) {
            string cmdName = parts.Length > 1 ? parts[1].ToLower() : "";
            string cmdArgs = parts.Length > 2 ? parts[2] : "";
            Command.Search(ref cmdName, ref cmdArgs);
            
            string error;
            if (!CheckIRCCommand(user, cmdName, channel, out error)) {
            	if (error != null) bot.Message(error, channel);
                return false;
            }
            
            return HandleIRCCommand(user, channel, cmdName, cmdArgs);
        }
        
        bool HandleWhoCommand(UserInfo user, string channel, string ircCmd, bool opchat) {
            bool whoCmd = ircCmd == ".who" || ircCmd == ".players" || ircCmd == "!players";
            DateTime last = opchat ? lastOpWho : lastWho;
            if (!whoCmd || (DateTime.UtcNow - last).TotalSeconds <= 5) return false;
            
            try {
                Player p = MakeIRCPlayer(user.Nick, channel);
                CmdPlayers.DisplayPlayers(p, "", false, false);
            } catch (Exception e) {
                Server.ErrorLog(e);
            }
            
            if (opchat) lastOpWho = DateTime.UtcNow;
            else lastWho = DateTime.UtcNow;
            return true;
        }
        
        bool HandleIRCCommand(UserInfo user, string channel, string cmdName, string cmdArgs) {
            Command cmd = Command.all.Find(cmdName);
            Player p = MakeIRCPlayer(user.Nick, channel);
            if (cmd == null) { Player.Message(p, "Unknown command!"); return false; }

            string logCmd = cmdArgs == "" ? cmdName : cmdName + " " + cmdArgs;
            Server.s.Log("IRC Command: /" + logCmd + " (by " + user.Nick + ")");
            
            try {
                if (!p.group.CanExecute(cmd)) { cmd.MessageCannotUse(p); return false; }
                cmd.Use(p, cmdArgs);
            } catch (Exception ex) {
                Player.Message(p, "CMD Error: " + ex);
            }
            return true;
        }

        bool CheckIRCCommand(UserInfo user, string cmdName, string channel, out string error) {
            List<string> chanNicks;
            error = null;
            if (!Server.ircControllers.Contains(user.Nick))
                return false;
            if (!userMap.TryGetValue(channel, out chanNicks))
                return false;
            
            int index = GetNickIndex(user.Nick, chanNicks);
            if (index < 0) {
                error = "You are not on the bot's list of users for some reason, please leave and rejoin."; return false;
            }
            if (!VerifyNick(chanNicks[index], ref error)) return false;
            
            if (bot.BannedCommands.Contains(cmdName)) {
                error = "You are not allowed to use this command from IRC."; return false;
            }
            return true;
        }
        
        
        static Player MakeIRCPlayer(string ircNick, string ircChannel) {
            Player p = new Player("IRC");
            p.group = Group.findPerm(Server.ircControllerRank);
            if (p.group == null) p.group = Group.NobodyRank;
            
            p.ircChannel = ircChannel;
            p.ircNick = ircNick;
            p.color = "&a";
            
            if (ircNick != null)
                p.UserID = NameConverter.InvalidNameID("(IRC " + ircNick + ")");
            return p;
        }
        
        void Listener_OnRegistered() {
            Server.s.Log("Connected to IRC!");
            bot.reset = false;
            bot.retries = 0;
            
            Authenticate();
            Server.s.Log("Joining channels...");
            JoinChannels();
        }
        
        void JoinChannels() {
            bot.Join(bot.channel);
            bot.Join(bot.opchannel);
        }
        
        void Listener_OnPrivateNotice(UserInfo user, string notice) {
            if (!notice.CaselessStarts("You are now identified")) return;
            Server.s.Log("Joining channels...");
            JoinChannels();
        }
        
        void Authenticate() {
            if (Server.ircIdentify && Server.ircPassword != "") {
                Server.s.Log("Identifying with NickServ");
                bot.connection.Sender.PrivateMessage("NickServ", "IDENTIFY " + Server.ircPassword);
            }
        }

        void Listener_OnDisconnected() {
            if (!bot.reset && bot.retries < 3) { bot.retries++; bot.Connect(); }
        }

        void Listener_OnNick(UserInfo user, string newNick) {
            //Player.GlobalMessage(Server.IRCColour + "(IRC) " + user.Nick + " changed nick to " + newNick);
            // We have successfully reclaimed our nick, so try to sign in again.
            if (newNick == bot.nick) Authenticate();

            if (newNick.Trim() == "") return;
            
            foreach (var kvp in userMap) {
                int index = GetNickIndex(user.Nick, kvp.Value);
                if (index >= 0) {
                    string prefix = GetPrefix(kvp.Value[index]);
                    kvp.Value[index] = prefix + newNick;
                } else {
                    // should never happen, but just in case.
                    bot.connection.Sender.Names(kvp.Key);
                }
            }

            Player.GlobalIRCMessage("%I(IRC) " + user.Nick + " %Sis now known as %I" + newNick);
        }
        
        void Listener_OnNames(string channel, string[] nicks, bool last) {
            List<string> chanNicks = GetNicks(channel);
            foreach (string n in nicks)
                UpdateNick(n, chanNicks);
        }
        
        void Listener_OnChannelModeChange(UserInfo who, string channel, ChannelModeInfo[] modes) {
            bot.connection.Sender.Names(channel);
        }
        
        void Listener_OnKick(UserInfo user, string channel, string kickee, string reason) {
            List<string> chanNicks = GetNicks(channel);
            RemoveNick(user.Nick, chanNicks);
            
            if (reason != "") reason = " (" + reason + ")";
            Server.s.Log(user.Nick + " kicked " + kickee + " from IRC" + reason);
            Player.GlobalIRCMessage("%I(IRC) " + user.Nick + " kicked " + kickee + reason);
        }
        
        void Listener_OnKill(UserInfo user, string nick, string reason) {
            foreach (var chans in userMap)
                RemoveNick(user.Nick, chans.Value);
        }
        
        List<string> GetNicks(string channel) {
            List<string> chanNicks;
            if (!userMap.TryGetValue(channel, out chanNicks)) {
                chanNicks = new List<string>();
                userMap[channel] = chanNicks;
            }
            return chanNicks;
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
        
        bool VerifyNick(string nick, ref string error) {
            IRCControllerVerify verify = Server.IRCVerify;
            if (verify == IRCControllerVerify.None) return true;
            
            if (verify == IRCControllerVerify.HalfOp) {
                string prefix = GetPrefix(nick);
                if (prefix == "" || prefix == "+") {
                    error = "You must be at least a half-op on the channel to use commands from IRC."; return false;
                }
                return true;
            } else {
                List<string> chanNicks = null;
                userMap.TryGetValue(bot.opchannel, out chanNicks);
                int index = GetNickIndex(nick, chanNicks);
                if (index == -1) {
                    error = "You must have joined the opchannel to use commands from IRC."; return false;
                }
                return true;
            }
        }
    }
}
