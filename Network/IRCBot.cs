/*
    Copyright 2011 MCForge
        
    Dual-licensed under the    Educational Community License, Version 2.0 and
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
using MCGalaxy.Commands;
using Sharkbite.Irc;

namespace MCGalaxy {
    
    public enum IRCControllerVerify { None, HalfOp, OpChannel };
    
    public sealed class IRCBot {
        public const string ResetSignal = "\x0F\x03";
        Connection connection;
        List<string> banCmd;
        string channel, opchannel;
        string nick, server;
        bool reset = false;
        byte retries = 0;
        
        Dictionary<string, List<string>> users = new Dictionary<string, List<string>>();
        static char[] trimChars = { ' ' };
        ConnectionArgs args;
        DateTime lastWho, lastOpWho;
        
        public IRCBot() {
            UpdateState();
            InitConnectionState();
        }

        public void Say(string message, bool opchat = false, bool color = true) {
            if (!Server.irc ||!IsConnected()) return;
            message = ConvertMessage(message, color);
            
            string chan = opchat ? opchannel : channel;
            if (!String.IsNullOrEmpty(chan))
                connection.Sender.PublicMessage(chan, message);
        }
        
        public void Pm(string user, string message, bool color = true) {
            if (!Server.irc ||!IsConnected()) return;
            message = ConvertMessage(message, color);
            connection.Sender.PrivateMessage(user, message);
        }
        
        public void Reset() {
            reset = true;
            retries = 0;
            Disconnect("IRC Bot resetting...");
            if (!Server.irc) return;
            Connect();
        }
        
        void InitConnectionState() {
            if (!Server.irc || connection != null) return;
            connection = new Connection(new UTF8Encoding(false), args);
            LoadBannedCommands();
        }
        
        public void Connect() {
            if (!Server.irc || IsConnected() || Server.shuttingDown) return;
            InitConnectionState();
            if (!hookedEvents) HookEvents();
            
            Server.s.Log("Connecting to IRC...");
            UpdateState();
            connection.connectionArgs = args;
            
            try { 
                connection.Connect(); 
            } catch (Exception e) {
                Server.s.Log("Failed to connect to IRC!");
                Server.ErrorLog(e);
            }
        }
        
        public void Disconnect(string reason) {
            if (!IsConnected()) return;
            if (hookedEvents) UnhookEvents();
            
            connection.Disconnect(reason); 
            Server.s.Log("Disconnected from IRC!");
            users.Clear();
        }
        
        public bool IsConnected() { return connection != null && connection.Connected; }
        
        
        static string ConvertMessage(string message, bool color) {
            if (String.IsNullOrEmpty(message.Trim()))
                message = ".";
            message = EmotesHandler.Replace(message);
            message = FullCP437Handler.Replace(message);
            message = ChatTokens.ApplyCustom(message);
            message = CP437Writer.ConvertToUnicode(message);
            
            if (color)
                message = Colors.MinecraftToIrcColors(message.Replace("%S", ResetSignal));
            return message;
        }
        
        void UpdateState() {
            channel = Server.ircChannel.Trim();
            opchannel = Server.ircOpChannel.Trim();
            nick = Server.ircNick.Replace(" ", "");
            server = Server.ircServer;
            
            args = new ConnectionArgs(nick, server);
            args.Port = Server.ircPort;
            args.ServerPassword = Server.ircIdentify && Server.ircPassword != "" ? Server.ircPassword : "*";
        }
        
        void LoadBannedCommands() {
            banCmd = new List<string>() { "resetbot", "resetirc", "oprules", "irccontrollers", "ircctrl" };
            
            if (File.Exists("text/ircbancmd.txt")) { // Backwards compatibility
                using (StreamWriter w = new StreamWriter("text/irccmdblacklist.txt")) {
                    w.WriteLine("#Here you can put commands that cannot be used from the IRC bot.");
                    w.WriteLine("#Lines starting with \"#\" are ignored.");
                    foreach (string line in File.ReadAllLines("text/ircbancmd.txt"))
                        w.WriteLine(line);
                }
                File.Delete("text/ircbancmd.txt");
            } else {
                if (!File.Exists("text/irccmdblacklist.txt"))
                    File.WriteAllLines("text/irccmdblacklist.txt", new [] {
                                           "#Here you can put commands that cannot be used from the IRC bot.",
                                           "#Lines starting with \"#\" are ignored." });
                foreach (string line in File.ReadAllLines("text/irccmdblacklist.txt"))
                    if (line[0] != '#') banCmd.Add(line);
            }
        }
        
        
        #region In-game event handlers
        
        void Player_PlayerAction(Player p, PlayerAction action, 
                                      string message, bool stealth) {
            if (!Server.irc ||!IsConnected()) return;
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
            else if (action == PlayerAction.JoinWorld && Server.ircShowWorldChanges)
                msg = p.ColoredName + " %Swent to &8" + message;
            
            if (msg != null) Say(msg, stealth);
        }
        
        void Player_PlayerDisconnect(Player p, string reason) {
            if (!Server.irc ||!IsConnected() || p.hidden) return;
            if (!Server.guestLeaveNotify && p.Rank <= LevelPermission.Guest) return;
            Say(p.DisplayName + " %Sleft the game (" + reason + "%S)", false);
        }

        void Player_PlayerConnect(Player p) {
            if (!Server.irc ||!IsConnected() || p.hidden) return;
            if (!Server.guestJoinNotify && p.Rank <= LevelPermission.Guest) return;
            Say(p.DisplayName + " %Sjoined the game", false);
        }
        
        void Player_PlayerChat(Player p, string message) {
            if (!Server.irc ||!IsConnected()) return;
            if (String.IsNullOrEmpty(message.Trim(trimChars))) return;
            
            string name = Server.ircPlayerTitles ? p.FullName : p.group.prefix + p.ColoredName;
            Say(name + "%S: " + message, p.opchat);
        }        
        #endregion
        
        
        #region IRC event handlers
        
        void Listener_OnAction(UserInfo user, string channel, string description) {
            Player.GlobalIRCMessage(String.Format("%I[IRC] * {0} {1}", user.Nick, description));
        }
        
        void Listener_OnJoin(UserInfo user, string channel) {
            connection.Sender.Names(channel);
            DoJoinLeaveMessage(user.Nick, "joined", channel);
        }
        
        void Listener_OnPart(UserInfo user, string channel, string reason) {
            List<string> chanNicks = GetNicks(channel);
            RemoveNick(user.Nick, chanNicks);
            if (user.Nick == nick) return;
            DoJoinLeaveMessage(user.Nick, "left", channel);
        }

        void DoJoinLeaveMessage(string who, string verb, string channel) {
            Server.s.Log(String.Format("{0} {1} channel {2}", who, verb, channel));
            string which = channel.CaselessEq(opchannel) ? " operator" : "";
            Player.GlobalIRCMessage(String.Format("%I[IRC] {0} {1} the{2} channel", who, verb, which));
        }

        void Listener_OnQuit(UserInfo user, string reason) {
            // Old bot was disconnected, try to reclaim it.
            if (user.Nick == nick) connection.Sender.Nick(nick);
            
            RemoveNick(user.Nick);
            if (user.Nick == nick) return;
            Server.s.Log(user.Nick + " left IRC");
            Player.GlobalIRCMessage("%I[IRC] " + user.Nick + " left");
        }

        void Listener_OnError(ReplyCode code, string message) {
            Server.s.Log("IRC Error: " + message);
        }

        void Listener_OnPrivate(UserInfo user, string message) {
            message = Colors.IrcToMinecraftColors(message);
            message = CP437Reader.ConvertToRaw(message);
            string[] parts = message.SplitSpaces(2);
            string cmdName = parts[0].ToLower();
            string cmdArgs = parts.Length > 1 ? parts[1] : "";
            
            if (HandleWhoCommand(user.Nick, cmdName, false)) return;
            Command.Search(ref cmdName, ref cmdArgs);
            
            string error;
            string chan = String.IsNullOrEmpty(channel) ? opchannel : channel;
            if (!CheckIRCCommand(user, cmdName, chan, out error)) {
                if (error != null) Pm(user.Nick, error);
                return;
            }            
            HandleIRCCommand(user.Nick, user.Nick, cmdName, cmdArgs);
        }

        void Listener_OnPublic(UserInfo user, string channel, string message) {
            message = message.TrimEnd();
            if (message.Length == 0) return;
            bool opchat = channel == opchannel;
            
            message = Colors.IrcToMinecraftColors(message);
            message = CP437Reader.ConvertToRaw(message);
            string[] parts = message.SplitSpaces(3);
            string ircCmd = parts[0].ToLower();
            
            string nick = opchat ? "#@private@#" : "#@public@#";
            if (HandleWhoCommand(nick, ircCmd, opchat)) return;
            
            if (ircCmd == ".x" && !HandlePublicCommand(user, channel, message, parts, opchat)) return;

            if (channel.CaselessEq(opchannel)) {
                Server.s.Log(String.Format("(OPs): [IRC] {0}: {1}", user.Nick, message));
                Chat.MessageOps(String.Format("To Ops &f-%I[IRC] {0}&f- {1}", user.Nick, 
                                                    Server.profanityFilter ? ProfanityFilter.Parse(message) : message));
            } else {
                Server.s.Log(String.Format("[IRC] {0}: {1}", user.Nick, message));
                Player.GlobalIRCMessage(String.Format("%I[IRC] {0}: &f{1}", user.Nick, 
                                                      Server.profanityFilter ? ProfanityFilter.Parse(message) : message));
            }
        }
    
        bool HandlePublicCommand(UserInfo user, string channel, string message,
                                 string[] parts, bool opchat) {
            string cmdName = parts.Length > 1 ? parts[1].ToLower() : "";
            string cmdArgs = parts.Length > 2 ? parts[2] : "";
            Command.Search(ref cmdName, ref cmdArgs);
            
            string error;
            if (!CheckIRCCommand(user, cmdName, channel, out error)) {
                if (error != null) Say(error, opchat);
                return false;
            }
            
            string nick = opchat ? "#@private@#" : "#@public@#";
            return HandleIRCCommand(nick, user.Nick, cmdName, cmdArgs);
        }
        
        bool HandleWhoCommand(string nick, string ircCmd, bool opchat) {
            bool whoCmd = ircCmd == ".who" || ircCmd == ".players" || ircCmd == "!players";
            DateTime last = opchat ? lastOpWho : lastWho;
            if (!whoCmd || (DateTime.UtcNow - last).TotalSeconds <= 1) return false;
            
            try {
                Player p = MakeIRCPlayer(nick);
                CmdPlayers.DisplayPlayers(p, "", false, false);
            } catch (Exception e) {
                Server.ErrorLog(e);
            }
            
            if (opchat) lastOpWho = DateTime.UtcNow;
            else lastWho = DateTime.UtcNow;
            return true;
        }
        
        bool HandleIRCCommand(string nick, string logNick, string cmdName, string cmdArgs) {
            Command cmd = Command.all.Find(cmdName);
            Player p = MakeIRCPlayer(nick);
            if (cmd == null) { Player.Message(p, "Unknown command!"); return false; }    

            string logCmd = cmdArgs == "" ? cmdName : cmdName + " " + cmdArgs;
            Server.s.Log("IRC Command: /" + logCmd + " (by " + logNick + ")");
            
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
            if (!users.TryGetValue(channel, out chanNicks))
                return false;
            
            int index = GetNickIndex(user.Nick, chanNicks);
            if (index < 0) {
                error = "You are not on the bot's list of users for some reason, please leave and rejoin."; return false;
            }
            if (!VerifyNick(chanNicks[index], ref error)) return false;
            
            if (banCmd != null && banCmd.Contains(cmdName)) {
                error = "You are not allowed to use this command from IRC."; return false;
            }
            return true;
        }
        
        static Player MakeIRCPlayer(string ircNick) {
            Player p = new Player("IRC");
            p.group = Group.findPerm(Server.ircControllerRank);
            if (p.group == null)
                p.group = Group.findPerm(LevelPermission.Nobody);
            p.ircNick = ircNick;
            p.color = "&a"; return p;
        }
        
        void Listener_OnRegistered() {
            Server.s.Log("Connected to IRC!");
            reset = false;
            retries = 0;
            Authenticate();
            Server.s.Log("Joining channels...");
            JoinChannels();
        }
        
        void JoinChannels() {
            if (!String.IsNullOrEmpty(channel))
                connection.Sender.Join(channel);
            if (!String.IsNullOrEmpty(opchannel))
                connection.Sender.Join(opchannel);
        }
        
        void Listener_OnPrivateNotice(UserInfo user, string notice) {
            if (!notice.CaselessStarts("You are now identified")) return;
            Server.s.Log("Joining channels...");
            JoinChannels();
        }
        
        void Authenticate() {
            if (Server.ircIdentify && Server.ircPassword != "") {
                Server.s.Log("Identifying with NickServ");
                connection.Sender.PrivateMessage("NickServ", "IDENTIFY " + Server.ircPassword);
            }
        }

        void Listener_OnDisconnected() {
            if (!reset && retries < 3) { retries++; Connect(); }
        }

        void Listener_OnNick(UserInfo user, string newNick) {
            //Player.GlobalMessage(Server.IRCColour + "[IRC] " + user.Nick + " changed nick to " + newNick);
            // We have successfully reclaimed our nick, so try to sign in again.
            if (newNick == nick) Authenticate();

            if (newNick.Trim() == "") return;
            
            foreach (var kvp in users) {
                int index = GetNickIndex(user.Nick, kvp.Value);
                if (index >= 0) {
                    string prefix = GetPrefix(kvp.Value[index]);
                    kvp.Value[index] = prefix + newNick;
                } else {
                    // should never happen, but just in case.
                    connection.Sender.Names(kvp.Key);
                }
            }

            string key;
            if (newNick.Split('|').Length == 2) {
                key = newNick.Split('|')[1];
                if (key != null && key != "") {
                    switch (key) {
                        case "AFK":
                            Player.GlobalIRCMessage("%I[IRC] " + user.Nick + " %Sis AFK"); Server.ircafkset.Add(user.Nick); break;
                        case "Away":
                            Player.GlobalIRCMessage("%I[IRC] " + user.Nick + " %Sis Away"); Server.ircafkset.Add(user.Nick); break;
                    }
                }
            } else if (Server.ircafkset.Contains(newNick)) {
                Player.GlobalIRCMessage("%I[IRC] " + newNick + " %Sis back");
                Server.ircafkset.Remove(newNick);
            } else {
                Player.GlobalIRCMessage("%I[IRC] " + user.Nick + " %Sis now known as %I" + newNick);
            }
        }
        
         void Listener_OnNames(string channel, string[] nicks, bool last) {
            List<string> chanNicks = GetNicks(channel);
            foreach (string n in nicks)
                UpdateNick(n, chanNicks);
        }
        
        void Listener_OnChannelModeChange(UserInfo who, string channel, ChannelModeInfo[] modes) {
            connection.Sender.Names(channel);
        }
        
        void Listener_OnKick(UserInfo user, string channel, string kickee, string reason) {
            List<string> chanNicks = GetNicks(channel);
            RemoveNick(user.Nick, chanNicks);
            Server.s.Log(user.Nick + " kicked " + kickee + " from IRC");
            Player.GlobalIRCMessage("%I[IRC] " + user.Nick + " kicked " + kickee);           
        }
        
        void Listener_OnKill(UserInfo user, string nick, string reason) {
            foreach (var kvp in users)
                RemoveNick(user.Nick, kvp.Value);
        }
        
        List<string> GetNicks(string channel) {
            List<string> chanNicks;
            if (!users.TryGetValue(channel, out chanNicks)) {
                chanNicks = new List<string>();
                users[channel] = chanNicks;
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
        
        void RemoveNick(string nick) {
            if (!String.IsNullOrEmpty(channel)) {
                List<string> chanNicks = GetNicks(channel);
                RemoveNick(nick, chanNicks);
            }
            if (!String.IsNullOrEmpty(opchannel)) {
                List<string> chanNicks = GetNicks(opchannel);
                RemoveNick(nick, chanNicks);
            }
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
                users.TryGetValue(opchannel, out chanNicks);
                int index = GetNickIndex(nick, chanNicks);
                if (index == -1) {
                    error = "You must have joined the opchannel to use commands from IRC."; return false;
                }
                return true;
            }
        }
        
        #endregion
        
        
        #region Event hooks
        
        volatile bool hookedEvents = false;
        void HookEvents() {
            hookedEvents = true;
            // Register events for outgoing
            Player.PlayerChat += Player_PlayerChat;
            Player.PlayerConnect += Player_PlayerConnect;
            Player.PlayerDisconnect += Player_PlayerDisconnect;
            Player.DoPlayerAction += Player_PlayerAction;

            // Regster events for incoming
            connection.Listener.OnNick += Listener_OnNick;
            connection.Listener.OnRegistered += Listener_OnRegistered;
            connection.Listener.OnAction += Listener_OnAction;
            connection.Listener.OnPublic += Listener_OnPublic;
            connection.Listener.OnPrivate += Listener_OnPrivate;
            connection.Listener.OnError += Listener_OnError;
            connection.Listener.OnQuit += Listener_OnQuit;
            connection.Listener.OnJoin += Listener_OnJoin;
            connection.Listener.OnPart += Listener_OnPart;
            connection.Listener.OnDisconnected += Listener_OnDisconnected;
            connection.Listener.OnChannelModeChange += Listener_OnChannelModeChange;
            connection.Listener.OnNames += Listener_OnNames;
            connection.Listener.OnKick += Listener_OnKick;
            connection.Listener.OnKill += Listener_OnKill;
            connection.Listener.OnPrivateNotice += Listener_OnPrivateNotice;
        }
        
        void UnhookEvents() {
            hookedEvents = false;
             // Register events for outgoing
            Player.PlayerChat -= Player_PlayerChat;
            Player.PlayerConnect -= Player_PlayerConnect;
            Player.PlayerDisconnect -= Player_PlayerDisconnect;
            Player.DoPlayerAction -= Player_PlayerAction;

            // Regster events for incoming
            connection.Listener.OnNick -= Listener_OnNick;
            connection.Listener.OnRegistered -= Listener_OnRegistered;
            connection.Listener.OnAction -= Listener_OnAction;
            connection.Listener.OnPublic -= Listener_OnPublic;
            connection.Listener.OnPrivate -= Listener_OnPrivate;
            connection.Listener.OnError -= Listener_OnError;
            connection.Listener.OnQuit -= Listener_OnQuit;
            connection.Listener.OnJoin -= Listener_OnJoin;
            connection.Listener.OnPart -= Listener_OnPart;
            connection.Listener.OnDisconnected -= Listener_OnDisconnected;
            connection.Listener.OnChannelModeChange -= Listener_OnChannelModeChange;
            connection.Listener.OnNames -= Listener_OnNames;
            connection.Listener.OnKick -= Listener_OnKick;
            connection.Listener.OnKill -= Listener_OnKill;
            connection.Listener.OnPrivateNotice -= Listener_OnPrivateNotice;
        }
        #endregion
    }
}
