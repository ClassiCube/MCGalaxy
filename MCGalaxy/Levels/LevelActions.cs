﻿/*
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
using System.Collections.Generic;
using System.IO;
using MCGalaxy.Blocks;
using MCGalaxy.Blocks.Extended;
using MCGalaxy.Bots;
using MCGalaxy.DB;
using MCGalaxy.Events.LevelEvents;
using MCGalaxy.Levels.IO;
using MCGalaxy.SQL;
using MCGalaxy.Util;

namespace MCGalaxy {
    
    public static class LevelActions {
        
        static string BlockPropsLvlPath(string map) { return Paths.BlockPropsPath("_" + map); }
        static string BlockPropsOldPath(string map) { return Paths.BlockPropsPath("lvl_" + map); }
        
        public static bool Backup(string map, string backupName) {
            string basePath = LevelInfo.BackupBasePath(map);
            UnsafeIO.CreateDirectory(basePath);
            string path = Path.Combine(basePath, backupName);
            UnsafeIO.CreateDirectory(path);
            
            bool lvl    = DoAction(LevelInfo.MapPath(map),   Path.Combine(path, map + ".lvl"),     action_copy);
            bool props  = DoAction(LevelInfo.PropsPath(map), Path.Combine(path, "map.properties"), action_copy);
            bool defs   = DoAction(Paths.MapBlockDefs(map),  Path.Combine(path, "blockdefs.json"), action_copy);
            bool blkOld = DoAction(BlockPropsOldPath(map),   Path.Combine(path, "blockprops.txt"), action_copy);
            bool blkCur = DoAction(BlockPropsLvlPath(map),   Path.Combine(path, "blockprops.txt"), action_copy);
            bool bots   = DoAction(Paths.BotsPath(map),      Path.Combine(path, "bots.json"),      action_copy);
            
            return lvl && props && defs && blkOld && blkCur && bots;
        }
        
        
        /// <summary> Renames the given map and associated metadata. Does not unload. </summary>
        /// <remarks> Backups are NOT renamed. </remarks>
        public static bool Rename(Player p, string src, string dst) {
            if (LevelInfo.MapExists(dst)) { 
                p.Message("&WLevel \"{0}\" already exists.", dst); return false;
            }
            
            Level lvl = LevelInfo.FindExact(src);
            if (lvl == Server.mainLevel) {
                p.Message("Cannot rename the main level."); return false;
            }
            
            List<Player> players = null;
            if (lvl != null) players = lvl.getPlayers();
            
            if (lvl != null && !lvl.Unload()) {
                p.Message("Unable to rename the level, because it could not be unloaded. " +
                          "A game may currently be running on it.");
                return false;
            }
            
            File.Move(LevelInfo.MapPath(src), LevelInfo.MapPath(dst));
            DoAll(src, dst, action_move);
            
            // TODO: Should we move backups still
            try {
                //MoveBackups(src, dst);
            } catch {
            }
            
            RenameDatabaseTables(p, src, dst);
            BlockDBFile.MoveBackingFile(src, dst);
            OnLevelRenamedEvent.Call(src, dst);
            if (players == null) return true;
            
            // Move all the old players to the renamed map
            Load(p, dst, false);
            foreach (Player pl in players)
                PlayerActions.ChangeMap(pl, dst);
            return true;
        }
        
        static void RenameDatabaseTables(Player p, string src, string dst) {
            if (Database.TableExists("Block" + src)) {
                Database.RenameTable("Block" + src, "Block" + dst);
            }
            object srcLocker = ThreadSafeCache.DBCache.GetLocker(src);
            object dstLocker = ThreadSafeCache.DBCache.GetLocker(dst);
            
            lock (srcLocker)
                lock (dstLocker)
            {
                Portal.MoveAll(src, dst);
                MessageBlock.MoveAll(src, dst);
                
                if (Database.TableExists("Zone" + src)) {
                    Database.RenameTable("Zone" + src, "Zone" + dst);
                }
            }
            
            p.Message("Updating portals that go to {0}..", src);
            List<string> tables = Database.Backend.AllTables();
            foreach (string table in tables) {
                if (!table.StartsWith("Portals")) continue;
                
                Database.UpdateRows(table, "ExitMap=@1", 
                                    "WHERE ExitMap=@0", src, dst);
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
        
        
        /// <summary> Deletes a map and associated metadata. </summary>
        public static bool Delete(Player p, string map) {
            Level lvl = LevelInfo.FindExact(map);
            if (lvl == Server.mainLevel) {
                p.Message("Cannot delete the main level."); return false;
            }
            
            if (lvl != null && !lvl.Unload()) {
                p.Message("Unable to delete the level, because it could not be unloaded. " +
                          "A game may currently be running on it.");
                return false;
            }
            
            p.Message("Created backup.");
            UnsafeIO.CreateDirectory("levels/deleted");
            
            if (File.Exists(Paths.DeletedMapFile(map))) {
                int num = 0;
                while (File.Exists(Paths.DeletedMapFile(map + num))) num++;

                File.Move(LevelInfo.MapPath(map), Paths.DeletedMapFile(map + num));
            } else {
                File.Move(LevelInfo.MapPath(map), Paths.DeletedMapFile(map));
            }

            DoAll(map, "", action_delete);
            DeleteDatabaseTables(map);
            BlockDBFile.DeleteBackingFile(map);
            OnLevelDeletedEvent.Call(map);
            return true;
        }
        
        static void DeleteDatabaseTables(string map) {
            if (Database.TableExists("Block" + map)) {
                Database.DeleteTable("Block" + map);
            }
            
            object locker = ThreadSafeCache.DBCache.GetLocker(map);
            lock (locker) {
                Portal.DeleteAll(map);
                MessageBlock.DeleteAll(map);
                
                if (Database.TableExists("Zone" + map)) {
                    Database.DeleteTable("Zone" + map);
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
                PlayerActions.ReloadMap(pl);
            }
            
            old.Unload(true, false);
            if (old == Server.mainLevel)
                Server.mainLevel = lvl;
        }
        
        
        /// <summary> Copies a map and related metadata. </summary>
        /// <remarks> Backups and BlockDB are NOT copied. </remarks>
        public static bool Copy(Player p, string src, string dst) {
            if (LevelInfo.MapExists(dst)) { 
                p.Message("&WLevel \"{0}\" already exists.", dst); return false;
            }
            
            // Make sure any changes to live map are saved first
            Level lvl = LevelInfo.FindExact(src);
            if (lvl != null && !lvl.Save(true)) {
                p.Message("&WUnable to save {0}! Some recent block changes may not be copied.", src);
            }
            
            File.Copy(LevelInfo.MapPath(src), LevelInfo.MapPath(dst));
            DoAll(src, dst, action_copy);
            CopyDatabaseTables(src, dst);
            OnLevelCopiedEvent.Call(src, dst);
            return true;
        }
        
        static void CopyDatabaseTables(string src, string dst) {
            object srcLocker = ThreadSafeCache.DBCache.GetLocker(src);
            object dstLocker = ThreadSafeCache.DBCache.GetLocker(dst);
            
            lock (srcLocker)
                lock (dstLocker)
            {
                Portal.CopyAll(src, dst);
                MessageBlock.CopyAll(src, dst);
            }
        }

        
        public static void ReloadAll(Level lvl, Player src, bool announce) {
            Player[] players = PlayerInfo.Online.Items;
            foreach (Player p in players) {
                if (p.level != lvl) continue;
                PlayerActions.ReloadMap(p);
                if (!announce) continue;
                
                if (src == null || !p.CanSee(src)) {
                    p.Message("&bMap reloaded");
                } else {
                    p.Message("&bMap reloaded by " + p.FormatNick(src));
                }
                if (src.CanSee(p)) {
                    src.Message("&4Finished reloading for " + src.FormatNick(p));
                }
            }
        }
        
        const byte action_delete = 0;
        const byte action_move = 1;
        const byte action_copy = 2;
        
        static void DoAll(string src, string dst, byte action) {
            DoAction(LevelInfo.MapPath(src) + ".backup",
                     LevelInfo.MapPath(dst) + ".backup", action);
            DoAction(LevelInfo.PropsPath(src),
                     LevelInfo.PropsPath(dst), action);
            DoAction("levels/level properties/" + src,
                     LevelInfo.PropsPath(dst), action);
            DoAction(Paths.MapBlockDefs(src),
                     Paths.MapBlockDefs(dst), action);
            DoAction(BlockPropsOldPath(src),
                     BlockPropsOldPath(dst), action);
            DoAction(BlockPropsLvlPath(src),
                     BlockPropsLvlPath(dst), action);
            DoAction(Paths.BotsPath(src),
                     Paths.BotsPath(dst), action);
        }
        
        static bool DoAction(string src, string dst, byte action) {
            if (!File.Exists(src)) return true;
            try {
                if (action == action_delete) {
                    File.Delete(src);
                } else if (action == action_move) {
                    File.Move(src, dst);
                } else if (action == action_copy) {
                    File.Copy(src, dst, true);
                }
                return true;
            } catch (Exception ex) {
                Logger.LogError(ex);
                return false;
            }
        }
        
        
        public static Level Load(Player p, string map, bool announce) {
            map = map.ToLower();
            Level cur = LevelInfo.FindExact(map);
            if (cur != null) {
                p.Message("&WLevel {0} &Wis already loaded.", cur.ColoredName); return null;
            }
            
            try {
                Level lvl = ReadLevel(p, map);
                if (lvl == null || !lvl.CanJoin(p)) return null;

                cur = LevelInfo.FindExact(map);
                if (cur != null) {
                    p.Message("&WLevel {0} &Wis already loaded.", cur.ColoredName); return null;
                }

                LevelInfo.Add(lvl);
                if (!announce) return lvl;
                
                string autoloadMsg = "Level " + lvl.ColoredName + " &Sloaded.";
                Chat.Message(ChatScope.All, autoloadMsg, null, Chat.FilterVisible(p));
                return lvl;
            } finally {
                Server.DoGC();
            }
        }
        
        static Level ReadBackup(Player p, string map, string path, string type) {
            Logger.Log(LogType.Warning, "Attempting to load {1} for {0}", map, type);
            Level lvl = Level.Load(map, path);
            
            if (lvl != null) return lvl;
            p.Message("&WLoading {1} of {0} failed.", map, type);
            return null;
        }
        
        static Level ReadLevel(Player p, string map) {
            Level lvl = Level.Load(map);
            if (lvl != null) return lvl;
            
            string path = LevelInfo.MapPath(map) + ".backup";
            if (!File.Exists(path)) { p.Message("Level \"{0}\" does not exist", map); return lvl; }
            lvl = ReadBackup(p, map, path, "backup copy");
            if (lvl != null) return lvl;
            
            path = Paths.PrevMapFile(map);
            lvl  = ReadBackup(p, map, path, "previous save");
            if (lvl != null) return lvl;
            
            string backupDir = LevelInfo.BackupBasePath(map);
            if (Directory.Exists(backupDir)) {
                int latest = LevelInfo.LatestBackup(map);
                path = LevelInfo.BackupFilePath(map, latest.ToString());
                lvl = ReadBackup(p, map, path, "latest backup");
            } else {
                p.Message("&WLatest backup of {0} does not exist.", map);
            }
            return lvl;
        }
        
        
        public static void Resize(ref Level lvl, int width, int height, int length) {
            Level res = new Level(lvl.name, (ushort)width, (ushort)height, (ushort)length);
            res.hasPortals       = lvl.hasPortals;
            res.hasMessageBlocks = lvl.hasMessageBlocks;
            byte[] src = lvl.blocks, dst = res.blocks;
            
            // Copy blocks in bulk
            width  = Math.Min(lvl.Width,  res.Width);
            height = Math.Min(lvl.Height, res.Height);
            length = Math.Min(lvl.Length, res.Length);
            for (int y = 0; y < height; y++) {
                for (int z = 0; z < length; z++) {
                    int srcI = lvl.Width * (z + y * lvl.Length);
                    int dstI = res.Width * (z + y * res.Length);
                    Buffer.BlockCopy(src, srcI, dst, dstI, width);
                }
            }
            
            // Copy extended blocks in bulk
            width  = Math.Min(lvl.ChunksX, res.ChunksX);
            height = Math.Min(lvl.ChunksY, res.ChunksY);
            length = Math.Min(lvl.ChunksZ, res.ChunksZ);
            for (int cy = 0; cy < height; cy++)
                for (int cz = 0; cz < length; cz++)
                    for (int cx = 0; cx < width; cx++)
            {
                src = lvl.CustomBlocks[(cy * lvl.ChunksZ + cz) * lvl.ChunksX + cx];
                if (src == null) continue;
                
                dst = new byte[16 * 16 * 16];
                res.CustomBlocks[(cy * res.ChunksZ + cz) * res.ChunksX + cx] = dst;
                Buffer.BlockCopy(src, 0, dst, 0, 16 * 16 * 16);
            }
            
            // TODO: This copying is really ugly and probably not 100% right
            res.spawnx = lvl.spawnx; res.spawny = lvl.spawny; res.spawnz = lvl.spawnz;
            res.rotx = lvl.rotx; res.roty = lvl.roty;
            
            lock (lvl.saveLock) {
                lvl.Backup(true);
                
                // Make sure zones are kept
                res.Zones = lvl.Zones;
                lvl.Zones = new VolatileArray<Zone>(false);
            
                IMapExporter.Formats[0].Write(LevelInfo.MapPath(lvl.name), res);
                lvl.SaveChanges = false;
            }
            
            res.backedup = true;
            Level.LoadMetadata(res);
            BotsFile.Load(res);
            
            LevelActions.Replace(lvl, res);
            lvl = res;
        }
    }
}
