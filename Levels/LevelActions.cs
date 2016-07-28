/*
    Copyright 2011 MCForge
        
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
using MCGalaxy.Bots;
using MCGalaxy.SQL;

namespace MCGalaxy {
    
    public static class LevelActions {
        
        /// <summary> Renames the .lvl (and related) files and database tables. 
        /// Does not perform any unloading. </summary>
        public static void Rename(string src, string dst) {
            File.Move(LevelInfo.LevelPath(src), LevelInfo.LevelPath(dst));
            try {
                File.Move(LevelInfo.LevelPath(src) + ".backup", LevelInfo.LevelPath(dst) + ".backup");
            } catch {
            }
            
            try {
                File.Move("levels/level properties/" + src + ".properties", "levels/level properties/" + dst + ".properties");
            } catch {
            }
            
            try {
                File.Move("levels/level properties/" + src, "levels/level properties/" + dst + ".properties");
            } catch {
            }
            
            try {
                if (File.Exists("blockdefs/lvl_" + src + ".json"))
                    File.Move("blockdefs/lvl_" + src + ".json", "blockdefs/lvl_" + dst + ".json");
            } catch {
            }

            try {
                MoveBackups(src, dst);
            } catch {
            }
            BotsFile.MoveBots(src, dst);

            //safe against SQL injections because foundLevel is being checked and,
            //newName is being split and partly checked on illegal characters reserved for Windows.
            if (Server.useMySQL)
                Database.Execute(String.Format("RENAME TABLE `Block{0}` TO `Block{1}`, " +
                                               "`Portals{0}` TO `Portals{1}`, " +
                                               "`Messages{0}` TO `Messages{1}`, " +
                                               "`Zone{0}` TO `Zone{1}`", src, dst));
            else {
                using (BulkTransaction helper = SQLiteBulkTransaction.Create()) { // ensures that it's either all work, or none work.
                    helper.Execute(String.Format("ALTER TABLE `Block{0}` RENAME TO `Block{1}`", src, dst));
                    helper.Execute(String.Format("ALTER TABLE `Portals{0}` RENAME TO `Portals{1}`", src, dst));
                    helper.Execute(String.Format("ALTER TABLE `Messages{0}` RENAME TO `Messages{1}`", src, dst));
                    helper.Execute(String.Format("ALTER TABLE `Zone{0}` RENAME TO `Zone{1}`", src, dst));
                    helper.Commit();
                }
            }
        }
        
        static bool DirectoryEmpty(string dir) {
            if (!Directory.Exists(dir))  return true;
            if (Directory.GetDirectories(dir).Length > 0) return false;
            if (Directory.GetFiles(dir).Length > 0) return false;
            return true;
        }
        
        static void MoveBackups(string src, string dst) {
            for (int i = 1; ; i++) {
                string oldDir = LevelInfo.BackupPath(src, i.ToString());
                string newDir = LevelInfo.BackupPath(dst, i.ToString());

                if (File.Exists(oldDir + src + ".lvl")) {
                    Directory.CreateDirectory(newDir);
                    File.Move(oldDir + src + ".lvl", newDir + dst + ".lvl");
                    if (DirectoryEmpty(oldDir)) Directory.Delete(oldDir);
                } else {
                    if (DirectoryEmpty(Server.backupLocation + "/" + src + "/"))
                        Directory.Delete(Server.backupLocation + "/" + src + "/");
                    break;
                }
            }
        }
        
        
        /// <summary> Deletes the .lvl (and related) files and database tables. 
        /// Unloads a level (if present) which exactly matches name. </summary>
        public static void Delete(string name) {
            Level lvl = LevelInfo.FindExact(name);
            if (lvl != null) lvl.Unload();
            
            if (!Directory.Exists("levels/deleted")) 
                Directory.CreateDirectory("levels/deleted");
            
            if (File.Exists("levels/deleted/" + name + ".lvl")) {
                int num = 0;
                while (File.Exists("levels/deleted/" + name + num + ".lvl")) num++;

                File.Move(LevelInfo.LevelPath(name), "levels/deleted/" + name + num + ".lvl");
            } else {
                File.Move(LevelInfo.LevelPath(name), "levels/deleted/" + name + ".lvl");
            }

            try { File.Delete("levels/level properties/" + name + ".properties"); } catch { }
            try { File.Delete("levels/level properties/" + name); } catch { }
            try {
                if (File.Exists("blockdefs/lvl_" + name + ".json"))
                    File.Delete("blockdefs/lvl_" + name + ".json");
            } catch {}
            BotsFile.DeleteBots(name);

            //safe against SQL injections because the levelname (message) is first being checked if it exists
            Database.Execute("DROP TABLE `Block" + name + "`");
            object locker = ThreadSafeCache.DBCache.Get(name);
            lock (locker) {
                Database.Execute("DROP TABLE `Portals" + name + "`");
                Database.Execute("DROP TABLE `Messages" + name + "`");
                Database.Execute("DROP TABLE `Zone" + name + "`");
            }
        }
        
        public static void Replace(Level old, Level lvl) {
            LevelInfo.Loaded.Remove(old);
            LevelInfo.Loaded.Add(lvl);
            
            old.setPhysics(0);
            old.ClearPhysics();
            lvl.StartPhysics();
            
            Player[] players = PlayerInfo.Online.Items;
            foreach (Player pl in players) {
                if (pl.level != old) continue;
                pl.level = lvl;
                ReloadMap(null, pl, false);
            }
            
            old.Unload(true, false);
            if (old == Server.mainLevel)
                Server.mainLevel = lvl;
        }
        
        public static void ReloadMap(Player p, Player who, bool showMessage) {
            who.Loading = true;
            Entities.DespawnEntities(who);
            who.SendUserMOTD(); who.SendMap(who.level);
            Entities.SpawnEntities(who);
            who.Loading = false;

            if (!showMessage) return;
            if (p != null && !p.hidden) { who.SendMessage("&bMap reloaded by " + p.name); }
            if (p != null && p.hidden) { who.SendMessage("&bMap reloaded"); }
            Player.Message(p, "&4Finished reloading for " + who.name);
        }
    }
}
