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
    public sealed class ForgeBot {
        public static readonly string ResetSignal = "\x0F\x03";
        private Connection connection;
        private List<string> banCmd;
        private string channel, opchannel;
        private string nick;
        private string server;
        private bool reset = false;
        private byte retries = 0;
        public string usedCmd = "";
        Dictionary<string, List<string>> users = new Dictionary<string, List<string>>();
        static char[] trimChars = { ' ' };
        
        public ForgeBot(string channel, string opchannel, string nick, string server) {
            this.channel = channel.Trim(); this.opchannel = opchannel.Trim(); this.nick = nick.Replace(" ", ""); this.server = server;
            banCmd = new List<string>();
            banCmd.Add("resetbot");
            banCmd.Add("resetirc");
            banCmd.Add("oprules");
            banCmd.Add("irccontrollers");
            banCmd.Add("ircctrl");
            
            if (Server.irc) {

                ConnectionArgs con = new ConnectionArgs(nick, server);
                con.Port = Server.ircPort;
                connection = new Connection(new UTF8Encoding(false), con);

                // Regster events for outgoing
                Player.PlayerChat += new Player.OnPlayerChat(Player_PlayerChat);
                Player.PlayerConnect += new Player.OnPlayerConnect(Player_PlayerConnect);
                Player.PlayerDisconnect += new Player.OnPlayerDisconnect(Player_PlayerDisconnect);

                // Regster events for incoming
                connection.Listener.OnNick += new NickEventHandler(Listener_OnNick);
                connection.Listener.OnRegistered += new RegisteredEventHandler(Listener_OnRegistered);
                connection.Listener.OnPublic += new PublicMessageEventHandler(Listener_OnPublic);
                connection.Listener.OnPrivate += new PrivateMessageEventHandler(Listener_OnPrivate);
                connection.Listener.OnError += new ErrorMessageEventHandler(Listener_OnError);
                connection.Listener.OnQuit += new QuitEventHandler(Listener_OnQuit);
                connection.Listener.OnJoin += new JoinEventHandler(Listener_OnJoin);
                connection.Listener.OnPart += new PartEventHandler(Listener_OnPart);
                connection.Listener.OnDisconnected += new DisconnectedEventHandler(Listener_OnDisconnected);
                connection.Listener.OnChannelModeChange += new ChannelModeChangeEventHandler(Listener_OnChannelModeChange);
                connection.Listener.OnNames += new NamesEventHandler(Listener_OnNames);
                connection.Listener.OnKick += new KickEventHandler(Listener_OnKick);
                connection.Listener.OnKill += new KillEventHandler(Listener_OnKill);

                // Load banned commands list
                if (File.Exists("text/ircbancmd.txt")) { // Backwards compatibility            
                    using (StreamWriter sw = File.CreateText("text/irccmdblacklist.txt")) {
                        sw.WriteLine("#Here you can put commands that cannot be used from the IRC bot.");
                        sw.WriteLine("#Lines starting with \"#\" are ignored.");
                        foreach (string line in File.ReadAllLines("text/ircbancmd.txt"))
                            sw.WriteLine(line);
                    }
                    File.Delete("text/ircbancmd.txt");
                } else {
                    if (!File.Exists("text/irccmdblacklist.txt")) 
                        File.WriteAllLines("text/irccmdblacklist.txt", new String[] { "#Here you can put commands that cannot be used from the IRC bot.", "#Lines starting with \"#\" are ignored." });
                    foreach (string line in File.ReadAllLines("text/irccmdblacklist.txt"))
                        if (line[0] != '#') banCmd.Add(line);
                }
            }
        }

        public void Say(string message, bool opchat = false, bool color = true) {
            if (!Server.irc || !IsConnected()) return;
            message = ConvertMessage(message, color);
            connection.Sender.PublicMessage(opchat ? opchannel : channel, message);
        }
        
        public void Pm(string user, string message, bool color = true) {
            if (!Server.irc || !IsConnected()) return;
            message = ConvertMessage(message, color);
            connection.Sender.PrivateMessage(user, message);
        }
        
        static string ConvertMessage(string message, bool color) {
            if (String.IsNullOrEmpty(message.Trim()))
                message = ".";
            message = CP437Writer.ConvertToUnicode(message);
            if (color)
                message = Colors.MinecraftToIrcColors(message.Replace("%r", ResetSignal));
            return message;
        }
        
        public void Reset() {
            if (!Server.irc) return;
            reset = true;
            retries = 0;
            Disconnect("IRC Bot resetting...");
            Connect();
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
            Server.s.Log(String.Format("{0} has {1} channel {2}", who, verb, channel));
            Player.GlobalMessage(String.Format("%I[IRC] {0} has {1} the{2} channel", who, verb, (channel.ToLower() == opchannel.ToLower() ? " operator" : "")));
        }
        
        void Player_PlayerDisconnect(Player p, string reason) {
            if (!Server.irc || !IsConnected()) return;
            if (!Server.guestLeaveNotify && p.group.Permission <= LevelPermission.Guest) return;
            reason = Colors.MinecraftToIrcColors(reason);
            if (!p.hidden)
                connection.Sender.PublicMessage(channel, p.name + " left the game (" + reason + ")");
        }

        void Player_PlayerConnect(Player p) {
            if (!Server.irc || !IsConnected()) return;
            if (!Server.guestJoinNotify && p.group.Permission <= LevelPermission.Guest) return;
            if (!p.hidden)
                connection.Sender.PublicMessage(channel, p.name + " joined the game");
        }

        void Listener_OnQuit(UserInfo user, string reason) {
            List<string> chanNicks = GetNicks(channel);
            RemoveNick(user.Nick, chanNicks);
            if (user.Nick == nick) return;
            Server.s.Log(user.Nick + " has left IRC");
            Player.GlobalMessage("%I" + user.Nick + Server.DefaultColor + " has left IRC");
        }

        void Listener_OnError(ReplyCode code, string message) {
            Server.s.Log("IRC Error: " + message);
        }

        void Listener_OnPrivate(UserInfo user, string message) {
            message = Colors.IrcToMinecraftColors(message);
            message = CP437Reader.ConvertToRaw(message);
            string[] parts = message.Split(trimChars, 2);
            string ircCmd = parts[0].ToLower();
            if (ircCmd == ".who" || ircCmd == ".players") {
                try {
                    CmdPlayers.DisplayPlayers(null, "", text => Pm(user.Nick, text), false);
                } catch (Exception e) {
                    Server.ErrorLog(e);
                }
                return;
            }
            
            string error;
            if (!CheckUserAndCommand(user, ircCmd, message, out error)) {
                if (error != null) Pm(user.Nick, error);
                return;
            }

            Command cmd = Command.all.Find(ircCmd);
            if (cmd != null) {
                Server.s.Log("IRC Command: /" + message + " (by " + user.Nick + ")");
                usedCmd = user.Nick;
                string args = parts.Length > 1 ? parts[1] : "";
                try {
                    cmd.Use(new Player("IRC"), args);
                } catch (Exception e) {
                    Pm(user.Nick, "CMD Error: " + e.ToString());
                }
                usedCmd = "";
            }
            else
                Pm(user.Nick, "Unknown command!");
        }

        void Listener_OnPublic(UserInfo user, string channel, string message) {
            message = Colors.IrcToMinecraftColors(message);
            message = CP437Reader.ConvertToRaw(message);
            string[] parts = message.Split(trimChars, 3);
            string ircCmd = parts[0].ToLower();
            if (ircCmd == ".who" || ircCmd == ".players") {
                try {
                    CmdPlayers.DisplayPlayers(null, "", text => Say(text, false, true), false);
                } catch (Exception e) {
                    Server.ErrorLog(e);
                }
            }
            
            if (ircCmd == ".x") {
                string cmdName = parts.Length > 1 ? parts[1].ToLower() : "";
                string error;
                if (!CheckUserAndCommand(user, cmdName, message, out error)) {
                    if (error != null) Server.IRC.Say(error);
                    return;
                }

                Command cmd = Command.all.Find(cmdName);
                if (cmdName != "" && cmd != null) {
                    Server.s.Log("IRC Command: /" + message.Replace(".x ", "") + " (by " + user.Nick + ")");
                    usedCmd = "";
                    string args = parts.Length > 2 ? parts[2] : "";
                    try {
                        cmd.Use(new Player("IRC"), args);
                    } catch (Exception e) {
                        Server.IRC.Say("CMD Error: " + e.ToString());
                    }
                    usedCmd = "";
                } else {
                    Server.IRC.Say("Unknown command!");
                }
            }

            if (String.IsNullOrEmpty(message.Trim()))
                message = ".";

            if (channel.ToLower() == opchannel.ToLower()) {
                Server.s.Log(String.Format("(OPs): [IRC] {0}: {1}", user.Nick, message));
                Chat.GlobalMessageOps(String.Format("To Ops &f-%I[IRC] {0}&f- {1}", user.Nick, Server.profanityFilter ? ProfanityFilter.Parse(message) : message));
            } else {
                Server.s.Log(String.Format("[IRC] {0}: {1}", user.Nick, message));
                Player.GlobalMessage(String.Format("%I[IRC] {0}: &f{1}", user.Nick, Server.profanityFilter ? ProfanityFilter.Parse(message) : message));
            }
        }

        bool CheckUserAndCommand(UserInfo user, string cmdName, string message, out string error) {
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
        	string prefix = GetPrefix(chanNicks[index]);
        	if (prefix == "" || prefix == "+") {
        		error = "You must be at least a half-op on the channel to use commands from IRC."; return false;
        	}
        	
        	if (banCmd.Contains(cmdName)) {
        		error = "You are not allowed to use this command from IRC."; return false;
        	}
        	return true;
        }
        
        void Listener_OnRegistered() {
            Server.s.Log("Connected to IRC!");
            reset = false;
            retries = 0;
            if (Server.ircIdentify && Server.ircPassword != "") {
                Server.s.Log("Identifying with NickServ");
                connection.Sender.PrivateMessage("NickServ", "IDENTIFY " + Server.ircPassword);
            }

            Server.s.Log("Joining channels...");

            if (!String.IsNullOrEmpty(channel))
                connection.Sender.Join(channel);
            if (!String.IsNullOrEmpty(opchannel))
                connection.Sender.Join(opchannel);
        }

        void Listener_OnDisconnected() {
            if (!reset && retries < 3) { retries++; Connect(); }
        }

        void Listener_OnNick(UserInfo user, string newNick) {
            //Player.GlobalMessage(Server.IRCColour + "[IRC] " + user.Nick + " changed nick to " + newNick);

            if (newNick.Trim() == "") {
                this.Pm(user.Nick, "You cannot have that username");
                return;
            }
            
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
                            Player.GlobalMessage("[IRC] %I" + user.Nick + Server.DefaultColor + " is AFK"); Server.ircafkset.Add(user.Nick); break;
                        case "Away":
                            Player.GlobalMessage("[IRC] %I" + user.Nick + Server.DefaultColor + " is Away"); Server.ircafkset.Add(user.Nick); break;
                    }
                }
            }
            else if (Server.ircafkset.Contains(newNick)) {
                Player.GlobalMessage("[IRC] %I" + newNick + Server.DefaultColor + " is back");
                Server.ircafkset.Remove(newNick);
            }
            else
                Player.GlobalMessage("[IRC] %I" + user.Nick + Server.DefaultColor + " is now known as " + newNick);
        }
        
        void Player_PlayerChat(Player p, string message) {    
            if (String.IsNullOrEmpty(message.Trim())) {
                Player.SendMessage(p, "You cannot send that message");
                return;
            }

            if (Server.ircColorsEnable && Server.irc && IsConnected())
                Say(p.FullName + "%r: " + message, p.opchat);
            if (Server.ircColorsEnable == false && Server.irc && IsConnected())
            {
                Say(p.DisplayName + ": " + message, p.opchat);
            }
        }
        
        public void Connect() {
            if (!Server.irc || Server.shuttingDown) return;

            /*new Thread(new ThreadStart(delegate
            {
                try { connection.Connect(); }
                catch (Exception e)
                {
                    Server.s.Log("Failed to connect to IRC");
                    Server.ErrorLog(e);
                }
            })).Start();*/

            Server.s.Log("Connecting to IRC...");

            try { connection.Connect(); }
            catch (Exception e) {
                Server.s.Log("Failed to connect to IRC!");
                Server.ErrorLog(e);
            }
        }
        
        public void Disconnect(string reason) {
            if (IsConnected()) { 
                connection.Disconnect(reason); 
                Server.s.Log("Disconnected from IRC!");
                users.Clear();
            }
        }
        
        public bool IsConnected() {
            if (!Server.irc) return false;
            try { return connection.Connected; }
            catch { return false; }
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
        
        void RemoveNick(string n, List<string> chanNicks) {
            int index = GetNickIndex(n, chanNicks);
            if (index >= 0) chanNicks.RemoveAt(index);
        }
        
        int GetNickIndex(string n, List<string> chanNicks) {
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
    }
}
