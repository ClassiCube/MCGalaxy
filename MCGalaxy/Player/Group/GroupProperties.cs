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
using System.IO;

namespace MCGalaxy {
    public sealed class GroupProperties {
        
        public static void InitAll() {
            Group temp = null;
            PropertiesFile.Read(Paths.RankPropsFile, ref temp, ParseProperty, '=', false);
            if (temp != null) AddGroup(ref temp);
        }
        
        static void ParseProperty(string key, string value, ref Group temp) {
            string raw = value; // for prefix we need to keep space
            value = value.Trim();
            
            if (key.CaselessEq("rankname")) {
                if (temp != null) AddGroup(ref temp);
                value = value.Replace(" ", "");

                if (value.CaselessEq("op")) {
                    Logger.Log(LogType.Warning, "Cannot have a rank named \"{0}\", this rank is hard-coded.", value);
                } else if (Group.Find(value) == null) {
                    temp = new Group();
                    temp.Name = value;
                } else {
                    Logger.Log(LogType.Warning, "Cannot add the rank {0} twice", value);
                }
                return;
            }
            if (temp == null) return;

            switch (key.ToLower()) {
                case "permission":
                    int perm;
                    
                    if (!int.TryParse(value, out perm)) {
                        Logger.Log(LogType.Warning, "Invalid permission: " + value);
                        temp = null;
                    } if (perm > 120 || perm < -50) {
                        Logger.Log(LogType.Warning, "Permission must be between -50 and 120 for ranks");
                        temp = null;
                    } else if (Group.Find((LevelPermission)perm) == null) {
                        temp.Permission = (LevelPermission)perm;
                    } else {
                        Logger.Log(LogType.Warning, "Cannot have 2 ranks set at permission level " + value);
                        temp = null;
                    } break;
                    
                case "limit":
                    temp.DrawLimit = int.Parse(value);
                    break;
                case "maxundo":
                    temp.MaxUndo = int.Parse(value);
                    break;
                case "genvolume":
                    temp.GenVolume = int.Parse(value);
                    break;
                case "afkkicked":
                    temp.AfkKicked = bool.Parse(value);
                    break;
                case "afkkickminutes":
                    temp.AfkKickMinutes = int.Parse(value);
                    break;
                    
                case "color":
                    char col;
                    char.TryParse(value, out col);

                    if (Colors.IsDefined(col)) {
                        temp.Color = "&" + col;
                    } else {
                        Logger.Log(LogType.Warning, "Invalid color code: " + value);
                        temp = null;
                    } break;
                case "filename":
                    if (value.Contains("\\") || value.Contains("/")) {
                        Logger.Log(LogType.Warning, "Invalid filename: " + value);
                        temp = null;
                    } else {
                        temp.filename = value;
                    } break;
                case "motd":
                    temp.MOTD = value;
                    break;
                case "osmaps":
                    temp.OverseerMaps = byte.Parse(value);
                    break;
                case "prefix":
                    if (!String.IsNullOrEmpty(value))
                        temp.Prefix = raw.TrimStart();
                    
                    if (Colors.Strip(temp.Prefix).Length > 3) {
                        Logger.Log(LogType.Warning, "Prefixes may only consist of color codes and three letters");
                        temp.Prefix = temp.Prefix.Substring(0, 3);
                    }
                    break;
                case "copyslots":
                    temp.CopySlots = byte.Parse(value);
                    break;
            }
        }
        
        static void AddGroup(ref Group temp) {
            Group.Register(temp.CopyConfig());
            temp = null;
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
                w.WriteLine("#GenVolume = num");
                w.WriteLine("#\tThe maximum volume of a map that can be generated by players of this rank.");
                w.WriteLine("#AfkKickMinutes = num");
                w.WriteLine("#\tNumber of minutes a player can be AFK for, before they can be AFK kicked.");
                w.WriteLine();
                w.WriteLine();
                
                foreach (Group grp in givenList) {
                    w.WriteLine("RankName = " + grp.Name);
                    w.WriteLine("Permission = " + (int)grp.Permission);
                    w.WriteLine("Limit = " + grp.DrawLimit);
                    w.WriteLine("MaxUndo = " + grp.MaxUndo);
                    w.WriteLine("Color = " + grp.Color[1]);
                    w.WriteLine("MOTD = " + grp.MOTD);
                    w.WriteLine("FileName = " + grp.filename);
                    w.WriteLine("OSMaps = " + grp.OverseerMaps);
                    w.WriteLine("Prefix = " + grp.Prefix);
                    w.WriteLine("GenVolume = " + grp.GenVolume);
                    w.WriteLine("AfkKicked = " + grp.AfkKicked);
                    w.WriteLine("AfkKickMinutes = " + grp.AfkKickMinutes);
                    w.WriteLine("CopySlots = " + grp.CopySlots);
                    w.WriteLine();
                }
            }
        }
    }
}
