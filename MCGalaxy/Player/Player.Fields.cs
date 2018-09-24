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
using MCGalaxy.Drawing;
using MCGalaxy.Drawing.Transforms;
using MCGalaxy.Events.PlayerEvents;
using MCGalaxy.Games;
using MCGalaxy.Maths;
using MCGalaxy.Network;
using MCGalaxy.Tasks;
using MCGalaxy.Undo;
using BlockID = System.UInt16;

namespace MCGalaxy {
    
    public partial class Player : IDisposable {

        public static VolatileArray<Player> pending = new VolatileArray<Player>(false);
        public PlayerIgnores Ignores = new PlayerIgnores();
        public static string lastMSG = "";
        public Zone ZoneIn;
        
        //TpA
        internal bool Request;
        internal string senderName = "";
        internal string currentTpa = "";

        public string truename;
        internal bool nonPlayerClient = false;
        public INetworkSocket Socket;
        public PingList Ping = new PingList();
        public BlockID MaxRawBlock = Block.OriginalMaxBlock;
        
        public DateTime LastAction, AFKCooldown;
        public bool IsAfk, AutoAfk;
        public bool cmdTimer;
        public bool UsingWom;
        public string BrushName = "Normal", DefaultBrushArgs = "";
        public Transform Transform = NoTransform.Instance;
        public string afkMessage;
        public bool disconnected, ClickToMark = true;

        public string name;
        public string DisplayName;
        public int warn = 0;
        public byte id;
        public string ip;
        public string color;
        public Group group;
        public LevelPermission hideRank = LevelPermission.Banned;
        public bool hidden;
        public bool painting;
        public bool muted;
        public bool jailed;
        public bool agreed = true;
        public bool invincible;
        public string prefix = "";
        public string title = "";
        public string titlecolor = "";
        public int passtries = 0;
        public bool hasreadrules;
        public DateTime NextReviewTime, NextEat, NextTeamInvite;
        public float ReachDistance = 5;
        public bool hackrank;
              
        public string SuperName;
        public bool IsSuper;
        public bool IsConsole { get { return this == Player.Console; } }
        
        public virtual string FullName { get { return color + prefix + DisplayName; } }  
        public string ColoredName { get { return color + DisplayName; } }
        public string GroupPrefix { get { return group.Prefix.Length == 0 ? "" : "&f" + group.Prefix; } }

        public bool deleteMode;
        public bool ignoreGrief;
        public bool parseEmotes = ServerConfig.ParseEmotes;
        public bool opchat;
        public bool adminchat;
        public bool whisper;
        public string whisperTo = "";
        string partialMessage = "";

        public bool trainGrab;
        public bool onTrain, trainInvincible;
        int mbRecursion;

        public bool frozen;
        public string following = "";
        public string possess = "";

        // Only used for possession.
        //Using for anything else can cause unintended effects!
        public bool canBuild = true;
        /// <summary> Whether the player has build permission in the current world. </summary>
        public bool AllowBuild = true;

        public int money;
        public long TotalModified, TotalDrawn, TotalPlaced, TotalDeleted;
        public int SessionModified;
        public int TimesVisited, TimesBeenKicked, TimesDied;
        public int TotalMessagesSent; // TODO: implement this
        
        DateTime startTime;
        public TimeSpan TotalTime {
            get { return DateTime.UtcNow - startTime; }
            set { startTime = DateTime.UtcNow.Subtract(value); }
        }
        public DateTime SessionStartTime;
        public DateTime FirstLogin, LastLogin;

        public bool staticCommands;
        internal DateTime lastAccessStatus;
        public VolatileArray<SchedulerTask> CriticalTasks = new VolatileArray<SchedulerTask>();

        public bool aiming;
        public bool isFlying;

        public bool joker;
        public bool adminpen;
        public bool voice;
        
        public CommandData DefaultCmdData {
            get { 
                CommandData data = default(CommandData);
                data.Rank = Rank; return data;
            }
        }

        public bool useCheckpointSpawn;
        public int lastCheckpointIndex = -1;
        public ushort checkpointX, checkpointY, checkpointZ;
        public byte checkpointRotX, checkpointRotY;
        public bool voted;
        public bool flipHead;
        public GameProps Game = new GameProps();
        
        /// <summary> Persistent ID of this user in the Players table. </summary>
        public int DatabaseID;
        public const int SessionIDMask = (1 << 20) - 1;
        /// <summary> Temp unique ID for this session only. </summary>
        public int SessionID;

        public List<CopyState> CopySlots = new List<CopyState>();
        public int CurrentCopySlot;
        public CopyState CurrentCopy { 
            get { return CurrentCopySlot >= CopySlots.Count ? null : CopySlots[CurrentCopySlot]; }
            set {
                while (CurrentCopySlot >= CopySlots.Count) { CopySlots.Add(null); }
                CopySlots[CurrentCopySlot] = value;
            }
        }
        
        // BlockDefinitions
        internal int gbStep = 0, lbStep = 0;
        internal BlockDefinition gbBlock, lbBlock;

        //Undo
        public VolatileArray<UndoDrawOpEntry> DrawOps = new VolatileArray<UndoDrawOpEntry>();
        internal readonly object pendingDrawOpsLock = new object();
        internal List<PendingDrawOp> PendingDrawOps = new List<PendingDrawOp>();

        public bool showPortals, showMBs;
        public string prevMsg = "";

        //Movement
        internal int oldIndex = -1, lastWalkthrough = -1, startFallY = -1, lastFallY = -1;
        public DateTime drownTime = DateTime.MaxValue;

        //Games
        public DateTime lastDeath = DateTime.UtcNow;

        public BlockID ModeBlock;
        public BlockID RawHeldBlock = Block.Stone;
        public BlockID[] BlockBindings = new BlockID[Block.ExtendedCount];        
        public string[] CmdBindings = new string[10];
        public string[] CmdArgsBindings = new string[10];
        
        public string lastCMD = "";
        public DateTime lastCmdTime;
        public sbyte c4circuitNumber = -1;

        public Level level;
        public bool Loading = true; //True if player is loading a map.
        internal int UsingGoto = 0, GeneratingMap = 0, LoadingMuseum = 0, UsingDelay = 0;
        public Vec3U16 lastClick = Vec3U16.Zero;
        
        public Position PreTeleportPos;
        public Orientation PreTeleportRot;
        public string PreTeleportMap;
        
        public string summonedMap;
        internal Position tempPos;

        // Extra storage for custom commands
        public ExtrasCollection Extras = new ExtrasCollection();
        
        SpamChecker spamChecker;
        internal DateTime cmdUnblocked;

        //Chatrooms
        public string Chatroom;
        public List<string> spyChatRooms = new List<string>();
        public DateTime lastchatroomglobal;

        public WarpList Waypoints = new WarpList();
        public DateTime LastPatrol;
        public LevelPermission Rank { get { return group.Permission; } }

        public bool loggedIn;
        public bool verifiedName;
        bool gotSQLData;
        
        
        public bool cancelcommand, cancelchat, cancelmove, cancelBlock, cancelmysql;
        public bool cancelmessage, cancellogin, cancelconnecting, cancelDeath;     
      
        /// <summary> Called when a player removes or places a block.
        /// NOTE: Currently this prevents the OnBlockChange event from being called. </summary>
        public event SelectionBlockChange Blockchange;
        
        public void ClearBlockchange() { ClearSelection(); }
        public object blockchangeObject;
        
        /// <summary> Called when the player has finished providing all the marks for a selection. </summary>
        /// <returns> Whether to repeat this selection, if /static mode is enabled. </returns>
        public delegate bool SelectionHandler(Player p, Vec3S32[] marks, object state, BlockID block);
        
        /// <summary> Called when the player has provided a mark for a selection. </summary>
        /// <remarks> i is the index of the mark, so the 'first' mark has i of 0. </remarks>
        public delegate void SelectionMarkHandler(Player p, Vec3S32[] marks, int i, object state, BlockID block);
    }
}
