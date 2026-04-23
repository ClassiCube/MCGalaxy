/*
    Copyright 2015-2024 MCGalaxy
        
    Dual-licensed under the Educational Community License, Version 2.0 and
    the GNU General Public License, Version 3 (the "Licenses"); you may
    not use this file except in compliance with the Licenses. You may
    obtain a copy of the Licenses at
    
    https://opensource.org/license/ecl-2-0/
    https://www.gnu.org/licenses/gpl-3.0.html
    
    Unless required by applicable law or agreed to in writing,
    software distributed under the Licenses are distributed on an "AS IS"
    BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express
    or implied. See the Licenses for the specific language governing
    permissions and limitations under the Licenses.
 */
using System;

namespace MCGalaxy.Modules.Relay.IRC
{
    public static class IRCUtils
    {
        public static char[] SPACE = { ' ' };
        
        const string CHAN_PREFIXES = "#!+&";
        public static bool IsValidChannel(string channel) {
            // valid channels start with #, !, + or &
            return !IsEmptyOrWhitespace(channel)
                && CHAN_PREFIXES.IndexOf(channel[0]) >= 0;
        }

        public static bool IsEmptyOrWhitespace(string str) {
            return str == null || str.Length == 0
                || str.IndexOf( ' ' ) != -1;
        }
        
        
        public static string ExtractNick(string fullUserName)
        {
            // from RFC - nickname [ [ "!" user ] "@" host ]
            // i.e. 'user' and 'host' are both optional parameters
            if (String.IsNullOrEmpty(fullUserName)) return "";
            
            int userBeg = TryFindPrefix( fullUserName, '!' );
            int hostBeg = TryFindPrefix( fullUserName, '@' );
            int nickEnd = Math.Min( userBeg, hostBeg );

            return fullUserName.Substring(0, nickEnd);
        }
        
        static int TryFindPrefix(string str, char c) {
            int index = str.IndexOf(c);
            return index == -1 ? str.Length : index;
        }
        
        
        public static string ExtractPrefix(string str, ref int index) {
            // See RFC 2812, 2.3.1 Message format in Augmented BNF
            // message =  [ ":" prefix SPACE ] command [ params ] crlf
            EatWhitespace(str, ref index);
            if (index >= str.Length || str[index] != ':') return "";
            
            index++; // skip :
            return EatWord(str, ref index);
        }
        
        public static string NextParam(string str, ref int index) {
            EatWhitespace(str, ref index);
            if (index >= str.Length) return "";
            
            if (str[index] != ':')
                return EatWord(str, ref index);
            
            index++; // skip :
            return EatToEnd(str, ref index);
        }
        
        public static string NextAll(string str, ref int index) {
            EatWhitespace(str, ref index);
            if (index >= str.Length) return "";
            
            if (str[index] == ':') index++; // skip :
            return EatToEnd(str, ref index);
        }
        
        
        static void EatWhitespace(string str, ref int index) {
            while (index < str.Length && str[index] == ' ') index++;
        }
        
        static string EatWord(string str, ref int index) {
            int spaceIdx = str.IndexOf(' ', index);
            if (spaceIdx == -1) spaceIdx = str.Length;
            
            string part = str.Substring(index, spaceIdx - index);
            index = spaceIdx;
            return part;
        }
        
        static string EatToEnd(string str, ref int index) {
            string rest = str.Substring(index);
            index = str.Length;
            return rest.TrimEnd(SPACE);
        }
    }
}
