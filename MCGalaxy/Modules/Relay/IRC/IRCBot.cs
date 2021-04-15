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
using System.Text.RegularExpressions;
using MCGalaxy.Modules.Relay;
using MCGalaxy.Network;
using Sharkbite.Irc;

namespace MCGalaxy {
    
    public enum IRCControllerVerify { None, HalfOp, OpChannel };
    
    /// <summary> Manages a connection to an IRC server, and handles associated events. </summary>
    public sealed class IRCBot : RelayBot {
        internal Connection connection;
        internal string[] channels, opchannels;
        internal string nick, server;
        internal bool resetting;
        internal byte retries;
        IRCHandlers handlers;
        ConnectionArgs args;
        
        public override string RelayName { get { return "IRC"; } }
        
        public IRCBot() {
            handlers = new IRCHandlers(this);
            SetDefaultBannedCommands();
            UpdateState();
            InitConnectionState();
        }
        

        /// <summary> Sends an IRC message to either the normal or operator IRC channel. </summary>
        public void Say(string message, bool opchat) {
            string[] chans = opchat ? opchannels : channels;
            foreach (string chan in chans) { Message(chan, message); }
        }
        
        /// <summary> Sends an IRC private message to the given user. </summary>
        public void Pm(string user, string message) {
            if (!Enabled) return;
            message = ConvertMessage(message);
            connection.Sender.PrivateMessage(user, message);
        }
        
        public void Message(string channel, string message) {
            if (!Enabled) return;
            message = ConvertMessage(message);
            connection.Sender.PublicMessage(channel, message);
        }

        public void Join(string channel) {
            if (String.IsNullOrEmpty(channel)) return;
            connection.Sender.Join(channel);
        }
        
        public void Raw(string message) {
            if (!Enabled) return;
            connection.Sender.Raw(message);
        }
        
        
        public void Connect() {
            if (!Server.Config.UseIRC || Connected || Server.shuttingDown) return;
            InitConnectionState();
            if (!handlers.hookedEvents) HookEvents();
            handlers.Hook();
            
            Logger.Log(LogType.RelayActivity, "Connecting to IRC...");
            UpdateState();
            connection.connectionArgs = args;
            
            try {
                connection.Connect();
            } catch (Exception e) {
                Logger.Log(LogType.RelayActivity, "Failed to connect to IRC!");
                Logger.LogError(e);
            }
        }
        
        public void Disconnect(string reason) {
            if (!Connected) return;
            handlers.Unhook();
            UnhookEvents();
            
            connection.Disconnect(reason);
            Logger.Log(LogType.RelayActivity, "Disconnected from IRC!");
        }
        
        public void Reset() {
            resetting = true;
            retries = 0;
            Disconnect("IRC Bot resetting...");
            if (Server.Config.UseIRC) Connect();
        }
        
        /// <summary> Returns whether this bot is connected to IRC. </summary>
        public bool Connected { get { return connection != null && connection.Connected; } }        
        /// <summary> Returns whether this bot is connected to IRC and is able to send messages. </summary>
        public bool Enabled { get { return Server.Config.UseIRC && connection != null && connection.Connected; } }
        
        public override void SendPublicMessage(string message) { Say(message, false); }
        public override void SendStaffMessage(string message)  { Say(message, true); }       
        
        void InitConnectionState() {
            if (!Server.Config.UseIRC || connection != null) return;
            connection = new Connection(new UTF8Encoding(false), args);
            LoadBannedCommands();
        }
        
        void UpdateState() {
            channels = Server.Config.IRCChannels.SplitComma();
            opchannels = Server.Config.IRCOpChannels.SplitComma();
            nick = Server.Config.IRCNick.Replace(" ", "");
            server = Server.Config.IRCServer;
            
            args = new ConnectionArgs(nick, server);
            args.RealName = Server.SoftwareNameVersioned;
            args.Port     = Server.Config.IRCPort;
            args.UseSSL   = Server.Config.IRCSSL;
            bool usePass  = Server.Config.IRCIdentify && Server.Config.IRCPassword.Length > 0;
            args.ServerPassword = usePass ? Server.Config.IRCPassword : "*";
        }
        
        
        static readonly string[] ircColors = new string[] {
            "\u000300", "\u000301", "\u000302", "\u000303", "\u000304", "\u000305",
            "\u000306", "\u000307", "\u000308", "\u000309", "\u000310", "\u000311",
            "\u000312", "\u000313", "\u000314", "\u000315",
        };
        static readonly string[] ircSingle = new string[] {
            "\u00030", "\u00031", "\u00032", "\u00033", "\u00034", "\u00035",
            "\u00036", "\u00037", "\u00038", "\u00039",
        };
        static readonly string[] ircReplacements = new string[] {
            "&f", "&0", "&1", "&2", "&c", "&4", "&5", "&6",
            "&e", "&a", "&3", "&b", "&9", "&d", "&8", "&7",
        };
        static readonly Regex ircTwoColorCode = new Regex("(\x03\\d{1,2}),\\d{1,2}");
        
        public static string ParseMessage(string input) {
            // get rid of background color component of some IRC color codes.
            input = ircTwoColorCode.Replace(input, "$1");
            StringBuilder sb = new StringBuilder(input);
            
            for (int i = 0; i < ircColors.Length; i++) {
                sb.Replace(ircColors[i], ircReplacements[i]);
            }
            for (int i = 0; i < ircSingle.Length; i++) {
                sb.Replace(ircSingle[i], ircReplacements[i]);
            }
            
            // simplify fancy quotes
            sb.Replace("“", "\"");
            sb.Replace("”", "\"");
            sb.Replace("‘", "'");
            sb.Replace("’", "'"); 
            
            // remove misc formatting chars
            sb.Replace("\x02", ""); // bold
            sb.Replace("\x1D", ""); // italic
            sb.Replace("\x1F", ""); // underline
            
            sb.Replace("\x03", "&f"); // color reset
            sb.Replace("\x0f", "&f"); // reset
            return sb.ToString();
        }
        
        public static string ConvertMessage(string message) {
            if (String.IsNullOrEmpty(message.Trim())) message = ".";
            const string resetSignal = "\x03\x0F";
            
            message = EmotesHandler.Replace(message);
            message = ChatTokens.ApplyCustom(message);
            message = message.Replace("%S", "&f"); // TODO remove
            message = message.Replace("&S", "&f");
            message = message.Replace("&f", resetSignal);
            message = ToIRCColors(message);
            return message;
        }

        static string ToIRCColors(string input) {
            input = Colors.Escape(input);
            input = LineWrapper.CleanupColors(input, true, false);
            
            StringBuilder sb = new StringBuilder(input);
            for (int i = 0; i < ircColors.Length; i++) {
                sb.Replace(ircReplacements[i], ircColors[i]);
            }
            return sb.ToString();
        }
        
      
        protected override bool CanUseCommands(RelayUser user, string cmdName, out string error) {
            return handlers.CheckIRCCommand(user.Nick, cmdName, out error);
        }
        
        public override void MessageChannel(string channel, string message) {
            Message(channel, message);
        }
        
        public override void MessageUser(RelayUser user, string message) {
            Pm(user.Nick, message);
        }
        
        // TODO remove
        public void HandlePublic(string nick, string channel, string message, bool opchat) {
            RelayUser user = new RelayUser();
            user.Nick      = nick;
            HandleChannelMessage(user, channel, message, opchat);
        }
        
        public void HandlePrivate(string nick, string message) {
            RelayUser user = new RelayUser();
            user.Nick      = nick;
            HandleUserMessage(user, message);
        }
    }
}
