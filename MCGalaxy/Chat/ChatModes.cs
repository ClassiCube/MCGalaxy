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
using MCGalaxy.Commands;
using MCGalaxy.Commands.Chatting;

namespace MCGalaxy {
    public static class ChatModes {
        
        public static bool Handle(Player p, string text) {
            if (text.Length >= 2 && text[0] == '@' && text[1] == '@') {
                text = text.Remove(0, 2);
                DoPM(p, Player.Console, text);
                return true;
            }
            
            if (text[0] == '@' || p.whisper) {
                if (text[0] == '@') text = text.Remove(0, 1).Trim();
                
                string target = p.whisperTo;
                if (target.Length == 0) {
                    text.Separate(out target, out text);
                    
                    if (text.Length == 0) {
                        p.Message("No message entered");
                        return true;
                    }
                }

                Player who = PlayerInfo.FindMatches(p, target);
                if (who == null) return true;
                if (who == p) { p.Message("Trying to talk to yourself, huh?"); return true; }
                
                DoPM(p, who, text);
                return true;
            }
            
            if (p.opchat) {
                MessageOps(p, text);
                return true;
            } else if (p.adminchat) {
                MessageAdmins(p, text);
                return true;
            } else if (text[0] == '#') {
                if (text.Length > 1 && text[1] == '#') {
                    MessageOps(p, text.Substring(2));
                    return true;
                } else {
                    p.Message("&HIf you meant to send this to opchat, use &T##" + text.Substring(1));
                }
            } else if (text[0] == '+') {
                if (text.Length > 1 && text[1] == '+') {
                    MessageAdmins(p, text.Substring(2));
                    return true;
                } else {
                    p.Message("&HIf you meant to send this to adminchat, use &T++" + text.Substring(1));
                }
            }
            return false;
        }
        
        public static void MessageOps(Player p, string message) {
            if (!MessageCmd.CanSpeak(p, "OpChat")) return;
            MessageStaff(p, message, Chat.OpchatPerms, "Ops");
        }

        public static void MessageAdmins(Player p, string message) {
            if (!MessageCmd.CanSpeak(p, "AdminChat")) return;
            MessageStaff(p, message, Chat.AdminchatPerms, "Admins");
        }
        
        public static void MessageStaff(Player p, string message,
                                        ItemPerms perms, string group) {
            if (message.Length == 0) { p.Message("No message to send."); return; }
            
            string chatMsg = "To " + group + " &f-λNICK&f- " + message;
            Chat.MessageChat(ChatScope.Perms, p, chatMsg, perms, null, true);
        }
        
        static void DoPM(Player p, Player target, string message) {
            if (message.Length == 0) { p.Message("No message entered"); return; }
            Logger.Log(LogType.PrivateChat, "{0} @{1}: {2}", p.name, target.name, message);
            
            if (!p.IsConsole) {
                p.Message("[<] {0}: &f{1}", p.FormatNick(target), message);
            }
            Chat.MessageChat(ChatScope.PM, p, "&9[>] λNICK: &f" + message, target, null);
        }
    }
}
