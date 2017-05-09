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
using MCGalaxy.Drawing.Transforms;
using MCGalaxy.Games;
using MCGalaxy.Undo;

namespace MCGalaxy {
    
    public enum VoteKickChoice { NotYetVoted, Yes, No }
    
    public sealed partial class Player : IDisposable {
        
        public Dictionary<string, object> ExtraData = new Dictionary<string, object>();

        public void ClearChat() { OnChat = null; }
        
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
        public static List<Player> connections = new List<Player>(Server.players);
        public List<string> listignored = new List<string>();
        public static byte number { get { return (byte)PlayerInfo.Online.Count; } }
        public static string lastMSG = "";
        
        //TpA
        internal bool Request = false;
        internal string senderName = "";
        internal string currentTpa = "";

        public static bool storeHelp = false;
        public static string storedHelp = "";
        public string truename;
        internal bool dontmindme = false;
        public Socket socket;
        public DateTime LastAction, AFKCooldown;
        public bool IsAfk = false, AutoAfk;
        public bool cmdTimer = false;
        public bool UsingWom = false;
        public string BrushName = "normal", DefaultBrushArgs = "";
        public Transform Transform = NoTransform.Instance;
        public string afkMessage;

        byte[] leftBuffer = new byte[0];
        byte[] tempbuffer = new byte[0xFF];
        public bool disconnected = false;
        
        DateTime startTime;
        public TimeSpan time {
            get { return DateTime.UtcNow - startTime; }
            set { startTime = DateTime.UtcNow.Subtract(value); }
        }

        public string name;
        public string DisplayName;
        public string realName;
        public int warn = 0;
        public byte id;
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
        internal string ircChannel, ircNick;
        
        public string FullName { get { return color + prefix + DisplayName; } }
        
        public string ColoredName { get { return color + DisplayName; } }

        public bool deleteMode = false;
        public bool ignorePermission = false;
        public bool ignoreGrief = false;
        public bool parseEmotes = Server.parseSmiley;
        public bool opchat = false;
        public bool adminchat = false;
        public bool onWhitelist = false;
        public bool whisper = false;
        public string whisperTo = "";
        public bool ignoreAll, ignoreGlobal, ignoreIRC, ignoreTitles, ignoreNicks, ignore8ball;

        public string storedMessage = "";

        public bool trainGrab = false;
        public bool onTrain = false, trainInvincible = false;
        public bool allowTnt = true;

        public bool frozen = false;
        public string following = "";
        public string possess = "";

        // Only used for possession.
        //Using for anything else can cause unintended effects!
        public bool canBuild = true;
        /// <summary> Whether the player has build permission in the current world. </summary>
        public bool AllowBuild = true;

        public int money, loginMoney;
        public long overallBlocks, TotalDrawn, TotalPlaced, TotalDeleted;
        public int loginBlocks;

        public DateTime timeLogged;
        public DateTime firstLogin, lastLogin;
        public int totalLogins;
        public int totalKicked;
        public int totalBanned;
        public int overallDeath;

        public bool staticCommands = false;
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
        
        /// <summary> Persistent ID of this user in the Players table. </summary>
        public int UserID;
        public const int SessionIDMask = (1 << 20) - 1;
        /// <summary> Temp unique ID for this session only. </summary>
        public int SessionID;

        //Countdown
        public bool playerofcountdown = false;
        public bool incountdown = false;
        public int countdowntempx;
        public int countdowntempz;
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
        
        // BlockDefinitions
        internal int gbStep = 0, lbStep = 0;
        internal BlockDefinition gbBlock, lbBlock;
        public bool spawned = false;

        public bool Mojangaccount { get { return truename.IndexOf('@') >= 0; } }

        //Undo
        internal VolatileArray<UndoDrawOpEntry> DrawOps = new VolatileArray<UndoDrawOpEntry>(false);
        internal readonly object pendingDrawOpsLock = new object();
        internal List<PendingDrawOp> PendingDrawOps = new List<PendingDrawOp>();

        public bool showPortals = false, showMBs = false;
        public string prevMsg = "";
        internal bool showedWelcome = false;

        //Movement
        public int oldIndex = -1, lastWalkthrough = -1, oldFallY = 10000;
        public int fallCount = 0, drownCount = 0;

        //Games
        public DateTime lastDeath = DateTime.UtcNow;

        public byte modeType, RawHeldBlock = Block.rock;
        public byte[] bindings = new byte[Block.CpeCount];
        public string[] cmdBind = new string[10];
        public string[] messageBind = new string[10];
        public string lastCMD = "";
        public DateTime lastCmdTime;
        public sbyte c4circuitNumber = -1;

        public Level level = Server.mainLevel;
        public bool Loading = true; //True if player is loading a map.
        internal int UsingGoto = 0, GeneratingMap = 0, LoadingMuseum = 0;
        public Vec3U16 lastClick = Vec3U16.Zero;
        public Position beforeTeleportPos = default(Position);
        public string beforeTeleportMap = "";
        public ushort[] pos = new ushort[3];        
        public byte[] rot = new byte[2];
        internal Position tempPos;

        //ushort[] clippos = new ushort[3] { 0, 0, 0 };
        //byte[] cliprot = new byte[2] { 0, 0 };
        public int consecutivemessages;

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
        
        /// <summary> Returns whether the given player is console or IRC. </summary>
        public static bool IsSuper(Player p) { return p == null || p.ircChannel != null || p.ircNick != null; }
        
        public void SetMoney(int amount) {
            money = amount;
            OnMoneyChanged();
        }
    }
}
