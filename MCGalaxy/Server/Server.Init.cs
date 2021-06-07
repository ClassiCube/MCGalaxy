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
using MCGalaxy.DB;
using MCGalaxy.Games;
using MCGalaxy.Generator;
using MCGalaxy.Network;
using MCGalaxy.Tasks;
using MCGalaxy.Util;

namespace MCGalaxy {
    
    public sealed partial class Server {

        static void LoadMainLevel(SchedulerTask task) {
            try {
                mainLevel = LevelActions.Load(Player.Console, Server.Config.MainLevel, false);
                if (mainLevel == null) GenerateMain();
            } catch (Exception ex) {
                Logger.LogError("Error loading main level", ex);
            }
        }
        
        static void GenerateMain() {
            Logger.Log(LogType.SystemActivity, "main level not found, generating..");
            mainLevel = new Level(Server.Config.MainLevel, 128, 64, 128);
            
            MapGen.Find("Flat").Generate(Player.Console, mainLevel, "");
            mainLevel.Save();
            Level.LoadMetadata(mainLevel);
            LevelInfo.Add(mainLevel);
        }

        static void LoadAllPlugins(SchedulerTask task) { Plugin.LoadAll(); }
        
        static void InitPlayerLists(SchedulerTask task) {
            try {
                UpgradeTasks.UpgradeOldAgreed();
            } catch (Exception ex) {
                Logger.LogError("Error upgrading agreed list", ex);
            }
            
            LoadPlayerLists();
            ModerationTasks.QueueTasks();
        }
		
        internal static void LoadPlayerLists() {
            agreed = PlayerList.Load("ranks/agreed.txt");
            invalidIds = PlayerList.Load("extra/invalidids.txt");
            Player.Console.DatabaseID = NameConverter.InvalidNameID("(console)");
            
            bannedIP = PlayerList.Load("ranks/banned-ip.txt");
            hidden   = PlayerList.Load("ranks/hidden.txt");
            vip      = PlayerList.Load("text/vip.txt");
            noEmotes = PlayerList.Load("text/emotelist.txt");
            lockdown = PlayerList.Load("text/lockdown.txt");
            
            models = PlayerExtList.Load("extra/models.txt");
            skins  = PlayerExtList.Load("extra/skins.txt");
            reach  = PlayerExtList.Load("extra/reach.txt");
            rotations   = PlayerExtList.Load("extra/rotations.txt");
            modelScales = PlayerExtList.Load("extra/modelscales.txt");

            muted  = PlayerExtList.Load("ranks/muted.txt");
            frozen = PlayerExtList.Load("ranks/frozen.txt");
            tempRanks = PlayerExtList.Load(Paths.TempRanksFile);
            tempBans  = PlayerExtList.Load(Paths.TempBansFile);
            whiteList = PlayerList.Load("ranks/whitelist.txt");
	    }
        
        static void LoadAutoloadMaps(SchedulerTask task) {
            AutoloadMaps = PlayerExtList.Load("text/autoload.txt", '=');
            List<string> maps = AutoloadMaps.AllNames();
            
            foreach (string map in maps) {
                if (map.CaselessEq(Server.Config.MainLevel)) continue;
                LevelActions.Load(Player.Console, map, false);
            }
        }
        
        static void SetupSocket(SchedulerTask task) {
            Listener = new TcpListen();            
            IPAddress ip;
            
            if (!IPAddress.TryParse(Server.Config.ListenIP, out ip)) {
                Logger.Log(LogType.Warning, "Unable to parse listen IP config key, listening on any IP");
                ip = IPAddress.Any;
            }            
            Listener.Listen(ip, Server.Config.Port);
        }
        
        static void InitHeartbeat(SchedulerTask task) {
            try {
                Heartbeat.InitHeartbeats();
            } catch (Exception ex) {
                Logger.LogError("Error initialising heartbeat", ex);
            }
        }
        
        static void InitTimers(SchedulerTask task) {
            MainScheduler.QueueRepeat(RandomMessage, null, 
                                      Server.Config.AnnouncementInterval);
            Critical.QueueRepeat(ServerTasks.UpdateEntityPositions, null,
                                 TimeSpan.FromMilliseconds(Server.Config.PositionUpdateInterval));
        }
        
        static void InitRest(SchedulerTask task) {
            CountdownGame.Instance.AutoStart();
            ZSGame.Instance.AutoStart();
            LSGame.Instance.AutoStart();
            CTFGame.Instance.AutoStart();
            TWGame.Instance.AutoStart();
            
            MainScheduler.QueueRepeat(BlockQueue.Loop, null, 
                                      TimeSpan.FromMilliseconds(BlockQueue.Interval));
            Critical.QueueRepeat(ServerTasks.TickPlayers, null,
                                 TimeSpan.FromMilliseconds(20));

            Logger.Log(LogType.SystemActivity, "Finished setting up server, finding classicube.net url..");
            SetupFinished = true;
        }
    }
}