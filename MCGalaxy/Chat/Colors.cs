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
using MCGalaxy.Network;

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
        
        public static ColorDesc[] List = new ColorDesc[256];
        static Colors() {
            for (int i = 0; i < List.Length; i++) {
                List[i] = DefaultCol((char)i);
            }
        }
        
        
        /// <summary> Returns whether the given color code is defined. </summary>
        /// <remarks> NOTE: This returns false for A to F, be warned! </remarks>
        public static bool IsDefined(char c) { return c <= '\xff' && List[c].Fallback != '\0'; }
        
        /// <summary> Returns whether c is a color code in 0-9, a-f, or A-F. </summary>
        public static bool IsStandard(char c) {
            return (c >= '0' && c <= '9') || (c >= 'a' && c <= 'f') || (c >= 'A' && c <= 'F');
        }
        

        /// <summary> Gets the default color description for the given color code. </summary>
        public static ColorDesc DefaultCol(char code) {
            switch (code) {
                case '0': return new ColorDesc('0', "Black");
                case '1': return new ColorDesc('1', "Navy");
                case '2': return new ColorDesc('2', "Green");
                case '3': return new ColorDesc('3', "Teal");
                case '4': return new ColorDesc('4', "Maroon");
                case '5': return new ColorDesc('5', "Purple");
                case '6': return new ColorDesc('6', "Gold");
                case '7': return new ColorDesc('7', "Silver");
                case '8': return new ColorDesc('8', "Gray");
                case '9': return new ColorDesc('9', "Blue");
                case 'a': return new ColorDesc('a', "Lime");
                case 'b': return new ColorDesc('b', "Aqua");
                case 'c': return new ColorDesc('c', "Red");
                case 'd': return new ColorDesc('d', "Pink");
                case 'e': return new ColorDesc('e', "Yellow");
                case 'f': return new ColorDesc('f', "White");
            }
            
            ColorDesc col = default(ColorDesc);
            col.Code = code;
            return col;
        }
        
        /// <summary> Updates the colors list array, sends change to all players, then saves color list. </summary>
        public static void Update(ColorDesc col) {
            List[col.Code] = col;
            Player[] players = PlayerInfo.Online.Items;
            foreach (Player p in players) {
                if (!p.HasCpeExt(CpeExt.TextColors)) continue;
                p.Send(Packet.SetTextColor(col));
            }
            SaveList();
        }

        /// <summary> Finds the color code which has the given name, or empty string if not found. </summary>
        public static string Parse(string name) {
            for (int i = 0; i < List.Length; i++) {
                if (List[i].Undefined) continue;
                if (!List[i].Name.CaselessEq(name)) continue;
                return "&" + List[i].Code;
            }
            return "";
        }
        
        /// <summary> Gets the name of the given color code. </summary>
        public static string Name(string color) {
            if (color.Length != 2 || color[0] != '&') return "";
            return Name(color[1]);
        }
        
        /// <summary> Gets the name of the given color code. </summary>
        public static string Name(char code) {
            if (code >= 'A' && code <= 'F') code += ' ';
            return IsDefined(code) ? List[code].Name : "";
        }
            
        
        static readonly Dictionary<string, string> ircColors = new Dictionary<string, string> {
            { white, "\u000300" }, { black, "\u000301" }, { navy, "\u000302" }, { green, "\u000303" }, 
            { red, "\u000304" }, { maroon, "\u000305" }, { purple, "\u000306" }, { gold, "\u000307" },
            { yellow, "\u000308" }, { lime, "\u000309" }, { teal, "\u000310" }, { aqua, "\u000311" },
            { blue, "\u000312" }, { pink, "\u000313" }, { gray, "\u000314" }, { silver, "\u000315" },
        };
        static readonly Dictionary<string, string> ircSingleColors = new Dictionary<string, string> {
            { white, "\u00030" }, { black, "\u00031" }, { navy, "\u00032" }, { green, "\u00033" }, 
            { red, "\u00034" }, { maroon, "\u00035" }, { purple, "\u00036" }, { gold, "\u00037" },
            { yellow, "\u00038" }, { lime, "\u00039" },
        };
        static readonly Regex IrcTwoColorCode = new Regex("(\x03\\d{1,2}),\\d{1,2}");
        
        /// <summary> Converts IRC color codes into normal color codes. </summary>
        public static string ConvertIRCToMC(string input) {
            if (input == null) throw new ArgumentNullException("input");
            // get rid of background color component of some IRC color codes.
            input = IrcTwoColorCode.Replace(input, "$1");
            StringBuilder sb = new StringBuilder(input);
            
            foreach (var kvp in ircColors) {
                sb.Replace(kvp.Value, kvp.Key);
            }
            foreach (var kvp in ircSingleColors) {
                sb.Replace(kvp.Value, kvp.Key);
            }
            
            sb.Replace("\u0003", white); // color reset
            sb.Replace("\u000f", white); // reset
            return sb.ToString();
        }

        /// <summary> Escapes then converts color codes into IRC color codes. </summary>
        public static string ConvertMCToIRC(string input) {
            if (input == null) throw new ArgumentNullException("input");
            input = Escape(input);
            StringBuilder sb = new StringBuilder(input);
            
            for (int i = 0; i < List.Length; i++) {
                ColorDesc col = List[i];
                if (col.Undefined || col.Code == col.Fallback) continue;
                sb.Replace("&" + col.Code, "&" + col.Fallback);
            }
            
            foreach (var kvp in ircColors) {
                sb.Replace(kvp.Key, kvp.Value);
            }
            return sb.ToString();
        }
        
        /// <summary> Maps internal system color codes to their actual color codes. </summary>
        /// <remarks> Also converts uppercase standard color codes to lowercase. </remarks>
        /// <returns> Whether given color code was a valid color code. </returns>
        public static bool Map(ref char col) {
            if (IsStandard(col)) {
                if (col >= 'A' && col <= 'F') col += ' ';
                return true;
            }
            
            if (col == 'S') { col = ServerConfig.DefaultColor[1]; return true; }
            if (col == 'H') { col = ServerConfig.HelpDescriptionColor[1]; return true; }
            if (col == 'T') { col = ServerConfig.HelpSyntaxColor[1]; return true; }
            if (col == 'I') { col = ServerConfig.IRCColor[1]; return true; }
            return IsDefined(col);
        }
        
        
        /// <summary> Converts percentage color codes to their actual/real color codes. </summary>
        public static string Escape(string value) {
            if (value.IndexOf('%') == -1) return value;
            char[] chars = new char[value.Length];
            
            for (int i = 0; i < value.Length; i++ ) {
                char c = value[i];
                bool validCode = c == '%' && i < value.Length - 1;
                if (!validCode) { chars[i] = c; continue; }
                
                char color = value[i + 1];
                if (Map(ref color)) {
                    chars[i] = '&';
                    chars[i + 1] = color;
                    i++; continue;
                }
                chars[i] = '%';
            }
            return new string(chars);
        }
        
        /// <summary> Removes all percentage and actual color codes from the given string. </summary>
        public static string Strip(string value) {
            if (value.IndexOf('%') == -1 && value.IndexOf('&') == -1) return value;
            char[] output = new char[value.Length];
            int usedChars = 0;
            
            for (int i = 0; i < value.Length; i++) {
                char token = value[i];
                if (token == '%' || token == '&') {
                    i++; // Skip over the following color code.
                } else {
                    output[usedChars++] = token;
                }
            }
            return new string(output, 0, usedChars);
        }        
       
        /// <summary> Removes all non-existent color codes, and converts
        /// custom colors to their fallback standard color codes if required. </summary>
        public static string Cleanup(string value, bool supportsCustomCols) {
            StringBuilder sb = new StringBuilder(value);
            Cleanup(sb, supportsCustomCols);
            return sb.ToString();
        }
        
        /// <summary> Removes all non-existent color codes, and converts
        /// custom colors to their fallback standard color codes if required. </summary>
        public static void Cleanup(StringBuilder value, bool supportsCustomCols) {
            for (int i = 0; i < value.Length; i++) {
                char c = value[i];
                if (c != '&' || i == value.Length - 1) continue;
                
                char code = value[i + 1];
                if (IsStandard(code)) {
                    if (code >= 'A' && code <= 'F') {
                        value[i + 1] += ' '; // WoM doesn't work with uppercase colors
                    }
                } else if (!IsDefined(code)) {
                    value.Remove(i, 2); i--; // now need to check char at i again
                } else if (!supportsCustomCols) {
                    value[i + 1] = List[code].Fallback;
                }
            }
        }
        

        /// <summary> Saves the list of all colors. </summary>
        internal static void SaveList() {
            using (StreamWriter w = new StreamWriter(Paths.CustomColorsFile)) {
                foreach (ColorDesc col in List) {
                    if (!col.IsModified()) continue;
                    
                    w.WriteLine(col.Code + " " + col.Fallback + " " + col.Name +
                                " " + col.R + " " + col.G + " " + col.B + " " + col.A);
                }
            }
        }
        
        /// <summary> Loads the list of all colors. </summary>
        internal static void LoadList() {
            if (!File.Exists(Paths.CustomColorsFile)) return;
            string[] lines = File.ReadAllLines(Paths.CustomColorsFile);
            ColorDesc col = default(ColorDesc);
            
            for (int i = 0; i < lines.Length; i++) {
                string[] parts = lines[i].SplitSpaces();
                if (parts.Length != 7) continue;
                col.Code = parts[0][0]; col.Fallback = parts[1][0]; col.Name = parts[2];
                
                if (Byte.TryParse(parts[3], out col.R) && Byte.TryParse(parts[4], out col.G)
                    && Byte.TryParse(parts[5], out col.B) && Byte.TryParse(parts[6], out col.A)) {
                    List[col.Code] = col;
                }
            }
        }

        
        /// <summary> Parses an #RRGGBB hex color string. </summary>
        public static ColorDesc ParseHex(string hex) {
            if (hex.Length > 0 && hex[0] == '#') hex = hex.Remove(0, 1);
            if (hex.Length != 3 && hex.Length != 6)
                throw new ArgumentException("hex must be either 3 or 6 chars long");
            
            ColorDesc c = default(ColorDesc);
            int R, G, B;
            if (hex.Length == 6) {
                R = (Hex(hex[0]) << 4) | Hex(hex[1]);
                G = (Hex(hex[2]) << 4) | Hex(hex[3]);
                B = (Hex(hex[4]) << 4) | Hex(hex[5]);
            } else {
                R = Hex(hex[0]); R |= (R << 4);
                G = Hex(hex[1]); G |= (G << 4);
                B = Hex(hex[2]); B |= (B << 4);
            }
            
            c.R = (byte)R; c.G = (byte)G; c.B = (byte)B; c.A = 255;
            return c;
        }
        
        /// <summary> Gets the index of the given hex character. </summary>
        public static int Hex(char value) {
            if (value >= '0' && value <= '9')
                return (int)(value - '0');
            if (value >= 'a' && value <= 'f')
                return (int)(value - 'a') + 10;
            if (value >= 'A' && value <= 'F')
                return (int)(value - 'A') + 10;
            throw new ArgumentException("Non hex char: " + value);
        }
    }
    
	/// <summary> Describes information about a color code. </summary>
    public struct ColorDesc {
        public char Code, Fallback;
        public byte R, G, B, A;
        public string Name;
        public bool Undefined { get { return Fallback == '\0'; } }
        
        public ColorDesc(byte r, byte g, byte b) {
            Code = '\0'; Fallback = '\0'; Name = null;
            R = r; G = g; B = b; A = 255;
        }
        
        internal ColorDesc(char code, string name) {
            Code = code; Fallback = code; Name = name; A = 255;
            
            if (code >= '0' && code <= '9') {
                HexDecode(code - '0', out R, out G, out B);
            } else {
                HexDecode(code - 'a' + 10, out R, out G, out B);
            }
        }
        
        static void HexDecode(int hex, out byte r, out byte g, out byte b) {
            r = (byte)(191 * ((hex >> 2) & 1) + 64 * (hex >> 3));
            g = (byte)(191 * ((hex >> 1) & 1) + 64 * (hex >> 3));
            b = (byte)(191 * ((hex >> 0) & 1) + 64 * (hex >> 3));
        }      
        
        public bool IsModified() {
            if ((Code >= '0' && Code <= '9') || (Code >= 'a' && Code <= 'f')) {
                ColorDesc def = Colors.DefaultCol(Code);
                return R != def.R || G != def.G || B != def.B || Name != def.Name;
            }
            return !Undefined;
        }
    }
}