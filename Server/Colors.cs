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
using System.IO;
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

        public static string Parse(string name) {
            name = name.ToLower();
            switch (name) {
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
                    default: return GetExtColor(name);
            }
        }
        
        public static string Name(string str) {
            if (str.Length != 2 || str[0] != '&') return "";
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
                    default:
                        char fallback = GetFallback(str[1]);
                        return fallback == '\0' ? "" : ExtColors[str[1]].Name;
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
            input = EscapeColors(input);
            StringBuilder sb = new StringBuilder(input);
            
            for (int i = 0; i < 128; i++) {
                CustomColor col = ExtColors[i];
                if (col.Undefined) continue;
                sb.Replace("&" + col.Code, "&" + col.Fallback);
            }
            
            foreach (var kvp in ircColors)
                sb.Replace(kvp.Key, kvp.Value);
            return sb.ToString();
        }
        
        public static bool IsStandardColor(char c) {
            return (c >= '0' && c <= '9') || (c >= 'a' && c <= 'f') || (c >= 'A' && c <= 'F');
        }

        public static string EscapeColors(string value) {
            if (value.IndexOf('%') == -1)
                return value;
            char[] chars = new char[value.Length];
            
            for (int i = 0; i < value.Length; i++ ) {
                char c = value[i];
                bool validCode = c == '%' && i < value.Length - 1;
                if (!validCode) { chars[i] = c; continue; }
                
                char color = value[i + 1];
                if (MapColor(ref color)) {
                    chars[i] = '&';
                    chars[i + 1] = color;
                    i++; continue;
                }
                chars[i] = '%';
            }
            return new string(chars);
        }
        
        public static bool MapColor(ref char color) {
            if (IsStandardColor(color)) return true;
            if (color == 's' || color == 'S') { color = Server.DefaultColor[1]; return true; }
            if (color == 'h' || color == 'H') { color = 'e'; return true; }
            if (color == 't' || color == 'T') { color = 'a'; return true; }
            if (color == 'i' || color == 'I') { color = Server.IRCColour[1]; return true; }
            if (color == 'g' || color == 'G') { color = Server.GlobalChatColor[1]; return true; }
            if (color == 'r' || color == 'R') { color = 'f'; return true; }            
            return Colors.GetFallback(color) != '\0';
        }        
        
        public static string StripColours(string value) {
            if (value.IndexOf('%') == -1)
                return value;
            char[] output = new char[value.Length];
            int usedChars = 0;
            
            for (int i = 0; i < value.Length; i++) {
                char token = value[i];
                if( token == '%' ) {
                    i++; // Skip over the following colour code.
                } else {
                    output[usedChars++] = token;
                }
            }
            return new string(output, 0, usedChars);
        }
        
        public static CustomColor[] ExtColors = new CustomColor[256];
        
        public static char GetFallback(char c) {
            return (int)c >= 256 ? '\0' : ExtColors[c].Fallback;
        }
        
        static string GetExtColor(string name) {
            for (int i = 0; i < ExtColors.Length; i++) {
                CustomColor col = ExtColors[i];
                if (col.Undefined) continue;
                if (col.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                    return "&" + col.Code;
            }
            return "";
        }
        
        public static void AddExtColor(CustomColor col) { SetExtCol(col); }
        
        public static void RemoveExtColor(char code) {
            CustomColor col = default(CustomColor);
            col.Code = code;
            SetExtCol(col);
        }
        
        static void SetExtCol(CustomColor col) {
            ExtColors[col.Code] = col;
            foreach (Player p in PlayerInfo.players) {
                if (!p.HasCpeExt(CpeExt.TextColors)) continue;
                SendSetTextColor(p, col);
            }
            SaveExtColors();
        }
        
        internal static void SendSetTextColor(Player p, CustomColor col) {
            byte[] buffer = new byte[6];
            buffer[0] = Opcode.CpeSetTextColor;
            buffer[1] = col.R; buffer[2] = col.G; buffer[3] = col.B; buffer[4] = col.A; 
            buffer[5] = (byte)col.Code;
            p.SendRaw(buffer);
        }
        
        internal static void SaveExtColors() {
            using (StreamWriter w = new StreamWriter("text/customcolors.txt")) {
                foreach (CustomColor col in ExtColors) {
                    if (col.Undefined) continue;
                    w.WriteLine(col.Code + " " + col.Fallback + " " + col.Name + " " +
                            col.R + " " + col.G + " " + col.B + " " + col.A);              
                }
            }
        }
        
        internal static void LoadExtColors() {
            if (!File.Exists("text/customcolors.txt")) return;
            string[] lines = File.ReadAllLines("text/customcolors.txt");
            CustomColor col = default(CustomColor);
            
            for (int i = 0; i < lines.Length; i++) {
                string[] parts = lines[i].Split(' ');
                if (parts.Length != 7) continue;
                col.Code = parts[0][0]; col.Fallback = parts[1][0];
                col.Name = parts[2];
                
                if (!Byte.TryParse(parts[3], out col.R) || !Byte.TryParse(parts[4], out col.G) ||
                    !Byte.TryParse(parts[5], out col.B) || !Byte.TryParse(parts[6], out col.A))
                    continue;
                ExtColors[col.Code] = col;
            }
        }
    }    
    
    public struct CustomColor {
        public char Code, Fallback;
        public byte R, G, B, A;
        public string Name;
        
        public bool Undefined { get { return Fallback == '\0'; } }
    }
}