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
using System.Text;
using System.Text.RegularExpressions;
using MCGalaxy.Events.PlayerEvents;
using MCGalaxy.Events.ServerEvents;
using MCGalaxy.Modules.Relay;
using MCGalaxy.Network;
using Sharkbite.Irc;

namespace MCGalaxy {
    
    public enum IRCControllerVerify { None, HalfOp, OpChannel };
    
    /// <summary> Manages a connection to an IRC server, and handles associated events. </summary>
    public sealed class IRCBot : RelayBot {
        internal Connection connection;
        internal string nick, server;
        internal bool resetting;
        internal byte retries;
        ConnectionArgs args;
        IRCNickList nicks;
        
        public override string RelayName { get { return "IRC"; } }
        
        public IRCBot() {
            nicks     = new IRCNickList();
            nicks.bot = this;
            
            SetDefaultBannedCommands();
            UpdateState();
            InitConnectionState();
        }
        
        
        public override void MessageUser(RelayUser user, string message) {
            if (!Enabled) return;
            message = ConvertMessage(message);
            connection.Sender.PrivateMessage(user.Nick, message);
        }
        
        public override void MessageChannel(string channel, string message) {
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
            Hook();
            
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
            Unhook();
            nicks.Clear();
            
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
        
        void InitConnectionState() {
            if (!Server.Config.UseIRC || connection != null) return;
            connection = new Connection(new UTF8Encoding(false), args);
            LoadBannedCommands();
        }
        
        void UpdateState() {
            Channels   = Server.Config.IRCChannels.SplitComma();
            OpChannels = Server.Config.IRCOpChannels.SplitComma();
            nick   = Server.Config.IRCNick.Replace(" ", "");
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
            return CheckIRCCommand(user.Nick, cmdName, out error);
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


        volatile bool hookedEvents = false;
        
        /// <summary> Hooks IRC events so they are handled. </summary>
        void Hook() {
            if (hookedEvents) return;
            hookedEvents = true;
            HookEvents();
            OnShuttingDownEvent.Register(HandleShutdown, Priority.Low);

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

        /// <summary> Unhooks IRC events so they are no longer handled. </summary>
        void Unhook() {
            if (!hookedEvents) return;
            hookedEvents = false;
            UnhookEvents();
            
            OnShuttingDownEvent.Unregister(HandleShutdown);
            
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

        
        void HandleShutdown(bool restarting, string message) {
            Disconnect(restarting ? "Server is restarting." : "Server is shutting down.");
        }
        
        
        void Listener_OnAction(UserInfo user, string channel, string description) {
            MessageInGame(user.Nick, string.Format("&I(IRC) * {0} {1}", user.Nick, description));
        }
        
        void Listener_OnJoin(UserInfo user, string channel) {
            connection.Sender.Names(channel);
            DoJoinLeaveMessage(user.Nick, "joined", channel);
        }
        
        void Listener_OnPart(UserInfo user, string channel, string reason) {
            nicks.OnLeftChannel(user, channel);
            if (user.Nick == nick) return;
            DoJoinLeaveMessage(user.Nick, "left", channel);
        }

        void DoJoinLeaveMessage(string nick, string verb, string channel) {
            Logger.Log(LogType.RelayActivity, "{0} {1} channel {2}", nick, verb, channel);
            string which = OpChannels.CaselessContains(channel) ? " operator" : "";
            MessageInGame(nick, string.Format("&I(IRC) {0} {1} the{2} channel", nick, verb, which));
        }

        void Listener_OnQuit(UserInfo user, string reason) {
            // Old bot was disconnected, try to reclaim it
            if (user.Nick == nick) connection.Sender.Nick(nick);
            nicks.OnLeft(user);
            
            if (user.Nick == nick) return;
            Logger.Log(LogType.RelayActivity, user.Nick + " left IRC");
            MessageInGame(user.Nick, "&I(IRC) " + user.Nick + " left");
        }

        void Listener_OnError(ReplyCode code, string message) {
            Logger.Log(LogType.RelayActivity, "IRC Error: " + message);
        }

        void Listener_OnPrivate(UserInfo user, string message) {
            message = IRCBot.ParseMessage(message);
            HandlePrivate(user.Nick, message);
        }        

        void Listener_OnPublic(UserInfo user, string channel, string message) {
            message = message.TrimEnd();
            if (message.Length == 0) return;
            bool opchat = OpChannels.CaselessContains(channel);
            
            message = IRCBot.ParseMessage(message);
            HandlePublic(user.Nick, channel, message, opchat);
        }

        public bool CheckIRCCommand(string nick, string cmdName, out string error) {
            error = null;
            if (!Server.ircControllers.Contains(nick)) return false;
            
            bool foundAtAll = false;
            foreach (string chan in Channels) {
                if (nicks.VerifyNick(chan, nick, ref error, ref foundAtAll)) return true;
            }
            foreach (string chan in OpChannels) {
                if (nicks.VerifyNick(chan, nick, ref error, ref foundAtAll)) return true;
            }
            
            if (!foundAtAll) {
                error = "You are not on the bot's list of users for some reason, please leave and rejoin."; return false;
            }
            if (BannedCommands.CaselessContains(cmdName)) {
                error = "You are not allowed to use this command from IRC.";
            }
            return false;
        }
        
        
        void Listener_OnRegistered() {
            Logger.Log(LogType.RelayActivity, "Connected to IRC!");
            resetting = false;
            retries = 0;
            
            Authenticate();
            JoinChannels();
        }
        
        void JoinChannels() {
            Logger.Log(LogType.RelayActivity, "Joining IRC channels...");
            foreach (string chan in Channels)   { Join(chan); }
            foreach (string chan in OpChannels) { Join(chan); }
        }
        
        void Listener_OnPrivateNotice(UserInfo user, string notice) {
            if (!notice.CaselessStarts("You are now identified")) return;
            JoinChannels();
        }
        
        void Authenticate() {
            string nickServ = Server.Config.IRCNickServName;
            if (nickServ.Length == 0) return;
            
            if (Server.Config.IRCIdentify && Server.Config.IRCPassword.Length > 0) {
                Logger.Log(LogType.RelayActivity, "Identifying with " + nickServ);
                connection.Sender.PrivateMessage(nickServ, "IDENTIFY " + Server.Config.IRCPassword);
            }
        }

        void Listener_OnDisconnected() {
            if (!resetting && retries < 3) { retries++; Connect(); }
        }

        void Listener_OnNick(UserInfo user, string newNick) {
            //Chat.MessageGlobal(Server.IRCColor + "(IRC) " + user.Nick + " changed nick to " + newNick);
            // We have successfully reclaimed our nick, so try to sign in again.
            if (newNick == nick) Authenticate();
            if (newNick.Trim().Length == 0) return;
            
            nicks.OnChangedNick(user, newNick);
            MessageInGame(user.Nick, "&I(IRC) " + user.Nick + " &Sis now known as &I" + newNick);
        }
        
        void Listener_OnNames(string channel, string[] _nicks, bool last) {
            nicks.UpdateFor(channel, _nicks);
        }
        
        void Listener_OnChannelModeChange(UserInfo who, string channel) {
            connection.Sender.Names(channel);
        }
        
        void Listener_OnKick(UserInfo user, string channel, string kickee, string reason) {
            nicks.OnLeftChannel(user, channel);
            
            if (reason.Length > 0) reason = " (" + reason + ")";
            Logger.Log(LogType.RelayActivity, "{0} kicked {1} from IRC{2}", user.Nick, kickee, reason);
            MessageInGame(user.Nick, "&I(IRC) " + user.Nick + " kicked " + kickee + reason);
        }
        
        void Listener_OnKill(UserInfo user, string nick, string reason) {
            nicks.OnLeft(user);
        }
    }
}

