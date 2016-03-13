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
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading;
using MCGalaxy.SQL;
using Timer = System.Timers.Timer;
using MCGalaxy.BlockPhysics;
using MCGalaxy.Levels.IO;
//WARNING! DO NOT CHANGE THE WAY THE LEVEL IS SAVED/LOADED!
//You MUST make it able to save and load as a new version other wise you will make old levels incompatible!

namespace MCGalaxy
{
    public enum LevelPermission //int is default
    {
        Banned = -20,
        Guest = 0,
        Builder = 30,
        AdvBuilder = 50,
        Operator = 80,
        Admin = 100,
        Nobody = 120,
        Null = 150
    }

    public sealed partial class Level : IDisposable
    {
        #region Delegates

        public delegate void OnLevelLoad(string level);

        public delegate void OnLevelLoaded(Level l);

        public delegate void OnLevelSave(Level l);

        public delegate void OnLevelUnload(Level l);

        public delegate void OnPhysicsUpdate(ushort x, ushort y, ushort z, byte time, string extraInfo, Level l);

        public delegate void OnPhysicsStateChanged(object sender, PhysicsState state);

        #endregion

        public static event OnPhysicsStateChanged PhysicsStateChanged;
        public static bool cancelload;
        public static bool cancelsave;
        public static bool cancelphysics;
        internal readonly FastList<Check> ListCheck = new FastList<Check>(); //A list of blocks that need to be updated
        internal readonly FastList<Update> ListUpdate = new FastList<Update>(); //A list of block to change after calculation
        internal readonly SparseBitSet listCheckExists, listUpdateExists;

        internal readonly Dictionary<int, sbyte> leaves = new Dictionary<int, sbyte>();
        // Holds block state for leaf decay

        internal readonly Dictionary<int, bool[]> liquids = new Dictionary<int, bool[]>();
        // Holds random flow data for liqiud physics
        bool physicssate = false;
        public bool Death;
        public ExtrasCollection Extras = new ExtrasCollection();
        public bool GrassDestroy = true;
        public bool GrassGrow = true;
        public bool Killer = true;
        public List<UndoPos> UndoBuffer = new List<UndoPos>();
        public List<Zone> ZoneList;
        public bool ai = true;
        public bool backedup;
        public List<BlockPos> blockCache = new List<BlockPos>();
        public bool Buildable = true, Deletable = true;
        
        public byte weather;
        public string terrainUrl = "", texturePackUrl = "";

        public bool cancelsave1;
        public bool cancelunload;
        public bool changed;
        public bool physicschanged
        {
            get { return ListCheck.Count > 0; }
        }
        public bool countdowninprogress;
        public bool ctfmode;
        public int currentUndo;
        public ushort Width, Height, Length;
        // NOTE: These are for legacy code only, you should use upper case Width/Height/Length
        // as these correctly map Y to being Height
        [Obsolete] public ushort width;
        [Obsolete] public ushort height;
        [Obsolete] public ushort depth;
        [Obsolete] public ushort length;
        
        public bool IsMuseum { 
            get { return name.StartsWith("&cMuseum " + Server.DefaultColor, StringComparison.Ordinal); } 
        }
        
        public int drown = 70;
        public bool edgeWater;
        public int fall = 9;
        public bool finite;
        public bool fishstill;
        public bool growTrees;
        public bool guns = true;
        public byte jailrotx, jailroty;
        /// <summary> Color of the clouds (RGB packed into an int). Set to -1 to use client defaults. </summary>
        public string CloudColor = null;

        /// <summary> Color of the fog (RGB packed into an int). Set to -1 to use client defaults. </summary>
        public string FogColor = null;

        /// <summary> Color of the sky (RGB packed into an int). Set to -1 to use client defaults. </summary>
        public string SkyColor = null;

        /// <summary> Color of the blocks in shadows (RGB packed into an int). Set to -1 to use client defaults. </summary>
        public string ShadowColor = null;

        /// <summary> Color of the blocks in the light (RGB packed into an int). Set to -1 to use client defaults. </summary>
        public string LightColor = null;

        /// <summary> Elevation of the "ocean" that surrounds maps. Default is map height / 2. </summary>
        public short EdgeLevel;
        
        /// <summary> Elevation of the clouds. Default is map height + 2. </summary>
        public short CloudsHeight;
        
        /// <summary> Max fog distance the client can see. Default is 0, meaning use the client-side defined maximum fog distance. </summary>
        public short MaxFogDistance;

        /// <summary> The block which will be displayed on the horizon. </summary>
        public byte HorizonBlock = Block.water;

        /// <summary> The block which will be displayed on the edge of the map. </summary>
        public byte EdgeBlock = Block.blackrock;
        
        public BlockDefinition[] CustomBlockDefs;
        
        public ushort jailx, jaily, jailz;
        public int lastCheck;
        public int lastUpdate;
        public bool leafDecay;
        public bool loadOnGoto = true;
        public string motd = "ignore";
        public string name;
        public int overload = 1500;
        public LevelPermission perbuildmax = LevelPermission.Nobody;
        public LevelPermission permissionbuild = LevelPermission.Guest;
        // What ranks can go to this map (excludes banned)
        public LevelPermission permissionvisit = LevelPermission.Guest;
        public LevelPermission pervisitmax = LevelPermission.Nobody;

        public bool physPause;
        public DateTime physResume;
        public Thread physThread;
        public Timer physTimer = new Timer(1000);
        //public Timer physChecker = new Timer(1000);
        public int physics
        {
            get { return Physicsint; }
            set
            {
                if (value > 0 && Physicsint == 0)
                    StartPhysics();
                Physicsint = value;
            }
        }
        int Physicsint;
        public bool randomFlow = true;
        public bool realistic = true;
        public byte rotx;
        public byte roty;
        public bool rp = true;
        public ushort spawnx, spawny, spawnz;

        public int speedPhysics = 250;

        public string theme = "Normal";
        public bool unload = true;
        public bool worldChat = true;
        public bool bufferblocks = Server.bufferblocks;
        public List<BlockQueue.block> blockqueue = new List<BlockQueue.block>();
        private readonly object physThreadLock = new object();
        BufferedBlockSender bulkSender;

        public List<C4.C4s> C4list = new List<C4.C4s>();

        public Level(string n, ushort x, ushort y, ushort z, string type, int seed = 0, bool useSeed = false)
        {
            //onLevelSave += null;
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

            MapGen.Generate(this, type, seed, useSeed);
            spawnx = (ushort)(Width / 2);
            spawny = (ushort)(Height * 0.75f);
            spawnz = (ushort)(Length / 2);
            rotx = 0;
            roty = 0;
            listCheckExists = new SparseBitSet(Width, Height, Length);
            listUpdateExists = new SparseBitSet(Width, Height, Length);
            //season = new SeasonsCore(this);
        }

        public List<Player> players
        {
            get { return getPlayers(); }
        }

        #region IDisposable Members

        public void Dispose()
        {
            Extras.Clear();
            liquids.Clear();
            leaves.Clear();
            ListCheck.Clear(); listCheckExists.Clear();
            ListUpdate.Clear(); listUpdateExists.Clear();
            UndoBuffer.Clear();
            blockCache.Clear();
            ZoneList.Clear();
            blockqueue.Clear();
            blocks = null;
            CustomBlocks = null;
        }

        #endregion
        [Obsolete("Please use OnPhysicsUpdate.Register()")]
        public event OnPhysicsUpdate PhysicsUpdate = null;
        [Obsolete("Please use OnLevelUnloadEvent.Register()")]
        public static event OnLevelUnload LevelUnload = null;
        [Obsolete("Please use OnLevelSaveEvent.Register()")]
        public static event OnLevelSave LevelSave = null;
        //public static event OnLevelSave onLevelSave = null;
        [Obsolete("Please use OnLevelUnloadEvent.Register()")]
        public event OnLevelUnload onLevelUnload = null;
        [Obsolete("Please use OnLevelUnloadEvent.Register()")]
        public static event OnLevelLoad LevelLoad = null;
        [Obsolete("Please use OnLevelUnloadEvent.Register()")]
        public static event OnLevelLoaded LevelLoaded;

        public bool ShouldSaveLevelFile() {
        	if (Server.ZombieModeOn && (name == Server.zombie.currentLevelName 
        	                             || name == Server.zombie.lastLevelName)) return false;
        	if (Server.lava.active && Server.lava.HasMap(name)) return false;
        	return true;
        }
        
        public bool Unload(bool silent = false, bool save = true)
        {
            if (Server.mainLevel == this || IsMuseum) return false;
            if (Server.lava.active && Server.lava.map == this) return false;
            if (LevelUnload != null)
                LevelUnload(this);
            OnLevelUnloadEvent.Call(this);
            if (cancelunload)
            {
                Server.s.Log("Unload canceled by Plugin! (Map: " + name + ")");
                cancelunload = false;
                return false;
            }
            MovePlayersToMain();

            if (changed) {
            	if (ShouldSaveLevelFile()) Save(false, true);
                saveChanges();
            }
            if (TntWarsGame.Find(this) != null)
            {
                foreach (TntWarsGame.player pl in TntWarsGame.Find(this).Players)
                {
                    pl.p.CurrentTntGameNumber = -1;
                    Player.SendMessage(pl.p, "TNT Wars: The TNT Wars game you are currently playing has been deleted!");
                    pl.p.PlayingTntWars = false;
                    pl.p.canBuild = true;
                    TntWarsGame.SetTitlesAndColor(pl, true);
                }
                Server.s.Log("TNT Wars: Game deleted on " + name);
                TntWarsGame.GameList.Remove(TntWarsGame.Find(this));

            }
            MovePlayersToMain();
            LevelInfo.Loaded.Remove(this);

            try
            {
                PlayerBot.RemoveAllFromLevel(this);
                //physChecker.Stop();
                //physChecker.Dispose();
                physThread.Abort();
                physThread.Join();

            }
            catch
            {
            }

            finally
            {
                Dispose();
                GC.Collect();
                GC.WaitForPendingFinalizers();

                if (!silent) Chat.GlobalMessageOps("&3" + name + Server.DefaultColor + " was unloaded.");
                Server.s.Log(string.Format("{0} was unloaded.", name));
            }
            return true;
        }

        void MovePlayersToMain() {
        	Player[] players = PlayerInfo.Online.Items; 
            foreach (Player p in players) {
                if (p.level.name.ToLower() == name.ToLower()) {
                    Player.SendMessage(p, "You were moved to the main level as " + name + " was unloaded.");
                    Command.all.Find("goto").Use(p, Server.mainLevel.name);
                }
            }
        }
        
        public unsafe void saveChanges() {
            if (blockCache.Count == 0) return;
            List<BlockPos> tempCache = blockCache;
            string date = new String('-', 19); //yyyy-mm-dd hh:mm:ss
            
            using (DatabaseTransactionHelper transaction = DatabaseTransactionHelper.Create()) {
                fixed (char* ptr = date) {
                    ptr[4] = '-'; ptr[7] = '-'; ptr[10] = ' '; ptr[13] = ':'; ptr[16] = ':';
                    DoSaveChanges(tempCache, ptr, date, transaction);
                }
            }
            tempCache.Clear();
            blockCache = new List<BlockPos>();
            Server.s.Log("Saved BlockDB changes for:" + name);
        }
        
        unsafe bool DoSaveChanges(List<BlockPos> tempCache, char* ptr, string date, 
                                  DatabaseTransactionHelper transaction) {
            string template = "INSERT INTO `Block" + name +
                "` (Username, TimePerformed, X, Y, Z, type, deleted) VALUES (@Name, @Time, @X, @Y, @Z, @Tile, @Del)";
            ushort x, y, z;
            
            IDbCommand cmd = transaction.CreateCommand(template);
            DbParameter nameP = transaction.CreateParam("@Name", DbType.AnsiStringFixedLength); cmd.Parameters.Add(nameP);
            DbParameter timeP = transaction.CreateParam("@Time", DbType.AnsiStringFixedLength); cmd.Parameters.Add(timeP);
            DbParameter xP = transaction.CreateParam("@X", DbType.UInt16); cmd.Parameters.Add(xP);
            DbParameter yP = transaction.CreateParam("@Y", DbType.UInt16); cmd.Parameters.Add(yP);
            DbParameter zP = transaction.CreateParam("@Z", DbType.UInt16); cmd.Parameters.Add(zP);
            DbParameter tileP = transaction.CreateParam("@Tile", DbType.Byte); cmd.Parameters.Add(tileP);
            DbParameter delP = transaction.CreateParam("@Del", DbType.Boolean); cmd.Parameters.Add(delP);
            
            for (int i = 0; i < tempCache.Count; i++) {
                BlockPos bP = tempCache[i];
                IntToPos(bP.index, out x, out y, out z);
                nameP.Value = bP.name;
                DateTime time = Server.StartTimeLocal.AddTicks(bP.timeDelta * TimeSpan.TicksPerSecond);
                MakeInt(time.Year, 4, 0, ptr); MakeInt(time.Month, 2, 5, ptr); MakeInt(time.Day, 2, 8, ptr);
                MakeInt(time.Hour, 2, 11, ptr); MakeInt(time.Minute, 2, 14, ptr); MakeInt(time.Second, 2, 17, ptr);
                
                timeP.Value = date;
                xP.Value = x; yP.Value = y; zP.Value = z;
                tileP.Value = bP.type;
                delP.Value = bP.deleted;

                if (!DatabaseTransactionHelper.Execute(template, cmd)) {
                    cmd.Dispose();
                    transaction.Rollback(); return false;
                }
            }
            cmd.Dispose();
            transaction.Commit();
            return true;
        }
        
        unsafe static void MakeInt(int value, int chars, int offset, char* ptr) {
            for (int i = 0; i < chars; i++, value /= 10) {
        	    char c = (char)('0' + (value % 10));
                ptr[offset + (chars - 1 - i)] = c; 
            }
        }
        

        /// <summary> Returns whether the given coordinates are insides the boundaries of this level. </summary>
        public bool InBound(ushort x, ushort y, ushort z)
        {
            return x >= 0 && y >= 0 && z >= 0 && x < Width && y < Height && z < Length;
        }

        [Obsolete]
        public static Level Find(string name) { return LevelInfo.Find(name); }

        [Obsolete]
        public static Level FindExact(string name) { return LevelInfo.FindExact(name); }

        public static void SaveSettings(Level level) {
            LvlProperties.Save(level, "levels/level properties/" + level.name);
        }

        // Returns true if ListCheck does not already have an check in the position.
        // Useful for fireworks, which depend on two physics blocks being checked, one with extraInfo.
        public bool CheckClear(ushort x, ushort y, ushort z) {
        	return x >= Width || y >= Height || z >= Length || !listCheckExists.Get(x, y, z);
        }

        public void Save(bool Override = false, bool clearPhysics = false)
        {
            if (blocks == null) return;
            string path = "levels/" + name + ".lvl";
            if (LevelSave != null) LevelSave(this);
            OnLevelSaveEvent.Call(this);
            if (cancelsave1) { cancelsave1 = false; return; }
            if (cancelsave) { cancelsave = false; return; }
            
            try
            {
                if (!Directory.Exists("levels")) Directory.CreateDirectory("levels");
                if (!Directory.Exists("levels/level properties")) Directory.CreateDirectory("levels/level properties");

                if (changed || !File.Exists(path) || Override || (physicschanged && clearPhysics))
                {
                    if (clearPhysics)
                        ClearPhysics();
                    
                    if (File.Exists(path)) {
                        if (File.Exists(path + ".prev"))
                            File.Delete(path + ".prev");
                        File.Copy(path, path + ".prev");
                        File.Delete(path);
                    }
                    LvlFile.Save(this, path + ".backup");
                    File.Copy(path + ".backup", path);
                    SaveSettings(this);

                    Server.s.Log(string.Format("SAVED: Level \"{0}\". ({1}/{2}/{3})", name, players.Count,
                                               PlayerInfo.Online.Count, Server.players));
                    changed = false;
                }
                else
                {
                    Server.s.Log("Skipping level save for " + name + ".");
                }
            }
            catch (OutOfMemoryException e)
            {
                Server.ErrorLog(e);
                if (Server.mono)
                {
                    Process[] prs = Process.GetProcesses();
                    foreach (Process pr in prs)
                    {
                        if (pr.ProcessName == "MCGalaxy")
                            pr.Kill();
                    }
                }
                else
                    Command.all.Find("restart").Use(null, "");
            } catch (Exception e) {
                Server.s.Log("FAILED TO SAVE :" + name);
                Player.GlobalMessage("FAILED TO SAVE :" + name);
                Server.ErrorLog(e);
            }
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        public int Backup(bool Forced = false, string backupName = "")
        {
            if (!backedup || Forced)
            {
                int backupNumber = 1;
                string backupPath = @Server.backupLocation;
                if (Directory.Exists(string.Format("{0}/{1}", backupPath, name)))
                {
                    backupNumber = Directory.GetDirectories(string.Format("{0}/" + name, backupPath)).Length + 1;
                }
                else
                {
                    Directory.CreateDirectory(backupPath + "/" + name);
                }
                string path = string.Format("{0}/" + name + "/" + backupNumber, backupPath);
                if (backupName != "")
                {
                    path = string.Format("{0}/" + name + "/" + backupName, backupPath);
                }
                Directory.CreateDirectory(path);

                string BackPath = string.Format("{0}/{1}.lvl", path, name);
                string current = LevelInfo.LevelPath(name);
                try
                {
                    File.Copy(current, BackPath, true);
                    backedup = true;
                    return backupNumber;
                }
                catch (Exception e)
                {
                    Server.ErrorLog(e);
                    Server.s.Log(string.Format("FAILED TO INCREMENTAL BACKUP :{0}", name));
                    return -1;
                }
            }
            Server.s.Log("Level unchanged, skipping backup");
            return -1;
        }

        public static void CreateLeveldb(string givenName)
        {
            Database.executeQuery("CREATE TABLE if not exists `Block" + givenName +
                                  "` (Username CHAR(20), TimePerformed DATETIME, X SMALLINT UNSIGNED, Y SMALLINT UNSIGNED, Z SMALLINT UNSIGNED, Type TINYINT UNSIGNED, Deleted " +
                                  (Server.useMySQL ? "BOOL" : "INT") + ")");
            Database.executeQuery("CREATE TABLE if not exists `Portals" + givenName +
                                  "` (EntryX SMALLINT UNSIGNED, EntryY SMALLINT UNSIGNED, EntryZ SMALLINT UNSIGNED, ExitMap CHAR(20), ExitX SMALLINT UNSIGNED, ExitY SMALLINT UNSIGNED, ExitZ SMALLINT UNSIGNED)");
            Database.executeQuery("CREATE TABLE if not exists `Messages" + givenName +
                                  "` (X SMALLINT UNSIGNED, Y SMALLINT UNSIGNED, Z SMALLINT UNSIGNED, Message CHAR(255));");
            Database.executeQuery("CREATE TABLE if not exists `Zone" + givenName +
                                  "` (SmallX SMALLINT UNSIGNED, SmallY SMALLINT UNSIGNED, SmallZ SMALLINT UNSIGNED, BigX SMALLINT UNSIGNED, BigY SMALLINT UNSIGNED, BigZ SMALLINT UNSIGNED, Owner VARCHAR(20));");
        }

        public static Level Load(string givenName)
        {
            return Load(givenName, 0);
        }

        //givenName is safe against SQL injections, it gets checked in CmdLoad.cs
        public static Level Load(string givenName, byte phys)
        {
            if (LevelLoad != null)
                LevelLoad(givenName);
            OnLevelLoadEvent.Call(givenName);
            if (cancelload)
            {
                cancelload = false;
                return null;
            }
            CreateLeveldb(givenName);

            string path = LevelInfo.LevelPath(givenName);
            if (File.Exists(path))
            {
                try
                {
                    Level level = LvlFile.Load(givenName, path);
                    level.permissionbuild = LevelPermission.Builder;
                    level.setPhysics(phys);
                    //level.textures = new LevelTextures(level);
                    level.backedup = true;

                    using (DataTable ZoneDB = Database.fillData("SELECT * FROM `Zone" + givenName + "`"))
                    {
                        Zone Zn;
                        for (int i = 0; i < ZoneDB.Rows.Count; ++i)
                        {
                            Zn.smallX = ushort.Parse(ZoneDB.Rows[i]["SmallX"].ToString());
                            Zn.smallY = ushort.Parse(ZoneDB.Rows[i]["SmallY"].ToString());
                            Zn.smallZ = ushort.Parse(ZoneDB.Rows[i]["SmallZ"].ToString());
                            Zn.bigX = ushort.Parse(ZoneDB.Rows[i]["BigX"].ToString());
                            Zn.bigY = ushort.Parse(ZoneDB.Rows[i]["BigY"].ToString());
                            Zn.bigZ = ushort.Parse(ZoneDB.Rows[i]["BigZ"].ToString());
                            Zn.Owner = ZoneDB.Rows[i]["Owner"].ToString();
                            level.ZoneList.Add(Zn);
                        }
                    }

                    level.jailx = (ushort)(level.spawnx * 32);
                    level.jaily = (ushort)(level.spawny * 32);
                    level.jailz = (ushort)(level.spawnz * 32);
                    level.jailrotx = level.rotx;
                    level.jailroty = level.roty;
                    level.StartPhysics();
                    //level.physChecker.Elapsed += delegate
                    //{
                    //    if (!level.physicssate && level.physics > 0)
                    //        level.StartPhysics();
                    //};
                    //level.physChecker.Start();
                    //level.season = new SeasonsCore(level);
                    try
                    {
                        DataTable foundDB = Database.fillData("SELECT * FROM `Portals" + givenName + "`");

                        for (int i = 0; i < foundDB.Rows.Count; ++i)
                        {
                            if (
                                !Block.portal(level.GetTile(ushort.Parse(foundDB.Rows[i]["EntryX"].ToString()),
                                                            ushort.Parse(foundDB.Rows[i]["EntryY"].ToString()),
                                                            ushort.Parse(foundDB.Rows[i]["EntryZ"].ToString()))))
                            {
                                Database.executeQuery("DELETE FROM `Portals" + givenName + "` WHERE EntryX=" +
                                                      foundDB.Rows[i]["EntryX"] + " AND EntryY=" +
                                                      foundDB.Rows[i]["EntryY"] + " AND EntryZ=" +
                                                      foundDB.Rows[i]["EntryZ"]);
                            }
                        }
                        foundDB = Database.fillData("SELECT * FROM `Messages" + givenName + "`");

                        for (int i = 0; i < foundDB.Rows.Count; ++i)
                        {
                            if (
                                !Block.mb(level.GetTile(ushort.Parse(foundDB.Rows[i]["X"].ToString()),
                                                        ushort.Parse(foundDB.Rows[i]["Y"].ToString()),
                                                        ushort.Parse(foundDB.Rows[i]["Z"].ToString()))))
                            {
                                //givenName is safe against SQL injections, it gets checked in CmdLoad.cs
                                Database.executeQuery("DELETE FROM `Messages" + givenName + "` WHERE X=" +
                                                      foundDB.Rows[i]["X"] + " AND Y=" + foundDB.Rows[i]["Y"] +
                                                      " AND Z=" + foundDB.Rows[i]["Z"]);
                            }
                        }
                        foundDB.Dispose();
                    } catch (Exception e) {
                        Server.ErrorLog(e);
                    }

                    try {
                        string propsPath = LevelInfo.GetPropertiesPath(level.name);
                        if (propsPath != null)
                            LvlProperties.Load(level, propsPath);
                        else
                            Server.s.Log(".properties file for level " + level.name + " was not found.");
                        LvlProperties.LoadEnv(level, level.name);
                    } catch (Exception e) {
                        Server.ErrorLog(e);
                    }
                    
                    BlockDefinition[] defs = BlockDefinition.Load(false, level);
                    for (int i = 0; i < defs.Length; i++) {
                        if (defs[i] == null) continue;
                        level.CustomBlockDefs[i] = defs[i];
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
            Server.s.Log("ERROR loading level.");
            return null;
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
            	if (pl.group.Permission < minPerm) continue;
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

        public static LevelPermission PermissionFromName(string name)
        {
            Group foundGroup = Group.Find(name);
            return foundGroup != null ? foundGroup.Permission : LevelPermission.Null;
        }

        public static string PermissionToName(LevelPermission perm)
        {
            Group foundGroup = Group.findPerm(perm);
            return foundGroup != null ? foundGroup.name : ((int)perm).ToString();
        }

        public List<Player> getPlayers()
        {
        	Player[] players = PlayerInfo.Online.Items; 
            return players.Where(p => p.level == this).ToList();
        }

        public struct BlockPos {
        	public string name;
            public int timeDelta;
            public int index;           
            public byte type, extType;
            public bool deleted;
        }

        public struct UndoPos {
            public int location;
            public byte newType, newExtType;
            public byte oldType, oldExtType;
            public int timeDelta;
        }

        public struct Zone {
            public string Owner;
            public ushort bigX, bigY, bigZ;
            public ushort smallX, smallY, smallZ;
        }
    }
}