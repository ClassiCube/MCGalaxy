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
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using MCGalaxy;

namespace MCGalaxy.Gui {
    public static class Program {

        [DllImport("kernel32")]
        static extern IntPtr GetConsoleWindow();
        
        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        
        internal static LevelPermission GetPermission(ComboBox box, LevelPermission defPerm) {
            Group grp = Group.Find(box.SelectedItem.ToString());
            return grp == null ? defPerm : grp.Permission;
        }

        // NOTE: //Console.ReadLine() is ignored while Starter is set as Windows Application in properties. (At least on Windows)
        static bool useConsole, useHighQualityGui, isWindows;
        [STAThread]
        public static void Main(string[] args) {
            Environment.CurrentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            if (!File.Exists("MCGalaxy_.dll")) {
                Console.WriteLine("MCGalaxy_.dll is missing from the folder");
                Console.WriteLine("Go to https://github.com/Hetal728/MCGalaxy yourself and download it, please");
                Console.WriteLine("Place it in the same folder.");
                Console.WriteLine("If you have any issues, get the files from the https://github.com/Hetal728/MCGalaxy download page and try again.");
                Console.WriteLine("Press any key to close me...");
                Console.ReadLine();
                Console.WriteLine("Bye!");
                return;
            }
            
            DateTime startTime = DateTime.UtcNow;
            Logger.Init();
            AppDomain.CurrentDomain.UnhandledException += GlobalExHandler;
            useHighQualityGui = false;
            
            PlatformID platform = Environment.OSVersion.Platform;
            isWindows = platform == PlatformID.Win32NT || platform == PlatformID.Win32Windows;
            useConsole = !isWindows;
            
            try {
                ReadViewmode();
                if (useConsole) {
                    RunCli();
                } else {
                    RunGui();
                }
                WriteToConsole("Completed in " + (DateTime.UtcNow - startTime).Milliseconds + "ms");
            }
            catch (Exception e) { Server.ErrorLog(e); }
        }
        
        static void RunCli() {
            Server s = new Server();
            s.OnLog += WriteToConsole;
            s.OnCommand += WriteToConsole;
            s.OnSystem += delegate { };
            MCGalaxy.Gui.App.usingConsole = true;
            
            s.Start();
            Console.Title = Server.name + " - MCGalaxy " + Server.Version;            
            ConsoleLoop();
        }
        
        static void RunGui() {
            Application.ThreadException += ThreadExHandler;
            if (isWindows) { // get rid of console window on Windows
                IntPtr hConsole = GetConsoleWindow();
                if (IntPtr.Zero != hConsole) ShowWindow(hConsole, 0);
            }
            
            if (useHighQualityGui) {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
            }

            Application.Run(new Window());
        }
        
        static void ReadViewmode() {
            if (!File.Exists("Viewmode.cfg")) {
                using (StreamWriter w = new StreamWriter("Viewmode.cfg")) {
                    w.WriteLine("#This file controls how the console window is shown to the server host");
                    w.WriteLine("#cli: True or False (Determines whether a CLI interface is used) (Set True if on Mono)");
                    w.WriteLine("#high-quality: True or false (Determines whether the GUI interface uses higher quality objects)");
                    w.WriteLine();
                    w.WriteLine("cli = " + useConsole);
                    w.WriteLine("high-quality = true");
                }
            }
            PropertiesFile.Read("Viewmode.cfg", ViewmodeLineProcessor);
        }

        static void ViewmodeLineProcessor(string key, string value) {
            switch (key.ToLower()) {
                case "cli":
        	        useConsole = value.CaselessEq("true"); break;
                case "high-quality":
        	        useHighQualityGui = value.CaselessEq("true"); break;
            }
        }

        static void WriteToConsole(string message) {
            LineFormatter.Format(message, LineFormatter.FormatConsole);
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine();
        }

        static void ConsoleLoop() {
            while (true) {
                try {
                    string msg = Console.ReadLine().Trim(); // Make sure we have no whitespace!
                    if (msg.Length > 0 && msg[0] == '/') {
                        msg = msg.Remove(0, 1);
                        Thread t = Handlers.HandleCommand(msg, Console.WriteLine);
                        if (msg.CaselessEq("restart")) { t.Join(); break; }
                    } else {
                        Handlers.HandleChat(msg, WriteToConsole);
                    }
                } catch (Exception ex) {
                    Server.ErrorLog(ex);
                }
            }
        }
        
        
        static void GlobalExHandler(object sender, UnhandledExceptionEventArgs e) {
            Exception ex = (Exception)e.ExceptionObject;
            Server.ErrorLog(ex);
            Thread.Sleep(500);

            if (Server.restartOnError)
                App.ExitProgram(true);
        }

        static void ThreadExHandler(object sender, ThreadExceptionEventArgs e) {
            Exception ex = e.Exception;
            Server.ErrorLog(ex);
            Thread.Sleep(500);

            if (Server.restartOnError)
                App.ExitProgram(true);
        }
    }
}

