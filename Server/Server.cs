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
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Forms;
using MCGalaxy.Config;
using MCGalaxy.Games;
using MCGalaxy.SQL;
using MonoTorrent.Client;
using Newtonsoft.Json;

namespace MCGalaxy
{
    public sealed partial class Server
    {
        public static bool cancelcommand = false;
        public static bool canceladmin = false;
        public static bool cancellog = false;
        public static bool canceloplog = false;
        public static string apppath = Application.StartupPath;
        public delegate void OnConsoleCommand(string cmd, string message);
        public static event OnConsoleCommand ConsoleCommand;
        public delegate void OnServerError(Exception error);
        public static event OnServerError ServerError = null;
        public delegate void OnServerLog(string message);
        public static event OnServerLog ServerLog;
        public static event OnServerLog ServerAdminLog;
        public static event OnServerLog ServerOpLog;
        public delegate void HeartBeatHandler();
        public delegate void MessageEventHandler(string message);
        public delegate void PlayerListHandler(List<Player> playerList);
        public delegate void VoidHandler();
        public delegate void LogHandler(string message);
        public event LogHandler OnLog;
        public event LogHandler OnSystem;
        public event LogHandler OnCommand;
        public event LogHandler OnError;
        public event LogHandler OnOp;
        public event LogHandler OnAdmin;
        public event HeartBeatHandler HeartBeatFail;
        public event MessageEventHandler OnURLChange;
        public event PlayerListHandler OnPlayerListChange;
        public event VoidHandler OnSettingsUpdate;
        public static ForgeBot IRC;
        public static Thread locationChecker;
        public static Thread blockThread;
        //public static List<MySql.Data.MySqlClient.MySqlCommand> mySQLCommands = new List<MySql.Data.MySqlClient.MySqlCommand>();
        public static WebServer APIServer;
        public static WebServer InfoServer;
        public static DateTime StartTime, StartTimeLocal;
        [ConfigBool("enable-http-api", "Server", null, false)]
        public static bool EnableHttpApi = false;
        [ConfigInt("position-interval", "Server", null, 100, 20, 2000)]
        public static int PositionInterval = 100;

        public static PlayersFile AutoloadMaps = new PlayersFile("text/autoload.txt");
        public static PlayersFile Frozen = new PlayersFile("ranks/frozen.txt");
        public static PlayersFile RankInfo = new PlayersFile("text/rankinfo.txt");
        public static PlayersFile Muted = new PlayersFile("ranks/muted.txt");
        public static PlayersFile Jailed = new PlayersFile("ranks/jailed.txt");
        public static PlayersFile TempRanks = new PlayersFile("text/tempranks.txt");
        public static PlayersFile Notes = new PlayersFile("text/notes.txt"); 
        public static PlayersFile Hidden = new PlayersFile("ranks/hidden.txt");
        public static PlayersFile Skins = new PlayersFile("extra/skins.txt");
        public static PlayersFile Models = new PlayersFile("extra/models.txt");
        public static Version Version { get { return System.Reflection.Assembly.GetAssembly(typeof(Server)).GetName().Version; } }

        public static string VersionString {
            get {
                Version v = Version;
                return v.Major + "." + v.Minor + "." + v.Build;
            }
        }

        // URL hash for connecting to the server
        public static string Hash = String.Empty;
        public static string URL = String.Empty;

        public static Socket listen;
        public static System.Timers.Timer updateTimer = new System.Timers.Timer(100);
        //static System.Timers.Timer heartbeatTimer = new System.Timers.Timer(60000); //Every 45 seconds
        static System.Timers.Timer messageTimer = new System.Timers.Timer(60000 * 5); //Every 5 mins

        //Chatrooms
        public static List<string> Chatrooms = new List<string>();
        //Other
        [ConfigBool("allow-tp-to-higher-ranks", "Other", null, true)]        
        public static bool higherranktp = true;
        [ConfigBool("agree-to-rules-on-entry", "Other", null, false)]        
        public static bool agreetorulesonentry = false;
        public static bool UseCTF = false;
        public static bool ServerSetupFinished = false;
        public static Auto_CTF ctf = null;
        public static PlayerList bannedIP;
        public static PlayerList whiteList;
        public static PlayerList ircControllers;
        public static PlayerList muted;
        public static PlayerList ignored;
        public static PlayerList frozen;

        public static readonly List<string> Devs = new List<string>();
        public static readonly List<string> Mods = new List<string>();

        internal static readonly List<string> opstats = new List<string>(new string[] { "ban", "tempban", "kick", "warn", "mute", "freeze", "undo", "kickban", "demote", "promote" });
        public static List<string> Opstats { get { return new List<string>(opstats); } }

        public static List<TempBan> tempBans = new List<TempBan>();
        public struct TempBan { public string name, reason; public DateTime expiryTime; }

        public static PerformanceCounter PCCounter = null;
        public static PerformanceCounter ProcessCounter = null;

        public static Level mainLevel;
        public static List<Level> levels;
        //reviewlist intitialize
        public static List<string> reviewlist = new List<string>();

        public static List<string> ircafkset = new List<string>();
        public static List<string> messages = new List<string>();

        public static string IP;
        //auto updater stuff
        [ConfigBool("auto-update", "Update", null, false)]
        public static bool autoupdate;
        public static bool autonotify;
        [ConfigBool("in-game-update-notify", "Server", null, false)]        
        public static bool notifyPlayers;
        [ConfigInt("update-countdown", "Update", null, 10)]        
        public static int restartcountdown = 10;
        [ConfigBool("auto-restart", "Server", null, false)]    
        public static bool autorestart;
        [ConfigDateTime("restarttime", "Server", null)]
        public static DateTime restarttime;

        public static bool chatmod = false;
        [ConfigBool("log-notes", "Other", null, true)]
        public static bool LogNotes = true;
        
        [ConfigPerm("os-perbuild-default", "other", null, LevelPermission.Nobody)]
        public static LevelPermission osPerbuildDefault = LevelPermission.Nobody;
        [ConfigBool("tablist-rank-sorted", "Tablist", null, true)]
        public static bool TablistRankSorted = true;
        [ConfigBool("tablist-global", "Tablist", null, false)]
        public static bool TablistGlobal = false;
        [ConfigBool("tablist-bots", "Tablist", null, false)]
        public static bool TablistBots = false;
        
        //Global VoteKick In Progress Flag
        public static bool voteKickInProgress = false;
        public static int voteKickVotesNeeded = 0;

        // Extra storage for custom commands
        public ExtrasCollection Extras = new ExtrasCollection();

        //Zombie
        public static ZombieGame zombie;
        
        [ConfigBool("bufferblocks", "Other", null, false)]        
        public static bool bufferblocks = true;
        public static int YesVotes = 0;
        public static int NoVotes = 0;
        public static bool voting = false;
        public static bool votingforlevel = false;
        // Lava Survival
        public static LavaSurvival lava;
        
        public static CountdownGame Countdown;

        //Settings
        #region Server Settings
        public const byte version = 7;
        public static string salt = "";

        [ConfigString("server-name", "General", null, 
                      "[MCGalaxy] Default", false, "![]:.,{}~-+()?_/\\' ")]
        public static string name = "[MCGalaxy] Default";
        [ConfigString("motd", "General", null, "Welcome",
                     false,  "=![]&:.,{}~-+()?_/\\' ")]
        public static string motd = "Welcome!";
        [ConfigInt("max-players", "Server", null, 12, 1, 128)]
        public static int players = 12;
        [ConfigInt("max-guests", "Server", null, 10, 1, 128)]
        public static int maxGuests = 10;

        [ConfigInt("port", "Server", null, 25565, 0, 65535)]
        public static int port = 25565;
        [ConfigBool("public", "Server", null, true)]
        public static bool pub = true;
        [ConfigBool("verify-names", "Server", null, true)]
        public static bool verify = true;
        [ConfigBool("world-chat", "Server", null, true)]
        public static bool worldChat = true;

        //Spam Prevention
        [ConfigBool("mute-on-spam", "Spam control", null, false)]        
        public static bool checkspam = false;
        [ConfigInt("spam-messages", "Spam control", null, 8)]
        public static int spamcounter = 8;
        [ConfigInt("spam-mute-time", "Spam control", null, 60)]        
        public static int mutespamtime = 60;
        [ConfigInt("spam-counter-reset-time", "Spam control", null, 5)]        
        public static int spamcountreset = 5;
        [ConfigString("host-state", "Other", null, "Alive")]
        public static string ZallState = "Alive";

        [ConfigString("main-name", "General", null, "main", false, "._+")]
        public static string level = "main";
        public static string errlog = "error.log";

        [ConfigBool("report-back", "Error handling", null, true)]
        public static bool reportBack = true;

        [ConfigBool("irc", "IRC bot", null, false)]
        public static bool irc = false;
        [ConfigBool("irc-colorsenable", "IRC bot", null, true)]
        public static bool ircColorsEnable = true;
        [ConfigInt("irc-port", "IRC bot", null, 6667, 0, 65535)]        
        public static int ircPort = 6667;        
        [ConfigString("irc-nick", "IRC bot", null, "ForgeBot")]
        public static string ircNick = "ForgeBot";
        [ConfigString("irc-server", "IRC bot", null, "irc.esper.net")]
        public static string ircServer = "irc.esper.net";
        [ConfigString("irc-channel", "IRC bot", null, "#changethis", true)]        
        public static string ircChannel = "#changethis";
        [ConfigString("irc-opchannel", "IRC bot", null, "#changethistoo", true)]        
        public static string ircOpChannel = "#changethistoo";      
        [ConfigBool("irc-identify", "IRC bot", null, false)]
        public static bool ircIdentify = false;
        [ConfigString("irc-password", "IRC bot", null, "", true)]        
        public static string ircPassword = "";
        [ConfigEnum("irc-controller-verify", "IRC bot", null, IRCControllerVerify.HalfOp, typeof(IRCControllerVerify))]
        public static IRCControllerVerify IRCVerify = IRCControllerVerify.HalfOp;
        
        [ConfigBool("admin-verification", "Admin", null, true)]        
        public static bool verifyadmins = true;
        [ConfigPerm("verify-admin-perm", "Admin", null, LevelPermission.Operator)]
        public static LevelPermission verifyadminsrank = LevelPermission.Operator;

        [ConfigBool("restart-on-error", "Error handling", null, true)]           
        public static bool restartOnError = true;
        [ConfigInt("rplimit", "Other", null, 500, 0, 50000)]
        public static int rpLimit = 500;
        [ConfigInt("rplimit-norm", "Other", null, 10000, 0, 50000)] 
        public static int rpNormLimit = 10000;

        [ConfigInt("backup-time", "Backup", null, 300, 1)] 
        public static int backupInterval = 300;
        public static int blockInterval = 60;
        [ConfigString("backup-location", "Backup", null, "")]
        public static string backupLocation = Application.StartupPath + "/levels/backups";
        [ConfigStringList("disabledstandardtokens", "Other", null)]
        internal static List<string> disabledChatTokens = new List<string>();

        [ConfigBool("physicsrestart", "Other", null, true)]          
        public static bool physicsRestart = true;
        [ConfigBool("deathcount", "Other", null, true)]
        public static bool deathcount = true;
        [ConfigBool("autoload", "Server", null, false)]
        public static bool AutoLoad = false;
        [ConfigInt("physics-undo-max", "Other", null, 20000)]          
        public static int physUndo = 20000;
        [ConfigInt("total-undo", "Other", null, 200)]          
        public static int totalUndo = 200;
        [ConfigBool("parse-emotes", "Other", null, true)]          
        public static bool parseSmiley = true;
        [ConfigBool("use-whitelist", "Other", null, false)]          
        public static bool useWhitelist = false;
        [ConfigBool("force-cuboid", "Other", null, false)]        
        public static bool forceCuboid = false;
        [ConfigBool("profanity-filter", "Other", null, false)]        
        public static bool profanityFilter = false;
        [ConfigBool("repeat-messages", "Other", null, false)]
        public static bool repeatMessage = false;

        [ConfigBool("check-updates", "Update", null, false)]
        public static bool checkUpdates = true;

        [ConfigBool("UseMySQL", "Database", null, false)] 
        public static bool useMySQL = false;
        [ConfigString("host", "Database", null, "127.0.0.1")]        
        public static string MySQLHost = "127.0.0.1";
        [ConfigString("SQLPort", "Database", null, "3306")]          
        public static string MySQLPort = "3306";
        [ConfigString("Username", "Database", null, "root", true)]          
        public static string MySQLUsername = "root";
        [ConfigString("Password", "Database", null, "password", true)]          
        public static string MySQLPassword = "password";
        [ConfigString("DatabaseName", "Database", null, "MCZallDB")]          
        public static string MySQLDatabaseName = "MCZallDB";
        [ConfigBool("Pooling", "Database", null, true)]          
        public static bool DatabasePooling = true;

        [ConfigColor("defaultColor", "Colors", null, "&e")]
        public static string DefaultColor = "&e";
        [ConfigColor("irc-color", "Colors", null, "&5")]
        public static string IRCColour = "&5";
        [ConfigColor("global-chat-color", "Colors", null, "&6")]        
        public static string GlobalChatColor = "&6";
        [ConfigColor("help-syntax-color", "Colors", null, "&a")]        
        public static string HelpSyntaxColor = "&a";
        [ConfigColor("help-desc-color", "Colors", null, "&e")]        
        public static string HelpDescriptionColor = "&e";
        
        [ConfigBool("global-chat-enabled", "Other", null, true)]
        public static bool UseGlobalChat = true;
        [ConfigInt("afk-minutes", "Other", null, 10)] 
        public static int afkminutes = 10;
        [ConfigInt("afk-kick", "Other", null, 45)]
        public static int afkkick = 45;
        [ConfigPerm("afk-kick-perm", "Other", null, LevelPermission.AdvBuilder)]
        public static LevelPermission afkkickperm = LevelPermission.AdvBuilder;
        [ConfigString("default-rank", "General", null, "guest")]
        public static string defaultRank = "guest";

        [ConfigBool("dollar-before-dollar", "Other", null, true)]
        public static bool dollarNames = true;
        public static bool unsafe_plugin = true;
        [ConfigBool("cheapmessage", "Other", null, true)]        
        public static bool cheapMessage = true;
        
        [ConfigString("cheap-message-given", "Messages", null, " is now being cheap and being immortal")]
        public static string cheapMessageGiven = " is now being cheap and being immortal";
        [ConfigString("custom-ban-message", "Messages", null, "You're banned!")]
        public static string defaultBanMessage = "You're banned!";
        [ConfigString("custom-shutdown-message", "Messages", null, "Server shutdown. Rejoin in 10 seconds.")]
        public static string shutdownMessage = "Server shutdown. Rejoin in 10 seconds.";
        [ConfigString("custom-promote-message", "Messages", null, "&6Congratulations for working hard and getting &2PROMOTED!")]
        public static string defaultPromoteMessage = "&6Congratulations for working hard and getting &2PROMOTED!";
        [ConfigString("custom-demote-message", "Messages", null, "&4DEMOTED! &6We're sorry for your loss. Good luck on your future endeavors! &1:'(")]
        public static string defaultDemoteMessage = "&4DEMOTED! &6We're sorry for your loss. Good luck on your future endeavors! &1:'(";

        [ConfigString("money-name", "Other", null, "moneys")]        
        public static string moneys = "moneys";
        [ConfigPerm("opchat-perm", "Other", null, LevelPermission.Operator)]                
        public static LevelPermission opchatperm = LevelPermission.Operator;
        [ConfigPerm("adminchat-perm", "Other", null, LevelPermission.Admin)]        
        public static LevelPermission adminchatperm = LevelPermission.Admin;
        
        [ConfigBool("log-heartbeat", "Other", null, false)]
        public static bool logbeat = false;
        [ConfigBool("admins-join-silent", "Other", null, false)]
        public static bool adminsjoinsilent = false;
        public static bool mono { get { return (Type.GetType("Mono.Runtime") != null); } }
        [ConfigString("server-owner", "Other", null, "Notch")]        
        public static string server_owner = "Notch";
        
        [ConfigBool("guest-limit-notify", "Other", null, false)]        
        public static bool guestLimitNotify = false;
        [ConfigBool("guest-join-notify", "Other", null, true)]        
        public static bool guestJoinNotify = true;
        [ConfigBool("guest-leave-notify", "Other", null, true)]        
        public static bool guestLeaveNotify = true;
        [ConfigString("default-texture-url", "General", null, "", true)]
        public static string defaultTerrainUrl = "";
        [ConfigString("default-texture-pack-url", "General", null, "", true)]        
        public static string defaultTextureUrl = "";

        public static bool flipHead = false;

        public static bool shuttingDown = false;
        public static bool restarting = false;

        //hackrank stuff
        [ConfigBool("kick-on-hackrank", "Other", null, true)]
        public static bool hackrank_kick = true;
        [ConfigInt("hackrank-kick-time", "Other", null, 5)]        
        public static int hackrank_kick_time = 5; //seconds, it converts it to milliseconds in the command.
        [ConfigBool("show-empty-ranks", "Other", null, false)]  
        public static bool showEmptyRanks = false;

        //reviewoptions intitialize
        [ConfigInt("review-cooldown", "Review", null, 600)]
        public static int reviewcooldown = 600;
        [ConfigPerm("review-enter-perm", "Review", null, LevelPermission.Guest)]        
        public static LevelPermission reviewenter = LevelPermission.Guest;
        [ConfigPerm("review-leave-perm", "Review", null, LevelPermission.Guest)]        
        public static LevelPermission reviewleave = LevelPermission.Guest;
        [ConfigPerm("review-view-perm", "Review", null, LevelPermission.Operator)]        
        public static LevelPermission reviewview = LevelPermission.Operator;
        [ConfigPerm("review-next-perm", "Review", null, LevelPermission.Operator)]        
        public static LevelPermission reviewnext = LevelPermission.Operator;
        [ConfigPerm("review-clear-perm", "Review", null, LevelPermission.Operator)]
        public static LevelPermission reviewclear = LevelPermission.Operator;

        [ConfigInt("draw-reload-limit", "Other", null, 10000)]
        public static int DrawReloadLimit = 10000;
        [ConfigInt("map-gen-limit-admin", "Other", null, 225 * 1000 * 1000)]
        public static int MapGenLimitAdmin = 225 * 1000 * 1000;
        [ConfigInt("map-gen-limit", "Other", null, 30 * 1000 * 1000)] 
        public static int MapGenLimit = 30 * 1000 * 1000;
        #endregion

        public static MainLoop ml;
        public static Server s;
        public Server() {
            ml = new MainLoop("MCG_Scheduler");
            Server.s = this;
        }
        //True = cancel event
        //Fale = dont cacnel event
        public static bool Check(string cmd, string message)
        {
            if (ConsoleCommand != null)
                ConsoleCommand(cmd, message);
            return cancelcommand;
        }
        
        void CheckFile(string file) {
            if (File.Exists(file)) return;
            
            Log(file + " doesn't exist, Downloading");
            try {
                using (WebClient web = new WebClient())
                    web.DownloadFile("https://github.com/Hetal728/MCGalaxy/blob/master/" + file + "?raw=true", file);
                if (File.Exists(file))
                    Log(file + " download succesful!");
            } catch {
                Log("Downloading " + file + " failed, please try again later");
            }
        }
        
        internal static ConfigElement[] serverConfig, levelConfig;
        public void Start() {
            serverConfig = ConfigElement.GetAll(typeof(Server), typeof(ZombieGame));
            levelConfig = ConfigElement.GetAll(typeof(Level));
        	
            PlayerInfo.players = PlayerInfo.Online.list;
            Player.players = PlayerInfo.Online.list;
            Server.levels = LevelInfo.Loaded.list;
            PlayerBot.playerbots = PlayerBot.Bots.list;
            StartTime = DateTime.UtcNow;
            StartTimeLocal = StartTime.ToLocalTime();
            shuttingDown = false;
            Log("Starting Server");
            try {
                if (File.Exists("Restarter.exe"))
                    File.Delete("Restarter.exe");
            } catch { }
            try {
                if (File.Exists("Restarter.pdb"))
                    File.Delete("Restarter.pdb");
            } catch { }
            
            CheckFile("MySql.Data.dll");
            CheckFile("System.Data.SQLite.dll");
            CheckFile("sqlite3.dll");
            CheckFile("Newtonsoft.Json.dll");
            CheckFile("LibNoise.dll");

            EnsureFilesExist();
            MoveOutdatedFiles();

            if (File.Exists("text/emotelist.txt")) {
                foreach (string s in File.ReadAllLines("text/emotelist.txt"))
                    Player.emoteList.Add(s);
            } else {
                File.Create("text/emotelist.txt").Dispose();
            }

            lava = new LavaSurvival();
            zombie = new ZombieGame();
            Countdown = new CountdownGame();
            LoadAllSettings();

            InitDatabase();
            Economy.LoadDatabase();
            Server.zombie.CheckTableExists();

            Level[] loaded = LevelInfo.Loaded.Items;
            foreach (Level l in loaded)
                l.Unload();
            ml.Queue(LoadMainLevel);
            Plugin.Load();
            ml.Queue(LoadPlayerLists);
            ml.Queue(LoadAutoloadCommands);
            ml.Queue(SetupSocket);

            ml.Queue(InitTimers);
            ml.Queue(InitRest);
            ml.Queue(InitHeartbeat);
            UpdateStaffList();
        }
        
        void EnsureFilesExist() {
            if (!Directory.Exists("properties")) Directory.CreateDirectory("properties");
            if (!Directory.Exists("levels")) Directory.CreateDirectory("levels");
            if (!Directory.Exists("bots")) Directory.CreateDirectory("bots");
            if (!Directory.Exists("text")) Directory.CreateDirectory("text");
            TempRanks.EnsureExists();
            RankInfo.EnsureExists();
            if (!File.Exists("text/bans.txt")) File.CreateText("text/bans.txt").Dispose();

            if (!Directory.Exists("extra")) Directory.CreateDirectory("extra");
            if (!Directory.Exists("extra/undo")) Directory.CreateDirectory("extra/undo");
            if (!Directory.Exists("extra/undoPrevious")) Directory.CreateDirectory("extra/undoPrevious");
            if (!Directory.Exists("extra/copy/")) Directory.CreateDirectory("extra/copy/");
            if (!Directory.Exists("extra/copyBackup/")) Directory.CreateDirectory("extra/copyBackup/");
            if (!Directory.Exists("extra/Waypoints")) Directory.CreateDirectory("extra/Waypoints");
            if (!Directory.Exists("blockdefs")) Directory.CreateDirectory("blockdefs");
            if (!Directory.Exists("text/rankreqs")) Directory.CreateDirectory("text/rankreqs");
        }
        
        void MoveOutdatedFiles() {
            try {
                if (File.Exists("blocks.json")) File.Move("blocks.json", "blockdefs/global.json");
                if (File.Exists("server.properties")) File.Move("server.properties", "properties/server.properties");
                if (File.Exists("rules.txt")) File.Move("rules.txt", "text/rules.txt");
                if (File.Exists("welcome.txt")) File.Move("welcome.txt", "text/welcome.txt");
                if (File.Exists("messages.txt")) File.Move("messages.txt", "text/messages.txt");
                if (File.Exists("externalurl.txt")) File.Move("externalurl.txt", "text/externalurl.txt");
                if (File.Exists("autoload.txt")) File.Move("autoload.txt", "text/autoload.txt");
                if (File.Exists("IRC_Controllers.txt")) File.Move("IRC_Controllers.txt", "ranks/IRC_Controllers.txt");
                if (useWhitelist && File.Exists("whitelist.txt")) File.Move("whitelist.txt", "ranks/whitelist.txt");
            }
            catch { }
        }
        
        void InitDatabase() {
            try {
                if (Server.useMySQL)
                    Database.executeQuery("CREATE DATABASE if not exists `" + MySQLDatabaseName + "`", true);
            } catch (Exception e) {
                ErrorLog(e);
                s.Log("MySQL settings have not been set! Please Setup using the properties window.");
                return;
            }
            
            string autoInc = useMySQL ? "AUTO_INCREMENT" : "AUTOINCREMENT";
            Database.executeQuery(string.Format("CREATE TABLE if not exists Players (ID INTEGER {0}" + autoInc + " NOT NULL, " +
                                                "Name TEXT, IP CHAR(15), FirstLogin DATETIME, LastLogin DATETIME, totalLogin MEDIUMINT, " +
                                                "Title CHAR(20), TotalDeaths SMALLINT, Money MEDIUMINT UNSIGNED, totalBlocks BIGINT, " +
                                                "totalCuboided BIGINT, totalKicked MEDIUMINT, TimeSpent VARCHAR(20), color VARCHAR(6), " +
                                                "title_color VARCHAR(6){1});", (useMySQL ? "" : "PRIMARY KEY "), (useMySQL ? ", PRIMARY KEY (ID)" : "")));
            Database.executeQuery(string.Format("CREATE TABLE if not exists Opstats (ID INTEGER {0}" + autoInc + " NOT NULL, " +
                                                "Time DATETIME, Name TEXT, Cmd VARCHAR(40), Cmdmsg VARCHAR(40){1});",
                                                (useMySQL ? "" : "PRIMARY KEY "), (useMySQL ? ", PRIMARY KEY (ID)" : "")));
            if (!File.Exists("extra/alter.txt") && useMySQL) {
                Database.executeQuery("ALTER TABLE Players MODIFY Name TEXT");
                Database.executeQuery("ALTER TABLE Opstats MODIFY Name TEXT");
                File.Create("extra/alter.txt");
            }
            
            //since 5.5.11 we are cleaning up the table Playercmds
            string query = Server.useMySQL ? "SHOW TABLES LIKE 'Playercmds'" : "SELECT name FROM sqlite_master WHERE type='table' AND name='Playercmds';";
            DataTable playercmds = Database.fillData(query), opstats = Database.fillData("SELECT * FROM Opstats");
            //if Playercmds exists copy-filter to Ostats and remove Playercmds
            if (playercmds.Rows.Count != 0) {
                foreach (string cmd in Server.Opstats)
                    Database.executeQuery(string.Format("INSERT INTO Opstats (Time, Name, Cmd, Cmdmsg) SELECT Time, Name, Cmd, Cmdmsg FROM Playercmds WHERE cmd = '{0}';", cmd));
                Database.executeQuery("INSERT INTO Opstats (Time, Name, Cmd, Cmdmsg) SELECT Time, Name, Cmd, Cmdmsg FROM Playercmds WHERE cmd = 'review' AND cmdmsg = 'next';");
                Database.fillData("DROP TABLE Playercmds");
            }
            playercmds.Dispose(); opstats.Dispose();

            // Here, since SQLite is a NEW thing from 5.3.0.0, we do not have to check for existing tables in SQLite.
            if (!useMySQL) return;
            // Check if the color column exists.
            DataTable colorExists = Database.fillData("SHOW COLUMNS FROM Players WHERE `Field`='color'");
            if (colorExists.Rows.Count == 0)
                Database.executeQuery("ALTER TABLE Players ADD COLUMN color VARCHAR(6) AFTER totalKicked");
            colorExists.Dispose();

            DataTable tcolorExists = Database.fillData("SHOW COLUMNS FROM Players WHERE `Field`='title_color'");
            if (tcolorExists.Rows.Count == 0)
                Database.executeQuery("ALTER TABLE Players ADD COLUMN title_color VARCHAR(6) AFTER color");
            tcolorExists.Dispose();

            DataTable timespent = Database.fillData("SHOW COLUMNS FROM Players WHERE `Field`='TimeSpent'");
            if (timespent.Rows.Count == 0)
                Database.executeQuery("ALTER TABLE Players ADD COLUMN TimeSpent VARCHAR(20) AFTER totalKicked");
            timespent.Dispose();

            DataTable totalCuboided = Database.fillData("SHOW COLUMNS FROM Players WHERE `Field`='totalCuboided'");
            if (totalCuboided.Rows.Count == 0)
                Database.executeQuery("ALTER TABLE Players ADD COLUMN totalCuboided BIGINT AFTER totalBlocks");
            totalCuboided.Dispose();
        }
        
        public static string SendResponse(HttpListenerRequest request)
        {
            try {
                string api = "";
                API API = new API();
                API.max_players = (int)Server.players;
                API.players = PlayerInfo.players.Select(mc => mc.name).ToArray();
                API.chat = Player.Last50Chat.ToArray();
                api = JsonConvert.SerializeObject(API, Formatting.Indented);
                return api;
            }
            catch(Exception e)
            {
                Logger.WriteError(e);
            }
            return "Error";
        }
        public static string WhoIsResponse(HttpListenerRequest request)
        {
            try
            {
                string p = request.QueryString.Get("name");
                if (p == null || p == "")
                    return "Error";
                var whois = new WhoWas(p);
                Group grp = Group.Find(whois.rank);
                if (grp != null && grp.Permission == LevelPermission.Banned)
                    whois.banned = true;
                else
                    whois.banned = Ban.IsBanned(p);
                
                if (whois.banned) {
                    string[] bandata = Ban.GetBanData(p);
                    whois.banned_by = bandata[0];
                    whois.ban_reason = bandata[1];
                    whois.banned_time = bandata[2];
                }
                return JsonConvert.SerializeObject(whois, Formatting.Indented);
            }
            catch(Exception e)
            {
                Logger.WriteError(e);
            }
            return "Error";
        }
        
        public static void LoadAllSettings() {
            zombie.LoadInfectMessages();
            Colors.LoadExtColors();
            Alias.Load();
            Bots.BotsFile.Load();
            BlockDefinition.LoadGlobal();
            SrvProperties.Load("properties/server.properties");
            Updater.Load("properties/update.properties");
            Group.InitAll();
            Command.InitAll();
            GrpCommands.fillRanks();
            Block.SetBlocks();
            Awards.Load();
            Economy.Load();
            WarpList.Global.Load(null);
            CommandOtherPerms.Load();
            ProfanityFilter.Init();
            Team.LoadList();
            Chat.LoadCustomTokens();
        }

        public static void Setup()
        {
            try
            {
                IPEndPoint endpoint = new IPEndPoint(IPAddress.Any, port);
                listen = new Socket(endpoint.Address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                listen.Bind(endpoint);
                listen.Listen((int)SocketOptionName.MaxConnections);
                listen.BeginAccept(Accept, null);
            }
            catch (SocketException e) { ErrorLog(e); s.Log("Error Creating listener, socket shutting down"); }
            catch (Exception e) { ErrorLog(e); s.Log("Error Creating listener, socket shutting down"); }
        }

        static void Accept(IAsyncResult result)
        {
            if (shuttingDown) return;

            Player p = null;
            bool begin = false;
            try
            {
                p = new Player(listen.EndAccept(result));
                //new Thread(p.Start).Start();
                listen.BeginAccept(Accept, null);
                begin = true;
            }
            catch (SocketException)
            {
                if (p != null)
                    p.Disconnect();
                if (!begin)
                    listen.BeginAccept(Accept, null);
            }
            catch (Exception e)
            {
                ErrorLog(e);
                if (p != null)
                    p.Disconnect();
                if (!begin)
                    listen.BeginAccept(Accept, null);
            }

        }

        public static void Exit(bool AutoRestart)
        {
            Player[] players = PlayerInfo.Online.Items;
            foreach (Player p in players) { p.save(); }
            foreach (Player p in players)
            {
                string msg = AutoRestart ? "Server restarted. Sign in again and rejoin." : Server.shutdownMessage;
                p.LeaveServer(msg, msg);
            }
            if (APIServer != null) APIServer.Stop();
            if (InfoServer != null) InfoServer.Stop();
            //PlayerInfo.players.ForEach(delegate(Player p) { p.Kick("Server shutdown. Rejoin in 10 seconds."); });
            Player.connections.ForEach(
                delegate(Player p)
                {
                    string msg = AutoRestart ? "Server restarted. Sign in again and rejoin." : Server.shutdownMessage;
                    p.LeaveServer(msg, msg);
                }
            );
            Plugin.Unload();
            if (listen != null)
            {
                listen.Close();
            }
            try
            {
                IRC.Disconnect(!AutoRestart ? "Server is shutting down." : "Server is restarting.");
            }
            catch { }
        }

        [Obsolete("Use LevelInfo.Loaded.Add()")]
        public static void addLevel(Level level)
        {
            LevelInfo.Loaded.Add(level);
        }

        public void PlayerListUpdate()
        {
            if (Server.s.OnPlayerListChange != null) Server.s.OnPlayerListChange(PlayerInfo.players);
        }

        public void FailBeat()
        {
            if (HeartBeatFail != null) HeartBeatFail();
        }

        public void UpdateUrl(string url)
        {
            if (OnURLChange != null) OnURLChange(url);
        }

        public void Log(string message, bool systemMsg = false)
        {
            message = CP437Writer.ConvertToUnicode(message);
            if (ServerLog != null)
            {
                ServerLog(message);
                if (cancellog)
                {
                    cancellog = false;
                    return;
                }
            }
            
            if (!systemMsg)
                OnServerLogEvent.Call(message);
            if (!systemMsg && OnLog != null)
                OnLog(DateTime.Now.ToString("(HH:mm:ss) ") + message);
            if (systemMsg && OnSystem != null)
                OnSystem(DateTime.Now.ToString("(HH:mm:ss) ") + message);
            Logger.Write(DateTime.Now.ToString("(HH:mm:ss) ") + message + Environment.NewLine);
        }
        public void OpLog(string message, bool systemMsg = false)
        {
            message = CP437Writer.ConvertToUnicode(message);
            if (ServerOpLog != null)
            {
                OpLog(message);
                if (canceloplog)
                {
                    canceloplog = false;
                    return;
                }
            }
            if (OnOp != null)
            {
                if (!systemMsg)
                {
                    OnOp(DateTime.Now.ToString("(HH:mm:ss) ") + message);
                }
                else
                {
                    OnSystem(DateTime.Now.ToString("(HH:mm:ss) ") + message);
                }
            }

            Logger.Write(DateTime.Now.ToString("(HH:mm:ss) ") + message + Environment.NewLine);
        }

        public void AdminLog(string message, bool systemMsg = false)
        {
            message = CP437Writer.ConvertToUnicode(message);
            if (ServerAdminLog != null)
            {
                ServerAdminLog(message);
                if (canceladmin)
                {
                    canceladmin = false;
                    return;
                }
            }
            if (OnAdmin != null)
            {
                if (!systemMsg)
                {
                    OnAdmin(DateTime.Now.ToString("(HH:mm:ss) ") + message);
                }
                else
                {
                    OnSystem(DateTime.Now.ToString("(HH:mm:ss) ") + message);
                }
            }

            Logger.Write(DateTime.Now.ToString("(HH:mm:ss) ") + message + Environment.NewLine);
        }

        public void ErrorCase(string message)
        {
            if (OnError != null)
                OnError(message);
        }

        public void CommandUsed(string message)
        {
            message = CP437Writer.ConvertToUnicode(message);
            if (OnCommand != null) OnCommand(DateTime.Now.ToString("(HH:mm:ss) ") + message);
            Logger.Write(DateTime.Now.ToString("(HH:mm:ss) ") + message + Environment.NewLine);
        }

        public static void ErrorLog(Exception ex)
        {
            if (ServerError != null)
                ServerError(ex);
            OnServerErrorEvent.Call(ex);
            Logger.WriteError(ex);
            try
            {
                s.Log("!!!Error! See " + Logger.ErrorLogPath + " for more information.");
            }
            catch { }
        }

        public static void RandomMessage()
        {
            if (Player.number != 0 && messages.Count > 0)
                Player.GlobalMessage(messages[new Random().Next(0, messages.Count)]);
        }

        internal void SettingsUpdate()
        {
            if (OnSettingsUpdate != null) OnSettingsUpdate();
        }

        public static string FindColor(string Username)
        {
            foreach (Group grp in Group.GroupList.Where(grp => grp.playerList.Contains(Username)))
            {
                return grp.color;
            }
            return Group.standard.color;
        }

        public static bool canusegc = true; //badpokerface
        public static int gcmultiwarns = 0, gcspamcount = 0, gccapscount = 0, gcfloodcount = 0;
        public static DateTime gclastmsgtime = DateTime.MinValue;
        public static string gclastmsg = "";
    }
}