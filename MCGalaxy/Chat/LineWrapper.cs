/*
Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCForge)
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
            // necessary to remove useless trailing color codes, 
            //  as crashes original minecraft classic otherwise
            length = TrimTrailingInvisible(line, length);

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

        static bool StartsWithColor(char[] message, int messageLen, int offset) {
            return message[offset] == '&' 
                && (offset + 1) < messageLen
                && Colors.Lookup(message[offset + 1]) != '\0';
        }
        
        // TODO: Add outputLine argument, instead of returning string list
        public static List<string> Wordwrap(char[] message, int messageLen, bool supportsEmotes) {
            List<string> lines   = new List<string>();
            const int limit      = NetUtils.StringSize; // max characters on one line
            const int maxLineLen = limit + 1; // +1 because need to know if length of line overshot limit
            
            char[] line    = new char[maxLineLen];
            bool firstLine = true;
            char lastColor = 'f';
            
            for (int offset = 0; offset < messageLen; ) {
                int length = 0;
                // "Line1", "> Line2", "> Line3"
                if (!firstLine) {
                    line[0] = '>'; line[1] = ' ';
                    length += 2;
                    
                    // Make sure split up lines have the right colour
                    if (lastColor != 'f' && !StartsWithColor(message, messageLen, offset)) {
                        line[2] = '&'; line[3] = lastColor;
                        length += 2;
                    }
                } else if (!supportsEmotes) {
                    // If message starts with emote or space then prepend &f
                    // (otherwise original minecraft classic client trims it)
                    char first = message[0];
                    if (first <= ' ' || first > '~') {
                        line[0] = '&'; line[1] = 'f';
                        length += 2;
                    }
                }
                
                // Copy across text up to current line length
                // Also trim leading spaces on subsequent lines
                // (note that first line is NOT trimmed for spaces)
                bool foundStart = firstLine;
                for (; length < maxLineLen && offset < messageLen;) {
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
        
        /// <summary> Removes redundant colour codes and fixes some colour codes to behave correctly for older clients </summary>
        /// <param name="fullAmpersands"> if false, ampersands not followed by valid colour code are converted to percents </param>
        /// <param name="customColors"> if false, converts custom colour codes into fallback colour codes </param>
        public static string CleanupColors(string value, bool fullAmpersands, bool customColors) {
            if (value.IndexOf('&') == -1) return value;
            
            int len;
            char[] chars = CleanupColors(value, out len, fullAmpersands, customColors);
            return new string(chars, 0, len);
        }

        public static char[] CleanupColors(string value, out int bufferLen,
                                           bool fullAmpersands, bool customColors) {
            char[] chars = new char[value.Length];
            int lastIdx  = -1, len = 0;
            char lastColor  = 'f', col;
            bool combinable = false;
            
            for (int i = 0; i < value.Length; i++) {
                char c = value[i];
                // Definitely not a colour code
                if (c != '&') {
                    if (c != ' ') combinable = false;
                    chars[len++] = c; 
                    continue;
                }
                
                // Maybe still not a colour code
                if (i == value.Length - 1 || (col = Colors.Lookup(value[i + 1])) == '\0') {
                    // Treat the & like a normal character
                    //  For clients not supporting standalone '&', show '%' instead
                    combinable   = false;
                    chars[len++] = fullAmpersands ? '&' : '%';
                    continue;
                }
                if (!customColors) col = Colors.Get(col).Fallback;
                
                // Don't append duplicate colour codes
                if (lastColor != col) {
                    // If no gap or only whitepsace since prior color code,
                    //  then just replace the prior color code with this one
                    if (combinable) {
                        //  e.g. "&a&bTest"   -> "&bTest"
                        //  e.g. "&a  &bTest" -> "&b  Test"
                        // (it's particularly useful to replace prior color codes
                        //  since original classic trims leading whitespace)
                        chars[lastIdx + 1] = col;
                    } else {
                        // can't simplify, so just append this color code
                        lastIdx      = len;
                        chars[len++] = '&';
                        chars[len++] = col;
                    }
                    
                    lastColor  = col;
                    combinable = true;
                }
                i++; // skip over color code
            }

            bufferLen = TrimTrailingInvisible(chars, len);
            return chars;
        }

        // Trims trailing color codes and whitespace
        static int TrimTrailingInvisible(char[] chars, int len) {
            while (len >= 2)
            {
                char c = chars[len - 1];
                if (c == ' ') { len--; continue; }

                if (chars[len - 2] != '&')    break;
                if (Colors.Lookup(c) == '\0') break;
                len -= 2; // remove color code
            }
            return len;
        }
    }
}
