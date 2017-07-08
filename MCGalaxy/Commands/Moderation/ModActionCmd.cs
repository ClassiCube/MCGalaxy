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
using MCGalaxy.Network;

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
            
            Player.Message(p, "No rule has number \"{0}\". Current rule numbers are: {1}",
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
            rule = Colors.StripColors(rule);
            
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
        
        
        /// <summary> Gets a formatted rank change message for the given name. </summary>
        internal static string FormatRankChange(Group curRank, Group newRank, string name, string reason) {
            string direction = newRank.Permission >= curRank.Permission ? " %Swas promoted to " : " %Swas demoted to ";
            Player who = PlayerInfo.FindExact(name);
            
            if (who != null) name = who.ColoredName;
            return name + direction + newRank.ColoredName + "%S. (" + reason + "%S)";
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
            if (who.color == "" || who.color == who.group.Color)
                who.color = newRank.Color;
            
            who.group = newRank;
            AccessResult access = who.level.BuildAccess.Check(who);
            who.AllowBuild = access == AccessResult.Whitelisted || access == AccessResult.Allowed;
            
            who.SetPrefix();
            who.Send(Packet.UserType(who));
            who.SendCurrentBlockPermissions();
            Entities.SpawnEntities(who, false);
            CheckBlockBindings(who);
        }
        
        static void CheckBlockBindings(Player who) {
            ExtBlock block = who.ModeBlock;
            if (block != ExtBlock.Air && !CommandParser.IsBlockAllowed(who, "place", block)) {
                who.ModeBlock = ExtBlock.Air;
                Player.Message(who, "   Hence, &b{0} %Smode was turned &cOFF",
                               who.level.BlockName(block));
            }
            
            for (int i = 0; i < who.BlockBindings.Length; i++) {
                block = who.BlockBindings[i];
                ExtBlock defaultBinding = ExtBlock.FromRaw((byte)i);
                if (block == defaultBinding) continue;
                
                if (!CommandParser.IsBlockAllowed(who, "place", block)) {
                    who.BlockBindings[i] = defaultBinding;
                    Player.Message(who, "   Hence, binding for &b{0} %Swas unbound",
                                   who.level.BlockName(defaultBinding));
                }
            }
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
                Player.Message(p, "1 player matches \"{0}\": {1}", name, match);
            }

            if (confirmed != null) return name;
            string msgReason = String.IsNullOrEmpty(reason) ? "" : " " + reason;
            Player.Message(p, "If you still want to {0} \"{1}\", use %T/{3} {1}{4}{2} confirm",
                           action, name, msgReason, cmd, cmdSuffix);
            return null;
        }
        
        static string MatchName(Player p, ref string name) {
            int matches = 0;
            Player target = PlayerInfo.FindMatches(p, name, out matches);
            if (matches > 1) return null;
            if (matches == 1) { name = target.name; return name; }
            
            Player.Message(p, "Searching PlayerDB...");
            return PlayerInfo.FindOfflineNameMatches(p, name);
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
        internal static string FindIP(Player p, string message, string action, string cmd) {
            IPAddress ip;
            // TryParse returns "0.0.0.123" for "123", we do not want that behaviour
            if (IPAddress.TryParse(message, out ip) && message.Split('.').Length == 4) {
                string account = ServerConfig.ClassicubeAccountPlus ? message + "+" : message;
                if (PlayerInfo.FindName(account) == null) return message;

                // Some classicube.net accounts can be parsed as valid IPs, so warn in this case.
                Player.Message(p, "Note: \"{0}\" is an IP, but also an account name. "
                               + "If you meant to {1} the account, use %T/{2} @{0}",
                               message, action, cmd);
                return message;
            }
            
            if (message[0] == '@') message = message.Remove(0, 1);
            Player who = PlayerInfo.FindMatches(p, message);
            if (who != null) return who.ip;
            
            Player.Message(p, "Searching PlayerDB..");
            string databaseIP;
            PlayerInfo.FindOfflineIPMatches(p, message, out databaseIP);
            return databaseIP;
        }
    }
}
