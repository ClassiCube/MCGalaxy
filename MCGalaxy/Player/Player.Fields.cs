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

namespace MCGalaxy {
    
    public enum VoteKickChoice { NotYetVoted, Yes, No }
    
    public partial class Player : IDisposable {

        internal class PendingItem {
            public string Name;
            public DateTime Connected;
            
            public PendingItem(string name) {
                Name = name;
                Connected = DateTime.UtcNow;
            }
        }
        internal static List<PendingItem> pendingNames = new List<PendingItem>();
        internal static object pendingLock = new object();        
        public static List<Player> connections = new List<Player>(ServerConfig.MaxPlayers);
        public List<string> listignored = new List<string>();
        public static string lastMSG = "";
        
        //TpA
        internal bool Request;
        internal string senderName = "";
        internal string currentTpa = "";

        public string truename;
        internal bool nonPlayerClient = false;
        public INetworkSocket Socket;
        public PingList Ping = new PingList();
        
        public DateTime LastAction, AFKCooldown;
        public bool IsAfk, AutoAfk;
        public bool cmdTimer;
        public bool UsingWom;
        public string BrushName = "normal", DefaultBrushArgs = "";
        public Transform Transform = NoTransform.Instance;
        public string afkMessage;
        public bool disconnected;

        public string name;
        public string DisplayName;
        public int warn = 0;
        public byte id;
        public string ip;
        public string color;
        public Group group;
        public LevelPermission oHideRank = LevelPermission.Null;
        public bool otherRankHidden;
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
        public DateTime NextReviewTime, NextEat;
        public float ReachDistance = 5;
        public bool hackrank;
        public bool SuperUser;
        
        public string FullName { get { return color + prefix + DisplayName; } }
        
        public string ColoredName { get { return color + DisplayName; } }

        public bool deleteMode;
        public bool ignorePermission;
        public bool ignoreGrief;
        public bool parseEmotes = ServerConfig.ParseEmotes;
        public bool opchat;
        public bool adminchat;
        public bool onWhitelist;
        public bool whisper;
        public string whisperTo = "";
        public bool ignoreAll, ignoreGlobal, ignoreIRC, ignoreTitles, ignoreNicks, ignore8ball, ignoreDrawOutput;

        string partialMessage = "";

        public bool trainGrab;
        public bool onTrain, trainInvincible;

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
        public DateTime ZoneSpam;
        public VolatileArray<SchedulerTask> CriticalTasks = new VolatileArray<SchedulerTask>();

        public bool aiming;
        public bool isFlying;

        public bool joker;
        public bool adminpen;
        public bool voice;

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

        //Countdown
        public int CountdownFreezeX;
        public int CountdownFreezeZ;

        //Tnt Wars
        public bool PlayingTntWars;
        public int CurrentAmountOfTnt = 0;
        public int CurrentTntGameNumber; //For keeping track of which game is which
        public int TntWarsHealth = 2;
        public int TntWarsKillStreak = 0;
        public float TntWarsScoreMultiplier = 1f;
        public int TNTWarsLastKillStreakAnnounced = 0;
        public bool inTNTwarsMap;
        public Player HarmedBy = null; //For Assists

        public CopyState CopyBuffer;
        
        // BlockDefinitions
        internal int gbStep = 0, lbStep = 0;
        internal BlockDefinition gbBlock, lbBlock;
        public bool spawned;

        //Undo
        internal VolatileArray<UndoDrawOpEntry> DrawOps = new VolatileArray<UndoDrawOpEntry>();
        internal readonly object pendingDrawOpsLock = new object();
        internal List<PendingDrawOp> PendingDrawOps = new List<PendingDrawOp>();

        public bool showPortals, showMBs;
        public string prevMsg = "";
        internal bool showedWelcome;

        //Movement
        internal int oldIndex = -1, lastWalkthrough = -1, startFallY = -1, lastFallY = -1;
        public DateTime drownTime = DateTime.MaxValue;

        //Games
        public DateTime lastDeath = DateTime.UtcNow;

        public ExtBlock ModeBlock;
        public ExtBlock RawHeldBlock = (ExtBlock)Block.Stone;
        public ExtBlock[] BlockBindings = new ExtBlock[Block.Count];        
        public string[] CmdBindings = new string[10];
        public string[] CmdArgsBindings = new string[10];
        
        public string lastCMD = "";
        public DateTime lastCmdTime;
        public sbyte c4circuitNumber = -1;

        public Level level = Server.mainLevel;
        public bool Loading = true; //True if player is loading a map.
        internal int UsingGoto = 0, GeneratingMap = 0, LoadingMuseum = 0, UsingDelay = 0;
        public Vec3U16 lastClick = Vec3U16.Zero;
        public Position beforeTeleportPos = default(Position);
        public string lastTeleportMap = "";
        public string summonedMap;
        public ushort[] pos = new ushort[3];        
        public byte[] rot = new byte[2];
        internal Position tempPos;

        // CmdVoteKick
        public VoteKickChoice voteKickChoice = VoteKickChoice.NotYetVoted;

        // Extra storage for custom commands
        public ExtrasCollection Extras = new ExtrasCollection();
        
        SpamChecker spamChecker;
        internal DateTime cmdUnblocked;

        //Chatrooms
        public string Chatroom;
        public List<string> spyChatRooms = new List<string>();
        public DateTime lastchatroomglobal;

        public WarpList Waypoints = new WarpList(true);

        public Random random = new Random();
        public LevelPermission Rank { get { return group.Permission; } }

        public bool loggedIn;
        public bool isDev, isMod;
        public bool verifiedName;
        bool gotSQLData;
        
        /// <summary> Returns whether the given player is console or IRC. </summary>
        public static bool IsSuper(Player p) { return p == null || p.SuperUser; }
        
        
        public bool cancelcommand, cancelchat, cancelmove, cancelBlock, cancelmysql;
        public bool cancelmessage, cancellogin, cancelconnecting;        
      
        /// <summary> Called when a player removes or places a block.
        /// NOTE: Currently this prevents the OnBlockChange event from being called. </summary>
        public event SelectionBlockChange Blockchange;
        
        internal bool HasBlockchange { get { return Blockchange != null; } }
        public void ClearBlockchange() { ClearSelection(); }
        public object blockchangeObject;
        
        /// <summary> Called when the player has finished providing all the marks for a selection. </summary>
        /// <returns> Whether to repeat this selection, if /static mode is enabled. </returns>
        public delegate bool SelectionHandler(Player p, Vec3S32[] marks, object state, ExtBlock block);
        
        /// <summary> Called when the player has provided a mark for a selection. </summary>
        /// <remarks> i is the index of the mark, so the 'first' mark has i of 0. </remarks>
        public delegate void SelectionMarkHandler(Player p, Vec3S32[] marks, int i, object state, ExtBlock block);
    }
}
