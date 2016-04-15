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
    public static class Program
    {
        public static DateTime startTime;

        [DllImport("kernel32")]
        public static extern IntPtr GetConsoleWindow();
        [DllImport("user32.dll")]
        public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        public static void GlobalExHandler(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = (Exception)e.ExceptionObject;
            Server.ErrorLog(ex);
            Thread.Sleep(500);

            if (Server.restartOnError)
                App.ExitProgram(true);
        }

        public static void ThreadExHandler(object sender, ThreadExceptionEventArgs e)
        {
            Exception ex = e.Exception;
            Server.ErrorLog(ex);
            Thread.Sleep(500);

            if (Server.restartOnError)
                App.ExitProgram(true);
        }

        static bool useConsole, useHighQualityGui;
        [STAThread]
        public static void Main(string[] args)
        {
            startTime = DateTime.Now;
            if (Process.GetProcessesByName("MCGalaxy").Length != 1)
            {
                foreach (Process pr in Process.GetProcessesByName("MCGalaxy"))
                {
                    if (pr.MainModule.BaseAddress == Process.GetCurrentProcess().MainModule.BaseAddress)
                        if (pr.Id != Process.GetCurrentProcess().Id)
                            pr.Kill();
                }
            }
            Logger.Init();
            AppDomain.CurrentDomain.UnhandledException += GlobalExHandler;
            Application.ThreadException += ThreadExHandler;
            useConsole = false; useHighQualityGui = false;

            try
            {
                if (!File.Exists("Viewmode.cfg")) {
                    using (StreamWriter SW = new StreamWriter("Viewmode.cfg")) {
                        SW.WriteLine("#This file controls how the console window is shown to the server host");
                        SW.WriteLine("#cli: True or False (Determines whether a CLI interface is used) (Set True if on Mono)");
                        SW.WriteLine("#high-quality: True or false (Determines whether the GUI interface uses higher quality objects)");
                        SW.WriteLine();
                        SW.WriteLine("cli = false");
                        SW.WriteLine("high-quality = true");
                    }
                }
                PropertiesFile.Read("Viewmode.cfg", ViewmodeLineProcessor);
            	
                if (useConsole) {
                    Server s = new Server();
                    s.OnLog += WriteToConsole;
                    s.OnCommand += WriteToConsole;
                    s.OnSystem += WriteToConsole;
                    s.Start();

                    Console.Title = Server.name + " - MCGalaxy " + Server.Version;
                    MCGalaxy.Gui.App.usingConsole = true;
                    handleComm();
                    //Application.Run();
                } else {
                    IntPtr hConsole = GetConsoleWindow();
                    if (IntPtr.Zero != hConsole)
                        ShowWindow(hConsole, 0);
                    App.UpdateCheck(true);
                    
                    if (useHighQualityGui) {
                        Application.EnableVisualStyles();
                        Application.SetCompatibleTextRenderingDefault(false);
                    }

                    App.updateTimer.Elapsed += delegate { App.UpdateCheck(); }; App.updateTimer.Start();
                    Application.Run(new MCGalaxy.Gui.Window());
                }
                WriteToConsole("Completed in " + (DateTime.Now - startTime).Milliseconds + "ms");
            }
            catch (Exception e) { Server.ErrorLog(e); }
        }

        static void ViewmodeLineProcessor(string key, string value) {
            switch (key.ToLower()) {
                case "cli":
                    useConsole = value.ToLower() == "true"; break;
                case "high-quality":
                    useHighQualityGui = value.ToLower() == "true"; break;
            }
        }
        
        //U WANT TO SEE HOW WE DO IT MCSTORM?
        //I WOULD EXPLAIN, BUT ITS BETTER TO JUST COPY AND PASTE, AMIRITE?
        private static void WriteToConsole(string message)
        {
            if (!message.Contains("&") && !message.Contains("%"))
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(message);
                return;
            }
            string[] splitted = message.Split('&', '%');
            for (int i = 0; i < splitted.Length; i++)
            {
                string elString = splitted[i];
                if (String.IsNullOrEmpty(elString))
                    continue;
                Console.ForegroundColor = GetColor(elString[0]);
                Console.Write(elString.Substring(1));
                if (i != splitted.Length - 1)
                    continue;
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write('\n');
            }
        }

        private static ConsoleColor GetColor(char c) {
        	Colors.MapColor(ref c);
            switch (c) {             
                case '0': return ConsoleColor.Black;
                case '1': return ConsoleColor.DarkBlue;
                case '2': return ConsoleColor.DarkGreen;
                case '3': return ConsoleColor.DarkCyan;
                case '4': return ConsoleColor.DarkRed;
                case '5': return ConsoleColor.DarkMagenta;
                case '7': return ConsoleColor.Gray;
                case '6': return ConsoleColor.DarkYellow;
                case '8': return ConsoleColor.DarkGray;
                case '9': return ConsoleColor.Blue;
                case 'a': case 'A': return ConsoleColor.Green;
                case 'b': case 'B': return ConsoleColor.Cyan;
                case 'c': case 'C': return ConsoleColor.Red;
                case 'd': case 'D': return ConsoleColor.Magenta;
                case 'e': case 'E': return ConsoleColor.Yellow;
                case 'f': case 'F': return ConsoleColor.White;
                default:
                    char fallback = Colors.GetFallback(c);
                    return fallback == '\0' ? ConsoleColor.White : GetColor(fallback);
            }
        }

        public static void handleComm()
        {
            string s, msg;
            while (true)
            {
                try
                {
                    string sentCmd = String.Empty, sentMsg = String.Empty;
                    s = Console.ReadLine().Trim(); // Make sure we have no whitespace!

                    if (s.Length < 1) continue;
                    if (s[0] == '/') s = s.Remove(0, 1);
                    else goto talk;
                    if (s.IndexOf(' ') != -1)
                    {
                        sentCmd = s.Substring(0, s.IndexOf(' '));
                        sentMsg = s.Substring(s.IndexOf(' ') + 1);
                    }
                    else if (s != String.Empty) sentCmd = s;
                    else goto talk;

                    try
                    {
                        if (Server.Check(sentCmd, sentMsg)) { Server.cancelcommand = false; continue; }
                        Command cmd = Command.all.Find(sentCmd);
                        if (cmd != null)
                        {
                            cmd.Use(null, sentMsg);
                            Console.WriteLine("CONSOLE: USED /" + sentCmd + " " + sentMsg);
                            if (sentCmd.ToLower() != "restart")
                                continue;
                            break;
                        }
                        else
                        {
                            Console.WriteLine("CONSOLE: Unknown command.");
                            continue;
                        }
                    }
                    catch (Exception e)
                    {
                        Server.ErrorLog(e);
                        Console.WriteLine("CONSOLE: Failed command.");
                        continue;
                    }

                talk:
                    if (!Chat.HandleModes(null, s)) {
                        msg = String.Format("{0}Console [&a{1}{0}]: &f{2}", Server.DefaultColor, Server.ZallState, s);
                        Player.GlobalMessage(msg);
                        Server.IRC.Say(msg);
                        WriteToConsole(msg);
                    }
                }
                catch (Exception ex)
                {
                    Server.ErrorLog(ex);
                }
            }
        }
    }
}

