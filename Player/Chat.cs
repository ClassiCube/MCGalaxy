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
using System.Text;
using System.Text.RegularExpressions;

namespace MCGalaxy {
    
    public static class Chat {
        
        public static void GlobalChatLevel(Player from, string message, bool showname) {
            if (showname) {
                message = "<Level>" + from.color + from.voicestring + from.color + from.prefix + from.name + ": &f" + message;
            }
            Player.players.ForEach(
                delegate(Player p) {
                    if (p.level == from.level && p.Chatroom == null)
                        SendGlobalMessage(p, from, message);
                });
        }
        
        public static void GlobalChatRoom(Player from, string message, bool showname) {
            string oldmessage = message;
            if ( showname ) {
                message = "<GlobalChatRoom> " + from.color + from.voicestring + from.color + from.prefix + from.name + ": &f" + message;
            }
            Player.players.ForEach(
                delegate(Player p) {
                    if ( p.Chatroom != null )
                        SendGlobalMessage(p, from, message);
                });
            Server.s.Log(oldmessage + "<GlobalChatRoom>" + from.prefix + from.name + message);
        }
        
        public static void ChatRoom(Player from, string message, bool showname, string chatroom) {
            string oldmessage = message;
            string messageforspy = ( "<ChatRoomSPY: " + chatroom + "> " + from.color + from.voicestring + from.color + from.prefix + from.name + ": &f" + message );
            if ( showname ) {
                message = "<ChatRoom: " + chatroom + "> " + from.color + from.voicestring + from.color + from.prefix + from.name + ": &f" + message;
            }
            Player.players.ForEach(
                delegate(Player p) {
                    if (p.Chatroom == chatroom)
                        SendGlobalMessage(p, from, message);
                });
            Player.players.ForEach(
                delegate(Player p) {
                    if ( p.spyChatRooms.Contains(chatroom) && p.Chatroom != chatroom )
                        SendGlobalMessage(p, from, message);
                });
            Server.s.Log(oldmessage + "<ChatRoom" + chatroom + ">" + from.prefix + from.name + message);
        }

        public static void GlobalChatWorld(Player from, string message, bool showname) {
            if ( showname ) {
                message = "<World>" + from.color + from.voicestring + from.color + from.prefix + from.name + ": &f" + message;
            }
            Player.players.ForEach(
                delegate(Player p) {
                    if ( p.level.worldChat && p.Chatroom == null )
                        SendGlobalMessage(p, from, message);
                });
        }
        
        public static void GlobalMessageLevel(Level l, string message) {
            Player.players.ForEach(
                delegate(Player p) {
                    if ( p.level == l && p.Chatroom == null )
                        Player.SendMessage(p, message);
                });
        }
        
        public static void GlobalMessageOps(string message) {
            Player.players.ForEach(
                delegate(Player p) {
                    if (p.group.Permission >= Server.opchatperm)
                        Player.SendMessage(p, message);
                });
        }
        
        public static void GlobalMessageAdmins(string message) {
            Player.players.ForEach(
                delegate(Player p) {
                    if (p.group.Permission >= Server.adminchatperm)
                        Player.SendMessage(p, message);
                });
        }
        
        static void SendGlobalMessage(Player p, Player from, string message) {
            if (!p.ignoreglobal) {
                if (from != null && p.listignored.Contains(from.name))
                    return;
                Player.SendMessage(p, Server.DefaultColor + message);
                return;
            }
            
            if (!Server.globalignoreops) {
                if (from.group.Permission >= Server.opchatperm) {
                    if (p.group.Permission < from.group.Permission) {
                        Player.SendMessage(p, Server.DefaultColor + message);
                    }
                }
            }
            if (from != null && from == p) {
                Player.SendMessage(from, Server.DefaultColor + message);
            }
        }
        
        public static bool IsValidColor(char c) {
            return (c >= '0' && c <= '9') || (c >= 'a' && c <= 'f') || (c >= 'A' && c <= 'F');
        }

        public static string EscapeColours(string value) {
            if (value.IndexOf('%') == -1)
                return value;
            char[] chars = new char[value.Length];
            
            for (int i = 0; i < value.Length; i++ ) {
                char c = value[i];
                bool validCode = c == '%' && i < value.Length - 1;
                if (!validCode) { chars[i] = c; continue; }
                
                char color = value[i + 1];
                if (Map(ref color)) {
                    chars[i] = '&';
                    chars[i + 1] = color; 
                    i++; continue;
                }
                chars[i] = '%';
            }
            return new string(chars);
        }
        
        public static bool Map(ref char color) {
            if (IsValidColor(color)) return true;
            if (color == 's' || color == 'S') { color = Server.DefaultColor[1]; return true; }
            if (color == 'h' || color == 'H') { color = 'e'; return true; }
            if (color == 't' || color == 'T') { color = 'a'; return true; }
            if (color == 'i' || color == 'I') { color = Server.IRCColour[1]; return true; }
            if (color == 'g' || color == 'G') { color = Server.GlobalChatColor[1]; return true; }
            if (color == 'r' || color == 'R') { color = 'f'; return true; }
            return false;
        }
        
        public static string StripColours(string value) {
            if (value.IndexOf('%') == -1)
                return value;
            char[] output = new char[value.Length];
            int usedChars = 0;
            
            for (int i = 0; i < value.Length; i++) {
                char token = value[i];
                if( token == '%' ) {
                    i++; // Skip over the following colour code.
                } else {
                    output[usedChars++] = token;
                }
            }
            return new string(output, 0, usedChars);
        }
        
        public static void ApplyDollarTokens(StringBuilder sb, Player p, bool colorParse) {
            if (p.DisplayName != null) {
                string prefix = Server.dollardollardollar ? "$" : "";
                sb.Replace("$name", prefix + Chat.StripColours(p.DisplayName));
            }
            sb.Replace("$date", DateTime.Now.ToString("yyyy-MM-dd"));
            sb.Replace("$time", DateTime.Now.ToString("HH:mm:ss"));
            sb.Replace("$ip", p.ip);
            sb.Replace("$serverip", Player.IsLocalIpAddress(p.ip) ? p.ip : Server.IP);
            if ( colorParse ) 
                sb.Replace("$color", p.color);
            sb.Replace("$rank", p.group.name);
            sb.Replace("$level", p.level.name);
            sb.Replace("$deaths", p.overallDeath.ToString());
            sb.Replace("$money", p.money.ToString());
            sb.Replace("$blocks", p.overallBlocks.ToString());
            sb.Replace("$first", p.firstLogin.ToString());
            sb.Replace("$kicked", p.totalKicked.ToString());
            sb.Replace("$server", Server.name);
            sb.Replace("$motd", Server.motd);
            sb.Replace("$banned", Player.GetBannedCount().ToString());
            sb.Replace("$irc", Server.ircServer + " > " + Server.ircChannel);

            foreach ( var customReplacement in Server.customdollars ) {
                if ( !customReplacement.Key.StartsWith("//") ) {
                    try {
                        sb.Replace(customReplacement.Key, customReplacement.Value);
                    }
                    catch { }
                }
            }
        }
    }
}
