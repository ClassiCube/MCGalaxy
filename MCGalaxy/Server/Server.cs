/*
    Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCForge)
    
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
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Threading;
using MCGalaxy.Authentication;
using MCGalaxy.Blocks;
using MCGalaxy.Commands;
using MCGalaxy.DB;
using MCGalaxy.Drawing;
using MCGalaxy.Eco;
using MCGalaxy.Events.ServerEvents;
using MCGalaxy.Games;
using MCGalaxy.Network;
using MCGalaxy.Scripting;
using MCGalaxy.SQL;
using MCGalaxy.Tasks;
using MCGalaxy.Util;
using MCGalaxy.Modules.Awards;

namespace MCGalaxy 
{
    public sealed partial class Server 
    {
        public Server() { Server.s = this; }
        
        //True = cancel event
        //Fale = dont cacnel event
        public static bool Check(string cmd, string message) {
            if (ConsoleCommand != null) ConsoleCommand(cmd, message);
            return cancelcommand;
        }
        
        [Obsolete("Use Logger.Log(LogType, String)")]
        public void Log(string message) { Logger.Log(LogType.SystemActivity, message); }
        
        [Obsolete("Use Logger.Log(LogType, String)")]
        public void Log(string message, bool systemMsg = false) {
            LogType type = systemMsg ? LogType.BackgroundActivity : LogType.SystemActivity;
            Logger.Log(type, message);
        }
        
        public static void CheckFile(string file) {
            if (File.Exists(file)) return;
            
            Logger.Log(LogType.SystemActivity, file + " doesn't exist, Downloading..");
            try {
                using (WebClient client = HttpUtil.CreateWebClient()) {
                    client.DownloadFile(Updater.BaseURL + file, file);
                }
                if (File.Exists(file)) {
                    Logger.Log(LogType.SystemActivity, file + " download succesful!");
                }
            } catch (Exception ex) {
                Logger.LogError("Downloading " + file +" failed, try again later", ex);
            }
        }
        
        internal static ConfigElement[] serverConfig, levelConfig, zoneConfig;
        public static void Start() {
            serverConfig = ConfigElement.GetAll(typeof(ServerConfig));
            levelConfig  = ConfigElement.GetAll(typeof(LevelConfig));
            zoneConfig   = ConfigElement.GetAll(typeof(ZoneConfig));

            IOperatingSystem.DetectOS().Init();
            #pragma warning disable 0618
            Player.players = PlayerInfo.Online.list;
            #pragma warning restore 0618
            
            StartTime = DateTime.UtcNow;
            Logger.Log(LogType.SystemActivity, "Starting Server");
            ServicePointManager.Expect100Continue = false;
            ForceEnableTLS();

            SQLiteBackend.Instance.LoadDependencies();
#if !MCG_STANDALONE
            MySQLBackend.Instance.LoadDependencies();
#endif

            EnsureFilesExist();
            IScripting.Init();

            LoadAllSettings(true);
            InitDatabase();
            Economy.LoadDatabase();

            Background.QueueOnce(LoadMainLevel);
            Background.QueueOnce(LoadAllPlugins);
            Background.QueueOnce(LoadAutoloadMaps);
            Background.QueueOnce(UpgradeTasks.UpgradeOldTempranks);
            Background.QueueOnce(UpgradeTasks.UpgradeDBTimeSpent);
            Background.QueueOnce(InitPlayerLists);
            
            Background.QueueOnce(SetupSocket);
            Background.QueueOnce(InitTimers);
            Background.QueueOnce(InitRest);
            Background.QueueOnce(InitHeartbeat);

            ServerTasks.QueueTasks();
            Background.QueueRepeat(ThreadSafeCache.DBCache.CleanupTask,
                                   null, TimeSpan.FromMinutes(5));
        }
        
        static void ForceEnableTLS() {
            // Force enable TLS 1.1/1.2, otherwise checking for updates on Github doesn't work
            try { ServicePointManager.SecurityProtocol |= (SecurityProtocolType)0x300; } catch { }
            try { ServicePointManager.SecurityProtocol |= (SecurityProtocolType)0xC00; } catch { }
        }
        
        static void EnsureFilesExist() {
            EnsureDirectoryExists("properties");
            EnsureDirectoryExists("levels");
            EnsureDirectoryExists("bots");
            EnsureDirectoryExists("text");
            EnsureDirectoryExists("ranks");
            RankInfo.EnsureExists();
            Ban.EnsureExists();
            PlayerDB.EnsureDirectoriesExist();

            EnsureDirectoryExists("extra");
            EnsureDirectoryExists(Paths.WaypointsDir);
            EnsureDirectoryExists("extra/bots");
            EnsureDirectoryExists(Paths.ImportsDir);
            EnsureDirectoryExists("blockdefs");
#if !MCG_STANDALONE
            EnsureDirectoryExists(MCGalaxy.Modules.Compiling.ICompiler.COMMANDS_SOURCE_DIR); // TODO move to compiling module
#endif
            EnsureDirectoryExists("text/discord"); // TODO move to discord plugin
        }
        
        static void EnsureDirectoryExists(string dir) {
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
        }     
        
        public static void LoadAllSettings() { LoadAllSettings(false); }
        
        // TODO rethink this
        static void LoadAllSettings(bool commands) {
            Colors.Load();
            Alias.LoadCustom();
            BlockDefinition.LoadGlobal();
            ImagePalette.Load();
            
            SrvProperties.Load();
            if (commands) Command.InitAll();
            AuthService.ReloadDefault();
            Group.LoadAll();
            CommandPerms.Load();
            Block.SetBlocks();
            BlockPerms.Load();
            AwardsList.Load();
            PlayerAwards.Load();
            Economy.Load();
            WarpList.Global.Filename = "extra/warps.save";
            WarpList.Global.Load();
            CommandExtraPerms.Load();
            ProfanityFilter.Init();
            Team.LoadList();
            ChatTokens.LoadCustom();
            SrvProperties.FixupOldPerms();
            CpeExtension.LoadDisabledList();
            
            TextFile announcementsFile = TextFile.Files["Announcements"];
            announcementsFile.EnsureExists();
            announcements = announcementsFile.GetText();
            
            OnConfigUpdatedEvent.Call();
        }
        

        static readonly object stopLock = new object();
        static volatile Thread stopThread;
        public static Thread Stop(bool restart, string msg) {
            Server.shuttingDown = true;
            lock (stopLock) {
                if (stopThread != null) return stopThread;
                stopThread = new Thread(() => ShutdownThread(restart, msg));
                stopThread.Start();
                return stopThread;
            }
        }
        
        static void ShutdownThread(bool restarting, string msg) {
            try {
                Logger.Log(LogType.SystemActivity, "Server shutting down ({0})", msg);
            } catch { }
            
            // Stop accepting new connections and disconnect existing sessions
            Listener.Close();            
            try {
                Player[] players = PlayerInfo.Online.Items;
                foreach (Player p in players) { p.Leave(msg); }
            } catch (Exception ex) { Logger.LogError(ex); }
            
            byte[] kick = Packet.Kick(msg, false);
            try {
                INetSocket[] pending = INetSocket.pending.Items;
                foreach (INetSocket p in pending) { p.Send(kick, SendFlags.None); }
            } catch (Exception ex) { Logger.LogError(ex); }

            OnShuttingDownEvent.Call(restarting, msg);
            Plugin.UnloadAll();

            try {
                string autoload = SaveAllLevels();
                if (Server.SetupFinished && !Server.Config.AutoLoadMaps) {
                    File.WriteAllText("text/autoload.txt", autoload);
                }
            } catch (Exception ex) { Logger.LogError(ex); }
            
            try {
                Logger.Log(LogType.SystemActivity, "Server shutdown completed");
            } catch { }
            
            try { FileLogger.Flush(null); } catch { }
            
            if (restarting) {
                // first try to use excevp to restart in CLI mode under mono 
                // - see detailed comment in HACK_Execvp for why this is required
                IOperatingSystem.DetectOS().RestartProcess();

                Process.Start(GetRestartPath());
            }
            Environment.Exit(0);
        }

        public static string SaveAllLevels() {
            string autoload = null;
            Level[] loaded  = LevelInfo.Loaded.Items;

            foreach (Level lvl in loaded)
            {
                if (!lvl.SaveChanges) continue;

                autoload = autoload + lvl.name + "=" + lvl.physics + Environment.NewLine;
                lvl.Save();
                lvl.SaveBlockDBChanges();
            }
            return autoload;
        }


        public static string GetServerDLLPath() {
            return Assembly.GetExecutingAssembly().Location;
        }

        public static string GetRestartPath() {
#if !NETSTANDARD
            return RestartPath;
#else
            // NET core/5/6 executables tend to use the following structure:
            //   MCGalaxyCLI_core --> MCGalaxyCLI_core.dll
            // in this case, 'RestartPath' will include '.dll' since this file
            //  is actually the managed assembly, but we need to remove '.dll'
            //   as the actual executable which must be started is the non .dll file
            string path = RestartPath;
            if (path.CaselessEnds(".dll")) path = path.Substring(0, path.Length - 4);
            return path;
#endif
        }

        static bool checkedOnMono, runningOnMono;
        public static bool RunningOnMono() {
            if (!checkedOnMono) {
                runningOnMono = Type.GetType("Mono.Runtime") != null;
                checkedOnMono = true;
            }
            return runningOnMono;
        }


        public static void UpdateUrl(string url) {
            if (OnURLChange != null) OnURLChange(url);
        }

        static void RandomMessage(SchedulerTask task) {
            if (PlayerInfo.Online.Count > 0 && announcements.Length > 0) {
                Chat.MessageGlobal(announcements[new Random().Next(0, announcements.Length)]);
            }
        }

        internal static void SettingsUpdate() {
            if (OnSettingsUpdate != null) OnSettingsUpdate();
        }
        
        public static bool SetMainLevel(string map) {
            string main = mainLevel != null ? mainLevel.name : Server.Config.MainLevel;
            if (map.CaselessEq(main)) return false;
            
            Level lvl = LevelInfo.FindExact(map);
            if (lvl == null)
                lvl = LevelActions.Load(Player.Console, map, false);
            if (lvl == null) return false;
            
            SetMainLevel(lvl); return true;
        }
        
        public static void SetMainLevel(Level lvl) {
            Level oldMain = mainLevel;
            mainLevel = lvl;
            Server.Config.MainLevel = lvl.name;         
            oldMain.AutoUnload();
        }
        
        public static void DoGC() {
            var sw = Stopwatch.StartNew();
            long start = GC.GetTotalMemory(false);
            GC.Collect();
            GC.WaitForPendingFinalizers();
            
            long end = GC.GetTotalMemory(false);
            double deltaKB = (start - end) / 1024.0;
            if (deltaKB < 100.0) return;
            
            Logger.Log(LogType.BackgroundActivity, "GC performed in {0:F2} ms (tracking {1:F2} KB, freed {2:F2} KB)",
                       sw.Elapsed.TotalMilliseconds, end / 1024.0, deltaKB);
        }
        
        
        // only want ASCII alphanumerical characters for salt
        static bool AcceptableSaltChar(char c) {
            return (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') 
                || (c >= '0' && c <= '9');
        }
        
        /// <summary> Generates a random salt that is used for calculating mppasses. </summary>
        public static string GenerateSalt() {
            RandomNumberGenerator rng = RandomNumberGenerator.Create();
            char[] str = new char[32];
            byte[] one = new byte[1];
            
            for (int i = 0; i < str.Length; ) {
                rng.GetBytes(one);
                if (!AcceptableSaltChar((char)one[0])) continue;
                
                str[i] = (char)one[0]; i++;
            }
            return new string(str);
        }
        
        static System.Text.ASCIIEncoding enc = new System.Text.ASCIIEncoding();
        static MD5CryptoServiceProvider md5  = new MD5CryptoServiceProvider();
        static object md5Lock = new object();
        
        /// <summary> Calculates mppass (verification token) for the given username. </summary>
        public static string CalcMppass(string name, string salt) {
            byte[] hash = null;
            lock (md5Lock) hash = md5.ComputeHash(enc.GetBytes(salt + name));
            return BitConverter.ToString(hash).Replace("-", "");
        }
        
        /// <summary> Converts a formatted username into its original username </summary>
        /// <remarks> If ClassiCubeAccountPlus option is set, removes trailing + </remarks>
        public static string ToRawUsername(string name) {
            if (Config.ClassicubeAccountPlus)
                return name.RemoveLastPlus();
            return name;
        }

        /// <summary> Converts a username into its formatted username </summary>
        /// <remarks> If ClassiCubeAccountPlus option is set, adds trailing + </remarks>
        public static string FromRawUsername(string name) {
            if (!Config.ClassicubeAccountPlus) return name;

            if (!name.EndsWith("+")) name += "+";
            return name;
        }
    }
}