/*
    Copyright 2010 MCLawl Team -
    Created by Snowl (David D.) and Cazzar (Cayde D.)

    Dual-licensed under the Educational Community License, Version 2.0 and
    the GNU General Public License, Version 3 (the "Licenses"); you may
    not use this file except in compliance with the Licenses. You may
    obtain a copy of the Licenses at
    
    http://www.osedu.org/licenses/ECL-2.0
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

namespace MCGalaxy.Games.ZS {
    internal class ZSLevelPicker : LevelPicker { 
        
        public override List<string> GetCandidateLevels() {
            List<string> maps = null;
            if (ZSConfig.LevelList.Count > 0) {
                maps = new List<string>(ZSConfig.LevelList);
            } else {
                maps = AllMaps();
            }
            
            foreach (string ignore in ZSConfig.IgnoredLevelList)
                maps.Remove(ignore);
            
            bool useLevelList = ZSConfig.LevelList.Count > 0;
            if (ZSConfig.ChangeLevels && maps.Count <= 3) {
                string group = useLevelList ? "in your level list " : "";
                Logger.Log(LogType.Warning, "You must have more than 3 levels {0}to change levels in Zombie Survival", group);
                return null;
            } else if (!ZSConfig.ChangeLevels && maps.Count == 0) {
                string group = useLevelList ? "in your level list " : "";
                Logger.Log(LogType.Warning, "You must have at least 1 level {0}to play Zombie Survival", group);
                return null;
            }
            return maps;
        }
        
        /// <summary> Returns a list of all possible maps (exclusing personal realms if 'ignore realms' setting is true) </summary>
        static List<string> AllMaps() {
            List<string> maps = new List<string>();
            string[] files = LevelInfo.AllMapFiles();
            
            foreach (string file in files) {
                string map = Path.GetFileNameWithoutExtension(file);
                if (map.IndexOf('+') >= 0 && ZSConfig.IgnorePersonalWorlds) continue;
                maps.Add(map);
            }
            return maps;
        }
    }
}
