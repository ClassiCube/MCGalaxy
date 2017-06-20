﻿/*
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
        
        [Obsolete("Prefer Matcher.FindLevels() or FindExact()")]
        public static Level Find(string name) {
            Level match = null; int matches = 0;
            Level[] loaded = Loaded.Items;
            
            foreach (Level lvl in loaded) {
                if (lvl.name.CaselessEq(name)) return lvl;
                if (lvl.name.CaselessContains(name)) {
                    match = lvl; matches++;
                }
            }
            return matches == 1 ? match : null;
        }

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
            return File.Exists(BackupPath(name, backup));
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
        
        /// <summary> Relative path of a level's backup map file </summary>
        public static string BackupPath(string name, string backup) {
            return Server.backupLocation + "/" + name + "/" + backup + "/" + name + ".lvl";
        }
        
        /// <summary> Relative path of a level's property file </summary>
        public static string PropertiesPath(string name) {
            return "levels/level properties/" + name + ".properties";
        }
        
        
        public static string FindPropertiesFile(string name) {
            string file = "levels/level properties/" + name + ".properties";
            if (!File.Exists(file)) file = "levels/level properties/" + name;
            return File.Exists(file) ? file : null;
        }      
        
        public static string FindOfflineProperty(string name, string propKey) {
            string file = FindPropertiesFile(name);
            if (file == null) return null;

            string[] lines = null;
            try {
                lines = File.ReadAllLines(file);
            } catch {
                return null;
            }
            
            foreach (string line in lines) {
                try {
                    if (line == "" || line[0] == '#') continue;
                    int index = line.IndexOf(" = ");
                    if (index == -1) continue;
                    
                    string key = line.Substring(0, index).ToLower();
                    if (key == propKey) return line.Substring(index + 3);
                } catch (Exception e) {
                    Logger.LogError(e);
                }
            }
            return null;
        }
    }
}