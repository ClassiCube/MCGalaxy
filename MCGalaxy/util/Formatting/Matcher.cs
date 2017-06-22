﻿/*
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
using System.Collections.Generic;
using System.IO;
using System.Text;
using MCGalaxy.Eco;

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
                                      null, l => Path.GetFileNameWithoutExtension(l), "levels", 10);
            
            if (map != null) map = Path.GetFileNameWithoutExtension(map);
            return map;
        }
        
        /// <summary> Find partial matches of 'name' against the list of ranks. </summary>
        public static Group FindRanks(Player p, string name) {
            name = name.ToLower();
            Group.MapName(ref name);
            
            int matches = 0;
            return Find<Group>(p, name, out matches, Group.GroupList,
                               null, g => Colors.StripColors(g.name), "ranks");
        }
        
        
        /// <summary> Finds partial matches of 'name' against the names of the items in the 'items' enumerable. </summary>
        /// <returns> If exactly one match, the matching item. </returns>
        public static T Find<T>(Player p, string name, out int matches, IEnumerable items,
                                Predicate<T> filter, StringFormatter<T> nameGetter, string group, int limit = 5)  {
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
            
            if (matches == 1) return match;
            if (matches == 0) {
                Player.Message(p, "No " + group + " match \"" + name + "\".");
            } else {
                OutputMulti(p, name, nameMatches, matches, group, limit);
            }
            return default(T);
        }
        
        /// <summary> Finds partial matches of 'name' against the names of the items in the 'items' enumerable. </summary>
        /// <remarks> Outputs multiple matching entries, as 'items' enumerable may have multiple entries. </remarks>
        /// <returns> If exactly one match, the matching list of items. </returns>
        public static List<T> FindMulti<T>(Player p, string name, out int matches, IEnumerable items,
                                           Predicate<T> filter, StringFormatter<T> nameGetter, string group, int limit = 5)  {
            List<T> matchItems = null; matches = 0;
            StringBuilder nameMatches = new StringBuilder();
            List<string> outputtedNames = new List<string>(limit);
            string match = null;

            foreach (T item in items) {
                if (filter != null && !filter(item)) continue;
                string itemName = nameGetter(item);
                
                // Found an exact name match - only output items now which exactly match
                if (itemName.Equals(name, comp)) {
                    if (match == null || !name.Equals(match, comp))
                        matchItems = new List<T>();
                    matchItems.Add(item);
                    
                    matches = 1; match = name;
                    continue;
                }
                
                if (itemName.IndexOf(name, comp) < 0) continue;
                if (matches == 0) { // Found our first partial match - init the list
                    matchItems = new List<T>();
                    matchItems.Add(item);
                    match = itemName;
                } else if (match != null && itemName.Equals(match, comp)) { // Found same partial match
                    matchItems.Add(item);
                }
                
                // We do not want to output the same name multiple times
                if (outputtedNames.CaselessContains(itemName) || matches > (limit + 1)) continue;
                matches++;
                
                if (matches <= limit) {
                    nameMatches.Append(itemName).Append(", ");
                } else if (matches == limit + 1) {
                    nameMatches.Append("(and more)").Append(", ");
                }
                outputtedNames.Add(itemName);
            }
            
            if (matches == 1) return matchItems;
            if (matches == 0) {
                Player.Message(p, "No " + group + " found for \"" + name + "\".");
            } else {
                OutputMulti(p, name, nameMatches, matches, "players", limit);
            }
            return null;
        }
        
        static void OutputMulti(Player p, string name, StringBuilder nameMatches,
                                int matches, string group, int limit = 5) {
            string count = matches > limit ? limit + "+ " : matches + " ";
            string names = nameMatches.ToString(0, nameMatches.Length - 2);
            
            Player.Message(p, count + group + " match \"" + name + "\":");
            Player.Message(p, names);
        }
    }
}
