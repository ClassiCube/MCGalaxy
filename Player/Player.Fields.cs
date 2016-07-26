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
using System.Net.Sockets;
using System.Security.Cryptography;
using MCGalaxy.Drawing;
using MCGalaxy.Games;
using MCGalaxy.Util;

namespace MCGalaxy {
    
    public sealed partial class Player : IDisposable {
        
        /// <summary>
        /// Key - Name  Value - IP
        /// All players who have left this restart.
        /// </summary>
        public Dictionary<string, object> ExtraData = new Dictionary<string, object>();

        public void ClearChat() { OnChat = null; }
        public static Dictionary<string, string> left = new Dictionary<string, string>();
        
        class PendingItem {
            public string Name;
            public DateTime Connected;
            
            public PendingItem(string name) {
                Name = name;
                Connected = DateTime.UtcNow;
            }
        }
        static List<PendingItem> pendingNames = new List<PendingItem>();
        static object pendingLock = new object();
        
        public static List<Player> connections = new List<Player>(Server.players);
        System.Timers.Timer muteTimer = new System.Timers.Timer(1000);
        public List<string> listignored = new List<string>();
        public static byte number { get { return (byte)PlayerInfo.Online.Count; } }
        
        static System.Text.ASCIIEncoding enc = new System.Text.ASCIIEncoding();
        static MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
        static object md5Lock = new object();
        public static string lastMSG = "";
        
        //TpA
        internal bool Request = false;
        internal string senderName = "";
        internal string currentTpa = "";

        public static bool storeHelp = false;
        public static string storedHelp = "";
        public string truename, skinName;
        internal bool dontmindme = false;
        public Socket socket;
        System.Timers.Timer loginTimer = new System.Timers.Timer(1000);
        System.Timers.Timer extraTimer = new System.Timers.Timer(22000);
        public System.Timers.Timer checkTimer = new System.Timers.Timer(2000);
        public DateTime LastAction;
        public bool IsAfk = false, AutoAfk;
        public bool cmdTimer = false;
        public bool UsingWom = false;
        public string BrushName = "normal", DefaultBrushArgs = "";
        public string afkMessage;

        byte[] buffer = new byte[0];
        byte[] tempbuffer = new byte[0xFF];
        public bool disconnected = false;
        
        DateTime startTime;
        public TimeSpan time {
            get { return DateTime.UtcNow - startTime; }
            set { startTime = DateTime.UtcNow.Add(-value); }
        }

        public string name;
        public string DisplayName;
        public string realName;
        public int warn = 0;
        public byte id;
        public int userID = -1;
        public string ip;
        public string color;
        public Group group;
        public LevelPermission oHideRank = LevelPermission.Null;
        public bool otherRankHidden = false;
        public bool hidden = false;
        public bool painting = false;
        public bool muted = false;
        public bool jailed = false;
        public bool agreed = true;
        public bool invincible = false;
        public string prefix = "";
        public string title = "";
        public string titlecolor;
        public int TotalMessagesSent = 0;
        public int passtries = 0;
        public int ponycount = 0;
        public int rdcount = 0;
        public bool hasreadrules = false;
        public DateTime NextReviewTime, NextEat;
        public float ReachDistance = 5;
        public bool hackrank;
        internal string ircNick;
        
        public string FullName { get { return color + prefix + DisplayName; } }
        
        public string ColoredName { get { return color + DisplayName; } }

        //Gc checks
        public string lastmsg = "";
        public int spamcount = 0, capscount = 0, floodcount = 0, multi = 0;
        public DateTime lastmsgtime = DateTime.MinValue;
        public bool canusegc = true;

        public bool deleteMode = false;
        public bool ignorePermission = false;
        public bool ignoreGrief = false;
        public bool parseEmotes = true;
        public bool opchat = false;
        public bool adminchat = false;
        public bool onWhitelist = false;
        public bool whisper = false;
        public string whisperTo = "";
        public bool ignoreAll, ignoreGlobal, ignoreIRC, ignoreTitles, ignoreNicks;

        public string storedMessage = "";

        public bool trainGrab = false;
        public bool onTrain = false;
        public bool allowTnt = true;

        public bool frozen = false;
        public string following = "";
        public string possess = "";

        // Only used for possession.
        //Using for anything else can cause unintended effects!
        public bool canBuild = true;

        public int money = 0, loginMoney = 0;
        public long overallBlocks = 0;

        public int loginBlocks = 0;

        public DateTime timeLogged;
        public DateTime firstLogin, lastLogin;
        public int totalLogins = 0;
        public int totalKicked = 0;
        public int overallDeath = 0;

        public bool staticCommands = false;
        internal bool outdatedClient = false; // for ClassicalSharp 0.98.5, which didn't reload map for BlockDefinitions

        public DateTime ZoneSpam;

        public bool aiming;
        public bool isFlying = false;

        public bool joker = false;
        public bool adminpen = false;

        public bool voice = false;
        public string voicestring = "";

        public bool useCheckpointSpawn = false;
        public int lastCheckpointIndex = -1;
        public ushort checkpointX, checkpointY, checkpointZ;
        public byte checkpointRotX, checkpointRotY;
        public bool voted = false;
        public bool flipHead = false;
        public GameProps Game = new GameProps();
        public const int SessionIDMask = (1 << 23) - 1;
        public int SessionID;

        //Countdown
        public bool playerofcountdown = false;
        public bool incountdown = false;
        public ushort countdowntempx;
        public ushort countdowntempz;
        public bool countdownsettemps = false;

        //Tnt Wars
        public bool PlayingTntWars = false;
        public int CurrentAmountOfTnt = 0;
        public int CurrentTntGameNumber; //For keeping track of which game is which
        public int TntWarsHealth = 2;
        public int TntWarsKillStreak = 0;
        public float TntWarsScoreMultiplier = 1f;
        public int TNTWarsLastKillStreakAnnounced = 0;
        public bool inTNTwarsMap = false;
        public Player HarmedBy = null; //For Assists

        public CopyState CopyBuffer;
        
        // GlobalBlock
        internal int gbStep = 0, gbTargetId = 0;
        internal BlockDefinition gbBlock;
        internal int lbStep = 0, lbTargetId = 0;
        internal BlockDefinition lbBlock;
        
        public string model = "humanoid";
        public bool spawned = false;

        public bool Mojangaccount { get { return truename.IndexOf('@') >= 0; } }

        //Undo
        public struct UndoPos { public ushort x, y, z; public byte type, extType, newtype, newExtType; public string mapName; public int timeDelta; }
        public UndoCache UndoBuffer = new UndoCache();
        internal VolatileArray<UndoDrawOpEntry> DrawOps = new VolatileArray<UndoDrawOpEntry>(false);
        internal readonly object pendingDrawOpsLock = new object();
        internal List<PendingDrawOp> PendingDrawOps = new List<PendingDrawOp>();

        public bool showPortals = false;
        public bool showMBs = false;

        public string prevMsg = "";

        //Movement
        public int oldIndex = -1, lastWalkthrough = -1, oldFallY = 10000;
        public int fallCount = 0, drownCount = 0;

        //Games
        public DateTime lastDeath = DateTime.UtcNow;

        public byte modeType;
        public byte[] bindings = new byte[128];
        public string[] cmdBind = new string[10];
        public string[] messageBind = new string[10];
        public string lastCMD = "";
        public DateTime lastCmdTime;
        public sbyte c4circuitNumber = -1;

        public Level level = Server.mainLevel;
        public bool Loading = true; //True if player is loading a map.
        internal bool usingGoto = false;
        public Vec3U16 lastClick = Vec3U16.Zero;
        public ushort[] beforeTeleportPos = new ushort[3];
        public string beforeTeleportMap = "";
        public ushort[] pos = new ushort[3];        
        public byte[] rot = new byte[2];
        internal ushort[] oldpos = new ushort[3], tempPos = new ushort[3];
        internal byte[] oldrot = new byte[2];

        //ushort[] clippos = new ushort[3] { 0, 0, 0 };
        //byte[] cliprot = new byte[2] { 0, 0 };

        // grief/spam detection
        public static int spamBlockCount = 200;
        public static int spamBlockTimer = 5;
        Queue<DateTime> spamBlockLog = new Queue<DateTime>(spamBlockCount);

        public int consecutivemessages;
        //public static int spamChatCount = 3;
        //public static int spamChatTimer = 4;
        //Queue<DateTime> spamChatLog = new Queue<DateTime>(spamChatCount);

        // CmdVoteKick
        public VoteKickChoice voteKickChoice = VoteKickChoice.HasntVoted;

        // Extra storage for custom commands
        public ExtrasCollection Extras = new ExtrasCollection();

        //Chatrooms
        public string Chatroom;
        public List<string> spyChatRooms = new List<string>();
        public DateTime lastchatroomglobal;

        public WarpList Waypoints = new WarpList(true);

        public Random random = new Random();
        public LevelPermission Rank { get { return group.Permission; } }

        //Global Chat
        public bool loggedIn;
        public bool isDev, isMod;
        public bool verifiedName;
        
        public static bool IsSuper(Player p) { return p == null || p.ircNick != null; }
    }
}
