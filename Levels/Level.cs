/*
	Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCGalaxy)
	
	Dual-licensed under the	Educational Community License, Version 2.0 and
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

    public sealed class Level : IDisposable
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
        private readonly List<Check> ListCheck = new List<Check>(); //A list of blocks that need to be updated
        private readonly List<Update> ListUpdate = new List<Update>(); //A list of block to change after calculation

        private readonly Dictionary<int, sbyte> leaves = new Dictionary<int, sbyte>();
        // Holds block state for leaf decay

        private readonly Dictionary<int, bool[]> liquids = new Dictionary<int, bool[]>();
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
        public byte[] blocks;
        public byte weather;
        public string textureUrl = "";

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
        public ushort depth; // y       THIS IS STUPID, SHOULD HAVE BEEN Z
        public int drown = 70;
        public bool edgeWater;
        public int fall = 9;
        public bool finite;
        public bool fishstill;
        public bool growTrees;
        public bool guns = true;
        public ushort height; // z      THIS IS STUPID, SHOULD HAVE BEEN Y
        public int id;
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
        public ushort spawnx;
        public ushort spawny;
        public ushort spawnz;

        public int speedPhysics = 250;
        public LevelTextures textures;

        public string theme = "Normal";
        public bool unload = true;
        public ushort width; // x
        public bool worldChat = true;
        public bool bufferblocks = Server.bufferblocks;
        public List<BlockQueue.block> blockqueue = new List<BlockQueue.block>();
        private readonly object physThreadLock = new object();

        public List<C4.C4s> C4list = new List<C4.C4s>();

        public Level(string n, ushort x, ushort y, ushort z, string type, int seed = 0, bool useSeed = false)
        {
            //onLevelSave += null;
            width = x;
            depth = y;
            height = z;
            if (width < 16)
            {
                width = 16;
            }
            if (depth < 16)
            {
                depth = 16;
            }
            if (height < 16)
            {
                height = 16;
            }

            name = n;
            blocks = new byte[width * depth * height];
            ZoneList = new List<Zone>();

            var half = (ushort)(depth / 2);
            switch (type)
            {
                case "flat":
                    for (x = 0; x < width; ++x)
                        for (z = 0; z < height; ++z)
                            for (y = 0; y <= half; ++y)
                                SetTile(x, y, z, y < half ? Block.dirt : Block.grass);
                    //SetTile(x, y, z, (byte)(y != half ? (y >= half) ? 0 : 3 : 2));
                    break;
                case "pixel":
                    for (x = 0; x < width; ++x)
                        for (z = 0; z < height; ++z)
                            for (y = 0; y < depth; ++y)
                                if (y == 0)
                                    SetTile(x, y, z, 7);
                                else if (x == 0 || x == width - 1 || z == 0 || z == height - 1)
                                    SetTile(x, y, z, 36);
                    break;

                case "space":
                    Random rand = useSeed ? new Random(seed) : new Random();

                    for (x = 0; x < width; ++x)
                        for (z = 0; z < height; ++z)
                            for (y = 0; y < depth; ++y)
                                if (y == 0)
                                    SetTile(x, y, z, 7);
                                else if (x == 0 || x == width - 1 || z == 0 || z == height - 1 || y == 1 ||
                                         y == depth - 1)
                                    SetTile(x, y, z, rand.Next(100) == 0 ? Block.iron : Block.obsidian);
                    break;

                case "rainbow":
                    Random random = useSeed ? new Random(seed) : new Random();
                    for (x = 0; x < width; ++x)
                        for (z = 0; z < height; ++z)
                            for (y = 0; y < depth; ++y)
                                if (y == 0 || y == depth - 1 || x == 0 || x == width - 1 || z == 0 || z == height - 1)
                                    SetTile(x, y, z, (byte)random.Next(21, 36));

                    break;


                case "hell":
                    Random random2 = useSeed ? new Random(seed) : new Random();
                    for (x = 0; x < width; ++x)
                        for (z = 0; z < height; ++z)
                            for (y = 0; y < depth; ++y)
                                if (y == 0)
                                    SetTile(x, y, z, 7);
                                else if (x == 0 || x == width - 1 || z == 0 || z == height - 1 || y == 0 ||
                                         y == depth - 1)
                                    SetTile(x, y, z, Block.obsidian);
                                else if (x == 1 || x == width - 2 || z == 1 || z == height - 2)
                                {
                                    if (random2.Next(1000) == 7)
                                    {
                                        for (int i = 1; i < (depth - y); ++i)
                                        {
                                            SetTile(x, (ushort)(depth - i), z, Block.lava);
                                        }
                                    }
                                }

                    Server.MapGen.GenerateMap(this, type, seed, useSeed);
                    break;
                case "island":
                case "mountains":
                case "ocean":
                case "forest":
                case "desert":
                    Server.MapGen.GenerateMap(this, type, seed, useSeed);
                    break;

                //no need for default
            }
            spawnx = (ushort)(width / 2);
            spawny = (ushort)(depth * 0.75f);
            spawnz = (ushort)(height / 2);
            rotx = 0;
            roty = 0;
            textures = new LevelTextures(this);
            //season = new SeasonsCore(this);
        }

        public ushort length
        {
            get { return height; }
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

        public void CopyBlocks(byte[] source, int offset)
        {
            blocks = new byte[width * depth * height];
            Array.Copy(source, offset, blocks, 0, blocks.Length);

            for (int i = 0; i < blocks.Length; i++)
            {
                if (blocks[i] >= 50) blocks[i] = 0;
                switch (blocks[i])
                {
                    case Block.waterstill:
                        blocks[i] = Block.water;
                        break;
                    case Block.water:
                        blocks[i] = Block.waterstill;
                        break;
                    case Block.lava:
                        blocks[i] = Block.lavastill;
                        break;
                    case Block.lavastill:
                        blocks[i] = Block.lava;
                        break;
                }
            }
        }

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

                if (!silent) Player.GlobalMessageOps("&3" + name + Server.DefaultColor + " was unloaded.");
                Server.s.Log(string.Format("{0} was unloaded.", name));
            }
            return true;
        }

        public void saveChanges()
        {
            //if (!Server.useMySQL) return;
            if (blockCache.Count == 0) return;
            List<BlockPos> tempCache = new List<BlockPos>();
            tempCache.AddRange(blockCache);
            blockCache = new List<BlockPos>();

            string template = "INSERT INTO `Block" + name +
                              "` (Username, TimePerformed, X, Y, Z, type, deleted) VALUES ('{0}', '{1}', {2}, {3}, {4}, {5}, {6})";
            DatabaseTransactionHelper transaction = DatabaseTransactionHelper.Create();
            using (transaction)
            {
                foreach (BlockPos bP in tempCache)
                {
                    int deleted = bP.deleted ? 1 : 0;
                    transaction.Execute(String.Format(template, bP.name,
                                                      bP.TimePerformed.ToString("yyyy-MM-dd HH:mm:ss"), (int)bP.x,
                                                      (int)bP.y, (int)bP.z, bP.type, deleted));
                }
                transaction.Commit();
            }
            tempCache.Clear();
        }

        public byte GetTile(ushort x, ushort y, ushort z)
        {
            if (blocks == null) return Block.Zero;
            //if (PosToInt(x, y, z) >= blocks.Length) { return null; }
            //Avoid internal overflow
            return !InBound(x, y, z) ? Block.Zero : blocks[PosToInt(x, y, z)];
        }

        public byte GetTile(int b)
        {
            ushort x = 0, y = 0, z = 0;
            IntToPos(b, out x, out y, out z);
            return GetTile(x, y, z);
        }
        public void SetTile(int b, byte type)
        {
            if (blocks == null) return;
            if (b >= blocks.Length) return;
            if (b < 0) return;
            blocks[b] = type;
            //blockchanges[x + width * z + width * height * y] = pName;
        }
        public void SetTile(ushort x, ushort y, ushort z, byte type, Player p = null)
        {
            byte oldType = GetTile(x, y, z);
            if (blocks == null) return;
            if (!InBound(x, y, z)) return;
            blocks[PosToInt(x, y, z)] = type;
            if (p != null)
            {
                Level.BlockPos bP;
                bP.name = p.name;
                bP.TimePerformed = DateTime.Now;
                bP.x = x; bP.y = y; bP.z = z;
                bP.type = type;
                if (bP.type == 0)
                    bP.deleted = true;
                else
                    bP.deleted = false;
                blockCache.Add(bP);
                Player.UndoPos Pos;
                Pos.x = x;
                Pos.y = y;
                Pos.z = z;
                Pos.mapName = this.name;
                Pos.type = oldType;
                Pos.newtype = type;
                Pos.timePlaced = DateTime.Now;
                p.UndoBuffer.Add(Pos);
            }
        }

        public bool InBound(ushort x, ushort y, ushort z)
        {
            return x >= 0 && y >= 0 && z >= 0 && x < width && y < depth && z < height;
        }

        public static Level Find(string levelName)
        {
            Level tempLevel = null;
            bool returnNull = false;

            foreach (Level level in Server.levels)
            {
                if (level.name.ToLower() == levelName) return level;
                if (level.name.ToLower().IndexOf(levelName.ToLower(), System.StringComparison.Ordinal) == -1) continue;
                if (tempLevel == null) tempLevel = level;
                else returnNull = true;
            }

            return returnNull ? null : tempLevel;
        }

        public static Level FindExact(string levelName)
        {
            return Server.levels.Find(lvl => levelName.ToLower() == lvl.name.ToLower());
        }


        public void Blockchange(Player p, ushort x, ushort y, ushort z, byte type, bool addaction = true)
        {
            string errorLocation = "start";
        retry:
            try
            {
                if (x < 0 || y < 0 || z < 0) return;
                if (x >= width || y >= depth || z >= height) return;

                byte b = GetTile(x, y, z);

                errorLocation = "Block rank checking";
                if (!Block.AllowBreak(b))
                {
                    if (!Block.canPlace(p, b) && !Block.BuildIn(b))
                    {
                        p.SendBlockchange(x, y, z, b);
                        return;
                    }
                }
                errorLocation = "Allowed to place tnt there (TNT Wars)";
                if (type == Block.tnt || type == Block.smalltnt || type == Block.bigtnt || type == Block.nuketnt)
                {
                    if (p.PlayingTntWars)
                    {
                        if (TntWarsGame.GetTntWarsGame(p).InZone(x, y, z, true))
                        {
                            p.SendBlockchange(x, y, z, b);
                            return;
                        }
                    }
                }
                errorLocation = "Max tnt for TNT Wars checking";
                if (type == Block.tnt || type == Block.smalltnt || type == Block.bigtnt || type == Block.nuketnt)
                {
                    if (p.PlayingTntWars)
                    {
                        if (p.CurrentAmountOfTnt == TntWarsGame.GetTntWarsGame(p).TntPerPlayerAtATime)
                        {
                            p.SendBlockchange(x, y, z, b);
                            Player.SendMessage(p, "TNT Wars: Maximum amount of TNT placed");
                            return;
                        }
                        if (p.CurrentAmountOfTnt > TntWarsGame.GetTntWarsGame(p).TntPerPlayerAtATime)
                        {
                            p.SendBlockchange(x, y, z, b);
                            Player.SendMessage(p, "TNT Wars: You have passed the maximum amount of TNT that can be placed!");
                            return;
                        }
                        else
                        {
                            p.TntAtATime();
                        }
                    }
                }

                errorLocation = "TNT Wars switch TNT block to smalltnt";
                if ((type == Block.tnt || type == Block.bigtnt || type == Block.nuketnt || type == Block.smalltnt) && p.PlayingTntWars)
                {
                    type = Block.smalltnt;
                }

                errorLocation = "Zone checking";

                #region zones

                bool AllowBuild = true, foundDel = false, inZone = false;
                string Owners = "";
                var toDel = new List<Zone>();
                if ((p.group.Permission < LevelPermission.Admin || p.ZoneCheck || p.zoneDel) && !Block.AllowBreak(b))
                {
                    if (ZoneList.Count == 0) AllowBuild = true;
                    else
                    {
                        for (int index = 0; index < ZoneList.Count; index++)
                        {
                            Zone Zn = ZoneList[index];
                            if (Zn.smallX <= x && x <= Zn.bigX && Zn.smallY <= y && y <= Zn.bigY && Zn.smallZ <= z &&
                                z <= Zn.bigZ)
                            {
                                inZone = true;
                                if (p.zoneDel)
                                {
                                    //DB
                                    Database.executeQuery("DELETE FROM `Zone" + p.level.name + "` WHERE Owner='" +
                                                          Zn.Owner + "' AND SmallX='" + Zn.smallX + "' AND SMALLY='" +
                                                          Zn.smallY + "' AND SMALLZ='" + Zn.smallZ + "' AND BIGX='" +
                                                          Zn.bigX + "' AND BIGY='" + Zn.bigY + "' AND BIGZ='" + Zn.bigZ +
                                                          "'");
                                    toDel.Add(Zn);

                                    p.SendBlockchange(x, y, z, b);
                                    Player.SendMessage(p, "Zone deleted for &b" + Zn.Owner);
                                    foundDel = true;
                                }
                                else
                                {
                                    if (Zn.Owner.Substring(0, 3) == "grp")
                                    {
                                        if (Group.Find(Zn.Owner.Substring(3)).Permission <= p.group.Permission &&
                                            !p.ZoneCheck)
                                        {
                                            AllowBuild = true;
                                            break;
                                        }
                                        AllowBuild = false;
                                        Owners += ", " + Zn.Owner.Substring(3);
                                    }
                                    else
                                    {
                                        if (Zn.Owner.ToLower() == p.name.ToLower() && !p.ZoneCheck)
                                        {
                                            AllowBuild = true;
                                            break;
                                        }
                                        AllowBuild = false;
                                        Owners += ", " + Zn.Owner;
                                    }
                                }
                            }
                        }
                    }

                    if (p.zoneDel)
                    {
                        if (!foundDel) Player.SendMessage(p, "No zones found to delete.");
                        else
                        {
                            foreach (Zone Zn in toDel)
                            {
                                ZoneList.Remove(Zn);
                            }
                        }
                        p.zoneDel = false;
                        return;
                    }

                    if (!AllowBuild || p.ZoneCheck)
                    {
                        if (Owners != "") Player.SendMessage(p, "This zone belongs to &b" + Owners.Remove(0, 2) + ".");
                        else Player.SendMessage(p, "This zone belongs to no one.");

                        p.ZoneSpam = DateTime.Now;
                        p.SendBlockchange(x, y, z, b);

                        if (p.ZoneCheck) if (!p.staticCommands) p.ZoneCheck = false;
                        return;
                    }
                }

                #endregion

                errorLocation = "Map rank checking";
                if (Owners == "")
                {
                    if (p.group.Permission < permissionbuild && (!inZone || !AllowBuild))
                    {
                        p.SendBlockchange(x, y, z, b);
                        Player.SendMessage(p, "Must be at least " + PermissionToName(permissionbuild) + " to build here");
                        return;
                    }
                }

                errorLocation = "Map Max Rank Checking";
                if (Owners == "")
                {
                    if (p.group.Permission > perbuildmax && (!inZone || !AllowBuild))
                    {
                        if (!p.group.CanExecute(Command.all.Find("perbuildmax")))
                        {
                            p.SendBlockchange(x, y, z, b);
                            Player.SendMessage(p, "Your rank must be " + perbuildmax + " or lower to build here!");
                            return;
                        }
                    }
                }

                errorLocation = "Block sending";
                if (Block.Convert(b) != Block.Convert(type) && !Instant)
                    Player.GlobalBlockchange(this, x, y, z, type);

                if (b == Block.sponge && physics > 0 && type != Block.sponge) PhysSpongeRemoved(PosToInt(x, y, z));
                if (b == Block.lava_sponge && physics > 0 && type != Block.lava_sponge)
                    PhysSpongeRemoved(PosToInt(x, y, z), true);

                errorLocation = "Undo buffer filling";
                Player.UndoPos Pos;
                Pos.x = x;
                Pos.y = y;
                Pos.z = z;
                Pos.mapName = name;
                Pos.type = b;
                Pos.newtype = type;
                Pos.timePlaced = DateTime.Now;
                p.UndoBuffer.Add(Pos);

                errorLocation = "Setting tile";
                p.loginBlocks++;
                p.overallBlocks++;
                SetTile(x, y, z, type); //Updates server level blocks

                errorLocation = "Growing grass";
                if (GetTile(x, (ushort)(y - 1), z) == Block.grass && GrassDestroy && !Block.LightPass(type))
                {
                    Blockchange(p, x, (ushort)(y - 1), z, Block.dirt);
                }

                errorLocation = "Adding physics";
                if (p.PlayingTntWars && type == Block.smalltnt) AddCheck(PosToInt(x, y, z), "", false, p);
                if (physics > 0) if (Block.Physics(type)) AddCheck(PosToInt(x, y, z), "", false, p);

                changed = true;
                backedup = false;
            }
            catch (OutOfMemoryException)
            {
                Player.SendMessage(p, "Undo buffer too big! Cleared!");
                p.UndoBuffer.Clear();
                goto retry;
            }
            catch (Exception e)
            {
                Server.ErrorLog(e);
                Player.GlobalMessageOps(p.name + " triggered a non-fatal error on " + name);
                Player.GlobalMessageOps("Error location: " + errorLocation);
                Server.s.Log(p.name + " triggered a non-fatal error on " + name);
                Server.s.Log("Error location: " + errorLocation);
            }

            //if (addaction)
            //{
            //    if (edits.Count == edits.Capacity) { edits.Capacity += 1024; }
            //    if (p.actions.Count == p.actions.Capacity) { p.actions.Capacity += 128; }
            //    if (b.lastaction.Count == 5) { b.lastaction.RemoveAt(0); }
            //    Edit foo = new Edit(this); foo.block = b; foo.from = p.name;
            //    foo.before = b.type; foo.after = type;
            //    b.lastaction.Add(foo); edits.Add(foo); p.actions.Add(foo);
            //} b.type = type;
        }

        public static void SaveSettings(Level level)
        {
            try
            {
                File.Create("levels/level properties/" + level.name + ".properties").Dispose();
                File.Create("levels/level properties/" + level.name + ".env").Dispose();
                using (StreamWriter SW = File.CreateText("levels/level properties/" + level.name + ".properties"))
                {
                    SW.WriteLine("#Level properties for " + level.name);
                    SW.WriteLine("#Drown-time in seconds is [drown time] * 200 / 3 / 1000");
                    SW.WriteLine("Theme = " + level.theme);
                    SW.WriteLine("Physics = " + level.physics.ToString());
                    SW.WriteLine("Physics speed = " + level.speedPhysics.ToString());
                    SW.WriteLine("Physics overload = " + level.overload.ToString());
                    SW.WriteLine("Finite mode = " + level.finite.ToString());
                    SW.WriteLine("Animal AI = " + level.ai.ToString());
                    SW.WriteLine("Edge water = " + level.edgeWater.ToString());
                    SW.WriteLine("Survival death = " + level.Death.ToString());
                    SW.WriteLine("Fall = " + level.fall.ToString());
                    SW.WriteLine("Drown = " + level.drown.ToString());
                    SW.WriteLine("MOTD = " + level.motd);
                    SW.WriteLine("JailX = " + level.jailx.ToString());
                    SW.WriteLine("JailY = " + level.jaily.ToString());
                    SW.WriteLine("JailZ = " + level.jailz.ToString());
                    SW.WriteLine("Unload = " + level.unload.ToString());
                    SW.WriteLine("WorldChat = " + level.worldChat.ToString());
                    SW.WriteLine("PerBuild = " +
                                 (Group.Exists(PermissionToName(level.permissionbuild).ToLower())
                                      ? PermissionToName(level.permissionbuild).ToLower()
                                      : PermissionToName(LevelPermission.Builder)));
                    SW.WriteLine("PerVisit = " +
                                 (Group.Exists(PermissionToName(level.permissionvisit).ToLower())
                                      ? PermissionToName(level.permissionvisit).ToLower()
                                      : PermissionToName(LevelPermission.Guest)));
                    SW.WriteLine("PerBuildMax = " +
                                 (Group.Exists(PermissionToName(level.perbuildmax).ToLower())
                                      ? PermissionToName(level.perbuildmax).ToLower()
                                      : PermissionToName(LevelPermission.Nobody)));
                    SW.WriteLine("PerVisitMax = " +
                                 (Group.Exists(PermissionToName(level.pervisitmax).ToLower())
                                      ? PermissionToName(level.pervisitmax).ToLower()
                                      : PermissionToName(LevelPermission.Nobody)));
                    SW.WriteLine("Guns = " + level.guns.ToString());
                    SW.WriteLine("LoadOnGoto = " + level.loadOnGoto.ToString());
                    SW.WriteLine("LeafDecay = " + level.leafDecay.ToString());
                    SW.WriteLine("RandomFlow = " + level.randomFlow.ToString());
                    SW.WriteLine("GrowTrees = " + level.growTrees.ToString());
                    SW.WriteLine("Weather = " + level.weather.ToString());
                    SW.WriteLine("Texture = " + level.textureUrl);
                }
                try
                {
                    StreamWriter sw = new StreamWriter(File.Create("levels/level properties/" + level.name.ToLower() + ".env"));
                    if(level.CloudColor != null)
                        sw.WriteLine("CloudColor = " + level.CloudColor.ToString());
                    if (level.SkyColor != null)
                        sw.WriteLine("SkyColor = " + level.SkyColor.ToString());
                    if (level.LightColor != null)
                        sw.WriteLine("LightColor = " + level.LightColor.ToString());
                    if (level.ShadowColor != null) 
                        sw.WriteLine("ShadowColor = " + level.ShadowColor.ToString());
                    if (level.FogColor != null)
                        sw.WriteLine("FogColor = " + level.FogColor.ToString());
                    if (level.EdgeLevel != null)
                        sw.WriteLine("EdgeLevel = " + level.EdgeLevel.ToString());
                    if (level.EdgeBlock != null) 
                        sw.WriteLine("EdgeBlock = " + level.EdgeBlock.ToString());
                    if (level.HorizonBlock != null)
                        sw.WriteLine("HorizonBlock = " + level.HorizonBlock.ToString());
                    sw.Flush();
                    sw.Close();
                    sw.Dispose();
                }
                catch (Exception e)
                {
                    Server.s.Log("Failed to save environment properties");
                    Logger.WriteError(e);
                }
            }
            catch (Exception)
            {
                Server.s.Log("Failed to save level properties!");
            }
        }
        public void Blockchange(int b, byte type, bool overRide = false, string extraInfo = "")
        //Block change made by physics
        {
            if (b < 0) return;
            if (b >= blocks.Length) return;
            byte bb = GetTile(b);

            try
            {
                if (!overRide)
                    if (Block.OPBlocks(bb) || (Block.OPBlocks(type) && extraInfo != "")) return;

                if (Block.Convert(bb) != Block.Convert(type))
                    //Should save bandwidth sending identical looking blocks, like air/op_air changes.
                    Player.GlobalBlockchange(this, b, type);

                if (b == Block.sponge && physics > 0 && type != Block.sponge)
                    PhysSpongeRemoved(b);

                if (b == Block.lava_sponge && physics > 0 && type != Block.lava_sponge)
                    PhysSpongeRemoved(b, true);

                try
                {
                    UndoPos uP;
                    uP.location = b;
                    uP.newType = type;
                    uP.oldType = bb;
                    uP.timePerformed = DateTime.Now;

                    if (currentUndo > Server.physUndo)
                    {
                        currentUndo = 0;
                        UndoBuffer[currentUndo] = uP;
                    }
                    else if (UndoBuffer.Count < Server.physUndo)
                    {
                        currentUndo++;
                        UndoBuffer.Add(uP);
                    }
                    else
                    {
                        currentUndo++;
                        UndoBuffer[currentUndo] = uP;
                    }
                }
                catch
                {
                }

                SetTile(b, type); //Updates server level blocks

                if (physics > 0)
                    if (Block.Physics(type) || extraInfo != "") AddCheck(b, extraInfo);
            }
            catch
            {
                SetTile(b, type);
            }
        }
        public void Blockchange(ushort x, ushort y, ushort z, byte type, bool overRide = false, string extraInfo = "")
        //Block change made by physics
        {
            if (x < 0 || y < 0 || z < 0) return;
            if (x >= width || y >= depth || z >= height) return;
            byte b = GetTile(x, y, z);

            try
            {
                if (!overRide)
                    if (Block.OPBlocks(b) || (Block.OPBlocks(type) && extraInfo != "")) return;

                if (Block.Convert(b) != Block.Convert(type))
                    //Should save bandwidth sending identical looking blocks, like air/op_air changes.
                    Player.GlobalBlockchange(this, x, y, z, type);

                if (b == Block.sponge && physics > 0 && type != Block.sponge)
                    PhysSpongeRemoved(PosToInt(x, y, z));

                if (b == Block.lava_sponge && physics > 0 && type != Block.lava_sponge)
                    PhysSpongeRemoved(PosToInt(x, y, z), true);

                try
                {
                    UndoPos uP;
                    uP.location = PosToInt(x, y, z);
                    uP.newType = type;
                    uP.oldType = b;
                    uP.timePerformed = DateTime.Now;

                    if (currentUndo > Server.physUndo)
                    {
                        currentUndo = 0;
                        UndoBuffer[currentUndo] = uP;
                    }
                    else if (UndoBuffer.Count < Server.physUndo)
                    {
                        currentUndo++;
                        UndoBuffer.Add(uP);
                    }
                    else
                    {
                        currentUndo++;
                        UndoBuffer[currentUndo] = uP;
                    }
                }
                catch
                {
                }

                SetTile(x, y, z, type); //Updates server level blocks

                if (physics > 0)
                    if (Block.Physics(type) || extraInfo != "") AddCheck(PosToInt(x, y, z), extraInfo);
            }
            catch
            {
                SetTile(x, y, z, type);
            }
        }

        // Returns true if ListCheck does not already have an check in the position.
        // Useful for fireworks, which depend on two physics blocks being checked, one with extraInfo.
        public bool CheckClear(ushort x, ushort y, ushort z)
        {
            int b = PosToInt(x, y, z);
            return !ListCheck.Exists(Check => Check.b == b);
        }

        public void skipChange(ushort x, ushort y, ushort z, byte type)
        {
            if (x < 0 || y < 0 || z < 0) return;
            if (x >= width || y >= depth || z >= height) return;

            SetTile(x, y, z, type);
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
                    {
                        ClearPhysics();
                    }
                    using (FileStream fs = File.Create(string.Format("{0}.back", path)))
                    {
                        using (GZipStream gs = new GZipStream(fs, CompressionMode.Compress))
                        {

                            var header = new byte[16];
                            BitConverter.GetBytes(1874).CopyTo(header, 0);
                            gs.Write(header, 0, 2);

                            BitConverter.GetBytes(width).CopyTo(header, 0);
                            BitConverter.GetBytes(height).CopyTo(header, 2);
                            BitConverter.GetBytes(depth).CopyTo(header, 4);
                            BitConverter.GetBytes(spawnx).CopyTo(header, 6);
                            BitConverter.GetBytes(spawnz).CopyTo(header, 8);
                            BitConverter.GetBytes(spawny).CopyTo(header, 10);
                            header[12] = rotx;
                            header[13] = roty;
                            header[14] = (byte)permissionvisit;
                            header[15] = (byte)permissionbuild;
                            gs.Write(header, 0, header.Length);
                            var level = new byte[blocks.Length];
                            for (int i = 0; i < blocks.Length; ++i)
                            {
                                if (blocks[i] < 66)
                                //CHANGED THIS TO INCOPARATE SOME MORE SPACE THAT I NEEDED FOR THE door_orange_air ETC.
                                {
                                    level[i] = blocks[i];
                                }
                                else
                                {
                                    level[i] = Block.SaveConvert(blocks[i]);
                                }
                            }
                            gs.Write(level, 0, level.Length);
                            gs.Close();
                            File.Delete(string.Format("{0}.backup", path));
                            File.Copy(string.Format("{0}.back", path), path + ".backup");
                            File.Delete(path);
                            File.Move(string.Format("{0}.back", path), path);

                            SaveSettings(this);

                            Server.s.Log(string.Format("SAVED: Level \"{0}\". ({1}/{2}/{3})", name, players.Count,
                                                       Player.players.Count, Server.players));
                            changed = false;

                            gs.Dispose();
                            fs.Dispose();
                        }
                    }

                    // UNCOMPRESSED LEVEL SAVING! DO NOT USE!
                    /*using (FileStream fs = File.Create(path + ".wtf"))
                    {
                        byte[] header = new byte[16];
                        BitConverter.GetBytes(1874).CopyTo(header, 0);
                        fs.Write(header, 0, 2);

                        BitConverter.GetBytes(width).CopyTo(header, 0);
                        BitConverter.GetBytes(height).CopyTo(header, 2);
                        BitConverter.GetBytes(depth).CopyTo(header, 4);
                        BitConverter.GetBytes(spawnx).CopyTo(header, 6);
                        BitConverter.GetBytes(spawnz).CopyTo(header, 8);
                        BitConverter.GetBytes(spawny).CopyTo(header, 10);
                        header[12] = rotx; header[13] = roty;
                        header[14] = (byte)permissionvisit;
                        header[15] = (byte)permissionbuild;
                        fs.Write(header, 0, header.Length);
                        byte[] level = new byte[blocks.Length];
                        for (int i = 0; i < blocks.Length; ++i)
                        {
                            if (blocks[i] < 80)
                            {
                                level[i] = blocks[i];
                            }
                            else
                            {
                                level[i] = Block.SaveConvert(blocks[i]);
                            }
                        } fs.Write(level, 0, level.Length); fs.Close();
                    }*/
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
                FileStream fs = File.OpenRead(path);
                try
                {
                    var gs = new GZipStream(fs, CompressionMode.Decompress);
                    var ver = new byte[2];
                    gs.Read(ver, 0, ver.Length);
                    ushort version = BitConverter.ToUInt16(ver, 0);
                    var vars = new ushort[6];
                    var rot = new byte[2];

                    if (version == 1874)
                    {
                        var header = new byte[16];
                        gs.Read(header, 0, header.Length);

                        vars[0] = BitConverter.ToUInt16(header, 0);
                        vars[1] = BitConverter.ToUInt16(header, 2);
                        vars[2] = BitConverter.ToUInt16(header, 4);
                        vars[3] = BitConverter.ToUInt16(header, 6);
                        vars[4] = BitConverter.ToUInt16(header, 8);
                        vars[5] = BitConverter.ToUInt16(header, 10);

                        rot[0] = header[12];
                        rot[1] = header[13];

                        //level.permissionvisit = (LevelPermission)header[14];
                        //level.permissionbuild = (LevelPermission)header[15];
                    }
                    else
                    {
                        var header = new byte[12];
                        gs.Read(header, 0, header.Length);

                        vars[0] = version;
                        vars[1] = BitConverter.ToUInt16(header, 0);
                        vars[2] = BitConverter.ToUInt16(header, 2);
                        vars[3] = BitConverter.ToUInt16(header, 4);
                        vars[4] = BitConverter.ToUInt16(header, 6);
                        vars[5] = BitConverter.ToUInt16(header, 8);

                        rot[0] = header[10];
                        rot[1] = header[11];
                    }

                    var level = new Level(givenName, vars[0], vars[2], vars[1], "empty")
                                    {
                                        permissionbuild = (LevelPermission)30,
                                        spawnx = vars[3],
                                        spawnz = vars[4],
                                        spawny = vars[5],
                                        rotx = rot[0],
                                        roty = rot[1],
                                        name = givenName
                                    };


                    level.setPhysics(phys);

                    var blocks = new byte[level.width * level.height * level.depth];
                    gs.Read(blocks, 0, blocks.Length);
                    level.blocks = blocks;
                    gs.Close();
                    gs.Dispose();
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
                    }
                    catch (Exception e)
                    {
                        Server.ErrorLog(e);
                    }

                    try
                    {
                        string foundLocation;
                        foundLocation = "levels/level properties/" + level.name + ".properties";
                        if (!File.Exists(foundLocation))
                        {
                            foundLocation = "levels/level properties/" + level.name;
                        }
                        foreach (string line in File.ReadAllLines(foundLocation))
                        {
                            try
                            {
                                if (line[0] == '#') continue;
                                string value = line.Substring(line.IndexOf(" = ") + 3);

                                switch (line.Substring(0, line.IndexOf(" = ")).ToLower())
                                {
                                    case "theme":
                                        level.theme = value;
                                        break;
                                    case "physics":
                                        level.setPhysics(int.Parse(value));
                                        break;
                                    case "physics speed":
                                        level.speedPhysics = int.Parse(value);
                                        break;
                                    case "physics overload":
                                        level.overload = int.Parse(value);
                                        break;
                                    case "finite mode":
                                        level.finite = bool.Parse(value);
                                        break;
                                    case "animal ai":
                                        level.ai = bool.Parse(value);
                                        break;
                                    case "edge water":
                                        level.edgeWater = bool.Parse(value);
                                        break;
                                    case "survival death":
                                        level.Death = bool.Parse(value);
                                        break;
                                    case "fall":
                                        level.fall = int.Parse(value);
                                        break;
                                    case "drown":
                                        level.drown = int.Parse(value);
                                        break;
                                    case "motd":
                                        level.motd = value;
                                        break;
                                    case "jailx":
                                        level.jailx = ushort.Parse(value);
                                        break;
                                    case "jaily":
                                        level.jaily = ushort.Parse(value);
                                        break;
                                    case "jailz":
                                        level.jailz = ushort.Parse(value);
                                        break;
                                    case "unload":
                                        level.unload = bool.Parse(value);
                                        break;
                                    case "worldchat":
                                        level.worldChat = bool.Parse(value);
                                        break;
                                    case "perbuild":
                                        level.permissionbuild = PermissionFromName(value) != LevelPermission.Null ? PermissionFromName(value) : LevelPermission.Guest;
                                        break;
                                    case "pervisit":
                                        level.permissionvisit = PermissionFromName(value) != LevelPermission.Null ? PermissionFromName(value) : LevelPermission.Guest;
                                        break;
                                    case "perbuildmax":
                                        level.perbuildmax = PermissionFromName(value) != LevelPermission.Null ? PermissionFromName(value) : LevelPermission.Guest;
                                        break;
                                    case "pervisitmax":
                                        level.pervisitmax = PermissionFromName(value) != LevelPermission.Null ? PermissionFromName(value) : LevelPermission.Guest;
                                        break;
                                    case "guns":
                                        level.guns = bool.Parse(value);
                                        break;
                                    case "loadongoto":
                                        level.loadOnGoto = bool.Parse(value);
                                        break;
                                    case "leafdecay":
                                        level.leafDecay = bool.Parse(value);
                                        break;
                                    case "randomflow":
                                        level.randomFlow = bool.Parse(value);
                                        break;
                                    case "growtrees":
                                        level.growTrees = bool.Parse(value);
                                        break;
                                    case "weather": level.weather = byte.Parse(value); break;
                                    case "texture": level.textureUrl = value; break;
                                }
                            }
                            catch (Exception e)
                            {
                                Server.ErrorLog(e);
                            }
                        }
                        if(File.Exists(("levels/level properties/" + level.name + ".env")))
                        {
                            foreach (string line in File.ReadAllLines("levels/level properties/" + level.name + ".env"))
                            {
                                try
                                {
                                    if (line[0] == '#') continue;
                                    string value = line.Substring(line.IndexOf(" = ") + 3);

                                    switch (line.Substring(0, line.IndexOf(" = ")).ToLower())
                                    {
                                        case "cloudcolor": level.CloudColor = value; break;
                                        case "fogcolor": level.FogColor = value; break;
                                        case "skycolor": level.SkyColor = value; break;
                                        case "shadowcolor": level.ShadowColor = value; break;
                                        case "lightcolor": level.LightColor = value; break;
                                        case "edgeblock": level.EdgeBlock = byte.Parse(value); break;
                                        case "edgelevel": level.EdgeLevel = byte.Parse(value); break;
                                        case "horizonblock": level.HorizonBlock = byte.Parse(value); break;
                                    }
                                }
                                catch { }
                            }
                        }
                    }
                    catch
                    {
                    }

                    Server.s.Log(string.Format("Level \"{0}\" loaded.", level.name));
                    if (LevelLoaded != null)
                        LevelLoaded(level);
                    OnLevelLoadedEvent.Call(level);
                    return level;
                }
                catch (Exception ex)
                {
                    Server.ErrorLog(ex);
                    return null;
                }
                finally
                {
                    fs.Close();
                    fs.Dispose();
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
                        (pl.group.Permission >= Server.opchatperm || pl.isStaff )))
            {
                pl.SendMessage(message);
            }
        }

        public void ChatLevelAdmins(string message)
        {
            foreach (
                Player pl in
                    Player.players.Where(
                        pl =>
                        pl.level == this &&
                        (pl.group.Permission >= Server.adminchatperm || pl.isStaff)))
            {
                pl.SendMessage(message);
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
                PhysicsEnabled = true;
                physThread.Start();
                physicssate = true;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether physics are enabled.
        /// </summary>
        /// <value>
        ///   <c>true</c> if physics are enabled; otherwise, <c>false</c>.
        /// </value>
        public bool PhysicsEnabled { get; set; }

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

                    DateTime Start = DateTime.Now;

                    if (physics > 0) CalcPhysics();

                    TimeSpan Took = DateTime.Now - Start;
                    wait = speedPhysics - (int)Took.TotalMilliseconds;

                    if (wait < (int)(-overload * 0.75f))
                    {
                        Level Cause = this;

                        if (wait < -overload)
                        {
                            if (!Server.physicsRestart) Cause.setPhysics(0);
                            Cause.ClearPhysics();

                            Player.GlobalMessage("Physics shutdown on &b" + Cause.name);
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

        public int PosToInt(ushort x, ushort y, ushort z)
        {
            if (x < 0 || x >= width || y < 0 || y >= depth || z < 0 || z >= height)
                return -1;
            return x + (z * width) + (y * width * height);
            //alternate method: (h * widthY + y) * widthX + x;
        }

        public void IntToPos(int pos, out ushort x, out ushort y, out ushort z)
        {
            y = (ushort)(pos / width / height);
            pos -= y * width * height;
            z = (ushort)(pos / width);
            pos -= z * width;
            x = (ushort)pos;
        }

        public int IntOffset(int pos, int x, int y, int z)
        {
            return pos + x + z * width + y * width * height;
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

        public string foundInfo(ushort x, ushort y, ushort z)
        {
            Check foundCheck = null;
            try
            {
                foundCheck = ListCheck.Find(Check => Check.b == PosToInt(x, y, z));
            }
            catch
            {
            }
            if (foundCheck != null)
                return foundCheck.extraInfo;
            return "";
        }

        public void CalcPhysics()
        {
            try
            {
                if (physics == 5)
                {
                    #region == INCOMING ==

                    ushort x, y, z;

                    lastCheck = ListCheck.Count;
                    ListCheck.ForEach(delegate(Check C)
                                          {
                                              try
                                              {
                                                  IntToPos(C.b, out x, out y, out z);
                                                  string foundInfo = C.extraInfo;
                                                  if (PhysicsUpdate != null)
                                                  {
                                                      PhysicsUpdate(x, y, z, C.time, C.extraInfo, this);
                                                  }
                                              newPhysic:
                                                  if (foundInfo != "")
                                                  {
                                                      int currentLoop = 0;
                                                      if (!foundInfo.Contains("wait"))
                                                          if (blocks[C.b] == Block.air) C.extraInfo = "";

                                                      bool wait = false;
                                                      int waitnum = 0;
                                                      bool door = false;

                                                      foreach (string s in C.extraInfo.Split(' '))
                                                      {
                                                          if (currentLoop % 2 == 0)
                                                          {
                                                              //Type of code
                                                              switch (s)
                                                              {
                                                                  case "wait":
                                                                      wait = true;
                                                                      waitnum =
                                                                          int.Parse(
                                                                              C.extraInfo.Split(' ')[currentLoop + 1]);
                                                                      break;
                                                                  case "door":
                                                                      door = true;
                                                                      break;
                                                              }
                                                          }
                                                          currentLoop++;
                                                      }

                                                  startCheck:
                                                      if (wait)
                                                      {
                                                          int storedInt = 0;
                                                          if (door && C.time < 2)
                                                          {
                                                              storedInt = IntOffset(C.b, -1, 0, 0);
                                                              if (Block.tDoor(blocks[storedInt]))
                                                              {
                                                                  AddUpdate(storedInt, Block.air, false,
                                                                            "wait 10 door 1 revert " +
                                                                            blocks[storedInt].ToString());
                                                              }
                                                              storedInt = IntOffset(C.b, 1, 0, 0);
                                                              if (Block.tDoor(blocks[storedInt]))
                                                              {
                                                                  AddUpdate(storedInt, Block.air, false,
                                                                            "wait 10 door 1 revert " +
                                                                            blocks[storedInt].ToString());
                                                              }
                                                              storedInt = IntOffset(C.b, 0, 1, 0);
                                                              if (Block.tDoor(blocks[storedInt]))
                                                              {
                                                                  AddUpdate(storedInt, Block.air, false,
                                                                            "wait 10 door 1 revert " +
                                                                            blocks[storedInt].ToString());
                                                              }
                                                              storedInt = IntOffset(C.b, 0, -1, 0);
                                                              if (Block.tDoor(blocks[storedInt]))
                                                              {
                                                                  AddUpdate(storedInt, Block.air, false,
                                                                            "wait 10 door 1 revert " +
                                                                            blocks[storedInt].ToString());
                                                              }
                                                              storedInt = IntOffset(C.b, 0, 0, 1);
                                                              if (Block.tDoor(blocks[storedInt]))
                                                              {
                                                                  AddUpdate(storedInt, Block.air, false,
                                                                            "wait 10 door 1 revert " +
                                                                            blocks[storedInt].ToString());
                                                              }
                                                              storedInt = IntOffset(C.b, 0, 0, -1);
                                                              if (Block.tDoor(blocks[storedInt]))
                                                              {
                                                                  AddUpdate(storedInt, Block.air, false,
                                                                            "wait 10 door 1 revert " +
                                                                            blocks[storedInt].ToString());
                                                              }
                                                          }

                                                          if (waitnum <= C.time)
                                                          {
                                                              wait = false;
                                                              C.extraInfo =
                                                                  C.extraInfo.Substring(0, C.extraInfo.IndexOf("wait ")) +
                                                                  C.extraInfo.Substring(
                                                                      C.extraInfo.IndexOf(' ',
                                                                                          C.extraInfo.IndexOf("wait ") +
                                                                                          5) + 1);
                                                              //C.extraInfo = C.extraInfo.Substring(8);
                                                              goto startCheck;
                                                          }
                                                          C.time++;
                                                          foundInfo = "";
                                                          goto newPhysic;
                                                      }
                                                  }
                                                  else
                                                  {
                                                      switch (blocks[C.b])
                                                      {
                                                          case Block.door_air:
                                                          //door_air         Change any door blocks nearby into door_air
                                                          case Block.door2_air:
                                                          //door_air         Change any door blocks nearby into door_air
                                                          case Block.door3_air:
                                                          //door_air         Change any door blocks nearby into door_air
                                                          case Block.door4_air:
                                                          //door_air         Change any door blocks nearby into door_air
                                                          case Block.door5_air:
                                                          //door_air         Change any door blocks nearby into door_air
                                                          case Block.door6_air:
                                                          //door_air         Change any door blocks nearby into door_air
                                                          case Block.door7_air:
                                                          //door_air         Change any door blocks nearby into door_air
                                                          case Block.door8_air:
                                                          //door_air         Change any door blocks nearby into door_air
                                                          case Block.door10_air:
                                                          //door_air         Change any door blocks nearby into door_air
                                                          case Block.door12_air:
                                                          case Block.door13_air:
                                                          case Block.door_iron_air:
                                                          case Block.door_gold_air:
                                                          case Block.door_cobblestone_air:
                                                          case Block.door_red_air:

                                                          case Block.door_dirt_air:
                                                          case Block.door_grass_air:
                                                          case Block.door_blue_air:
                                                          case Block.door_book_air:
                                                              AnyDoor(C, x, y, z, 16);
                                                              break;
                                                          case Block.door11_air:
                                                          case Block.door14_air:
                                                              AnyDoor(C, x, y, z, 4, true);
                                                              break;
                                                          case Block.door9_air:
                                                              //door_air         Change any door blocks nearby into door_air
                                                              AnyDoor(C, x, y, z, 4);
                                                              break;

                                                          case Block.odoor1_air:
                                                          case Block.odoor2_air:
                                                          case Block.odoor3_air:
                                                          case Block.odoor4_air:
                                                          case Block.odoor5_air:
                                                          case Block.odoor6_air:
                                                          case Block.odoor7_air:
                                                          case Block.odoor8_air:
                                                          case Block.odoor9_air:
                                                          case Block.odoor10_air:
                                                          case Block.odoor11_air:
                                                          case Block.odoor12_air:

                                                          case Block.odoor1:
                                                          case Block.odoor2:
                                                          case Block.odoor3:
                                                          case Block.odoor4:
                                                          case Block.odoor5:
                                                          case Block.odoor6:
                                                          case Block.odoor7:
                                                          case Block.odoor8:
                                                          case Block.odoor9:
                                                          case Block.odoor10:
                                                          case Block.odoor11:
                                                          case Block.odoor12:
                                                              odoor(C);
                                                              break;
                                                          default:
                                                              //non special blocks are then ignored, maybe it would be better to avoid getting here and cutting down the list
                                                              if (!C.extraInfo.Contains("wait")) C.time = 255;
                                                              break;
                                                      }
                                                  }
                                              }
                                              catch
                                              {
                                                  ListCheck.Remove(C);
                                                  //Server.s.Log(e.Message);
                                              }
                                          });

                    ListCheck.RemoveAll(delegate(Check Check) { return Check.time == 255; }); //Remove all that are finished with 255 time

                    lastUpdate = ListUpdate.Count;
                    ListUpdate.ForEach(delegate(Update C)
                                           {
                                               try
                                               {
                                                   IntToPos(C.b, out x, out y, out z);
                                                   Blockchange(x, y, z, C.type, false, C.extraInfo);
                                               }
                                               catch
                                               {
                                                   Server.s.Log("Phys update issue");
                                               }
                                           });

                    ListUpdate.Clear();

                    #endregion

                    return;
                }
                if (physics > 0)
                {
                    #region == INCOMING ==

                    ushort x, y, z;
                    int mx, my, mz;

                    var rand = new Random();
                    lastCheck = ListCheck.Count;
                    ListCheck.ForEach(delegate(Check C)
                                          {
                                              try
                                              {
                                                  IntToPos(C.b, out x, out y, out z);
                                                  bool InnerChange = false;
                                                  bool skip = false;
                                                  int storedRand = 0;
                                                  Player foundPlayer = null;
                                                  int foundNum = 75, currentNum;
                                                  int oldNum;
                                                  string foundInfo = C.extraInfo;
                                                  if (PhysicsUpdate != null)
                                                      PhysicsUpdate(x, y, z, C.time, C.extraInfo, this);
                                                  OnPhysicsUpdateEvent.Call(x, y, z, C.time, C.extraInfo, this);
                                              newPhysic:
                                                  if (foundInfo != "")
                                                  {
                                                      int currentLoop = 0;
                                                      if (!foundInfo.Contains("wait"))
                                                          if (blocks[C.b] == Block.air) C.extraInfo = "";

                                                      bool drop = false;
                                                      int dropnum = 0;
                                                      bool wait = false;
                                                      int waitnum = 0;
                                                      bool dissipate = false;
                                                      int dissipatenum = 0;
                                                      bool revert = false;
                                                      byte reverttype = 0;
                                                      bool explode = false;
                                                      int explodenum = 0;
                                                      bool finiteWater = false;
                                                      bool rainbow = false;
                                                      int rainbownum = 0;
                                                      bool door = false;

                                                      foreach (string s in C.extraInfo.Split(' '))
                                                      {
                                                          if (currentLoop % 2 == 0)
                                                          {
                                                              //Type of code
                                                              switch (s)
                                                              {
                                                                  case "wait":
                                                                      wait = true;
                                                                      waitnum =
                                                                          int.Parse(
                                                                              C.extraInfo.Split(' ')[currentLoop + 1]);
                                                                      break;
                                                                  case "drop":
                                                                      drop = true;
                                                                      dropnum =
                                                                          int.Parse(
                                                                              C.extraInfo.Split(' ')[currentLoop + 1]);
                                                                      break;
                                                                  case "dissipate":
                                                                      dissipate = true;
                                                                      dissipatenum =
                                                                          int.Parse(
                                                                              C.extraInfo.Split(' ')[currentLoop + 1]);
                                                                      break;
                                                                  case "revert":
                                                                      revert = true;
                                                                      reverttype =
                                                                          byte.Parse(
                                                                              C.extraInfo.Split(' ')[currentLoop + 1]);
                                                                      break;
                                                                  case "explode":
                                                                      explode = true;
                                                                      explodenum =
                                                                          int.Parse(
                                                                              C.extraInfo.Split(' ')[currentLoop + 1]);
                                                                      break;

                                                                  case "finite":
                                                                      finiteWater = true;
                                                                      break;

                                                                  case "rainbow":
                                                                      rainbow = true;
                                                                      rainbownum =
                                                                          int.Parse(
                                                                              C.extraInfo.Split(' ')[currentLoop + 1]);
                                                                      break;

                                                                  case "door":
                                                                      door = true;
                                                                      break;
                                                              }
                                                          }
                                                          currentLoop++;
                                                      }

                                                  startCheck:
                                                      if (wait)
                                                      {
                                                          int storedInt = 0;
                                                          if (door && C.time < 2)
                                                          {
                                                              storedInt = IntOffset(C.b, -1, 0, 0);
                                                              if (Block.tDoor(blocks[storedInt]))
                                                              {
                                                                  AddUpdate(storedInt, Block.air, false,
                                                                            "wait 10 door 1 revert " +
                                                                            blocks[storedInt].ToString());
                                                              }
                                                              storedInt = IntOffset(C.b, 1, 0, 0);
                                                              if (Block.tDoor(blocks[storedInt]))
                                                              {
                                                                  AddUpdate(storedInt, Block.air, false,
                                                                            "wait 10 door 1 revert " +
                                                                            blocks[storedInt].ToString());
                                                              }
                                                              storedInt = IntOffset(C.b, 0, 1, 0);
                                                              if (Block.tDoor(blocks[storedInt]))
                                                              {
                                                                  AddUpdate(storedInt, Block.air, false,
                                                                            "wait 10 door 1 revert " +
                                                                            blocks[storedInt].ToString());
                                                              }
                                                              storedInt = IntOffset(C.b, 0, -1, 0);
                                                              if (Block.tDoor(blocks[storedInt]))
                                                              {
                                                                  AddUpdate(storedInt, Block.air, false,
                                                                            "wait 10 door 1 revert " +
                                                                            blocks[storedInt].ToString());
                                                              }
                                                              storedInt = IntOffset(C.b, 0, 0, 1);
                                                              if (Block.tDoor(blocks[storedInt]))
                                                              {
                                                                  AddUpdate(storedInt, Block.air, false,
                                                                            "wait 10 door 1 revert " +
                                                                            blocks[storedInt].ToString());
                                                              }
                                                              storedInt = IntOffset(C.b, 0, 0, -1);
                                                              if (Block.tDoor(blocks[storedInt]))
                                                              {
                                                                  AddUpdate(storedInt, Block.air, false,
                                                                            "wait 10 door 1 revert " +
                                                                            blocks[storedInt].ToString());
                                                              }
                                                          }

                                                          if (waitnum <= C.time)
                                                          {
                                                              wait = false;
                                                              C.extraInfo =
                                                                  C.extraInfo.Substring(0, C.extraInfo.IndexOf("wait ")) +
                                                                  C.extraInfo.Substring(
                                                                      C.extraInfo.IndexOf(' ',
                                                                                          C.extraInfo.IndexOf("wait ") +
                                                                                          5) + 1);
                                                              //C.extraInfo = C.extraInfo.Substring(8);
                                                              goto startCheck;
                                                          }
                                                          C.time++;
                                                          foundInfo = "";
                                                          goto newPhysic;
                                                      }
                                                      if (finiteWater)
                                                          finiteMovement(C, x, y, z);
                                                      else if (rainbow)
                                                          if (C.time < 4)
                                                          {
                                                              C.time++;
                                                          }
                                                          else
                                                          {
                                                              if (rainbownum > 2)
                                                              {
                                                                  if (blocks[C.b] < Block.red ||
                                                                      blocks[C.b] > Block.darkpink)
                                                                  {
                                                                      AddUpdate(C.b, Block.red, true);
                                                                  }
                                                                  else
                                                                  {
                                                                      if (blocks[C.b] == Block.darkpink)
                                                                          AddUpdate(C.b, Block.red);
                                                                      else AddUpdate(C.b, (blocks[C.b] + 1));
                                                                  }
                                                              }
                                                              else
                                                              {
                                                                  AddUpdate(C.b, rand.Next(21, 33));
                                                              }
                                                          }
                                                      else
                                                      {
                                                          if (revert)
                                                          {
                                                              AddUpdate(C.b, reverttype);
                                                              C.extraInfo = "";
                                                          }
                                                          // Not setting drop = false can cause occasional leftover blocks, since C.extraInfo is emptied, so
                                                          // drop can generate another block with no dissipate/explode information.
                                                          if (dissipate)
                                                              if (rand.Next(1, 100) <= dissipatenum)
                                                              {
                                                                  if (!ListUpdate.Exists(Update => Update.b == C.b))
                                                                  {
                                                                      AddUpdate(C.b, Block.air);
                                                                      C.extraInfo = "";
                                                                      drop = false;
                                                                  }
                                                                  else
                                                                  {
                                                                      AddUpdate(C.b, blocks[C.b], false, C.extraInfo);
                                                                  }
                                                              }
                                                          if (explode)
                                                              if (rand.Next(1, 100) <= explodenum)
                                                              {
                                                                  MakeExplosion(x, y, z, 0);
                                                                  C.extraInfo = "";
                                                                  drop = false;
                                                              }
                                                          if (drop)
                                                              if (rand.Next(1, 100) <= dropnum)
                                                                  if (GetTile(x, (ushort)(y - 1), z) == Block.air ||
                                                                      GetTile(x, (ushort)(y - 1), z) == Block.lava ||
                                                                      GetTile(x, (ushort)(y - 1), z) == Block.water)
                                                                  {
                                                                      if (rand.Next(1, 100) <
                                                                          int.Parse(C.extraInfo.Split(' ')[1]))
                                                                      {
                                                                          if (
                                                                              AddUpdate(
                                                                                  PosToInt(x, (ushort)(y - 1), z),
                                                                                  blocks[C.b], false, C.extraInfo))
                                                                          {
                                                                              AddUpdate(C.b, Block.air);
                                                                              C.extraInfo = "";
                                                                          }
                                                                      }
                                                                  }
                                                      }
                                                  }
                                                  else
                                                  {
                                                      int newNum;
                                                      switch (blocks[C.b])
                                                      {
                                                          case Block.air: //Placed air
                                                              //initialy checks if block is valid
                                                              PhysAir(PosToInt((ushort)(x + 1), y, z));
                                                              PhysAir(PosToInt((ushort)(x - 1), y, z));
                                                              PhysAir(PosToInt(x, y, (ushort)(z + 1)));
                                                              PhysAir(PosToInt(x, y, (ushort)(z - 1)));
                                                              PhysAir(PosToInt(x, (ushort)(y + 1), z));
                                                              //Check block above the air
                                                              PhysAir(PosToInt(x, (ushort)(y - 1), z));
                                                              // Check block below the air

                                                              //Edge of map water
                                                              if (edgeWater)
                                                              {
                                                                  if (y < depth / 2 && y >= (depth / 2) - 2)
                                                                  {
                                                                      if (x == 0 || x == width - 1 || z == 0 ||
                                                                          z == height - 1)
                                                                      {
                                                                          AddUpdate(C.b, Block.water);
                                                                      }
                                                                  }
                                                              }

                                                              if (!C.extraInfo.Contains("wait")) C.time = 255;
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

                                                          case Block.water: //Active_water
                                                          case Block.activedeathwater:
                                                              //initialy checks if block is valid
                                                              if (!finite)
                                                              {
                                                                  if (randomFlow)
                                                                  {
                                                                      if (!PhysSpongeCheck(C.b))
                                                                      {
                                                                          if (!liquids.ContainsKey(C.b))
                                                                              liquids.Add(C.b, new bool[5]);

                                                                          if (GetTile(x, (ushort)(y + 1), z) !=
                                                                              Block.Zero)
                                                                          {
                                                                              PhysSandCheck(PosToInt(x, (ushort)(y + 1),
                                                                                                     z));
                                                                          }
                                                                          if (!liquids[C.b][0] && rand.Next(4) == 0)
                                                                          {
                                                                              PhysWater(
                                                                                  PosToInt((ushort)(x + 1), y, z),
                                                                                  blocks[C.b]);
                                                                              liquids[C.b][0] = true;
                                                                          }
                                                                          if (!liquids[C.b][1] && rand.Next(4) == 0)
                                                                          {
                                                                              PhysWater(
                                                                                  PosToInt((ushort)(x - 1), y, z),
                                                                                  blocks[C.b]);
                                                                              liquids[C.b][1] = true;
                                                                          }
                                                                          if (!liquids[C.b][2] && rand.Next(4) == 0)
                                                                          {
                                                                              PhysWater(
                                                                                  PosToInt(x, y, (ushort)(z + 1)),
                                                                                  blocks[C.b]);
                                                                              liquids[C.b][2] = true;
                                                                          }
                                                                          if (!liquids[C.b][3] && rand.Next(4) == 0)
                                                                          {
                                                                              PhysWater(
                                                                                  PosToInt(x, y, (ushort)(z - 1)),
                                                                                  blocks[C.b]);
                                                                              liquids[C.b][3] = true;
                                                                          }
                                                                          if (!liquids[C.b][4] && rand.Next(4) == 0)
                                                                          {
                                                                              PhysWater(
                                                                                  PosToInt(x, (ushort)(y - 1), z),
                                                                                  blocks[C.b]);
                                                                              liquids[C.b][4] = true;
                                                                          }

                                                                          if (!liquids[C.b][0] &&
                                                                              !PhysWaterCheck(PosToInt(
                                                                                  (ushort)(x + 1), y, z)))
                                                                              liquids[C.b][0] = true;
                                                                          if (!liquids[C.b][1] &&
                                                                              !PhysWaterCheck(PosToInt(
                                                                                  (ushort)(x - 1), y, z)))
                                                                              liquids[C.b][1] = true;
                                                                          if (!liquids[C.b][2] &&
                                                                              !PhysWaterCheck(PosToInt(x, y,
                                                                                                       (ushort)(z + 1))))
                                                                              liquids[C.b][2] = true;
                                                                          if (!liquids[C.b][3] &&
                                                                              !PhysWaterCheck(PosToInt(x, y,
                                                                                                       (ushort)(z - 1))))
                                                                              liquids[C.b][3] = true;
                                                                          if (!liquids[C.b][4] &&
                                                                              !PhysWaterCheck(PosToInt(x,
                                                                                                       (ushort)(y - 1),
                                                                                                       z)))
                                                                              liquids[C.b][4] = true;
                                                                      }
                                                                      else
                                                                      {
                                                                          AddUpdate(C.b, Block.air);
                                                                          //was placed near sponge
                                                                          if (C.extraInfo.IndexOf("wait") == -1)
                                                                              C.time = 255;
                                                                      }

                                                                      if (C.extraInfo.IndexOf("wait") == -1 &&
                                                                          liquids.ContainsKey(C.b))
                                                                          if (liquids[C.b][0] && liquids[C.b][1] &&
                                                                              liquids[C.b][2] && liquids[C.b][3] &&
                                                                              liquids[C.b][4])
                                                                          {
                                                                              liquids.Remove(C.b);
                                                                              C.time = 255;
                                                                          }
                                                                  }
                                                                  else
                                                                  {
                                                                      if (liquids.ContainsKey(C.b)) liquids.Remove(C.b);
                                                                      if (!PhysSpongeCheck(C.b))
                                                                      {
                                                                          if (GetTile(x, (ushort)(y + 1), z) !=
                                                                              Block.Zero)
                                                                          {
                                                                              PhysSandCheck(PosToInt(x, (ushort)(y + 1),
                                                                                                     z));
                                                                          }
                                                                          PhysWater(PosToInt((ushort)(x + 1), y, z),
                                                                                    blocks[C.b]);
                                                                          PhysWater(PosToInt((ushort)(x - 1), y, z),
                                                                                    blocks[C.b]);
                                                                          PhysWater(PosToInt(x, y, (ushort)(z + 1)),
                                                                                    blocks[C.b]);
                                                                          PhysWater(PosToInt(x, y, (ushort)(z - 1)),
                                                                                    blocks[C.b]);
                                                                          PhysWater(PosToInt(x, (ushort)(y - 1), z),
                                                                                    blocks[C.b]);
                                                                      }
                                                                      else
                                                                      {
                                                                          AddUpdate(C.b, Block.air);
                                                                          //was placed near sponge
                                                                      }

                                                                      if (C.extraInfo.IndexOf("wait") == -1)
                                                                          C.time = 255;
                                                                  }
                                                              }
                                                              else
                                                              {
                                                                  if (liquids.ContainsKey(C.b)) liquids.Remove(C.b);
                                                                  goto case Block.finiteWater;
                                                              }
                                                              break;

                                                          case Block.WaterDown:
                                                              rand = new Random();

                                                              switch (GetTile(x, (ushort)(y - 1), z))
                                                              {
                                                                  case Block.air:
                                                                      AddUpdate(PosToInt(x, (ushort)(y - 1), z),
                                                                                Block.WaterDown);
                                                                      if (C.extraInfo.IndexOf("wait") == -1) C.time = 255;
                                                                      break;
                                                                  case Block.air_flood_down:
                                                                      break;
                                                                  case Block.lavastill:
                                                                  case Block.waterstill:
                                                                      break;
                                                                  default:
                                                                      if (GetTile(x, (ushort)(y - 1), z) !=
                                                                          Block.WaterDown)
                                                                      {
                                                                          PhysWater(
                                                                              PosToInt((ushort)(x + 1), y, z),
                                                                              blocks[C.b]);
                                                                          PhysWater(
                                                                              PosToInt((ushort)(x - 1), y, z),
                                                                              blocks[C.b]);
                                                                          PhysWater(
                                                                              PosToInt(x, y, (ushort)(z + 1)),
                                                                              blocks[C.b]);
                                                                          PhysWater(
                                                                              PosToInt(x, y, (ushort)(z - 1)),
                                                                              blocks[C.b]);
                                                                          if (C.extraInfo.IndexOf("wait") == -1)
                                                                              C.time = 255;
                                                                      }
                                                                      break;
                                                              }
                                                              break;

                                                          case Block.LavaDown:
                                                              rand = new Random();

                                                              switch (GetTile(x, (ushort)(y - 1), z))
                                                              {
                                                                  case Block.air:
                                                                      AddUpdate(PosToInt(x, (ushort)(y - 1), z),
                                                                                Block.LavaDown);
                                                                      if (C.extraInfo.IndexOf("wait") == -1) C.time = 255;
                                                                      break;
                                                                  case Block.air_flood_down:
                                                                      break;
                                                                  case Block.lavastill:
                                                                  case Block.waterstill:
                                                                      break;
                                                                  default:
                                                                      if (GetTile(x, (ushort)(y - 1), z) !=
                                                                          Block.LavaDown)
                                                                      {
                                                                          PhysLava(
                                                                              PosToInt((ushort)(x + 1), y, z),
                                                                              blocks[C.b]);
                                                                          PhysLava(
                                                                              PosToInt((ushort)(x - 1), y, z),
                                                                              blocks[C.b]);
                                                                          PhysLava(
                                                                              PosToInt(x, y, (ushort)(z + 1)),
                                                                              blocks[C.b]);
                                                                          PhysLava(
                                                                              PosToInt(x, y, (ushort)(z - 1)),
                                                                              blocks[C.b]);
                                                                          if (C.extraInfo.IndexOf("wait") == -1)
                                                                              C.time = 255;
                                                                      }
                                                                      break;
                                                              }
                                                              break;

                                                          case Block.WaterFaucet:
                                                              //rand = new Random();
                                                              C.time++;
                                                              if (C.time < 2) break;

                                                              C.time = 0;

                                                              switch (GetTile(x, (ushort)(y - 1), z))
                                                              {
                                                                  case Block.WaterDown:
                                                                  case Block.air:
                                                                      if (rand.Next(1, 10) > 7)
                                                                          AddUpdate(PosToInt(x, (ushort)(y - 1), z),
                                                                                    Block.air_flood_down);
                                                                      break;
                                                                  case Block.air_flood_down:
                                                                      if (rand.Next(1, 10) > 4)
                                                                          AddUpdate(PosToInt(x, (ushort)(y - 1), z),
                                                                                    Block.WaterDown);
                                                                      break;
                                                              }
                                                              break;

                                                          case Block.LavaFaucet:
                                                              //rand = new Random();
                                                              C.time++;
                                                              if (C.time < 2) break;

                                                              C.time = 0;

                                                              switch (GetTile(x, (ushort)(y - 1), z))
                                                              {
                                                                  case Block.LavaDown:
                                                                  case Block.air:
                                                                      if (rand.Next(1, 10) > 7)
                                                                          AddUpdate(PosToInt(x, (ushort)(y - 1), z),
                                                                                    Block.air_flood_down);
                                                                      break;
                                                                  case Block.air_flood_down:
                                                                      if (rand.Next(1, 10) > 4)
                                                                          AddUpdate(PosToInt(x, (ushort)(y - 1), z),
                                                                                    Block.LavaDown);
                                                                      break;
                                                              }
                                                              break;

                                                          case Block.lava: //Active_lava
                                                          case Block.activedeathlava:
                                                              //initialy checks if block is valid
                                                              if (C.time < 4)
                                                              {
                                                                  C.time++;
                                                                  break;
                                                              }
                                                              if (!finite)
                                                              {
                                                                  if (randomFlow)
                                                                  {
                                                                      if (!PhysSpongeCheck(C.b, true))
                                                                      {
                                                                          C.time = (byte)rand.Next(3);
                                                                          if (!liquids.ContainsKey(C.b))
                                                                              liquids.Add(C.b, new bool[5]);

                                                                          if (!liquids[C.b][0] && rand.Next(4) == 0)
                                                                          {
                                                                              PhysLava(
                                                                                  PosToInt((ushort)(x + 1), y, z),
                                                                                  blocks[C.b]);
                                                                              liquids[C.b][0] = true;
                                                                          }
                                                                          if (!liquids[C.b][1] && rand.Next(4) == 0)
                                                                          {
                                                                              PhysLava(
                                                                                  PosToInt((ushort)(x - 1), y, z),
                                                                                  blocks[C.b]);
                                                                              liquids[C.b][1] = true;
                                                                          }
                                                                          if (!liquids[C.b][2] && rand.Next(4) == 0)
                                                                          {
                                                                              PhysLava(
                                                                                  PosToInt(x, y, (ushort)(z + 1)),
                                                                                  blocks[C.b]);
                                                                              liquids[C.b][2] = true;
                                                                          }
                                                                          if (!liquids[C.b][3] && rand.Next(4) == 0)
                                                                          {
                                                                              PhysLava(
                                                                                  PosToInt(x, y, (ushort)(z - 1)),
                                                                                  blocks[C.b]);
                                                                              liquids[C.b][3] = true;
                                                                          }
                                                                          if (!liquids[C.b][4] && rand.Next(4) == 0)
                                                                          {
                                                                              PhysLava(
                                                                                  PosToInt(x, (ushort)(y - 1), z),
                                                                                  blocks[C.b]);
                                                                              liquids[C.b][4] = true;
                                                                          }

                                                                          if (!liquids[C.b][0] &&
                                                                              !PhysLavaCheck(PosToInt((ushort)(x + 1),
                                                                                                      y, z)))
                                                                              liquids[C.b][0] = true;
                                                                          if (!liquids[C.b][1] &&
                                                                              !PhysLavaCheck(PosToInt((ushort)(x - 1),
                                                                                                      y, z)))
                                                                              liquids[C.b][1] = true;
                                                                          if (!liquids[C.b][2] &&
                                                                              !PhysLavaCheck(PosToInt(x, y,
                                                                                                      (ushort)(z + 1))))
                                                                              liquids[C.b][2] = true;
                                                                          if (!liquids[C.b][3] &&
                                                                              !PhysLavaCheck(PosToInt(x, y,
                                                                                                      (ushort)(z - 1))))
                                                                              liquids[C.b][3] = true;
                                                                          if (!liquids[C.b][4] &&
                                                                              !PhysLavaCheck(PosToInt(x,
                                                                                                      (ushort)(y - 1),
                                                                                                      z)))
                                                                              liquids[C.b][4] = true;
                                                                      }
                                                                      else
                                                                      {
                                                                          AddUpdate(C.b, Block.air);
                                                                          //was placed near sponge
                                                                          if (C.extraInfo.IndexOf("wait") == -1)
                                                                              C.time = 255;
                                                                      }

                                                                      if (C.extraInfo.IndexOf("wait") == -1 &&
                                                                          liquids.ContainsKey(C.b))
                                                                          if (liquids[C.b][0] && liquids[C.b][1] &&
                                                                              liquids[C.b][2] && liquids[C.b][3] &&
                                                                              liquids[C.b][4])
                                                                          {
                                                                              liquids.Remove(C.b);
                                                                              C.time = 255;
                                                                          }
                                                                  }
                                                                  else
                                                                  {
                                                                      if (liquids.ContainsKey(C.b)) liquids.Remove(C.b);
                                                                      if (!PhysSpongeCheck(C.b, true))
                                                                      {
                                                                          PhysLava(PosToInt((ushort)(x + 1), y, z),
                                                                                   blocks[C.b]);
                                                                          PhysLava(PosToInt((ushort)(x - 1), y, z),
                                                                                   blocks[C.b]);
                                                                          PhysLava(PosToInt(x, y, (ushort)(z + 1)),
                                                                                   blocks[C.b]);
                                                                          PhysLava(PosToInt(x, y, (ushort)(z - 1)),
                                                                                   blocks[C.b]);
                                                                          PhysLava(PosToInt(x, (ushort)(y - 1), z),
                                                                                   blocks[C.b]);
                                                                      }
                                                                      else
                                                                      {
                                                                          AddUpdate(C.b, Block.air);
                                                                          //was placed near sponge
                                                                      }

                                                                      if (C.extraInfo.IndexOf("wait") == -1)
                                                                          C.time = 255;
                                                                  }
                                                              }
                                                              else
                                                              {
                                                                  if (liquids.ContainsKey(C.b)) liquids.Remove(C.b);
                                                                  goto case Block.finiteWater;
                                                              }
                                                              break;

                                                          #region fire

                                                          case Block.fire:
                                                              if (C.time < 2)
                                                              {
                                                                  C.time++;
                                                                  break;
                                                              }

                                                              storedRand = rand.Next(1, 20);
                                                              if (storedRand < 2 && C.time % 2 == 0)
                                                              {
                                                                  storedRand = rand.Next(1, 18);

                                                                  if (storedRand <= 3 &&
                                                                      GetTile((ushort)(x - 1), y, z) == Block.air)
                                                                      AddUpdate(PosToInt((ushort)(x - 1), y, z),
                                                                                Block.fire);
                                                                  else if (storedRand <= 6 &&
                                                                           GetTile((ushort)(x + 1), y, z) == Block.air)
                                                                      AddUpdate(PosToInt((ushort)(x + 1), y, z),
                                                                                Block.fire);
                                                                  else if (storedRand <= 9 &&
                                                                           GetTile(x, (ushort)(y - 1), z) ==
                                                                           Block.air)
                                                                      AddUpdate(
                                                                          PosToInt(x, (ushort)(y - 1), z),
                                                                          Block.fire);
                                                                  else if (storedRand <= 12 &&
                                                                           GetTile(x, (ushort)(y + 1), z) ==
                                                                           Block.air)
                                                                      AddUpdate(
                                                                          PosToInt(x, (ushort)(y + 1), z),
                                                                          Block.fire);
                                                                  else if (storedRand <= 15 &&
                                                                           GetTile(x, y, (ushort)(z - 1)) ==
                                                                           Block.air)
                                                                      AddUpdate(
                                                                          PosToInt(x, y,
                                                                                   (ushort)(z - 1)),
                                                                          Block.fire);
                                                                  else if (storedRand <= 18 &&
                                                                           GetTile(x, y, (ushort)(z + 1)) ==
                                                                           Block.air)
                                                                      AddUpdate(
                                                                          PosToInt(x, y,
                                                                                   (ushort)(z + 1)),
                                                                          Block.fire);
                                                              }

                                                              if (
                                                                  Block.LavaKill(GetTile((ushort)(x - 1), y,
                                                                                         (ushort)(z - 1))))
                                                              {
                                                                  if (GetTile((ushort)(x - 1), y, z) == Block.air)
                                                                      AddUpdate(PosToInt((ushort)(x - 1), y, z),
                                                                                Block.fire);
                                                                  if (GetTile(x, y, (ushort)(z - 1)) == Block.air)
                                                                      AddUpdate(PosToInt(x, y, (ushort)(z - 1)),
                                                                                Block.fire);
                                                              }
                                                              if (
                                                                  Block.LavaKill(GetTile((ushort)(x + 1), y,
                                                                                         (ushort)(z - 1))))
                                                              {
                                                                  if (GetTile((ushort)(x + 1), y, z) == Block.air)
                                                                      AddUpdate(PosToInt((ushort)(x + 1), y, z),
                                                                                Block.fire);
                                                                  if (GetTile(x, y, (ushort)(z - 1)) == Block.air)
                                                                      AddUpdate(PosToInt(x, y, (ushort)(z - 1)),
                                                                                Block.fire);
                                                              }
                                                              if (
                                                                  Block.LavaKill(GetTile((ushort)(x - 1), y,
                                                                                         (ushort)(z + 1))))
                                                              {
                                                                  if (GetTile((ushort)(x - 1), y, z) == Block.air)
                                                                      AddUpdate(PosToInt((ushort)(x - 1), y, z),
                                                                                Block.fire);
                                                                  if (GetTile(x, y, (ushort)(z + 1)) == Block.air)
                                                                      AddUpdate(PosToInt(x, y, (ushort)(z + 1)),
                                                                                Block.fire);
                                                              }
                                                              if (
                                                                  Block.LavaKill(GetTile((ushort)(x + 1), y,
                                                                                         (ushort)(z + 1))))
                                                              {
                                                                  if (GetTile((ushort)(x + 1), y, z) == Block.air)
                                                                      AddUpdate(PosToInt((ushort)(x + 1), y, z),
                                                                                Block.fire);
                                                                  if (GetTile(x, y, (ushort)(z + 1)) == Block.air)
                                                                      AddUpdate(PosToInt(x, y, (ushort)(z + 1)),
                                                                                Block.fire);
                                                              }
                                                              if (
                                                                  Block.LavaKill(GetTile(x, (ushort)(y - 1),
                                                                                         (ushort)(z - 1))))
                                                              {
                                                                  if (GetTile(x, (ushort)(y - 1), z) == Block.air)
                                                                      AddUpdate(PosToInt(x, (ushort)(y - 1), z),
                                                                                Block.fire);
                                                                  if (GetTile(x, y, (ushort)(z - 1)) == Block.air)
                                                                      AddUpdate(PosToInt(x, y, (ushort)(z - 1)),
                                                                                Block.fire);
                                                              }
                                                              else if (GetTile(x, (ushort)(y - 1), z) == Block.grass)
                                                                  AddUpdate(PosToInt(x, (ushort)(y - 1), z), Block.dirt);

                                                              if (
                                                                  Block.LavaKill(GetTile(x, (ushort)(y + 1),
                                                                                         (ushort)(z - 1))))
                                                              {
                                                                  if (GetTile(x, (ushort)(y + 1), z) == Block.air)
                                                                      AddUpdate(PosToInt(x, (ushort)(y + 1), z),
                                                                                Block.fire);
                                                                  if (GetTile(x, y, (ushort)(z - 1)) == Block.air)
                                                                      AddUpdate(PosToInt(x, y, (ushort)(z - 1)),
                                                                                Block.fire);
                                                              }
                                                              if (
                                                                  Block.LavaKill(GetTile(x, (ushort)(y - 1),
                                                                                         (ushort)(z + 1))))
                                                              {
                                                                  if (GetTile(x, (ushort)(y - 1), z) == Block.air)
                                                                      AddUpdate(PosToInt(x, (ushort)(y - 1), z),
                                                                                Block.fire);
                                                                  if (GetTile(x, y, (ushort)(z + 1)) == Block.air)
                                                                      AddUpdate(PosToInt(x, y, (ushort)(z + 1)),
                                                                                Block.fire);
                                                              }
                                                              if (
                                                                  Block.LavaKill(GetTile(x, (ushort)(y + 1),
                                                                                         (ushort)(z + 1))))
                                                              {
                                                                  if (GetTile(x, (ushort)(y + 1), z) == Block.air)
                                                                      AddUpdate(PosToInt(x, (ushort)(y + 1), z),
                                                                                Block.fire);
                                                                  if (GetTile(x, y, (ushort)(z + 1)) == Block.air)
                                                                      AddUpdate(PosToInt(x, y, (ushort)(z + 1)),
                                                                                Block.fire);
                                                              }
                                                              if (
                                                                  Block.LavaKill(GetTile((ushort)(x - 1),
                                                                                         (ushort)(y - 1), z)))
                                                              {
                                                                  if (GetTile(x, (ushort)(y - 1), z) == Block.air)
                                                                      AddUpdate(PosToInt(x, (ushort)(y - 1), z),
                                                                                Block.fire);
                                                                  if (GetTile((ushort)(x - 1), y, z) == Block.air)
                                                                      AddUpdate(PosToInt((ushort)(x - 1), y, z),
                                                                                Block.fire);
                                                              }
                                                              if (
                                                                  Block.LavaKill(GetTile((ushort)(x - 1),
                                                                                         (ushort)(y + 1), z)))
                                                              {
                                                                  if (GetTile(x, (ushort)(y + 1), z) == Block.air)
                                                                      AddUpdate(PosToInt(x, (ushort)(y + 1), z),
                                                                                Block.fire);
                                                                  if (GetTile((ushort)(x - 1), y, z) == Block.air)
                                                                      AddUpdate(PosToInt((ushort)(x - 1), y, z),
                                                                                Block.fire);
                                                              }
                                                              if (
                                                                  Block.LavaKill(GetTile((ushort)(x + 1),
                                                                                         (ushort)(y - 1), z)))
                                                              {
                                                                  if (GetTile(x, (ushort)(y - 1), z) == Block.air)
                                                                      AddUpdate(PosToInt(x, (ushort)(y - 1), z),
                                                                                Block.fire);
                                                                  if (GetTile((ushort)(x + 1), y, z) == Block.air)
                                                                      AddUpdate(PosToInt((ushort)(x + 1), y, z),
                                                                                Block.fire);
                                                              }
                                                              if (
                                                                  Block.LavaKill(GetTile((ushort)(x + 1),
                                                                                         (ushort)(y + 1), z)))
                                                              {
                                                                  if (GetTile(x, (ushort)(y + 1), z) == Block.air)
                                                                      AddUpdate(PosToInt(x, (ushort)(y + 1), z),
                                                                                Block.fire);
                                                                  if (GetTile((ushort)(x + 1), y, z) == Block.air)
                                                                      AddUpdate(PosToInt((ushort)(x + 1), y, z),
                                                                                Block.fire);
                                                              }

                                                              if (physics >= 2)
                                                              {
                                                                  if (C.time < 4)
                                                                  {
                                                                      C.time++;
                                                                      break;
                                                                  }

                                                                  if (Block.LavaKill(GetTile((ushort)(x - 1), y, z)))
                                                                      AddUpdate(PosToInt((ushort)(x - 1), y, z),
                                                                                Block.fire);
                                                                  else if (GetTile((ushort)(x - 1), y, z) == Block.tnt)
                                                                      MakeExplosion((ushort)(x - 1), y, z, -1);

                                                                  if (Block.LavaKill(GetTile((ushort)(x + 1), y, z)))
                                                                      AddUpdate(PosToInt((ushort)(x + 1), y, z),
                                                                                Block.fire);
                                                                  else if (GetTile((ushort)(x + 1), y, z) == Block.tnt)
                                                                      MakeExplosion((ushort)(x + 1), y, z, -1);

                                                                  if (Block.LavaKill(GetTile(x, (ushort)(y - 1), z)))
                                                                      AddUpdate(PosToInt(x, (ushort)(y - 1), z),
                                                                                Block.fire);
                                                                  else if (GetTile(x, (ushort)(y - 1), z) == Block.tnt)
                                                                      MakeExplosion(x, (ushort)(y - 1), z, -1);

                                                                  if (Block.LavaKill(GetTile(x, (ushort)(y + 1), z)))
                                                                      AddUpdate(PosToInt(x, (ushort)(y + 1), z),
                                                                                Block.fire);
                                                                  else if (GetTile(x, (ushort)(y + 1), z) == Block.tnt)
                                                                      MakeExplosion(x, (ushort)(y + 1), z, -1);

                                                                  if (Block.LavaKill(GetTile(x, y, (ushort)(z - 1))))
                                                                      AddUpdate(PosToInt(x, y, (ushort)(z - 1)),
                                                                                Block.fire);
                                                                  else if (GetTile(x, y, (ushort)(z - 1)) == Block.tnt)
                                                                      MakeExplosion(x, y, (ushort)(z - 1), -1);

                                                                  if (Block.LavaKill(GetTile(x, y, (ushort)(z + 1))))
                                                                      AddUpdate(PosToInt(x, y, (ushort)(z + 1)),
                                                                                Block.fire);
                                                                  else if (GetTile(x, y, (ushort)(z + 1)) == Block.tnt)
                                                                      MakeExplosion(x, y, (ushort)(z + 1), -1);
                                                              }

                                                              C.time++;
                                                              if (C.time > 5)
                                                              {
                                                                  storedRand = (rand.Next(1, 10));
                                                                  if (storedRand <= 2)
                                                                  {
                                                                      AddUpdate(C.b, Block.coal);
                                                                      C.extraInfo = "drop 63 dissipate 10";
                                                                  }
                                                                  else if (storedRand <= 4)
                                                                  {
                                                                      AddUpdate(C.b, Block.obsidian);
                                                                      C.extraInfo = "drop 63 dissipate 10";
                                                                  }
                                                                  else if (storedRand <= 8) AddUpdate(C.b, Block.air);
                                                                  else C.time = 3;
                                                              }

                                                              break;

                                                          #endregion

                                                          case Block.finiteWater:
                                                          case Block.finiteLava:
                                                              finiteMovement(C, x, y, z);
                                                              break;

                                                          case Block.finiteFaucet:
                                                              var bufferfinitefaucet = new List<int>();

                                                              for (int i = 0; i < 6; ++i) bufferfinitefaucet.Add(i);

                                                              for (int k = bufferfinitefaucet.Count - 1; k > 1; --k)
                                                              {
                                                                  int randIndx = rand.Next(k);
                                                                  int temp = bufferfinitefaucet[k];
                                                                  bufferfinitefaucet[k] = bufferfinitefaucet[randIndx];
                                                                  // move random num to end of list.
                                                                  bufferfinitefaucet[randIndx] = temp;
                                                              }

                                                              foreach (int i in bufferfinitefaucet)
                                                              {
                                                                  switch (i)
                                                                  {
                                                                      case 0:
                                                                          if (GetTile((ushort)(x - 1), y, z) ==
                                                                              Block.air)
                                                                          {
                                                                              if (
                                                                                  AddUpdate(
                                                                                      PosToInt((ushort)(x - 1), y, z),
                                                                                      Block.finiteWater))
                                                                                  InnerChange = true;
                                                                          }
                                                                          break;
                                                                      case 1:
                                                                          if (GetTile((ushort)(x + 1), y, z) ==
                                                                              Block.air)
                                                                          {
                                                                              if (
                                                                                  AddUpdate(
                                                                                      PosToInt((ushort)(x + 1), y, z),
                                                                                      Block.finiteWater))
                                                                                  InnerChange = true;
                                                                          }
                                                                          break;
                                                                      case 2:
                                                                          if (GetTile(x, (ushort)(y - 1), z) ==
                                                                              Block.air)
                                                                          {
                                                                              if (
                                                                                  AddUpdate(
                                                                                      PosToInt(x, (ushort)(y - 1), z),
                                                                                      Block.finiteWater))
                                                                                  InnerChange = true;
                                                                          }
                                                                          break;
                                                                      case 3:
                                                                          if (GetTile(x, (ushort)(y + 1), z) ==
                                                                              Block.air)
                                                                          {
                                                                              if (
                                                                                  AddUpdate(
                                                                                      PosToInt(x, (ushort)(y + 1), z),
                                                                                      Block.finiteWater))
                                                                                  InnerChange = true;
                                                                          }
                                                                          break;
                                                                      case 4:
                                                                          if (GetTile(x, y, (ushort)(z - 1)) ==
                                                                              Block.air)
                                                                          {
                                                                              if (
                                                                                  AddUpdate(
                                                                                      PosToInt(x, y, (ushort)(z - 1)),
                                                                                      Block.finiteWater))
                                                                                  InnerChange = true;
                                                                          }
                                                                          break;
                                                                      case 5:
                                                                          if (GetTile(x, y, (ushort)(z + 1)) ==
                                                                              Block.air)
                                                                          {
                                                                              if (
                                                                                  AddUpdate(
                                                                                      PosToInt(x, y, (ushort)(z + 1)),
                                                                                      Block.finiteWater))
                                                                                  InnerChange = true;
                                                                          }
                                                                          break;
                                                                  }

                                                                  if (InnerChange) break;
                                                              }

                                                              break;

                                                          case Block.sand: //Sand
                                                              if (PhysSand(C.b, Block.sand))
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

                                                          case Block.gravel: //Gravel
                                                              if (PhysSand(C.b, Block.gravel))
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
                                                              PhysStair(C.b);
                                                              C.time = 255;
                                                              break;

                                                          case Block.wood_float: //wood_float
                                                              PhysFloatwood(C.b);
                                                              C.time = 255;
                                                              break;

                                                          case Block.lava_fast: //lava_fast
                                                          case Block.fastdeathlava:
                                                              //initialy checks if block is valid
                                                              if (randomFlow)
                                                              {
                                                                  if (!PhysSpongeCheck(C.b, true))
                                                                  {
                                                                      if (!liquids.ContainsKey(C.b))
                                                                          liquids.Add(C.b, new bool[5]);

                                                                      if (!liquids[C.b][0] && rand.Next(4) == 0)
                                                                      {
                                                                          PhysLava(PosToInt((ushort)(x + 1), y, z),
                                                                                   blocks[C.b]);
                                                                          liquids[C.b][0] = true;
                                                                      }
                                                                      if (!liquids[C.b][1] && rand.Next(4) == 0)
                                                                      {
                                                                          PhysLava(PosToInt((ushort)(x - 1), y, z),
                                                                                   blocks[C.b]);
                                                                          liquids[C.b][1] = true;
                                                                      }
                                                                      if (!liquids[C.b][2] && rand.Next(4) == 0)
                                                                      {
                                                                          PhysLava(PosToInt(x, y, (ushort)(z + 1)),
                                                                                   blocks[C.b]);
                                                                          liquids[C.b][2] = true;
                                                                      }
                                                                      if (!liquids[C.b][3] && rand.Next(4) == 0)
                                                                      {
                                                                          PhysLava(PosToInt(x, y, (ushort)(z - 1)),
                                                                                   blocks[C.b]);
                                                                          liquids[C.b][3] = true;
                                                                      }
                                                                      if (!liquids[C.b][4] && rand.Next(4) == 0)
                                                                      {
                                                                          PhysLava(PosToInt(x, (ushort)(y - 1), z),
                                                                                   blocks[C.b]);
                                                                          liquids[C.b][4] = true;
                                                                      }

                                                                      if (!liquids[C.b][0] &&
                                                                          !PhysLavaCheck(PosToInt((ushort)(x + 1), y, z)))
                                                                          liquids[C.b][0] = true;
                                                                      if (!liquids[C.b][1] &&
                                                                          !PhysLavaCheck(PosToInt((ushort)(x - 1), y, z)))
                                                                          liquids[C.b][1] = true;
                                                                      if (!liquids[C.b][2] &&
                                                                          !PhysLavaCheck(PosToInt(x, y, (ushort)(z + 1))))
                                                                          liquids[C.b][2] = true;
                                                                      if (!liquids[C.b][3] &&
                                                                          !PhysLavaCheck(PosToInt(x, y, (ushort)(z - 1))))
                                                                          liquids[C.b][3] = true;
                                                                      if (!liquids[C.b][4] &&
                                                                          !PhysLavaCheck(PosToInt(x, (ushort)(y - 1), z)))
                                                                          liquids[C.b][4] = true;
                                                                  }
                                                                  else
                                                                  {
                                                                      AddUpdate(C.b, Block.air);
                                                                      //was placed near sponge
                                                                      C.time = 255;
                                                                  }

                                                                  if (liquids.ContainsKey(C.b))
                                                                      if (liquids[C.b][0] && liquids[C.b][1] &&
                                                                          liquids[C.b][2] && liquids[C.b][3] &&
                                                                          liquids[C.b][4])
                                                                      {
                                                                          liquids.Remove(C.b);
                                                                          C.time = 255;
                                                                      }
                                                              }
                                                              else
                                                              {
                                                                  if (liquids.ContainsKey(C.b)) liquids.Remove(C.b);
                                                                  if (!PhysSpongeCheck(C.b, true))
                                                                  {
                                                                      PhysLava(PosToInt((ushort)(x + 1), y, z),
                                                                               blocks[C.b]);
                                                                      PhysLava(PosToInt((ushort)(x - 1), y, z),
                                                                               blocks[C.b]);
                                                                      PhysLava(PosToInt(x, y, (ushort)(z + 1)),
                                                                               blocks[C.b]);
                                                                      PhysLava(PosToInt(x, y, (ushort)(z - 1)),
                                                                               blocks[C.b]);
                                                                      PhysLava(PosToInt(x, (ushort)(y - 1), z),
                                                                               blocks[C.b]);
                                                                  }
                                                                  else
                                                                      AddUpdate(C.b, Block.air);
                                                                  //was placed near sponge

                                                                  C.time = 255;
                                                              }
                                                              break;

                                                          //Special blocks that are not saved
                                                          case Block.air_flood: //air_flood
                                                              if (C.time < 1)
                                                              {
                                                                  PhysAirFlood(PosToInt((ushort)(x + 1), y, z),
                                                                               Block.air_flood);
                                                                  PhysAirFlood(PosToInt((ushort)(x - 1), y, z),
                                                                               Block.air_flood);
                                                                  PhysAirFlood(PosToInt(x, y, (ushort)(z + 1)),
                                                                               Block.air_flood);
                                                                  PhysAirFlood(PosToInt(x, y, (ushort)(z - 1)),
                                                                               Block.air_flood);
                                                                  PhysAirFlood(PosToInt(x, (ushort)(y - 1), z),
                                                                               Block.air_flood);
                                                                  PhysAirFlood(PosToInt(x, (ushort)(y + 1), z),
                                                                               Block.air_flood);

                                                                  C.time++;
                                                              }
                                                              else
                                                              {
                                                                  AddUpdate(C.b, 0); //Turn back into normal air
                                                                  C.time = 255;
                                                              }
                                                              break;

                                                          case Block.door_air:
                                                          //door_air         Change any door blocks nearby into door_air
                                                          case Block.door2_air:
                                                          //door_air         Change any door blocks nearby into door_air
                                                          case Block.door3_air:
                                                          //door_air         Change any door blocks nearby into door_air
                                                          case Block.door4_air:
                                                          //door_air         Change any door blocks nearby into door_air
                                                          case Block.door5_air:
                                                          //door_air         Change any door blocks nearby into door_air
                                                          case Block.door6_air:
                                                          //door_air         Change any door blocks nearby into door_air
                                                          case Block.door7_air:
                                                          //door_air         Change any door blocks nearby into door_air
                                                          case Block.door8_air:
                                                          //door_air         Change any door blocks nearby into door_air
                                                          case Block.door10_air:
                                                          //door_air         Change any door blocks nearby into door_air
                                                          case Block.door12_air:
                                                          case Block.door13_air:
                                                          case Block.door_iron_air:
                                                          case Block.door_gold_air:
                                                          case Block.door_cobblestone_air:
                                                          case Block.door_red_air:

                                                          case Block.door_dirt_air:
                                                          case Block.door_grass_air:
                                                          case Block.door_blue_air:
                                                          case Block.door_book_air:
                                                              AnyDoor(C, x, y, z, 16);
                                                              break;
                                                          case Block.door11_air:
                                                          case Block.door14_air:
                                                              AnyDoor(C, x, y, z, 4, true);
                                                              break;
                                                          case Block.door9_air:
                                                              //door_air         Change any door blocks nearby into door_air
                                                              AnyDoor(C, x, y, z, 4);
                                                              break;

                                                          case Block.odoor1_air:
                                                          case Block.odoor2_air:
                                                          case Block.odoor3_air:
                                                          case Block.odoor4_air:
                                                          case Block.odoor5_air:
                                                          case Block.odoor6_air:
                                                          case Block.odoor7_air:
                                                          case Block.odoor8_air:
                                                          case Block.odoor9_air:
                                                          case Block.odoor10_air:
                                                          case Block.odoor11_air:
                                                          case Block.odoor12_air:

                                                          case Block.odoor1:
                                                          case Block.odoor2:
                                                          case Block.odoor3:
                                                          case Block.odoor4:
                                                          case Block.odoor5:
                                                          case Block.odoor6:
                                                          case Block.odoor7:
                                                          case Block.odoor8:
                                                          case Block.odoor9:
                                                          case Block.odoor10:
                                                          case Block.odoor11:
                                                          case Block.odoor12:
                                                              odoor(C);
                                                              break;

                                                          case Block.air_flood_layer: //air_flood_layer
                                                              if (C.time < 1)
                                                              {
                                                                  PhysAirFlood(PosToInt((ushort)(x + 1), y, z),
                                                                               Block.air_flood_layer);
                                                                  PhysAirFlood(PosToInt((ushort)(x - 1), y, z),
                                                                               Block.air_flood_layer);
                                                                  PhysAirFlood(PosToInt(x, y, (ushort)(z + 1)),
                                                                               Block.air_flood_layer);
                                                                  PhysAirFlood(PosToInt(x, y, (ushort)(z - 1)),
                                                                               Block.air_flood_layer);

                                                                  C.time++;
                                                              }
                                                              else
                                                              {
                                                                  AddUpdate(C.b, 0); //Turn back into normal air
                                                                  C.time = 255;
                                                              }
                                                              break;

                                                          case Block.air_flood_down: //air_flood_down
                                                              if (C.time < 1)
                                                              {
                                                                  PhysAirFlood(PosToInt((ushort)(x + 1), y, z),
                                                                               Block.air_flood_down);
                                                                  PhysAirFlood(PosToInt((ushort)(x - 1), y, z),
                                                                               Block.air_flood_down);
                                                                  PhysAirFlood(PosToInt(x, y, (ushort)(z + 1)),
                                                                               Block.air_flood_down);
                                                                  PhysAirFlood(PosToInt(x, y, (ushort)(z - 1)),
                                                                               Block.air_flood_down);
                                                                  PhysAirFlood(PosToInt(x, (ushort)(y - 1), z),
                                                                               Block.air_flood_down);

                                                                  C.time++;
                                                              }
                                                              else
                                                              {
                                                                  AddUpdate(C.b, 0); //Turn back into normal air
                                                                  C.time = 255;
                                                              }
                                                              break;

                                                          case Block.air_flood_up: //air_flood_up
                                                              if (C.time < 1)
                                                              {
                                                                  PhysAirFlood(PosToInt((ushort)(x + 1), y, z),
                                                                               Block.air_flood_up);
                                                                  PhysAirFlood(PosToInt((ushort)(x - 1), y, z),
                                                                               Block.air_flood_up);
                                                                  PhysAirFlood(PosToInt(x, y, (ushort)(z + 1)),
                                                                               Block.air_flood_up);
                                                                  PhysAirFlood(PosToInt(x, y, (ushort)(z - 1)),
                                                                               Block.air_flood_up);
                                                                  PhysAirFlood(PosToInt(x, (ushort)(y + 1), z),
                                                                               Block.air_flood_up);

                                                                  C.time++;
                                                              }
                                                              else
                                                              {
                                                                  AddUpdate(C.b, 0); //Turn back into normal air
                                                                  C.time = 255;
                                                              }
                                                              break;

                                                          case Block.smalltnt:
                                                              //For TNT Wars
                                                              if (C.p != null && C.p.PlayingTntWars)
                                                              {
                                                                  int ExplodeDistance = -1;
                                                                  switch (TntWarsGame.GetTntWarsGame(C.p).GameDifficulty)
                                                                  {
                                                                      case TntWarsGame.TntWarsDifficulty.Easy:
                                                                          if (C.time < 7)
                                                                          {
                                                                              C.time += 1;
                                                                              Blockchange(x, (ushort)(y + 1), z,
                                                                                          GetTile(x, (ushort)(y + 1), z) ==
                                                                                          Block.lavastill
                                                                                              ? Block.air
                                                                                              : Block.lavastill);
                                                                          }
                                                                          else ExplodeDistance = 2;
                                                                          break;

                                                                      case TntWarsGame.TntWarsDifficulty.Normal:
                                                                          if (C.time < 5)
                                                                          {
                                                                              C.time += 1;
                                                                              Blockchange(x, (ushort)(y + 1), z,
                                                                                          GetTile(x, (ushort)(y + 1), z) ==
                                                                                          Block.lavastill
                                                                                              ? Block.air
                                                                                              : Block.lavastill);
                                                                          }
                                                                          else ExplodeDistance = 2;
                                                                          break;

                                                                      case TntWarsGame.TntWarsDifficulty.Hard:
                                                                          if (C.time < 3)
                                                                          {
                                                                              C.time += 1;
                                                                              Blockchange(x, (ushort)(y + 1), z,
                                                                                          GetTile(x, (ushort)(y + 1), z) ==
                                                                                          Block.lavastill
                                                                                              ? Block.air
                                                                                              : Block.lavastill);
                                                                          }
                                                                          else
                                                                          {
                                                                              ExplodeDistance = 2;
                                                                          }
                                                                          break;

                                                                      case TntWarsGame.TntWarsDifficulty.Extreme:
                                                                          if (C.time < 3)
                                                                          {
                                                                              C.time += 1;
                                                                              Blockchange(x, (ushort)(y + 1), z,
                                                                                          GetTile(x, (ushort)(y + 1), z) ==
                                                                                          Block.lavastill
                                                                                              ? Block.air
                                                                                              : Block.lavastill);
                                                                          }
                                                                          else
                                                                          {
                                                                              ExplodeDistance = 3;
                                                                          }
                                                                          break;
                                                                  }
                                                                  if (ExplodeDistance != -1)
                                                                  {
                                                                      if (C.p.TntWarsKillStreak >= TntWarsGame.Properties.DefaultStreakTwoAmount && TntWarsGame.GetTntWarsGame(C.p).Streaks)
                                                                      {
                                                                          ExplodeDistance += 1;
                                                                      }
                                                                      MakeExplosion(x, y, z, ExplodeDistance - 2, true, TntWarsGame.GetTntWarsGame(C.p));
                                                                      List<Player> Killed = new List<Player>();
                                                                      players.ForEach(delegate(Player p1)
                                                                      {
                                                                          if (p1.PlayingTntWars && p1 != C.p && Math.Abs((int)(p1.pos[0] / 32) - x) + Math.Abs((int)(p1.pos[1] / 32) - y) + Math.Abs((int)(p1.pos[2] / 32) - z) < ((ExplodeDistance * 3) + 1))
                                                                          {
                                                                              Killed.Add(p1);
                                                                          }
                                                                      });
                                                                      TntWarsGame.GetTntWarsGame(C.p).HandleKill(C.p, Killed);
                                                                  }
                                                              }
                                                              //Normal
                                                              else
                                                              {
                                                                  if (physics < 3) Blockchange(x, y, z, Block.air);

                                                                  if (physics >= 3)
                                                                  {
                                                                      rand = new Random();

                                                                      if (C.time < 5 && physics == 3)
                                                                      {
                                                                          C.time += 1;
                                                                          Blockchange(x, (ushort)(y + 1), z,
                                                                                      GetTile(x, (ushort)(y + 1), z) ==
                                                                                      Block.lavastill
                                                                                          ? Block.air
                                                                                          : Block.lavastill);
                                                                          break;
                                                                      }

                                                                      MakeExplosion(x, y, z, 0);
                                                                  }
                                                                  else
                                                                  {
                                                                      Blockchange(x, y, z, Block.air);
                                                                  }
                                                              }
                                                              break;

                                                          case Block.bigtnt:
                                                              if (physics < 3) Blockchange(x, y, z, Block.air);

                                                              if (physics >= 3)
                                                              {
                                                                  rand = new Random();

                                                                  if (C.time < 5 && physics == 3)
                                                                  {
                                                                      C.time += 1;
                                                                      Blockchange(x, (ushort)(y + 1), z,
                                                                                  GetTile(x, (ushort)(y + 1), z) ==
                                                                                  Block.lavastill
                                                                                      ? Block.air
                                                                                      : Block.lavastill);
                                                                      Blockchange(x, (ushort)(y - 1), z,
                                                                                  GetTile(x, (ushort)(y - 1), z) ==
                                                                                  Block.lavastill
                                                                                      ? Block.air
                                                                                      : Block.lavastill);
                                                                      Blockchange((ushort)(x + 1), y, z,
                                                                                  GetTile((ushort)(x + 1), y, z) ==
                                                                                  Block.lavastill
                                                                                      ? Block.air
                                                                                      : Block.lavastill);
                                                                      Blockchange((ushort)(x - 1), y, z,
                                                                                  GetTile((ushort)(x - 1), y, z) ==
                                                                                  Block.lavastill
                                                                                      ? Block.air
                                                                                      : Block.lavastill);
                                                                      Blockchange(x, y, (ushort)(z + 1),
                                                                                  GetTile(x, y, (ushort)(z + 1)) ==
                                                                                  Block.lavastill
                                                                                      ? Block.air
                                                                                      : Block.lavastill);
                                                                      Blockchange(x, y, (ushort)(z - 1),
                                                                                  GetTile(x, y, (ushort)(z - 1)) ==
                                                                                  Block.lavastill
                                                                                      ? Block.air
                                                                                      : Block.lavastill);

                                                                      break;
                                                                  }

                                                                  MakeExplosion(x, y, z, 1);
                                                              }
                                                              else
                                                              {
                                                                  Blockchange(x, y, z, Block.air);
                                                              }
                                                              break;

                                                          case Block.nuketnt:
                                                              if (physics < 3) Blockchange(x, y, z, Block.air);

                                                              if (physics >= 3)
                                                              {
                                                                  rand = new Random();

                                                                  if (C.time < 5 && physics == 3)
                                                                  {
                                                                      C.time += 1;
                                                                      Blockchange(x, (ushort)(y + 2), z,
                                                                                  GetTile(x, (ushort)(y + 2), z) ==
                                                                                  Block.lavastill
                                                                                      ? Block.air
                                                                                      : Block.lavastill);
                                                                      Blockchange(x, (ushort)(y - 2), z,
                                                                                  GetTile(x, (ushort)(y - 2), z) ==
                                                                                  Block.lavastill
                                                                                      ? Block.air
                                                                                      : Block.lavastill);
                                                                      Blockchange((ushort)(x + 1), y, z,
                                                                                  GetTile((ushort)(x + 1), y, z) ==
                                                                                  Block.lavastill
                                                                                      ? Block.air
                                                                                      : Block.lavastill);
                                                                      Blockchange((ushort)(x - 1), y, z,
                                                                                  GetTile((ushort)(x - 1), y, z) ==
                                                                                  Block.lavastill
                                                                                      ? Block.air
                                                                                      : Block.lavastill);
                                                                      Blockchange(x, y, (ushort)(z + 1),
                                                                                  GetTile(x, y, (ushort)(z + 1)) ==
                                                                                  Block.lavastill
                                                                                      ? Block.air
                                                                                      : Block.lavastill);
                                                                      Blockchange(x, y, (ushort)(z - 1),
                                                                                  GetTile(x, y, (ushort)(z - 1)) ==
                                                                                  Block.lavastill
                                                                                      ? Block.air
                                                                                      : Block.lavastill);

                                                                      break;
                                                                  }

                                                                  MakeExplosion(x, y, z, 4);
                                                              }
                                                              else
                                                              {
                                                                  Blockchange(x, y, z, Block.air);
                                                              }
                                                              break;

                                                          case Block.tntexplosion:
                                                              if (rand.Next(1, 11) <= 7) AddUpdate(C.b, Block.air);
                                                              break;

                                                          case Block.train:
                                                              if (rand.Next(1, 10) <= 5) mx = 1;
                                                              else mx = -1;
                                                              if (rand.Next(1, 10) <= 5) my = 1;
                                                              else my = -1;
                                                              if (rand.Next(1, 10) <= 5) mz = 1;
                                                              else mz = -1;

                                                              for (int cx = (-1 * mx);
                                                                   cx != ((1 * mx) + mx);
                                                                   cx = cx + (1 * mx))
                                                                  for (int cy = (-1 * my);
                                                                       cy != ((1 * my) + my);
                                                                       cy = cy + (1 * my))
                                                                      for (int cz = (-1 * mz);
                                                                           cz != ((1 * mz) + mz);
                                                                           cz = cz + (1 * mz))
                                                                      {
                                                                          if (
                                                                              GetTile((ushort)(x + cx),
                                                                                      (ushort)(y + cy - 1),
                                                                                      (ushort)(z + cz)) == Block.red &&
                                                                              (GetTile((ushort)(x + cx),
                                                                                       (ushort)(y + cy),
                                                                                       (ushort)(z + cz)) == Block.air ||
                                                                               GetTile((ushort)(x + cx),
                                                                                       (ushort)(y + cy),
                                                                                       (ushort)(z + cz)) == Block.water) &&
                                                                              !InnerChange)
                                                                          {
                                                                              AddUpdate(
                                                                                  PosToInt((ushort)(x + cx),
                                                                                           (ushort)(y + cy),
                                                                                           (ushort)(z + cz)),
                                                                                  Block.train);
                                                                              AddUpdate(PosToInt(x, y, z), Block.air);
                                                                              AddUpdate(IntOffset(C.b, 0, -1, 0),
                                                                                        Block.obsidian, true,
                                                                                        "wait 5 revert " +
                                                                                        Block.red.ToString());

                                                                              InnerChange = true;
                                                                              break;
                                                                          }
                                                                          if (
                                                                              GetTile((ushort)(x + cx),
                                                                                      (ushort)(y + cy - 1),
                                                                                      (ushort)(z + cz)) == Block.op_air &&
                                                                              (GetTile((ushort)(x + cx),
                                                                                       (ushort)(y + cy),
                                                                                       (ushort)(z + cz)) == Block.air ||
                                                                               GetTile((ushort)(x + cx),
                                                                                       (ushort)(y + cy),
                                                                                       (ushort)(z + cz)) == Block.water) &&
                                                                              !InnerChange)
                                                                          {
                                                                              AddUpdate(
                                                                                  PosToInt((ushort)(x + cx),
                                                                                           (ushort)(y + cy),
                                                                                           (ushort)(z + cz)),
                                                                                  Block.train);
                                                                              AddUpdate(PosToInt(x, y, z), Block.air);
                                                                              AddUpdate(IntOffset(C.b, 0, -1, 0),
                                                                                        Block.glass, true,
                                                                                        "wait 5 revert " +
                                                                                        Block.op_air.ToString());

                                                                              InnerChange = true;
                                                                              break;
                                                                          }
                                                                      }
                                                              break;

                                                          case Block.magma:
                                                              C.time++;
                                                              if (C.time < 3) break;

                                                              if (GetTile(x, (ushort)(y - 1), z) == Block.air)
                                                                  AddUpdate(PosToInt(x, (ushort)(y - 1), z),
                                                                            Block.magma);
                                                              else if (GetTile(x, (ushort)(y - 1), z) != Block.magma)
                                                              {
                                                                  PhysLava(PosToInt((ushort)(x + 1), y, z), blocks[C.b]);
                                                                  PhysLava(PosToInt((ushort)(x - 1), y, z), blocks[C.b]);
                                                                  PhysLava(PosToInt(x, y, (ushort)(z + 1)), blocks[C.b]);
                                                                  PhysLava(PosToInt(x, y, (ushort)(z - 1)), blocks[C.b]);
                                                              }

                                                              if (physics > 1)
                                                              {
                                                                  if (C.time > 10)
                                                                  {
                                                                      C.time = 0;

                                                                      if (Block.LavaKill(GetTile((ushort)(x + 1), y, z)))
                                                                      {
                                                                          AddUpdate(PosToInt((ushort)(x + 1), y, z),
                                                                                    Block.magma);
                                                                          InnerChange = true;
                                                                      }
                                                                      if (Block.LavaKill(GetTile((ushort)(x - 1), y, z)))
                                                                      {
                                                                          AddUpdate(PosToInt((ushort)(x - 1), y, z),
                                                                                    Block.magma);
                                                                          InnerChange = true;
                                                                      }
                                                                      if (Block.LavaKill(GetTile(x, y, (ushort)(z + 1))))
                                                                      {
                                                                          AddUpdate(PosToInt(x, y, (ushort)(z + 1)),
                                                                                    Block.magma);
                                                                          InnerChange = true;
                                                                      }
                                                                      if (Block.LavaKill(GetTile(x, y, (ushort)(z - 1))))
                                                                      {
                                                                          AddUpdate(PosToInt(x, y, (ushort)(z - 1)),
                                                                                    Block.magma);
                                                                          InnerChange = true;
                                                                      }
                                                                      if (Block.LavaKill(GetTile(x, (ushort)(y - 1), z)))
                                                                      {
                                                                          AddUpdate(PosToInt(x, (ushort)(y - 1), z),
                                                                                    Block.magma);
                                                                          InnerChange = true;
                                                                      }

                                                                      if (InnerChange)
                                                                      {
                                                                          if (
                                                                              Block.LavaKill(GetTile(x, (ushort)(y + 1),
                                                                                                     z)))
                                                                              AddUpdate(
                                                                                  PosToInt(x, (ushort)(y + 1), z),
                                                                                  Block.magma);
                                                                      }
                                                                  }
                                                              }

                                                              break;
                                                          case Block.geyser:
                                                              C.time++;

                                                              if (GetTile(x, (ushort)(y - 1), z) == Block.air)
                                                                  AddUpdate(PosToInt(x, (ushort)(y - 1), z),
                                                                            Block.geyser);
                                                              else if (GetTile(x, (ushort)(y - 1), z) != Block.geyser)
                                                              {
                                                                  PhysWater(PosToInt((ushort)(x + 1), y, z),
                                                                            blocks[C.b]);
                                                                  PhysWater(PosToInt((ushort)(x - 1), y, z),
                                                                            blocks[C.b]);
                                                                  PhysWater(PosToInt(x, y, (ushort)(z + 1)),
                                                                            blocks[C.b]);
                                                                  PhysWater(PosToInt(x, y, (ushort)(z - 1)),
                                                                            blocks[C.b]);
                                                              }

                                                              if (physics > 1)
                                                              {
                                                                  if (C.time > 10)
                                                                  {
                                                                      C.time = 0;

                                                                      if (
                                                                          Block.WaterKill(GetTile((ushort)(x + 1), y, z)))
                                                                      {
                                                                          AddUpdate(PosToInt((ushort)(x + 1), y, z),
                                                                                    Block.geyser);
                                                                          InnerChange = true;
                                                                      }
                                                                      if (
                                                                          Block.WaterKill(GetTile((ushort)(x - 1), y, z)))
                                                                      {
                                                                          AddUpdate(PosToInt((ushort)(x - 1), y, z),
                                                                                    Block.geyser);
                                                                          InnerChange = true;
                                                                      }
                                                                      if (
                                                                          Block.WaterKill(GetTile(x, y, (ushort)(z + 1))))
                                                                      {
                                                                          AddUpdate(PosToInt(x, y, (ushort)(z + 1)),
                                                                                    Block.geyser);
                                                                          InnerChange = true;
                                                                      }
                                                                      if (
                                                                          Block.WaterKill(GetTile(x, y, (ushort)(z - 1))))
                                                                      {
                                                                          AddUpdate(PosToInt(x, y, (ushort)(z - 1)),
                                                                                    Block.geyser);
                                                                          InnerChange = true;
                                                                      }
                                                                      if (
                                                                          Block.WaterKill(GetTile(x, (ushort)(y - 1), z)))
                                                                      {
                                                                          AddUpdate(PosToInt(x, (ushort)(y - 1), z),
                                                                                    Block.geyser);
                                                                          InnerChange = true;
                                                                      }

                                                                      if (InnerChange)
                                                                      {
                                                                          if (
                                                                              Block.WaterKill(GetTile(x,
                                                                                                      (ushort)(y + 1),
                                                                                                      z)))
                                                                              AddUpdate(
                                                                                  PosToInt(x, (ushort)(y + 1), z),
                                                                                  Block.geyser);
                                                                      }
                                                                  }
                                                              }
                                                              break;

                                                          case Block.birdblack:
                                                          case Block.birdwhite:
                                                          case Block.birdlava:
                                                          case Block.birdwater:
                                                              switch (rand.Next(1, 15))
                                                              {
                                                                  case 1:
                                                                      if (GetTile(x, (ushort)(y - 1), z) == Block.air)
                                                                          AddUpdate(PosToInt(x, (ushort)(y - 1), z),
                                                                                    blocks[C.b]);
                                                                      else goto case 3;
                                                                      break;
                                                                  case 2:
                                                                      if (GetTile(x, (ushort)(y + 1), z) == Block.air)
                                                                          AddUpdate(PosToInt(x, (ushort)(y + 1), z),
                                                                                    blocks[C.b]);
                                                                      else goto case 6;
                                                                      break;
                                                                  case 3:
                                                                  case 4:
                                                                  case 5:
                                                                      switch (GetTile((ushort)(x - 1), y, z))
                                                                      {
                                                                          case Block.air:
                                                                              AddUpdate(PosToInt((ushort)(x - 1), y, z),
                                                                                        blocks[C.b]);
                                                                              break;
                                                                          case Block.op_air:
                                                                              break;
                                                                          default:
                                                                              AddUpdate(C.b, Block.red, false,
                                                                                        "dissipate 25");
                                                                              break;
                                                                      }
                                                                      break;
                                                                  case 6:
                                                                  case 7:
                                                                  case 8:
                                                                      switch (GetTile((ushort)(x + 1), y, z))
                                                                      {
                                                                          case Block.air:
                                                                              AddUpdate(PosToInt((ushort)(x + 1), y, z),
                                                                                        blocks[C.b]);
                                                                              break;
                                                                          case Block.op_air:
                                                                              break;
                                                                          default:
                                                                              AddUpdate(C.b, Block.red, false,
                                                                                        "dissipate 25");
                                                                              break;
                                                                      }
                                                                      break;
                                                                  case 9:
                                                                  case 10:
                                                                  case 11:
                                                                      switch (GetTile(x, y, (ushort)(z - 1)))
                                                                      {
                                                                          case Block.air:
                                                                              AddUpdate(PosToInt(x, y, (ushort)(z - 1)),
                                                                                        blocks[C.b]);
                                                                              break;
                                                                          case Block.op_air:
                                                                              break;
                                                                          default:
                                                                              AddUpdate(C.b, Block.red, false,
                                                                                        "dissipate 25");
                                                                              break;
                                                                      }
                                                                      break;
                                                                  default:
                                                                      switch (GetTile(x, y, (ushort)(z + 1)))
                                                                      {
                                                                          case Block.air:
                                                                              AddUpdate(PosToInt(x, y, (ushort)(z + 1)),
                                                                                        blocks[C.b]);
                                                                              break;
                                                                          case Block.op_air:
                                                                              break;
                                                                          default:
                                                                              AddUpdate(C.b, Block.red, false,
                                                                                        "dissipate 25");
                                                                              break;
                                                                      }
                                                                      break;
                                                              }
                                                              AddUpdate(C.b, Block.air);
                                                              C.time = 255;

                                                              break;

                                                          case Block.snaketail:
                                                              if (GetTile(IntOffset(C.b, -1, 0, 0)) != Block.snake ||
                                                                  GetTile(IntOffset(C.b, 1, 0, 0)) != Block.snake ||
                                                                  GetTile(IntOffset(C.b, 0, 0, 1)) != Block.snake ||
                                                                  GetTile(IntOffset(C.b, 0, 0, -1)) != Block.snake)
                                                                  C.extraInfo = "revert 0";
                                                              break;
                                                          case Block.snake:

                                                              #region SNAKE

                                                              if (ai)
                                                                  Player.players.ForEach(delegate(Player p)
                                                                                             {
                                                                                                 if (p.level == this &&
                                                                                                     !p.invincible)
                                                                                                 {
                                                                                                     currentNum =
                                                                                                         Math.Abs(
                                                                                                             (p.pos[0] /
                                                                                                              32) - x) +
                                                                                                         Math.Abs(
                                                                                                             (p.pos[1] /
                                                                                                              32) - y) +
                                                                                                         Math.Abs(
                                                                                                             (p.pos[2] /
                                                                                                              32) - z);
                                                                                                     if (currentNum <
                                                                                                         foundNum)
                                                                                                     {
                                                                                                         foundNum =
                                                                                                             currentNum;
                                                                                                         foundPlayer = p;
                                                                                                     }
                                                                                                 }
                                                                                             });

                                                          randomMovement_Snake:
                                                              if (foundPlayer != null && rand.Next(1, 20) < 19)
                                                              {
                                                                  currentNum = rand.Next(1, 10);
                                                                  foundNum = 0;

                                                                  switch (currentNum)
                                                                  {
                                                                      case 1:
                                                                      case 2:
                                                                      case 3:
                                                                          if ((foundPlayer.pos[0] / 32) - x != 0)
                                                                          {
                                                                              newNum =
                                                                                  PosToInt(
                                                                                      (ushort)
                                                                                      (x +
                                                                                       Math.Sign((foundPlayer.pos[0] / 32) -
                                                                                                 x)), y, z);
                                                                              if (GetTile(newNum) == Block.air)
                                                                                  if (IntOffset(newNum, -1, 0, 0) ==
                                                                                      Block.grass ||
                                                                                      IntOffset(newNum, -1, 0, 0) ==
                                                                                      Block.dirt)
                                                                                      if (AddUpdate(newNum, blocks[C.b]))
                                                                                          goto removeSelf_Snake;
                                                                          }
                                                                          foundNum++;
                                                                          if (foundNum >= 3) goto default;
                                                                          goto case 4;

                                                                      case 4:
                                                                      case 5:
                                                                      case 6:
                                                                          if ((foundPlayer.pos[1] / 32) - y != 0)
                                                                          {
                                                                              newNum = PosToInt(x,
                                                                                                (ushort)
                                                                                                (y +
                                                                                                 Math.Sign(
                                                                                                     (foundPlayer.pos[1] /
                                                                                                      32) - y)), z);
                                                                              if (GetTile(newNum) == Block.air)
                                                                                  if (newNum > 0)
                                                                                  {
                                                                                      if (IntOffset(newNum, 0, 1, 0) ==
                                                                                          Block.grass ||
                                                                                          IntOffset(newNum, 0, 1, 0) ==
                                                                                          Block.dirt &&
                                                                                          IntOffset(newNum, 0, 2, 0) ==
                                                                                          Block.air)
                                                                                          if (AddUpdate(newNum,
                                                                                                        blocks[C.b]))
                                                                                              goto removeSelf_Snake;
                                                                                  }
                                                                                  else if (newNum < 0)
                                                                                  {
                                                                                      if (IntOffset(newNum, 0, -2, 0) ==
                                                                                          Block.grass ||
                                                                                          IntOffset(newNum, 0, -2, 0) ==
                                                                                          Block.dirt &&
                                                                                          IntOffset(newNum, 0, -1, 0) ==
                                                                                          Block.air)
                                                                                          if (AddUpdate(newNum,
                                                                                                        blocks[C.b]))
                                                                                              goto removeSelf_Snake;
                                                                                  }
                                                                          }
                                                                          foundNum++;
                                                                          if (foundNum >= 3) goto default;
                                                                          goto case 7;

                                                                      case 7:
                                                                      case 8:
                                                                      case 9:
                                                                          if ((foundPlayer.pos[2] / 32) - z != 0)
                                                                          {
                                                                              newNum = PosToInt(x, y,
                                                                                                (ushort)
                                                                                                (z +
                                                                                                 Math.Sign(
                                                                                                     (foundPlayer.pos[2] /
                                                                                                      32) - z)));
                                                                              if (GetTile(newNum) == Block.air)
                                                                                  if (IntOffset(newNum, 0, 0, -1) ==
                                                                                      Block.grass ||
                                                                                      IntOffset(newNum, 0, 0, -1) ==
                                                                                      Block.dirt)
                                                                                      if (AddUpdate(newNum, blocks[C.b]))
                                                                                          goto removeSelf_Snake;
                                                                          }
                                                                          foundNum++;
                                                                          if (foundNum >= 3) goto default;
                                                                          else goto case 1;
                                                                      default:
                                                                          foundPlayer = null;
                                                                          goto randomMovement_Snake;
                                                                  }
                                                              }
                                                              else
                                                              {
                                                                  switch (rand.Next(1, 13))
                                                                  {
                                                                      case 1:
                                                                      case 2:
                                                                      case 3:
                                                                          newNum = IntOffset(C.b, -1, 0, 0);
                                                                          oldNum = PosToInt(x, y, z);

                                                                          if (GetTile(IntOffset(newNum, 0, -1, 0)) ==
                                                                              Block.air && GetTile(newNum) == Block.air)
                                                                              newNum = IntOffset(newNum, 0, -1, 0);
                                                                          else if (GetTile(newNum) == Block.air &&
                                                                                   GetTile(IntOffset(newNum, 0, 1, 0)) ==
                                                                                   Block.air)
                                                                          {
                                                                          }

                                                                          else if (
                                                                              GetTile(IntOffset(newNum, 0, 2, 0)) ==
                                                                              Block.air &&
                                                                              GetTile(IntOffset(newNum, 0, 1, 0)) ==
                                                                              Block.air)
                                                                              newNum = IntOffset(newNum, 0, 1, 0);
                                                                          else skip = true; //Not used...

                                                                          if (AddUpdate(newNum, blocks[C.b]))
                                                                          {
                                                                              AddUpdate(IntOffset(oldNum, 0, 0, 0),
                                                                                        Block.snaketail, true,
                                                                                        string.Format("wait 5 revert {0}", Block.air));
                                                                              goto removeSelf_Snake;
                                                                          }

                                                                          foundNum++;
                                                                          if (foundNum >= 4) InnerChange = true;
                                                                          else goto case 4;
                                                                          break;

                                                                      case 4:
                                                                      case 5:
                                                                      case 6:
                                                                          newNum = IntOffset(C.b, 1, 0, 0);
                                                                          oldNum = PosToInt(x, y, z);

                                                                          if (GetTile(IntOffset(newNum, 0, -1, 0)) ==
                                                                              Block.air && GetTile(newNum) == Block.air)
                                                                              newNum = IntOffset(newNum, 0, -1, 0);
                                                                          else if (GetTile(newNum) == Block.air &&
                                                                                   GetTile(IntOffset(newNum, 0, 1, 0)) ==
                                                                                   Block.air)
                                                                          {
                                                                          }

                                                                          else if (
                                                                              GetTile(IntOffset(newNum, 0, 2, 0)) ==
                                                                              Block.air &&
                                                                              GetTile(IntOffset(newNum, 0, 1, 0)) ==
                                                                              Block.air)
                                                                              newNum = IntOffset(newNum, 0, 1, 0);
                                                                          else skip = true;

                                                                          if (AddUpdate(newNum, blocks[C.b]))
                                                                          {
                                                                              AddUpdate(IntOffset(oldNum, 0, 0, 0),
                                                                                        Block.snaketail, true,
                                                                                        "wait 5 revert " +
                                                                                        Block.air.ToString());
                                                                              goto removeSelf_Snake;
                                                                          }

                                                                          foundNum++;
                                                                          if (foundNum >= 4) InnerChange = true;
                                                                          else goto case 7;
                                                                          break;

                                                                      case 7:
                                                                      case 8:
                                                                      case 9:
                                                                          newNum = IntOffset(C.b, 0, 0, 1);
                                                                          oldNum = PosToInt(x, y, z);

                                                                          if (GetTile(IntOffset(newNum, 0, -1, 0)) ==
                                                                              Block.air && GetTile(newNum) == Block.air)
                                                                              newNum = IntOffset(newNum, 0, -1, 0);
                                                                          else if (GetTile(newNum) == Block.air &&
                                                                                   GetTile(IntOffset(newNum, 0, 1, 0)) ==
                                                                                   Block.air)
                                                                          {
                                                                          }

                                                                          else if (
                                                                              GetTile(IntOffset(newNum, 0, 2, 0)) ==
                                                                              Block.air &&
                                                                              GetTile(IntOffset(newNum, 0, 1, 0)) ==
                                                                              Block.air)
                                                                              newNum = IntOffset(newNum, 0, 1, 0);
                                                                          else skip = true;

                                                                          if (AddUpdate(newNum, blocks[C.b]))
                                                                          {
                                                                              AddUpdate(IntOffset(oldNum, 0, 0, 0),
                                                                                        Block.snaketail, true,
                                                                                        "wait 5 revert " +
                                                                                        Block.air.ToString());
                                                                              goto removeSelf_Snake;
                                                                          }

                                                                          foundNum++;
                                                                          if (foundNum >= 4) InnerChange = true;
                                                                          else goto case 10;
                                                                          break;
                                                                      case 10:
                                                                      case 11:
                                                                      case 12:
                                                                      default:
                                                                          newNum = IntOffset(C.b, 0, 0, -1);
                                                                          oldNum = PosToInt(x, y, z);

                                                                          if (GetTile(IntOffset(newNum, 0, -1, 0)) ==
                                                                              Block.air && GetTile(newNum) == Block.air)
                                                                              newNum = IntOffset(newNum, 0, -1, 0);
                                                                          else if (GetTile(newNum) == Block.air &&
                                                                                   GetTile(IntOffset(newNum, 0, 1, 0)) ==
                                                                                   Block.air)
                                                                          {
                                                                          }

                                                                          else if (
                                                                              GetTile(IntOffset(newNum, 0, 2, 0)) ==
                                                                              Block.air &&
                                                                              GetTile(IntOffset(newNum, 0, 1, 0)) ==
                                                                              Block.air)
                                                                              newNum = IntOffset(newNum, 0, 1, 0);
                                                                          else skip = true;

                                                                          if (AddUpdate(newNum, blocks[C.b]))
                                                                          {
                                                                              AddUpdate(IntOffset(oldNum, 0, 0, 0),
                                                                                        Block.snaketail, true,
                                                                                        "wait 5 revert " +
                                                                                        Block.air.ToString());
                                                                              goto removeSelf_Snake;
                                                                          }

                                                                          foundNum++;
                                                                          if (foundNum >= 4) InnerChange = true;
                                                                          else goto case 1;
                                                                          break;
                                                                  }
                                                              }

                                                          removeSelf_Snake:
                                                              if (!InnerChange)
                                                                  AddUpdate(C.b, Block.air);
                                                              break;

                                                              #endregion

                                                          case Block.birdred:
                                                          case Block.birdblue:
                                                          case Block.birdkill:

                                                              #region HUNTER BIRDS

                                                              if (ai)
                                                                  Player.players.ForEach(delegate(Player p)
                                                                                             {
                                                                                                 if (p.level == this &&
                                                                                                     !p.invincible)
                                                                                                 {
                                                                                                     currentNum =
                                                                                                         Math.Abs(
                                                                                                             (p.pos[0] /
                                                                                                              32) - x) +
                                                                                                         Math.Abs(
                                                                                                             (p.pos[1] /
                                                                                                              32) - y) +
                                                                                                         Math.Abs(
                                                                                                             (p.pos[2] /
                                                                                                              32) - z);
                                                                                                     if (currentNum <
                                                                                                         foundNum)
                                                                                                     {
                                                                                                         foundNum =
                                                                                                             currentNum;
                                                                                                         foundPlayer = p;
                                                                                                     }
                                                                                                 }
                                                                                             });

                                                          randomMovement:
                                                              if (foundPlayer != null && rand.Next(1, 20) < 19)
                                                              {
                                                                  currentNum = rand.Next(1, 10);
                                                                  foundNum = 0;

                                                                  switch (currentNum)
                                                                  {
                                                                      case 1:
                                                                      case 2:
                                                                      case 3:
                                                                          if ((foundPlayer.pos[0] / 32) - x != 0)
                                                                          {
                                                                              newNum =
                                                                                  PosToInt(
                                                                                      (ushort)
                                                                                      (x +
                                                                                       Math.Sign((foundPlayer.pos[0] / 32) -
                                                                                                 x)), y, z);
                                                                              if (GetTile(newNum) == Block.air)
                                                                                  if (AddUpdate(newNum, blocks[C.b]))
                                                                                      goto removeSelf;
                                                                          }

                                                                          foundNum++;
                                                                          if (foundNum >= 3) goto default;
                                                                          else goto case 4;
                                                                      case 4:
                                                                      case 5:
                                                                      case 6:
                                                                          if ((foundPlayer.pos[1] / 32) - y != 0)
                                                                          {
                                                                              newNum = PosToInt(x,
                                                                                                (ushort)
                                                                                                (y +
                                                                                                 Math.Sign(
                                                                                                     (foundPlayer.pos[1] /
                                                                                                      32) - y)), z);
                                                                              if (GetTile(newNum) == Block.air)
                                                                                  if (AddUpdate(newNum, blocks[C.b]))
                                                                                      goto removeSelf;
                                                                          }

                                                                          foundNum++;
                                                                          if (foundNum >= 3) goto default;
                                                                          else goto case 7;
                                                                      case 7:
                                                                      case 8:
                                                                      case 9:
                                                                          if ((foundPlayer.pos[2] / 32) - z != 0)
                                                                          {
                                                                              newNum = PosToInt(x, y,
                                                                                                (ushort)
                                                                                                (z +
                                                                                                 Math.Sign(
                                                                                                     (foundPlayer.pos[2] /
                                                                                                      32) - z)));
                                                                              if (GetTile(newNum) == Block.air)
                                                                                  if (AddUpdate(newNum, blocks[C.b]))
                                                                                      goto removeSelf;
                                                                          }

                                                                          foundNum++;
                                                                          if (foundNum >= 3) goto default;
                                                                          else goto case 1;
                                                                      default:
                                                                          foundPlayer = null;
                                                                          goto randomMovement;
                                                                  }
                                                              }
                                                              else
                                                              {
                                                                  switch (rand.Next(1, 15))
                                                                  {
                                                                      case 1:
                                                                          if (GetTile(x, (ushort)(y - 1), z) ==
                                                                              Block.air)
                                                                              if (
                                                                                  AddUpdate(
                                                                                      PosToInt(x, (ushort)(y - 1), z),
                                                                                      blocks[C.b])) break;
                                                                              else goto case 3;
                                                                          else goto case 3;
                                                                      case 2:
                                                                          if (GetTile(x, (ushort)(y + 1), z) ==
                                                                              Block.air)
                                                                              if (
                                                                                  AddUpdate(
                                                                                      PosToInt(x, (ushort)(y + 1), z),
                                                                                      blocks[C.b])) break;
                                                                              else goto case 6;
                                                                          else goto case 6;
                                                                      case 3:
                                                                      case 4:
                                                                      case 5:
                                                                          if (GetTile((ushort)(x - 1), y, z) ==
                                                                              Block.air)
                                                                              if (
                                                                                  AddUpdate(
                                                                                      PosToInt((ushort)(x - 1), y, z),
                                                                                      blocks[C.b])) break;
                                                                              else goto case 9;
                                                                          else goto case 9;
                                                                      case 6:
                                                                      case 7:
                                                                      case 8:
                                                                          if (GetTile((ushort)(x + 1), y, z) ==
                                                                              Block.air)
                                                                              if (
                                                                                  AddUpdate(
                                                                                      PosToInt((ushort)(x + 1), y, z),
                                                                                      blocks[C.b])) break;
                                                                              else goto case 12;
                                                                          else goto case 12;
                                                                      case 9:
                                                                      case 10:
                                                                      case 11:
                                                                          if (GetTile(x, y, (ushort)(z - 1)) ==
                                                                              Block.air)
                                                                              if (
                                                                                  AddUpdate(
                                                                                      PosToInt(x, y, (ushort)(z - 1)),
                                                                                      blocks[C.b])) break;
                                                                              else InnerChange = true;
                                                                          else InnerChange = true;
                                                                          break;
                                                                      case 12:
                                                                      case 13:
                                                                      case 14:
                                                                      default:
                                                                          if (GetTile(x, y, (ushort)(z + 1)) ==
                                                                              Block.air)
                                                                              if (
                                                                                  AddUpdate(
                                                                                      PosToInt(x, y, (ushort)(z + 1)),
                                                                                      blocks[C.b])) break;
                                                                              else InnerChange = true;
                                                                          else InnerChange = true;
                                                                          break;
                                                                  }
                                                              }

                                                          removeSelf:
                                                              if (!InnerChange)
                                                                  AddUpdate(C.b, Block.air);
                                                              break;

                                                              #endregion

                                                          case Block.fishbetta:
                                                          case Block.fishgold:
                                                          case Block.fishsalmon:
                                                          case Block.fishshark:
                                                          case Block.fishsponge:

                                                              #region FISH

                                                              if (ai)
                                                                  Player.players.ForEach(delegate(Player p)
                                                                                             {
                                                                                                 if (p.level == this &&
                                                                                                     !p.invincible)
                                                                                                 {
                                                                                                     currentNum =
                                                                                                         Math.Abs(
                                                                                                             (p.pos[0] /
                                                                                                              32) - x) +
                                                                                                         Math.Abs(
                                                                                                             (p.pos[1] /
                                                                                                              32) - y) +
                                                                                                         Math.Abs(
                                                                                                             (p.pos[2] /
                                                                                                              32) - z);
                                                                                                     if (currentNum <
                                                                                                         foundNum)
                                                                                                     {
                                                                                                         foundNum =
                                                                                                             currentNum;
                                                                                                         foundPlayer = p;
                                                                                                     }
                                                                                                 }
                                                                                             });

                                                          randomMovement_fish:
                                                              if (foundPlayer != null && rand.Next(1, 20) < 19)
                                                              {
                                                                  currentNum = rand.Next(1, 10);
                                                                  foundNum = 0;

                                                                  switch (currentNum)
                                                                  {
                                                                      case 1:
                                                                      case 2:
                                                                      case 3:
                                                                          if ((foundPlayer.pos[0] / 32) - x != 0)
                                                                          {
                                                                              if (blocks[C.b] == Block.fishbetta ||
                                                                                  blocks[C.b] == Block.fishshark)
                                                                                  newNum =
                                                                                      PosToInt(
                                                                                          (ushort)
                                                                                          (x +
                                                                                           Math.Sign(
                                                                                               (foundPlayer.pos[0] / 32) -
                                                                                               x)), y, z);
                                                                              else
                                                                                  newNum =
                                                                                      PosToInt(
                                                                                          (ushort)
                                                                                          (x -
                                                                                           Math.Sign(
                                                                                               (foundPlayer.pos[0] / 32) -
                                                                                               x)), y, z);


                                                                              if (GetTile(newNum) == Block.water)
                                                                                  if (AddUpdate(newNum, blocks[C.b]))
                                                                                      goto removeSelf_fish;
                                                                          }

                                                                          foundNum++;
                                                                          if (foundNum >= 3) goto default;
                                                                          else goto case 4;
                                                                      case 4:
                                                                      case 5:
                                                                      case 6:
                                                                          if ((foundPlayer.pos[1] / 32) - y != 0)
                                                                          {
                                                                              if (blocks[C.b] == Block.fishbetta ||
                                                                                  blocks[C.b] == Block.fishshark)
                                                                                  newNum = PosToInt(x,
                                                                                                    (ushort)
                                                                                                    (y +
                                                                                                     Math.Sign(
                                                                                                         (foundPlayer.
                                                                                                              pos[1] / 32) -
                                                                                                         y)), z);
                                                                              else
                                                                                  newNum = PosToInt(x,
                                                                                                    (ushort)
                                                                                                    (y -
                                                                                                     Math.Sign(
                                                                                                         (foundPlayer.
                                                                                                              pos[1] / 32) -
                                                                                                         y)), z);

                                                                              if (GetTile(newNum) == Block.water)
                                                                                  if (AddUpdate(newNum, blocks[C.b]))
                                                                                      goto removeSelf_fish;
                                                                          }

                                                                          foundNum++;
                                                                          if (foundNum >= 3) goto default;
                                                                          else goto case 7;
                                                                      case 7:
                                                                      case 8:
                                                                      case 9:
                                                                          if ((foundPlayer.pos[2] / 32) - z != 0)
                                                                          {
                                                                              if (blocks[C.b] == Block.fishbetta ||
                                                                                  blocks[C.b] == Block.fishshark)
                                                                                  newNum = PosToInt(x, y,
                                                                                                    (ushort)
                                                                                                    (z +
                                                                                                     Math.Sign(
                                                                                                         (foundPlayer.
                                                                                                              pos[2] / 32) -
                                                                                                         z)));
                                                                              else
                                                                                  newNum = PosToInt(x, y,
                                                                                                    (ushort)
                                                                                                    (z -
                                                                                                     Math.Sign(
                                                                                                         (foundPlayer.
                                                                                                              pos[2] / 32) -
                                                                                                         z)));

                                                                              if (GetTile(newNum) == Block.water)
                                                                                  if (AddUpdate(newNum, blocks[C.b]))
                                                                                      goto removeSelf_fish;
                                                                          }

                                                                          foundNum++;
                                                                          if (foundNum >= 3) goto default;
                                                                          else goto case 1;
                                                                      default:
                                                                          foundPlayer = null;
                                                                          goto randomMovement_fish;
                                                                  }
                                                              }
                                                              else
                                                              {
                                                                  switch (rand.Next(1, 15))
                                                                  {
                                                                      case 1:
                                                                          if (GetTile(x, (ushort)(y - 1), z) ==
                                                                              Block.water)
                                                                              if (
                                                                                  AddUpdate(
                                                                                      PosToInt(x, (ushort)(y - 1), z),
                                                                                      blocks[C.b])) break;
                                                                              else goto case 3;
                                                                          else goto case 3;
                                                                      case 2:
                                                                          if (GetTile(x, (ushort)(y + 1), z) ==
                                                                              Block.water)
                                                                              if (
                                                                                  AddUpdate(
                                                                                      PosToInt(x, (ushort)(y + 1), z),
                                                                                      blocks[C.b])) break;
                                                                              else goto case 6;
                                                                          else goto case 6;
                                                                      case 3:
                                                                      case 4:
                                                                      case 5:
                                                                          if (GetTile((ushort)(x - 1), y, z) ==
                                                                              Block.water)
                                                                              if (
                                                                                  AddUpdate(
                                                                                      PosToInt((ushort)(x - 1), y, z),
                                                                                      blocks[C.b])) break;
                                                                              else goto case 9;
                                                                          else goto case 9;
                                                                      case 6:
                                                                      case 7:
                                                                      case 8:
                                                                          if (GetTile((ushort)(x + 1), y, z) ==
                                                                              Block.water)
                                                                              if (
                                                                                  AddUpdate(
                                                                                      PosToInt((ushort)(x + 1), y, z),
                                                                                      blocks[C.b])) break;
                                                                              else goto case 12;
                                                                          else goto case 12;
                                                                      case 9:
                                                                      case 10:
                                                                      case 11:
                                                                          if (GetTile(x, y, (ushort)(z - 1)) ==
                                                                              Block.water)
                                                                              if (
                                                                                  AddUpdate(
                                                                                      PosToInt(x, y, (ushort)(z - 1)),
                                                                                      blocks[C.b])) break;
                                                                              else InnerChange = true;
                                                                          else InnerChange = true;
                                                                          break;
                                                                      case 12:
                                                                      case 13:
                                                                      case 14:
                                                                      default:
                                                                          if (GetTile(x, y, (ushort)(z + 1)) ==
                                                                              Block.water)
                                                                              if (
                                                                                  AddUpdate(
                                                                                      PosToInt(x, y, (ushort)(z + 1)),
                                                                                      blocks[C.b])) break;
                                                                              else InnerChange = true;
                                                                          else InnerChange = true;
                                                                          break;
                                                                  }
                                                              }

                                                          removeSelf_fish:
                                                              if (!InnerChange)
                                                                  AddUpdate(C.b, Block.water);
                                                              break;

                                                              #endregion

                                                          case Block.fishlavashark:

                                                              #region lavafish

                                                              if (ai)
                                                                  Player.players.ForEach(delegate(Player p)
                                                                                             {
                                                                                                 if (p.level == this &&
                                                                                                     !p.invincible)
                                                                                                 {
                                                                                                     currentNum =
                                                                                                         Math.Abs(
                                                                                                             (p.pos[0] /
                                                                                                              32) - x) +
                                                                                                         Math.Abs(
                                                                                                             (p.pos[1] /
                                                                                                              32) - y) +
                                                                                                         Math.Abs(
                                                                                                             (p.pos[2] /
                                                                                                              32) - z);
                                                                                                     if (currentNum <
                                                                                                         foundNum)
                                                                                                     {
                                                                                                         foundNum =
                                                                                                             currentNum;
                                                                                                         foundPlayer = p;
                                                                                                     }
                                                                                                 }
                                                                                             });

                                                          randomMovement_lavafish:
                                                              if (foundPlayer != null && rand.Next(1, 20) < 19)
                                                              {
                                                                  currentNum = rand.Next(1, 10);
                                                                  foundNum = 0;

                                                                  switch (currentNum)
                                                                  {
                                                                      case 1:
                                                                      case 2:
                                                                      case 3:
                                                                          if ((foundPlayer.pos[0] / 32) - x != 0)
                                                                          {
                                                                              if (blocks[C.b] == Block.fishlavashark)
                                                                                  newNum =
                                                                                      PosToInt(
                                                                                          (ushort)
                                                                                          (x +
                                                                                           Math.Sign(
                                                                                               (foundPlayer.pos[0] / 32) -
                                                                                               x)), y, z);
                                                                              else
                                                                                  newNum =
                                                                                      PosToInt(
                                                                                          (ushort)
                                                                                          (x -
                                                                                           Math.Sign(
                                                                                               (foundPlayer.pos[0] / 32) -
                                                                                               x)), y, z);


                                                                              if (GetTile(newNum) == Block.lava)
                                                                                  if (AddUpdate(newNum, blocks[C.b]))
                                                                                      goto removeSelf_lavafish;
                                                                          }

                                                                          foundNum++;
                                                                          if (foundNum >= 3) goto default;
                                                                          else goto case 4;
                                                                      case 4:
                                                                      case 5:
                                                                      case 6:
                                                                          if ((foundPlayer.pos[1] / 32) - y != 0)
                                                                          {
                                                                              if (blocks[C.b] == Block.fishlavashark)
                                                                                  newNum = PosToInt(x,
                                                                                                    (ushort)
                                                                                                    (y +
                                                                                                     Math.Sign(
                                                                                                         (foundPlayer.
                                                                                                              pos[1] / 32) -
                                                                                                         y)), z);
                                                                              else
                                                                                  newNum = PosToInt(x,
                                                                                                    (ushort)
                                                                                                    (y -
                                                                                                     Math.Sign(
                                                                                                         (foundPlayer.
                                                                                                              pos[1] / 32) -
                                                                                                         y)), z);

                                                                              if (GetTile(newNum) == Block.lava)
                                                                                  if (AddUpdate(newNum, blocks[C.b]))
                                                                                      goto removeSelf_lavafish;
                                                                          }

                                                                          foundNum++;
                                                                          if (foundNum >= 3) goto default;
                                                                          else goto case 7;
                                                                      case 7:
                                                                      case 8:
                                                                      case 9:
                                                                          if ((foundPlayer.pos[2] / 32) - z != 0)
                                                                          {
                                                                              if (blocks[C.b] == Block.fishlavashark)
                                                                                  newNum = PosToInt(x, y,
                                                                                                    (ushort)
                                                                                                    (z +
                                                                                                     Math.Sign(
                                                                                                         (foundPlayer.
                                                                                                              pos[2] / 32) -
                                                                                                         z)));
                                                                              else
                                                                                  newNum = PosToInt(x, y,
                                                                                                    (ushort)
                                                                                                    (z -
                                                                                                     Math.Sign(
                                                                                                         (foundPlayer.
                                                                                                              pos[2] / 32) -
                                                                                                         z)));

                                                                              if (GetTile(newNum) == Block.lava)
                                                                                  if (AddUpdate(newNum, blocks[C.b]))
                                                                                      goto removeSelf_lavafish;
                                                                          }

                                                                          foundNum++;
                                                                          if (foundNum >= 3) goto default;
                                                                          else goto case 1;
                                                                      default:
                                                                          foundPlayer = null;
                                                                          goto randomMovement_lavafish;
                                                                  }
                                                              }
                                                              else
                                                              {
                                                                  switch (rand.Next(1, 15))
                                                                  {
                                                                      case 1:
                                                                          if (GetTile(x, (ushort)(y - 1), z) ==
                                                                              Block.lava)
                                                                              if (
                                                                                  AddUpdate(
                                                                                      PosToInt(x, (ushort)(y - 1), z),
                                                                                      blocks[C.b])) break;
                                                                              else goto case 3;
                                                                          else goto case 3;
                                                                      case 2:
                                                                          if (GetTile(x, (ushort)(y + 1), z) ==
                                                                              Block.lava)
                                                                              if (
                                                                                  AddUpdate(
                                                                                      PosToInt(x, (ushort)(y + 1), z),
                                                                                      blocks[C.b])) break;
                                                                              else goto case 6;
                                                                          else goto case 6;
                                                                      case 3:
                                                                      case 4:
                                                                      case 5:
                                                                          if (GetTile((ushort)(x - 1), y, z) ==
                                                                              Block.lava)
                                                                              if (
                                                                                  AddUpdate(
                                                                                      PosToInt((ushort)(x - 1), y, z),
                                                                                      blocks[C.b])) break;
                                                                              else goto case 9;
                                                                          else goto case 9;
                                                                      case 6:
                                                                      case 7:
                                                                      case 8:
                                                                          if (GetTile((ushort)(x + 1), y, z) ==
                                                                              Block.lava)
                                                                              if (
                                                                                  AddUpdate(
                                                                                      PosToInt((ushort)(x + 1), y, z),
                                                                                      blocks[C.b])) break;
                                                                              else goto case 12;
                                                                          else goto case 12;
                                                                      case 9:
                                                                      case 10:
                                                                      case 11:
                                                                          if (GetTile(x, y, (ushort)(z - 1)) ==
                                                                              Block.lava)
                                                                              if (
                                                                                  AddUpdate(
                                                                                      PosToInt(x, y, (ushort)(z - 1)),
                                                                                      blocks[C.b])) break;
                                                                              else InnerChange = true;
                                                                          else InnerChange = true;
                                                                          break;
                                                                      case 12:
                                                                      case 13:
                                                                      case 14:
                                                                      default:
                                                                          if (GetTile(x, y, (ushort)(z + 1)) ==
                                                                              Block.lava)
                                                                              if (
                                                                                  AddUpdate(
                                                                                      PosToInt(x, y, (ushort)(z + 1)),
                                                                                      blocks[C.b])) break;
                                                                              else InnerChange = true;
                                                                          else InnerChange = true;
                                                                          break;
                                                                  }
                                                              }

                                                          removeSelf_lavafish:
                                                              if (!InnerChange)
                                                                  AddUpdate(C.b, Block.lava);
                                                              break;

                                                              #endregion

                                                          case Block.rockethead:
                                                              if (rand.Next(1, 10) <= 5) mx = 1;
                                                              else mx = -1;
                                                              if (rand.Next(1, 10) <= 5) my = 1;
                                                              else my = -1;
                                                              if (rand.Next(1, 10) <= 5) mz = 1;
                                                              else mz = -1;

                                                              for (int cx = (-1 * mx);
                                                                   cx != ((1 * mx) + mx) && InnerChange == false;
                                                                   cx = cx + (1 * mx))
                                                                  for (int cy = (-1 * my);
                                                                       cy != ((1 * my) + my) && InnerChange == false;
                                                                       cy = cy + (1 * my))
                                                                      for (int cz = (-1 * mz);
                                                                           cz != ((1 * mz) + mz) && InnerChange == false;
                                                                           cz = cz + (1 * mz))
                                                                      {
                                                                          if (
                                                                              GetTile((ushort)(x + cx),
                                                                                      (ushort)(y + cy),
                                                                                      (ushort)(z + cz)) == Block.fire)
                                                                          {
                                                                              int bp1 = PosToInt((ushort)(x - cx),
                                                                                                 (ushort)(y - cy),
                                                                                                 (ushort)(z - cz));
                                                                              int bp2 = PosToInt(x, y, z);
                                                                              bool unblocked =
                                                                                  !ListUpdate.Exists(
                                                                                      Update => Update.b == bp1) &&
                                                                                  !ListUpdate.Exists(
                                                                                      Update => Update.b == bp2);
                                                                              if (unblocked &&
                                                                                  GetTile((ushort)(x - cx),
                                                                                          (ushort)(y - cy),
                                                                                          (ushort)(z - cz)) ==
                                                                                  Block.air ||
                                                                                  GetTile((ushort)(x - cx),
                                                                                          (ushort)(y - cy),
                                                                                          (ushort)(z - cz)) ==
                                                                                  Block.rocketstart)
                                                                              {
                                                                                  AddUpdate(
                                                                                      PosToInt((ushort)(x - cx),
                                                                                               (ushort)(y - cy),
                                                                                               (ushort)(z - cz)),
                                                                                      Block.rockethead);
                                                                                  AddUpdate(PosToInt(x, y, z),
                                                                                            Block.fire);
                                                                              }
                                                                              else if (
                                                                                  GetTile((ushort)(x - cx),
                                                                                          (ushort)(y - cy),
                                                                                          (ushort)(z - cz)) ==
                                                                                  Block.fire)
                                                                              {
                                                                              }
                                                                              else
                                                                              {
                                                                                  if (physics > 2)
                                                                                      MakeExplosion(x, y, z, 2);
                                                                                  else
                                                                                      AddUpdate(PosToInt(x, y, z),
                                                                                                Block.fire);
                                                                              }
                                                                              InnerChange = true;
                                                                          }
                                                                      }
                                                              break;

                                                          case Block.firework:
                                                              if (GetTile(x, (ushort)(y - 1), z) == Block.lavastill)
                                                              {
                                                                  if (GetTile(x, (ushort)(y + 1), z) == Block.air)
                                                                  {
                                                                      if ((depth / 100) * 80 < y) mx = rand.Next(1, 20);
                                                                      else mx = 5;

                                                                      if (mx > 1)
                                                                      {
                                                                          int bp = PosToInt(x, (ushort)(y + 1), z);
                                                                          bool unblocked =
                                                                              !ListUpdate.Exists(
                                                                                  Update => Update.b == bp);
                                                                          if (unblocked)
                                                                          {
                                                                              AddUpdate(
                                                                                  PosToInt(x, (ushort)(y + 1), z),
                                                                                  Block.firework, false);
                                                                              AddUpdate(PosToInt(x, y, z),
                                                                                        Block.lavastill, false,
                                                                                        "wait 1 dissipate 100");
                                                                              // AddUpdate(PosToInt(x, (ushort)(y - 1), z), Block.air);
                                                                              C.extraInfo = "wait 1 dissipate 100";
                                                                              break;
                                                                          }
                                                                      }
                                                                  }
                                                                  Firework(x, y, z, 4);
                                                                  break;
                                                              }
                                                              break;
                                                          //Zombie + creeper stuff
                                                          case Block.zombiehead:
                                                              if (GetTile(IntOffset(C.b, 0, -1, 0)) != Block.zombiebody &&
                                                                  GetTile(IntOffset(C.b, 0, -1, 0)) != Block.creeper)
                                                                  C.extraInfo = "revert 0";
                                                              break;
                                                          case Block.zombiebody:
                                                          case Block.creeper:

                                                              #region ZOMBIE

                                                              if (GetTile(x, (ushort)(y - 1), z) == Block.air)
                                                              {
                                                                  AddUpdate(C.b, Block.zombiehead);
                                                                  AddUpdate(IntOffset(C.b, 0, -1, 0), blocks[C.b]);
                                                                  AddUpdate(IntOffset(C.b, 0, 1, 0), Block.air);
                                                                  break;
                                                              }

                                                              if (ai)
                                                                  Player.players.ForEach(delegate(Player p)
                                                                                             {
                                                                                                 if (p.level == this &&
                                                                                                     !p.invincible)
                                                                                                 {
                                                                                                     currentNum =
                                                                                                         Math.Abs(
                                                                                                             (p.pos[0] /
                                                                                                              32) - x) +
                                                                                                         Math.Abs(
                                                                                                             (p.pos[1] /
                                                                                                              32) - y) +
                                                                                                         Math.Abs(
                                                                                                             (p.pos[2] /
                                                                                                              32) - z);
                                                                                                     if (currentNum <
                                                                                                         foundNum)
                                                                                                     {
                                                                                                         foundNum =
                                                                                                             currentNum;
                                                                                                         foundPlayer = p;
                                                                                                     }
                                                                                                 }
                                                                                             });

                                                          randomMovement_zomb:
                                                              if (foundPlayer != null && rand.Next(1, 20) < 18)
                                                              {
                                                                  currentNum = rand.Next(1, 7);
                                                                  foundNum = 0;

                                                                  switch (currentNum)
                                                                  {
                                                                      case 1:
                                                                      case 2:
                                                                      case 3:
                                                                          if ((foundPlayer.pos[0] / 32) - x != 0)
                                                                          {
                                                                              skip = false;
                                                                              newNum =
                                                                                  PosToInt(
                                                                                      (ushort)
                                                                                      (x +
                                                                                       Math.Sign((foundPlayer.pos[0] / 32) -
                                                                                                 x)), y, z);

                                                                              if (
                                                                                  GetTile(IntOffset(newNum, 0, -1, 0)) ==
                                                                                  Block.air &&
                                                                                  GetTile(newNum) == Block.air)
                                                                                  newNum = IntOffset(newNum, 0, -1, 0);
                                                                              else if (GetTile(newNum) == Block.air &&
                                                                                       GetTile(IntOffset(newNum, 0, 1, 0)) ==
                                                                                       Block.air)
                                                                              {
                                                                              }

                                                                              else if (
                                                                                  GetTile(IntOffset(newNum, 0, 2,
                                                                                                    0)) ==
                                                                                  Block.air &&
                                                                                  GetTile(IntOffset(newNum, 0, 1,
                                                                                                    0)) ==
                                                                                  Block.air)
                                                                                  newNum = IntOffset(newNum, 0,
                                                                                                     1, 0);
                                                                              else skip = true;

                                                                              if (!skip)
                                                                                  if (AddUpdate(newNum, blocks[C.b]))
                                                                                  {
                                                                                      AddUpdate(
                                                                                          IntOffset(newNum, 0, 1, 0),
                                                                                          Block.zombiehead);
                                                                                      goto removeSelf_zomb;
                                                                                  }
                                                                          }

                                                                          foundNum++;
                                                                          if (foundNum >= 2) goto default;
                                                                          else goto case 4;
                                                                      case 4:
                                                                      case 5:
                                                                      case 6:
                                                                          if ((foundPlayer.pos[2] / 32) - z != 0)
                                                                          {
                                                                              skip = false;
                                                                              newNum = PosToInt(x, y,
                                                                                                (ushort)
                                                                                                (z +
                                                                                                 Math.Sign(
                                                                                                     (foundPlayer.pos[2] /
                                                                                                      32) - z)));

                                                                              if (
                                                                                  GetTile(IntOffset(newNum, 0, -1, 0)) ==
                                                                                  Block.air &&
                                                                                  GetTile(newNum) == Block.air)
                                                                                  newNum = IntOffset(newNum, 0, -1, 0);
                                                                              else if (GetTile(newNum) == Block.air &&
                                                                                       GetTile(IntOffset(newNum, 0, 1, 0)) ==
                                                                                       Block.air)
                                                                              {
                                                                              }

                                                                              else if (
                                                                                  GetTile(IntOffset(newNum, 0, 2,
                                                                                                    0)) ==
                                                                                  Block.air &&
                                                                                  GetTile(IntOffset(newNum, 0, 1,
                                                                                                    0)) ==
                                                                                  Block.air)
                                                                                  newNum = IntOffset(newNum, 0,
                                                                                                     1, 0);
                                                                              else skip = true;

                                                                              if (!skip)
                                                                                  if (AddUpdate(newNum, blocks[C.b]))
                                                                                  {
                                                                                      AddUpdate(
                                                                                          IntOffset(newNum, 0, 1, 0),
                                                                                          Block.zombiehead);
                                                                                      goto removeSelf_zomb;
                                                                                  }
                                                                          }

                                                                          foundNum++;
                                                                          if (foundNum >= 2) goto default;
                                                                          else goto case 1;
                                                                      default:
                                                                          foundPlayer = null;
                                                                          skip = true;
                                                                          goto randomMovement_zomb;
                                                                  }
                                                              }
                                                              else
                                                              {
                                                                  if (!skip)
                                                                      if (C.time < 3)
                                                                      {
                                                                          C.time++;
                                                                          break;
                                                                      }

                                                                  foundNum = 0;
                                                                  switch (rand.Next(1, 13))
                                                                  {
                                                                      case 1:
                                                                      case 2:
                                                                      case 3:
                                                                          skip = false;
                                                                          newNum = IntOffset(C.b, -1, 0, 0);

                                                                          if (GetTile(IntOffset(newNum, 0, -1, 0)) ==
                                                                              Block.air && GetTile(newNum) == Block.air)
                                                                              newNum = IntOffset(newNum, 0, -1, 0);
                                                                          else if (GetTile(newNum) == Block.air &&
                                                                                   GetTile(IntOffset(newNum, 0, 1, 0)) ==
                                                                                   Block.air)
                                                                          {
                                                                          }

                                                                          else if (
                                                                              GetTile(IntOffset(newNum, 0, 2, 0)) ==
                                                                              Block.air &&
                                                                              GetTile(IntOffset(newNum, 0, 1, 0)) ==
                                                                              Block.air)
                                                                              newNum = IntOffset(newNum, 0, 1, 0);
                                                                          else skip = true;

                                                                          if (!skip)
                                                                              if (AddUpdate(newNum, blocks[C.b]))
                                                                              {
                                                                                  AddUpdate(IntOffset(newNum, 0, 1, 0),
                                                                                            Block.zombiehead);
                                                                                  goto removeSelf_zomb;
                                                                              }

                                                                          foundNum++;
                                                                          if (foundNum >= 4) InnerChange = true;
                                                                          else goto case 4;
                                                                          break;

                                                                      case 4:
                                                                      case 5:
                                                                      case 6:
                                                                          skip = false;
                                                                          newNum = IntOffset(C.b, 1, 0, 0);

                                                                          if (GetTile(IntOffset(newNum, 0, -1, 0)) ==
                                                                              Block.air && GetTile(newNum) == Block.air)
                                                                              newNum = IntOffset(newNum, 0, -1, 0);
                                                                          else if (GetTile(newNum) == Block.air &&
                                                                                   GetTile(IntOffset(newNum, 0, 1, 0)) ==
                                                                                   Block.air)
                                                                          {
                                                                          }

                                                                          else if (
                                                                              GetTile(IntOffset(newNum, 0, 2, 0)) ==
                                                                              Block.air &&
                                                                              GetTile(IntOffset(newNum, 0, 1, 0)) ==
                                                                              Block.air)
                                                                              newNum = IntOffset(newNum, 0, 1, 0);
                                                                          else skip = true;

                                                                          if (!skip)
                                                                              if (AddUpdate(newNum, blocks[C.b]))
                                                                              {
                                                                                  AddUpdate(IntOffset(newNum, 0, 1, 0),
                                                                                            Block.zombiehead);
                                                                                  goto removeSelf_zomb;
                                                                              }

                                                                          foundNum++;
                                                                          if (foundNum >= 4) InnerChange = true;
                                                                          else goto case 7;
                                                                          break;

                                                                      case 7:
                                                                      case 8:
                                                                      case 9:
                                                                          skip = false;
                                                                          newNum = IntOffset(C.b, 0, 0, 1);

                                                                          if (GetTile(IntOffset(newNum, 0, -1, 0)) ==
                                                                              Block.air && GetTile(newNum) == Block.air)
                                                                              newNum = IntOffset(newNum, 0, -1, 0);
                                                                          else if (GetTile(newNum) == Block.air &&
                                                                                   GetTile(IntOffset(newNum, 0, 1, 0)) ==
                                                                                   Block.air)
                                                                          {
                                                                          }

                                                                          else if (
                                                                              GetTile(IntOffset(newNum, 0, 2, 0)) ==
                                                                              Block.air &&
                                                                              GetTile(IntOffset(newNum, 0, 1, 0)) ==
                                                                              Block.air)
                                                                              newNum = IntOffset(newNum, 0, 1, 0);
                                                                          else skip = true;

                                                                          if (!skip)
                                                                              if (AddUpdate(newNum, blocks[C.b]))
                                                                              {
                                                                                  AddUpdate(IntOffset(newNum, 0, 1, 0),
                                                                                            Block.zombiehead);
                                                                                  goto removeSelf_zomb;
                                                                              }

                                                                          foundNum++;
                                                                          if (foundNum >= 4) InnerChange = true;
                                                                          else goto case 10;
                                                                          break;
                                                                      case 10:
                                                                      case 11:
                                                                      case 12:
                                                                      default:
                                                                          skip = false;
                                                                          newNum = IntOffset(C.b, 0, 0, -1);

                                                                          if (GetTile(IntOffset(newNum, 0, -1, 0)) ==
                                                                              Block.air && GetTile(newNum) == Block.air)
                                                                              newNum = IntOffset(newNum, 0, -1, 0);
                                                                          else if (GetTile(newNum) == Block.air &&
                                                                                   GetTile(IntOffset(newNum, 0, 1, 0)) ==
                                                                                   Block.air)
                                                                          {
                                                                          }

                                                                          else if (
                                                                              GetTile(IntOffset(newNum, 0, 2, 0)) ==
                                                                              Block.air &&
                                                                              GetTile(IntOffset(newNum, 0, 1, 0)) ==
                                                                              Block.air)
                                                                              newNum = IntOffset(newNum, 0, 1, 0);
                                                                          else skip = true;

                                                                          if (!skip)
                                                                              if (AddUpdate(newNum, blocks[C.b]))
                                                                              {
                                                                                  AddUpdate(IntOffset(newNum, 0, 1, 0),
                                                                                            Block.zombiehead);
                                                                                  goto removeSelf_zomb;
                                                                              }

                                                                          foundNum++;
                                                                          if (foundNum >= 4) InnerChange = true;
                                                                          else goto case 1;
                                                                          break;
                                                                  }
                                                              }

                                                          removeSelf_zomb:
                                                              if (!InnerChange)
                                                              {
                                                                  AddUpdate(C.b, Block.air);
                                                                  AddUpdate(IntOffset(C.b, 0, 1, 0), Block.air);
                                                              }
                                                              break;

                                                              #endregion

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
                                                              //non special blocks are then ignored, maybe it would be better to avoid getting here and cutting down the list
                                                              if (!C.extraInfo.Contains("wait")) C.time = 255;
                                                              break;
                                                      }
                                                  }
                                              }
                                              catch
                                              {
                                                  ListCheck.Remove(C);
                                                  //Server.s.Log(e.Message);
                                              }
                                          });

                    ListCheck.RemoveAll(Check => Check.time == 255); //Remove all that are finished with 255 time

                    lastUpdate = ListUpdate.Count;
                    ListUpdate.ForEach(delegate(Update C)
                                           {
                                               try
                                               {
                                                   //IntToPos(C.b, out x, out y, out z); NO!
                                                   Blockchange(C.b, C.type, false, C.extraInfo);
                                               }
                                               catch
                                               {
                                                   Server.s.Log("Phys update issue");
                                               }
                                           });

                    ListUpdate.Clear();

                    #endregion
                }
            }
            catch (Exception e)
            {
                Server.s.Log("Level physics error");
                Server.ErrorLog(e);
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

        private bool AddUpdate(int b, int type, bool overRide = false, string extraInfo = "")
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
                                          case 200:
                                          case 202:
                                          case 203:
                                              blocks[C.b] = 0;
                                              break;
                                          case 201:
                                              //blocks[C.b] = 111;
                                              Blockchange(x, y, z, 111);
                                              break;
                                          case 205:
                                              //blocks[C.b] = 113;
                                              Blockchange(x, y, z, 113);
                                              break;
                                          case 206:
                                              //blocks[C.b] = 114;
                                              Blockchange(x, y, z, 114);
                                              break;
                                          case 207:
                                              //blocks[C.b] = 115;
                                              Blockchange(x, y, z, 115);
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

        //================================================================================================================
        private void PhysWater(int b, byte type)
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

            switch (blocks[b])
            {
                case 0:
                    if (!PhysSpongeCheck(b))
                    {
                        AddUpdate(b, type);
                    }
                    break;

                case 10: //hit active_lava
                case 112: //hit lava_fast
                case Block.activedeathlava:
                    if (!PhysSpongeCheck(b))
                    {
                        AddUpdate(b, 1);
                    }
                    break;

                case 6:
                case 37:
                case 38:
                case 39:
                case 40:
                    if (physics > 1) //Adv physics kills flowers and mushrooms in water
                    {
                        if (physics != 5)
                        {
                            if (!PhysSpongeCheck(b))
                            {
                                AddUpdate(b, 0);
                            }
                        }
                    }
                    break;

                case 12: //sand
                case 13: //gravel
                case 110: //woodfloat
                    AddCheck(b);
                    break;

                default:
                    break;
            }
        }

        //================================================================================================================
        private bool PhysWaterCheck(int b)
        {
            if (b == -1)
            {
                return false;
            }
            ushort x, y, z;
            IntToPos(b, out x, out y, out z);
            if (Server.lava.active && Server.lava.map == this && Server.lava.InSafeZone(x, y, z))
            {
                return false;
            }

            switch (blocks[b])
            {
                case 0:
                    if (!PhysSpongeCheck(b))
                    {
                        return true;
                    }
                    break;

                case 10: //hit active_lava
                case 112: //hit lava_fast
                case Block.activedeathlava:
                    if (!PhysSpongeCheck(b))
                    {
                        return true;
                    }
                    break;

                case 6:
                case 37:
                case 38:
                case 39:
                case 40:
                    if (physics > 1) //Adv physics kills flowers and mushrooms in water
                    {
                        if (physics != 5)
                        {
                            if (!PhysSpongeCheck(b))
                            {
                                return true;
                            }
                        }
                    }
                    break;

                case 12: //sand
                case 13: //gravel
                case 110: //woodfloat
                    return true;
            }
            return false;
        }

        //================================================================================================================
        private void PhysLava(int b, byte type)
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
        private bool PhysLavaCheck(int b)
        {
            if (b == -1)
            {
                return false;
            }
            ushort x, y, z;
            IntToPos(b, out x, out y, out z);
            if (Server.lava.active && Server.lava.map == this && Server.lava.InSafeZone(x, y, z))
            {
                return false;
            }

            if (physics > 1 && physics != 5 && !PhysSpongeCheck(b, true) && blocks[b] >= 21 && blocks[b] <= 36)
            {
                return true;
            } // Adv physics destroys cloth
            switch (blocks[b])
            {
                case 0:
                    return true;

                case 8: //hit active_water
                case Block.activedeathwater:
                    if (!PhysSpongeCheck(b, true)) return true;
                    break;

                case 12: //sand
                    if (physics > 1) //Adv physics changes sand to glass next to lava
                    {
                        if (physics != 5)
                        {
                            return true;
                        }
                    }
                    else
                    {
                        return true;
                    }
                    break;

                case 13: //gravel
                    return true;

                case 5:
                case 6:
                case 17:
                case 18:
                case 37:
                case 38:
                case 39:
                case 40:
                    if (physics > 1 && physics != 5) //Adv physics kills flowers and mushrooms plus wood in lava
                        if (!PhysSpongeCheck(b, true)) return true;
                    break;
            }
            return false;
        }

        //================================================================================================================
        private void PhysAir(int b)
        {
            if (b == -1)
            {
                return;
            }
            if (Block.Convert(blocks[b]) == Block.water || Block.Convert(blocks[b]) == Block.lava ||
                (blocks[b] >= 21 && blocks[b] <= 36))
            {
                AddCheck(b);
                return;
            }

            switch (blocks[b])
            {
                //case 8:     //active water
                //case 10:    //active_lava
                case 6: //shrub
                case 12: //sand
                case 13: //gravel
                case 18: //leaf
                case 110: //wood_float
                    /*case 112:   //lava_fast
                    case Block.WaterDown:
                    case Block.LavaDown:
                    case Block.deathlava:
                    case Block.deathwater:
                    case Block.geyser:
                    case Block.magma:*/
                    AddCheck(b);
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

        private void PhysSandCheck(int b) //also does gravel
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
        private void PhysStair(int b)
        {
            int tempb = IntOffset(b, 0, -1, 0); //Get block below
            if (GetTile(tempb) != Block.Zero)
            {
                if (GetTile(tempb) == Block.staircasestep)
                {
                    AddUpdate(b, 0);
                    AddUpdate(tempb, 43);
                }
            }
        }

        //================================================================================================================
        private bool PhysSpongeCheck(int b, bool lava = false) //return true if sponge is near
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
                            if ((!lava && GetTile(temp) == 19) || (lava && GetTile(temp) == 109))
                            {
                                return true;
                            }
                        }
                    }
                }
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
        private void PhysAirFlood(int b, byte type)
        {
            if (b == -1)
            {
                return;
            }
            if (Block.Convert(blocks[b]) == Block.water || Block.Convert(blocks[b]) == Block.lava) AddUpdate(b, type);
        }

        //================================================================================================================
        private void PhysFall(byte newBlock, ushort x, ushort y, ushort z, bool random)
        {
            var randNum = new Random();
            byte b;
            if (!random)
            {
                b = GetTile((ushort)(x + 1), y, z);
                if (b == Block.air || b == Block.waterstill) Blockchange((ushort)(x + 1), y, z, newBlock);
                b = GetTile((ushort)(x - 1), y, z);
                if (b == Block.air || b == Block.waterstill) Blockchange((ushort)(x - 1), y, z, newBlock);
                b = GetTile(x, y, (ushort)(z + 1));
                if (b == Block.air || b == Block.waterstill) Blockchange(x, y, (ushort)(z + 1), newBlock);
                b = GetTile(x, y, (ushort)(z - 1));
                if (b == Block.air || b == Block.waterstill) Blockchange(x, y, (ushort)(z - 1), newBlock);
            }
            else
            {
                if (GetTile((ushort)(x + 1), y, z) == Block.air && randNum.Next(1, 10) < 3)
                    Blockchange((ushort)(x + 1), y, z, newBlock);
                if (GetTile((ushort)(x - 1), y, z) == Block.air && randNum.Next(1, 10) < 3)
                    Blockchange((ushort)(x - 1), y, z, newBlock);
                if (GetTile(x, y, (ushort)(z + 1)) == Block.air && randNum.Next(1, 10) < 3)
                    Blockchange(x, y, (ushort)(z + 1), newBlock);
                if (GetTile(x, y, (ushort)(z - 1)) == Block.air && randNum.Next(1, 10) < 3)
                    Blockchange(x, y, (ushort)(z - 1), newBlock);
            }
        }

        //================================================================================================================
        private void PhysReplace(int b, byte typeA, byte typeB) //replace any typeA with typeB
        {
            if (b == -1)
            {
                return;
            }
            if (blocks[b] == typeA)
            {
                AddUpdate(b, typeB);
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
        private byte PhysFlowDirections(int b, bool down = true, bool up = false)
        {
            byte dir = 0;
            ushort x, y, z;
            IntToPos(b, out x, out y, out z);

            if (GetTile((ushort)(x + 1), y, z) == Block.air) dir++;
            if (GetTile((ushort)(x - 1), y, z) == Block.air) dir++;
            if (up && GetTile(x, (ushort)(y + 1), z) == Block.air) dir++;
            if (down && GetTile(x, (ushort)(y - 1), z) == Block.air) dir++;
            if (GetTile(x, y, (ushort)(z + 1)) == Block.air) dir++;
            if (GetTile(x, y, (ushort)(z - 1)) == Block.air) dir++;

            return dir;
        }

        //================================================================================================================


        public void odoor(Check C)
        {
            if (C.time == 0)
            {
                byte foundBlock;

                foundBlock = Block.odoor(GetTile(IntOffset(C.b, -1, 0, 0)));
                if (foundBlock == blocks[C.b])
                {
                    AddUpdate(IntOffset(C.b, -1, 0, 0), foundBlock, true);
                }
                foundBlock = Block.odoor(GetTile(IntOffset(C.b, 1, 0, 0)));
                if (foundBlock == blocks[C.b])
                {
                    AddUpdate(IntOffset(C.b, 1, 0, 0), foundBlock, true);
                }
                foundBlock = Block.odoor(GetTile(IntOffset(C.b, 0, -1, 0)));
                if (foundBlock == blocks[C.b])
                {
                    AddUpdate(IntOffset(C.b, 0, -1, 0), foundBlock, true);
                }
                foundBlock = Block.odoor(GetTile(IntOffset(C.b, 0, 1, 0)));
                if (foundBlock == blocks[C.b])
                {
                    AddUpdate(IntOffset(C.b, 0, 1, 0), foundBlock, true);
                }
                foundBlock = Block.odoor(GetTile(IntOffset(C.b, 0, 0, -1)));
                if (foundBlock == blocks[C.b])
                {
                    AddUpdate(IntOffset(C.b, 0, 0, -1), foundBlock, true);
                }
                foundBlock = Block.odoor(GetTile(IntOffset(C.b, 0, 0, 1)));
                if (foundBlock == blocks[C.b])
                {
                    AddUpdate(IntOffset(C.b, 0, 0, 1), foundBlock, true);
                }
            }
            else
            {
                C.time = 255;
            }
            C.time++;
        }

        public void AnyDoor(Check C, ushort x, ushort y, ushort z, int timer, bool instaUpdate = false)
        {
            if (C.time == 0)
            {
                try
                {
                    PhysDoor((ushort)(x + 1), y, z, instaUpdate);
                }
                catch
                {
                }
                try
                {
                    PhysDoor((ushort)(x - 1), y, z, instaUpdate);
                }
                catch
                {
                }
                try
                {
                    PhysDoor(x, y, (ushort)(z + 1), instaUpdate);
                }
                catch
                {
                }
                try
                {
                    PhysDoor(x, y, (ushort)(z - 1), instaUpdate);
                }
                catch
                {
                }
                try
                {
                    PhysDoor(x, (ushort)(y - 1), z, instaUpdate);
                }
                catch
                {
                }
                try
                {
                    PhysDoor(x, (ushort)(y + 1), z, instaUpdate);
                }
                catch
                {
                }

                try
                {
                    if (blocks[C.b] == Block.door8_air)
                    {
                        for (int xx = -1; xx <= 1; xx++)
                        {
                            for (int yy = -1; yy <= 1; yy++)
                            {
                                for (int zz = -1; zz <= 1; zz++)
                                {
                                    byte b = GetTile(IntOffset(C.b, xx, yy, zz));
                                    if (b == Block.rocketstart)
                                    {
                                        if (physics == 5)
                                        {
                                            Blockchange(x, y, z, Block.air);
                                            return;
                                        }
                                        int b1 = IntOffset(C.b, xx * 3, yy * 3, zz * 3);
                                        int b2 = IntOffset(C.b, xx * 2, yy * 2, zz * 2);
                                        bool unblocked = blocks[b1] == Block.air && blocks[b2] == Block.air &&
                                                         !ListUpdate.Exists(Update => Update.b == b1) &&
                                                         !ListUpdate.Exists(Update => Update.b == b2);
                                        if (unblocked)
                                        {
                                            AddUpdate(IntOffset(C.b, xx * 3, yy * 3, zz * 3), Block.rockethead);
                                            AddUpdate(IntOffset(C.b, xx * 2, yy * 2, zz * 2), Block.fire);
                                        }
                                    }
                                    else if (b == Block.firework)
                                    {
                                        if (physics == 5)
                                        {
                                            Blockchange(x, y, z, Block.air);
                                            return;
                                        }
                                        int b1 = IntOffset(C.b, xx, yy + 1, zz);
                                        int b2 = IntOffset(C.b, xx, yy + 2, zz);
                                        bool unblocked = blocks[b1] == Block.air && blocks[b2] == Block.air &&
                                                         !ListUpdate.Exists(Update => Update.b == b1) &&
                                                         !ListUpdate.Exists(Update => Update.b == b2);
                                        if (unblocked)
                                        {
                                            AddUpdate(b2, Block.firework);
                                            AddUpdate(b1, Block.lavastill, false, "dissipate 100");
                                        }
                                    }
                                    else if (b == Block.tnt)
                                    {
                                        if (physics == 5)
                                        {
                                            Blockchange(x, y, z, Block.air);
                                            return;
                                        }
                                        MakeExplosion((ushort)(x + xx), (ushort)(y + yy), (ushort)(z + zz), 0);
                                    }
                                }
                            }
                        }
                    }
                }
                catch
                {
                }
            }
            if (C.time < timer) C.time++;
            else
            {
                AddUpdate(C.b, Block.SaveConvert(blocks[C.b])); //turn back into door
                C.time = 255;
            }
        }

        public void PhysDoor(ushort x, ushort y, ushort z, bool instaUpdate)
        {
            int foundInt = PosToInt(x, y, z);
            byte FoundAir = Block.DoorAirs(blocks[foundInt]);

            if (FoundAir != 0)
            {
                if (!instaUpdate) AddUpdate(foundInt, FoundAir);
                else Blockchange(x, y, z, FoundAir);
                return;
            }

            if (Block.tDoor(blocks[foundInt]))
            {
                AddUpdate(foundInt, Block.air, false, "wait 16 door 1 revert " + blocks[foundInt].ToString());
            }

            if (Block.odoor(blocks[foundInt]) != Block.Zero) AddUpdate(foundInt, Block.odoor(blocks[foundInt]), true);
        }

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

        public void Firework(ushort x, ushort y, ushort z, int size)
        {
            ushort xx, yy, zz;
            var rand = new Random();
            int storedRand1, storedRand2;

            if (physics < 1) return;
            if (physics == 5) return;
            storedRand1 = rand.Next(21, 36);
            storedRand2 = rand.Next(21, 36);
            // Not using override, since override = true makes it more likely that a colored block will be generated with no extraInfo, because it sets a Check for that position with no extraInfo.
            AddUpdate(PosToInt(x, y, z), Block.air);

            for (xx = (ushort)(x - (size + 1)); xx <= (ushort)(x + (size + 1)); ++xx)
                for (yy = (ushort)(y - (size + 1)); yy <= (ushort)(y + (size + 1)); ++yy)
                    for (zz = (ushort)(z - (size + 1)); zz <= (ushort)(z + (size + 1)); ++zz)
                        if (GetTile(xx, yy, zz) == Block.air)
                            if (rand.Next(1, 40) < 2)
                                AddUpdate(PosToInt(xx, yy, zz),
                                          (byte)
                                          rand.Next(Math.Min(storedRand1, storedRand2),
                                                    Math.Max(storedRand1, storedRand2)), false, "drop 100 dissipate 25");
        }

        public void finiteMovement(Check C, ushort x, ushort y, ushort z)
        {
            var rand = new Random();

            var bufferfiniteWater = new List<int>();
            var bufferfiniteWaterList = new List<Pos>();

            if (GetTile(x, (ushort)(y - 1), z) == Block.air)
            {
                AddUpdate(PosToInt(x, (ushort)(y - 1), z), blocks[C.b], false, C.extraInfo);
                AddUpdate(C.b, Block.air);
                C.extraInfo = "";
            }
            else if (GetTile(x, (ushort)(y - 1), z) == Block.waterstill ||
                     GetTile(x, (ushort)(y - 1), z) == Block.lavastill)
            {
                AddUpdate(C.b, Block.air);
                C.extraInfo = "";
            }
            else
            {
                for (int i = 0; i < 25; ++i) bufferfiniteWater.Add(i);

                for (int k = bufferfiniteWater.Count - 1; k > 1; --k)
                {
                    int randIndx = rand.Next(k); //
                    int temp = bufferfiniteWater[k];
                    bufferfiniteWater[k] = bufferfiniteWater[randIndx]; // move random num to end of list.
                    bufferfiniteWater[randIndx] = temp;
                }

                Pos pos;

                for (var xx = (ushort)(x - 2); xx <= x + 2; ++xx)
                {
                    for (var zz = (ushort)(z - 2); zz <= z + 2; ++zz)
                    {
                        pos.x = xx;
                        pos.z = zz;
                        bufferfiniteWaterList.Add(pos);
                    }
                }

                foreach (int i in bufferfiniteWater)
                {
                    pos = bufferfiniteWaterList[i];
                    if (GetTile(pos.x, (ushort)(y - 1), pos.z) == Block.air &&
                        GetTile(pos.x, y, pos.z) == Block.air)
                    {
                        if (pos.x < x) pos.x = (ushort)(Math.Floor((double)(pos.x + x) / 2));
                        else pos.x = (ushort)(Math.Ceiling((double)(pos.x + x) / 2));
                        if (pos.z < z) pos.z = (ushort)(Math.Floor((double)(pos.z + z) / 2));
                        else pos.z = (ushort)(Math.Ceiling((double)(pos.z + z) / 2));

                        if (GetTile(pos.x, y, pos.z) == Block.air)
                        {
                            if (AddUpdate(PosToInt(pos.x, y, pos.z), blocks[C.b], false, C.extraInfo))
                            {
                                AddUpdate(C.b, Block.air);
                                C.extraInfo = "";
                                break;
                            }
                        }
                    }
                }
            }
        }

        public struct Pos
        {
            public ushort x, z;
        }

        #endregion

        #region Nested type: BlockPos

        public struct BlockPos
        {
            public DateTime TimePerformed;
            public bool deleted;
            public string name;
            public byte type;
            public ushort x, y, z;
        }

        #endregion

        #region Nested type: UndoPos

        public struct UndoPos
        {
            public int location;
            public byte newType;
            public byte oldType;
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
