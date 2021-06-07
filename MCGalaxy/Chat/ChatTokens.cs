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
using MCGalaxy.Network;
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
                if (Server.Config.DisabledChatTokens.Contains(token.Trigger)) continue;
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
            new ChatToken("$date", "Current date (year-month-day)", TokenDate),
            new ChatToken("$time", "Current time of day (hour:minute:second)", TokenTime),
            new ChatToken("$irc", "IRC server and channels", TokenIRC),
            new ChatToken("$banned", "Number of banned players", TokenBanned),
            new ChatToken("$server", "Server's name", TokenServerName),
            new ChatToken("$motd", "Server's MOTD", TokenServerMOTD),
            new ChatToken("$loaded", "Number of loaded levels", TokenLoaded),
            new ChatToken("$worlds", "Number of worlds", TokenWorlds),
            new ChatToken("$online", "Number of players online", TokenOnline),
            
            new ChatToken("$name", "Nickname of the player", TokenName),
            new ChatToken("$truename", "Account name of the player", TokenTrueName),
            new ChatToken("$color", "Color code of the player's nick", TokenColor),
            new ChatToken("$rank", "Name of player's rank/group", TokenRank),
            new ChatToken("$deaths", "Times the player died", TokenDeaths),
            new ChatToken("$money", "Amount of server currency player has", TokenMoney),
            new ChatToken("$blocks", "Number of blocks player has modified", TokenBlocks),
            new ChatToken("$placed", "Number of blocks player has placed", TokenPlaced),
            new ChatToken("$deleted", "Number of blocks player has deleted", TokenDeleted),
            new ChatToken("$drawn", "Number of blocks player has drawn", TokenDrawn),
            new ChatToken("$playtime", "Total time player has spent", TokenPlaytime),
            new ChatToken("$first", "Date player first logged in", TokenFirst),
            new ChatToken("$visits", "Times the player logged in", TokenVisits),
            new ChatToken("$kicked", "Times the player was kicked", TokenKicked),
            new ChatToken("$ip", "IP of the player", TokenIP),
            new ChatToken("$model", "Model of the player", TokenModel),
            new ChatToken("$skin", "Skin of the player", TokenSkin),
            new ChatToken("$level", "Name of level/map player is on", TokenLevel),    
        };

        static string TokenDate(Player p) { return DateTime.Now.ToString("yyyy-MM-dd"); }
        static string TokenTime(Player p) { return DateTime.Now.ToString("hh:mm tt"); }
        static string TokenIRC(Player p)  { return Server.Config.IRCServer + " > " + Server.Config.IRCChannels; }
        static string TokenBanned(Player p) { return Group.BannedRank.Players.Count.ToString(); }
        static string TokenServerName(Player p) { return Server.Config.Name; }
        static string TokenServerMOTD(Player p) { return Server.Config.MOTD; }
        static string TokenLoaded(Player p) { return LevelInfo.Loaded.Count.ToString(); }
        static string TokenWorlds(Player p) { return LevelInfo.AllMapFiles().Length.ToString(); }
        static string TokenOnline(Player p) {
            Player[] players = PlayerInfo.Online.Items;
            int count = 0;
            foreach (Player pl in players) {
                if (p.CanSee(pl)) count++;
            }
            return count.ToString();
        }
        
        static string TokenName(Player p)    { return (Server.Config.DollarNames ? "$" : "") + Colors.StripUsed(p.DisplayName); }
        static string TokenTrueName(Player p){ return (Server.Config.DollarNames ? "$" : "") + p.truename; }
        static string TokenColor(Player p)   { return p.color; }
        static string TokenRank(Player p)    { return p.group.Name; }
        static string TokenDeaths(Player p)  { return p.TimesDied.ToString(); }
        static string TokenMoney(Player p)   { return p.money.ToString(); }
        static string TokenBlocks(Player p)  { return p.TotalModified.ToString(); }
        static string TokenPlaced(Player p)  { return p.TotalPlaced.ToString(); }
        static string TokenDeleted(Player p) { return p.TotalDeleted.ToString(); }
        static string TokenDrawn(Player p)   { return p.TotalDrawn.ToString(); }
        static string TokenPlaytime(Player p){ return p.TotalTime.Shorten(); }
        static string TokenFirst(Player p)   { return p.FirstLogin.ToString(); }
        static string TokenVisits(Player p)  { return p.TimesVisited.ToString(); }
        static string TokenKicked(Player p)  { return p.TimesBeenKicked.ToString(); }
        static string TokenIP(Player p)      { return p.ip; }
        static string TokenModel(Player p)   { return p.Model; }
        static string TokenSkin(Player p)    { return p.SkinName; }
        static string TokenLevel(Player p)   { return p.level == null ? null : p.level.name; }

        public static List<ChatToken> Custom = new List<ChatToken>();
        static bool hookedCustom;        
        internal static void LoadCustom() {            
            TextFile tokensFile = TextFile.Files["Custom $s"];
            tokensFile.EnsureExists();
            
            if (!hookedCustom) {
                hookedCustom = true;
                tokensFile.OnTextChanged += LoadCustom;
            }           
            string[] lines = tokensFile.GetText();
            
            Custom.Clear();
            LoadTokens(lines, 
                       (key, value) => Custom.Add(new ChatToken(key, value, null)));
        }
        
        public delegate void TokenLineProcessor(string phrase, string replacement);
        public static void LoadTokens(string[] lines, TokenLineProcessor addToken) {
            foreach (string line in lines) {
                if (line.StartsWith("//") || line.Length == 0) continue;
                // Need to handle special case of :discord: emotes
                int offset = 0;
                if (line[0] == ':') {
                    int emoteEnd = line.IndexOf(':', 1);
                    if (emoteEnd == -1) continue;
                    offset = emoteEnd + 1;
                }
                
                int separator = FindColon(line, offset);
                if (separator == -1) continue; // not a proper line
                
                string key   = line.Substring(0, separator).Trim().Replace("\\:", ":");
                string value = line.Substring(separator + 1).Trim();
                if (key.Length == 0) continue;
                addToken(key, value);
            }
        }
       
        static int FindColon(string s, int offset) {
            for (int i = offset; i < s.Length; i++) {
                if (s[i] != ':') continue;
                
                // "\:" is used to specify 'this colon is not the separator'
                if (i > 0 && s[i - 1] == '\\') continue;
                return i;
            }
            return -1;
        }
    }
}
