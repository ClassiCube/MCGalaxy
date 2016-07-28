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
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using MCGalaxy.BlockPhysics;
using MCGalaxy.Config;
using MCGalaxy.Games;
using MCGalaxy.Generator;
using MCGalaxy.Levels.IO;
using Timer = System.Timers.Timer;

//WARNING! DO NOT CHANGE THE WAY THE LEVEL IS SAVED/LOADED!
//You MUST make it able to save and load as a new version other wise you will make old levels incompatible!

namespace MCGalaxy {
    public enum LevelPermission {
        Banned = -20, Guest = 0, Builder = 30,
        AdvBuilder = 50, Operator = 80,
        Admin = 100, Nobody = 120, Null = 150
    }
	
    public enum BuildType { Normal, ModifyOnly, NoModify };

    public sealed partial class Level : IDisposable {    
    	
        public Level(string n, ushort x, ushort y, ushort z) { Init(n, x, y, z); }
        
        [Obsolete("Use MapGen.Generate instead")]
        public Level(string n, ushort x, ushort y, ushort z, string theme, int seed = 0, bool useSeed = false) {
            Init(n, x, y, z);
            string args = useSeed ? seed.ToString() : "";
            MapGen.Generate(this, theme, args, null);
        }
        
        [Obsolete("Use MapGen.Generate instead")]
        public Level(string n, ushort x, ushort y, ushort z, string theme, string genArgs) {
            Init(n, x, y, z);
            MapGen.Generate(this, theme, genArgs, null);
        }
        
        void Init(string n, ushort x, ushort y, ushort z) {
            Width = x;
            Height = y;
            Length = z;
            if (Width < 16) Width = 16;
            if (Height < 16) Height = 16;
            if (Length < 16) Length = 16;
            width = Width;
            length = Height;
            height = Length; depth = Length;

            CustomBlockDefs = new BlockDefinition[256];
            for (int i = 0; i < CustomBlockDefs.Length; i++)
                CustomBlockDefs[i] = BlockDefinition.GlobalDefs[i];
            name = n;
            EdgeLevel = (short)(y / 2);
            CloudsHeight = (short)(y + 2);
            blocks = new byte[Width * Height * Length];
            ChunksX = (Width + 15) >> 4;
            ChunksY = (Height + 15) >> 4;
            ChunksZ = (Length + 15) >> 4;
            CustomBlocks = new byte[ChunksX * ChunksY * ChunksZ][];
            ZoneList = new List<Zone>();

            spawnx = (ushort)(Width / 2);
            spawny = (ushort)(Height * 0.75f);
            spawnz = (ushort)(Length / 2);
            rotx = 0;
            roty = 0;
            listCheckExists = new SparseBitSet(Width, Height, Length);
            listUpdateExists = new SparseBitSet(Width, Height, Length);
        }

        public List<Player> players { get { return getPlayers(); } }

        public void Dispose() {
            Extras.Clear();
            liquids.Clear();
            leaves.Clear();
            ListCheck.Clear(); listCheckExists.Clear();
            ListUpdate.Clear(); listUpdateExists.Clear();
            UndoBuffer.Clear();
            blockCache.Clear();
            ZoneList.Clear();
            
            lock (queueLock)
                blockqueue.Clear();
            lock (saveLock) {
                blocks = null;
                CustomBlocks = null;
            }
        }
        
        public bool CanUseHacks(Player p) {
             bool serverMotd = p.level == Server.mainLevel || motd == "ignore";
             string realMotd = serverMotd ? Server.motd : motd;
             bool noHacks = realMotd.Contains("-hax") || ctfmode || CurrentGame() != null;
             if (noHacks && p.Rank >= LevelPermission.Operator && realMotd.Contains("+ophax"))
                 return true;
             return noHacks;
        }

        /// <summary> Whether block changes made on this level should be 
        /// saved to the BlockDB and .lvl files. </summary>
        public bool ShouldSaveChanges() {
            if (!saveLevel) return false;
        	if (Server.zombie.Running && !ZombieGame.SaveLevelBlockchanges &&
        	    (name.CaselessEq(Server.zombie.CurLevelName)
        	     || name.CaselessEq(Server.zombie.LastLevelName))) return false;
        	if (Server.lava.active && Server.lava.HasMap(name)) return false;
        	return true;
        }
        
        /// <summary> The currently active game running on this map, 
        /// or null if there is no game running. </summary>
        public IGame CurrentGame() {
            if (Server.zombie.Running && name.CaselessEq(Server.zombie.CurLevelName))
                return Server.zombie;
            if (Server.lava.active && Server.lava.HasMap(name)) 
                return Server.lava;
            return null;
        }
        
        public bool CanJoin(Player p) {
            if (p == null) return true;
            if (Player.BlacklistCheck(p.name, name) || VisitBlacklist.CaselessContains(p.name)) {
                Player.Message(p, "You are blacklisted from going to {0}.", name); return false;
            }
            
            bool whitelisted = VisitWhitelist.CaselessContains(p.name);
            if (!p.ignorePermission && !whitelisted && p.Rank < permissionvisit) {
                Player.Message(p, "You are not allowed to go to {0}.", name); return false;
            }
            if (!p.ignorePermission && !whitelisted && p.Rank > pervisitmax && !p.group.CanExecute("pervisitmax")) {
                Player.Message(p, "Your rank must be ranked {1} or lower to go to {0}.", name, pervisitmax); return false;
            }
            if (File.Exists("text/lockdown/map/" + name)) {
                Player.Message(p, "The level " + name + " is locked."); return false;
            }
            return true;
        }
        
        public bool Unload(bool silent = false, bool save = true) {
            if (Server.mainLevel == this || IsMuseum) return false;
            if (Server.lava.active && Server.lava.map == this) return false;
            if (LevelUnload != null)
                LevelUnload(this);
            OnLevelUnloadEvent.Call(this);
            if (cancelunload) {
                Server.s.Log("Unload canceled by Plugin! (Map: " + name + ")");
                cancelunload = false; return false;
            }
            MovePlayersToMain();

            if (save && changed && ShouldSaveChanges()) Save(false, true);
            if (save && ShouldSaveChanges()) saveChanges();
            
            if (TntWarsGame.Find(this) != null) {
                foreach (TntWarsGame.player pl in TntWarsGame.Find(this).Players) {
                    pl.p.CurrentTntGameNumber = -1;
                    Player.Message(pl.p, "TNT Wars: The TNT Wars game you are currently playing has been deleted!");
                    pl.p.PlayingTntWars = false;
                    pl.p.canBuild = true;
                    TntWarsGame.SetTitlesAndColor(pl, true);
                }
                Server.s.Log("TNT Wars: Game deleted on " + name);
                TntWarsGame.GameList.Remove(TntWarsGame.Find(this));

            }
            MovePlayersToMain();
            LevelInfo.Loaded.Remove(this);

            try {
                PlayerBot.UnloadFromLevel(this);
                //physChecker.Stop();
                //physChecker.Dispose();
                physThread.Abort();
                physThread.Join();

            } catch {
            } finally {
                Dispose();
                GC.Collect();
                GC.WaitForPendingFinalizers();

                if (!silent) Chat.GlobalMessageOps("&3" + name + " %Swas unloaded.");
                Server.s.Log(name + " was unloaded.");
            }
            return true;
        }

        void MovePlayersToMain() {
        	Player[] players = PlayerInfo.Online.Items; 
            foreach (Player p in players) {
                if (p.level == this) {
                    Player.Message(p, "You were moved to the main level as " + name + " was unloaded.");
                    PlayerActions.ChangeMap(p, Server.mainLevel.name);
                }
            }
        }        

        /// <summary> Returns whether the given coordinates are insides the boundaries of this level. </summary>
        public bool InBound(ushort x, ushort y, ushort z) {
            return x >= 0 && y >= 0 && z >= 0 && x < Width && y < Height && z < Length;
        }

        [Obsolete]
        public static Level Find(string name) { return LevelInfo.Find(name); }

        [Obsolete]
        public static Level FindExact(string name) { return LevelInfo.FindExact(name); }

        public static void SaveSettings(Level lvl) {
            lock (lvl.savePropsLock)
            	LvlProperties.Save(lvl, LevelInfo.PropertiesPath(lvl.name));
        }

        // Returns true if ListCheck does not already have an check in the position.
        // Useful for fireworks, which depend on two physics blocks being checked, one with extraInfo.
        public bool CheckClear(ushort x, ushort y, ushort z) {
        	return x >= Width || y >= Height || z >= Length || !listCheckExists.Get(x, y, z);
        }

        public void Save(bool Override = false, bool clearPhysics = false) {
            if (blocks == null) return;
            string path = LevelInfo.LevelPath(name);
            if (LevelSave != null) LevelSave(this);
            OnLevelSaveEvent.Call(this);
            if (cancelsave1) { cancelsave1 = false; return; }
            if (cancelsave) { cancelsave = false; return; }
            
            try {
                if (!Directory.Exists("levels")) Directory.CreateDirectory("levels");
                if (!Directory.Exists("levels/level properties")) Directory.CreateDirectory("levels/level properties");
                if (!Directory.Exists("levels/prev")) Directory.CreateDirectory("levels/prev");
                
                if (changed || !File.Exists(path) || Override || (physicschanged && clearPhysics)) {
                    if (clearPhysics) ClearPhysics();
                    
                    lock (saveLock)
                        SaveCore(path);
                } else {
                    Server.s.Log("Skipping level save for " + name + ".");
                }
            } catch (Exception e) {
                Server.s.Log("FAILED TO SAVE :" + name);
                Player.GlobalMessage("FAILED TO SAVE :" + name);
                Server.ErrorLog(e);
            }
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
        
        void SaveCore(string path) {
            if (blocks == null) return;
            if (File.Exists(path)) {
                string prevPath = LevelInfo.PrevPath(name);
                if (File.Exists(prevPath)) File.Delete(prevPath);
                File.Copy(path, prevPath, true);
                File.Delete(path);
            }
            
            LvlFile.Save(this, path + ".backup");
            File.Copy(path + ".backup", path);
            SaveSettings(this);

            Server.s.Log(string.Format("SAVED: Level \"{0}\". ({1}/{2}/{3})", name, players.Count,
                                       PlayerInfo.Online.Count, Server.players));
            changed = false;
        }

        public int Backup(bool Forced = false, string backupName = "") {
            if (!backedup || Forced) {
                int backupNumber = 1;
                string dir = Path.Combine(Server.backupLocation, name);
                backupNumber = IncrementBackup(dir);

                string path = Path.Combine(dir, backupNumber.ToString());
                if (backupName != "")
                    path = Path.Combine(dir, backupName);
                Directory.CreateDirectory(path);

                string backup = Path.Combine(path, name + ".lvl");
                string current = LevelInfo.LevelPath(name);
                try {
                    File.Copy(current, backup, true);
                    backedup = true;
                    return backupNumber;
                } catch (Exception e) {
                    Server.ErrorLog(e);
                    Server.s.Log("FAILED TO INCREMENTAL BACKUP :" + name);
                    return -1;
                }
            }
            Server.s.Log("Level unchanged, skipping backup");
            return -1;
        }
        
        int IncrementBackup(string dir) {
            if (Directory.Exists(dir)) {
                int max = 0;
                string[] backups = Directory.GetDirectories(dir);
                foreach (string s in backups) {
                    string name = s.Substring(s.LastIndexOf(Path.DirectorySeparatorChar) + 1);
                    int num;
                    
                    if (!int.TryParse(name, out num)) continue;
                    max = Math.Max(num, max);
                }
                return max + 1;
            } else {
                Directory.CreateDirectory(dir);
                return 1;
            }
        }

        public static Level Load(string name) { return Load(name, 0); }

        //givenName is safe against SQL injections, it gets checked in CmdLoad.cs
        public static Level Load(string name, byte phys) {
            if (LevelLoad != null) LevelLoad(name);
            OnLevelLoadEvent.Call(name);
            if (cancelload) { cancelload = false; return null; }
            LevelDB.CreateTables(name);

            string path = LevelInfo.LevelPath(name);
            if (!File.Exists(path)) {
                Server.s.Log("Attempted to load " + name + ", but the level file does not exist.");
                return null;
            }
            
            try {
                Level level = LvlFile.Load(name, path);
                level.setPhysics(phys);
                level.backedup = true;              

                level.jailx = (ushort)(level.spawnx * 32);
                level.jaily = (ushort)(level.spawny * 32);
                level.jailz = (ushort)(level.spawnz * 32);
                level.jailrotx = level.rotx;
                level.jailroty = level.roty;
                level.StartPhysics();
                
                try {
                    string propsPath = LevelInfo.FindPropertiesFile(level.name);
                    if (propsPath != null)
                        LvlProperties.Load(level, propsPath);
                    else
                        Server.s.Log(".properties file for level " + level.name + " was not found.");
                    // Backwards compatibility for older levels which had .env files.
                    LvlProperties.LoadEnv(level);
                } catch (Exception e) {
                    Server.ErrorLog(e);
                }
                
                BlockDefinition[] defs = BlockDefinition.Load(false, level);
                for (int i = 0; i < defs.Length; i++) {
                    if (defs[i] == null) continue;
                    level.CustomBlockDefs[i] = defs[i];
                }
                Bots.BotsFile.LoadBots(level);
                
                object locker = ThreadSafeCache.DBCache.Get(name);
                lock (locker) {
                    LevelDB.LoadZones(level, name);
                    LevelDB.LoadPortals(level, name);
                    LevelDB.LoadMessages(level, name);
                }

                Server.s.Log(string.Format("Level \"{0}\" loaded.", level.name));
                if (LevelLoaded != null)
                    LevelLoaded(level);
                OnLevelLoadedEvent.Call(level);
                return level;
            } catch (Exception ex) {
                Server.ErrorLog(ex);
                return null;
            }
        }

        public static bool CheckLoadOnGoto(string givenName) {
            string value = LevelInfo.FindOfflineProperty(givenName, "loadongoto");
            if (value == null) return true;
            bool load;
            if (!bool.TryParse(value, out load)) return true;
            return load;
        }

        public void ChatLevel(string message) { ChatLevel(message, LevelPermission.Banned); }

        public void ChatLevelOps(string message) { ChatLevel(message, Server.opchatperm); }

        public void ChatLevelAdmins(string message) { ChatLevel(message, Server.adminchatperm); }
        
        /// <summary> Sends a chat messages to all players in the level, who have at least the minPerm rank. </summary>
        public void ChatLevel(string message, LevelPermission minPerm) {
        	Player[] players = PlayerInfo.Online.Items; 
            foreach (Player pl in players) {
            	if (pl.level != this) continue;
            	if (pl.Rank < minPerm) continue;
                pl.SendMessage(message);
            }
        }
        
        public void UpdateBlockPermissions() {
        	Player[] players = PlayerInfo.Online.Items; 
        	foreach (Player p in players) {
        		if (p.level != this) continue;
        		if (!p.HasCpeExt(CpeExt.BlockPermissions)) continue;
        		p.SendCurrentBlockPermissions();
        	}
        }

        public static LevelPermission PermissionFromName(string name) {
            Group foundGroup = Group.Find(name);
            return foundGroup != null ? foundGroup.Permission : LevelPermission.Null;
        }

        public static string PermissionToName(LevelPermission perm) {
            Group foundGroup = Group.findPerm(perm);
            return foundGroup != null ? foundGroup.name : ((int)perm).ToString();
        }
        
        public bool HasPlayers() {
            Player[] players = PlayerInfo.Online.Items; 
            foreach (Player p in players)
                if (p.level == this) return true;
            return false;
        }
        
        readonly object dbLock = new object();
        public void saveChanges() {
            lock (dbLock)
                LevelDB.SaveBlockDB(this); 
        }

        public List<Player> getPlayers() {
            Player[] players = PlayerInfo.Online.Items; 
            return players.Where(p => p.level == this).ToList();
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct BlockPos {
            public string name;
            public int flags, index; // bit 0 = is deleted, bit 1 = is ext, rest bits = time delta
            public byte rawType;
            
            public void SetData(byte type, byte extType, bool delete) {
                TimeSpan delta = DateTime.UtcNow.Subtract(Server.StartTime);
                flags = (int)delta.TotalSeconds << 2;
                flags |= (byte)(delete ? 1 : 0);
                
                if (type == Block.custom_block) {
                    rawType = extType; flags |= 2;
                } else {
                    rawType = type;
                }
            }
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct UndoPos {
            public int flags, index; // bit 0 = is old ext, bit 1 = is new ext, rest bits = time delta
            public byte oldRawType, newRawType;
            
            public void SetData(byte oldType, byte oldExtType, byte newType, byte newExtType) {
                TimeSpan delta = DateTime.UtcNow.Subtract(Server.StartTime);
                flags = (int)delta.TotalSeconds << 2;
                
                if (oldType == Block.custom_block) {
                    oldRawType = oldExtType; flags |= 1;
                } else {
                    oldRawType = oldType;
                }                
                if (newType == Block.custom_block) {
                    newRawType = newExtType; flags |= 2;
                } else {
                    newRawType = newType;
                }
            }
        }

        public struct Zone {
            public string Owner;
            public ushort bigX, bigY, bigZ;
            public ushort smallX, smallY, smallZ;
        }
    }
}