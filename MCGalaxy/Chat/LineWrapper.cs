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
using System.Text.RegularExpressions;

namespace MCGalaxy {

    public static class LineWrapper {
        
        static string FinishLine(char[] line, int len, out char prevColCode) {
            prevColCode = '\0';
            // find last colour code in the line
            for (int i = len - 2; i >= 0; i--) {
                if (line[i] != '&') continue;
                
                char col = line[i + 1];
                if (Colors.Map(ref col)) { prevColCode = col; break; }
            }
            return new string(line, 0, len);
        }
        
        // TODO: Optimize this using a StringBuilder
        public static List<string> Wordwrap(string message) {
            List<string> lines = new List<string>();
            message = Regex.Replace(message, @"(&[0-9a-f])+(&[0-9a-f])", "$2");
            message = Regex.Replace(message, @"(&[0-9a-f])+$", "");

            const int limit = NetUtils.StringSize; // max characters on one line
            const int maxLineLen = limit + 2;      // +2 in case text is longer than one line
            char[] line = new char[maxLineLen];
            
            bool multilined  = false;
            char prevColCode = '\0';
            int trim;
            // TODO: How does < 32 or > 127 behave with original java client
            
            for (int offset = 0; offset < message.Length; ) {
                int length = 0;
                // "Line1", "> Line2", "> Line3"
                if (multilined) {
                    line[0] = '>'; line[1] = ' ';
                    length += 2;
                    
                    // Make sure split up lines have the right colour
                    if (prevColCode != '\0') {
                        line[2] = '&'; line[3] = prevColCode;
                        length += 2;
                    }
                }
                
                // Copy across text up to current line length
                // Also trim the line of starting spaces
                bool foundStart = false;
                for (; length < maxLineLen && offset < message.Length;) {
                    char c = message[offset++];
                    
                    if (c != ' ' || foundStart) {
                        line[length++] = c;
                        foundStart      = true;
                    }
                }
                
                // No need for any more linewrapping
                if (length < limit) {
                    lines.Add(FinishLine(line, length, out prevColCode)); 
                    break;
                }
                multilined = true;
                
                // Try to split up this line nicely
                for (int i = limit - 1; i > limit - 20; i--) {
                    if (line[i] != ' ') continue;
                    
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
                lines.Add(FinishLine(line, length, out prevColCode));  
            }
            return lines;
        }
        
        static bool ValidColor(char c) { return Colors.IsStandard(c) || Colors.IsDefined(c); }
        
        static string CleanupColorCodes(string value) {
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
                    combinable = false;
                    sb.Append(c); continue;
                }
                
                char col = value[i + 1];
                if (col >= 'A' && col <= 'F') col += ' ';
                
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
