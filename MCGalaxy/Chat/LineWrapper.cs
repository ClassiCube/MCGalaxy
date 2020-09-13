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
        
        static char LastColCode(char[] line, int len) {
            for (int i = len - 2; i >= 0; i--) {
                if (line[i] != '&') continue;
                
                char col = line[i + 1];
                if (Colors.Map(ref col)) return col;
            }
            return '\0';
        }
        
        static bool IsWrapper(char c) {
            return c == ' ' || c == '-' || c == '/' || c == '\\';
        }
        
        // TODO: Add outputLine argument, instead of returning string list
        public static List<string> Wordwrap(string message, bool supportsCustomCols) {
            List<string> lines = new List<string>();
            message = CleanupColors(message, supportsCustomCols);

            const int limit = NetUtils.StringSize; // max characters on one line
            const int maxLineLen = limit + 2;      // +2 in case text is longer than one line
            char[] line = new char[maxLineLen];
            
            bool firstLine   = true;
            char prevColCode = '\0';
            int trim;
            // TODO: How does < 32 or > 127 behave with original java client
            
            for (int offset = 0; offset < message.Length; ) {
                int length = 0;
                // "Line1", "> Line2", "> Line3"
                if (!firstLine) {
                    line[0] = '>'; line[1] = ' ';
                    length += 2;
                    
                    // Make sure split up lines have the right colour
                    if (prevColCode != '\0') {
                        line[2] = '&'; line[3] = prevColCode;
                        length += 2;
                    }
                }
                
                // Copy across text up to current line length
                // Also trim the line of starting spaces on subsequent lines
                // (note first line is NOT trimmed for spaces)
                bool foundStart = firstLine;
                for (; length < maxLineLen && offset < message.Length;) {
                    char c = message[offset++];
                    
                    if (c != ' ' || foundStart) {
                        line[length++] = c;
                        foundStart      = true;
                    }
                }
                
                // No need for any more linewrapping
                if (length < limit) {
                    lines.Add(new string(line, 0, length));
                    break;
                }
                firstLine = false;
                
                // Try to split up this line nicely
                for (int i = limit - 1; i > limit - 20; i--) {
                    if (!IsWrapper(line[i])) continue;
                    
                    trim = length - i;
                    length -= trim; offset -= trim;
                    break;
                }
                
                // Couldn't split line up? Deal with leftover characters next line
                if (length > limit) {
                    trim = length - limit;
                    length -= trim; offset -= trim;    
                }
                
                // Don't split up line in middle of colour code
                if (line[length - 1] == '&') {
                    length--; offset--;
                }
                
                prevColCode = LastColCode(line, length);
                lines.Add(new string(line, 0, length));
            }
            return lines;
        }
        
        static bool ValidColor(char c) { return Colors.IsStandard(c) || Colors.IsDefined(c); }
        
        /// <summary> Removes redundant colour codes and fixes some colour codes to behave correctly for older clients </summary>
        /// <param name="supportsCustomCols">if false, fixes colour codes for compatibility 
        /// (e.g. converts custom colour codes into fallback colour code) </param>
        public static string CleanupColors(string value, bool supportsCustomCols) {
            if (value.IndexOf('&') == -1) return value;
            StringBuilder sb = new StringBuilder(value.Length);
            int lastIdx  = -1;
            char lastCol = 'f';
            bool combinable = false;
            
            for (int i = 0; i < value.Length; i++) {
                char c = value[i];
                // Definitely not a colour code
                if (c != '&') {
                    if (c != ' ') combinable = false;
                    sb.Append(c); continue;
                }
                
                // Maybe still not a colour code
                if (i == value.Length - 1 || !ValidColor(value[i + 1])) {
                    if (!supportsCustomCols) {
                        // Although ClassiCube in classic mode supports invalid colours,
                        //  the original vanilla client crashes with invalid colour codes
                        // Since it's impossible to identify which client is being used,
                        //  just remove the ampersands to be on the safe side
                        i++;
                    } else {
                        // Treat the & like a normal character
                        combinable = false;
                        sb.Append(c); 
                    }
                    continue;
                }
                
                char col = value[i + 1];
                // A-F --> a-f
                if (col >= 'A' && col <= 'F') col += ' ';
                if (!supportsCustomCols) col = Colors.Get(col).Fallback;
                
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
                if (sb[sb.Length - 2] != '&')       break;
                if (!ValidColor(sb[sb.Length - 1])) break;
                sb.Remove(sb.Length - 2, 2);
            }
            return sb.ToString();
        }
    }
}
