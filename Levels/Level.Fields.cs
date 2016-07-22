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


namespace MCGalaxy {
    public sealed partial class Level : IDisposable {
		
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
        [ConfigString("RealmOwner", "Other", null, "", true)]
        public string RealmOwner = "";
        
        [ConfigInt("Weather", "Env", null, 0, 0, 2)]        
        public int Weather;
        [ConfigString("Texture", "Env", null, "", true)]
        public string terrainUrl = "";
        [ConfigString("TexturePack", "Env", null, "", true)]
        public string texturePackUrl = "";
        
        public bool cancelsave1;
        public bool cancelunload;
        public bool changed;
        public bool physicschanged { get { return ListCheck.Count > 0; } }
        internal bool saveLevel = true;
        
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
        [ConfigString("CloudColor", "Env", null, "", true)]
        public string CloudColor = "";

        /// <summary> Color of the fog (RGB packed into an int). Set to -1 to use client defaults. </summary>
        [ConfigString("FogColor", "Env", null, "", true)] 
        public string FogColor = "";

        /// <summary> Color of the sky (RGB packed into an int). Set to -1 to use client defaults. </summary>
        [ConfigString("SkyColor", "Env", null, "", true)]
        public string SkyColor = "";

        /// <summary> Color of the blocks in shadows (RGB packed into an int). Set to -1 to use client defaults. </summary>
        [ConfigString("ShadowColor", "Env", null, "", true)] 
        public string ShadowColor = "";

        /// <summary> Color of the blocks in the light (RGB packed into an int). Set to -1 to use client defaults. </summary>
        [ConfigString("LightColor", "Env", null, "", true)]
        public string LightColor = "";

        /// <summary> Elevation of the "ocean" that surrounds maps. Default is map height / 2. </summary>
        [ConfigInt("EdgeLevel", "Env", null, -1, short.MinValue, short.MaxValue)]
        public int EdgeLevel;
        
        /// <summary> Elevation of the clouds. Default is map height + 2. </summary>
        [ConfigInt("CloudsHeight", "Env", null, -1, short.MinValue, short.MaxValue)]
        public int CloudsHeight;
        
        /// <summary> Max fog distance the client can see. 
        /// Default is 0, meaning use the client-side defined maximum fog distance. </summary>
        [ConfigInt("MaxFog", "Env", null, 0, short.MinValue, short.MaxValue)]
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
        [ConfigInt("HorizonBlock", "Env", null, Block.water, 0, 255)]
        public int HorizonBlock = Block.water;

        /// <summary> The block which will be displayed on the edge of the map. </summary>
        [ConfigInt("EdgeBlock", "Env", null, Block.blackrock, 0, 255)]
        public int EdgeBlock = Block.blackrock;
        
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

        public int physics {
            get { return Physicsint; }
            set {
                if (value > 0 && Physicsint == 0)
                    StartPhysics();
                Physicsint = value;
            }
        }
        int Physicsint;
        
        [ConfigBool("RandomFlow", "General", null, true)]        
        public bool randomFlow = true;
        public byte rotx, roty;
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
        internal readonly object queueLock = new object(), saveLock = new object(), savePropsLock = new object();
        public List<ulong> blockqueue = new List<ulong>();
        readonly object physThreadLock = new object();
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
        
        [ConfigEnum("BuildType", "Game", null, BuildType.Normal, typeof(BuildType))]
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
    }
}