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
using System.Runtime.InteropServices;
using MCGalaxy.Blocks;
using MCGalaxy.Commands;
using MCGalaxy.DB;
using MCGalaxy.Events;
using MCGalaxy.Games;
using MCGalaxy.Generator;
using MCGalaxy.Levels.IO;
using MCGalaxy.Util;

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
            Width = x; Height = y; Length = z;
            if (Width < 16) Width = 16;
            if (Height < 16) Height = 16;
            if (Length < 16) Length = 16;
            
            #pragma warning disable 0612
            width = Width;
            length = Height;
            height = Length; depth = Length;
            #pragma warning restore 0612

            CustomBlockDefs = new BlockDefinition[Block.Count];
            for (int i = 0; i < CustomBlockDefs.Length; i++)
                CustomBlockDefs[i] = BlockDefinition.GlobalDefs[i];
            
            BlockProps = new BlockProps[Block.Count * 2];
            for (int i = 0; i < BlockProps.Length; i++)
                BlockProps[i] = BlockDefinition.GlobalProps[i];
            
            name = n; MapName = n.ToLower();
            BlockDB = new BlockDB(this);
            EdgeLevel = (short)(y / 2);
            CloudsHeight = (short)(y + 2);
            
            blocks = new byte[Width * Height * Length];
            ChunksX = Utils.CeilDiv16(Width);
            ChunksY = Utils.CeilDiv16(Height);
            ChunksZ = Utils.CeilDiv16(Length);
            CustomBlocks = new byte[ChunksX * ChunksY * ChunksZ][];

            spawnx = (ushort)(Width / 2);
            spawny = (ushort)(Height * 0.75f);
            spawnz = (ushort)(Length / 2);
            rotx = 0; roty = 0;
            SetBlockHandlers();
            
            ZoneList = new List<Zone>();
            VisitAccess = new LevelAccessController(this, true);
            BuildAccess = new LevelAccessController(this, false);
            listCheckExists = new SparseBitSet(Width, Height, Length);
            listUpdateExists = new SparseBitSet(Width, Height, Length);
        }

        public List<Player> players { get { return getPlayers(); } }

        public void Dispose() {
            Extras.Clear();
            leaves.Clear();
            ListCheck.Clear(); listCheckExists.Clear();
            ListUpdate.Clear(); listUpdateExists.Clear();
            UndoBuffer.Clear();
            BlockDB.Cache.Clear();
            ZoneList.Clear();
            
            lock (queueLock)
                blockqueue.Clear();
            lock (saveLock) {
                blocks = null;
                CustomBlocks = null;
            }
        }
        
        public string GetMotd(Player p) {
            if (motd != "ignore") return motd;
            return String.IsNullOrEmpty(p.group.MOTD) ? Server.motd : p.group.MOTD;
        }

        /// <summary> Whether block changes made on this level should be
        /// saved to the BlockDB and .lvl files. </summary>
        public bool ShouldSaveChanges() {
            if (!saveLevel) return false;
            ZombieGame zs = Server.zombie;
            
            if (zs.Running && !ZombieGameProps.SaveLevelBlockchanges &&
                (name.CaselessEq(zs.CurLevelName) || name.CaselessEq(zs.LastLevelName)))
                return false;
            if (Server.lava.active && Server.lava.HasMap(name))
                return false;
            return true;
        }
        
        public bool ShouldShowJoinMessage(Level prev) {
            ZombieGame zs = Server.zombie;
            if (zs.Running && name.CaselessEq(zs.CurLevelName) &&
                (prev == this || zs.LastLevelName == "" || prev.name.CaselessEq(zs.LastLevelName)))
                return false;
            if (Server.lava.active && Server.lava.HasMap(name))
                return false;
            return true;
        }
        
        /// <summary> The currently active game running on this map,
        /// or null if there is no game running. </summary>
        public IGame CurrentGame() {
            if (Server.zombie.Running && name.CaselessEq(Server.zombie.CurLevelName))
                return Server.zombie;
            if (Server.lava.active && Server.lava.map == this)
                return Server.lava;
            return null;
        }
        
        public bool CanJoin(Player p, bool ignorePerms = false) {
            if (p == null) return true;
            if (!VisitAccess.CheckDetailed(p, ignorePerms)) return false;
            if (Server.lockdown.Contains(name)) {
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
                physThread.Abort();
                physThread.Join();
            } catch {
            } finally {
                Dispose();
                Server.DoGC();

                if (!silent) Chat.MessageOps(ColoredName + " %Swas unloaded.");
                Server.s.Log(name + " was unloaded.");
            }
            return true;
        }

        void MovePlayersToMain() {
            Player[] players = PlayerInfo.Online.Items;
            foreach (Player p in players) {
                if (p.level == this) {
                    Player.Message(p, "You were moved to the main level as " + ColoredName + " %Swas unloaded.");
                    PlayerActions.ChangeMap(p, Server.mainLevel);
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
            if (lvl.IsMuseum) return; // museums do not save properties
            
            lock (lvl.savePropsLock)
                LvlProperties.Save(lvl, LevelInfo.PropertiesPath(lvl.MapName));
        }

        // Returns true if ListCheck does not already have an check in the position.
        // Useful for fireworks, which depend on two physics blocks being checked, one with extraInfo.
        public bool CheckClear(ushort x, ushort y, ushort z) {
            return x >= Width || y >= Height || z >= Length || !listCheckExists.Get(x, y, z);
        }

        public void Save(bool Override = false, bool clearPhysics = false) {
            if (blocks == null || IsMuseum) return; // museums do not save properties
            
            string path = LevelInfo.MapPath(MapName);
            if (LevelSave != null) LevelSave(this);
            OnLevelSaveEvent.Call(this);
            if (cancelsave1) { cancelsave1 = false; return; }
            if (cancelsave) { cancelsave = false; return; }
            
            try {
                if (!Directory.Exists("levels")) Directory.CreateDirectory("levels");
                if (!Directory.Exists("levels/level properties")) Directory.CreateDirectory("levels/level properties");
                if (!Directory.Exists("levels/prev")) Directory.CreateDirectory("levels/prev");
                
                if (changed || !File.Exists(path) || Override || (physicschanged && clearPhysics)) {
                    lock (saveLock)
                        SaveCore(path);
                    
                    if (clearPhysics) ClearPhysics();
                } else {
                    Server.s.Log("Skipping level save for " + name + ".");
                }
            } catch (Exception e) {
                Server.s.Log("FAILED TO SAVE :" + name);
                Chat.MessageGlobal("FAILED TO SAVE {0}", ColoredName);
                Server.ErrorLog(e);
            }
            Server.DoGC();
        }
        
        void SaveCore(string path) {
            if (blocks == null) return;
            if (File.Exists(path)) {
                string prevPath = LevelInfo.PrevPath(name);
                if (File.Exists(prevPath)) File.Delete(prevPath);
                File.Copy(path, prevPath, true);
                File.Delete(path);
            }
            
            IMapExporter.Formats[0].Write(path + ".backup", this);
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
                string current = LevelInfo.MapPath(name);
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

        public static Level Load(string name) { return Load(name, LevelInfo.MapPath(name)); }

        public static Level Load(string name, string path) {
            if (LevelLoad != null) LevelLoad(name);
            OnLevelLoadEvent.Call(name);
            if (cancelload) { cancelload = false; return null; }

            if (!File.Exists(path)) {
                Server.s.Log("Attempted to load " + name + ", but the level file does not exist.");
                return null;
            }
            
            try {
                Level lvl = IMapImporter.Formats[0].Read(path, name, true);
                lvl.backedup = true;

                lvl.jailx = (ushort)(lvl.spawnx * 32);
                lvl.jaily = (ushort)(lvl.spawny * 32);
                lvl.jailz = (ushort)(lvl.spawnz * 32);
                lvl.jailrotx = lvl.rotx;
                lvl.jailroty = lvl.roty;
                
                LoadMetadata(lvl);
                Bots.BotsFile.LoadBots(lvl);

                object locker = ThreadSafeCache.DBCache.Get(name);
                lock (locker) {
                    LevelDB.LoadZones(lvl, name);
                    LevelDB.LoadPortals(lvl, name);
                    LevelDB.LoadMessages(lvl, name);
                }

                Server.s.Log(string.Format("Level \"{0}\" loaded.", lvl.name));
                if (LevelLoaded != null)
                    LevelLoaded(lvl);
                OnLevelLoadedEvent.Call(lvl);
                return lvl;
            } catch (Exception ex) {
                Server.ErrorLog(ex);
                return null;
            }
        }
        
        public static void LoadMetadata(Level lvl) {
            try {
                string propsPath = LevelInfo.FindPropertiesFile(lvl.MapName);
                if (propsPath != null) {
                    LvlProperties.Load(lvl, propsPath);
                } else {
                    Server.s.Log(".properties file for level " + lvl.MapName + " was not found.");
                }
                
                // Backwards compatibility for older levels which had .env files.
                LvlProperties.LoadEnv(lvl);
            } catch (Exception e) {
                Server.ErrorLog(e);
            }
            lvl.BlockDB.Cache.Enabled = lvl.UseBlockDB;
            
            BlockDefinition[] defs = BlockDefinition.Load(false, lvl);
            for (int i = 0; i < defs.Length; i++) {
                if (defs[i] == null) continue;
                lvl.CustomBlockDefs[i] = defs[i];
            }
            
            for (int i = 0; i < lvl.BlockProps.Length; i++) {
                lvl.BlockProps[i] = BlockDefinition.GlobalProps[i];
            }
            MCGalaxy.Blocks.BlockProps.Load("lvl_" + lvl.MapName, lvl.BlockProps, true);
        }

        public static bool CheckLoadOnGoto(string givenName) {
            string value = LevelInfo.FindOfflineProperty(givenName, "loadongoto");
            if (value == null) return true;
            bool load;
            if (!bool.TryParse(value, out load)) return true;
            return load;
        }

        public void ChatLevel(string message) {
            ChatLevel(message, LevelPermission.Banned);
        }

        public void ChatLevelOps(string message) {
            LevelPermission rank = CommandExtraPerms.MinPerm("opchat", LevelPermission.Operator);
            ChatLevel(message, rank);
        }

        public void ChatLevelAdmins(string message) {
            LevelPermission rank = CommandExtraPerms.MinPerm("adminchat", LevelPermission.Admin);
            ChatLevel(message, rank);
        }
        
        /// <summary> Sends a chat messages to all players in the level, who have at least the minPerm rank. </summary>
        public void ChatLevel(string message, LevelPermission minPerm) {
            Player[] players = PlayerInfo.Online.Items;
            foreach (Player pl in players) {
                if (pl.level != this) continue;
                if (pl.Rank < minPerm) continue;
                
                Player.Message(pl, message);
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
            List<Player> onLevel = new List<Player>();
            
            foreach (Player p in players) {
                if (p.level == this) onLevel.Add(p);
            }
            return onLevel;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct UndoPos {
            public int flags, index; // bit 0 = is old ext, bit 1 = is new ext, rest bits = time delta
            public byte oldRaw, newRaw;
            
            public void SetData(ExtBlock oldBlock, ExtBlock newBlock) {
                TimeSpan delta = DateTime.UtcNow.Subtract(Server.StartTime);
                flags = (int)delta.TotalSeconds << 2;
                
                if (oldBlock.BlockID == Block.custom_block) {
                    oldRaw = oldBlock.ExtID; flags |= 1;
                } else {
                    oldRaw = oldBlock.BlockID;
                }
                
                if (newBlock.BlockID == Block.custom_block) {
                    newRaw = newBlock.ExtID; flags |= 2;
                } else {
                    newRaw = newBlock.BlockID;
                }
            }
        }

        public struct Zone {
            public string Owner;
            public ushort bigX, bigY, bigZ;
            public ushort smallX, smallY, smallZ;
        }
        
        public void SetBlockHandlers() {            
            for (int i = 0; i < Block.Count; i++) {
                SetBlockHandler(new ExtBlock((byte)i, 0));
                SetBlockHandler(new ExtBlock(Block.custom_block, (byte)i));
            }
        }
        
        public void SetBlockHandler(ExtBlock block) {
            bool notCustom = !block.IsCustomType &&
                (block.BlockID >= Block.CpeCount || CustomBlockDefs[block.BlockID] == null);
            
            bool nonSolid;
            if (notCustom) {
                nonSolid = Block.Walkthrough(Block.Convert(block.BlockID));
            } else {
                nonSolid = CustomBlockDefs[block.BlockID].CollideType != CollideType.Solid;
            }
            
            int i = block.Index;
            deleteHandlers[i] = BlockBehaviour.GetDeleteHandler(block, BlockProps);
            placeHandlers[i] = BlockBehaviour.GetPlaceHandler(block, BlockProps);
            walkthroughHandlers[i] = BlockBehaviour.GetWalkthroughHandler(block, BlockProps, nonSolid);
            physicsHandlers[i] = BlockBehaviour.GetPhysicsHandler(block, BlockProps);
            physicsDoorsHandlers[i] = BlockBehaviour.GetPhysicsDoorsHandler(block, BlockProps);
        }
    }
}