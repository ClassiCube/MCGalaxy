/*
    Copyright 2015 MCGalaxy
    
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
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace MCGalaxy 
{
    public static class Wildcard 
    {
        /// <summary> Filters the given list of items to matching item names. Accepts * and ? wildcard tokens. </summary>
        public static List<string> Filter<T>(IList<T> input, string keyword, StringFormatter<T> nameGetter,
                                          Predicate<T> filter = null, StringFormatter<T> listFormatter = null) {
            List<string> matches = new List<string>();
            Regex regex = null;
            // wildcard matching
            if (keyword.Contains("*") || keyword.Contains("?")) {
                string pattern = "^" + Regex.Escape(keyword).Replace("\\?", ".").Replace("\\*", ".*") + "$";
                regex = new Regex(pattern, RegexOptions.IgnoreCase);
            }
            
            foreach (T item in input) {
                if (filter != null && !filter(item)) continue;
                string name = nameGetter(item);
                
                if (regex != null) { if (!regex.IsMatch(name)) continue; }
                else { if (!name.CaselessContains(keyword))    continue; }
                
                // format this item for display
                if (listFormatter != null) name = listFormatter(item);
                matches.Add(name);
            }
            return matches;
        }
        
        public static string ToSQLFilter(string filter) {
            // player names can include _ in them
            filter = filter.Replace("_", "#_");
            
            filter = filter.Replace('*', '%').Replace('?', '_');
            return "%" + filter + "%";
        }
    }
}
