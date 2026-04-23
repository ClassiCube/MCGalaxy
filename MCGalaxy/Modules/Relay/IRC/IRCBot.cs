/*
    Copyright 2011 MCForge
        
    Dual-licensed under the    Educational Community License, Version 2.0 and
    the GNU General Public License, Version 3 (the "Licenses"); you may
    not use this file except in compliance with the Licenses. You may
    obtain a copy of the Licenses at
    
    https://opensource.org/license/ecl-2-0/
    https://www.gnu.org/licenses/gpl-3.0.html
    
    Unless required by applicable law or agreed to in writing,
    software distributed under the Licenses are distributed on an "AS IS"
    BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express
    or implied. See the Licenses for the specific language governing
    permissions and limitations under the Licenses.
 */
using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using MCGalaxy.Network;

namespace MCGalaxy.Modules.Relay.IRC
{
    public enum IRCControllerVerify { None, HalfOp, OpChannel };
    
    /// <summary> Manages a connection to an IRC server, and handles associated events. </summary>
    public class IRCBot : RelayBot
    {
        TcpClient client;
        StreamReader reader;
        StreamWriter writer;
        
        string botNick, curNick;
        IRCNickList nicks;
        bool ready, registered;
        Random rnd = new Random();
        
        public override string RelayName { get { return "IRC"; } }
        public override bool Enabled  { get { return Server.Config.UseIRC; } }
        public override string UserID { get { return curNick; } }
        
        public override void LoadControllers() {
            Controllers = PlayerList.Load("ranks/IRC_Controllers.txt");
        }
        
        public IRCBot() {
            nicks     = new IRCNickList();
            nicks.bot = this;
        }
        
        
        static char[] newline = { '\n' };
        protected override void DoSendMessage(string channel, string message) {
            if (!ready) return;
            message = ConvertMessage(message);
            
            // IRC messages can't have \r or \n in them
            //  https://stackoverflow.com/questions/13898584/insert-line-breaks-into-an-irc-message
            if (message.IndexOf('\n') == -1) {
                SendPrivMsg(channel, message);
                return;
            }
            
            string[] parts = message.Split(newline, StringSplitOptions.RemoveEmptyEntries);
            foreach (string part in parts)
            {
                SendPrivMsg(channel, part.Replace("\r", ""));
            }
        }
        
        public void Raw(string message) {
            if (!Enabled || !Connected) return;
            SendRaw(message);
        }

        void Join(string channel) {
            if (String.IsNullOrEmpty(channel)) return;
            SendRaw(IRCCmds.Join(channel));
        }
        
        
        protected override bool CanReconnect { get { return canReconnect; } }
        
        protected override void DoConnect() {
            ready   = false;
            botNick = Server.Config.IRCNick.Replace(" ", "");
            
            string host = Server.Config.IRCServer;
            int port    = Server.Config.IRCPort;
            bool useSSL = Server.Config.IRCSSL;

            // most IRC servers supporting SSL/TLS do so on port 6697
            if (port == 6697) useSSL = true;
            
            curNick = botNick;
            bool usePass = Server.Config.IRCIdentify && Server.Config.IRCPassword.Length > 0;
            string serverPass = usePass ? Server.Config.IRCPassword : "*";

            client = new TcpClient();
            client.Connect(host, port);
            
            Stream s = client.GetStream();
            if (useSSL) s = HttpUtil.WrapSSLStream(s, host);
            
            Encoding encoding = new UTF8Encoding(false);
            registered = false;
            
            writer = new StreamWriter(s, encoding);
            writer.AutoFlush = true;
            reader = new StreamReader(s, encoding);
            
            SendRaw(IRCCmds.Pass(serverPass));
            SendRaw(IRCCmds.User(botNick, Server.SoftwareNameVersioned));
            // NOTE: This command may fail if nick is already in use by another IRC user
            SendRaw(IRCCmds.Nick(curNick));
        }
        
        protected override void DoReadLoop() {
            string line;
            
            try {
                while ((line = reader.ReadLine()) != null) { ParseLine(line); }
            } finally {
                client.Close();
            }
        }
        
        protected override void DoDisconnect(string reason) {
            nicks.Clear();
            
            try {
                SendRaw(IRCCmds.Quit(reason));
                client.Close();
            } catch {
                // no point logging disconnect failures
            }
        }
        
        protected override void UpdateConfig() {
            Channels     = Server.Config.IRCChannels.SplitComma();
            OpChannels   = Server.Config.IRCOpChannels.SplitComma();
            IgnoredUsers = Server.Config.IRCIgnored.SplitComma();
            LoadBannedCommands();
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
        
        protected override string ParseMessage(string input) {
            // get rid of background color component of some IRC color codes.
            input = ircTwoColorCode.Replace(input, "$1");
            StringBuilder sb = new StringBuilder(input);
            
            for (int i = 0; i < ircColors.Length; i++) {
                sb.Replace(ircColors[i], ircReplacements[i]);
            }
            for (int i = 0; i < ircSingle.Length; i++) {
                sb.Replace(ircSingle[i], ircReplacements[i]);
            }
            SimplifyCharacters(sb);
            
            // remove misc formatting chars
            sb.Replace(BOLD,      "");
            sb.Replace(ITALIC,    "");
            sb.Replace(UNDERLINE, "");
            
            sb.Replace("\x03", "&f"); // color reset
            sb.Replace("\x0f", "&f"); // reset
            return sb.ToString();
        }
        
        /// <summary> Formats a message for displaying on IRC </summary>
        /// <example> Converts colors such as &amp;0 into IRC color codes </example>
        string ConvertMessage(string message) {
            if (String.IsNullOrEmpty(message.Trim())) message = ".";
            const string resetSignal = "\x03\x0F";
            
            message = ConvertMessageCommon(message);
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
        
        
        protected override bool CheckController(string userID, ref string error) {
            bool foundAtAll = false;
            foreach (string chan in Channels) {
                if (nicks.VerifyNick(chan, userID, ref error, ref foundAtAll)) return true;
            }
            foreach (string chan in OpChannels) {
                if (nicks.VerifyNick(chan, userID, ref error, ref foundAtAll)) return true;
            }
            
            if (!foundAtAll) {
                error = "You are not on the bot's list of known users for some reason, please leave and rejoin.";
            }
            return false;
        }


        void AnnounceJoinLeave(string nick, string verb, string channel) {
            Logger.Log(LogType.RelayActivity, "{0} {1} channel {2}", nick, verb, channel);
            string which = OpChannels.CaselessContains(channel) ? " operator" : "";
            MessageInGame(nick, string.Format("&I(IRC) {0} {1} the{2} channel", nick, verb, which));
        }
        
        static RelayUser NickToUser(string nick) {
            RelayUser rUser = new RelayUser();
            rUser.ID        = nick;
            rUser.Nick      = nick;
            return rUser;
        }

        void JoinChannels() {
            Logger.Log(LogType.RelayActivity, "Joining IRC channels...");
            
            foreach (string chan in Channels)   { Join(chan); }
            foreach (string chan in OpChannels) { Join(chan); }
            ready = true;
        }
        
        void Authenticate() {
            string nickServ = Server.Config.IRCNickServName;
            if (nickServ.Length == 0) return;
            
            if (Server.Config.IRCIdentify && Server.Config.IRCPassword.Length > 0) {
                Logger.Log(LogType.RelayActivity, "Identifying with " + nickServ);
                SendPrivMsg(nickServ, "IDENTIFY " + Server.Config.IRCPassword);
            }
        }
        
        
        public const string BOLD      = "\x02";
        public const string ITALIC    = "\x1D";
        public const string UNDERLINE = "\x1F";
        
        string GenNewNick() {
            // prefer just adding _ to end of real nick
            if (curNick.Length < MAX_USER_LEN) return curNick + "_";

            // .. and then just randomly mutate a leading character
            int idx  = rnd.Next(MAX_USER_LEN / 3);
            char val = (char)('A' + rnd.Next(26));
            return curNick.Substring(0, idx) + val + curNick.Substring(idx + 1);
        }
        

        // See RFC 2812
        const int MAX_CMD_SIZE = 512; // "and these messages SHALL NOT exceed 512 characters in length"
        const int CRLF_LEN     = 2;
        const int MAX_HOST_LEN = 63;  // "<hostname> has a maximum length of 63 characters"
        const int MAX_USER_LEN = 30;
        readonly object sendLock = new object();
        
        public void SendRaw(string msg) {
            const int maxLen = MAX_CMD_SIZE - CRLF_LEN;
            if (msg.Length > maxLen) 
                msg = msg.Substring(0, maxLen);

            try {
                lock (sendLock) { writer.WriteLine(msg); }
            } catch { }
        }

        // target is either a channel name or user nickname
        void SendPrivMsg(string target, string message) {
            string cmd = "PRIVMSG " + target + " :";
            // The maximum 512 byte limit isn't just on the direct client->server message though, 
            //  but also when forwarded to other servers/clients (which adds nick!user@host to message)
            // So take the pessimistic approach and assume worst case scenario
            int maxLen = MAX_CMD_SIZE - MAX_HOST_LEN - MAX_USER_LEN - cmd.Length - CRLF_LEN;

            lock (sendLock)
            {
                for (int idx = 0; idx < message.Length; ) {
                    int partLen = Math.Min(maxLen, message.Length - idx);
                    string part = message.Substring(idx, partLen);
                    
                    SendRaw(cmd + part);
                    idx += partLen;
                }
            }
        }
        
        
        const string CTCP_ACTION = "\u0001ACTION";
        const string CTCP_PREFIX = "\u0001";

        void ParseLine(string line) {
            int index = 0;
            string prefix = IRCUtils.ExtractPrefix(line, ref index);
            string cmd    = IRCUtils.NextParam(line, ref index);
            int code;
            
            if (int.TryParse(cmd, out code)) {
                ParseReply(prefix, code, line, index);
            } else {
                ParseCommand(prefix, cmd, line, index);
            }
        }
        
        void ParseCommand(string user, string cmd, string line, int index) {
            string nick = IRCUtils.ExtractNick(user);
            string msg, channel, target, newNick;
            
            switch (cmd)
            {
                case "PING":
                    // 3.7.2 Ping
                    msg = IRCUtils.NextAll(line, ref index);
                    SendRaw(IRCCmds.Pong(msg));
                    break;
                    
                case "ERROR":
                    // 3.7.4 Error - <error message>
                    msg = IRCUtils.NextAll(line, ref index);
                    Logger.Log(LogType.RelayActivity, "IRC Error: " + msg);
                    break;
                    
                case "NOTICE":
                    // 3.3.2 Notice - <msgtarget> <text>
                    // "The difference between NOTICE and PRIVMSG is that automatic replies
                    //  MUST NEVER be sent in response to a NOTICE message"
                    target = IRCUtils.NextParam(line, ref index);
                    msg    = IRCUtils.NextAll(  line, ref index);
                    
                    if (IRCUtils.IsValidChannel(target)) {
                        //OnPublicNotice(user, target, msg);
                    } else if (msg.CaselessStarts("You are now identified")) {
                        JoinChannels();
                    }
                    break;
                    
                case "JOIN":
                    // 3.2.1 Join - ( <channel> *( "," <channel> ) [ <key> *( "," <key> ) ] ) / "0"
                    channel = IRCUtils.NextParam(line, ref index);
                    SendRaw(IRCCmds.Names(channel));
                    
                    AnnounceJoinLeave(nick, "joined", channel);
                    break;
                    
                case "PRIVMSG":
                    // 3.3.1 Private messages - <msgtarget> <text to be sent>
                    target = IRCUtils.NextParam(line, ref index);
                    msg    = IRCUtils.NextAll(  line, ref index);
                    
                    if (msg.StartsWith(CTCP_ACTION)) {
                        msg = msg.Replace("\x01", "");
                        
                        if (IRCUtils.IsValidChannel(target)) {
                            MessageInGame(nick, string.Format("&I(IRC) * {0} {1}", nick, msg));
                        }
                    } else if (msg.StartsWith(CTCP_PREFIX)) {
                        // Other CTCP/DCC etc messages aren't supported
                    } else if (IRCUtils.IsValidChannel(target)) {
                        RelayUser rUser = NickToUser(nick);
                        HandleChannelMessage(rUser, target, msg);
                    } else {
                        RelayUser rUser = NickToUser(nick);
                        HandleDirectMessage(rUser, nick, msg);
                    }
                    break;
                    
                case "NICK":
                    // 3.1.2 Nick message - <nickname>
                    newNick = IRCUtils.NextParam(line, ref index);   
                    
                    if (curNick == nick) curNick = newNick;
                    // Successfully reclaimed desired nick, so try to sign in again.
                    if (newNick == botNick) Authenticate();

                    nicks.OnChangedNick(nick, newNick);
                    MessageInGame(nick, "&I(IRC) " + nick + " &Sis now known as &I" + newNick);
                    break;
                    
                case "PART":
                    // 3.2.2 Part - Parameters: <channel> *( "," <channel> ) [ <Part Message> ]
                    channel = IRCUtils.NextParam(line, ref index);
                    nicks.OnLeftChannel(nick, channel);

                    if (nick == botNick) return;
                    AnnounceJoinLeave(nick, "left", channel);
                    break;
                    
                case "QUIT":
                    // 3.1.7 Quit - [ <Quit Message> ]
                    nicks.OnLeft(nick);
                    
                    if (nick == botNick) {
                        // IRC user with desired nick disconnected, try to reclaim it
                        SendRaw(IRCCmds.Nick(nick));
                    } else {
                        Logger.Log(LogType.RelayActivity, nick + " left IRC");
                        MessageInGame(nick, "&I(IRC) " + nick + " left");
                    }
                    break;
                    
                case "KICK":
                    // 3.2.8 Kick - <channel> *( "," <channel> ) <user> *( "," <user> ) [<comment>]
                    channel = IRCUtils.NextParam(line, ref index);
                    target  = IRCUtils.NextParam(line, ref index);
                    msg     = IRCUtils.NextAll(  line, ref index);
                    nicks.OnLeftChannel(nick, channel);
                    
                    if (msg.Length > 0) msg = " (" + msg + ")";
                    Logger.Log(LogType.RelayActivity, "{0} kicked {1} from IRC{2}", nick, target, msg);
                    MessageInGame(nick, "&I(IRC) " + nick + " kicked " + target + msg);
                    break;
                    
                case "MODE":
                    // 3.1.5 User mode - <nickname> *( ( "+" / "-" ) *( "i" / "w" / "o" / "O" / "r" ) )
                    // 3.2.3 Channel mode - <channel> *( ( "-" / "+" ) *<modes> *<modeparams> )
                    target = IRCUtils.NextParam(line, ref index);
                    
                    if (IRCUtils.IsValidChannel(target)) {
                        SendRaw(IRCCmds.Names(target));
                    }
                    break;
                    
                case "KILL":
                    // 3.7.1 Kill - <nickname> <comment>
                    target = IRCUtils.NextParam(line, ref index);
                    
                    nicks.OnLeft(target); // TODO log leave
                    break;
            }
        }
        
        const int RPL_WELCOME    = 001;
        const int RPL_NAMREPLY   = 353;
        const int RPL_ENDOFNAMES = 366;
        const int RPL_TRYAGAIN   = 263;
        
        const int ERR_RANGE_BEG  = 401;
        const int ERR_RANGE_END  = 599;
        
        const int ERR_ERRONEUSNICKNAME = 432;
        const int ERR_NICKNAMEINUSE    = 433;
        const int ERR_NICKCOLLISION    = 436;
        const int ERR_USERSDONTMATCH   = 502;
        
        void ParseReply(string prefix, int code, string line, int index) {
            string channel, chanType;
            string target = IRCUtils.NextParam(line, ref index);
            string[] names;
            
            if (code == RPL_WELCOME) {
                registered = true;
                OnReady();
                
                Authenticate();
                JoinChannels();
            } else if (code == RPL_NAMREPLY) {
                // RPL_NAMREPLY - ( "=" / "*" / "@" ) <channel> :[ "@" / "+" ] <nick> *( " " [ "@" / "+" ] <nick> )
                chanType = IRCUtils.NextParam(line, ref index);
                channel  = IRCUtils.NextParam(line, ref index);
                names    = IRCUtils.NextAll(  line, ref index).Split(IRCUtils.SPACE);
                
                nicks.UpdateFor(channel, names);
            } else if (code == RPL_ENDOFNAMES) {
                // RPL_ENDOFNAMES - <channel> :End of NAMES list
            } else if (code == ERR_ERRONEUSNICKNAME) {
                canReconnect = false;
                throw new InvalidOperationException("Invalid characters in IRC bot nickname");
            } else if (code == ERR_NICKNAMEINUSE || code == ERR_NICKCOLLISION) {
                // ERR_NICKNAMEINUSE - <nick> :Nickname is already in use
                // ERR_NICKCOLLISION - <nick> :Nickname collision KILL from <user>@<host>
                if (registered) return;
                
                // If this is the bot's initial connection attempt
                curNick = GenNewNick();
                SendRaw(IRCCmds.Nick(curNick));
            } else if (code >= ERR_RANGE_BEG && code <= ERR_RANGE_END) {
                Logger.Log(LogType.RelayActivity, "IRC Error #{0}: {1}", code,
                           IRCUtils.NextAll(line, ref index));
            }
        }
    }
}

