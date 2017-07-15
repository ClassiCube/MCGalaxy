﻿/*
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
using System.Threading;
using MCGalaxy.Blocks;
using MCGalaxy.Blocks.Physics;
using MCGalaxy.DB;
using MCGalaxy.Config;
using MCGalaxy.Games;
using MCGalaxy.Maths;
using MCGalaxy.Network;
using MCGalaxy.Util;

namespace MCGalaxy {
    public sealed partial class Level : IDisposable {
        
        public string name, MapName;
        public string Color { get { return Config.Color; } }
        public string ColoredName { get { return Config.Color + name; } }
        public LevelConfig Config = new LevelConfig();
        
        public byte rotx, roty;
        public ushort spawnx, spawny, spawnz;
        public Position SpawnPos { get { return new Position(16 + spawnx * 32, 32 + spawny * 32, 16 + spawnz * 32); } }
            
        public BlockDefinition[] CustomBlockDefs = new BlockDefinition[Block.Count];
        public BlockProps[] BlockProps = new BlockProps[Block.Count * 2];
        public ExtrasCollection Extras = new ExtrasCollection();
        
        internal HandleDelete[] deleteHandlers = new HandleDelete[Block.Count * 2];
        internal HandlePlace[] placeHandlers = new HandlePlace[Block.Count * 2];
        internal HandleWalkthrough[] walkthroughHandlers = new HandleWalkthrough[Block.Count * 2];
        internal HandlePhysics[] physicsHandlers = new HandlePhysics[Block.Count * 2];
        internal HandlePhysics[] physicsDoorsHandlers = new HandlePhysics[Block.Count * 2];
        internal AABB[] blockAABBs = new AABB[Block.Count * 2];
        
        public ushort Width, Height, Length;
        // NOTE: These are for legacy code only, you should use upper case Width/Height/Length
        // as these correctly map Y to being Height
        [Obsolete] public ushort width;
        [Obsolete] public ushort height;
        [Obsolete] public ushort depth;
        [Obsolete] public ushort length;
        
        public bool IsMuseum {
            get { return name.StartsWith("&cMuseum " + ServerConfig.DefaultColor, StringComparison.Ordinal); }
        }        
        
        public static bool cancelload;
        public bool cancelsave;
        public bool cancelunload;
        public bool changed;
        internal bool saveLevel = true;
        
        /// <summary> Whether this map sees server-wide chat. </summary>
        /// <remarks> true if both worldChat and Server.worldChat are true. </remarks>
        public bool SeesServerWideChat { get { return Config.ServerWideChat && ServerConfig.ServerWideChat; } }
        
        internal readonly object queueLock = new object(), saveLock = new object(), savePropsLock = new object();
        public List<ulong> blockqueue = new List<ulong>();
        BufferedBlockSender bulkSender;

        public List<UndoPos> UndoBuffer = new List<UndoPos>();
        public List<Zone> ZoneList;
        public bool backedup;
        public BlockDB BlockDB;
        public LevelAccessController VisitAccess, BuildAccess;
        
        // Physics fields and settings
        public int physics {
            get { return Physicsint; }
            set {
                if (value > 0 && Physicsint == 0) StartPhysics();
                Physicsint = value;
                Config.Physics = value;
            }
        }
        int Physicsint;
        public bool physicschanged { get { return ListCheck.Count > 0; } }
        public int currentUndo;
        
        public int lastCheck, lastUpdate;
        internal FastList<Check> ListCheck = new FastList<Check>(); //A list of blocks that need to be updated
        internal FastList<Update> ListUpdate = new FastList<Update>(); //A list of block to change after calculation
        internal SparseBitSet listCheckExists, listUpdateExists;
        
        public Random physRandom = new Random();
        public bool physPause;
        public Thread physThread;
        readonly object physThreadLock = new object();
        internal readonly object physStepLock = new object();
        bool physThreadStarted = false;
        
        public List<C4Data> C4list = new List<C4Data>();
        internal readonly Dictionary<int, sbyte> leaves = new Dictionary<int, sbyte>(); // Block state for leaf decay

        public bool CanPlace { get { return Config.Buildable && Config.BuildType != BuildType.NoModify; } }
        public bool CanDelete { get { return Config.Deletable && Config.BuildType != BuildType.NoModify; } }

        public int WinChance {
            get { return Config.RoundsPlayed == 0 ? 100 : (Config.RoundsHumanWon * 100) / Config.RoundsPlayed; }
        }
        
        internal bool hasPortals, hasMessageBlocks;
    }
}