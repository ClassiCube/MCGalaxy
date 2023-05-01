/*
    Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCForge)
    
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
using System.IO;
using System.Text;
using MCGalaxy.DB;
using MCGalaxy.Events.LevelEvents;
using MCGalaxy.SQL;

namespace MCGalaxy 
{
    public static class LevelOperations
    {
        public static void OutputBackups(Player p, Level lvl) {
            OutputBackups(p, lvl.MapName, lvl.Config);
        }
        
        public static void OutputBackups(Player p, string map, LevelConfig cfg) {
            map = map.ToLower();
            string root = LevelInfo.BackupBasePath(map);
            string name = cfg.Color + map;
            
            if (!Directory.Exists(root)) {
                p.Message(name + " &Shas no backups yet"); return;
            }

            string[] backups = Directory.GetDirectories(root);
            p.Message(name + " &Shas &b" + backups.Length + " &Sbackups");
            int count = 0;
            StringBuilder custom = new StringBuilder();

            foreach (string path in backups) 
            {
                string restore = LevelInfo.BackupNameFrom(path);
                int num;
                if (int.TryParse(restore, out num)) continue;

                count++;
                custom.Append(", " + restore);
            }

            if (count == 0) return;
            p.Message("&b" + count + " &Sof these are custom-named restores:");
            p.Message(custom.ToString(2, custom.Length - 2));
        }
        
        
        public static bool Backup(Player p, Level lvl, string backup) {
            string map = lvl.name;
            bool auto  = backup.Length == 0;
            if (auto) backup = LevelInfo.NextBackup(map);
            
            TimeSpan delta = lvl.lastBackup - DateTime.UtcNow;
            if (delta.TotalSeconds >= 0) {
                p.Message("You must wait another {0} to backup {1} &Sagain",
                          delta.Shorten(true, true), lvl.ColoredName);
                return false;
            }
            lvl.lastBackup = DateTime.UtcNow.AddSeconds(10);
            
            if (!LevelActions.Backup(map, backup)) {
                p.Message("&WFailed to backup {0}", lvl.ColoredName);
                return false;
            }
            
            if (auto) {
                Logger.Log(LogType.SystemActivity, "Backup {1} saved for {0}", map, backup);
                lvl.Message("Backup " + backup + " saved for " + lvl.ColoredName);
            } else {
                Logger.Log(LogType.SystemActivity, "{0} had a backup created named &b{1}", map, backup);
                lvl.Message(lvl.ColoredName + " &Shad a backup created named &b" + backup);
            }
            return true;
        }
    }
}