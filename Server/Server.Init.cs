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
                            LevelDB.CreateTables(level);
                        }
                    }
                } else {
                    Log("mainlevel not found");
                    mainLevel = new Level(level, 128, 64, 128);
                    MapGen.Generate(mainLevel, "flat", "", null);
                    mainLevel.Save();
                    LevelDB.CreateTables(level);
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
                UpgradeOldAgreed();
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
            
            jailed = PlayerExtList.Load("ranks/jailed.txt");
            models = PlayerExtList.Load("extra/models.txt");
            skins = PlayerExtList.Load("extra/skins.txt");
            reach = PlayerExtList.Load("extra/reach.txt");

            foreach (Group grp in Group.GroupList)
                grp.playerList = PlayerList.Load(grp.fileName);
            if (useWhitelist)
                whiteList = PlayerList.Load("whitelist.txt");
        }
        
        static void UpgradeOldBlacklist() {
            if (!Directory.Exists("levels/blacklists")) return;
            string[] files = Directory.GetFiles("levels/blacklists");
            for (int i = 0; i < files.Length; i++) {
                string[] blacklist = File.ReadAllLines(files[i]);
                List<string> names = new List<string>();
                
                // Lines are in the format: day.month.year name+
                foreach (string entry in blacklist) {
                    string[] parts = entry.Split(' ');
                    string name = parts[parts.Length - 1];
                    name = name.Substring(0, name.Length - 1);
                    names.Add(name);
                }
                
                if (names.Count > 0) {
                    string lvlName = Path.GetFileNameWithoutExtension(files[i]);
                    string propsPath = LevelInfo.PropertiesPath(lvlName);
                    using (StreamWriter w = new StreamWriter(propsPath, true)) {
                        w.WriteLine("VisitBlacklist = " + names.Join());
                    }
                }
                File.Delete(files[i]);
            }
            Directory.Delete("levels/blacklists");
        }
        
        static void UpgradeOldAgreed() {
            // agreed.txt format used to be names separated by spaces, we need to fix that up.
            if (!File.Exists("ranks/agreed.txt")) return;
            
            string data = null;
            using (FileStream fs = File.OpenRead("ranks/agreed.txt")) {
                if (fs.ReadByte() != ' ') return;
                data = new StreamReader(fs).ReadToEnd();
                data = data.Replace(" ", Environment.NewLine);
            }
            File.WriteAllText("ranks/agreed.txt", data);
        }
        
        void LoadAutoloadCommands() {
            if (File.Exists("text/autoload.txt")) {
                try {
                    PropertiesFile.Read("text/autoload.txt", AutoLoadLineProcessor);
                } catch {
                    s.Log("autoload.txt error");
                }
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
		
        void MovePreviousLevelFiles() {
            if (!Directory.Exists("levels")) return;
            try {
                string[] files = Directory.GetFiles("levels", "*.prev");
                if (files.Length == 0) return;
                if (!Directory.Exists("levels/prev"))
                    Directory.CreateDirectory("levels/prev");
                
                foreach (string file in files) {
                    string name = Path.GetFileName(file);
                    string newFile = "levels/prev/" + name;
                    
                    try {
                        File.Move(file, newFile);
                    } catch (Exception ex) {
                        Server.s.Log("Error while trying to move .lvl.prev file");
                        Server.ErrorLog(ex);
                    }
                }
            } catch (Exception ex) {
                Server.ErrorLog(ex);
            }
        }
        
        void CombineEnvFiles() {
            if (!Directory.Exists("levels/level properties")) return;
            try {
                string[] files = Directory.GetFiles("levels/level properties", "*.env");
                if (files.Length == 0) return;
                
                Server.s.Log("Combining " + files.Length + " .env and .properties files..");
                foreach (string envFile in files) {
                    try {
                        Combine(envFile);
                    } catch (Exception ex) {
                        Server.s.Log("Error while trying to combine .env and .properties file");
                        Server.ErrorLog(ex);
                    }
                }
                Server.s.Log("Finished combining .env and .properties files.");
            } catch (Exception ex) {
                Server.ErrorLog(ex);
            }
        }
        
        static void Combine(string envFile) {
            string name = Path.GetFileNameWithoutExtension(envFile);
            string propFile = LevelInfo.FindPropertiesFile(name);
            List<string> lines = new List<string>();
            if (propFile != null)
                lines = CP437Reader.ReadAllLines(propFile);
            
            using (StreamReader r = new StreamReader(envFile)) {
                string line = null;
                while ((line = r.ReadLine()) != null)
                    lines.Add(line);
            }
            
            propFile = LevelInfo.PropertiesPath(name);
            CP437Writer.WriteAllLines(propFile, lines.ToArray());
            File.Delete(envFile);
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
            try {
                if (EnableHttpApi) {
                    APIServer = new WebServer(SendResponse, "http://localhost:8080/api/");
                    APIServer.Run();
                    InfoServer = new WebServer(WhoIsResponse, "http://localhost:8080/whois/");
                    InfoServer.Run();
                }
            } catch {
                Server.s.Log("Failed to start local API server");
            }

            IRC = new IRCBot();
            if (Server.irc) IRC.Connect();

            locationChecker = new Thread(DoLocationChecks);
            locationChecker.Name = "MCG_LocationCheck";
            locationChecker.Start();
             
            InitZombieSurvival();
            BlockQueue.Start();

            Log("Finished setting up server, finding classicube.net url..");
            ServerSetupFinished = true;
        }
        
        void InitZombieSurvival() {
            if (!ZombieGame.StartImmediately) return;
            try {
                Level oldMain = Server.mainLevel;
                Server.zombie.Start(ZombieGameStatus.InfiniteRounds, null, 0);
                // Did zombie survival change the main world?
                if (oldMain != null && oldMain != Server.mainLevel)
                    oldMain.Unload(true, false);
            } catch (Exception e) { Server.ErrorLog(e); }
        }

        const string staffUrl = "https://raw.githubusercontent.com/Hetal728/MCGalaxy/master/Uploads/devs.txt";       
        void UpdateStaffListTask() {
            try {
                using (WebClient web = new WebClient()) {
                    string[] result = web.DownloadString(staffUrl).Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);
                    foreach (string line in result) {
                        string type = line.Split(':')[0].ToLower();
                        List<string> staffList = type.Equals("devs") ? Devs : type.Equals("mods") ? Mods : null;
                        foreach (string name in line.Split(':')[1].Split())
                        	staffList.Add(name.TrimEnd('+'));
                    }
                }
            } catch (Exception e) {
                ErrorLog(e);
                s.Log("Failed to update MCGalaxy staff list.");
                Devs.Clear();
                Mods.Clear();
            }
        }
    }
}