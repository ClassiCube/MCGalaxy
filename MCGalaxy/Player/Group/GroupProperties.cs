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
using System.Globalization;
using System.IO;

namespace MCGalaxy {
    public sealed class GroupProperties {
        
        public static void InitAll() {
            Group grp = null;
            PropertiesFile.Read(Paths.RankPropsFile, ref grp, ParseProperty, '=', false);
            if (grp != null) AddGroup(ref grp);
        }
        
        static void ParseProperty(string key, string value, ref Group grp) {
            string raw = value; // for prefix we need to keep space
            value = value.Trim();
            
            if (key.ToLower() == "rankname") {
                if (grp != null) AddGroup(ref grp);
                string name = value.ToLower();

                if (name == "adv" || name == "op" || name == "super" || name == "nobody" || name == "noone") {
                    Server.s.Log("Cannot have a rank named \"" + name + "\", this rank is hard-coded.");
                } else if (Group.GroupList.Find(g => g.name == name) == null) {
                    grp = new Group();
                    grp.trueName = value;
                } else {
                    Server.s.Log("Cannot add the rank " + value + " twice");
                }
                return;
            }
            if (grp == null) return;

            switch (key.ToLower()) {
                case "permission":
                    int perm;
                    
                    if (!int.TryParse(value, out perm)) {
                        Server.s.Log("Invalid permission: " + value);
                        grp = null;
                    } if (perm > 119 || perm < -50) {
                        Server.s.Log("Permission must be between -50 and 119 for ranks");
                        grp = null;
                    } else if (Group.GroupList.Find(g => g.Permission == (LevelPermission)perm) == null) {
                        grp.Permission = (LevelPermission)perm;
                    } else {
                        Server.s.Log("Cannot have 2 ranks set at permission level " + value);
                        grp = null;
                    } break;
                case "limit":
                    grp.maxBlocks = int.Parse(value);
                    break;
                case "maxundo":
                    grp.maxUndo = int.Parse(value);
                    break;
                case "color":
                    char col;
                    char.TryParse(value, out col);

                    if (Colors.IsStandardColor(col) || Colors.GetFallback(col) != '\0') {
                        grp.color = col.ToString(CultureInfo.InvariantCulture);
                    } else {
                        Server.s.Log("Invalid color code: " + value);
                        grp = null;
                    } break;
                case "filename":
                    if (value.Contains("\\") || value.Contains("/")) {
                        Server.s.Log("Invalid filename: " + value);
                        grp = null;
                    } else {
                        grp.fileName = value;
                    } break;
                case "motd":
                    grp.MOTD = value;
                    break;
                case "osmaps":
                    byte osmaps;
                    if (!byte.TryParse(value, out osmaps))
                        osmaps = 3;
                    grp.OverseerMaps = osmaps;
                    break;
                case "prefix":
                    if (!String.IsNullOrEmpty(value))
                        grp.prefix = raw.TrimStart();
                    break;
            }
        }
        
        static void AddGroup(ref Group grp) {
            Group.AddAndLoadGroup(
                new Group(grp.Permission, grp.maxBlocks, grp.maxUndo, grp.trueName,
                          grp.color[0], grp.MOTD, grp.fileName, grp.OverseerMaps, grp.prefix));
            grp = null;
        }
        
        /// <summary> Save givenList group </summary>
        /// <param name="givenList">The list of groups to save</param>
        public static void SaveGroups(List<Group> givenList) {
            using (StreamWriter w = new StreamWriter(Paths.RankPropsFile)) {
                w.WriteLine("#Version 3");
                w.WriteLine("#RankName = string");
                w.WriteLine("#\tThe name of the rank, use capitalization.");
                w.WriteLine("#");
                w.WriteLine("#Permission = num");
                w.WriteLine("#\tThe \"permission\" of the rank. It's a number.");
                w.WriteLine("#\tThere are pre-defined permissions already set. (for the old ranks)");
                w.WriteLine("#\t\tBanned = -20, Guest = 0, Builder = 30, AdvBuilder = 50, Operator = 80");
                w.WriteLine("#\t\tSuperOP = 100, Nobody = 120");
                w.WriteLine("#\tMust be greater than -50 and less than 120");
                w.WriteLine("#\tThe higher the number, the more commands do (such as undo allowing more seconds)");
                w.WriteLine("#Limit = num");
                w.WriteLine("#\tThe command limit for the rank (can be changed in-game with /limit)");
                w.WriteLine("#\tMust be greater than 0 and less than 10000000");
                w.WriteLine("#MaxUndo = num");
                w.WriteLine("#\tThe undo limit for the rank, only applies when undoing others.");
                w.WriteLine("#\tMust be greater than 0 and less than " + int.MaxValue);
                w.WriteLine("#Color = char");
                w.WriteLine("#\tA single letter or number denoting the color of the rank");
                w.WriteLine("#\tPossibilities: 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, a, b, c, d, e, f");
                w.WriteLine("#FileName = string.txt");
                w.WriteLine("#\tThe file which players of this rank will be stored in");
                w.WriteLine("#\t\tIt doesn't need to be a .txt file, but you may as well");
                w.WriteLine("#\t\tGenerally a good idea to just use the same file name as the rank name");
                w.WriteLine("#MOTD = string");
                w.WriteLine("#\tAlternate MOTD players of the rank will see when joining the server.");
                w.WriteLine("#\tLeave blank to use the server MOTD.");
                w.WriteLine("#OSMaps = num");
                w.WriteLine("#\tThe number of maps the players will have in /os");
                w.WriteLine("#\tDefaults to 2 if invalid number (number has to be between 0-128");
                w.WriteLine("#Prefix = string");
                w.WriteLine("#\tCharacters that appear directly before a player's name in chat.");
                w.WriteLine("#\tLeave blank to have no characters before the names of players.");                
                w.WriteLine();
                w.WriteLine();
                
                foreach (Group grp in givenList) {
                    if (grp.name == "nobody") continue;
                    
                    w.WriteLine("RankName = " + grp.trueName);
                    w.WriteLine("Permission = " + (int)grp.Permission);
                    w.WriteLine("Limit = " + grp.maxBlocks);
                    w.WriteLine("MaxUndo = " + grp.maxUndo);
                    w.WriteLine("Color = " + grp.color[1]);
                    w.WriteLine("MOTD = " + grp.MOTD);
                    w.WriteLine("FileName = " + grp.fileName);
                    w.WriteLine("OSMaps = " + grp.OverseerMaps);
                    w.WriteLine("Prefix = " + grp.prefix);
                    w.WriteLine();
                }
            }
        }
    }
}
