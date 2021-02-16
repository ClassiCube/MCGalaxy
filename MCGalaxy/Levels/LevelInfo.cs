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
using MCGalaxy.Events.LevelEvents;
using MCGalaxy.SQL;

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
        
        public static void Add(Level lvl) {
            Loaded.Add(lvl);
            OnLevelAddedEvent.Call(lvl);
        }
        
        public static void Remove(Level lvl) {
            Loaded.Remove(lvl);
            OnLevelRemovedEvent.Call(lvl);
        }
        
        
        // TODO: support loading other map files eventually
        public static string[] AllMapFiles() {
            return Directory.GetFiles("levels", "*.lvl");
        }
        
        public static string[] AllMapNames() {
            string[] files = AllMapFiles();
            for (int i = 0; i < files.Length; i++) {
                files[i] = Path.GetFileNameWithoutExtension(files[i]);
            }
            return files;
        }
        
        public static bool MapExists(string name) {
            return File.Exists(MapPath(name));
        }
                
        /// <summary> Relative path of a level's map file </summary>
        public static string MapPath(string name) {
            return "levels/" + name.ToLower() + ".lvl";
        }
        

        /// <summary> Relative path of a level's backup folder </summary>
        public static string BackupBasePath(string name) {
            return Server.Config.BackupDirectory + "/" + name;
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
        public static string PropsPath(string name) {
            return "levels/level properties/" + name + ".properties";
        }
        
        public static LevelConfig GetConfig(string map, out Level lvl) {
            lvl = FindExact(map);
            if (lvl != null) return lvl.Config;
            
            string propsPath = PropsPath(map);
            LevelConfig cfg = new LevelConfig();
            cfg.Load(propsPath);
            return cfg;
        }
        
        public static bool Check(Player p, LevelPermission plRank, string map, string action, out LevelConfig cfg) {
            Level lvl; cfg = GetConfig(map, out lvl);
            if (p.IsConsole) return true;
            if (lvl != null) return Check(p, plRank, lvl, action);
            
            AccessController visit = new LevelAccessController(cfg, map, true);
            AccessController build = new LevelAccessController(cfg, map, false);
            if (!visit.CheckDetailed(p, plRank) || !build.CheckDetailed(p, plRank)) {
                p.Message("Hence, you cannot {0}.", action); return false;
            }
            return true;
        }
        
        public static bool Check(Player p, LevelPermission plRank, string map, string action) {
            LevelConfig ignored;
            return Check(p, plRank, map, action, out ignored);
        }
        
        public static bool Check(Player p, LevelPermission plRank, Level lvl, string action) {
            if (p.IsConsole) return true;
            if (!lvl.VisitAccess.CheckDetailed(p, plRank) || !lvl.BuildAccess.CheckDetailed(p, plRank)) {
                p.Message("Hence, you cannot {0}.", action); return false;
            }
            return true;
        }
        
        public static bool ValidName(string map) {
            foreach (char c in map) {
                if (!Database.ValidNameChar(c)) return false;
            }
            return true;
        }
        
        public static bool IsRealmOwner(string name, string map) {
            Level lvl = null;
            LevelConfig cfg = GetConfig(map, out lvl); 
            return IsRealmOwner(map, cfg, name);
        }
        
        public static bool IsRealmOwner(Level lvl, string name) {
            return IsRealmOwner(lvl.name, lvl.Config, name);
        }
        
        public static bool IsRealmOwner(string map, LevelConfig cfg, string name) {
            string[] owners = cfg.RealmOwner.SplitComma();
            if (owners.Length > 0) {
                foreach (string owner in owners) {
                    if (owner.CaselessEq(name)) return true;
                }
                return false;
            }
            
            // For backwards compatibility, treat name+XYZ map names as belonging to name+
            // If no + though, don't use because otherwise people can register accounts and claim maps
            return Server.Config.ClassicubeAccountPlus && map.CaselessStarts(name);
        }
    }
}