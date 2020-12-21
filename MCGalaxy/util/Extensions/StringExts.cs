/*
    Copyright 2015 MCGalaxy
    
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

namespace MCGalaxy {

    /// <summary> Extension methods relating to strings. </summary>
    public static class StringExts {

        /// <summary> Sets the first character of the input string to uppercase. </summary>
        public static string Capitalize(this string str) {
            if (String.IsNullOrEmpty(str)) return str;
            
            char[] a = str.ToCharArray();
            a[0] = char.ToUpper(a[0]);
            return new string(a);
        }
        
        /// <summary> Removes an ending + from an account name. </summary>
        public static string RemoveLastPlus(this string str) {
            if (String.IsNullOrEmpty(str)) return str;
            
            if (str[str.Length - 1] != '+') return str;
            return str.Substring(0, str.Length - 1);
        }

        /// <summary> Returns whether line is empty or starts with a #. </summary>
        public static bool IsCommentLine(this string line) {
            return line.Length == 0 || line[0] == '#';
        }
        
        
        /// <summary> Converts a string consisting of code page 437 indices into unicode. </summary>
        public static string Cp437ToUnicode(this string str) {
            if (str == null || str.Length == 0 || !HasSpecial(str)) return str;
            
            char[] c = str.ToCharArray();
            for (int i = 0; i < str.Length; i++)
                c[i] = Cp437ToUnicode(str[i]);
            return new String(c);
        }

        /// <summary> Converts a unicode string into a string consisting of code page 437 indices. </summary>
        /// <remarks> Unicode characters not in code page 437 are converted to '?'. </remarks> 
        public static string UnicodeToCp437(this string str) {
            if (str == null || str.Length == 0 || !HasSpecial(str)) return str;
            
            char[] c = str.ToCharArray();
            for (int i = 0; i < str.Length; i++)
                c[i] = UnicodeToCp437(str[i]);
            return new String(c);
        }
        
        
        /// <summary> Converts a code page 437 indice into unicode. </summary>
        public static char Cp437ToUnicode(this char c) {
            if (c < 0x20) {
                return EmotesHandler.ControlCharReplacements[c];
            } else if (c < 0x7F) {
                return c;
            } else if (c <= 0xFF) {
                return EmotesHandler.ExtendedCharReplacements[c - 0x7F];
            }
            return '?';
        }

        /// <summary> Converts a unicode character into a code page 437 indice. </summary>        
        public static char UnicodeToCp437(this char c) {
            int cpIndex = 0;
            if (c >= ' ' && c <= '~') {
                return c;
            } else if ((cpIndex = EmotesHandler.ControlCharReplacements.IndexOf(c)) >= 0 ) {
                return (char)cpIndex;
            } else if ((cpIndex = EmotesHandler.ExtendedCharReplacements.IndexOf(c)) >= 0 ) {
                return (char)(cpIndex + 127);
            }
            return '?';
        }
        
        static bool HasSpecial(string str) {
            for (int i = 0; i < str.Length; i++) {
                if (str[i] < ' ' || str[i] > '~') {
                    return true;
                }
            }
            return false;
        }
    }
}
