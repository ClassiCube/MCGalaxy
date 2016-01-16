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
using System.Data;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading;
using MCGalaxy.Levels.Textures;
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
    public enum PhysicsState
    {
        Stopped,
        Warning,
        Other
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
        internal readonly List<Check> ListCheck = new List<Check>(); //A list of blocks that need to be updated
        internal readonly List<Update> ListUpdate = new List<Update>(); //A list of block to change after calculation

        private readonly Dictionary<int, sbyte> leaves = new Dictionary<int, sbyte>();
        // Holds block state for leaf decay

        internal readonly Dictionary<int, bool[]> liquids = new Dictionary<int, bool[]>();
        // Holds random flow data for liqiud physics
        bool physicssate = false;
        public bool Death;
        public ExtrasCollection Extras = new ExtrasCollection();
        public bool GrassDestroy = true;
        public bool GrassGrow = true;
        public bool Instant;
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
        // NOTE: These are for legacy matching only, you should use upper case Width/Height/Length
        // as these correctly map Y to beinh Height
        public ushort width { get { return Width; } }
        public ushort height { get { return Length; } }
        public ushort depth { get { return Height; } }
        public ushort length { get { return Length; } }
        
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

        /// <summary> Elevation of the "ocean" that surrounds maps. Set to -1 to use client default (halfway up the map). </summary>
        public short EdgeLevel = -1;
        
        public short CloudsHeight = -1;

        /// <summary> The block which will be displayed on the horizon. </summary>
        public byte HorizonBlock = Block.water;

        /// <summary> The block which will be displayed on the edge of the map. </summary>
        public byte EdgeBlock = Block.blackrock;
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
        public LevelTextures textures;

        public string theme = "Normal";
        public bool unload = true;
        public bool worldChat = true;
        public bool bufferblocks = Server.bufferblocks;
        public List<BlockQueue.block> blockqueue = new List<BlockQueue.block>();
        private readonly object physThreadLock = new object();

        public List<C4.C4s> C4list = new List<C4.C4s>();

        public Level(string n, ushort x, ushort y, ushort z, string type, int seed = 0,
                     bool useSeed = false, bool loadTexturesConfig = true)
        {
            //onLevelSave += null;
            Width = x;
            Height = y;
            Length = z;
            if (Width < 16)
                Width = 16;
            if (Height < 16)
                Height = 16;
            if (Length < 16)
                Length = 16;

            name = n;
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
            if (loadTexturesConfig)
                textures = new LevelTextures(this);
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
            ListCheck.Clear();
            ListUpdate.Clear();
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

        public bool Unload(bool silent = false, bool save = true)
        {
            if (Server.mainLevel == this) return false;
            if (name.Contains("&cMuseum ")) return false;
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
            Player.players.ForEach(
                delegate(Player pl) { if (pl.level == this) Command.all.Find("goto").Use(pl, Server.mainLevel.name); });

            if (changed && (!Server.ZombieModeOn || !Server.noLevelSaving))
            {
                if ((!Server.lava.active || !Server.lava.HasMap(name)) && save) Save(false, true);
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

            Server.levels.Remove(this);

            try
            {
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

        public void saveChanges()
        {
            //if (!Server.useMySQL) return;
            if (blockCache.Count == 0) return;
            List<BlockPos> tempCache = blockCache;
            blockCache = new List<BlockPos>();
            ushort x, y, z;

            string template = "INSERT INTO `Block" + name +
                "` (Username, TimePerformed, X, Y, Z, type, deleted) VALUES ('{0}', '{1}', {2}, {3}, {4}, {5}, {6})";
            DatabaseTransactionHelper transaction = DatabaseTransactionHelper.Create();
            using (transaction)
            {
                foreach (BlockPos bP in tempCache)
                {
                    int deleted = bP.deleted ? 1 : 0;
                    IntToPos(bP.index, out x, out y, out z);
                    transaction.Execute(String.Format(template, bP.name,
                                                      bP.TimePerformed.ToString("yyyy-MM-dd HH:mm:ss"),
                                                      x, y, z, bP.type, deleted));
                }
                transaction.Commit();
            }
            tempCache.Clear();
        }

        public bool InBound(ushort x, ushort y, ushort z)
        {
            return x >= 0 && y >= 0 && z >= 0 && x < Width && y < Height && z < Length;
        }

        public static Level Find(string levelName)
        {
            Level tempLevel = null;
            bool returnNull = false;

            foreach (Level level in Server.levels)
            {
                if (level.name.ToLower() == levelName) return level;
                else { continue; }
                if (tempLevel == null) tempLevel = level;
                else returnNull = true;
            }

            return returnNull ? null : tempLevel;
        }

        public static Level FindExact(string levelName)
        {
            return Server.levels.Find(lvl => levelName.ToLower() == lvl.name.ToLower());
        }

        public static void SaveSettings(Level level) {
            LvlProperties.Save(level, "levels/level properties/" + level.name);
        }

        // Returns true if ListCheck does not already have an check in the position.
        // Useful for fireworks, which depend on two physics blocks being checked, one with extraInfo.
        public bool CheckClear(ushort x, ushort y, ushort z)
        {
            int b = PosToInt(x, y, z);
            return !ListCheck.Exists(C => C.b == b);
        }

        public void Save(bool Override = false, bool clearPhysics = false)
        {
            //if (season.started)
            //    season.Stop(this);
            if (blocks == null) return;
            string path = "levels/" + name + ".lvl";
            if (LevelSave != null)
                LevelSave(this);
            OnLevelSaveEvent.Call(this);
            if (cancelsave1)
            {
                cancelsave1 = false;
                return;
            }
            if (cancelsave)
            {
                cancelsave = false;
                return;
            }
            try
            {
                if (!Directory.Exists("levels")) Directory.CreateDirectory("levels");
                if (!Directory.Exists("levels/level properties")) Directory.CreateDirectory("levels/level properties");

                if (changed || !File.Exists(path) || Override || (physicschanged && clearPhysics))
                {
                    if (clearPhysics)
                        ClearPhysics();
                    
                    LvlFile.Save(this, path + ".back");
                    File.Delete(path + ".backup");
                    File.Copy(path + ".back", path + ".backup");
                    File.Delete(path);
                    File.Move(path + ".back", path);
                    SaveSettings(this);

                    Server.s.Log(string.Format("SAVED: Level \"{0}\". ({1}/{2}/{3})", name, players.Count,
                                               Player.players.Count, Server.players));
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
            }
            catch (Exception e)
            {
                Server.s.Log("FAILED TO SAVE :" + name);
                Player.GlobalMessage("FAILED TO SAVE :" + name);

                Server.ErrorLog(e);
                return;
            }
            //season.Start(this);
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
                string current = string.Format("levels/{0}.lvl", name);
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

            string path = string.Format("levels/{0}.lvl", givenName);
            if (File.Exists(path))
            {
                try
                {
                    Level level = LvlFile.Load(givenName,path);
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
                        LvlProperties.Load(level, "levels/level properties/" + level.name);
                    } catch (Exception e) {
                        Server.ErrorLog(e);
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

        public static bool CheckLoadOnGoto(string givenName)
        {
            try
            {
                string foundLocation;
                foundLocation = "levels/level properties/" + givenName + ".properties";
                if (!File.Exists(foundLocation))
                    foundLocation = "levels/level properties/" + givenName;
                if (!File.Exists(foundLocation))
                    return true;

                foreach (string line in File.ReadAllLines(foundLocation))
                {
                    try
                    {
                        if (line[0] == '#') continue;
                        string value = line.Substring(line.IndexOf(" = ") + 3);

                        switch (line.Substring(0, line.IndexOf(" = ")).ToLower())
                        {
                            case "loadongoto":
                                return bool.Parse(value);
                        }
                    }
                    catch (Exception e)
                    {
                        Server.ErrorLog(e);
                    }
                }
            }
            catch
            {
            }
            return true;
        }

        public void ChatLevel(string message)
        {
            foreach (Player pl in Player.players.Where(pl => pl.level == this))
            {
                pl.SendMessage(message);
            }
        }

        public void ChatLevelOps(string message)
        {
            foreach (
                Player pl in
                Player.players.Where(
                    pl =>
                    pl.level == this &&
                    pl.group.Permission >= Server.opchatperm))
            {
                pl.SendMessage(message);
            }
        }

        public void ChatLevelAdmins(string message)
        {
            foreach (
                Player pl in
                Player.players.Where(
                    pl => pl.level == this &&
                    pl.group.Permission >= Server.adminchatperm))
            {
                pl.SendMessage(message);
            }
        }
        
        public void UpdateBlockPermissions() {
        	foreach (Player p in Player.players) {
        		if (p.level != this) continue;
        		if (!p.HasCpeExt(CpeExt.BlockPermissions)) continue;
        		p.SendCurrentBlockPermissions();
        	}
        }

        public void setPhysics(int newValue)
        {
            if (physics == 0 && newValue != 0 && blocks != null)
            {
                for (int i = 0; i < blocks.Length; i++)
                    // Optimization hack, since no blocks under 183 ever need a restart
                    if (blocks[i] > 183)
                        if (Block.NeedRestart(blocks[i]))
                            AddCheck(i);
            }
            physics = newValue;
            //StartPhysics(); This isnt needed, the physics will start when we set the new value above
        }
        public void StartPhysics()
        {
            lock (physThreadLock)
            {
                if (physThread != null)
                {
                    if (physThread.ThreadState == System.Threading.ThreadState.Running)
                        return;
                }
                if (ListCheck.Count == 0 || physicssate)
                    return;
                physThread = new Thread(Physics);
                physThread.Name = "MCG_Physics";
                PhysicsEnabled = true;
                physThread.Start();
                physicssate = true;
            }
        }

        /// <summary> Gets or sets a value indicating whether physics are enabled. </summary>
        public bool PhysicsEnabled;

        public void Physics()
        {
            int wait = speedPhysics;
            while (true)
            {

                if (!PhysicsEnabled)
                {
                    Thread.Sleep(500);
                    continue;
                }

                try
                {
                    if (wait > 0) Thread.Sleep(wait);
                    if (physics == 0 || ListCheck.Count == 0)
                    {
                        lastCheck = 0;
                        wait = speedPhysics;
                        if (physics == 0) break;
                        continue;
                    }

                    DateTime start = DateTime.UtcNow;

                    if (physics > 0) CalcPhysics();

                    TimeSpan delta = DateTime.UtcNow - start;
                    wait = speedPhysics - (int)delta.TotalMilliseconds;

                    if (wait < (int)(-overload * 0.75f))
                    {
                        if (wait < -overload)
                        {
                            if (!Server.physicsRestart)
                                setPhysics(0);
                            ClearPhysics();

                            Player.GlobalMessage("Physics shutdown on &b" + name);
                            Server.s.Log("Physics shutdown on " + name);
                            if (PhysicsStateChanged != null)
                                PhysicsStateChanged(this, PhysicsState.Stopped);

                            wait = speedPhysics;
                        }
                        else
                        {
                            foreach (Player p in Player.players.Where(p => p.level == this))
                            {
                                Player.SendMessage(p, "Physics warning!");
                            }
                            Server.s.Log("Physics warning on " + name);

                            if (PhysicsStateChanged != null)
                                PhysicsStateChanged(this, PhysicsState.Warning);
                        }
                    }
                }
                catch
                {
                    wait = speedPhysics;
                }
            }
            physicssate = false;
            physThread.Abort();
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
            return Player.players.Where(p => p.level == this).ToList();
        }

        #region ==Physics==

        public string foundInfo(ushort x, ushort y, ushort z) {
            Check foundCheck = null;
            try {
                foundCheck = ListCheck.Find(Check => Check.b == PosToInt(x, y, z));
            } catch {
            }
            if (foundCheck != null)
                return foundCheck.extraInfo;
            return "";
        }

        public void CalcPhysics() {
            if (physics == 0) return;
            try {
                ushort x, y, z;
                lastCheck = ListCheck.Count;
                if (physics == 5) {
                    ListCheck.ForEach(
                        delegate(Check C)
                        {
                            try {
                                IntToPos(C.b, out x, out y, out z);
                                if (PhysicsUpdate != null)
                                    PhysicsUpdate(x, y, z, C.time, C.extraInfo, this);
                                
                                if (C.extraInfo == "" || ExtraInfoPhysics.DoDoorsOnly(this, C, null))
                                    DoorPhysics.Do(this, C);
                            } catch {
                                ListCheck.Remove(C);
                            }
                        });
                } else {
                    var rand = new Random();
                    ListCheck.ForEach(
                        delegate(Check C)
                        {
                            try {
                                IntToPos(C.b, out x, out y, out z);
                                string foundInfo = C.extraInfo;
                                if (PhysicsUpdate != null)
                                    PhysicsUpdate(x, y, z, C.time, C.extraInfo, this);
                                OnPhysicsUpdateEvent.Call(x, y, z, C.time, C.extraInfo, this);
                                
                                if (C.extraInfo == "" || ExtraInfoPhysics.DoComplex(this, C, rand))
                                    DoNormalPhysics(x, y, z, rand, C);
                            } catch {
                                ListCheck.Remove(C);
                            }
                        });
                }
                
                ListCheck.RemoveAll(Check => Check.time == 255); //Remove all that are finished with 255 time
                lastUpdate = ListUpdate.Count;
                ListUpdate.ForEach(
                    delegate(Update C)
                    {
                        try {
                            Blockchange(C.b, C.type, false, C.extraInfo);
                        } catch {
                            Server.s.Log("Phys update issue");
                        }
                    });
                ListUpdate.Clear();
            } catch (Exception e) {
                Server.s.Log("Level physics error");
                Server.ErrorLog(e);
            }
        }
        
        void DoNormalPhysics(ushort x, ushort y, ushort z, Random rand, Check C) {
            switch (blocks[C.b])
            {
                case Block.air: //Placed air
                    AirPhysics.DoAir(this, C, rand);
                    break;
                case Block.dirt: //Dirt
                    if (!GrassGrow)
                    {
                        C.time = 255;
                        break;
                    }

                    if (C.time > 20)
                    {
                        if (Block.LightPass(GetTile(x, (ushort)(y + 1), z)))
                        {
                            AddUpdate(C.b, Block.grass);
                        }
                        C.time = 255;
                    }
                    else
                    {
                        C.time++;
                    }
                    break;

                case Block.leaf:
                    if (physics > 1)
                        //Adv physics kills flowers and mushroos in water/lava
                    {
                        PhysAir(PosToInt((ushort)(x + 1), y, z));
                        PhysAir(PosToInt((ushort)(x - 1), y, z));
                        PhysAir(PosToInt(x, y, (ushort)(z + 1)));
                        PhysAir(PosToInt(x, y, (ushort)(z - 1)));
                        PhysAir(PosToInt(x, (ushort)(y + 1), z));
                        //Check block above
                    }

                    if (!leafDecay)
                    {
                        C.time = 255;
                        leaves.Clear();
                        break;
                    }
                    if (C.time < 5)
                    {
                        if (rand.Next(10) == 0) C.time++;
                        break;
                    }
                    if (PhysLeaf(C.b)) AddUpdate(C.b, 0);
                    C.time = 255;
                    break;

                case Block.shrub:
                    if (physics > 1)
                        //Adv physics kills flowers and mushroos in water/lava
                    {
                        PhysAir(PosToInt((ushort)(x + 1), y, z));
                        PhysAir(PosToInt((ushort)(x - 1), y, z));
                        PhysAir(PosToInt(x, y, (ushort)(z + 1)));
                        PhysAir(PosToInt(x, y, (ushort)(z - 1)));
                        PhysAir(PosToInt(x, (ushort)(y + 1), z));
                        //Check block above
                    }

                    if (!growTrees)
                    {
                        C.time = 255;
                        break;
                    }
                    if (C.time < 20)
                    {
                        if (rand.Next(20) == 0) C.time++;
                        break;
                    }
                    Server.MapGen.AddTree(this, x, y, z, rand, true, false);
                    C.time = 255;
                    break;

                case Block.water:
                case Block.activedeathwater:
                    LiquidPhysics.DoWater(this, C, rand);
                    break;
                case Block.WaterDown:
                    ExtLiquidPhysics.DoWaterfall(this, C, rand);
                    break;
                case Block.LavaDown:
                    ExtLiquidPhysics.DoLavafall(this, C, rand);
                    break;
                case Block.WaterFaucet:
                    ExtLiquidPhysics.DoFaucet(this, C, rand, Block.WaterDown);
                    break;
                case Block.LavaFaucet:
                    ExtLiquidPhysics.DoFaucet(this, C, rand, Block.LavaDown);
                    break;
                case Block.lava:
                case Block.activedeathlava:
                    LiquidPhysics.DoLava(this, C, rand);
                    break;
                case Block.fire:
                    FirePhysics.Do(this, C, rand);
                    break;
                case Block.finiteWater:
                case Block.finiteLava:
                    FinitePhysics.DoWaterOrLava(this, C, rand);
                    break;
                case Block.finiteFaucet:
                    FinitePhysics.DoFaucet(this, C, rand);
                    break;
                case Block.sand:
                case Block.gravel:
                    if (PhysSand(C.b, blocks[C.b]))
                    {
                        PhysAir(PosToInt((ushort)(x + 1), y, z));
                        PhysAir(PosToInt((ushort)(x - 1), y, z));
                        PhysAir(PosToInt(x, y, (ushort)(z + 1)));
                        PhysAir(PosToInt(x, y, (ushort)(z - 1)));
                        PhysAir(PosToInt(x, (ushort)(y + 1), z));
                    }
                    C.time = 255;
                    break;
                case Block.sponge: //SPONGE
                    PhysSponge(C.b);
                    C.time = 255;
                    break;
                case Block.lava_sponge: //SPONGE
                    PhysSponge(C.b, true);
                    C.time = 255;
                    break;
                    //Adv physics updating anything placed next to water or lava
                case Block.wood: //Wood to die in lava
                case Block.trunk: //Wood to die in lava
                case Block.yellowflower:
                case Block.redflower:
                case Block.mushroom:
                case Block.redmushroom:
                case Block.bookcase: //bookcase
                case Block.red: //Shitload of cloth
                case Block.orange:
                case Block.yellow:
                case Block.lightgreen:
                case Block.green:
                case Block.aquagreen:
                case Block.cyan:
                case Block.lightblue:
                case Block.blue:
                case Block.purple:
                case Block.lightpurple:
                case Block.pink:
                case Block.darkpink:
                case Block.darkgrey:
                case Block.lightgrey:
                case Block.white:
                    if (physics > 1)
                        //Adv physics kills flowers and mushroos in water/lava
                    {
                        PhysAir(PosToInt((ushort)(x + 1), y, z));
                        PhysAir(PosToInt((ushort)(x - 1), y, z));
                        PhysAir(PosToInt(x, y, (ushort)(z + 1)));
                        PhysAir(PosToInt(x, y, (ushort)(z - 1)));
                        PhysAir(PosToInt(x, (ushort)(y + 1), z));
                        //Check block above
                    }
                    C.time = 255;
                    break;

                case Block.staircasestep:
                case Block.cobblestoneslab:
                    PhysStair(C.b);
                    C.time = 255;
                    break;
                case Block.wood_float: //wood_float
                    PhysFloatwood(C.b);
                    C.time = 255;
                    break;
                case Block.lava_fast: //lava_fast
                case Block.fastdeathlava:
                    LiquidPhysics.DoFastLava(this, C, rand);
                    break;

                    //Special blocks that are not saved
                case Block.air_flood: //air_flood
                    AirPhysics.DoFlood(this, C, rand, AirFlood.Full, Block.air_flood);
                    break;
                case Block.air_flood_layer:
                    AirPhysics.DoFlood(this, C, rand, AirFlood.Layer, Block.air_flood_layer);
                    break;
                case Block.air_flood_down:
                    AirPhysics.DoFlood(this, C, rand, AirFlood.Down, Block.air_flood_down);
                    break;
                case Block.air_flood_up:
                    AirPhysics.DoFlood(this, C, rand, AirFlood.Up, Block.air_flood_up);
                    break;
                case Block.smalltnt:
                    TntPhysics.DoSmallTnt(this, C, rand);
                    break;
                case Block.bigtnt:
                    TntPhysics.DoLargeTnt(this, C, rand, 1);
                    break;
                case Block.nuketnt:
                    TntPhysics.DoLargeTnt(this, C, rand, 4);
                    break;
                case Block.tntexplosion:
                    if (rand.Next(1, 11) <= 7)
                        AddUpdate(C.b, Block.air);
                    break;
                case Block.train:
                    TrainPhysics.Do(this, C, rand);
                    break;
                case Block.magma:
                    ExtLiquidPhysics.DoMagma(this, C, rand);
                    break;
                case Block.geyser:
                    ExtLiquidPhysics.DoGeyser(this, C, rand);
                    break;
                case Block.birdblack:
                case Block.birdwhite:
                case Block.birdlava:
                case Block.birdwater:
                    BirdPhysics.Do(this, C, rand);
                    break;
                case Block.snaketail:
                    if (GetTile(IntOffset(C.b, -1, 0, 0)) != Block.snake ||
                        GetTile(IntOffset(C.b, 1, 0, 0)) != Block.snake ||
                        GetTile(IntOffset(C.b, 0, 0, 1)) != Block.snake ||
                        GetTile(IntOffset(C.b, 0, 0, -1)) != Block.snake)
                        C.extraInfo = "revert 0";
                    break;
                case Block.snake:
                    SnakePhysics.Do(this, C, rand);
                    break;
                case Block.birdred:
                case Block.birdblue:
                case Block.birdkill:
                    HunterPhysics.DoKiller(this, C, rand, Block.air);
                    break;
                case Block.fishbetta:
                case Block.fishshark:
                    HunterPhysics.DoKiller(this, C, rand, Block.water);
                    break;
                case Block.fishgold:
                case Block.fishsalmon:
                case Block.fishsponge:
                    HunterPhysics.DoFlee(this, C, rand, Block.water);
                    break;
                case Block.fishlavashark:
                    HunterPhysics.DoKiller(this, C, rand, Block.lava);
                    break;
                case Block.rockethead:
                    RocketPhysics.Do(this, C, rand);
                    break;
                case Block.firework:
                    FireworkPhysics.Do(this, C, rand);
                    break;
                case Block.zombiehead:
                    if (GetTile(IntOffset(C.b, 0, -1, 0)) != Block.zombiebody &&
                        GetTile(IntOffset(C.b, 0, -1, 0)) != Block.creeper)
                        C.extraInfo = "revert 0";
                    break;
                case Block.zombiebody:
                case Block.creeper:
                    ZombiePhysics.Do(this, C, rand);
                    break;

                case Block.c4:
                    C4.C4s c4 = C4.Find(this, C.p.c4circuitNumber);
                    if (c4 != null)
                    {
                        C4.C4s.OneC4 one = new C4.C4s.OneC4(x, y, z);
                        c4.list.Add(one);
                    }
                    C.time = 255;
                    break;

                case Block.c4det:
                    C4.C4s c = C4.Find(this, C.p.c4circuitNumber);
                    if (c != null)
                    {
                        c.detenator[0] = x;
                        c.detenator[1] = y;
                        c.detenator[2] = z;
                    }
                    C.p.c4circuitNumber = -1;
                    C.time = 255;
                    break;

                default:
                    DoorPhysics.Do(this, C);
                    break;
            }
        }

        public void AddCheck(int b, string extraInfo = "", bool overRide = false, MCGalaxy.Player Placer = null)
        {
            try
            {
                if (!ListCheck.Exists(Check => Check.b == b))
                {
                    ListCheck.Add(new Check(b, extraInfo, Placer)); //Adds block to list to be updated
                }
                else
                {
                    if (overRide)
                    {
                        foreach (Check C2 in ListCheck)
                        {
                            if (C2.b == b)
                            {
                                C2.extraInfo = extraInfo; //Dont need to check physics here because if the list is active, then physics is active :)
                                return;
                            }
                        }
                    }
                }
                if (!physicssate && physics > 0)
                    StartPhysics();
            }
            catch
            {
                //s.Log("Warning-PhysicsCheck");
                //ListCheck.Add(new Check(b));    //Lousy back up plan
            }
        }

        internal bool AddUpdate(int b, int type, bool overRide = false, string extraInfo = "")
        {
            try
            {
                if (overRide)
                {
                    ushort x, y, z;
                    IntToPos(b, out x, out y, out z);
                    AddCheck(b, extraInfo, true); //Dont need to check physics here....AddCheck will do that
                    Blockchange(x, y, z, (byte)type, true, extraInfo);
                    return true;
                }

                if (!ListUpdate.Exists(Update => Update.b == b))
                {
                    ListUpdate.Add(new Update(b, (byte)type, extraInfo));
                    if (!physicssate && physics > 0)
                        StartPhysics();
                    return true;
                }
                else
                {
                    if (type == 12 || type == 13)
                    {
                        ListUpdate.RemoveAll(Update => Update.b == b);
                        ListUpdate.Add(new Update(b, (byte)type, extraInfo));
                        if (!physicssate && physics > 0)
                            StartPhysics();
                        return true;
                    }
                }

                return false;
            }
            catch
            {
                //s.Log("Warning-PhysicsUpdate");
                //ListUpdate.Add(new Update(b, (byte)type));    //Lousy back up plan
                return false;
            }
        }

        public void ClearPhysics()
        {
            ushort x, y, z;
            ListCheck.ForEach(delegate(Check C)
                              {
                                  IntToPos(C.b, out x, out y, out z);
                                  //attemps on shutdown to change blocks back into normal selves that are active, hopefully without needing to send into to clients.
                                  switch (blocks[C.b])
                                  {
                                      case Block.air_flood:
                                      case Block.air_flood_layer:
                                      case Block.air_flood_down:
                                      case Block.air_flood_up:
                                          blocks[C.b] = 0;
                                          break;
                                      case Block.door_air:
                                          //blocks[C.b] = 111;
                                          Blockchange(x, y, z, Block.door);
                                          break;
                                      case Block.door2_air:
                                          //blocks[C.b] = 113;
                                          Blockchange(x, y, z, Block.door2);
                                          break;
                                      case Block.door3_air:
                                          //blocks[C.b] = 114;
                                          Blockchange(x, y, z, Block.door3);
                                          break;
                                      case Block.door4_air:
                                          //blocks[C.b] = 115;
                                          Blockchange(x, y, z, Block.door4);
                                          break;
                                  }

                                  try
                                  {
                                      if (C.extraInfo.Contains("revert"))
                                      {
                                          int i = 0;
                                          foreach (string s in C.extraInfo.Split(' '))
                                          {
                                              if (s == "revert")
                                              {
                                                  Blockchange(x, y, z, Byte.Parse(C.extraInfo.Split(' ')[i + 1]), true);
                                                  break;
                                              }
                                              i++;
                                          }
                                      }
                                  }
                                  catch (Exception e)
                                  {
                                      Server.ErrorLog(e);
                                  }
                              });

            ListCheck.Clear();
            ListUpdate.Clear();
        }

        internal void PhysAir(int b) { AirPhysics.PhysAir(this, b); }
        //================================================================================================================
        internal void PhysWater(int b, byte type)
        {
            if (b == -1)
                return;
            ushort x, y, z;
            IntToPos(b, out x, out y, out z);
            if (Server.lava.active && Server.lava.map == this && Server.lava.InSafeZone(x, y, z))
                return;

            switch (blocks[b])
            {
                case Block.air:
                    if (!PhysSpongeCheck(b))
                        AddUpdate(b, type);
                    break;

                case Block.lava:
                case Block.lava_fast:
                case Block.activedeathlava:
                    if (!PhysSpongeCheck(b))
                        AddUpdate(b, Block.rock);
                    break;

                case Block.shrub:
                case Block.yellowflower:
                case Block.redflower:
                case Block.mushroom:
                case Block.redmushroom:
                    if (physics > 1 && physics != 5 && !PhysSpongeCheck(b))
                        AddUpdate(b, 0); //Adv physics kills flowers and mushrooms in water
                    break;

                case Block.sand:
                case Block.gravel:
                case Block.wood_float:
                    AddCheck(b);
                    break;

                default:
                    break;
            }
        }

        //================================================================================================================
        internal void PhysLava(int b, byte type)
        {
            if (b == -1)
            {
                return;
            }
            ushort x, y, z;
            IntToPos(b, out x, out y, out z);
            if (Server.lava.active && Server.lava.map == this && Server.lava.InSafeZone(x, y, z))
            {
                return;
            }

            if (physics > 1 && physics != 5 && !PhysSpongeCheck(b, true) && blocks[b] >= 21 && blocks[b] <= 36)
            {
                AddUpdate(b, 0);
                return;
            } // Adv physics destroys cloth
            switch (blocks[b])
            {
                case 0:
                    if (!PhysSpongeCheck(b, true)) AddUpdate(b, type);
                    break;

                case 8: //hit active_water
                case Block.activedeathwater:
                    if (!PhysSpongeCheck(b, true)) AddUpdate(b, 1);
                    break;

                case 12: //sand
                    if (physics > 1) //Adv physics changes sand to glass next to lava
                    {
                        if (physics != 5)
                        {
                            AddUpdate(b, 20);
                        }
                    }
                    else
                    {
                        AddCheck(b);
                    }
                    break;

                case 13: //gravel
                    AddCheck(b);
                    break;

                case 5:
                case 6:
                case 17:
                case 18:
                case 37:
                case 38:
                case 39:
                case 40:
                    if (physics > 1 && physics != 5) //Adv physics kills flowers and mushrooms plus wood in lava
                        if (!PhysSpongeCheck(b, true)) AddUpdate(b, 0);
                    break;

                default:
                    break;
            }
        }

        //================================================================================================================
        private bool PhysSand(int b, byte type) //also does gravel
        {
            if (b == -1 || physics == 0) return false;
            if (physics == 5) return false;

            int tempb = b;
            bool blocked = false;
            bool moved = false;

            do
            {
                tempb = IntOffset(tempb, 0, -1, 0); //Get block below each loop
                if (GetTile(tempb) != Block.Zero)
                {
                    switch (blocks[tempb])
                    {
                        case 0: //air lava water
                        case 8:
                        case 10:
                            moved = true;
                            break;

                        case 6:
                        case 37:
                        case 38:
                        case 39:
                        case 40:
                            if (physics > 1 && physics != 5) //Adv physics crushes plants with sand
                            {
                                moved = true;
                            }
                            else
                            {
                                blocked = true;
                            }
                            break;

                        default:
                            blocked = true;
                            break;
                    }
                    if (physics > 1)
                    {
                        if (physics != 5)
                        {
                            blocked = true;
                        }
                    }
                }
                else
                {
                    blocked = true;
                }
            } while (!blocked);

            if (moved)
            {
                AddUpdate(b, 0);
                if (physics > 1)
                {
                    AddUpdate(tempb, type);
                }
                else
                {
                    AddUpdate(IntOffset(tempb, 0, 1, 0), type);
                }
            }

            return moved;
        }

        internal void PhysSandCheck(int b) //also does gravel
        {
            if (b == -1)
            {
                return;
            }
            switch (blocks[b])
            {
                case 12: //sand
                case 13: //gravel
                case 110: //wood_float
                    AddCheck(b);
                    break;

                default:
                    break;
            }
        }

        //================================================================================================================
        void PhysStair(int b) {
            int bBelow = IntOffset(b, 0, -1, 0);
            byte tile = GetTile(bBelow);
            
            if (tile == Block.staircasestep) {
                AddUpdate(b, 0);
                AddUpdate(bBelow, Block.staircasefull);
            } else if (tile == Block.cobblestoneslab) {
                AddUpdate(b, 0);
                AddUpdate(bBelow, Block.stone);
            }
        }

        //================================================================================================================
        internal bool PhysSpongeCheck(int b, bool lava = false) //return true if sponge is near
        {
            for (int y = -2; y <= +2; ++y)
                for (int z = -2; z <= +2; ++z)
                    for (int x = -2; x <= +2; ++x)
            {
                byte block = GetTile(IntOffset(b, x, y, z));
                if (block == Block.Zero) continue;
                if ((!lava && block == Block.sponge) || (lava && block == Block.lava_sponge))
                    return true;
            }
            return false;
        }

        //================================================================================================================
        private void PhysSponge(int b, bool lava = false) //turn near water into air when placed
        {
            int temp = 0;
            for (int x = -2; x <= +2; ++x)
            {
                for (int y = -2; y <= +2; ++y)
                {
                    for (int z = -2; z <= +2; ++z)
                    {
                        temp = IntOffset(b, x, y, z);
                        if (GetTile(temp) != Block.Zero)
                        {
                            if ((!lava && Block.Convert(GetTile(temp)) == 8) ||
                                (lava && Block.Convert(GetTile(temp)) == 10))
                            {
                                AddUpdate(temp, 0);
                            }
                        }
                    }
                }
            }
        }

        //================================================================================================================
        public void PhysSpongeRemoved(int b, bool lava = false) //Reactivates near water
        {
            int temp = 0;
            for (int x = -3; x <= +3; ++x)
            {
                for (int y = -3; y <= +3; ++y)
                {
                    for (int z = -3; z <= +3; ++z)
                    {
                        if (Math.Abs(x) == 3 || Math.Abs(y) == 3 || Math.Abs(z) == 3) //Calc only edge
                        {
                            temp = IntOffset(b, x, y, z);
                            if (GetTile(temp) != Block.Zero)
                            {
                                if ((!lava && Block.Convert(GetTile(temp)) == 8) ||
                                    (lava && Block.Convert(GetTile(temp)) == 10))
                                {
                                    AddCheck(temp);
                                }
                            }
                        }
                    }
                }
            }
        }

        //================================================================================================================
        private void PhysFloatwood(int b)
        {
            int tempb = IntOffset(b, 0, -1, 0); //Get block below
            if (GetTile(tempb) != Block.Zero)
            {
                if (GetTile(tempb) == 0)
                {
                    AddUpdate(b, 0);
                    AddUpdate(tempb, 110);
                    return;
                }
            }

            tempb = IntOffset(b, 0, 1, 0); //Get block above
            if (GetTile(tempb) != Block.Zero)
            {
                if (Block.Convert(GetTile(tempb)) == 8)
                {
                    AddUpdate(b, 8);
                    AddUpdate(tempb, 110);
                    return;
                }
            }
        }

        //================================================================================================================
        private bool PhysLeaf(int b)
        {
            byte type, dist = 4;
            int i, xx, yy, zz;
            ushort x, y, z;
            IntToPos(b, out x, out y, out z);

            for (xx = -dist; xx <= dist; xx++)
            {
                for (yy = -dist; yy <= dist; yy++)
                {
                    for (zz = -dist; zz <= dist; zz++)
                    {
                        type = GetTile((ushort)(x + xx), (ushort)(y + yy), (ushort)(z + zz));
                        if (type == Block.trunk)
                            leaves[PosToInt((ushort)(x + xx), (ushort)(y + yy), (ushort)(z + zz))] = 0;
                        else if (type == Block.leaf)
                            leaves[PosToInt((ushort)(x + xx), (ushort)(y + yy), (ushort)(z + zz))] = -2;
                        else
                            leaves[PosToInt((ushort)(x + xx), (ushort)(y + yy), (ushort)(z + zz))] = -1;
                    }
                }
            }

            for (i = 1; i <= dist; i++)
            {
                for (xx = -dist; xx <= dist; xx++)
                {
                    for (yy = -dist; yy <= dist; yy++)
                    {
                        for (zz = -dist; zz <= dist; zz++)
                        {
                            try
                            {
                                if (leaves[PosToInt((ushort)(x + xx), (ushort)(y + yy), (ushort)(z + zz))] == i - 1)
                                {
                                    if (
                                        leaves.ContainsKey(PosToInt((ushort)(x + xx - 1), (ushort)(y + yy),
                                                                    (ushort)(z + zz))) &&
                                        leaves[PosToInt((ushort)(x + xx - 1), (ushort)(y + yy), (ushort)(z + zz))] ==
                                        -2)
                                        leaves[PosToInt((ushort)(x + xx - 1), (ushort)(y + yy), (ushort)(z + zz))] =
                                            (sbyte)i;

                                    if (
                                        leaves.ContainsKey(PosToInt((ushort)(x + xx + 1), (ushort)(y + yy),
                                                                    (ushort)(z + zz))) &&
                                        leaves[PosToInt((ushort)(x + xx + 1), (ushort)(y + yy), (ushort)(z + zz))] ==
                                        -2)
                                        leaves[PosToInt((ushort)(x + xx + 1), (ushort)(y + yy), (ushort)(z + zz))] =
                                            (sbyte)i;

                                    if (
                                        leaves.ContainsKey(PosToInt((ushort)(x + xx), (ushort)(y + yy - 1),
                                                                    (ushort)(z + zz))) &&
                                        leaves[PosToInt((ushort)(x + xx), (ushort)(y + yy - 1), (ushort)(z + zz))] ==
                                        -2)
                                        leaves[PosToInt((ushort)(x + xx), (ushort)(y + yy - 1), (ushort)(z + zz))] =
                                            (sbyte)i;

                                    if (
                                        leaves.ContainsKey(PosToInt((ushort)(x + xx), (ushort)(y + yy + 1),
                                                                    (ushort)(z + zz))) &&
                                        leaves[PosToInt((ushort)(x + xx), (ushort)(y + yy + 1), (ushort)(z + zz))] ==
                                        -2)
                                        leaves[PosToInt((ushort)(x + xx), (ushort)(y + yy + 1), (ushort)(z + zz))] =
                                            (sbyte)i;

                                    if (
                                        leaves.ContainsKey(PosToInt((ushort)(x + xx), (ushort)(y + yy),
                                                                    (ushort)(z + zz - 1))) &&
                                        leaves[PosToInt((ushort)(x + xx), (ushort)(y + yy), (ushort)(z + zz - 1))] ==
                                        -2)
                                        leaves[PosToInt((ushort)(x + xx), (ushort)(y + yy), (ushort)(z + zz - 1))] =
                                            (sbyte)i;

                                    if (
                                        leaves.ContainsKey(PosToInt((ushort)(x + xx), (ushort)(y + yy),
                                                                    (ushort)(z + zz + 1))) &&
                                        leaves[PosToInt((ushort)(x + xx), (ushort)(y + yy), (ushort)(z + zz + 1))] ==
                                        -2)
                                        leaves[PosToInt((ushort)(x + xx), (ushort)(y + yy), (ushort)(z + zz + 1))] =
                                            (sbyte)i;
                                }
                            }
                            catch
                            {
                                Server.s.Log("Leaf decay error!");
                            }
                        }
                    }
                }
            }

            //Server.s.Log((leaves[b] < 0).ToString()); // This is a debug line that spams the console to hell!
            return leaves[b] < 0;
        }

        //================================================================================================================

        public void MakeExplosion(ushort x, ushort y, ushort z, int size, bool force = false, TntWarsGame CheckForExplosionZone = null)
        {
            //DateTime start = DateTime.Now;
            int xx, yy, zz;
            var rand = new Random();
            byte b;

            if (physics < 2 && force == false) return;
            if (physics == 5 && force == false) return;
            AddUpdate(PosToInt(x, y, z), Block.tntexplosion, true);

            for (xx = (x - (size + 1)); xx <= (x + (size + 1)); ++xx)
                for (yy = (y - (size + 1)); yy <= (y + (size + 1)); ++yy)
                    for (zz = (z - (size + 1)); zz <= (z + (size + 1)); ++zz)
                        try
            {
                b = GetTile((ushort)xx, (ushort)yy, (ushort)zz);
                if (b == Block.tnt)
                {
                    AddUpdate(PosToInt((ushort)xx, (ushort)yy, (ushort)zz), Block.smalltnt);
                }
                else if (b != Block.smalltnt && b != Block.bigtnt && b != Block.nuketnt)
                {
                    if (CheckForExplosionZone != null && b != Block.air)
                    {
                        if (CheckForExplosionZone.InZone((ushort)xx, (ushort)yy, (ushort)zz, false))
                        {
                            continue;
                        }
                    }
                    if (rand.Next(1, 11) <= 4)
                        AddUpdate(PosToInt((ushort)xx, (ushort)yy, (ushort)zz), Block.tntexplosion);
                    else if (rand.Next(1, 11) <= 8)
                        AddUpdate(PosToInt((ushort)xx, (ushort)yy, (ushort)zz), Block.air);
                    else
                        AddCheck(PosToInt((ushort)xx, (ushort)yy, (ushort)zz), "drop 50 dissipate 8");
                }
                else
                {
                    AddCheck(PosToInt((ushort)xx, (ushort)yy, (ushort)zz));
                }
            }
            catch
            {
            }

            for (xx = (x - (size + 2)); xx <= (x + (size + 2)); ++xx)
                for (yy = (y - (size + 2)); yy <= (y + (size + 2)); ++yy)
                    for (zz = (z - (size + 2)); zz <= (z + (size + 2)); ++zz)
            {
                b = GetTile((ushort)xx, (ushort)yy, (ushort)zz);
                if (rand.Next(1, 10) < 7)
                    if (Block.Convert(b) != Block.tnt)
                {
                    if (CheckForExplosionZone != null && b != Block.air)
                    {
                        if (CheckForExplosionZone.InZone((ushort)xx, (ushort)yy, (ushort)zz, false))
                        {
                            continue;
                        }
                    }
                    if (rand.Next(1, 11) <= 4)
                        AddUpdate(PosToInt((ushort)xx, (ushort)yy, (ushort)zz), Block.tntexplosion);
                    else if (rand.Next(1, 11) <= 8)
                        AddUpdate(PosToInt((ushort)xx, (ushort)yy, (ushort)zz), Block.air);
                    else
                        AddCheck(PosToInt((ushort)xx, (ushort)yy, (ushort)zz), "drop 50 dissipate 8");
                }
                if (b == Block.tnt)
                {
                    AddUpdate(PosToInt((ushort)xx, (ushort)yy, (ushort)zz), Block.smalltnt);
                }
                else if (b == Block.smalltnt || b == Block.bigtnt || b == Block.nuketnt)
                {
                    AddCheck(PosToInt((ushort)xx, (ushort)yy, (ushort)zz));
                }
            }

            for (xx = (x - (size + 3)); xx <= (x + (size + 3)); ++xx)
                for (yy = (y - (size + 3)); yy <= (y + (size + 3)); ++yy)
                    for (zz = (z - (size + 3)); zz <= (z + (size + 3)); ++zz)
            {
                b = GetTile((ushort)xx, (ushort)yy, (ushort)zz);
                if (rand.Next(1, 10) < 3)
                    if (Block.Convert(b) != Block.tnt)
                {
                    if (CheckForExplosionZone != null && b != Block.air)
                    {
                        if (CheckForExplosionZone.InZone((ushort)xx, (ushort)yy, (ushort)zz, false))
                        {
                            continue;
                        }
                    }
                    if (rand.Next(1, 11) <= 4)
                        AddUpdate(PosToInt((ushort)xx, (ushort)yy, (ushort)zz), Block.tntexplosion);
                    else if (rand.Next(1, 11) <= 8)
                        AddUpdate(PosToInt((ushort)xx, (ushort)yy, (ushort)zz), Block.air);
                    else
                        AddCheck(PosToInt((ushort)xx, (ushort)yy, (ushort)zz), "drop 50 dissipate 8");
                }
                if (b == Block.tnt)
                {
                    AddUpdate(PosToInt((ushort)xx, (ushort)yy, (ushort)zz), Block.smalltnt);
                }
                else if (b == Block.smalltnt || b == Block.bigtnt || b == Block.nuketnt)
                {
                    AddCheck(PosToInt((ushort)xx, (ushort)yy, (ushort)zz));
                }
            }
            //Server.s.Log("Explosion: " + (DateTime.Now - start).TotalMilliseconds.ToString());
        }

        #endregion

        public struct BlockPos
        {
            public DateTime TimePerformed;
            public string name;
            public int index;
            public bool deleted;
            public byte type, extType;
        }

        #region Nested type: UndoPos

        public struct UndoPos
        {
            public int location;
            public byte newType, newExtType;
            public byte oldType, oldExtType;
            public DateTime timePerformed;
        }

        #endregion

        #region Nested type: Zone

        public struct Zone
        {
            public string Owner;
            public ushort bigX, bigY, bigZ;
            public ushort smallX, smallY, smallZ;
        }

        #endregion

        public static class C4
        {
            public static void BlowUp(ushort[] detenator, Level lvl)
            {
                try
                {
                    foreach (C4s c4 in lvl.C4list)
                    {
                        if (c4.detenator[0] == detenator[0] && c4.detenator[1] == detenator[1] && c4.detenator[2] == detenator[2])
                        {
                            foreach (C4s.OneC4 c in c4.list)
                            {
                                lvl.MakeExplosion(c.pos[0], c.pos[1], c.pos[2], 0);
                            }
                            lvl.C4list.Remove(c4);
                        }
                    }
                }
                catch { }
            }
            public static sbyte NextCircuit(Level lvl)
            {
                sbyte number = 1;
                foreach (C4s c4 in lvl.C4list)
                {
                    number++;
                }
                return number;
            }
            public static C4s Find(Level lvl, sbyte CircuitNumber)
            {
                foreach (C4s c4 in lvl.C4list)
                {
                    if (c4.CircuitNumb == CircuitNumber)
                    {
                        return c4;
                    }
                }
                return null;
            }
            public class C4s
            {
                public sbyte CircuitNumb;
                public ushort[] detenator;
                public List<OneC4> list;
                public class OneC4
                {
                    public ushort[] pos = new ushort[3];
                    public OneC4(ushort x, ushort y, ushort z)
                    {
                        pos[0] = x;
                        pos[1] = y;
                        pos[2] = z;
                    }
                }
                public C4s(sbyte num)
                {
                    CircuitNumb = num;
                    list = new List<OneC4>();
                    detenator = new ushort[3];
                }
            }
        }
    }
}

//-------------------------------------------------------------------------------------------------------------------------------------------------------
public class Check
{
    public int b;
    public string extraInfo = "";
    public byte time;
    public MCGalaxy.Player p;

    public Check(int b, string extraInfo = "", MCGalaxy.Player placer = null)
    {
        this.b = b;
        time = 0;
        this.extraInfo = extraInfo;
        p = placer;
    }
}

//-------------------------------------------------------------------------------------------------------------------------------------------------------
public class Update
{
    public int b;
    public string extraInfo = "";
    public byte type;

    public Update(int b, byte type, string extraInfo = "")
    {
        this.b = b;
        this.type = type;
        this.extraInfo = extraInfo;
    }
}
