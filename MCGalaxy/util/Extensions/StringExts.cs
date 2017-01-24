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
    
    public static class StringExts {

        public static string Capitalize(this string str) {
            if (String.IsNullOrEmpty(str))
                return String.Empty;
            
            char[] a = str.ToCharArray();
            a[0] = char.ToUpper(a[0]);
            return new string(a);
        }
        
        
        public static string Cp437ToUnicode(this string str) {
            if (str == null || str.Length == 0 || !HasSpecial(str)) return str;
            
            char[] c = str.ToCharArray();
            for (int i = 0; i < str.Length; i++)
                c[i] = Cp437ToUnicode(str[i]);
            return new String(c);
        }
        
        public static string UnicodeToCp437(this string str) {
            if (str == null || str.Length == 0 || !HasSpecial(str)) return str;
            
            char[] c = str.ToCharArray();
            for (int i = 0; i < str.Length; i++)
                c[i] = UnicodeToCp437(str[i]);
            return new String(c);
        }

        public unsafe static void Cp437ToUnicodeInPlace(this string str) {
            fixed (char* ptr = str) {
                for (int i = 0; i < str.Length; i++) {
                    ptr[i] = Cp437ToUnicode(ptr[i]);
                }
            }
        }
        
        public static unsafe void UnicodeToCp437InPlace(this string str) {
            fixed (char* ptr = str) {
                for (int i = 0; i < str.Length; i++) {
                    ptr[i] = UnicodeToCp437(ptr[i]);
                }
            }
        }
        
        
        static char Cp437ToUnicode(char c) {
            if (c < 0x20) {
                return FullCP437Handler.ControlCharReplacements[c];
            } else if (c < 0x7F) {
                return c;
            } else if (c <= 0xFF) {
                return FullCP437Handler.ExtendedCharReplacements[c - 0x7F];
            }
            return '?';
        }
        
        static char UnicodeToCp437(char c) {
            int cpIndex = 0;
            if (c >= ' ' && c <= '~') {
                return c;
            } else if ((cpIndex = FullCP437Handler.ControlCharReplacements.IndexOf(c)) >= 0 ) {
                return (char)cpIndex;
            } else if ((cpIndex = FullCP437Handler.ExtendedCharReplacements.IndexOf(c)) >= 0 ) {
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
