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

namespace MCGalaxy.SQL 
{    
    public static class SqlUtils
    {
        public static string QuoteString(string value) {
            if (value.IndexOf('\'') >= 0) // escape '
                value = value.Replace("'", "''");
            
            return "'" + value + "'";
        }
        
        public static string QuoteName(string name) {
            ValidateName(name);
            return "`" + name + "`";
        }
        
        public static string WithTable(string sql, string table) {
            return sql.Replace("{table}", QuoteName(table));
        }
        
        
        public static bool ValidNameChar(char c) {
            return 
                c > ' '   && c != '"' && c != '%' && c != '&'  &&
                c != '\'' && c != '*' && c != '/' && c != ':'  &&
                c != '<'  && c != '>' && c != '?' && c != '\\' &&
                c != '`'  && c != '|' && c <= '~';
        }
        
        public static void ValidateName(string table) {
            foreach (char c in table) 
            {
                if (ValidNameChar(c)) continue;
                throw new ArgumentException("Invalid character '" + c + "' in table name '" + table + "'");
            }
        }
    }
}
