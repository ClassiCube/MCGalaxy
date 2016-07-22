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
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using MCGalaxy.Games;
using MCGalaxy.SQL;
using Newtonsoft.Json;

namespace MCGalaxy {
    public sealed partial class Server {
       
        public Server() {
            MainScheduler = new Scheduler("MCG_MainScheduler");
            Background = new Scheduler("MCG_BackgroundScheduler");
            Server.s = this;
        }
		
        //True = cancel event
        //Fale = dont cacnel event
        public static bool Check(string cmd, string message) {
            if (ConsoleCommand != null) ConsoleCommand(cmd, message);
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

            Background.QueueOnce(CombineEnvFiles);            
            Background.QueueOnce(LoadMainLevel);
            Plugin.Load();
            Background.QueueOnce(LoadPlayerLists);
            Background.QueueOnce(LoadAutoloadCommands);
            Background.QueueOnce(MovePreviousLevelFiles);

            Background.QueueOnce(SetupSocket);
            Background.QueueOnce(InitTimers);
            Background.QueueOnce(InitRest);
            Background.QueueOnce(InitHeartbeat);
            
            Devs.Clear();
            Mods.Clear();
            Background.QueueOnce(UpdateStaffListTask);
            
            Background.QueueRepeat(AutoSaveTask, 1, 
                                          TimeSpan.FromSeconds(Server.backupInterval));
            Background.QueueRepeat(BlockUpdatesTask, null,
                                          TimeSpan.FromSeconds(Server.blockInterval));
        }
        
        void EnsureFilesExist() {
            if (!Directory.Exists("properties")) Directory.CreateDirectory("properties");
            if (!Directory.Exists("levels")) Directory.CreateDirectory("levels");
            if (!Directory.Exists("bots")) Directory.CreateDirectory("bots");
            if (!Directory.Exists("text")) Directory.CreateDirectory("text");
            TempRanks.EnsureExists();
            RankInfo.EnsureExists();
            Ban.EnsureExists();

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
        
        public static string SendResponse(HttpListenerRequest request) {
            try {
                string api = "";
                API API = new API();
                API.max_players = (int)Server.players;
                API.players = PlayerInfo.players.Select(mc => mc.name).ToArray();
                API.chat = Player.Last50Chat.ToArray();
                api = JsonConvert.SerializeObject(API, Formatting.Indented);
                return api;
            } catch(Exception e) {
                Logger.WriteError(e);
            }
            return "Error";
        }
        
        public static string WhoIsResponse(HttpListenerRequest request) {
            try {
                string p = request.QueryString.Get("name");
                if (p == null || p == "")
                    return "Error";
                var whois = new WhoWas(p);
                Group grp = Group.Find(whois.rank);
                whois.banned = grp != null && grp.Permission == LevelPermission.Banned;
                
                if (whois.banned) {
                    string[] bandata = Ban.GetBanData(p);
                    whois.banned_by = bandata[0];
                    whois.ban_reason = bandata[1];
                    whois.banned_time = bandata[2];
                }
                return JsonConvert.SerializeObject(whois, Formatting.Indented);
            } catch(Exception e) {
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
            FixupOldReviewPerms();
        }
        
        static void FixupOldReviewPerms() {
            Command cmd = Command.all.Find("review");
            var perms = SrvProperties.reviewPerms;
            if (perms.clearPerm == -1 && perms.nextPerm == -1 && perms.viewPerm == -1) return;
            
            // Backwards compatibility with old config, where review permissions where global
            if (perms.viewPerm != -1)
                CommandOtherPerms.Edit(CommandOtherPerms.Find(cmd, 1), perms.viewPerm);
            if (perms.nextPerm != -1)
                CommandOtherPerms.Edit(CommandOtherPerms.Find(cmd, 2), perms.nextPerm);
            if (perms.clearPerm != -1)
                CommandOtherPerms.Edit(CommandOtherPerms.Find(cmd, 3), perms.clearPerm);
            CommandOtherPerms.Save();
        }

        public static void Setup() {
            try {
                IPEndPoint endpoint = new IPEndPoint(IPAddress.Any, port);
                listen = new Socket(endpoint.Address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                listen.Bind(endpoint);
                listen.Listen((int)SocketOptionName.MaxConnections);
                listen.BeginAccept(Accept, null);
            }
            catch (SocketException e) { ErrorLog(e); s.Log("Error Creating listener, socket shutting down"); }
            catch (Exception e) { ErrorLog(e); s.Log("Error Creating listener, socket shutting down"); }
        }

        static void Accept(IAsyncResult result) {
            if (shuttingDown) return;

            Player p = null;
            bool begin = false;
            try {
                p = new Player(listen.EndAccept(result));
                //new Thread(p.Start).Start();
                listen.BeginAccept(Accept, null);
                begin = true;
            } catch (SocketException) {
                if (p != null) p.Disconnect();
                if (!begin) listen.BeginAccept(Accept, null);
            } catch (Exception e) {
                ErrorLog(e);
                if (p != null) p.Disconnect();
                if (!begin) listen.BeginAccept(Accept, null);
            }

        }

        public static void Exit(bool AutoRestart) {
            string msg = AutoRestart ? "Server restarted. Sign in again and rejoin." : Server.shutdownMessage;
            Player[] players = PlayerInfo.Online.Items;
            foreach (Player p in players) { p.save(); }
            foreach (Player p in players) { p.Leave(msg); }
  
            if (APIServer != null) APIServer.Stop();
            if (InfoServer != null) InfoServer.Stop();

            Player.connections.ForEach(p => p.Leave(msg));
            Plugin.Unload();
            if (listen != null) listen.Close();
            try {
                IRC.Disconnect(!AutoRestart ? "Server is shutting down." : "Server is restarting.");
            } catch { 
            }
        }

        [Obsolete("Use LevelInfo.Loaded.Add()")]
        public static void addLevel(Level level) {
            LevelInfo.Loaded.Add(level);
        }

        public void PlayerListUpdate() {
            if (Server.s.OnPlayerListChange != null) Server.s.OnPlayerListChange(PlayerInfo.players);
        }

        public void FailBeat()  {
            if (HeartBeatFail != null) HeartBeatFail();
        }

        public void UpdateUrl(string url) {
            if (OnURLChange != null) OnURLChange(url);
        }

        public void Log(string message, bool systemMsg = false) {
            message = CP437Writer.ConvertToUnicode(message);
            if (ServerLog != null)  {
                ServerLog(message);
                if (cancellog) { cancellog = false; return; }
            }         
            if (!systemMsg) OnServerLogEvent.Call(message);
            
            string now = DateTime.Now.ToString("(HH:mm:ss) ");
            if (!systemMsg && OnLog != null) OnLog(now + message);
            if (systemMsg && OnSystem != null) OnSystem(now + message);
            Logger.Write(now + message + Environment.NewLine);
        }
        
        public void OpLog(string message, bool systemMsg = false) {
            message = CP437Writer.ConvertToUnicode(message);
            if (ServerOpLog != null) {
                ServerOpLog(message);
                if (canceloplog) { canceloplog = false; return; }
            }
            
            string now = DateTime.Now.ToString("(HH:mm:ss) ");
            if (OnOp != null) {
                if (!systemMsg) OnOp(now + message);
                else OnSystem(now + message);
            }
            Logger.Write(now + message + Environment.NewLine);
        }

        public void AdminLog(string message, bool systemMsg = false) {
            message = CP437Writer.ConvertToUnicode(message);
            if (ServerAdminLog != null) {
                ServerAdminLog(message);
                if (canceladmin) { canceladmin = false; return; }
            }
            
            string now = DateTime.Now.ToString("(HH:mm:ss) ");
            if (OnAdmin != null) {
                if (!systemMsg) OnAdmin(now + message);
                else OnSystem(now + message);
            }
            Logger.Write(now + message + Environment.NewLine);
        }

        public void ErrorCase(string message) {
            if (OnError != null) OnError(message);
        }

        public void CommandUsed(string message) {
            message = CP437Writer.ConvertToUnicode(message);
            string now = DateTime.Now.ToString("(HH:mm:ss) ");
            if (OnCommand != null) OnCommand(now + message);
            Logger.Write(now + message + Environment.NewLine);
        }

        public static void ErrorLog(Exception ex) {
            if (ServerError != null) ServerError(ex);
            OnServerErrorEvent.Call(ex);
            Logger.WriteError(ex);
            
            try {
                s.Log("!!!Error! See " + Logger.ErrorLogPath + " for more information.");
            } catch { 
            }
        }

        static void RandomMessage(SchedulerTask task) {
            if (Player.number != 0 && messages.Count > 0)
                Player.GlobalMessage(messages[new Random().Next(0, messages.Count)]);
        }

        internal void SettingsUpdate() {
            if (OnSettingsUpdate != null) OnSettingsUpdate();
        }

        public static string FindColor(string Username) {
            foreach (Group grp in Group.GroupList.Where(grp => grp.playerList.Contains(Username))) {
                return grp.color;
            }
            return Group.standard.color;
        }
    }
}