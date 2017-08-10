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
using System.Text.RegularExpressions;

namespace MCGalaxy {

    public static class LineWrapper {
        
        // TODO: Optimize this using a StringBuilder
        public static List<string> Wordwrap(string message) {
            List<string> lines = new List<string>();
            message = Regex.Replace(message, @"(&[0-9a-f])+(&[0-9a-f])", "$2");
            message = Regex.Replace(message, @"(&[0-9a-f])+$", "");

            int limit = NetUtils.StringSize; string color = "";
            while ( message.Length > 0 ) {
                //if (Regex.IsMatch(message, "&a")) break;

                if ( lines.Count > 0 ) {
                    if ( message[0] == '&' )
                        message = "> " + message.Trim();
                    else
                        message = "> " + color + message.Trim();
                }

                if ( message.IndexOf("&") == message.IndexOf("&", message.IndexOf("&") + 1) - 2 )
                    message = message.Remove(message.IndexOf("&"), 2);

                if ( message.Length <= limit ) { lines.Add(message); break; }
                for ( int i = limit - 1; i > limit - 20; --i )
                    if ( message[i] == ' ' ) {
                        lines.Add(message.Substring(0, i));
                        goto Next;
                    }

            retry:
                if ( message.Length == 0 || limit == 0 ) { return lines; }

                try {
                    if ( message.Substring(limit - 2, 1) == "&" || message.Substring(limit - 1, 1) == "&" ) {
                        message = message.Remove(limit - 2, 1);
                        limit -= 2;
                        goto retry;
                    }
                    else if ( message[limit - 1] < 32 || message[limit - 1] > 127 ) {
                        message = message.Remove(limit - 1, 1);
                        limit -= 1;
                        //goto retry;
                    }
                }
                catch { return lines; }
                lines.Add(message.Substring(0, limit));

            Next: message = message.Substring(lines[lines.Count - 1].Length);
                if ( lines.Count == 1 ) limit = 60;

                int index = lines[lines.Count - 1].LastIndexOf('&');
                if ( index != -1 ) {
                    if ( index < lines[lines.Count - 1].Length - 1 ) {
                        char next = lines[lines.Count - 1][index + 1];
                        if ( Colors.Map(ref next) ) color = "&" + next;
                        if ( index == lines[lines.Count - 1].Length - 1 ) {
                            lines[lines.Count - 1] = lines[lines.Count - 1].Substring(0, lines[lines.Count - 1].Length - 2);
                        }
                    }
                    else if ( message.Length != 0 ) {
                        char next = message[0];
                        if ( Colors.Map(ref next) ) color = "&" + next;
                        lines[lines.Count - 1] = lines[lines.Count - 1].Substring(0, lines[lines.Count - 1].Length - 1);
                        message = message.Substring(1);
                    }
                }
            }
            for ( int i = 0; i < lines.Count; i++ ) // Gotta do it the old fashioned way...
            {
                char[] temp = lines[i].ToCharArray();
                if ( temp[temp.Length - 2] == '&' ) {
                    temp[temp.Length - 1] = ' ';
                    temp[temp.Length - 2] = ' ';
                }
                lines[i] = new string(temp);
            }
            return lines;
        }
        
        static bool ValidColor(char c) { return Colors.IsStandard(c) || Colors.IsDefined(c); }
        
        static string CleanupColorCodes(string value) {
            if (value.IndexOf('&') == -1) return value;
            char[] output = new char[value.Length];
            int used = 0, last = -200;
            char lastCol = 'f';
            
            for (int i = 0; i < value.Length; i++) {
                // Not a color code so do nothing
                if (value[i] != '&') { 
                    output[used++] = value[i]; continue;
                }
                if (i == value.Length - 1 || !ValidColor(value[i + 1])) {
                    output[used++] = value[i]; continue;
                }
                char col = value[i + 1];
                if (col >= 'A' && col <= 'F') col += ' ';
                
                // Check for double color codes in a row
                if (last == i - 2) {
                    used--;
                    output[used++] = col;
                }
                // Check for same color codes but not in a row
                if (lastCol != col) {
                    output[used++] = '&';
                    output[used++] = col;
                }
                last = i; i++; // skip over color code
                lastCol = col;
            }
            
            // Trim trailing color codes
            int j = value.Length;
            while ((j - 2) >= 0) {
                if (value[j - 2] != '&') break;
                if (!ValidColor(value[j - 1])) break;
                j -= 2; used -= 2;
            }
            return new string(output, 0, used);
        }        
    }
}
