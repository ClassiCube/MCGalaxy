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
using System.Net;
using MCGalaxy.Commands.World;
using MCGalaxy.Games;
using MCGalaxy.Generator;
using MCGalaxy.Network;
using MCGalaxy.Tasks;
using MCGalaxy.Util;

namespace MCGalaxy {
    
    public sealed partial class Server {

        static void LoadMainLevel(SchedulerTask task) {
            try {
                mainLevel = CmdLoad.LoadLevel(null, ServerConfig.MainLevel);                
                if (mainLevel == null) GenerateMain();
                
                mainLevel.Config.AutoUnload = false;
                LevelInfo.Loaded.Add(mainLevel);
            } catch (Exception e) {
                Logger.LogError(e);
            }
        }
        
        static void GenerateMain() {
            Logger.Log(LogType.SystemActivity, "main level not found, generating..");
            mainLevel = new Level(ServerConfig.MainLevel, 128, 64, 128);
            MapGen.Generate(mainLevel, "flat", "", null);
            mainLevel.Save();
        }
        
        static void LoadPlayerLists(SchedulerTask task) {
            agreed = new PlayerList("ranks/agreed.txt");
            try {
                UpgradeTasks.UpgradeOldAgreed();
                agreed = PlayerList.Load("agreed.txt");
            } catch (Exception ex) {
                Logger.LogError(ex);
            }
            
            bannedIP = PlayerList.Load("banned-ip.txt");
            ircControllers = PlayerList.Load("IRC_Controllers.txt");
            hidden = PlayerList.Load("hidden.txt");
            vip = PlayerList.Load("text/vip.txt");
            noEmotes = PlayerList.Load("text/emotelist.txt");
            lockdown = PlayerList.Load("text/lockdown.txt");            
            jailed = PlayerExtList.Load("ranks/jailed.txt");
            models = PlayerExtList.Load("extra/models.txt");
            skins = PlayerExtList.Load("extra/skins.txt");
            reach = PlayerExtList.Load("extra/reach.txt");
            invalidIds = PlayerList.Load("extra/invalidids.txt");
            rotations = PlayerExtList.Load("extra/rotations.txt");

            muted = PlayerExtList.Load("ranks/muted.txt");
            frozen = PlayerExtList.Load("ranks/frozen.txt");            
            tempRanks = PlayerExtList.Load(Paths.TempRanksFile);
            tempBans = PlayerExtList.Load(Paths.TempBansFile);            
            ModerationTasks.QueueTasks();
            
            if (ServerConfig.WhitelistedOnly)
                whiteList = PlayerList.Load("whitelist.txt");
        }
        
        static void LoadAutoloadMaps(SchedulerTask task) {
            AutoloadMaps = PlayerExtList.Load("text/autoload.txt", '=');
            List<string> maps = AutoloadMaps.AllNames();
            
            foreach (string map in maps) {
                string name = map.ToLower();               
                if (name != mainLevel.name) CmdLoad.LoadLevel(null, name);
            }
        }
        
        static void SetupSocket(SchedulerTask task) {
		    Logger.Log(LogType.SystemActivity, "Creating listening socket on port {0}... ", ServerConfig.Port);
            Listener = new TcpListen();
            
            IPAddress ip;
            if (!IPAddress.TryParse(ServerConfig.ListenIP, out ip)) {
                Logger.Log(LogType.Warning, "Unable to parse listen IP config key, listening on any IP");
                ip = IPAddress.Any;
            }            
            Listener.Listen(ip, (ushort)ServerConfig.Port);
        }
        
        static void InitHeartbeat(SchedulerTask task) {
            try {
                Heartbeat.InitHeartbeats();
            } catch (Exception e) {
                Logger.LogError(e);
            }
        }
        
        static void InitTimers(SchedulerTask task) {
            TextFile announcementsFile = TextFile.Files["Announcements"];
            announcementsFile.EnsureExists();

            string[] lines = announcementsFile.GetText();
            messages = new List<string>(lines);
            
            MainScheduler.QueueRepeat(RandomMessage, null, 
                                      TimeSpan.FromMinutes(5));
            Critical.QueueRepeat(ServerTasks.UpdateEntityPositions, null,
                                 TimeSpan.FromMilliseconds(ServerConfig.PositionUpdateInterval));
        }
        
        static void InitRest(SchedulerTask task) {
            IRC = new IRCBot();
            if (ServerConfig.UseIRC) IRC.Connect();
             
            InitZombieSurvival();
            InitLavaSurvival();
            MainScheduler.QueueRepeat(BlockQueue.Loop, null, 
                                      TimeSpan.FromMilliseconds(BlockQueue.time));
            Critical.QueueRepeat(ServerTasks.LocationChecks, null,
                                 TimeSpan.FromMilliseconds(20));

            Logger.Log(LogType.SystemActivity, "Finished setting up server, finding classicube.net url..");
            ServerSetupFinished = true;
        }
        
        static void InitZombieSurvival() {
            if (!ZombieGameProps.StartImmediately) return;
            try {
                Level oldMain = Server.mainLevel;
                Server.zombie.Start(ZombieGameStatus.InfiniteRounds, null, 0);
                // Did zombie survival change the main world?
                if (oldMain != null && oldMain != Server.mainLevel)
                    oldMain.Unload(true, false);
            } catch (Exception e) { Logger.LogError(e); }
        }

        static void InitLavaSurvival() {
            if (!Server.lava.startOnStartup) return;
            try {
                Server.lava.Start();
            } catch (Exception e) { Logger.LogError(e); }
        }
    }
}