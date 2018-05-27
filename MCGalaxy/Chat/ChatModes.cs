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
                DoConsolePM(p, text);
                return true;
            }
            
            if (text[0] == '@' || (p != null && p.whisper)) {
                if (text[0] == '@') text = text.Remove(0, 1).Trim();
                
                if (p == null || p.whisperTo.Length == 0) {
                    int sepIndex = text.IndexOf(' ');
                    if (sepIndex != -1) {
                        string target = text.Substring(0, sepIndex);
                        text = text.Substring(sepIndex + 1);
                        HandleWhisper(p, target, text);
                    } else {
                        Player.Message(p, "No message entered");
                    }
                } else {
                    HandleWhisper(p, p.whisperTo, text);
                }
                return true;
            }
            
            if (p != null && p.opchat) {
                MessageOps(p, text);
                return true;
            } else if (p != null && p.adminchat) {
                MessageAdmins(p, text);
                return true;
            } else if (text[0] == '#') {
                if (text.Length > 1 && text[1] == '#') {
                    MessageOps(p, text.Substring(2));
                    return true;
                } else {
                    Player.Message(p, "%HIf you meant to send this to opchat, use %T##" + text.Substring(1));
                }
            } else if (text[0] == '+') {
                if (text.Length > 1 && text[1] == '+') {
                    MessageAdmins(p, text.Substring(2));
                    return true;
                } else {
                    Player.Message(p, "%HIf you meant to send this to adminchat, use %T++" + text.Substring(1));
                }
            }
            return false;
        }
        
        public static void MessageOps(Player p, string message) {
            LevelPermission rank = CommandExtraPerms.MinPerm("OpChat", LevelPermission.Operator);
            if (!MessageCmd.CanSpeak(p, "OpChat")) return;
            MessageStaff(p, message, rank, "Ops");
        }

        public static void MessageAdmins(Player p, string message) {
            LevelPermission rank = CommandExtraPerms.MinPerm("AdminChat", LevelPermission.Admin);
            if (!MessageCmd.CanSpeak(p, "AdminChat")) return;
            MessageStaff(p, message, rank, "Admins");
        }
        
        public static void MessageStaff(Player p, string message,
                                        LevelPermission perm, string group) {
            string displayName = p == null ? "(console)" : p.ColoredName;
            string name = p == null ? "(console)" : p.name;
            string chatMsg = "To " + group + " &f-" + displayName + "&f-" + message;
            
            if (message.Length == 0) { Player.Message(p, "No message to send."); return; }
            Chat.MessageFrom(ChatScope.AboveOrSameRank, p, chatMsg, perm, null);
            
            if (p != null) p.CheckForMessageSpam();
            Logger.Log(LogType.StaffChat, "({0}): {1}: {2}", group, name, message);
            Server.IRC.Say(displayName + "%S: " + message, true);
        }
        
        static void HandleWhisper(Player p, string target, string message) {
            Player who = PlayerInfo.FindMatches(p, target);
            if (who == null) return;
            if (who == p) { Player.Message(p, "Trying to talk to yourself, huh?"); return; }
            
            if (who.Ignores.All) {
                DoFakePM(p, who, message);
            } else if (p != null && who.Ignores.Names.CaselessContains(p.name)) {
                DoFakePM(p, who, message);
            } else {
                DoPM(p, who, message);
            }
            
            if (p != null) p.CheckForMessageSpam();
        }
        
        static void DoConsolePM(Player p, string message) {
            if (message.Length < 1) { Player.Message(p, "No message entered"); return; }
            Player.Message(p, "[<] Console: &f" + message);
            string name = p == null ? "(console)" : p.name;
            Logger.Log(LogType.PrivateChat, "{0} @(console): {1}", name, message);
            
            if (p != null) p.CheckForMessageSpam();
        }
        
        static void DoFakePM(Player p, Player who, string message) {
            string name = p == null ? "(console)" : p.name;
            Logger.Log(LogType.PrivateChat, "{0} @{1}: {2}", name, who.name, message);
            Player.Message(p, "[<] {0}: &f{1}", who.ColoredName, message);
        }
        
        static void DoPM(Player p, Player who, string message) {
            string name = p == null ? "(console)" : p.name;
            string fullName = p == null ? "%S(console)" : p.ColoredName;
            
            Logger.Log(LogType.PrivateChat, "{0} @{1}: {2}", name, who.name, message);
            Player.Message(p, "[<] {0}: &f{1}", who.ColoredName, message);
            Player.Message(who, "&9[>] {0}: &f{1}", fullName, message);
        }
    }
}
