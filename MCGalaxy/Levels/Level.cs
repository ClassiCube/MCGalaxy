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
using MCGalaxy.Bots;
using MCGalaxy.Commands;
using MCGalaxy.DB;
using MCGalaxy.Events.LevelEvents;
using MCGalaxy.Games;
using MCGalaxy.Generator;
using MCGalaxy.Levels.IO;
using MCGalaxy.Util;
using BlockID = System.UInt16;
using BlockRaw = System.Byte;

namespace MCGalaxy {
    public enum LevelPermission {
        Banned = -20, Guest = 0, Builder = 30,
        AdvBuilder = 50, Operator = 80,
        Admin = 100, Nobody = 120, Null = 150
    }
    
    public enum BuildType { Normal, ModifyOnly, NoModify };

    public sealed partial class Level : IDisposable {
        
        public Level(string name, ushort width, ushort height, ushort length) {
            if (width  < 1) width  = 1;
            if (height < 1) height = 1;
            if (length < 1) length = 1;
            
            Width = width; Height = height; Length = length;
            for (int i = 0; i < CustomBlockDefs.Length; i++) {
                CustomBlockDefs[i] = BlockDefinition.GlobalDefs[i];
            }
            
            LoadDefaultProps();
            for (int i = 0; i < blockAABBs.Length; i++) {
                blockAABBs[i] = Block.BlockAABB((ushort)i, this);
            }
            UpdateBlockHandlers();
            
            this.name = name; MapName = name.ToLower();
            BlockDB = new BlockDB(this);
            Config.SetDefaults(height);
            
            blocks = new byte[Width * Height * Length];
            ChunksX = Utils.CeilDiv16(Width);
            ChunksY = Utils.CeilDiv16(Height);
            ChunksZ = Utils.CeilDiv16(Length);
            CustomBlocks = new byte[ChunksX * ChunksY * ChunksZ][];

            spawnx = (ushort)(Width / 2);
            spawny = (ushort)(Height * 0.75f);
            spawnz = (ushort)(Length / 2);
            rotx = 0; roty = 0;
            
            VisitAccess = new LevelAccessController(Config, name, true);
            BuildAccess = new LevelAccessController(Config, name, false);
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
            Zones.Clear();
            
            blockqueue.ClearAll();
            lock (saveLock) {
                blocks = null;
                CustomBlocks = null;
            }
        }
        
        public string GetMotd(Player p) {
            Zone zone = p.ZoneIn;
            string zoneMOTD = zone == null ? null : zone.Config.MOTD;
            if (zoneMOTD != null && zoneMOTD != "ignore") return zoneMOTD;
            
            if (Config.MOTD != "ignore") return Config.MOTD;
            return String.IsNullOrEmpty(p.group.MOTD) ? ServerConfig.MOTD : p.group.MOTD;
        }
        
        public Zone FindZoneExact(string name) {
            Zone[] zones = Zones.Items;
            foreach (Zone zone in zones) {
                if (zone.Config.Name.CaselessEq(name)) return zone;
            }
            return null;
        }
        
        public bool CanJoin(Player p) {
            if (p.IsConsole) return true;
            
            bool skip = p.summonedMap != null && p.summonedMap.CaselessEq(name);
            LevelPermission plRank = skip ? LevelPermission.Nobody : p.Rank;
            if (!VisitAccess.CheckDetailed(p, plRank)) return false;
            
            if (Server.lockdown.Contains(name)) {
                p.Message("The level " + name + " is locked."); return false;
            }
            return true;
        }
        
        public bool AutoUnload() {
            return ServerConfig.AutoLoadMaps && Config.AutoUnload
                && !IsMuseum && !HasPlayers() && Unload(true);
        }
        
        public bool Unload(bool silent = false, bool save = true) {
            if (Server.mainLevel == this || IsMuseum) return false;
            OnLevelUnloadEvent.Call(this);
            if (cancelunload) {
                Logger.Log(LogType.SystemActivity, "Unload canceled by Plugin! (Map: {0})", name);
                cancelunload = false; return false;
            }
            MovePlayersToMain();

            if (save && SaveChanges && Changed) Save(false, true);
            if (save && SaveChanges) SaveBlockDBChanges();
            
            MovePlayersToMain();
            LevelInfo.Remove(this);
            
            try {
                if (!unloadedBots) {
                    unloadedBots = true;
                    BotsFile.Save(this);
                    PlayerBot.RemoveLoadedBots(this, false);
                }
            } catch (Exception ex) { 
                Logger.LogError("Error saving bots", ex);
            }

            try {
                physThread.Abort();
                physThread.Join();
            } catch {
            }
            
            Dispose();
            Server.DoGC();

            if (!silent) Chat.MessageOps(ColoredName + " %Swas unloaded.");
            Logger.Log(LogType.SystemActivity, name + " was unloaded.");
            return true;
        }

        void MovePlayersToMain() {
            Player[] players = PlayerInfo.Online.Items;
            foreach (Player p in players) {
                if (p.level == this) {
                    p.Message("You were moved to the main level as " + ColoredName + " %Swas unloaded.");
                    PlayerActions.ChangeMap(p, Server.mainLevel);
                }
            }
        }

        /// <summary> Returns whether the given coordinates are insides the boundaries of this level. </summary>
        public bool InBound(ushort x, ushort y, ushort z) {
            return x >= 0 && y >= 0 && z >= 0 && x < Width && y < Height && z < Length;
        }

        public static void SaveSettings(Level lvl) {
            if (!lvl.IsMuseum) lvl.Config.SaveFor(lvl.MapName);
        }

        // Returns true if ListCheck does not already have an check in the position.
        // Useful for fireworks, which depend on two physics blocks being checked, one with extraInfo.
        public bool CheckClear(ushort x, ushort y, ushort z) {
            return x >= Width || y >= Height || z >= Length || !listCheckExists.Get(x, y, z);
        }

        public bool Save(bool force = false, bool clearPhysics = false) {
            if (blocks == null || IsMuseum) return false; // museums do not save properties
            
            string path = LevelInfo.MapPath(MapName);
            OnLevelSaveEvent.Call(this);
            if (cancelsave) { cancelsave = false; return false; }
            
            try {
                if (!Directory.Exists("levels")) Directory.CreateDirectory("levels");
                if (!Directory.Exists("levels/level properties")) Directory.CreateDirectory("levels/level properties");
                if (!Directory.Exists("levels/prev")) Directory.CreateDirectory("levels/prev");
                
                if (Changed || !File.Exists(path) || force || (physicschanged && clearPhysics)) {
                    lock (saveLock) SaveCore(path);
                    if (clearPhysics) ClearPhysics();
                } else {
                    Logger.Log(LogType.SystemActivity, "Skipping level save for " + name + ".");
                }
            } catch (Exception e) {
                Logger.Log(LogType.Warning, "FAILED TO SAVE :" + name);
                Chat.MessageGlobal("FAILED TO SAVE {0}", ColoredName);
                Logger.LogError(e);
                return false;
            }
            Server.DoGC();
            return true;
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

            Logger.Log(LogType.SystemActivity, "SAVED: Level \"{0}\". ({1}/{2}/{3})", 
                       name, players.Count, PlayerInfo.Online.Count, ServerConfig.MaxPlayers);
            Changed = false;
        }

        public int Backup(bool Forced = false, string backupName = "") {
            if (!backedup || Forced) {
                string backupPath = LevelInfo.BackupBasePath(name);
                if (!Directory.Exists(backupPath)) Directory.CreateDirectory(backupPath);                
                int next = LevelInfo.LatestBackup(name) + 1;
                if (backupName.Length == 0) backupName = next.ToString();

                if (!LevelActions.Backup(name, backupName)) {
                    Logger.Log(LogType.Warning, "FAILED TO INCREMENTAL BACKUP :" + name);
                    return -1;
                }
                return next;
            }
            Logger.Log(LogType.SystemActivity, "Level unchanged, skipping backup");
            return -1;
        }

        public static Level Load(string name) { return Load(name, LevelInfo.MapPath(name)); }

        public static Level Load(string name, string path) {
            OnLevelLoadEvent.Call(name);
            if (cancelload) { cancelload = false; return null; }

            if (!File.Exists(path)) {
                Logger.Log(LogType.Warning, "Attempted to load {0}, but the level file does not exist.", name);
                return null;
            }
            
            try {
                Level lvl = IMapImporter.Formats[0].Read(path, name, true);
                lvl.backedup = true;
                LoadMetadata(lvl);
                BotsFile.Load(lvl);

                object locker = ThreadSafeCache.DBCache.GetLocker(name);
                lock (locker) {
                    LevelDB.LoadZones(lvl, name);
                    LevelDB.LoadPortals(lvl, name);
                    LevelDB.LoadMessages(lvl, name);
                }

                Logger.Log(LogType.SystemActivity, "Level \"{0}\" loaded.", lvl.name);
                OnLevelLoadedEvent.Call(lvl);
                return lvl;
            } catch (Exception ex) {
                Logger.LogError("Error loading map from " + path, ex);
                return null;
            }
        }
        
        public static void LoadMetadata(Level lvl) {
            lvl.Config.JailX = (ushort)(lvl.spawnx * 32);
            lvl.Config.JailY = (ushort)(lvl.spawny * 32);
            lvl.Config.JailZ = (ushort)(lvl.spawnz * 32);
            lvl.Config.jailrotx = lvl.rotx;
            lvl.Config.jailroty = lvl.roty;
                
            try {
                string propsPath = LevelInfo.PropsPath(lvl.MapName);
                bool propsExisted = lvl.Config.Load(propsPath);
                
                if (propsExisted) {
                    lvl.SetPhysics(lvl.Config.Physics);
                } else {
                    Logger.Log(LogType.ConsoleMessage, ".properties file for level {0} was not found.", lvl.MapName);
                }
                
                // Backwards compatibility for older levels which had .env files.
                string envPath = "levels/level properties/" + lvl.MapName + ".env";
                lvl.Config.Load(envPath);
            } catch (Exception e) {
                Logger.LogError(e);
            }
            lvl.BlockDB.Cache.Enabled = lvl.Config.UseBlockDB;
            
            BlockDefinition[] defs = BlockDefinition.Load(false, lvl.MapName);
            for (int b = 0; b < defs.Length; b++) {
                if (defs[b] == null) continue;
                lvl.UpdateCustomBlock((BlockID)b, defs[b]);
            }
            
            lvl.UpdateBlockProps();
            lvl.UpdateBlockHandlers();
        }

        public void Message(string message) {
            Chat.Message(ChatScope.Level, message, this, null);
        }
        
        public void UpdateBlockPermissions() {
            Player[] players = PlayerInfo.Online.Items;
            foreach (Player p in players) {
                if (p.level != this) continue;
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
        public void SaveBlockDBChanges() {
            lock (dbLock) LevelDB.SaveBlockDB(this);
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
            public int Index; 
            int flags;
            BlockRaw oldRaw, newRaw;
            
            public BlockID OldBlock { 
                get { return (BlockID)(oldRaw | ((flags & 0x03)       << Block.ExtendedShift)); }
            }
            public BlockID NewBlock { 
                get { return (BlockID)(newRaw | (((flags & 0xC >> 2)) << Block.ExtendedShift)); }
            }
            public DateTime Time { 
                get { return Server.StartTime.AddTicks((flags >> 4) * TimeSpan.TicksPerSecond); } 
            }
            
            public void SetData(BlockID oldBlock, BlockID newBlock) {
                TimeSpan delta = DateTime.UtcNow.Subtract(Server.StartTime);
                flags = (int)delta.TotalSeconds << 4;
                
                oldRaw = (BlockRaw)oldBlock; flags |= (oldBlock >> Block.ExtendedShift);
                newRaw = (BlockRaw)newBlock; flags |= (newBlock >> Block.ExtendedShift) << 2;
            }
        }
        
        void LoadDefaultProps() {
            for (int b = 0; b < Props.Length; b++) {
                Props[b] = BlockOptions.DefaultProps(Props, this, (BlockID)b);
            }
        }
        
        public void UpdateBlockProps() {
            LoadDefaultProps();
            string propsPath = BlockProps.PropsPath("_" + MapName);
                
            // backwards compatibility with older versions
            if (!File.Exists(propsPath)) {
                BlockProps.Load("lvl_" + MapName, Props, 2, true);
            } else {
                BlockProps.Load("_" + MapName,    Props, 2, false);
            }            
        }
        
        public void UpdateBlockHandlers() {
            for (int i = 0; i < Props.Length; i++) {
                UpdateBlockHandler((BlockID)i);
            }
        }
        
        public void UpdateBlockHandler(BlockID block) {
            bool nonSolid = !MCGalaxy.Blocks.CollideType.IsSolid(CollideType(block));           
            deleteHandlers[block]       = BlockBehaviour.GetDeleteHandler(block, Props);
            placeHandlers[block]        = BlockBehaviour.GetPlaceHandler(block, Props);
            walkthroughHandlers[block]  = BlockBehaviour.GetWalkthroughHandler(block, Props, nonSolid);
            physicsHandlers[block]      = BlockBehaviour.GetPhysicsHandler(block, Props);
            physicsDoorsHandlers[block] = BlockBehaviour.GetPhysicsDoorsHandler(block, Props);
        }
        
        public void UpdateCustomBlock(BlockID block, BlockDefinition def) {
            CustomBlockDefs[block] = def;
            UpdateBlockHandler(block);
            blockAABBs[block] = Block.BlockAABB(block, this);
        }
    }
}