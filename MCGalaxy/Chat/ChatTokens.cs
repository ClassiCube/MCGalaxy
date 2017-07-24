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
    public sealed class ChatToken {
        public readonly string Trigger;
        public readonly string Description;
        public readonly StringFormatter<Player> Formatter;
        
        public ChatToken(string trigger, string desc, StringFormatter<Player> formatter) {
            Trigger = trigger; Description = desc; Formatter = formatter;
        }
    }
    
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
                ApplyStandard(sb, p); break;
            }
            ApplyCustom(sb);
        }
        
        public static string ApplyCustom(string text) {
            if (Custom.Count == 0) return text;
            StringBuilder sb = new StringBuilder(text);
            ApplyCustom(sb);
            return sb.ToString();
        }
        
        static void ApplyStandard(StringBuilder sb, Player p) {
            foreach (ChatToken token in Standard) {
                if (ServerConfig.DisabledChatTokens.Contains(token.Trigger)) continue;
                string value = token.Formatter(p);
                if (value != null) sb.Replace(token.Trigger, value);
            }
        }
        
        static void ApplyCustom(StringBuilder sb) {
            foreach (ChatToken token in Custom) {
                sb.Replace(token.Trigger, token.Description);
            }
        }
        
        
        public static List<ChatToken> Standard = new List<ChatToken>() {
            new ChatToken("$name", "Nickname of the player",
                          p => (ServerConfig.DollarBeforeNamesToken ? "$" : "") + Colors.StripColors(p.DisplayName)),
            new ChatToken("$truename", "Account name of the player",
                          p => (ServerConfig.DollarBeforeNamesToken ? "$" : "") + p.truename),
            new ChatToken("$date", "Current date (year-month-day)",
                          p => DateTime.Now.ToString("yyyy-MM-dd")),
            new ChatToken("$time", "Current time of day (hour:minute:second)",
                          p => DateTime.Now.ToString("HH:mm:ss")),
            new ChatToken("$ip", "IP of the player", p => p.ip),
            new ChatToken("$serverip", "IP player connected to the server via",
                          p => Player.IsLocalIpAddress(p.ip) ? p.ip : Server.IP),
            new ChatToken("$color", "Color code of the player's nick", p => p.color),
            new ChatToken("$rank", "Name of player's rank/group", p => p.group.Name),
            new ChatToken("$level", "Name of level/map player is on",
                          p => p.level == null ? null : p.level.name),
            
            new ChatToken("$deaths", "Times the player died",
                          p => p.TimesDied.ToString()),
            new ChatToken("$money", "Amount of server currency player has",
                          p => p.money.ToString()),
            new ChatToken("$blocks", "Number of blocks modified by the player",
                          p => p.TotalModified.ToString()),
            new ChatToken("$first", "Date player first logged in",
                          p => p.FirstLogin.ToString()),
            new ChatToken("$kicked", "Times the player was kicked",
                          p => p.TimesBeenKicked.ToString()),
            new ChatToken("$server", "Server's name", p => ServerConfig.Name),
            new ChatToken("$motd", "Server's motd", p => ServerConfig.MOTD),
            new ChatToken("$banned", "Number of banned players",
                          p => Group.BannedRank.Players.Count.ToString()),
            new ChatToken("$irc", "IRC server and channels",
                          p => ServerConfig.IRCServer + " > " + ServerConfig.IRCChannels),
        };
        
        public static List<ChatToken> Custom = new List<ChatToken>();
        internal static void LoadCustom() {
            Custom.Clear();
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
                Custom.Add(new ChatToken(key, value, null));
            }
        }
    }
}
