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
using System.IO;
using System.Net;
using System.Threading;
using MCGalaxy.Commands.World;
using MCGalaxy.Games;
using MCGalaxy.Generator;
using MCGalaxy.Tasks;

namespace MCGalaxy {
    
    public sealed partial class Server {

        void LoadMainLevel() {
            try {
                mainLevel = CmdLoad.LoadLevel(null, level);                
                if (mainLevel == null) GenerateMain();
                
                mainLevel.unload = false;
                LevelInfo.Loaded.Add(mainLevel);
            } catch (Exception e) {
                ErrorLog(e);
            }
        }
        
        void GenerateMain() {
            Log("main level not found, generating..");
            mainLevel = new Level(level, 128, 64, 128);
            MapGen.Generate(mainLevel, "flat", "", null);
            mainLevel.Save();
        }
        
        void LoadPlayerLists() {
            agreed = new PlayerList("ranks/agreed.txt");
            try {
                UpgradeTasks.UpgradeOldAgreed();
                agreed = PlayerList.Load("agreed.txt");
            } catch (Exception ex) {
                Server.ErrorLog(ex);
            }
            
            bannedIP = PlayerList.Load("banned-ip.txt");
            ircControllers = PlayerList.Load("IRC_Controllers.txt");
            muted = PlayerList.Load("muted.txt");
            frozen = PlayerList.Load("frozen.txt");
            hidden = PlayerList.Load("hidden.txt");
            vip = PlayerList.Load("text/vip.txt");
            noEmotes = PlayerList.Load("text/emotelist.txt");
            lockdown = PlayerList.Load("text/lockdown.txt");
            
            jailed = PlayerExtList.Load("ranks/jailed.txt");
            models = PlayerExtList.Load("extra/models.txt");
            skins = PlayerExtList.Load("extra/skins.txt");
            reach = PlayerExtList.Load("extra/reach.txt");
            invalidIds = PlayerList.Load("extra/invalidids.txt");
            tempBans = PlayerExtList.Load("text/tempbans.txt");
            
            if (useWhitelist)
                whiteList = PlayerList.Load("whitelist.txt");
        }
        
        void LoadAutoloadMaps() {
            AutoloadMaps = PlayerExtList.Load("text/autoload.txt", '=');
            List<string> maps = AutoloadMaps.AllNames();
            
            foreach (string map in maps) {
                string name = map.ToLower();               
                if (name != mainLevel.name) CmdLoad.LoadLevel(null, name);
            }
        }
        
        void SetupSocket() {
            Log("Creating listening socket on port " + port + "... ");
            Setup();
        }
        
        void InitHeartbeat() {
            try {
                Heart.Init();
            } catch (Exception e) {
                Server.ErrorLog(e);
            }
        }
        
        void InitTimers() {
            updateTimer.Elapsed += delegate {
                Entities.GlobalUpdate();
                PlayerBot.GlobalUpdatePosition();
            };
            updateTimer.Start();

            if (File.Exists("text/messages.txt")) {
                string[] lines = File.ReadAllLines("text/messages.txt");
                messages = new List<string>(lines);
            } else {
                using (File.Create("text/messages.txt")) {}
            }
            Server.MainScheduler.QueueRepeat(RandomMessage, null, TimeSpan.FromMinutes(5));
        }
        
        void InitRest() {
            IRC = new IRCBot();
            if (Server.irc) IRC.Connect();

            locationChecker = new Thread(ServerTasks.LocationChecks);
            locationChecker.Name = "MCG_LocationCheck";
            locationChecker.Start();
             
            InitZombieSurvival();
            BlockQueue.Start();

            Log("Finished setting up server, finding classicube.net url..");
            ServerSetupFinished = true;
        }
        
        void InitZombieSurvival() {
            if (!ZombieGameProps.StartImmediately) return;
            try {
                Level oldMain = Server.mainLevel;
                Server.zombie.Start(ZombieGameStatus.InfiniteRounds, null, 0);
                // Did zombie survival change the main world?
                if (oldMain != null && oldMain != Server.mainLevel)
                    oldMain.Unload(true, false);
            } catch (Exception e) { Server.ErrorLog(e); }
        }
    }
}