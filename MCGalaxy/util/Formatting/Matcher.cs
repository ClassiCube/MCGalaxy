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
using System.IO;
using System.Text;

namespace MCGalaxy {
    
    /// <summary> Finds partial matches of a 'name' against the names of the items an enumerable. </summary>
    /// <remarks> returns number of matches found, and the matching item if only 1 match is found. </remarks>
    public static class Matcher {
        
        const StringComparison comp = StringComparison.OrdinalIgnoreCase;

        /// <summary> Finds partial matches of 'name' against the list of all awards. </summary>
        public static string FindAwards(Player p, string name) {
            int matches = 0;
            Awards.Award award = Find<Awards.Award>(p, name, out matches, Awards.AwardsList,
                                                    null, a => a.Name, "awards");
            return award == null ? null : award.Name;
        }
        
        /// <summary> Finds partial matches of 'name' against the list of all bots. </summary>
        public static PlayerBot FindBots(Player p, string name) {
            int matches = 0;
            return Find<PlayerBot>(p, name, out matches, PlayerBot.Bots.Items,
                                   null, b => b.name, "bots");
        }
        
        /// <summary> Finds partial matches of 'name' against the list of bots in same level as player. </summary>
        public static PlayerBot FindBotsInLevel(Player p, string name) {
            int matches = 0;
            return Find<PlayerBot>(p, name, out matches, PlayerBot.Bots.Items,
                                   b => b.level == p.level, b => b.name, "bots in this level");
        }
        
        /// <summary> Find partial matches of 'name' against the list of loaded maps/levels. </summary>
        public static Level FindLevels(Player p, string name) {
            int matches = 0;
            return Find<Level>(p, name, out matches, LevelInfo.Loaded.Items,
                               null, l => l.name, "loaded levels");
        }

        /// <summary> Find partial matches of 'name' against the list of all map files. </summary>
        public static string FindMaps(Player pl, string name) {
            int matches = 0;
            if (!Formatter.ValidName(pl, name, "level")) return null;
            
            string[] files = LevelInfo.AllMapFiles();
            string map = Find<string>(pl, name, out matches, files,
                                      null, l => Path.GetFileNameWithoutExtension(l), "levels");
            
            if (map != null) map = Path.GetFileNameWithoutExtension(map);
            return map;
        }
        
        /// <summary> Find partial matches of 'name' against the list of ranks. </summary>
        public static Group FindRanks(Player p, string name) {
            name = name.ToLower();
            Group.MapName(ref name);
            
            int matches = 0;
            return Find<Group>(p, name, out matches, Group.GroupList,
                               null, g => g.name, "ranks");
        }
        
        
        /// <summary> Finds partial matches of 'name' against the names of the items in the 'items' enumerable. </summary>
        /// <param name="pl"> The player to output messages to. </param>
        /// <param name="name"> The name to perform partial matching against. </param>
        /// <param name="matches"> The number of found/outputted matches. </param>
        /// <param name="items"> Enumerable of items that may be matched with. </param>
        /// <param name="filter"> Selects which items from 'items' are actually matched. </param>
        /// <param name="nameGetter"> Gets the name of a particular item. </param>
        /// <param name="group"> The group/type of the items. (e.g. 'players', 'commands') </param>
        /// <param name="limit"> The maximum number of matches that are outputted. </param>
        /// <returns> If exactly one match, the matching item. </returns>
        public static T Find<T>(Player pl, string name, out int matches, IEnumerable items,
                                Predicate<T> filter, Func<T, string> nameGetter, string group, int limit = 5)  {
            T match = default(T); matches = 0;
            StringBuilder nameMatches = new StringBuilder();

            foreach (T item in items) {
                if (filter != null && !filter(item)) continue;
                string itemName = nameGetter(item);
                if (itemName.Equals(name, comp)) { matches = 1; return item; }
                if (itemName.IndexOf(name, comp) < 0) continue;
                
                match = item; matches++;
                if (matches <= limit) {
                    nameMatches.Append(itemName).Append(", ");
                } else if (matches == limit + 1) {
                    nameMatches.Append("(and more)").Append(", ");
                }
            }
            
            if (matches == 0) {
                Player.Message(pl, "No " + group + " match \"" + name + "\"."); 
                return default(T);
            } else if (matches == 1) {
                return match;
            } else {
                string count = matches > limit ? limit + "+ " : matches + " ";
                string names = nameMatches.ToString(0, nameMatches.Length - 2);
                
                Player.Message(pl, count + group + " match \"" + name + "\":");
                Player.Message(pl, names); 
                return default(T);
            }
        }
    }
}
