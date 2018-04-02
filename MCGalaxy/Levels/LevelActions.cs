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
using MCGalaxy.DB;
using MCGalaxy.SQL;
using MCGalaxy.Util;

namespace MCGalaxy {
    
    public static class LevelActions {
        
        /// <summary> Renames the .lvl (and related) files and database tables. Does not unload. </summary>
        public static void Rename(string src, string dst) {
            File.Move(LevelInfo.MapPath(src), LevelInfo.MapPath(dst));
            DoAll(src, dst, action_move);
            
            // TODO: Should we move backups still
            try {
                //MoveBackups(src, dst);
            } catch {
            }
            
            RenameDatabaseTables(src, dst);
            BlockDBFile.MoveBackingFile(src, dst);
        }
        
        static void RenameDatabaseTables(string src, string dst) {
            if (Database.Backend.TableExists("Block" + src))
                Database.Backend.RenameTable("Block" + src, "Block" + dst);
            object srcLocker = ThreadSafeCache.DBCache.GetLocker(src);
            object dstLockder = ThreadSafeCache.DBCache.GetLocker(dst);
            
            lock (srcLocker)
                lock (dstLockder)
            {
                if (Database.TableExists("Portals" + src)) {
                    Database.Backend.RenameTable("Portals" + src, "Portals" + dst);
                    Database.Backend.UpdateRows("Portals" + dst, "ExitMap = @1",
                                                "WHERE ExitMap = @0", src, dst);
                }
                
                if (Database.TableExists("Messages" + src)) {
                    Database.Backend.RenameTable("Messages" + src, "Messages" + dst);
                }
                if (Database.TableExists("Zone" + src)) {
                    Database.Backend.RenameTable("Zone" + src, "Zone" + dst);
                }
            }
        }
        
        /*static void MoveBackups(string src, string dst) {
            string srcBase = LevelInfo.BackupBasePath(src);
            string dstBase = LevelInfo.BackupBasePath(dst);
            if (!Directory.Exists(srcBase)) return;
            Directory.CreateDirectory(dstBase);
            
            string[] backups = Directory.GetDirectories(srcBase);
            for (int i = 0; i < backups.Length; i++) {
                string name = LevelInfo.BackupNameFrom(backups[i]);
                string srcFile = LevelInfo.BackupFilePath(src, name);
                string dstFile = LevelInfo.BackupFilePath(dst, name);
                string dstDir = LevelInfo.BackupDirPath(dst, name);
                
                Directory.CreateDirectory(dstDir);
                File.Move(srcFile, dstFile);
                Directory.Delete(backups[i]);
            }
            Directory.Delete(srcBase);
        }*/
        
        
        public const string DeleteFailedMessage = "Unable to delete the level, because it could not be unloaded. A game may currently be running on it.";
        /// <summary> Deletes the .lvl (and related) files and database tables. Unloads level if it is loaded. </summary>
        public static bool Delete(string name) {
            Level lvl = LevelInfo.FindExact(name);
            if (lvl != null && !lvl.Unload()) return false;
            
            if (!Directory.Exists("levels/deleted"))
                Directory.CreateDirectory("levels/deleted");
            
            if (File.Exists(LevelInfo.DeletedPath(name))) {
                int num = 0;
                while (File.Exists(LevelInfo.DeletedPath(name + num))) num++;

                File.Move(LevelInfo.MapPath(name), LevelInfo.DeletedPath(name + num));
            } else {
                File.Move(LevelInfo.MapPath(name), LevelInfo.DeletedPath(name));
            }

            DoAll(name, "", action_delete);
            DeleteDatabaseTables(name);
            BlockDBFile.DeleteBackingFile(name);
            return true;
        }
        
        static void DeleteDatabaseTables(string name) {
            if (Database.Backend.TableExists("Block" + name))
                Database.Backend.DeleteTable("Block" + name);
            
            object locker = ThreadSafeCache.DBCache.GetLocker(name);
            lock (locker) {
                if (Database.TableExists("Portals" + name)) {
                    Database.Backend.DeleteTable("Portals" + name);
                }
                if (Database.TableExists("Messages" + name)) {
                    Database.Backend.DeleteTable("Messages" + name);
                }
                if (Database.TableExists("Zone" + name)) {
                    Database.Backend.DeleteTable("Zone" + name);
                }
            }
        }
        
        
        public static void Replace(Level old, Level lvl) {
            LevelDB.SaveBlockDB(old);
            LevelInfo.Remove(old);
            LevelInfo.Add(lvl);
            
            old.SetPhysics(0);
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
        
        public static void CopyLevel(string src, string dst) {
            File.Copy(LevelInfo.MapPath(src), LevelInfo.MapPath(dst));
            DoAll(src, dst, action_copy);
            CopyDatabaseTables(src, dst);
        }
        
        static void CopyDatabaseTables(string src, string dst) {
            object srcLocker = ThreadSafeCache.DBCache.GetLocker(src);
            object dstLockder = ThreadSafeCache.DBCache.GetLocker(dst);
            
            lock (srcLocker)
                lock (dstLockder)
            {
                if (Database.TableExists("Portals" + src)) {
                    Database.Backend.CreateTable("Portals" + dst, LevelDB.createPortals);
                    Database.Backend.CopyAllRows("Portals" + src, "Portals" + dst);
                    Database.Backend.UpdateRows("Portals" + dst, "ExitMap = @1",
                                                "WHERE ExitMap = @0", src, dst);
                }
                
                if (Database.TableExists("Messages" + src)) {
                    Database.Backend.CreateTable("Messages" + dst, LevelDB.createMessages);
                    Database.Backend.CopyAllRows("Messages" + src, "Messages" + dst);
                }
            }
        }

        
        public static void ReloadMap(Player p, Player who, bool showMessage) {
            who.Loading = true;
            Entities.DespawnEntities(who);
            who.SendMap(who.level);
            Entities.SpawnEntities(who);
            who.Loading = false;
            if (!showMessage) return;
            
            if (p == null || !Entities.CanSee(who, p)) {
                who.SendMessage("&bMap reloaded");
            } else {
                who.SendMessage("&bMap reloaded by " + p.ColoredName);
            }
            if (Entities.CanSee(p, who)) {
                Player.Message(p, "&4Finished reloading for " + who.ColoredName);
            }
        }
        
        const byte action_delete = 0;
        const byte action_move = 1;
        const byte action_copy = 2;
        
        static void DoAll(string src, string dst, byte action) {
            DoAction(LevelInfo.MapPath(src) + ".backup",
                     LevelInfo.MapPath(dst) + ".backup", action);
            DoAction("levels/level properties/" + src + ".properties",
                     "levels/level properties/" + dst + ".properties", action);
            DoAction("levels/level properties/" + src,
                     "levels/level properties/" + dst + ".properties", action);
            DoAction("blockdefs/lvl_" + src + ".json",
                     "blockdefs/lvl_" + dst + ".json", action);
            DoAction("blockprops/lvl_" + src + ".txt",
                     "blockprops/lvl_" + dst + ".txt", action);
            DoAction("blockprops/_" + src + ".txt", "" +
                     "blockprops/_" + dst + ".txt", action);
            DoAction(BotsFile.BotsPath(src),
                     BotsFile.BotsPath(dst), action);
        }
        
        static void DoAction(string src, string dst, byte action) {
            if (!File.Exists(src)) return;
            try {
                if (action == action_delete) {
                    File.Delete(src);
                } else if (action == action_move) {
                    File.Move(src, dst);
                } else if (action == action_copy) {
                    File.Copy(src, dst, true);
                }
            } catch (Exception ex) {
                Logger.LogError(ex);
            }
        }
    }
}
