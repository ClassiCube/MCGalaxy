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
                if (LevelInfo.ExistsOffline(level)) {
                    mainLevel = Level.Load(level);
                    mainLevel.unload = false;
                    if (mainLevel == null) {
                        if (File.Exists(LevelInfo.LevelPath(level) + ".backup")) {
                            Log("Attempting to load backup of " + level + ".");
                            File.Copy(LevelInfo.LevelPath(level) + ".backup", LevelInfo.LevelPath(level), true);
                            mainLevel = Level.Load(level);
                            if (mainLevel == null) {
                                Log("BACKUP FAILED!");
                                Console.ReadLine(); return;
                            }
                        } else {
                            Log("mainlevel not found");
                            mainLevel = new Level(level, 128, 64, 128);
                            MapGen.Generate(mainLevel, "flat", "", null);
                            mainLevel.Save();
                        }
                    }
                } else {
                    Log("mainlevel not found");
                    mainLevel = new Level(level, 128, 64, 128);
                    MapGen.Generate(mainLevel, "flat", "", null);
                    mainLevel.Save();
                }
                LevelInfo.Loaded.Add(mainLevel);

                // fenderrock - Make sure the level does have a physics thread
                if (mainLevel.physThread == null)
                    mainLevel.StartPhysics();
            } catch (Exception e) {
                ErrorLog(e);
            }
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

            foreach (Group grp in Group.GroupList)
                grp.playerList = PlayerList.Load(grp.fileName);
            if (useWhitelist)
                whiteList = PlayerList.Load("whitelist.txt");
        }
        
        void LoadAutoloadCommands() {
            if (File.Exists("text/autoload.txt")) {
                PropertiesFile.Read("text/autoload.txt", AutoLoadLineProcessor);
                GC.Collect();
                GC.WaitForPendingFinalizers();
            } else {
                Log("autoload.txt does not exist");
            }
        }
        
        static void AutoLoadLineProcessor(string name, string phys) {
            name = name.ToLower();
            if (phys == "") phys = "0";

            if (name != mainLevel.name) {
                CmdLoad.LoadLevel(null, name, phys);
            } else {
                try {
                    int physLevel = int.Parse(phys);
                    if (physLevel >= 0 && physLevel <= 5)
                        mainLevel.setPhysics(physLevel);
                } catch {
                    s.Log("Physics variable invalid");
                }
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
                messages = CP437Reader.ReadAllLines("text/messages.txt");
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