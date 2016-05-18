/*
Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCGalaxy)
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
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using TokenParser = System.Func<bool, MCGalaxy.Player, string>;

namespace MCGalaxy {
    
    public static class Chat {
        
        public static void GlobalChatLevel(Player from, string message, bool showname) {
            if (showname)
                message = "<Level>" + from.color + from.voicestring + from.color + from.prefix + from.name + ": &f" + message;
			Player[] players = PlayerInfo.Online.Items;
            foreach (Player p in players) {
                if (p.level == from.level && p.Chatroom == null)
                    SendMessage(p, from, message);
            }
        }
		
        [Obsolete("Use GlobalChatLevel instead, this method has been removed.")]
        public static void GlobalChatWorld(Player from, string message, bool showname) {
            GlobalChatLevel(from, message, showname);
        }
        
        public static void GlobalChatRoom(Player from, string message, bool showname) {
            string rawMessage = message;
            if ( showname ) {
                message = "<GlobalChatRoom> " + from.color + from.voicestring + from.color + from.prefix + from.name + ": &f" + message;
            }
            Player[] players = PlayerInfo.Online.Items;
            foreach (Player p in players) {
                if (p.Chatroom != null)
                    SendMessage(p, from, message);
            }
            Server.s.Log("<GlobalChatRoom>" + from.name + ": " + rawMessage);
        }
        
        public static void ChatRoom(Player from, string message, bool showname, string chatroom) {
            string rawMessage = message;
            string messageforspy = ( "<ChatRoomSPY: " + chatroom + "> " + from.color + from.voicestring + from.color + from.prefix + from.name + ": &f" + message );
            if (showname)
                message = "<ChatRoom: " + chatroom + "> " + from.color + from.voicestring + from.color + from.prefix + from.name + ": &f" + message;
            
            Player[] players = PlayerInfo.Online.Items;
            foreach (Player p in players) {
                if (p.Chatroom == chatroom)
                    SendMessage(p, from, message);
                if (p.spyChatRooms.Contains(chatroom) && p.Chatroom != chatroom)
                    SendMessage(p, from, message);
            }
            Server.s.Log("<ChatRoom " + chatroom + ">" + from.name + ": " + rawMessage);
        }
		
        public static void GlobalMessageLevel(Level l, string message) {
            Player[] players = PlayerInfo.Online.Items;
            foreach (Player p in players) {
                if (p.level == l && p.Chatroom == null)
                    Player.Message(p, message);
            }
        }
        
        public static void GlobalMessageMinPerms(string message, LevelPermission minPerm) {
            Player[] players = PlayerInfo.Online.Items;
            foreach (Player p in players) {
                if (p.group.Permission >= minPerm)
                    Player.Message(p, message);
            }
        }
        
        public static void GlobalMessageOps(string message) {
            GlobalMessageMinPerms(message, Server.opchatperm);
        }
        
        public static void GlobalMessageAdmins(string message) {
            GlobalMessageMinPerms(message, Server.adminchatperm);
        }
        
        static void SendMessage(Player p, Player from, string message) {
            if (from != null && p.listignored.Contains(from.name)) return;
            
            if (!p.ignoreAll || (from != null && from == p))
                Player.Message(p, Server.DefaultColor + message);
        }
        
        public static void ApplyTokens(StringBuilder sb, Player p, bool colorParse) {
            // only apply standard $tokens when necessary
            for (int i = 0; i < sb.Length; i++) {
                if (sb[i] != '$') continue;
                
                foreach (var token in standardTokens) {
                    if (Server.disabledChatTokens.Contains(token.Key)) continue;
                    string value = token.Value(colorParse, p);
                    if (value == null) continue;
                    sb.Replace(token.Key, value);
                }
                break;
            }
            foreach (var token in CustomTokens)
                sb.Replace(token.Key, token.Value);
        }
        
        internal static Dictionary<string, TokenParser> standardTokens = new Dictionary<string, TokenParser> {
            { "$name", (c, p) => p.DisplayName == null ? null :
                    (Server.dollarNames ? "$" : "") + Colors.StripColours(p.DisplayName) },
            { "$date", (c, p) => DateTime.Now.ToString("yyyy-MM-dd") },
            { "$time", (c, p) => DateTime.Now.ToString("HH:mm:ss") },
            { "$ip", (c, p) => p.ip },
            { "$serverip", (c, p) => Player.IsLocalIpAddress(p.ip) ? p.ip : Server.IP },
            { "$color", (c, p) => c ? p.color : null },
            { "$rank", (c, p) => p.group == null ? null : p.group.name },
            { "$level", (c, p) => p.level == null ? null : p.level.name },
            
            { "$deaths", (c, p) => p.overallDeath.ToString() },
            { "$money", (c, p) => p.money.ToString() },
            { "$blocks", (c, p) => p.overallBlocks.ToString() },
            { "$first", (c, p) => p.firstLogin.ToString() },
            { "$kicked", (c, p) => p.totalKicked.ToString() },
            { "$server", (c, p) => Server.name },
            { "$motd", (c, p) => Server.motd },
            { "$banned", (c, p) => Player.GetBannedCount().ToString() },
            { "$irc", (c, p) => Server.ircServer + " > " + Server.ircChannel },
            
            { "$infected", (c, p) => p.Game.TotalInfected.ToString() },
            { "$survived", (c, p) => p.Game.TotalRoundsSurvived.ToString() },            
        };
        public static Dictionary<string, string> CustomTokens = new Dictionary<string, string>();
        
        internal static void LoadCustomTokens() {
            CustomTokens.Clear();
            if (File.Exists("text/custom$s.txt")) {
                using (CP437Reader r = new CP437Reader("text/custom$s.txt")) {
                    string line;
                    while ((line = r.ReadLine()) != null)  {
                        if (line.StartsWith("//")) continue;
                        string[] split = line.Split(new[] { ':' }, 2);
                        if (split.Length == 2 && !String.IsNullOrEmpty(split[0]))
                            CustomTokens.Add(split[0], split[1]);
                    }
                }
            } else {
                Server.s.Log("custom$s.txt does not exist, creating");
                using (StreamWriter w = File.CreateText("text/custom$s.txt")) {
                    w.WriteLine("// This is used to create custom $s");
                    w.WriteLine("// If you start the line with a // it wont be used");
                    w.WriteLine("// It should be formatted like this:");
                    w.WriteLine("// $website:http://example.org");
                    w.WriteLine("// That would replace '$website' in any message to 'http://example.org'");
                    w.WriteLine("// It must not start with a // and it must not have a space between the 2 sides and the colon (:)");
                }
            }
        }
        
        public static bool HandleModes(Player p, string text) {
            if (text.Length >= 2 && text[0] == '@' && text[1] == '@') {
                text = text.Remove(0, 2);
                if (text.Length < 1) { Player.Message(p, "No message entered"); return true; }
                
                Player.Message(p, "[<] Console: &f" + text);
                string name = p == null ? "(console)" : p.name;
                Server.s.Log("[>] " + name + ": " + text);
                return true;
            }
            
            if (text[0] == '@' || (p != null && p.whisper)) {
                if (text[0] == '@') text = text.Remove(0, 1).Trim();

                if (p == null || p.whisperTo == "") {
                    int pos = text.IndexOf(' ');
                    if ( pos != -1 ) {
                        string to = text.Substring(0, pos);
                        string msg = text.Substring(pos + 1);
                        HandleWhisper(p, to, msg);
                    } else {
                        Player.Message(p, "No message entered");
                    }
                } else {
                    HandleWhisper(p, p.whisperTo, text);
                }
                return true;
            }
            
            if (text[0] == '#' || (p != null && p.opchat)) {
                if (text[0] == '#') text = text.Remove(0, 1).Trim();

                string displayName = p == null ? "(console)" : p.ColoredName;
                string name = p == null ? "(console)" : p.name;
                Chat.GlobalMessageOps("To Ops &f-" + displayName + "&f- " + text);
                if (p != null && p.group.Permission < Server.opchatperm )
                    p.SendMessage("To Ops &f-" + displayName + "&f- " + text);
                
                Server.s.Log("(OPs): " + name + ": " + text);
                Server.IRC.Say(name + ": " + text, true);
                return true;
            }
            if (text[0] == '+' || (p != null && p.adminchat)) {
                if (text[0] == '+') text = text.Remove(0, 1).Trim();

                string displayName = p == null ? "(console)" : p.ColoredName;
                string name = p == null ? "(console)" : p.name;
                Chat.GlobalMessageAdmins("To Admins &f-" + displayName + "&f- " + text);
                if (p != null && p.group.Permission < Server.adminchatperm)
                    p.SendMessage("To Admins &f-" + displayName + "&f- " + text);
                
                Server.s.Log("(Admins): " + name + ": " + text);
                Server.IRC.Say(name + ": " + text, true);
                return true;
            }
            return false;
        }
        
        static void HandleWhisper(Player p, string target, string message) {
            Player who = PlayerInfo.FindOrShowMatches(p, target);
            if (who == null) return;
            if (who == p) { Player.Message(p, "Trying to talk to yourself, huh?"); return; }
            
            if (who.ignoreAll) {
                DoFakePM(p, who, message); return;
            }          
            if (p != null && who.listignored.Contains(p.name)) {
                DoFakePM(p, who, message); return;
            }
            DoPM(p, who, message);
        }
        
        static void DoFakePM(Player p, Player who, string message) {
            string name = p == null ? "(console)" : p.name;
            Server.s.Log(name + " @" + who.name + ": " + message);
            Player.Message(p, "[<] " + who.ColoredName + ": &f" + message);
        }
        
        static void DoPM(Player p, Player who, string message) {
            string name = p == null ? "(console)" : p.name;
            string fullName = p == null ? "%S(console)" : p.ColoredName;
            
            Server.s.Log(name + " @" + who.name + ": " + message);
            Player.Message(p, "[<] " + who.ColoredName + ": &f" + message);
            Player.Message(who, "&9[>] " + fullName + ": &f" + message);
        }
    }
}
