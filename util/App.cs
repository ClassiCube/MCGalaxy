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
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using MCGalaxy;

namespace MCGalaxy.Gui
{
    public static class App
    {
        public static bool usingConsole = false;
        public static string parent = Path.GetFileName(Assembly.GetEntryAssembly().Location);
        public static string parentfullpathdir = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
        private static string CurrentVersionFile = "https://raw.githubusercontent.com/Hetal728/MCGalaxy/master/Uploads/current_version.txt";
        private static string DLLLocation = "https://github.com/Hetal728/MCGalaxy/blob/master/Uploads/MCGalaxy_.dll?raw=true";
        private static string ChangelogLocation = "https://raw.githubusercontent.com/Hetal728/MCGalaxy/master/Changelog.txt";
        private static string EXELocation = "https://github.com/Hetal728/MCGalaxy/blob/master/Uploads/MCGalaxy.exe?raw=true";

        public static bool CurrentUpdate = false;
        static bool msgOpen = false;
        public static System.Timers.Timer updateTimer = new System.Timers.Timer(120 * 60 * 1000);

        public static void UpdateCheck(bool wait = false, Player p = null)
        {
            CurrentUpdate = true;
            Thread updateThread = new Thread(new ThreadStart(delegate
            {
                WebClient Client = new WebClient();

                if (wait) { if (!Server.checkUpdates) return; Thread.Sleep(10000); }
                try
                {
                    string raw = Client.DownloadString(CurrentVersionFile);
                    Version availableUpdateVersion = new Version(raw);
                    if (availableUpdateVersion > Server.Version)
                    {
                        if (Server.autoupdate || p != null)
                        {
                            if (Server.notifyPlayers || p != null)
                            {
                                Chat.MessageAll("Update found. Prepare for restart in &f{0} %Sseconds.", Server.restartcountdown);
                                Server.s.Log("Update found. Prepare for restart in " + Server.restartcountdown + " seconds.");
                                int timeLeft = Server.restartcountdown;
                                System.Timers.Timer countDown = new System.Timers.Timer();
                                countDown.Interval = 1000;
                                countDown.Start();
                                countDown.Elapsed += delegate
                                {
                                    if (Server.autoupdate || p != null)
                                    {
                                        Chat.MessageAll("Updating in &f{0} %Sseconds.", timeLeft);
                                        Server.s.Log("Updating in " + timeLeft + " seconds.");
                                        timeLeft = timeLeft - 1;
                                        if (timeLeft < 0)
                                        {
                                            Chat.MessageAll("---UPDATING SERVER---");
                                            Server.s.Log("---UPDATING SERVER---");
                                            countDown.Stop();
                                            countDown.Dispose();
                                            PerformUpdate();
                                        }
                                    }
                                    else
                                    {
                                        Chat.MessageAll("Stopping auto restart.");
                                        Server.s.Log("Stopping auto restart.");
                                        countDown.Stop();
                                        countDown.Dispose();
                                    }
                                };
                            }
                            else
                            {
                                PerformUpdate();
                            }

                        }
                        else
                        {
                            if (!msgOpen && !usingConsole)
                            {
                                if (Server.autonotify)
                                {
                                    msgOpen = true;
                                    if (MessageBox.Show("New version found. Would you like to update?", "Update?", MessageBoxButtons.YesNo) == DialogResult.Yes)
                                    {
                                        PerformUpdate();
                                    }
                                    msgOpen = false;
                                }
                            }
                            else
                            {
                                ConsoleColor prevColor = Console.ForegroundColor;
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("An update was found!");
                                Console.WriteLine("Update using the file at " + DLLLocation + " and placing it over the top of your current MCGalaxy_.dll!");
                                Console.WriteLine("Also update using the file at " + EXELocation + " and placing it over the top of your current MCGalaxy.exe");
                                Console.ForegroundColor = prevColor;
                            }
                        }
                    }
                    else
                    {
                        Player.Message(p, "No update found!");
                    }
                }
                catch(Exception e) { /*try { Server.s.Log("No web server found to update on.");*/Logger.WriteError(e); } /*catch { }*/ //}
                Client.Dispose();
                CurrentUpdate = false;
            }));
            updateThread.Name = "MCG_UpdateCheck";
            updateThread.Start();
        }

        public static void PerformUpdate() {
            try {
                try {
                    DeleteFiles("MCLawl.new", "Changelog.txt", "MCGalaxy_.update", "MCGalaxy.update",
                                "Update.bat", "Update.sh", "Update_generated.bat", "Update_generated.sh");
                } catch {
                }
                
                WebClient Client = new WebClient();
                Client.DownloadFile(DLLLocation, "MCGalaxy_.update");
                Client.DownloadFile(EXELocation, "MCGalaxy.update");
                Client.DownloadFile(ChangelogLocation, "Changelog.txt");

                Level[] levels = LevelInfo.Loaded.Items;
                foreach (Level lvl in levels) {
                    if (!lvl.ShouldSaveChanges()) continue;
                    lvl.Save(); 
                    lvl.saveChanges();
                }

                Player[] players = PlayerInfo.Online.Items;
                foreach (Player pl in players) pl.save();
                
                if (!usingConsole) {
                    Process.Start("Updater.exe", "securitycheck10934579068013978427893755755270374" + parent);
                } else {
                    Process.Start("mono", parentfullpathdir + "/Updater.exe securitycheck10934579068013978427893755755270374" + parent);
                }
                ExitProgram(false);
            } catch (Exception e) { 
                Server.ErrorLog(e); 
            }
        }
        
        static void DeleteFiles(params string[] files) {
            foreach (string f in files) {
                if (File.Exists(f)) File.Delete(f);
            }
        }

        public static void ExitProgram(bool restarting) { ExitProgram(restarting, ""); }        
        public static void ExitProgram(bool restarting, string msg) {
            Server.restarting = restarting;
            Server.shuttingDown = true;
            if (msg == "")
                msg = restarting ? "Server restarted. Sign in again and rejoin." : Server.shutdownMessage;
            Server.Exit(restarting, msg);
            new Thread(() => ShutdownThread(restarting, msg)).Start();
        }
        
        static void ShutdownThread(bool restarting, string msg) {
            saveAll(restarting, msg);
            if (restarting) {
                if (!usingConsole) {
                    Process.Start(parent);
                    Environment.Exit(0);
                } else {
                    Process.Start(Application.ExecutablePath);
                    Application.Exit();
                }
            } else {
                Application.Exit();
                if (usingConsole)
                    Process.GetCurrentProcess().Kill();
                Environment.Exit(0);
            }
        }

        public static void saveAll(bool restarting, string msg) {
            try {
                Player[] players = PlayerInfo.Online.Items; 
                foreach (Player p in players) {
                    p.Leave(msg);
                }
            } catch (Exception ex) { 
                Server.ErrorLog(ex); 
            }

            try {
                string level = null;
                Level[] loaded = LevelInfo.Loaded.Items;
                foreach (Level lvl in loaded) {
                    if (!lvl.ShouldSaveChanges()) continue;
                    level = level + lvl.name + "=" + lvl.physics + Environment.NewLine;
                    lvl.Save(false, true);
                    lvl.saveChanges();
                }
                
                if (Server.ServerSetupFinished && !Server.AutoLoad) {
                    File.WriteAllText("text/autoload.txt", level);
                }
            } catch (Exception ex) {
                Server.ErrorLog(ex); 
            }
        }
    }
}

