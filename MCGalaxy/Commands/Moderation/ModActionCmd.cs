/*
    Copyright 2015 MCGalaxy
    
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
using System.Net;
using MCGalaxy.DB;
using MCGalaxy.Network;
using BlockID = System.UInt16;

namespace MCGalaxy.Commands.Moderation {
    
    /// <summary> Provides common helper methods for moderation commands. </summary>
    public static class ModActionCmd {
        
        /// <summary> Expands @[rule number] to the actual rule with that number. </summary>
        public static string ExpandReason(Player p, string reason) {
            if (reason.Length == 0 || reason[0] != '@') return reason;
            
            reason = reason.Substring(1);
            int num;
            if (!int.TryParse(reason, out num)) return "@" + reason;
            
            // Treat @num as a shortcut for rule #num
            Dictionary<int, string> sections = GetRuleSections();
            string rule;
            if (sections.TryGetValue(num, out rule)) return rule;
            
            p.Message("No rule has number \"{0}\". Current rule numbers are: {1}",
                           num, sections.Keys.Join(n => n.ToString()));
            return null;
        }
        
        static Dictionary<int, string> GetRuleSections() {
            Dictionary<int, string> sections = new Dictionary<int, string>();
            if (!File.Exists(Paths.RulesFile)) return sections;
            
            string[] rules = File.ReadAllLines(Paths.RulesFile);
            foreach (string rule in rules)
                ParseRule(rule, sections);
            return sections;
        }
        
        static void ParseRule(string rule, Dictionary<int, string> sections) {
            int ruleNum = -1;
            rule = Colors.Strip(rule);
            
            for (int i = 0; i < rule.Length; i++) {
                char c = rule[i];
                bool isNumber = c >= '0' && c <= '9';
                bool isLetter = (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z');
                if (!isNumber && !isLetter) continue;
                // Found start of a word, but didn't find a number - assume this is a non-numbered rule
                if (isLetter && ruleNum == -1) return;
                
                if (isNumber) { // e.g. line is: 1) Do not do X
                    if (ruleNum == -1) ruleNum = 0;
                    ruleNum *= 10;
                    ruleNum += (c - '0');
                } else {
                    sections[ruleNum] = rule.Substring(i);
                    return;
                }
            }
        }
        
        /// <summary> Changes the rank of the given player from the old to the new rank. </summary>
        internal static void ChangeRank(string name, Group oldRank, Group newRank,
                                        Player who, bool saveToNewRank = true) {
            Server.reviewlist.Remove(name);
            oldRank.Players.Remove(name);
            oldRank.Players.Save();
            
            if (saveToNewRank) {
                newRank.Players.Add(name);
                newRank.Players.Save();
            }
            if (who == null) return;

            Entities.DespawnEntities(who, false);
            string dbCol = PlayerDB.FindColor(who);
            if (dbCol.Length == 0) who.color = newRank.Color;
            
            who.group = newRank;
            who.AllowBuild = who.level.BuildAccess.CheckAllowed(who);
            if (who.hidden && who.hideRank < who.Rank) who.hideRank = who.Rank;
            
            who.SetPrefix();
            who.Send(Packet.UserType(who));
            who.SendCurrentBlockPermissions();
            Entities.SpawnEntities(who, false);
            CheckBlockBindings(who);
        }
        
        static void CheckBlockBindings(Player who) {
            BlockID block = who.ModeBlock;
            if (block != Block.Air && !CommandParser.IsBlockAllowed(who, "place", block)) {
                who.ModeBlock = Block.Air;
                who.Message("   Hence, &b{0} %Smode was turned &cOFF",
                            Block.GetName(who, block));
            }
            
            for (int b = 0; b < who.BlockBindings.Length; b++) {
                block = who.BlockBindings[b];
                if (block == b) continue;
                
                if (!CommandParser.IsBlockAllowed(who, "place", block)) {
                    who.BlockBindings[b] = (BlockID)b;
                    who.Message("   Hence, binding for &b{0} %Swas unbound",
                                Block.GetName(who, (BlockID)b));
                }
            }
        }
        
        internal static Group CheckTarget(Player p, CommandData data, string action, string target) {
            if (p.name.CaselessEq(target)) {
                p.Message("You cannot {0} yourself", action); return null; 
            }
            
            Group group = PlayerInfo.GetGroup(target);
            if (p.IsConsole) return group;
            if (!Command.CheckRank(p, data, group.Permission, action, false)) return null;
            return group;
        }
        
        
        /// <summary> Finds the matching name(s) for the input name,
        /// and requires a confirmation message for non-existent players. </summary>
        internal static string FindName(Player p, string action, string cmd,
                                        string cmdSuffix, string name, ref string reason) {
            if (!Formatter.ValidName(p, name, "player")) return null;
            string match = MatchName(p, ref name);
            string confirmed = IsConfirmed(reason);
            if (confirmed != null) reason = confirmed;
            
            if (match != null) {
                if (match.RemoveLastPlus().CaselessEq(name.RemoveLastPlus())) return match;
                // Not an exact match, may be wanting to ban a non-existent account
                p.Message("1 player matches \"{0}\": {1}", name, match);
            }

            if (confirmed != null) return name;
            string msgReason = String.IsNullOrEmpty(reason) ? "" : " " + reason;
            p.Message("If you still want to {0} \"{1}\", use %T/{3} {1}{4}{2} confirm",
                           action, name, msgReason, cmd, cmdSuffix);
            return null;
        }
        
        static string MatchName(Player p, ref string name) {
            int matches;
            Player target = PlayerInfo.FindMatches(p, name, out matches);
            if (matches > 1) return null;
            if (matches == 1) { name = target.name; return name; }
            
            p.Message("Searching PlayerDB...");
            return PlayerDB.MatchNames(p, name);
        }
        
        static string IsConfirmed(string reason) {
            if (reason == null) return null;
            if (reason.CaselessEq("confirm"))
                return "";
            if (reason.CaselessEnds(" confirm"))
                return reason.Substring(0, reason.Length - " confirm".Length);
            return null;
        }
        
        
        /// <summary> Attempts to either parse the message directly as an IP,
        /// or finds the IP of the account whose name matches the message. </summary>
        /// <remarks> "@input" can be used to always find IP by matching account name. <br/>
        /// Warns the player if the input matches both an IP and an account name. </remarks>
        internal static string FindIP(Player p, string message, string cmd, out string name) {
            IPAddress ip;
            name = null;
            
            // TryParse returns "0.0.0.123" for "123", we do not want that behaviour
            if (IPAddress.TryParse(message, out ip) && message.Split('.').Length == 4) {
                string account = ServerConfig.ClassicubeAccountPlus ? message + "+" : message;
                if (PlayerInfo.FindName(account) == null) return message;

                // Some classicube.net accounts can be parsed as valid IPs, so warn in this case.
                p.Message("Note: \"{0}\" is both an IP and an account name. "
                          + "If you meant the account, use %T/{1} @{0}", message, cmd);
                return message;
            }
            
            if (message[0] == '@') message = message.Remove(0, 1);
            Player who = PlayerInfo.FindMatches(p, message);
            if (who != null) { name = who.name; return who.ip; }
            
            p.Message("Searching PlayerDB..");
            string dbIP;
            name = PlayerInfo.FindOfflineIPMatches(p, message, out dbIP);
            return dbIP;
        }
    }
}
