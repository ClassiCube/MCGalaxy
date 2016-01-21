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
        
        public static bool IsStandardColor(char c) {
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
            if (IsStandardColor(color)) return true;
            if (color == 's' || color == 'S') { color = Server.DefaultColor[1]; return true; }
            if (color == 'h' || color == 'H') { color = 'e'; return true; }
            if (color == 't' || color == 'T') { color = 'a'; return true; }
            if (color == 'i' || color == 'I') { color = Server.IRCColour[1]; return true; }
            if (color == 'g' || color == 'G') { color = Server.GlobalChatColor[1]; return true; }
            if (color == 'r' || color == 'R') { color = 'f'; return true; }
            
            return GetFallback(color) != '\0';
        }
		public static CustomColor[] ExtColors = new CustomColor[256];
		
		public static char GetFallback(char c) {
			return (int)c >= 256 ? '\0' : ExtColors[c].Fallback;
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
            if (colorParse)
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

            foreach (var customReplacement in Server.customdollars) {
                if (!customReplacement.Key.StartsWith("//")) {
                    try {
                        sb.Replace(customReplacement.Key, customReplacement.Value);
                    } catch {
                    }
                }
            }
        }
        
        internal static bool HandleModes(Player p, string text) {
            if (text.Length >= 2 && text[0] == '@' && text[1] == '@') {
                text = text.Remove(0, 2);
                if (text.Length < 1) { Player.SendMessage(p, "No message entered"); return true; }
                
                Player.SendMessage(p, Server.DefaultColor + "[<] Console: &f" + text);
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
                        Player.SendMessage(p, "No message entered");
                    }
                } else {
                    HandleWhisper(p, p.whisperTo, text);
                }
                return true;
            }
            
            if (text[0] == '#' || (p != null && p.opchat)) {
                if (text[0] == '#') text = text.Remove(0, 1).Trim();

                string displayName = p == null ? "(console)" : p.color + p.DisplayName;
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

                string displayName = p == null ? "(console)" : p.color + p.DisplayName;
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
            Player who = Player.Find(target);
            if (who == null) { Player.SendMessage(p, "Could not find player."); return; }
            if (who == p) { Player.SendMessage(p, "Trying to talk to yourself, huh?"); return; }
            
            LevelPermission perm = p == null ? LevelPermission.Nobody : p.group.Permission;
            LevelPermission whoPerm = who.group.Permission;
            
            if (!who.hidden || (who.hidden && perm >= whoPerm)) {
                if (who.ignoreglobal) {
                    if (!Server.globalignoreops && (perm >= Server.opchatperm && whoPerm < perm)) {
                        DoPM(p, who, message); return;
                    }
                    DoFakePM(p, who, message); return;
                }
                
                foreach (string ignored in who.listignored) {
                    if (p != null && ignored == p.name) {
                        DoFakePM(p, who, message); return;
                    }
                }
                DoPM(p, who, message);
            } else {
                Player.SendMessage(p, "Player \"" + target + "\" isn't online.");
            }
        }
        
        static void DoFakePM(Player p, Player who, string message) {
            string name = p == null ? "(console)" : p.name;
            Server.s.Log(name + " @" + who.name + ": " + message);
            Player.SendMessage(p, "[<] " + who.FullName + ": &f" + message);
        }
        
        static void DoPM(Player p, Player who, string message) {
            string name = p == null ? "(console)" : p.name;
            string fullName = p == null ? "%S(console)" : p.FullName;
            
            Server.s.Log(name + " @" + who.name + ": " + message);
            Player.SendMessage(p, Server.DefaultColor + "[<] " + who.FullName + ": &f" + message);
            Player.SendMessage(who, "&9[>] " + fullName + ": &f" + message);
        }
    }
	
	public struct CustomColor {
		public char Code, Fallback;
		public byte R, G, B, A;
	}
}
