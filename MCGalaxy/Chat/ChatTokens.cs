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
using MCGalaxy.Util;

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
                    if (ServerConfig.DisabledChatTokens.Contains(token.Key)) continue;
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
        
        
        internal static Dictionary<string, StringFormatter<Player>> standardTokens
            = new Dictionary<string, StringFormatter<Player>> {
            { "$name", p => p.DisplayName == null ? null :
                    (ServerConfig.DollarBeforeNamesToken ? "$" : "") + Colors.StripColors(p.DisplayName) },
            { "$truename", p => p.truename == null ? null :
                    (ServerConfig.DollarBeforeNamesToken ? "$" : "") + p.truename },
            { "$date", p => DateTime.Now.ToString("yyyy-MM-dd") },
            { "$time", p => DateTime.Now.ToString("HH:mm:ss") },
            { "$ip", p => p.ip },
            { "$serverip", p => Player.IsLocalIpAddress(p.ip) ? p.ip : Server.IP },
            { "$color", p => p.color },
            { "$rank", p => p.group == null ? null : p.group.Name },
            { "$level", p => p.level == null ? null : p.level.name },
            
            { "$deaths", p => p.TimesDied.ToString() },
            { "$money", p => p.money.ToString() },
            { "$blocks", p => p.TotalModified.ToString() },
            { "$first", p => p.FirstLogin.ToString() },
            { "$kicked", p => p.TimesBeenKicked.ToString() },
            { "$server", p => ServerConfig.Name },
            { "$motd", p => ServerConfig.MOTD },
            { "$banned", p => Group.BannedRank.Players.Count.ToString() },
            { "$irc", p => ServerConfig.IRCServer + " > " + ServerConfig.IRCChannels },
            
            { "$infected", p => p.Game.TotalInfected.ToString() },
            { "$survived", p => p.Game.TotalRoundsSurvived.ToString() },
        };
        
        public static Dictionary<string, string> CustomTokens = new Dictionary<string, string>();
        internal static void LoadCustom() {
            CustomTokens.Clear();
            TextFile tokensFile = TextFile.Files["Custom $s"];
            tokensFile.EnsureExists();
            
            string[] lines = tokensFile.GetText();
            char[] colon = null;
            foreach (string line in lines) {
                if (line.StartsWith("//")) continue;
                
                if (colon == null) colon = new char[] { ':' };
                string[] parts = line.Split(colon, 2);
                if (parts.Length != 2) continue;
                
                string key = parts[0].Trim(), value = parts[1].Trim();
                if (key.Length == 0) continue;
                CustomTokens.Add(key, value);
            }
        }
    }
}
