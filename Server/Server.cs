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
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Forms;
using MCGalaxy.SQL;
using MonoTorrent.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MCGalaxy
{
    public sealed class Server
    {
        public static bool cancelcommand = false;
        public static bool canceladmin = false;
        public static bool cancellog = false;
        public static bool canceloplog = false;
        public static bool DownloadBeta = false;
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
        public static GlobalChatBot GlobalChat;
        public static Thread locationChecker;
        public static Thread blockThread;
        public static bool IgnoreOmnibans = false;
        //public static List<MySql.Data.MySqlClient.MySqlCommand> mySQLCommands = new List<MySql.Data.MySqlClient.MySqlCommand>();
        public static WebServer APIServer;
        public static WebServer InfoServer;
        public static DateTime StartTime, StartTimeLocal;

        public static int speedPhysics = 250;

        public static Version Version { get { return System.Reflection.Assembly.GetAssembly(typeof(Server)).GetName().Version; } }

        public static string VersionString
        {
            get
            {
                return Version.Major + "." + Version.Minor + "." + Version.Build;
            }
        }

        // URL hash for connecting to the server
        public static string Hash = String.Empty;
        public static string URL = String.Empty;

        public static Socket listen;
        public static System.Timers.Timer updateTimer = new System.Timers.Timer(100);
        //static System.Timers.Timer heartbeatTimer = new System.Timers.Timer(60000); //Every 45 seconds
        static System.Timers.Timer messageTimer = new System.Timers.Timer(60000 * 5); //Every 5 mins
        public static System.Timers.Timer cloneTimer = new System.Timers.Timer(5000);

        //public static Thread physThread;
        //public static bool physPause;
        //public static DateTime physResume = DateTime.Now;
        //public static System.Timers.Timer physTimer = new System.Timers.Timer(1000);
        // static Thread botsThread;
        //Chatrooms
        public static List<string> Chatrooms = new List<string>();
        //Other
        public static bool higherranktp = true;
        public static bool agreetorulesonentry = false;
        public static bool UseCTF = false;
        public static bool ServerSetupFinished = false;
        public static Auto_CTF ctf = null;
        public static PlayerList bannedIP;
        public static PlayerList whiteList;
        public static PlayerList ircControllers;
        public static PlayerList muted;
        public static PlayerList ignored;

        // The MCGalaxy Developer List
        internal static readonly List<string> devs = new List<string>();
        public static List<string> Devs { get { return new List<string>(devs); } }
        //The MCGalaxy Moderation List
        internal static readonly List<string> mods = new List<string>();
        public static List<string> Mods { get { return new List<string>(mods); } }
        //GCMods List
        internal static readonly List<string> gcmods = new List<string>();
        public static List<string> GCmods { get { return new List<string>(gcmods); } }
        internal static readonly List<string> protectover = new List<string>(new string[] { "moderate", "mute", "freeze", "lockdown", "ban", "banip", "kickban", "kick", "global", "xban", "xundo", "undo", "uban", "unban", "unbanip", "demote", "promote", "restart", "shutdown", "setrank", "warn", "tempban", "impersonate", "sendcmd", "possess", "joker", "jail", "ignore", "voice" });
        public static List<string> ProtectOver { get { return new List<string>(protectover); } }

        internal static readonly List<string> opstats = new List<string>(new string[] { "ban", "tempban", "kick", "warn", "mute", "freeze", "undo", "kickban", "demote", "promote" });
        public static List<string> Opstats { get { return new List<string>(opstats); } }

        public static List<TempBan> tempBans = new List<TempBan>();
        public struct TempBan { public string name; public DateTime allowedJoin; }

        public static PerformanceCounter PCCounter = null;
        public static PerformanceCounter ProcessCounter = null;

        public static Level mainLevel;
        public static List<Level> levels;
        //reviewlist intitialize
        public static List<string> reviewlist = new List<string>();
        //Global Chat Rules Accepted list
        public static List<string> gcaccepted = new List<string>();
        //public static List<levelID> allLevels = new List<levelID>();
        public struct levelID { public int ID; public string name; }

        public static List<string> afkset = new List<string>();
        public static List<string> ircafkset = new List<string>();
        public static List<string> afkmessages = new List<string>();
        public static List<string> messages = new List<string>();

        public static Dictionary<string, string> gcnamebans = new Dictionary<string, string>();
        public static Dictionary<string, string> gcipbans = new Dictionary<string, string>();

        public static DateTime timeOnline;
        public static string IP;
        //auto updater stuff
        public static bool autoupdate;
        public static bool autonotify;
        public static bool notifyPlayers;
        public static string restartcountdown = "";
        public static string selectedrevision = "";
        public static bool autorestart;
        public static DateTime restarttime;

        public static bool chatmod = false;

        //Global VoteKick In Progress Flag
        public static bool voteKickInProgress = false;
        public static int voteKickVotesNeeded = 0;


        //WoM Direct
        public static string Server_ALT = "";
        public static string Server_Disc = "";
        public static string Server_Flag = "";


        public static Dictionary<string, string> customdollars = new Dictionary<string, string>();

        // Extra storage for custom commands
        public ExtrasCollection Extras = new ExtrasCollection();

        //Color list as a char array
        public static Char[] ColourCodesNoPercent = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f' };

        //Zombie
        public static ZombieGame zombie;
        public static bool ZombieModeOn = false;
        public static bool startZombieModeOnStartup = false;
        public static bool noRespawn = true;
        public static bool noLevelSaving = true;
        public static bool noPillaring = true;
        public static string ZombieName = "";
        public static int gameStatus = 0; //0 = not started, 1 = always on, 2 = one time, 3 = certain amount of rounds, 4 = stop game next round
        public static bool queLevel = false;
        public static bool queZombie = false;
        public static string nextZombie = "";
        public static string nextLevel = "";
        public static bool zombieRound = false;
        public static string lastPlayerToInfect = "";
        public static int infectCombo = 0;
        public static int YesVotes = 0;
        public static int NoVotes = 0;
        public static bool voting = false;
        public static bool votingforlevel = false;
        public static int Level1Vote = 0;
        public static int Level2Vote = 0;
        public static int Level3Vote = 0;
        public static bool ChangeLevels = true;
        public static bool UseLevelList = false;
        public static bool ZombieOnlyServer = true;
        public static List<String> LevelList = new List<String>();
        public static string lastLevelVote1 = "";
        public static string lastLevelVote2 = "";
        public static bool bufferblocks = true;

        // Lava Survival
        public static LavaSurvival lava;
        
        public static CountdownGame Countdown;

        // OmniBan
        public static OmniBan omniban;
        public static System.Timers.Timer omnibanCheckTimer = new System.Timers.Timer(60000 * 120);

        //Settings
        #region Server Settings
        public const byte version = 7;
        public static string salt = "";

        public static string name = "[MCGalaxy] Default";
        public static string motd = "Welcome!";
        public static byte players = 12;
        public static int menustyle = 0;
        //for the limiting no. of guests:
        public static byte maxGuests = 10;

        public static byte maps = 5;
        public static int port = 25565;
        public static bool pub = true;
        public static bool verify = true;
        public static bool worldChat = true;
        //        public static bool guestGoto = false;

        //Spam Prevention
        public static bool checkspam = false;
        public static int spamcounter = 8;
        public static int mutespamtime = 60;
        public static int spamcountreset = 5;

        public static string ZallState = "Alive";

        //public static string[] userMOTD;

        public static string level = "main";
        public static string errlog = "error.log";

        //        public static bool console = false; // never used
        public static bool reportBack = true;

        public static bool irc = false;
        public static bool ircColorsEnable = true;
        //        public static bool safemode = false; //Never used
        public static int ircPort = 6667;
        public static string ircNick = "ForgeBot";
        public static string ircServer = "irc.geekshed.net";
        public static string ircChannel = "#changethis";
        public static string ircOpChannel = "#changethistoo";
        public static bool ircIdentify = false;
        public static string ircPassword = "";
        public static bool verifyadmins = true;
        public static LevelPermission verifyadminsrank = LevelPermission.Operator;

        public static bool restartOnError = true;
        public static int Overload = 1500;
        public static int rpLimit = 500;
        public static int rpNormLimit = 10000;

        public static int backupInterval = 300;
        public static int blockInterval = 60;
        public static string backupLocation = Application.StartupPath + "/levels/backups";

        public static bool physicsRestart = true;
        public static bool deathcount = true;
        public static bool AutoLoad = false;
        public static int physUndo = 20000;
        public static int totalUndo = 200;
        public static bool rankSuper = true;
        public static bool oldHelp = false;
        public static bool parseSmiley = true;
        public static bool useWhitelist = false;
        public static bool PremiumPlayersOnly = false;
        public static bool forceCuboid = false;
        public static bool profanityFilter = false;
        public static bool notifyOnJoinLeave = false;
        public static bool repeatMessage = false;
        public static bool globalignoreops = false;

        public static bool checkUpdates = true;

        public static bool useMySQL = false;
        public static string MySQLHost = "127.0.0.1";
        public static string MySQLPort = "3306";
        public static string MySQLUsername = "root";
        public static string MySQLPassword = "password";
        public static string MySQLDatabaseName = "MCZallDB";
        public static bool DatabasePooling = true;

        public static string DefaultColor = "&e";
        public static string IRCColour = "&5";

        public static bool UseGlobalChat = true;
        public static string GlobalChatNick()
        {
            string serverName = Server.name.Replace(" ", "").Replace("'", "").Replace("!", "");;
            if(serverName.Length > 28)
            { 
                serverName.Substring(0, 28);
            }
            return serverName;
        }
        public static string GlobalChatColor = "&6";


        public static int afkminutes = 10;
        public static int afkkick = 45;
        public static LevelPermission afkkickperm = LevelPermission.AdvBuilder;
        //public static int RemotePort = 1337; // Never used

        public static string defaultRank = "guest";

        public static bool dollardollardollar = true;
        public static bool unsafe_plugin = true;
        public static bool cheapMessage = true;
        public static string cheapMessageGiven = " is now being cheap and being immortal";
        public static bool customBan = false;
        public static string customBanMessage = "You're banned!";
        public static bool customShutdown = false;
        public static string customShutdownMessage = "Server shutdown. Rejoin in 10 seconds.";
        public static bool customGrieferStone = false;
        public static string customGrieferStoneMessage = "Oh noes! You were caught griefing!";
        public static string customPromoteMessage = "&6Congratulations for working hard and getting &2PROMOTED!";
        public static string customDemoteMessage = "&4DEMOTED! &6We're sorry for your loss. Good luck on your future endeavors! &1:'(";
        public static string moneys = "moneys";
        public static LevelPermission opchatperm = LevelPermission.Operator;
        public static LevelPermission adminchatperm = LevelPermission.Admin;
        public static bool logbeat = false;
        public static bool adminsjoinsilent = false;
        public static bool mono { get { return (Type.GetType("Mono.Runtime") != null); } }
        public static string server_owner = "Notch";
        public static bool UseSeasons = false;
        public static bool guestLimitNotify = false;
        public static bool guestJoinNotify = true;
        public static bool guestLeaveNotify = true;
        public static string defaultTerrainUrl = "", defaultTexturePackUrl = "";

        public static bool flipHead = false;

        public static bool shuttingDown = false;
        public static bool restarting = false;

        //hackrank stuff
        public static bool hackrank_kick = true;
        public static int hackrank_kick_time = 5; //seconds, it converts it to milliseconds in the command.

        public static bool showEmptyRanks = false;

        //reviewoptions intitialize
        public static int reviewcooldown = 600;
        public static LevelPermission reviewenter = LevelPermission.Guest;
        public static LevelPermission reviewleave = LevelPermission.Guest;
        public static LevelPermission reviewview = LevelPermission.Operator;
        public static LevelPermission reviewnext = LevelPermission.Operator;
        public static LevelPermission reviewclear = LevelPermission.Operator;

        public static int DrawReloadLimit = 10000;
        #endregion

        public static MainLoop ml;
        public static Server s;
        public Server()
        {
            ml = new MainLoop("server");
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
        
        public void Start() {
            PlayerInfo.players = new List<Player>();
            Player.players = PlayerInfo.players;
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

            //UpdateGlobalSettings();
            if (!Directory.Exists("properties")) Directory.CreateDirectory("properties");
            if (!Directory.Exists("levels")) Directory.CreateDirectory("levels");
            if (!Directory.Exists("bots")) Directory.CreateDirectory("bots");
            if (!Directory.Exists("text")) Directory.CreateDirectory("text");
            if (!File.Exists("text/tempranks.txt")) File.CreateText("text/tempranks.txt").Dispose();
            if (!File.Exists("text/rankinfo.txt")) File.CreateText("text/rankinfo.txt").Dispose();
            if (!File.Exists("text/transexceptions.txt")) File.CreateText("text/transexceptions.txt").Dispose();
            if (!File.Exists("text/gcaccepted.txt")) File.CreateText("text/gcaccepted.txt").Dispose();
            if (!File.Exists("text/bans.txt")) File.CreateText("text/bans.txt").Dispose();
            // DO NOT STICK ANYTHING IN BETWEEN HERE!!!!!!!!!!!!!!!
            else
            {
                string bantext = File.ReadAllText("text/bans.txt");
                if (!bantext.Contains("%20") && bantext != "")
                {
                    bantext = bantext.Replace("~", "%20");
                    bantext = bantext.Replace("-", "%20");
                    File.WriteAllText("text/bans.txt", bantext);
                }
            }



            if (!Directory.Exists("extra")) Directory.CreateDirectory("extra");
            if (!Directory.Exists("extra/undo")) Directory.CreateDirectory("extra/undo");
            if (!Directory.Exists("extra/undoPrevious")) Directory.CreateDirectory("extra/undoPrevious");
            if (!Directory.Exists("extra/copy/")) Directory.CreateDirectory("extra/copy/");
            if (!Directory.Exists("extra/copyBackup/")) Directory.CreateDirectory("extra/copyBackup/");
            if (!Directory.Exists("extra/Waypoints")) Directory.CreateDirectory("extra/Waypoints");
            if (!Directory.Exists("blockdefs")) Directory.CreateDirectory("blockdefs");

            try
            {
                if (File.Exists("blocks.json")) File.Move("blocks.json", "blockdefs/global.json");
                if (File.Exists("server.properties")) File.Move("server.properties", "properties/server.properties");
                if (File.Exists("rules.txt")) File.Move("rules.txt", "text/rules.txt");
                if (File.Exists("welcome.txt")) File.Move("welcome.txt", "text/welcome.txt");
                if (File.Exists("messages.txt")) File.Move("messages.txt", "text/messages.txt");
                if (File.Exists("externalurl.txt")) File.Move("externalurl.txt", "text/externalurl.txt");
                if (File.Exists("autoload.txt")) File.Move("autoload.txt", "text/autoload.txt");
                if (File.Exists("IRC_Controllers.txt")) File.Move("IRC_Controllers.txt", "ranks/IRC_Controllers.txt");
                if (useWhitelist) if (File.Exists("whitelist.txt")) File.Move("whitelist.txt", "ranks/whitelist.txt");
            }
            catch { }

            if (File.Exists("text/custom$s.txt"))
            {
                using (StreamReader r = new StreamReader("text/custom$s.txt"))
                {
                    string line;
                    while ((line = r.ReadLine()) != null)
                    {
                        if (line.StartsWith("//")) continue;
                        var split = line.Split(new[] { ':' }, 2);
                        if (split.Length == 2 && !String.IsNullOrEmpty(split[0]))
                        {
                            customdollars.Add(split[0], split[1]);
                        }
                    }
                }
            }
            else
            {
                s.Log("custom$s.txt does not exist, creating");
                using (StreamWriter SW = File.CreateText("text/custom$s.txt"))
                {
                    SW.WriteLine("// This is used to create custom $s");
                    SW.WriteLine("// If you start the line with a // it wont be used");
                    SW.WriteLine("// It should be formatted like this:");
                    SW.WriteLine("// $website:mcgalaxy.ml");
                    SW.WriteLine("// That would replace '$website' in any message to 'mcgalaxy.ml'");
                    SW.WriteLine("// It must not start with a // and it must not have a space between the 2 sides and the colon (:)");
                    SW.Close();
                }
            }

            if (File.Exists("text/emotelist.txt"))
            {
                foreach (string s in File.ReadAllLines("text/emotelist.txt"))
                {
                    Player.emoteList.Add(s);
                }
            }
            else
            {
                File.Create("text/emotelist.txt").Dispose();
            }


            lava = new LavaSurvival();
            zombie = new ZombieGame();
            Countdown = new CountdownGame();

            LoadAllSettings();

            //derp
            if (!Server.LevelList.Contains("#(Must be comma seperated, no spaces. Must have changing levels and use level list enabled.)"))
                Server.LevelList.Add("#(Must be comma seperated, no spaces. Must have changing levels and use level list enabled.)");

            // OmniBan
            omniban = new OmniBan();

            timeOnline = DateTime.Now;
            {//MYSQL stuff
                try
                {
                    Database.executeQuery("CREATE DATABASE if not exists `" + MySQLDatabaseName + "`", true); // works in both now, SQLite simply ignores this.
                }
                //catch (MySql.Data.MySqlClient.MySqlException e)
                //{
                //    Server.s.Log("MySQL settings have not been set! Many features will not be available if MySQL is not enabled");
                //  //  Server.ErrorLog(e);
                //}
                catch (Exception e)
                {
                    ErrorLog(e);
                    s.Log("MySQL settings have not been set! Please Setup using the properties window.");
                    //process.Kill();
                    return;
                }
                Database.executeQuery(string.Format("CREATE TABLE if not exists Players (ID INTEGER {0}AUTO{1}INCREMENT NOT NULL, Name TEXT, IP CHAR(15), FirstLogin DATETIME, LastLogin DATETIME, totalLogin MEDIUMINT, Title CHAR(20), TotalDeaths SMALLINT, Money MEDIUMINT UNSIGNED, totalBlocks BIGINT, totalCuboided BIGINT, totalKicked MEDIUMINT, TimeSpent VARCHAR(20), color VARCHAR(6), title_color VARCHAR(6){2});", (useMySQL ? "" : "PRIMARY KEY "), (useMySQL ? "_" : ""), (Server.useMySQL ? ", PRIMARY KEY (ID)" : "")));
                Database.executeQuery(string.Format("CREATE TABLE if not exists Opstats (ID INTEGER {0}AUTO{1}INCREMENT NOT NULL, Time DATETIME, Name TEXT, Cmd VARCHAR(40), Cmdmsg VARCHAR(40){2});", (useMySQL ? "" : "PRIMARY KEY "), (useMySQL ? "_" : ""), (Server.useMySQL ? ", PRIMARY KEY (ID)" : "")));
                if (!File.Exists("extra/alter.txt") && Server.useMySQL) {
                	Database.executeQuery("ALTER TABLE Players MODIFY Name TEXT");
                	Database.executeQuery("ALTER TABLE Opstats MODIFY Name TEXT");
                	File.Create("extra/alter.txt");
                }
                //since 5.5.11 we are cleaning up the table Playercmds
                string query = Server.useMySQL ? "SHOW TABLES LIKE 'Playercmds'" : "SELECT name FROM sqlite_master WHERE type='table' AND name='Playercmds';";
                DataTable playercmds = Database.fillData(query); DataTable opstats = Database.fillData("SELECT * FROM Opstats");
                //if Playercmds exists copy-filter to Ostats and remove Playercmds
                if (playercmds.Rows.Count != 0) {
                    foreach (string cmd in Server.Opstats)
                        Database.executeQuery(string.Format("INSERT INTO Opstats (Time, Name, Cmd, Cmdmsg) SELECT Time, Name, Cmd, Cmdmsg FROM Playercmds WHERE cmd = '{0}';", cmd));
                    Database.executeQuery("INSERT INTO Opstats (Time, Name, Cmd, Cmdmsg) SELECT Time, Name, Cmd, Cmdmsg FROM Playercmds WHERE cmd = 'review' AND cmdmsg = 'next';");
                    Database.fillData("DROP TABLE Playercmds");
                }
                playercmds.Dispose(); opstats.Dispose();

                // Here, since SQLite is a NEW thing from 5.3.0.0, we do not have to check for existing tables in SQLite.
                if (useMySQL)
                {
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
            }

            Economy.LoadDatabase();
            UpdateStaffList();

            if (levels != null)
                foreach (Level l in levels) { l.Unload(); }
            ml.Queue(delegate
            {
                try
                {
                    levels = new List<Level>(maps);

                    if (LevelInfo.ExistsOffline(level))
                    {
                        mainLevel = Level.Load(level);
                        mainLevel.unload = false;
                        if (mainLevel == null)
                        {
                            if (File.Exists("levels/" + level + ".lvl.backup"))
                            {
                                Log("Attempting to load backup of " + level + ".");
                                File.Copy("levels/" + level + ".lvl.backup", "levels/" + level + ".lvl", true);
                                mainLevel = Level.Load(level);
                                if (mainLevel == null)
                                {
                                    Log("BACKUP FAILED!");
                                    Console.ReadLine(); return;
                                }
                            }
                            else
                            {
                                Log("mainlevel not found");
                                mainLevel = new Level(level, 128, 64, 128, "flat") { permissionvisit = LevelPermission.Guest, permissionbuild = LevelPermission.Guest };
                                mainLevel.Save();
                                Level.CreateLeveldb(level);
                            }
                        }
                    }
                    else
                    {
                        Log("mainlevel not found");
                        mainLevel = new Level(level, 128, 64, 128, "flat") { permissionvisit = LevelPermission.Guest, permissionbuild = LevelPermission.Guest };
                        mainLevel.Save();
                        Level.CreateLeveldb(level);
                    }

                    addLevel(mainLevel);

                    // fenderrock - Make sure the level does have a physics thread
                    if (mainLevel.physThread == null)
                        mainLevel.StartPhysics();
                }
                catch (Exception e) { ErrorLog(e); }
            });
            Plugin.Load();
            ml.Queue(delegate
            {
                bannedIP = PlayerList.Load("banned-ip.txt", null);
                ircControllers = PlayerList.Load("IRC_Controllers.txt", null);
                muted = PlayerList.Load("muted.txt", null);

                foreach (Group grp in Group.GroupList)
                    grp.playerList = PlayerList.Load(grp.fileName, grp);
                if (useWhitelist)
                    whiteList = PlayerList.Load("whitelist.txt", null);
                if (!File.Exists("ranks/jailed.txt")) { File.Create("ranks/jailed.txt").Close(); Server.s.Log("CREATED NEW: ranks/jailed.txt"); }
                Extensions.UncapitalizeAll("ranks/banned.txt");
                Extensions.UncapitalizeAll("ranks/muted.txt");
            });

            ml.Queue(delegate
            {
                if (File.Exists("text/autoload.txt"))
                {
                    try
                    {
                        string[] lines = File.ReadAllLines("text/autoload.txt");
                        foreach (string _line in lines.Select(line => line.Trim()))
                        {
                            try
                            {
                                if (_line == "") { continue; }
                                if (_line[0] == '#') { continue; }

                                string key = _line.Split('=')[0].Trim();
                                string value;
                                try
                                {
                                    value = _line.Split('=')[1].Trim();
                                }
                                catch
                                {
                                    value = "0";
                                }

                                if (!key.Equals(mainLevel.name))
                                {
                                    Command.all.Find("load").Use(null, key + " " + value);
                                    Level l = LevelInfo.FindExact(key);
                                }
                                else
                                {
                                    try
                                    {
                                        int temp = int.Parse(value);
                                        if (temp >= 0 && temp <= 3)
                                        {
                                            mainLevel.setPhysics(temp);
                                        }
                                    }
                                    catch
                                    {
                                        s.Log("Physics variable invalid");
                                    }
                                }


                            }
                            catch
                            {
                                s.Log(_line + " failed.");
                            }
                        }
                    }
                    catch
                    {
                        s.Log("autoload.txt error");
                    }
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                }
                else
                {
                    Log("autoload.txt does not exist");
                }
            });

            ml.Queue(delegate
            {
                foreach (string line in File.ReadAllLines("text/gcaccepted.txt"))
                {
                    gcaccepted.Add(line); //loading all playernames of people who turned off translation
                }
                Log("Creating listening socket on port " + port + "... ");
                Setup();
                //s.Log(Setup() ? "Done." : "Could not create socket connection. Shutting down.");
            });

            ml.Queue(delegate
            {
                updateTimer.Elapsed += delegate
                {
                    Player.GlobalUpdate();
                    PlayerBot.GlobalUpdatePosition();
                };

                updateTimer.Start();
            });


            // Heartbeat code here:

            ml.Queue(delegate
            {
                try
                {
                    Heart.Init();
                }
                catch (Exception e)
                {
                    Server.ErrorLog(e);
                }
            });

            ml.Queue(delegate
            {
                messageTimer.Elapsed += delegate
                {
                    RandomMessage();
                };
                messageTimer.Start();

                if (File.Exists("text/messages.txt"))
                {
                    using (StreamReader r = File.OpenText("text/messages.txt"))
                    {
                        while (!r.EndOfStream)
                            messages.Add(r.ReadLine());
                    }
                }
                else File.Create("text/messages.txt").Close();

                try
                {
                    APIServer = new WebServer(SendResponse, "http://localhost:8080/api/");
                    APIServer.Run();
                    InfoServer = new WebServer(WhoIsResponse, "http://localhost:8080/whois/");
                    InfoServer.Run();
                }
                catch
                {
                    Server.s.Log("Failed to start local API server");
                }

                IRC = new ForgeBot(Server.ircChannel, Server.ircOpChannel, Server.ircNick, Server.ircServer);
                GlobalChat = new GlobalChatBot(GlobalChatNick());

                if (Server.irc) IRC.Connect();
                if (Server.UseGlobalChat) GlobalChat.Connect();

                // OmniBan stuff!
                new Thread(new ThreadStart(() => omniban.Load(true))).Start();

                omnibanCheckTimer.Elapsed += delegate
                {
                    omniban.Load(true);
                    omniban.KickAll();
                };
                omnibanCheckTimer.Start();


                new AutoSaver(Server.backupInterval);

                blockThread = new Thread(new ThreadStart(delegate
                {
                    while (true)
                    {
                        Thread.Sleep(blockInterval * 1000);
                        try {
                            levels.ForEach(
                                delegate(Level l) {
                                    try {
                                        l.saveChanges();
                                    } catch (Exception e) {
                                        Server.ErrorLog(e);
                                    }
                                });
                        } catch (Exception e) {
                            // an exception is raised on Mono if level list is modified
                            // while enumerating over it with ForEach
                            Server.ErrorLog(e);
                        }
                    }
                }));
                blockThread.Name = "MCG_BlockUpdates";
                blockThread.Start();

                locationChecker = new Thread(new ThreadStart(delegate
                {
                    Player p, who;
                    ushort x, y, z;
                    int i;
                    while (true)
                    {
                        Thread.Sleep(3);
                        for (i = 0; i < PlayerInfo.players.Count; i++)
                        {
                            try
                            {
                                p = PlayerInfo.players[i];

                                if (p.frozen)
                                {
                                    p.SendPos(0xFF, p.pos[0], p.pos[1], p.pos[2], p.rot[0], p.rot[1]); continue;
                                }
                                else if (p.following != "")
                                {
                                    who = PlayerInfo.Find(p.following);
                                    if (who == null || who.level != p.level)
                                    {
                                        p.following = "";
                                        if (!p.canBuild)
                                        {
                                            p.canBuild = true;
                                        }
                                        if (who != null && who.possess == p.name)
                                        {
                                            who.possess = "";
                                        }
                                        continue;
                                    }
                                    if (p.canBuild)
                                    {
                                        p.SendPos(0xFF, who.pos[0], (ushort)(who.pos[1] - 16), who.pos[2], who.rot[0], who.rot[1]);
                                    }
                                    else
                                    {
                                       p.SendPos(0xFF, who.pos[0], who.pos[1], who.pos[2], who.rot[0], who.rot[1]);
                                    }
                                }
                                else if (p.possess != "")
                                {
                                    who = PlayerInfo.Find(p.possess);
                                    if (who == null || who.level != p.level)
                                        p.possess = "";
                                }

                                x = (ushort)(p.pos[0] / 32);
                                y = (ushort)(p.pos[1] / 32);
                                z = (ushort)(p.pos[2] / 32);

                                if (p.level.Death)
                                    p.CheckSurvival(x, y, z);
                                p.CheckBlock(x, y, z);
                                p.oldIndex = p.level.PosToInt(x, y, z);
                            }
                            catch (Exception e) { Server.ErrorLog(e); }
                        }
                    }
                }));
                locationChecker.Name = "MCG_LocationCheck";
                locationChecker.Start();
#if DEBUG
	  UseTextures = true;          
#endif
                Log("Finished setting up server");
                ServerSetupFinished = true;
                Checktimer.StartTimer();
                Commands.CommandKeywords.SetKeyWords();
                try
                {
                    if (Server.lava.startOnStartup)
                        Server.lava.Start();
                    if (Server.startZombieModeOnStartup)
                        Server.zombie.StartGame(1, 0);
                    //This doesnt use the main map
                    if (Server.UseCTF)
                        ctf = new Auto_CTF();
                }
                catch (Exception e) { Server.ErrorLog(e); }
                BlockQueue.Start();
            });
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
                if (whois.rank.Contains("banned"))
                    whois.banned = true;
                else
                    whois.banned = Ban.IsBanned(p);
                string[] bandata;
                if (whois.banned)
                {
                    bandata = Ban.GetBanData(p);
                    whois.banned_by = bandata[0];
                    whois.ban_reason = bandata[1].Replace("%20", " ");
                    whois.banned_time = bandata[2].Replace("%20", " ");
                }
                
                return JsonConvert.SerializeObject(whois, Formatting.Indented);
            }
            catch(Exception e)
            {
                Logger.WriteError(e);
            }
            return "Error";
        }
        public static void LoadAllSettings()
        {
            Colors.LoadExtColors();
            BlockDefinition.LoadGlobal();
            SrvProperties.Load("properties/server.properties");
            Updater.Load("properties/update.properties");
            Group.InitAll();
            Command.InitAll();
            GrpCommands.fillRanks();
            Block.SetBlocks();
            Awards.Load();
            Economy.Load();
            Warp.LOAD();
            CommandOtherPerms.Load();
            ProfanityFilter.Init();
            Alias.Load();
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
            List<string> players = new List<string>();
            foreach (Player p in PlayerInfo.players) { p.save(); players.Add(p.name); }
            foreach (string p in players)
            {
                if (!AutoRestart)
                    PlayerInfo.Find(p).Kick(Server.customShutdown ? Server.customShutdownMessage : "Server shutdown. Rejoin in 10 seconds.");
                else
                    PlayerInfo.Find(p).Kick("Server restarted! Rejoin!");
            }
            APIServer.Stop();
            InfoServer.Stop();
            //PlayerInfo.players.ForEach(delegate(Player p) { p.Kick("Server shutdown. Rejoin in 10 seconds."); });
            Player.connections.ForEach(
            delegate(Player p)
            {
                if (!AutoRestart)
                    p.Kick(Server.customShutdown ? Server.customShutdownMessage : "Server shutdown. Rejoin in 10 seconds.");
                else
                    p.Kick("Server restarted! Rejoin!");
            }
            );
            Plugin.Unload();
            if (listen != null)
            {
                listen.Close();
            }
            try
            {
                GlobalChat.Disconnect(!AutoRestart ? "Server is shutting down." : "Server is restarting.");
            }
            catch { }
            try
            {
                IRC.Disconnect(!AutoRestart ? "Server is shutting down." : "Server is restarting.");
            }
            catch { }
        }

        public static void addLevel(Level level)
        {
            levels.Add(level);
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
            if (OnLog != null)
            {
                if (!systemMsg)
                {
                    OnLog(DateTime.Now.ToString("(HH:mm:ss) ") + message);
                }
                else
                {
                    OnSystem(DateTime.Now.ToString("(HH:mm:ss) ") + message);
                }
            }

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
        /*public static void UpdateGlobalSettings()
        {
            try
            {
                gcipbans.Clear();
                gcnamebans.Clear();
                JArray jason; //jason plz (troll)
                using (var client = new WebClient()) {
                    jason = JArray.Parse(client.DownloadString("https://raw.githubusercontent.com/Hetal728/MCGalaxy/master/Uploads/gcbanned.txt"));
                }
                foreach (JObject ban in jason) {
                    if((string)ban["banned_isIp"] == "0")
                        gcnamebans.Add(((string)ban["banned_name"]).ToLower(), "'" + (string)ban["banned_by"] + "', because: %d" + (string)ban["banned_reason"]);
                    else if((string)ban["banned_isIp"] == "1")
                        gcipbans.Add((string)ban["banned_name"], "'" + (string)ban["banned_by"] + "', because: %d" + (string)ban["banned_reason"]);
                }
                s.Log("GlobalChat Banlist updated!");
            }
            catch (Exception e)
            {
                ErrorLog(e);
                s.Log("Could not update GlobalChat Banlist!");
                gcnamebans.Clear();
                gcipbans.Clear();
            }
        }*/
        public void UpdateStaffList() {
            try {
                devs.Clear();
                mods.Clear();
                gcmods.Clear();
                using (WebClient web = new WebClient()) {
                    string[] result = web.DownloadString("https://raw.githubusercontent.com/Hetal728/MCGalaxy/master/Uploads/devs.txt").Split(new string[] { Environment.NewLine, "\n" }, StringSplitOptions.None);
                    foreach (string line in result) {
                        string type = line.Split(':')[0].ToLower();
                        List<string> staffList = type.Equals("devs") ? devs : type.Equals("mods") ? mods : type.Equals("gcmods") ? gcmods : null;
                        foreach (string name in line.Split(':')[1].Split())
                            staffList.Add(name.ToLower());
                    }
                }
            } catch (Exception e) {
                ErrorLog(e);
                s.Log("Couldn't update MCGalaxy staff list, turning MCGalaxy Staff Protection Level off. . . ");
                devs.Clear();
                mods.Clear();
                gcmods.Clear();
            }
        }

        public static bool canusegc = true; //badpokerface
        public static int gcmultiwarns = 0, gcspamcount = 0, gccapscount = 0, gcfloodcount = 0;
        public static DateTime gclastmsgtime = DateTime.MinValue;
        public static string gclastmsg = "";
    }
}