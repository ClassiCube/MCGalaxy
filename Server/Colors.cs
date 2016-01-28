/*
    Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCGalaxy)
    
    Dual-licensed under the    Educational Community License, Version 2.0 and
    the GNU General Public License, Version 3 (the "Licenses"); you may
    not use this file except in compliance with the Licenses. You may
    obtain a copy of the Licenses at
    
    http://www.opensource.org/licenses/ecl2.php
    http://www.gnu.org/licenses/gpl-3.0.html
    
    Unless required by applicable law or agreed to in writing,
    software distributed under the License is distributed on an "AS IS"
    BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express
    or implied. See the Licenses for the specific language governing
    permissions and limitations under the Licenses.
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace MCGalaxy {
    
    public static class Colors {
        
        public const string black = "&0";
        public const string navy = "&1";
        public const string green = "&2";
        public const string teal = "&3";
        public const string maroon = "&4";
        public const string purple = "&5";
        public const string gold = "&6";
        public const string silver = "&7";
        public const string gray = "&8";
        public const string blue = "&9";
        public const string lime = "&a";
        public const string aqua = "&b";
        public const string red = "&c";
        public const string pink = "&d";
        public const string yellow = "&e";
        public const string white = "&f";

        public static string Parse(string str) {
            switch (str.ToLower()) {
                    case "black": return black;
                    case "navy": return navy;
                    case "green": return green;
                    case "teal": return teal;
                    case "maroon": return maroon;
                    case "purple": return purple;
                    case "gold": return gold;
                    case "silver": return silver;
                    case "gray": return gray;
                    case "blue": return blue;
                    case "lime": return lime;
                    case "aqua": return aqua;
                    case "red": return red;
                    case "pink": return pink;
                    case "yellow": return yellow;
                    case "white": return white;
                    default: return "";
            }
        }
        
        public static string Name(string str) {
            switch (str) {
                    case black: return "black";
                    case navy: return "navy";
                    case green: return "green";
                    case teal: return "teal";
                    case maroon: return "maroon";
                    case purple: return "purple";
                    case gold: return "gold";
                    case silver: return "silver";
                    case gray: return "gray";
                    case blue: return "blue";
                    case lime: return "lime";
                    case aqua: return "aqua";
                    case red: return "red";
                    case pink: return "pink";
                    case yellow: return "yellow";
                    case white: return "white";
                    default: return "";
            }
        }
        
        static readonly Dictionary<string, string> ircColors = new Dictionary<string, string> {
            { white, "\u000300" }, { black, "\u000301" }, { navy, "\u000302" },
            { green, "\u000303" }, { red, "\u000304" }, { maroon, "\u000305" },
            { purple, "\u000306" }, { gold, "\u000307" }, { yellow, "\u000308" },
            { lime, "\u000309" }, { teal, "\u000310" }, { aqua, "\u000311" },
            { blue, "\u000312" }, { pink, "\u000313" }, { gray, "\u000314" },
            { silver, "\u000315" },
        };
        static readonly Dictionary<string, string> ircSingleColors = new Dictionary<string, string> {
            { white, "\u00030" }, { black, "\u00031" }, { navy, "\u00032" },
            { green, "\u00033" }, { red, "\u00034" }, { maroon, "\u00035" },
            { purple, "\u00036" }, { gold, "\u00037" }, { yellow, "\u00038" },
            { lime, "\u00039" },
        };
        
        static readonly Regex IrcTwoColorCode = new Regex("(\x03\\d{1,2}),\\d{1,2}");
        public static string IrcToMinecraftColors(string input) {
            if (input == null) throw new ArgumentNullException("input");
            // get rid of background colour component of some IRC colour codes.
            input = IrcTwoColorCode.Replace(input, "$1");
            StringBuilder sb = new StringBuilder(input);
            
            foreach (var kvp in ircColors)
                sb.Replace(kvp.Value, kvp.Key);
            foreach (var kvp in ircSingleColors)
                sb.Replace(kvp.Value, kvp.Key);
            sb.Replace("\u0003", white); // color reset
            sb.Replace("\u000f", white); // reset
            return sb.ToString();
        }

        public static string MinecraftToIrcColors(string input) {
            if (input == null) throw new ArgumentNullException("input");
            input = Chat.EscapeColours(input);
            StringBuilder sb = new StringBuilder(input);
            
            for (int i = 0; i < 128; i++) {
            	CustomColor col = Chat.ExtColors[i];
            	if (col.Undefined) continue;
            	sb.Replace("&" + col.Code, "&" + col.Fallback);
            }
            
            foreach (var kvp in ircColors)
                sb.Replace(kvp.Key, kvp.Value);
            return sb.ToString();
        }
    }
}