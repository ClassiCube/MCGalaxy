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

namespace MCGalaxy {   
    public static class ChatTokens {
        
        public static string Apply(string text, Player p) {
            if (text.IndexOf('$') == -1) return text;
            StringBuilder sb = new StringBuilder(text);
            Apply(sb, p);
            return sb.ToString();
        }
        
        public static void Apply(StringBuilder sb, Player p) {
            // only apply standard $tokens when necessary
            for (int i = 0; i < sb.Length; i++) {
                if (sb[i] != '$') continue;
                
                foreach (var token in standardTokens) {
                    if (Server.disabledChatTokens.Contains(token.Key)) continue;
                    string value = token.Value(p);
                    if (value == null) continue;
                    sb.Replace(token.Key, value);
                }
                break;
            }
            foreach (var token in CustomTokens)
                sb.Replace(token.Key, token.Value);
        }
        
        public static string ApplyCustom(string text) {
            if (CustomTokens.Count == 0) return text;
            StringBuilder sb = new StringBuilder(text);
            foreach (var token in CustomTokens)
                sb.Replace(token.Key, token.Value);
            return sb.ToString();
        }
        
        
        internal static Dictionary<string, Func<Player, string>> standardTokens 
            = new Dictionary<string, Func<Player, string>> {
            { "$name", p => p.DisplayName == null ? null :
                    (Server.dollarNames ? "$" : "") + Colors.StripColors(p.DisplayName) },
            { "$truename", p => p.truename == null ? null :
                    (Server.dollarNames ? "$" : "") + p.truename },
            { "$date", p => DateTime.Now.ToString("yyyy-MM-dd") },
            { "$time", p => DateTime.Now.ToString("HH:mm:ss") },
            { "$ip", p => p.ip },
            { "$serverip", p => Player.IsLocalIpAddress(p.ip) ? p.ip : Server.IP },
            { "$color", p => p.color },
            { "$rank", p => p.group == null ? null : p.group.name },
            { "$level", p => p.level == null ? null : p.level.name },
            
            { "$deaths", p => p.overallDeath.ToString() },
            { "$money", p => p.money.ToString() },
            { "$blocks", p => p.overallBlocks.ToString() },
            { "$first", p => p.firstLogin.ToString() },
            { "$kicked", p => p.totalKicked.ToString() },
            { "$server", p => Server.name },
            { "$motd", p => Server.motd },
            { "$banned", p => Player.GetBannedCount().ToString() },
            { "$irc", p => Server.ircServer + " > " + Server.ircChannel },
            
            { "$infected", p => p.Game.TotalInfected.ToString() },
            { "$survived", p => p.Game.TotalRoundsSurvived.ToString() },            
        };
        
        public static Dictionary<string, string> CustomTokens = new Dictionary<string, string>();        
        internal static void LoadCustom() {
            CustomTokens.Clear();
            if (File.Exists(Paths.CustomTokensFile)) {
                using (StreamReader r = new StreamReader(Paths.CustomTokensFile)) {
                    string line;
                    while ((line = r.ReadLine()) != null)  {
                        if (line.StartsWith("//")) continue;
                        string[] split = line.Split(new[] char{ ':' }, 2);
                        if (split.Length == 2 && !String.IsNullOrEmpty(split[0]))
                            CustomTokens.Add(split[0], split[1]);
                    }
                }
            } else {
                Server.s.Log("custom$s.txt does not exist, creating");
                using (StreamWriter w = new StreamWriter(Paths.CustomTokensFile)) {
                    w.WriteLine("// This is used to create custom $s");
                    w.WriteLine("// If you start the line with a // it wont be used");
                    w.WriteLine("// It should be formatted like this:");
                    w.WriteLine("// $website:http://example.org");
                    w.WriteLine("// That would replace '$website' in any message to 'http://example.org'");
                    w.WriteLine("// It must not start with a // and it must not have a space between the 2 sides and the colon (:)");
                }
            }
        }
    }
}
