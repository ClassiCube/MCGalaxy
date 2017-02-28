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
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using System.Windows.Forms;
using MCGalaxy;

namespace MCGalaxy {
    public static class Updater {
        
        public static string parent = Path.GetFileName(Assembly.GetEntryAssembly().Location);
        const string CurrentVersionFile = "https://raw.githubusercontent.com/Hetal728/MCGalaxy/master/Uploads/current_version.txt";
        const string DLLLocation = "https://github.com/Hetal728/MCGalaxy/blob/master/Uploads/MCGalaxy_.dll?raw=true";
        const string ChangelogLocation = "https://raw.githubusercontent.com/Hetal728/MCGalaxy/master/Changelog.txt";
        const string EXELocation = "https://github.com/Hetal728/MCGalaxy/blob/master/Uploads/MCGalaxy.exe?raw=true";

        public static bool CurrentUpdate = false;
        static bool msgOpen = false;
        
        public static void UpdaterTask(SchedulerTask task) {
            UpdateCheck(null);
            task.Delay = TimeSpan.FromHours(2);
        }

        public static void UpdateCheck(Player p = null) {
            CurrentUpdate = true;
            if (!Server.checkUpdates) return;            
            WebClient Client = new WebClient();

            try {
                string raw = Client.DownloadString(CurrentVersionFile);
                Version latestVersion = new Version(raw);
                if (latestVersion <= Server.Version) {
                    Player.Message(p, "No update found!");
                    return;
                }
                
                if (Server.autoupdate || p != null) {
                    if (Server.notifyPlayers || p != null) {
                        NotifyPlayersOfUpdate(p);
                    } else {
                        PerformUpdate();
                    }
                } else if (!msgOpen && !MCGalaxy.Gui.App.usingConsole) {
                    msgOpen = true;
                    if (MessageBox.Show("New version found. Would you like to update?", "Update?", MessageBoxButtons.YesNo) == DialogResult.Yes) {
                        PerformUpdate();
                    }
                    msgOpen = false;
                } else if (MCGalaxy.Gui.App.usingConsole) {
                    ConsoleColor prevColor = Console.ForegroundColor;
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("An update was found!");
                    Console.WriteLine("Update using the file at " + DLLLocation + " and placing it over the top of your current MCGalaxy_.dll!");
                    Console.WriteLine("Also update using the file at " + EXELocation + " and placing it over the top of your current MCGalaxy.exe");
                    Console.ForegroundColor = prevColor;
                }
            } catch(Exception e) {
                Logger.WriteError(e);
            }
            
            Client.Dispose();
            CurrentUpdate = false;
        }
        
        static void NotifyPlayersOfUpdate(Player p) {
            Chat.MessageAll("Update found. Prepare for restart in &f{0} %Sseconds.", Server.restartcountdown);
            Server.s.Log("Update found. Prepare for restart in " + Server.restartcountdown + " seconds.");
            
            int timeLeft = Server.restartcountdown;
            System.Timers.Timer countDown = new System.Timers.Timer();
            countDown.Interval = 1000;
            countDown.Start();
            
            countDown.Elapsed += delegate {
                if (Server.autoupdate || p != null) {
                    Chat.MessageAll("Updating in &f{0} %Sseconds.", timeLeft);
                    Server.s.Log("Updating in " + timeLeft + " seconds.");
                    timeLeft = timeLeft - 1;
                    if (timeLeft < 0) {
                        Chat.MessageAll("---UPDATING SERVER---");
                        Server.s.Log("---UPDATING SERVER---");
                        countDown.Stop();
                        countDown.Dispose();
                        PerformUpdate();
                    }
                } else {
                    Chat.MessageAll("Stopping auto restart.");
                    Server.s.Log("Stopping auto restart.");
                    countDown.Stop();
                    countDown.Dispose();
                }
            };
        }

        public static void PerformUpdate() {
            try {
                try {
                    DeleteFiles("MCLawl.new", "Changelog.txt", "MCGalaxy_.update", "MCGalaxy.update",
                                "Update.bat", "Update.sh", "Update_generated.bat", "Update_generated.sh");
                } catch {
                }
                
                WebClient client = new WebClient();
                client.DownloadFile(DLLLocation, "MCGalaxy_.update");
                client.DownloadFile(EXELocation, "MCGalaxy.update");
                client.DownloadFile(ChangelogLocation, "Changelog.txt");

                Level[] levels = LevelInfo.Loaded.Items;
                foreach (Level lvl in levels) {
                    if (!lvl.ShouldSaveChanges()) continue;
                    
                    lvl.Save();
                    lvl.saveChanges();
                }

                Player[] players = PlayerInfo.Online.Items;
                foreach (Player pl in players) pl.save();
                
                bool mono = Type.GetType("Mono.Runtime") != null;
                if (!mono) {
                    Process.Start("Updater.exe", "securitycheck10934579068013978427893755755270374" + parent);
                } else {
                    string path = Path.Combine(Utils.FolderPath, "Updater.exe");
                    Process.Start("mono", path + " securitycheck10934579068013978427893755755270374" + parent);
                }
                MCGalaxy.Gui.App.ExitProgram(false, "Updating server.");
            } catch (Exception e) {
                Server.ErrorLog(e);
            }
        }
        
        static void DeleteFiles(params string[] files) {
            foreach (string f in files) {
                if (File.Exists(f)) File.Delete(f);
            }
        }
    }
}
