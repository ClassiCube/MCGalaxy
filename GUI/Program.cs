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
        public static void GlobalExHandler(object sender, UnhandledExceptionEventArgs e) {
            Exception ex = (Exception)e.ExceptionObject;
            Server.ErrorLog(ex);
            Thread.Sleep(500);

            if (Server.restartOnError)
                App.ExitProgram(true);
        }

        public static void ThreadExHandler(object sender, ThreadExceptionEventArgs e) {
            Exception ex = e.Exception;
            Server.ErrorLog(ex);
            Thread.Sleep(500);

            if (Server.restartOnError)
                App.ExitProgram(true);
        }

        static bool useConsole, useHighQualityGui;
        [STAThread]
        public static void Main(string[] args) {
            DateTime startTime = DateTime.Now;
            Process[] duplicates = Process.GetProcessesByName("MCGalaxy");
            if (duplicates.Length != 1) {
                Process proc = Process.GetCurrentProcess();
                foreach (Process pr in duplicates) {
                    if (pr.MainModule.BaseAddress == proc.MainModule.BaseAddress)
                        if (pr.Id != proc.Id) pr.Kill();
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
                    s.OnSystem += delegate { };
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
                    Application.Run(new Window());
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

        static void WriteToConsole(string message) {
            Formatter.Format(message, Formatter.FormatConsole);
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine();
        }

        public static void handleComm() {
            while (true) {
                try {
                    string s = Console.ReadLine().Trim(); // Make sure we have no whitespace!
                    if (s.Length > 0 && s[0] == '/') {
                        s = s.Remove(0, 1);
                        Thread t = Handlers.HandleCommand(s, Console.WriteLine);
                        if (s.CaselessEq("restart")) { t.Join(); break; }
                    } else {
                        Handlers.HandleChat(s, WriteToConsole);
                    }
                } catch (Exception ex) {
                    Server.ErrorLog(ex);
                }
            }
        }
    }
}

