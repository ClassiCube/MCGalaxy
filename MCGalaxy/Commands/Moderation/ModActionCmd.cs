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

namespace MCGalaxy.Commands.Moderation {
    public abstract class ModActionCmd : Command {
        public override string type { get { return CommandTypes.Moderation; } }
        public override bool museumUsable { get { return true; } }
        
        protected string GetReason(Player p, string reason) {
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
            if (!File.Exists("text/rules.txt")) return sections;
            
            string[] rules = File.ReadAllLines("text/rules.txt");
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
    }
}
