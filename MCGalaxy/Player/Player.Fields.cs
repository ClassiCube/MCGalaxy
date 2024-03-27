﻿/*
Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCForge)
Dual-licensed under the Educational Community License, Version 2.0 and
the GNU General Public License, Version 3 (the "Licenses"); you may
not use this file except in compliance with the Licenses. You may
obtain a copy of the Licenses at
https://opensource.org/license/ecl-2-0/
https://www.gnu.org/licenses/gpl-3.0.html
Unless required by applicable law or agreed to in writing,
software distributed under the Licenses are distributed on an "AS IS"
BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express
or implied. See the Licenses for the specific language governing
permissions and limitations under the Licenses.
*/
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using MCGalaxy.Drawing;
using MCGalaxy.Drawing.Brushes;
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

        public PlayerIgnores Ignores = new PlayerIgnores();
        public static string lastMSG = "";
        internal PersistentMessages persistentMessages = new PersistentMessages();
        public Zone ZoneIn;
        
        //TpA
        internal bool Request;
        internal string senderName = "";
        internal string currentTpa = "";

        /// <summary> Account name of the user </summary>
        /// <remarks> Use 'truename' for displaying/logging, use 'name' for storing data </remarks>
        public string truename;
        /// <summary> The underlying socket for sending/receiving raw data </summary>
        public INetSocket Socket;
        public IGameSession Session;
        
        public DateTime LastAction, AFKCooldown;
        public bool IsAfk, AutoAfk;
        public bool cmdTimer;
        public bool UsingWom;
        public string afkMessage;
        public bool ClickToMark = true;
        
        public string BrushName    = Brush.DefaultBrush;
        public Transform Transform = Transform.DefaultTransform;
        public string DefaultBrushArgs = "";

        /// <summary> Account name of the user, plus a trailing '+' if ClassiCubeAccountPlus is enabled </summary>
        /// <remarks> Use 'truename' for displaying/logging, use 'name' for storing data </remarks>
        public string name;
        public string DisplayName;
        public Pronouns pronouns = Pronouns.Default;
        public int warn;
        public byte id;
        public IPAddress IP;
        public string ip;
        public string color;
        public Group group;
        public LevelPermission hideRank = LevelPermission.Banned;
        public bool hidden;
        public bool painting;
        public bool checkingBotInfo;
        public bool muted;
        public bool jailed;
        public bool agreed = true;
        public bool invincible;
        public string prefix = "";
        public string title = "";
        public string titlecolor = "";
        public int passtries;
        public bool hasreadrules;
        public DateTime NextReviewTime, NextEat, NextTeamInvite;
        public float ReachDistance = 5;
        public bool hackrank;
              
        public string SuperName;
        /// <summary> Whether this player is a 'Super' player (Console, IRC, etc) </summary>
        public readonly bool IsSuper;
        /// <summary> Whether this player is the console player instance. </summary>
        public bool IsConsole { get { return this == Player.Console; } }
        
        public virtual string FullName { get { return color + prefix + DisplayName; } }  
        public string ColoredName { get { return color + DisplayName; } }
        public string GroupPrefix { get { return group.Prefix.Length == 0 ? "" : "&f" + group.Prefix; } }

        public bool deleteMode;
        /// <summary> Whether automatic blockspam detection should be skipped for this player </summary>
        public bool ignoreGrief;
        public bool parseEmotes = Server.Config.ParseEmotes;
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
        public bool possessed;
        
        /// <summary> Whether this player has permission to build in the current level. </summary>
        public bool AllowBuild = true;

        public int money;
        public long TotalModified, TotalDrawn, TotalPlaced, TotalDeleted;
        public int TimesVisited, TimesBeenKicked, TimesDied;
        public int TotalMessagesSent;
        
        long startModified;
        public long SessionModified { get { return TotalModified - startModified; } }
        
        DateTime startTime;
        public TimeSpan TotalTime {
            get { return DateTime.UtcNow - startTime; }
            set { startTime = DateTime.UtcNow.Subtract(value); }
        }
        public DateTime SessionStartTime;
        public DateTime FirstLogin, LastLogin;

        public bool staticCommands;
        internal DateTime lastAccessStatus;
        public VolatileArray<SchedulerTask> CriticalTasks;

        public bool isFlying;
        public bool aiming;
        public Weapon weapon;
        internal BufferedBlockSender weaponBuffer;

        public bool joker;
        public bool Unverified, verifiedPass;
        /// <summary> Whether this player can speak even while chat moderation is on </summary>
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
        public bool flipHead, infected;
        public GameProps Game = new GameProps();     
        /// <summary> Persistent ID of this user in the Players table. </summary>
        public int DatabaseID;

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
        internal int gbStep, lbStep;
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

        public DateTime deathCooldown;

        public BlockID ModeBlock = Block.Invalid;
        /// <summary> The block ID this player's client specifies it is currently holding in hand. </summary>
        /// <remarks> This ignores /bind and /mode. GetHeldBlock() is usually preferred. </remarks>
        public BlockID ClientHeldBlock = Block.Stone;
        public BlockID[] BlockBindings = new BlockID[Block.SUPPORTED_COUNT];
        public Dictionary<string, string> CmdBindings = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        
        public string lastCMD = "";
        public DateTime lastCmdTime;
        public sbyte c4circuitNumber = -1;

        public Level level;
        public bool Loading = true; //True if player is loading a map.
        internal int UsingGoto, GeneratingMap, LoadingMuseum;
        public Vec3U16 lastClick = Vec3U16.Zero;
        
        public Position PreTeleportPos;
        public Orientation PreTeleportRot;
        public string PreTeleportMap;
        
        public string summonedMap;
        public Position _tempPos;

        // Extra storage for custom commands
        public ExtrasCollection Extras = new ExtrasCollection();
        
        SpamChecker spamChecker;
        internal DateTime cmdUnblocked;
        List<DateTime> partialLog;

        public DateTime LastPatrol;
        public LevelPermission Rank { get { return group.Permission; } }

        /// <summary> Whether player has completed login process and has been sent initial map. </summary>
        public bool loggedIn;
        public bool verifiedName;
        /// <summary> The URL of the authentication service this player's name was verified via. Can be null </summary>
        /// <example> http://www.classicube.net/heartbeat.jsp </example>
        public string VerifiedVia;
        bool gotSQLData;
        
        
        public bool cancelcommand, cancelchat;
        public bool cancellogin, cancelconnecting;
        
        Queue<SerialCommand> serialCmds = new Queue<SerialCommand>();
        object serialCmdsLock = new object();
        struct SerialCommand { public Command cmd; public string args; public CommandData data; }
      
        /// <summary> Called when a player removes or places a block.
        /// NOTE: Currently this prevents the OnBlockChange event from being called. </summary>
        public event SelectionBlockChange Blockchange;
        
        public void ClearBlockchange() { ClearSelection(); }
        public object blockchangeObject;
        
        /// <summary> Called when the player has finished providing all the marks for a selection. </summary>
        /// <returns> Whether to repeat this selection, if /static mode is enabled. </returns>
        public delegate bool SelectionHandler(Player p, Vec3S32[] marks, object state, BlockID block);
        
        /// <summary> Called when the player has provided a mark for a selection. </summary>
        /// <remarks> i is the index of the mark, so the 'first' mark has 0 for i. </remarks>
        public delegate void SelectionMarkHandler(Player p, Vec3S32[] marks, int i, object state, BlockID block);
    }
}
