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
using System.Runtime.InteropServices;
using System.Threading;
using MCGalaxy.SQL;
using Timer = System.Timers.Timer;
using MCGalaxy.BlockPhysics;
using MCGalaxy.Config;
using MCGalaxy.Games;
using MCGalaxy.Levels.IO;
using MCGalaxy.SQL.Native;
//WARNING! DO NOT CHANGE THE WAY THE LEVEL IS SAVED/LOADED!
//You MUST make it able to save and load as a new version other wise you will make old levels incompatible!

namespace MCGalaxy
{
    public enum LevelPermission {
        Banned = -20, Guest = 0, Builder = 30,
        AdvBuilder = 50, Operator = 80,
        Admin = 100, Nobody = 120, Null = 150
    }
	
    public enum BuildType { Normal, ModifyOnly, NoModify };

    public sealed partial class Level : IDisposable
    {
        #region Delegates

        public delegate void OnLevelLoad(string level);

        public delegate void OnLevelLoaded(Level l);

        public delegate void OnLevelSave(Level l);

        public delegate void OnLevelUnload(Level l);

        public delegate void OnPhysicsUpdate(ushort x, ushort y, ushort z, PhysicsArgs args, Level l);

        public delegate void OnPhysicsStateChanged(object sender, PhysicsState state);

        #endregion

        public static event OnPhysicsStateChanged PhysicsStateChanged;
        public static bool cancelload;
        public static bool cancelsave;
        public static bool cancelphysics;
        internal FastList<Check> ListCheck = new FastList<Check>(); //A list of blocks that need to be updated
        internal FastList<Update> ListUpdate = new FastList<Update>(); //A list of block to change after calculation
        internal SparseBitSet listCheckExists, listUpdateExists;

        internal readonly Dictionary<int, sbyte> leaves = new Dictionary<int, sbyte>();
        // Holds block state for leaf decay

        internal readonly Dictionary<int, bool[]> liquids = new Dictionary<int, bool[]>();
        // Holds random flow data for liqiud physics
        bool physicssate = false;
        [ConfigBool("Survival death", "General", null, false)]        
        public bool Death;
        public ExtrasCollection Extras = new ExtrasCollection();
        public bool GrassDestroy = true;
        public bool GrassGrow = true;
        [ConfigBool("Killer blocks", "General", null, true)]        
        public bool Killer = true;
        public List<UndoPos> UndoBuffer = new List<UndoPos>();
        public List<Zone> ZoneList;
        [ConfigBool("Animal AI", "General", null, true)]
        public bool ai = true;
        public bool backedup;
        public List<BlockPos> blockCache = new List<BlockPos>();
        [ConfigBool("Buildable", "Permissions", null, true)]        
        public bool Buildable = true;
        [ConfigBool("Deletable", "Permissions", null, true)]
        public bool Deletable = true;
        
        [ConfigBool("UseBlockDB", "Other", null, true)]
        public bool UseBlockDB = true;

        [ConfigInt("Weather", "Env", null, 0, 0, 2)]        
        public int Weather;
        [ConfigString("Texture", "Env", null, "", true)]
        public string terrainUrl = "";
        [ConfigString("TexturePack", "Env", null, "", true)]
        public string texturePackUrl = "";

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

        [ConfigInt("Drown", "General", null, 70)]   
        public int drown = 70;
        [ConfigBool("Edge water", "General", null, true)]
        public bool edgeWater;
        [ConfigInt("Fall", "General", null, 9)]
        public int fall = 9;
        [ConfigBool("Finite mode", "General", null, false)] 
        public bool finite;
        [ConfigBool("GrowTrees", "General", null, false)]
        public bool growTrees;
        [ConfigBool("Guns", "General", null, false)]
        public bool guns = false;
        
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
        public int EdgeLevel;
        
        /// <summary> Elevation of the clouds. Default is map height + 2. </summary>
        public int CloudsHeight;
        
        /// <summary> Max fog distance the client can see. 
        /// Default is 0, meaning use the client-side defined maximum fog distance. </summary>
        public int MaxFogDistance;
        
        /// <summary> Clouds speed, in units of 256ths. Default is 256 (1 speed). </summary>
        [ConfigInt("clouds-speed", "Env", null, 256, short.MinValue, short.MaxValue)]
        public int CloudsSpeed = 256;
        
        /// <summary> Weather speed, in units of 256ths. Default is 256 (1 speed). </summary>
        [ConfigInt("weather-speed", "Env", null, 256, short.MinValue, short.MaxValue)]
        public int WeatherSpeed = 256;
        
        /// <summary> Weather fade, in units of 256ths. Default is 256 (1 speed). </summary>
        [ConfigInt("weather-fade", "Env", null, 128, short.MinValue, short.MaxValue)]
        public int WeatherFade = 128;

        /// <summary> The block which will be displayed on the horizon. </summary>
        public byte HorizonBlock = Block.water;

        /// <summary> The block which will be displayed on the edge of the map. </summary>
        public byte EdgeBlock = Block.blackrock;
        
        public BlockDefinition[] CustomBlockDefs;
        
        [ConfigInt("JailX", "Jail", null, 0, 0, 65535)]
        public int jailx;
        [ConfigInt("JailY", "Jail", null, 0, 0, 65535)]
        public int jaily;
        [ConfigInt("JailZ", "Jail", null, 0, 0, 65535)]
        public int jailz;
        
        public int lastCheck;
        public int lastUpdate;
        [ConfigBool("LeafDecay", "General", null, false)]        
        public bool leafDecay;
        [ConfigBool("LoadOnGoto", "General", null, true)]
        public bool loadOnGoto = true;
        [ConfigString("MOTD", "General", null, "ignore", true)]
        public string motd = "ignore";
        public string name;
        [ConfigInt("Physics overload", "General", null, 250)]        
        public int overload = 1500;
        
        [ConfigPerm("PerBuildMax", "Permissions", null, LevelPermission.Nobody, true)]
        public LevelPermission perbuildmax = LevelPermission.Nobody;
        [ConfigPerm("PerBuild", "Permissions", null, LevelPermission.Guest, true)]
        public LevelPermission permissionbuild = LevelPermission.Guest;
        // What ranks can go to this map (excludes banned)
        [ConfigPerm("PerVisit", "Permissions", null, LevelPermission.Guest, true)]
        public LevelPermission permissionvisit = LevelPermission.Guest;
        [ConfigPerm("PerVisitMax", "Permissions", null, LevelPermission.Nobody, true)]
        public LevelPermission pervisitmax = LevelPermission.Nobody;
        // Other blacklists/whitelists
        [ConfigStringList("VisitWhitelist", "Permissions", null)]
        public List<string> VisitWhitelist = new List<string>();
        [ConfigStringList("VisitBlacklist", "Permissions", null)]
        public List<string> VisitBlacklist = new List<string>();
        
        public Random physRandom = new Random();
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
        [ConfigBool("RandomFlow", "General", null, true)]        
        public bool randomFlow = true;
        public byte rotx;
        public byte roty;
        public ushort spawnx, spawny, spawnz;

        [ConfigInt("Physics speed", "General", null, 250)]
        public int speedPhysics = 250;

        [ConfigString("Theme", "General", null, "Normal", true)]
        public string theme = "Normal";
        [ConfigBool("Unload", "General", null, true)]
        public bool unload = true;
        [ConfigBool("WorldChat", "General", null, true)]        
        public bool worldChat = true;
        
        public bool bufferblocks = Server.bufferblocks;
        internal readonly object queueLock = new object();
        public List<BlockQueue.block> blockqueue = new List<BlockQueue.block>();
        private readonly object physThreadLock = new object();
        BufferedBlockSender bulkSender;

        public List<C4Data> C4list = new List<C4Data>();
        // Games fields
        [ConfigInt("Likes", "Game", null, 0)]
        public int Likes;
        [ConfigInt("Dislikes", "Game", null, 0)]
        public int Dislikes;
        [ConfigString("Authors", "Game", null, "", true)]
        public string Authors = "";
        [ConfigBool("Pillaring", "Game", null, false)]
        public bool Pillaring = !ZombieGame.noPillaring;
        
        public BuildType BuildType = BuildType.Normal;
        public bool CanPlace { get { return Buildable && BuildType != BuildType.NoModify; } }
        public bool CanDelete { get { return Deletable && BuildType != BuildType.NoModify; } }
        
        [ConfigInt("MinRoundTime", "Game", null, 4)]
        public int MinRoundTime = 4;
        [ConfigInt("MaxRoundTime", "Game", null, 7)]
        public int MaxRoundTime = 7;
        [ConfigBool("DrawingAllowed", "Game", null, true)]
        public bool DrawingAllowed = true;
        [ConfigInt("RoundsPlayed", "Game", null, 0)]
        public int RoundsPlayed = 0;
        [ConfigInt("RoundsHumanWon", "Game", null, 0)]
        public int RoundsHumanWon = 0;
        
        public int WinChance {
            get { return RoundsPlayed == 0 ? 100 : (RoundsHumanWon * 100) / RoundsPlayed; }
        }
        
        public Level(string n, ushort x, ushort y, ushort z) {
            Init(n, x, y, z);
        }
        
        public Level(string n, ushort x, ushort y, ushort z, string type, int seed = 0, bool useSeed = false) {
            Init(n, x, y, z);
            MapGen.Generate(this, type, seed, useSeed);
        }
        
        void Init(string n, ushort x, ushort y, ushort z) {
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

            spawnx = (ushort)(Width / 2);
            spawny = (ushort)(Height * 0.75f);
            spawnz = (ushort)(Length / 2);
            rotx = 0;
            roty = 0;
            listCheckExists = new SparseBitSet(Width, Height, Length);
            listUpdateExists = new SparseBitSet(Width, Height, Length);
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
            lock (queueLock)
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

        /// <summary> Whether block changes made on this level should be 
        /// saved to the BlockDB and .lvl files. </summary>
        public bool ShouldSaveChanges() {
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

            if (changed && ShouldSaveChanges()) {
                Save(false, true);
                saveChanges();
            }
            if (TntWarsGame.Find(this) != null)
            {
                foreach (TntWarsGame.player pl in TntWarsGame.Find(this).Players)
                {
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
                if (p.level.name.ToLower() == name.ToLower()) {
                    Player.Message(p, "You were moved to the main level as " + name + " was unloaded.");
                    Command.all.Find("goto").Use(p, Server.mainLevel.name);
                }
            }
        }
        
        public unsafe void saveChanges() {
            if (blockCache.Count == 0) return;
            if (!UseBlockDB) { blockCache.Clear(); return; }
            List<BlockPos> tempCache = blockCache;
            string date = new String('-', 19); //yyyy-mm-dd hh:mm:ss
            
            using (BulkTransaction transaction = BulkTransaction.Create()) {
                fixed (char* ptr = date) {
                    ptr[4] = '-'; ptr[7] = '-'; ptr[10] = ' '; ptr[13] = ':'; ptr[16] = ':';
                    DoSaveChanges(tempCache, ptr, date, transaction);
                }
            }
            tempCache.Clear();
            blockCache = new List<BlockPos>();
            Server.s.Log("Saved BlockDB changes for:" + name, true);
        }
        
        unsafe bool DoSaveChanges(List<BlockPos> tempCache, char* ptr, string date, 
                                  BulkTransaction transaction) {
            string template = "INSERT INTO `Block" + name +
                "` (Username, TimePerformed, X, Y, Z, type, deleted) VALUES (@Name, @Time, @X, @Y, @Z, @Tile, @Del)";
            ushort x, y, z;
            
            IDbCommand cmd = transaction.CreateCommand(template);
            IDataParameter nameP = transaction.CreateParam("@Name", DbType.AnsiStringFixedLength); cmd.Parameters.Add(nameP);
            IDataParameter timeP = transaction.CreateParam("@Time", DbType.AnsiStringFixedLength); cmd.Parameters.Add(timeP);
            IDataParameter xP = transaction.CreateParam("@X", DbType.UInt16); cmd.Parameters.Add(xP);
            IDataParameter yP = transaction.CreateParam("@Y", DbType.UInt16); cmd.Parameters.Add(yP);
            IDataParameter zP = transaction.CreateParam("@Z", DbType.UInt16); cmd.Parameters.Add(zP);
            IDataParameter tileP = transaction.CreateParam("@Tile", DbType.Byte); cmd.Parameters.Add(tileP);
            IDataParameter delP = transaction.CreateParam("@Del", DbType.Boolean); cmd.Parameters.Add(delP);
            bool isNative = transaction is NativeBulkTransaction;
            
            for (int i = 0; i < tempCache.Count; i++) {
                BlockPos bP = tempCache[i];
                IntToPos(bP.index, out x, out y, out z);
                DateTime time = Server.StartTimeLocal.AddTicks((bP.flags >> 2) * TimeSpan.TicksPerSecond);
                MakeInt(time.Year, 4, 0, ptr); MakeInt(time.Month, 2, 5, ptr); MakeInt(time.Day, 2, 8, ptr);
                MakeInt(time.Hour, 2, 11, ptr); MakeInt(time.Minute, 2, 14, ptr); MakeInt(time.Second, 2, 17, ptr);               
                
                // For NativeParameter, we make the optimisation of avoiding boxing primitive types.
                if (!isNative) {
                    nameP.Value = bP.name;
                    timeP.Value = date;
                    xP.Value = x; yP.Value = y; zP.Value = z;
                    tileP.Value = (bP.flags & 2) != 0 ? Block.custom_block : bP.rawType;
                    delP.Value = (bP.flags & 1) != 0;
                } else {
                    ((NativeParameter)nameP).SetString(bP.name);
                    ((NativeParameter)timeP).SetString(date);
                    ((NativeParameter)xP).U16Value = x;
                    ((NativeParameter)yP).U16Value = y;
                    ((NativeParameter)zP).U16Value = z;
                    ((NativeParameter)tileP).U8Value = (bP.flags & 2) != 0 ? Block.custom_block : bP.rawType;
                    ((NativeParameter)delP).BoolValue = (bP.flags & 1) != 0;
                }

                if (!BulkTransaction.Execute(template, cmd)) {
                    cmd.Dispose();
                    cmd.Parameters.Clear();
                    transaction.Rollback(); return false;
                }
            }
            cmd.Dispose();
            cmd.Parameters.Clear();
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
        public bool InBound(ushort x, ushort y, ushort z) {
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
            string path = LevelInfo.LevelPath(name);
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
            } catch (Exception e) {
                Server.s.Log("FAILED TO SAVE :" + name);
                Player.GlobalMessage("FAILED TO SAVE :" + name);
                Server.ErrorLog(e);
            }
            GC.Collect();
            GC.WaitForPendingFinalizers();
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

        public static void CreateLeveldb(string givenName) {
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

        public static Level Load(string givenName) {
            return Load(givenName, 0);
        }

        //givenName is safe against SQL injections, it gets checked in CmdLoad.cs
        public static Level Load(string givenName, byte phys) {
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
                    level.setPhysics(phys);
                    level.backedup = true;

                    using (DataTable ZoneDB = Database.fillData("SELECT * FROM `Zone" + givenName + "`"))
                    {
                        Zone Zn;
                        for (int i = 0; i < ZoneDB.Rows.Count; ++i)
                        {
                            DataRow row = ZoneDB.Rows[i];
                            Zn.smallX = ushort.Parse(row["SmallX"].ToString());
                            Zn.smallY = ushort.Parse(row["SmallY"].ToString());
                            Zn.smallZ = ushort.Parse(row["SmallZ"].ToString());
                            Zn.bigX = ushort.Parse(row["BigX"].ToString());
                            Zn.bigY = ushort.Parse(row["BigY"].ToString());
                            Zn.bigZ = ushort.Parse(row["BigZ"].ToString());
                            Zn.Owner = row["Owner"].ToString();
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

                    try
                    {
                        DataTable foundDB = Database.fillData("SELECT * FROM `Portals" + givenName + "`");

                        for (int i = 0; i < foundDB.Rows.Count; ++i)
                        {
                            DataRow row = foundDB.Rows[i];
                            if (
                                !Block.portal(level.GetTile(ushort.Parse(row["EntryX"].ToString()),
                                                            ushort.Parse(row["EntryY"].ToString()),
                                                            ushort.Parse(row["EntryZ"].ToString()))))
                            {
                                Database.executeQuery("DELETE FROM `Portals" + givenName + "` WHERE EntryX=" +
                                                      row["EntryX"] + " AND EntryY=" +
                                                      row["EntryY"] + " AND EntryZ=" +
                                                      row["EntryZ"]);
                            }
                        }
                        foundDB = Database.fillData("SELECT * FROM `Messages" + givenName + "`");

                        for (int i = 0; i < foundDB.Rows.Count; ++i)
                        {
                            DataRow row = foundDB.Rows[i];                        	
                            if (
                                !Block.mb(level.GetTile(ushort.Parse(row["X"].ToString()),
                                                        ushort.Parse(row["Y"].ToString()),
                                                        ushort.Parse(row["Z"].ToString()))))
                            {
                                //givenName is safe against SQL injections, it gets checked in CmdLoad.cs
                                Database.executeQuery("DELETE FROM `Messages" + givenName + "` WHERE X=" +
                                                      row["X"] + " AND Y=" + row["Y"] +
                                                      " AND Z=" + row["Z"]);
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
                    
                    Bots.BotsFile.LoadBots(level);

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