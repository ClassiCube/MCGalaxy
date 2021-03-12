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
using System.Text;

namespace MCGalaxy {

    public static class LineWrapper {
        
        static bool EndsInEmote(char[] line, int length, int lineLength) {
            length = Math.Min(length, lineLength);
            
            // skip trailing spaces
            for (; length > 0 & line[length - 1] == ' '; length--) { }
            if (length == 0) return false;
            
            char last = line[length - 1];
            return last.UnicodeToCp437() != last;
        }
        
        static char LastColor(char[] line, int length) {
            for (int i = length - 2; i >= 0; i--) {
                if (line[i] != '&') continue;
                
                char col = Colors.Lookup(line[i + 1]);
                if (col != '\0') return col;
            }
            return 'f';
        }
        
        static string MakeLine(char[] line, int length, bool emotePad) {
            if (emotePad) line[length++] = '\'';
            return new string(line, 0, length);
        }

        static bool IsWrapper(char[] line, int i) {
            char c = line[i];
            if (c == ' ') return true;
            
            // For e.g. "item1/item2/item3", want to wordwrap on the '/'
            // However for "item1 /command", want to wordwrap BEFORE '/'
            // TODO: This probably needs to account for colour codes
            return (c == '-' || c == '/' || c == '\\') && line[i - 1] != ' ';
        }
        
        // TODO: Add outputLine argument, instead of returning string list
        public static List<string> Wordwrap(string message, bool supportsEmotes) {
            List<string> lines   = new List<string>();
            const int limit      = NetUtils.StringSize; // max characters on one line
            const int maxLineLen = limit + 1; // +1 because need to know if length of line overshot limit
            
            char[] line    = new char[maxLineLen];
            bool firstLine = true;
            char lastColor = 'f';
            
            for (int offset = 0; offset < message.Length; ) {
                int length = 0;
                // "Line1", "> Line2", "> Line3"
                if (!firstLine) {
                    line[0] = '>'; line[1] = ' ';
                    length += 2;
                    
                    // Make sure split up lines have the right colour
                    if (lastColor != 'f') {
                        line[2] = '&'; line[3] = lastColor;
                        length += 2;
                    }
                } else if (!supportsEmotes) {
                    // If message starts with emote then prepend &f
                    // (otherwise original minecraft classic trims it)
                    char first = message[0];
                    if (first < ' ' || first > '~') {
                        line[0] = '&'; line[1] = 'f';
                        length += 2;
                    }
                }
                
                // Copy across text up to current line length
                // Also trim leading spaces on subsequent lines
                // (note that first line is NOT trimmed for spaces)
                bool foundStart = firstLine;
                for (; length < maxLineLen && offset < message.Length;) {
                    char c = message[offset++];
                    
                    if (c != ' ' || foundStart) {
                        line[length++] = c;
                        foundStart      = true;
                    }
                }
                
                int lineLength = limit;
                bool emotePad  = false;
                // Check if need to add a padding ' to line end
                // (Lines ending in emote are trimmed by minecraft classic client)
                if (!supportsEmotes && EndsInEmote(line, length, lineLength)) {
                    lineLength--;
                    // If last character on line was an emote, but second last
                    // is NOT an emote, then don't add the trailing ' to line
                    // TODO: avoid calling twice? probably doesn't even matter
                    emotePad = EndsInEmote(line, length, lineLength);
                }
                
                // No need for any more linewrapping?
                if (length <= lineLength) {
                    lines.Add(MakeLine(line, length, emotePad));
                    break;
                }
                firstLine = false;
                
                // Try to split up this line nicely
                for (int i = lineLength - 1; i > limit - 20; i--) {
                    if (!IsWrapper(line, i)) continue;
                    
                    i++; // include line wrapper character on this line
                    offset -= length - i;
                    length  = i;
                    break;
                }
                
                // Couldn't split line up? Deal with leftover characters next line
                if (length > lineLength) {
                    offset -= length - lineLength;
                    length  = lineLength;
                }
                
                // Don't split up line in middle of colour code
                if (line[length - 1] == '&') { length--; offset--; }
                
                lastColor = LastColor(line, length);
                lines.Add(MakeLine(line, length, emotePad));
            }
            return lines;
        }

        public static string CleanupColors(string value, Player p) {
            // Although ClassiCube in classic mode supports invalid colours,
            //  the original vanilla client crashes with invalid colour codes
            // Since it's impossible to identify which client is being used,
            //  just remove the ampersands to be on the safe side
            //  when text colours extension is not supported
            return CleanupColors(value, p.hasTextColors, p.hasTextColors);
        }
        
        /// <summary> Removes redundant colour codes and fixes some colour codes to behave correctly for older clients </summary>
        /// <param name="fullAmpersands"> if false, ampersands not followed by valid colour code are removed </param>
        /// <param name="customCols"> if false, converts custom colour codes into fallback colour code </param>
        public static string CleanupColors(string value, bool fullAmpersands, bool customCols) {
            if (value.IndexOf('&') == -1) return value;
            StringBuilder sb = new StringBuilder(value.Length);
            int lastIdx  = -1;
            char lastCol = 'f', col;
            bool combinable = false;
            
            for (int i = 0; i < value.Length; i++) {
                char c = value[i];
                // Definitely not a colour code
                if (c != '&') {
                    if (c != ' ') combinable = false;
                    sb.Append(c); continue;
                }
                
                // Maybe still not a colour code
                if (i == value.Length - 1 || (col = Colors.Lookup(value[i + 1])) == '\0') {
                    // Treat the & like a normal character
                    //  For clients not supporting standalone '&', show '%' instead
                    combinable = false;
                    sb.Append(fullAmpersands ? '&' : '%');
                    continue;
                }
                if (!customCols) col = Colors.Get(col).Fallback;
                
                // Don't append duplicate colour codes
                if (lastCol != col) {
                    // Remove first colour code in "&a&b or "&a   &b"
                    if (combinable) sb.Remove(lastIdx, 2);
                    
                    sb.Append('&').Append(col);
                    lastIdx = sb.Length - 2;
                    lastCol = col;
                    combinable = true;
                }
                i++; // skip over color code
            }
            
            // Trim trailing color codes
            while (sb.Length >= 2) {
                if (sb[sb.Length - 2] != '&') break;
                if (Colors.Lookup(sb[sb.Length - 1]) == '\0') break;
                // got a color code at the end, remove
                sb.Remove(sb.Length - 2, 2);
            }
            return sb.ToString();
        }
    }
}
