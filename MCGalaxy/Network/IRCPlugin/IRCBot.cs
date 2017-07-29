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
using MCGalaxy.Network;
using Sharkbite.Irc;

namespace MCGalaxy {
    
    public enum IRCControllerVerify { None, HalfOp, OpChannel };
    
    /// <summary> Manages a connection to an IRC server, and handles associated events. </summary>
    public sealed class IRCBot {
        public const string ResetSignal = "\x0F\x03";
        internal Connection connection;
        internal string[] channels, opchannels;
        internal string nick, server;
        internal bool reset = false;
        internal byte retries = 0;
        IRCHandlers handlers;
        ConnectionArgs args;
        
        public IRCBot() {
            handlers = new IRCHandlers(this);
            SetDefaultBannedCommands();
            UpdateState();
            InitConnectionState();
        }
        
        
        /// <summary> List of commands that cannot be used by IRC controllers. </summary>
        public List<string> BannedCommands;
        

        /// <summary> Sends an IRC message to either the normal or operator IRC channel. </summary>
        public void Say(string message, bool opchat = false, bool color = true) {
            string[] chans = opchat ? opchannels : channels;
            foreach (string chan in chans) {
                Message(chan, message, color);
            }
        }
        
        /// <summary> Sends an IRC private message to the given user. </summary>
        public void Pm(string user, string message, bool color = true) {
            if (!Enabled) return;
            message = ConvertMessage(message, color);
            connection.Sender.PrivateMessage(user, message);
        }
        
        /// <summary> Sends an IRC message to the given channel. </summary>
        public void Message(string channel, string message, bool color = true) {
            if (!Enabled) return;
            message = ConvertMessage(message, color);
            connection.Sender.PublicMessage(channel, message);
        }

        /// <summary> Attempts to join the given IRC channel. </summary>        
        public void Join(string channel) {
            if (String.IsNullOrEmpty(channel)) return;
            connection.Sender.Join(channel);
        }
        
        
        /// <summary> Disconnects this bot from IRC, then reconnects to IRC if IRC is enabled. </summary>
        public void Reset() {
            reset = true;
            retries = 0;
            Disconnect("IRC Bot resetting...");
            if (!ServerConfig.UseIRC) return;
            Connect();
        }
        
        /// <summary> Connects this bot to IRC, if IRC is enabled. </summary>
        public void Connect() {
            if (!ServerConfig.UseIRC || Connected || Server.shuttingDown) return;
            InitConnectionState();
            handlers.Hook();
            
            Logger.Log(LogType.IRCCActivity, "Connecting to IRC...");
            UpdateState();
            connection.connectionArgs = args;
            
            try {
                connection.Connect();
            } catch (Exception e) {
                Logger.Log(LogType.IRCCActivity, "Failed to connect to IRC!");
                Logger.LogError(e);
            }
        }
        
        /// <summary> Disconnects this bot from IRC. </summary>
        public void Disconnect(string reason) {
            if (!Connected) return;
            handlers.Unhook();
            
            connection.Disconnect(reason);
            Logger.Log(LogType.IRCCActivity, "Disconnected from IRC!");
        }
        
        /// <summary> Returns whether this bot is connected to IRC. </summary>
        public bool Connected { get { return connection != null && connection.Connected; } }
        
        /// <summary> Returns whether this bot is connected to IRC and is able to send messages. </summary>
        public bool Enabled { get { return ServerConfig.UseIRC && connection != null && connection.Connected; } }
        
        
        void InitConnectionState() {
            if (!ServerConfig.UseIRC || connection != null) return;
            connection = new Connection(new UTF8Encoding(false), args);
            LoadBannedCommands();
        }
        
        public static string ConvertMessage(string message, bool color) {
            if (String.IsNullOrEmpty(message.Trim()))
                message = ".";
            message = EmotesHandler.Replace(message);
            message = FullCP437Handler.Replace(message);
            message = ChatTokens.ApplyCustom(message);
            
            if (color)
                message = Colors.ConvertMCToIRC(message.Replace("%S", ResetSignal));
            return message;
        }
        
        void UpdateState() {
            channels = GetChannels(ServerConfig.IRCChannels);
            opchannels = GetChannels(ServerConfig.IRCOpChannels);
            nick = ServerConfig.IRCNick.Replace(" ", "");
            server = ServerConfig.IRCServer;
            
            args = new ConnectionArgs(nick, server);
            args.RealName = Server.SoftwareNameVersioned;
            args.Port = ServerConfig.IRCPort;
            args.ServerPassword = ServerConfig.IRCIdentify && ServerConfig.IRCPassword != "" ? ServerConfig.IRCPassword : "*";
        }
        
        static string[] GetChannels(string names) {
            names = names.Trim().Replace(" ", "");
            if (names.Length == 0) return new string[0];
            return names.Split(',');
        }
        
        void SetDefaultBannedCommands() {
            BannedCommands = new List<string>() { "resetbot", "resetirc", "oprules", "irccontrollers", "ircctrl" };
        }
        
        void LoadBannedCommands() {
            SetDefaultBannedCommands();
            
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
                    File.WriteAllLines("text/irccmdblacklist.txt", new string[] {
                                           "#Here you can put commands that cannot be used from the IRC bot.",
                                           "#Lines starting with \"#\" are ignored." });
                foreach (string line in File.ReadAllLines("text/irccmdblacklist.txt")) {
                    if (line.Length > 0 && line[0] != '#') BannedCommands.Add(line);
                }
            }
        }
    }
}
