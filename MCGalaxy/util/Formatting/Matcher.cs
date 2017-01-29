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
using System.Collections;
using System.Text;

namespace MCGalaxy {
	
	/// <summary> Finds partial matches of a 'name' against the names of the items an enumerable. </summary>
	/// <remarks> returns number of matches found, and the matching item if only 1 match is found. </remarks>
    public static class Matcher {
        
        const StringComparison comp = StringComparison.OrdinalIgnoreCase;
        
        /// <summary> Finds partial matches of 'name' against the names of the items in the 'items' enumerable. </summary>
        /// <param name="pl"> The player to output messages to. </param>
        /// <param name="name"> The name to perform partial matching against. </param>
        /// <param name="matches"> The number of found/outputted matches. </param>
        /// <param name="items"> Enumerable of items that may be matched with. </param>
        /// <param name="filter"> Selects which items from 'items' are actually matched. </param>
        /// <param name="nameGetter"> Gets the name of a particular item. </param>
        /// <param name="type"> The type of the items. (e.g. 'players', 'commands') </param>
        /// <param name="limit"> The maximum number of matches that are outputted. </param>
        /// <returns> If exactly one match, the matching item. </returns>
        public static T Find<T>(Player pl, string name, out int matches, IEnumerable items,
                                Predicate<T> filter, Func<T, string> nameGetter, string type, int limit = 5)  {
            T match = default(T); matches = 0;
            StringBuilder nameMatches = new StringBuilder();

            foreach (T item in items) {
                if (!filter(item)) continue;
                string itemName = nameGetter(item);
                if (itemName.Equals(name, comp)) { matches = 1; return item; }
                if (itemName.IndexOf(name, comp) < 0) continue;
                
                match = item; matches++;
                if (matches <= limit)
                    nameMatches.Append(itemName).Append(", ");
                else if (matches == limit + 1)
                    nameMatches.Append("(and more)").Append(", ");
            }
            
            if (matches == 0) {
                Player.Message(pl, "No " + type + " match \"" + name + "\"."); return default(T);
            } else if (matches == 1) {
                return match;
            } else {
                string count = matches > limit ? limit + "+ " : matches + " ";
                string names = nameMatches.ToString(0, nameMatches.Length - 2);
                Player.Message(pl, count + type + " match \"" + name + "\":");
                Player.Message(pl, names); return default(T);
            }
        }
    }
}
