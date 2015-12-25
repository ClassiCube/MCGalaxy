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
        
        public static bool IsValidColorChar(char color) {
            return ( color >= '0' && color <= '9' ) || ( color >= 'a' && color <= 'f' ) || ( color >= 'A' && color <= 'F' );
        }

        public static string EscapeColours(string message) {
            try {
                int index = 1;
                StringBuilder sb = new StringBuilder();
                Regex r = new Regex("^[0-9a-fA-F]$");
                foreach (char c in message) {
                    if (c == '%') {
                        if (message.Length >= index)
                            sb.Append(r.IsMatch(message[index].ToString()) ? '&' : '%');
                        else
                            sb.Append('%');
                    } else {
                        sb.Append(c);
                    }
                    index++;
                }
                return sb.ToString();
            } catch {
                return message;
            }
        }
    }
}
