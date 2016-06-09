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
            string[] lines = File.ReadAllLines("properties/ranks.properties");

            Group grp = null;
            int version = 1;
            if (lines.Length > 0 && lines[0].StartsWith("#Version ")) {
                try { version = int.Parse(lines[0].Remove(0, 9)); }
                catch { Server.s.Log("The ranks.properties version header is invalid! Ranks may fail to load!"); }
            }

            foreach (string s in lines) {
                try {
                    if (s == "" || s[0] == '#') continue;
                    string[] parts = s.Split('=');
                    if (parts.Length != 2) {
                        Server.s.Log("In ranks.properties, the line " + s + " is wrongly formatted"); continue;
                    }
                    ParseProperty(parts[0].Trim(), parts[1].Trim(), s, ref grp, version);
                } catch (Exception e) {
                    Server.s.Log("Encountered an error at line \"" + s + "\" in ranks.properties"); Server.ErrorLog(e);
                }
            }            
            if (grp != null)
                AddGroup(ref grp, version);
        }
        
        static void ParseProperty(string key, string value, string s, ref Group grp, int version) {
            if (key.ToLower() == "rankname") {
                if (grp != null)
                    AddGroup(ref grp, version);
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
                        Server.s.Log("Invalid permission on " + s);
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
                    int foundLimit;
                    
                    if (!int.TryParse(value, out foundLimit)) {
                        Server.s.Log("Invalid limit on " + s);
                    } else {
                        grp.maxBlocks = foundLimit;
                    } break;
                case "maxundo":
                    int foundMax;
                    
                    if (!int.TryParse(value, out foundMax)) {
                        Server.s.Log("Invalid max undo on " + s);
                        grp = null;
                    } else {
                        grp.maxUndo = foundMax;
                    } break;
                case "color":
                    char col;
                    char.TryParse(value, out col);

                    if (Colors.IsStandardColor(col) || Colors.GetFallback(col) != '\0') {
                        grp.color = col.ToString(CultureInfo.InvariantCulture);
                    } else {
                        Server.s.Log("Invalid color code at " + s);
                        grp = null;
                    } break;
                case "filename":
                    if (value.Contains("\\") || value.Contains("/")) {                        
                        Server.s.Log("Invalid filename on " + s);
                        grp = null;
                    } else {
                        grp.fileName = value;
                    } break;
                case "motd":
                    if (!String.IsNullOrEmpty(value))
                        grp.MOTD = value;
                    break;
                case "osmaps":
                    byte osmaps;
                    if (!byte.TryParse(value, out osmaps))
                        osmaps = 3;
                    grp.OverseerMaps = osmaps;
                    break;
                case "prefix":
                    grp.prefix = CP437Reader.ConvertToRaw(value);
                    break;
                    
            }
        }
        
        static void AddGroup(ref Group grp, int version) {
            if (version < 2) {
                if ((int)grp.Permission >= 100)
                    grp.maxUndo = int.MaxValue;
                else if ((int)grp.Permission >= 80)
                    grp.maxUndo = 5400;
            }

            Group.GroupList.Add(
                new Group(grp.Permission, grp.maxBlocks, grp.maxUndo, grp.trueName,
                          grp.color[0], grp.MOTD, grp.fileName, grp.OverseerMaps, grp.prefix));
            grp = null;
        }
        
        /// <summary> Save givenList group </summary>
        /// <param name="givenList">The list of groups to save</param>
        public static void SaveGroups(List<Group> givenList) {
            File.Create("properties/ranks.properties").Dispose();
            using (StreamWriter SW = File.CreateText("properties/ranks.properties"))
            {
                SW.WriteLine("#Version 3");
                SW.WriteLine("#RankName = string");
                SW.WriteLine("#\tThe name of the rank, use capitalization.");
                SW.WriteLine("#");
                SW.WriteLine("#Permission = num");
                SW.WriteLine("#\tThe \"permission\" of the rank. It's a number.");
                SW.WriteLine("#\tThere are pre-defined permissions already set. (for the old ranks)");
                SW.WriteLine("#\t\tBanned = -20, Guest = 0, Builder = 30, AdvBuilder = 50, Operator = 80");
                SW.WriteLine("#\t\tSuperOP = 100, Nobody = 120");
                SW.WriteLine("#\tMust be greater than -50 and less than 120");
                SW.WriteLine("#\tThe higher the number, the more commands do (such as undo allowing more seconds)");
                SW.WriteLine("#Limit = num");
                SW.WriteLine("#\tThe command limit for the rank (can be changed in-game with /limit)");
                SW.WriteLine("#\tMust be greater than 0 and less than 10000000");
                SW.WriteLine("#MaxUndo = num");
                SW.WriteLine("#\tThe undo limit for the rank, only applies when undoing others.");
                SW.WriteLine("#\tMust be greater than 0 and less than " + int.MaxValue);
                SW.WriteLine("#Color = char");
                SW.WriteLine("#\tA single letter or number denoting the color of the rank");
                SW.WriteLine("#\tPossibilities: 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, a, b, c, d, e, f");
                SW.WriteLine("#FileName = string.txt");
                SW.WriteLine("#\tThe file which players of this rank will be stored in");
                SW.WriteLine("#\t\tIt doesn't need to be a .txt file, but you may as well");
                SW.WriteLine("#\t\tGenerally a good idea to just use the same file name as the rank name");
                SW.WriteLine("#MOTD = string");
                SW.WriteLine("#\tAlternate MOTD players of the rank will see when joining the server.");
                SW.WriteLine("#\tLeave blank to use the server MOTD.");
                SW.WriteLine("#OSMaps = num");
                SW.WriteLine("#\tThe number of maps the players will have in /os");
                SW.WriteLine("#\tDefaults to 2 if invalid number (number has to be between 0-128");
                SW.WriteLine("#Prefix = string");
                SW.WriteLine("#\tCharacters that appear directly before a player's name in chat.");
                SW.WriteLine("#\tLeave blank to have no characters before the names of players.");                
                SW.WriteLine();
                SW.WriteLine();
                
                foreach (Group grp in givenList) {
                    if (grp.name == "nobody") continue;
                    
                    SW.WriteLine("RankName = " + grp.trueName);
                    SW.WriteLine("Permission = " + (int)grp.Permission);
                    SW.WriteLine("Limit = " + grp.maxBlocks);
                    SW.WriteLine("MaxUndo = " + grp.maxUndo);
                    SW.WriteLine("Color = " + grp.color[1]);
                    SW.WriteLine("MOTD = " + grp.MOTD);
                    SW.WriteLine("FileName = " + grp.fileName);
                    SW.WriteLine("OSMaps = " + grp.OverseerMaps);
                    SW.WriteLine("Prefix = " + CP437Writer.ConvertToUnicode(grp.prefix));
                    SW.WriteLine();
                }
            }
        }
    }
}
