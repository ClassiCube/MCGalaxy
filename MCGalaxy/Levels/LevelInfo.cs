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

    public static class LevelInfo {
        
        /// <summary> Array of all current loaded levels. </summary>
        /// <remarks> Note this field is highly volatile, you should cache references to the items array. </remarks>
        public static VolatileArray<Level> Loaded = new VolatileArray<Level>(true);

        public static Level FindExact(string name) {
            Level[] loaded = Loaded.Items;
            foreach (Level lvl in loaded) {
                if (lvl.name.CaselessEq(name)) return lvl;
            }
            return null;
        }
        
        
        // TODO: support loading other map files eventually
        public static string[] AllMapFiles() {
            return Directory.GetFiles("levels", "*.lvl");
        }       
        
        public static bool MapExists(string name) {
            return File.Exists(MapPath(name));
        }
        
        public static bool ExistsBackup(string name, string backup) {
            return File.Exists(BackupFilePath(name, backup));
        }
        
        
        /// <summary> Relative path of a deleted level's map file </summary>
        public static string DeletedPath(string name) {
            return "levels/deleted/" + name + ".lvl";
        }
        
        /// <summary> Relative path of a level's map file </summary>
        public static string MapPath(string name) {
            return "levels/" + name.ToLower() + ".lvl";
        }
        
        /// <summary> Relative path of a level's previous save map file </summary>
        public static string PrevPath(string name) {
            return "levels/prev/" + name.ToLower() + ".lvl.prev";
        }
        

        /// <summary> Relative path of a level's backup folder </summary>
        public static string BackupBasePath(string name) {
            return ServerConfig.BackupDirectory + "/" + name;
        }
        
        /// <summary> Relative path of a level's backup map directory </summary>
        public static string BackupDirPath(string name, string backup) {
            return BackupBasePath(name) + "/" + backup;
        }
        
        /// <summary> Relative path of a level's backup map file </summary>
        public static string BackupFilePath(string name, string backup) {
            return BackupDirPath(name, backup) + "/" + name + ".lvl";
        }
        
        public static string BackupNameFrom(string path) {
            return path.Substring(path.LastIndexOf(Path.DirectorySeparatorChar) + 1);
        }
        
        public static int LatestBackup(string name) {
            string dir = BackupBasePath(name);
            string[] backups = Directory.GetDirectories(dir);
            int latest = 0;
            
            foreach (string path in backups) {
            	string backupName = BackupNameFrom(path);
                int num;
                
                if (!int.TryParse(backupName, out num)) continue;
                latest = Math.Max(num, latest);
            }
            return latest;
        }
               
        
        /// <summary> Relative path of a level's property file </summary>
        public static string PropertiesPath(string name) {
            return "levels/level properties/" + name + ".properties";
        }
        
        internal static LevelConfig GetConfig(string map, out Level lvl) {
            lvl = FindExact(map);
            if (lvl != null) return lvl.Config;
            
            string propsPath = PropertiesPath(map);
            LevelConfig cfg = new LevelConfig();
            LevelConfig.Load(propsPath, cfg); 
            return cfg;
        }
        
        internal static bool ValidateAction(Player p, string map, string action) {
            if (p == null) return true;
            LevelAccessController visit, build;
            Level lvl = null;
            LevelConfig cfg = GetConfig(map, out lvl); 
            
            if (lvl != null) {
                visit = lvl.VisitAccess;
                build = lvl.BuildAccess;
            } else {
                visit = new LevelAccessController(cfg, map, true);
                build = new LevelAccessController(cfg, map, false);
            }
            
            if (!visit.CheckDetailed(p) || !build.CheckDetailed(p)) {
                Player.Message(p, "Hence, you cannot {0}.", action);
                return false;
            }
            return true;
        }
    }
}