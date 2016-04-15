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
                                //if (p != null) Server.restartcountdown = "20";  This is set by the user.  Why change it?
                                Player.GlobalMessage("Update found. Prepare for restart in &f" + Server.restartcountdown + Server.DefaultColor + " seconds.");
                                Server.s.Log("Update found. Prepare for restart in " + Server.restartcountdown + " seconds.");
                                int timeLeft = Server.restartcountdown;
                                System.Timers.Timer countDown = new System.Timers.Timer();
                                countDown.Interval = 1000;
                                countDown.Start();
                                countDown.Elapsed += delegate
                                {
                                    if (Server.autoupdate || p != null)
                                    {
                                        Player.GlobalMessage("Updating in &f" + timeLeft + Server.DefaultColor + " seconds.");
                                        Server.s.Log("Updating in " + timeLeft + " seconds.");
                                        timeLeft = timeLeft - 1;
                                        if (timeLeft < 0)
                                        {
                                            Player.GlobalMessage("---UPDATING SERVER---");
                                            Server.s.Log("---UPDATING SERVER---");
                                            countDown.Stop();
                                            countDown.Dispose();
                                            PerformUpdate();
                                        }
                                    }
                                    else
                                    {
                                        Player.GlobalMessage("Stopping auto restart.");
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
                        Player.SendMessage(p, "No update found!");
                    }
                }
                catch(Exception e) { /*try { Server.s.Log("No web server found to update on.");*/Logger.WriteError(e); } /*catch { }*/ //}
                Client.Dispose();
                CurrentUpdate = false;
            }));
            updateThread.Name = "MCG_UpdateCheck";
            updateThread.Start();
        }

        public static void PerformUpdate()
        {
            try
            {
                //StreamWriter SW;
                //if (!Server.mono)
                //{
                //    if (!File.Exists("Update.bat"))
                //        SW = new StreamWriter(File.Create("Update.bat"));
                //    else
                //    {
                //        if (File.ReadAllLines("Update.bat")[0] != "::Version 3")
                //        {
                //            SW = new StreamWriter(File.Create("Update.bat"));
                //        }
                //        else
                //        {
                //            SW = new StreamWriter(File.Create("Update_generated.bat"));
                //        }
                //    }
                //    SW.WriteLine("::Version 3");
                //    SW.WriteLine("TASKKILL /pid %2 /F");
                //    SW.WriteLine("if exist MCGalaxy_.dll.backup (erase MCGalaxy_.dll.backup)");
                //    SW.WriteLine("if exist MCGalaxy_.dll (rename MCGalaxy_.dll MCGalaxy_.dll.backup)");
                //    SW.WriteLine("if exist MCGalaxy.new (rename MCGalaxy.new MCGalaxy_.dll)");
                //    SW.WriteLine("start MCGalaxy.exe");
                //}
                //else
                //{
                //    if (!File.Exists("Update.sh"))
                //        SW = new StreamWriter(File.Create("Update.sh"));
                //    else
                //    {
                //        if (File.ReadAllLines("Update.sh")[0] != "#Version 2")
                //        {
                //            SW = new StreamWriter(File.Create("Update.sh"));
                //        }
                //        else
                //        {
                //            SW = new StreamWriter(File.Create("Update_generated.sh"));
                //        }
                //    }
                //    SW.WriteLine("#Version 2");
                //    SW.WriteLine("#!/bin/bash");
                //    SW.WriteLine("kill $2");
                //    SW.WriteLine("rm MCGalaxy_.dll.backup");
                //    SW.WriteLine("mv MCGalaxy_.dll MCGalaxy.dll_.backup");
                //    SW.WriteLine("wget " + DLLLocation);
                //    SW.WriteLine("mono MCGalaxy.exe");
                //}

                //SW.Flush(); SW.Close(); SW.Dispose();

                //Process proc = Process.GetCurrentProcess();
                //string assemblyname = proc.ProcessName + ".exe";

                //WebClient client = new WebClient();
                //Server.selectedrevision = client.DownloadString(Program.CurrentVersionFile);
                //client.Dispose();

                //string verscheck = Server.selectedrevision.TrimStart('r');
                //int vers = int.Parse(verscheck.Split('.')[0]);
                try
                {
                    if (File.Exists("MCLawl.new"))
                        File.Delete("MCLawl.new");
                    if (File.Exists("Changelog.txt"))
                        File.Delete("Changelog.txt");
                    if (File.Exists("MCGalaxy_.update"))
                        File.Delete("MCGalaxy_.update");
                    if (File.Exists("MCGalaxy.update"))
                        File.Delete("MCGalaxy.update");
                    if (File.Exists("Update.bat"))
                        File.Delete("Update.bat");
                    if (File.Exists("Update_generated.bat"))
                        File.Delete("Update_generated.bat");
                    if (File.Exists("Update.sh"))
                        File.Delete("Update.sh");
                    if (File.Exists("Update_generated.sh"))
                        File.Delete("Update_generated.sh");
                }
                catch { }
                WebClient Client = new WebClient();
                Client.DownloadFile(DLLLocation, "MCGalaxy_.update");
                Client.DownloadFile(EXELocation, "MCGalaxy.update");
                Client.DownloadFile(ChangelogLocation, "Changelog.txt");

                Level[] levels = LevelInfo.Loaded.Items;
                foreach (Level lvl in levels) {
                	if (lvl.ShouldSaveLevelFile()) lvl.Save();
                	lvl.saveChanges();
                }

                Player[] players = PlayerInfo.Online.Items;
                foreach (Player pl in players) pl.save();
                
                if (!usingConsole)
                    Process.Start("Updater.exe", "securitycheck10934579068013978427893755755270374" + parent);
                else
                {
                    Process.Start("mono", parentfullpathdir + "/Updater.exe securitycheck10934579068013978427893755755270374" + parent);
                }
                ExitProgram(false);
            }
            catch (Exception e) { Server.ErrorLog(e); }
        }

        public static void ExitProgram(bool AutoRestart)
        {
            Server.restarting = AutoRestart;
            Server.shuttingDown = true;

            Server.Exit(AutoRestart);

            new Thread(new ThreadStart(delegate
            {
                /*try
                {
                    if (MCGalaxy.Gui.Window.thisWindow.notifyIcon1 != null)
                    {
                        MCGalaxy.Gui.Window.thisWindow.notifyIcon1.Icon = null;
                        MCGalaxy.Gui.Window.thisWindow.notifyIcon1.Visible = false;
                        MCGalaxy.Gui.Window.thisWindow.notifyIcon1.Dispose();
                    }
                }
                catch { }
                */
                if (AutoRestart)
                {
                    saveAll(true);

                    if (Server.listen != null) Server.listen.Close();
                    if (!usingConsole)
                    {
                        Process.Start(parent);
                        Environment.Exit(0);
                    }
                    else
                    {
                        System.Diagnostics.Process.Start(Application.ExecutablePath);
                        Application.Exit();
                    }
                }
                else
                {
                    saveAll(false);
                    Application.Exit();
                    if (usingConsole)
                    {
                        Process.GetProcessById(Process.GetCurrentProcess().Id).Kill();
                    }
                    Environment.Exit(0);
                }
            })).Start();
        }

        public static void saveAll(bool restarting)
        {
            try
            {
            	Player[] players = PlayerInfo.Online.Items; 
                foreach (Player p in players)
                {
                    string msg = restarting ? "Server restarted. Sign in again and rejoin." : Server.shutdownMessage;
                    p.LeaveServer(msg, msg);
                }
            }
            catch (Exception ex) { Server.ErrorLog(ex); }

            try
            {
                string level = null;
                Level[] loaded = LevelInfo.Loaded.Items;
                foreach (Level lvl in loaded)
                {
                	if (lvl.ShouldSaveLevelFile()) {
                        level = level + lvl.name + "=" + lvl.physics + Environment.NewLine;
                        lvl.Save(false, true);
                    }
                    lvl.saveChanges();
                }
                if (Server.ServerSetupFinished && !Server.AutoLoad)
                {
                    File.WriteAllText("text/autoload.txt", level);
                }
            }
            catch (Exception exc) { Server.ErrorLog(exc); }
        }
    }
}

