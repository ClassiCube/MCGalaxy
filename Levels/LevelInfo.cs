/*
    Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCGalaxy)
    
    Dual-licensed under the    Educational Community License, Version 2.0 and
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
        
        const StringComparison comp = StringComparison.OrdinalIgnoreCase;
        public static Level Find(string name) {
            Level match = null; int matches = 0;
            
            foreach (Level lvl in Server.levels) {
                if (lvl.name.Equals(name, comp)) return lvl;
                if (lvl.name.IndexOf(name, comp) >= 0) {
                    match = lvl; matches++;
                }
            }
            return matches == 1 ? match : null;
        }

        public static Level FindExact(string name) {
            foreach (Level lvl in Server.levels) {
            	if (lvl.name.Equals(name, comp)) return lvl;
            }
            return null;
        }
		
        public static bool ExistsOffline(string name) {
            return File.Exists("levels/" + name.ToLower() + ".lvl");
        }
        
        public static string FindOfflineProperty(string name, string propKey) {
            string file = "levels/level properties/" + name + ".properties";
            if (!File.Exists(file))
                file = "levels/level properties/" + name;
            if (!File.Exists(file)) return null;

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
                    Server.ErrorLog(e);
                }
            }
            return null;
        }
    }
}