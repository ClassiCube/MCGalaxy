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
        public static bool IsDefined(char c) {
            if (c >= ' ' && c <= '~') return List[c].Fallback != '\0';
            return List[c.UnicodeToCp437()].Fallback != '\0';
        }
        
        public static bool IsStandard(char c) {
            return (c >= '0' && c <= '9') || (c >= 'a' && c <= 'f') || (c >= 'A' && c <= 'F');
        }
        
        public static ColorDesc Get(char c) {
            if (c >= ' ' && c <= '~') return List[c];
            return List[c.UnicodeToCp437()];
        }
        
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

        public static void Update(ColorDesc col) {
            List[col.Index] = col;
            Player[] players = PlayerInfo.Online.Items;
            foreach (Player p in players) {
                if (!p.Supports(CpeExt.TextColors)) continue;
                p.Send(Packet.SetTextColor(col));
            }
            SaveList();
        }

        public static string Parse(string name) {
            for (int i = 0; i < List.Length; i++) {
                if (List[i].Undefined) continue;
                if (!List[i].Name.CaselessEq(name)) continue;
                return "&" + List[i].Code;
            }
            return "";
        }

        public static string Name(string color) {
            if (color.Length != 2 || color[0] != '&') return "";
            return Name(color[1]);
        }
        
        public static string Name(char code) {
            if (code >= 'A' && code <= 'F') code += ' ';
            return IsDefined(code) ? Get(code).Name : "";
        }
        
        
        static readonly string[] ircColors = new string[] {
            "\u000300", "\u000301", "\u000302", "\u000303", "\u000304", "\u000305",
            "\u000306", "\u000307", "\u000308", "\u000309", "\u000310", "\u000311",
            "\u000312", "\u000313", "\u000314", "\u000315",
        };
        static readonly string[] ircSingle = new string[] {
            "\u00030", "\u00031", "\u00032", "\u00033", "\u00034", "\u00035",
            "\u00036", "\u00037", "\u00038", "\u00039",
        };
        static readonly string[] ircReplacements = new string[] {
            white, black, navy, green, red, maroon, purple, gold,
            yellow, lime, teal, aqua, blue, pink, gray, silver,
        };
        static readonly Regex IrcTwoColorCode = new Regex("(\x03\\d{1,2}),\\d{1,2}");
        
        public static string ConvertIRCToMC(string input) {
            if (input == null) throw new ArgumentNullException("input");
            // get rid of background color component of some IRC color codes.
            input = IrcTwoColorCode.Replace(input, "$1");
            StringBuilder sb = new StringBuilder(input);
            
            for (int i = 0; i < ircColors.Length; i++) {
                sb.Replace(ircColors[i], ircReplacements[i]);
            }
            for (int i = 0; i < ircSingle.Length; i++) {
                sb.Replace(ircSingle[i], ircReplacements[i]);
            }
            
            // trim misc formatting chars
            sb.Replace("\x02", ""); // bold
            sb.Replace("\x1D", ""); // italic
            sb.Replace("\x1F", ""); // underline
            
            sb.Replace("\x03", white); // color reset
            sb.Replace("\x0f", white); // reset
            return sb.ToString();
        }

        public static string ConvertMCToIRC(string input) {
            if (input == null) throw new ArgumentNullException("input");
            input = Escape(input);
            StringBuilder sb = new StringBuilder(input);
            Cleanup(sb, false);
            
            for (int i = 0; i < ircColors.Length; i++) {
                sb.Replace(ircReplacements[i], ircColors[i]);
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
            if (col == 'W') { col = ServerConfig.WarningErrorColor[1]; return true; }
            return IsDefined(col);
        }
        
        
        /// <summary> Converts percentage color codes to their actual/real color codes. </summary>
        /// <remarks> Does not escape percentage codes that are part of urls. </remarks>
        public static string Escape(string value) {
            if (value.IndexOf('%') == -1) return value;
            char[] chars = new char[value.Length];
            for (int i = 0; i < chars.Length; i++) { chars[i] = value[i]; }
            
            for (int i = 0; i < chars.Length;) {
                int end = value.IndexOf(' ', i);
                if (end == -1) end = value.Length;
                
                if (!IsUrlAt(chars, i, end - i)) Escape(chars, i, end);
                i = end + 1;
            }
            return new string(chars);
        }
        
        static bool IsUrlAt(char[] chars, int i, int len) {
            const int prefixLen = 7; // "http://".Length
            if (len < prefixLen) return false;
            
            // skip color codes in url
            while (len > 0 && chars[i] == '&') { len -= 2; i += 2; }
            
            // Starts with "http" ?
            if (len < prefixLen) return false;
            if (chars[i] != 'h' || chars[i + 1] != 't' || chars[i + 2] != 't' || chars[i + 3] != 'p') return false;
            len -= 4; i += 4;
            
            // And then with "s://" or "://" ?
            if (chars[i] == 's') { len--; i++; }
            return len >= 3 && chars[i] == ':' && chars[i + 1] == '/' && chars[i + 2] == '/';
        }
        
        static void Escape(char[] chars, int start, int end) {
            for (int i = start; i < end; i++ ) {
                char c = chars[i];
                bool validCode = c == '%' && i < chars.Length - 1;
                
                if (!validCode) continue;
                char color = chars[i + 1];
                if (!Map(ref color)) continue;
                
                chars[i] = '&';
                chars[i + 1] = color;
                i++; // skip over color code
            }
        }
        
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
                    value[i + 1] = Get(code).Fallback;
                }
            }
        }

        
        internal static void SaveList() {
            using (StreamWriter w = new StreamWriter(Paths.CustomColorsFile)) {
                foreach (ColorDesc col in List) {
                    if (!col.IsModified()) continue;
                    
                    w.WriteLine(col.Code + " " + col.Fallback + " " + col.Name +
                                " " + col.R + " " + col.G + " " + col.B + " " + col.A);
                }
            }
        }
        
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
                    List[col.Index] = col;
                }
            }
        }

        
        /// <summary> Parses an #RRGGBB hex color string. </summary>
        public static bool TryParseHex(string hex, out ColorDesc c) {
            c = default(ColorDesc);
            if (hex == null || hex.Length == 0) return false;
            if (hex[0] == '#') hex = hex.Remove(0, 1);
            if (!(hex.Length == 3 || hex.Length == 6)) return false;
            
            for (int i = 0; i < hex.Length; i++) {
                if (UnHex(hex[i]) == -1) return false;
            }
            
            int R, G, B;
            if (hex.Length == 6) {
                R = (UnHex(hex[0]) << 4) | UnHex(hex[1]);
                G = (UnHex(hex[2]) << 4) | UnHex(hex[3]);
                B = (UnHex(hex[4]) << 4) | UnHex(hex[5]);
            } else {
                R = UnHex(hex[0]); R |= (R << 4);
                G = UnHex(hex[1]); G |= (G << 4);
                B = UnHex(hex[2]); B |= (B << 4);
            }

            c.R = (byte)R; c.G = (byte)G; c.B = (byte)B; c.A = 255;
            return true;
        }
        
        /// <summary> Parses an #RRGGBB hex color string. </summary>
        public static ColorDesc ParseHex(string hex) {
            ColorDesc c;
            if (!TryParseHex(hex, out c)) throw new ArgumentException("invalid input");
            return c;
        }
        
        public static int UnHex(char c) {
            if (c >= '0' && c <= '9') { return (int)(c - '0'); }
            if (c >= 'a' && c <= 'f') { return (int)(c - 'a') + 10; }
            if (c >= 'A' && c <= 'F') { return (int)(c - 'A') + 10; }
            
            return -1;
        }
    }
    
    /// <summary> Describes information about a color code. </summary>
    public struct ColorDesc {
        public char Code, Fallback;
        public byte R, G, B, A;
        public string Name;
        public bool Undefined { get { return Fallback == '\0'; } }
        public byte Index { get { return (byte)Code.UnicodeToCp437(); } }
        
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