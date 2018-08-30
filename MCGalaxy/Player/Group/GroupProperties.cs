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
        
        static ConfigElement[] cfg;
        public static void InitAll() {
            Group temp = null;
            if (cfg == null) cfg = ConfigElement.GetAll(typeof(Group));
            
            PropertiesFile.Read(Paths.RankPropsFile, ref temp, ParseProperty, '=', false);
            if (temp != null) AddGroup(ref temp);
        }
        
        static void ParseProperty(string key, string value, ref Group temp) {
            if (key.CaselessEq("RankName")) {
                if (temp != null) AddGroup(ref temp);
                temp = null;
                value = value.Replace(" ", "");

                if (value.CaselessEq("op")) {
                    Logger.Log(LogType.Warning, "Cannot have a rank named \"{0}\", this rank is hard-coded.", value);
                } else if (Group.Find(value) == null) {
                    temp = new Group();
                    temp.Name = value;
                } else {
                    Logger.Log(LogType.Warning, "Cannot add the rank {0} twice", value);
                }
            } else {
                if (temp == null) return;
                // for prefix we need to keep space at end
                if (!key.CaselessEq("Prefix")) {
                    value = value.Trim();
                } else {
                    value = value.TrimStart();
                }
                
                ConfigElement.Parse(cfg, temp, key, value);
            }
        }
        
        static void AddGroup(ref Group temp) {
            if (Group.Find(temp.Permission) != null) {
                Logger.Log(LogType.Warning, "Cannot have 2 ranks set at permission level " + (int)temp.Permission);
            } else if (temp.Permission == LevelPermission.Null) {
                Logger.Log(LogType.Warning, "Invalid permission level for rank " + temp.Name);
            } else {
                Group.Register(temp);
            }
            temp = null;
        }
        
        public static void SaveGroups(List<Group> givenList) {
            if (cfg == null) cfg = ConfigElement.GetAll(typeof(Group));
            
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
                w.WriteLine("#\tMust be greater than 0");
                w.WriteLine("#MaxUndo = num");
                w.WriteLine("#\tThe undo limit for the rank, only applies when undoing others.");
                w.WriteLine("#\tMust be greater than 0");
                w.WriteLine("#Color = char");
                w.WriteLine("#\tA single letter (a-f) or number (0-9) denoting the color of the rank");
                w.WriteLine("#MOTD = string");
                w.WriteLine("#\tAlternate MOTD players of the rank will see when joining the server.");
                w.WriteLine("#\tLeave blank to use the server MOTD.");
                w.WriteLine("#OSMaps = num");
                w.WriteLine("#\tThe number of maps the players will have in /os");
                w.WriteLine("#Prefix = string");
                w.WriteLine("#\tCharacters that appear directly before a player's name in chat.");
                w.WriteLine("#\tLeave blank to have no characters before the names of players.");
                w.WriteLine("#GenVolume = num");
                w.WriteLine("#\tThe maximum volume of a map that can be generated by players of this rank.");
                w.WriteLine("#AfkKickMinutes = num");
                w.WriteLine("#\tNumber of minutes a player can be AFK for, before they can be AFK kicked.");
                w.WriteLine();
                w.WriteLine();
                
                foreach (Group group in givenList) {
                    w.WriteLine("RankName = " + group.Name);
                    foreach (ConfigElement elem in cfg) {
                        w.WriteLine(elem.Format(group));
                    }
                    w.WriteLine();
                }
            }
        }
    }
}
