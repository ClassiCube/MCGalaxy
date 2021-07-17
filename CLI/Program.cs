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
using System.IO;
using System.Reflection;
using System.Threading;
using MCGalaxy.UI;

namespace MCGalaxy.Cli {
    public static class Program {

        [STAThread]
        public static void Main(string[] args) {
            SetCurrentDirectory();

            // If MCGalaxy_.dll is missing, a FileNotFoundException will get thrown for MCGalaxy dll
            try {
                EnableCLIMode();
            } catch (FileNotFoundException) {
                Console.WriteLine("Cannot start server as MCGalaxy_.dll is missing from " + Environment.CurrentDirectory);
                Console.WriteLine("Download from " + Updater.UploadsURL);
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey(true);
                return;
            }
            
            // separate method, in case MCGalaxy_.dll is missing
            StartCLI();
        }
        
        static void SetCurrentDirectory() {
            string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            try {
                Environment.CurrentDirectory = path;
            } catch {
                // assembly.Location usually gives full path of the .exe, but has issues with mkbundle
                //   https://mono-devel-list.ximian.narkive.com/KfCAxY1F/mkbundle-assembly-getentryassembly
                //   https://stackoverflow.com/questions/57648241/reliably-get-location-of-bundled-executable-on-linux
                // Rather than trying to guess when this issue happens, just don't bother at all
                //  (since most users will not be trying to run .exe from a different folder anyways)
                Console.WriteLine("Failed to set working directory to '{0}', running in current directory..", path);
            }
        }
        
        static void EnableCLIMode() {
            try {
                Server.CLIMode = true;
            } catch {
                // in case user is running CLI with older MCGalaxy dll which lacked CLIMode field
            }
            Server.RestartPath = Assembly.GetEntryAssembly().Location;
        }
        
        
        static void StartCLI() {
            FileLogger.Init();
            AppDomain.CurrentDomain.UnhandledException += GlobalExHandler;
            
            try {
                Logger.LogHandler += LogMessage;
                Updater.NewerVersionDetected += LogNewerVersionDetected;
                
                EnableCLIMode();
                Server.Start();
                Console.Title = Server.Config.Name + " - " + Server.SoftwareNameVersioned;
                Console.CancelKeyPress += OnCancelKeyPress;
                ConsoleLoop();
            } catch (Exception e) {
                Logger.LogError(e);
                FileLogger.Flush(null);
            }
        }
        
        static void OnCancelKeyPress(object sender, ConsoleCancelEventArgs e) {
            switch (e.SpecialKey) {
                case ConsoleSpecialKey.ControlBreak:
                    // Cannot set e.Cancel for this one
                    Write("&e-- Server shutdown (Ctrl+Break) --");
                    Thread stopThread = Server.Stop(false, Server.Config.DefaultShutdownMessage);
                    stopThread.Join();
                    break;
                    
                case ConsoleSpecialKey.ControlC:
                    e.Cancel = true;
                    Write("&e-- Server shutdown (Ctrl+C) --" );
                    Server.Stop(false, Server.Config.DefaultShutdownMessage);
                    break;
            }
        }
        
        static void LogAndRestart(Exception ex) {
            Logger.LogError(ex);
            FileLogger.Flush(null);
            
            Thread.Sleep(500);
            if (Server.Config.restartOnError) {
                Thread stopThread = Server.Stop(true, "Server restart - unhandled error");
                stopThread.Join();
            }
        }
        
        static void GlobalExHandler(object sender, UnhandledExceptionEventArgs e) {
            LogAndRestart((Exception)e.ExceptionObject);
        }
        
        
        static string CurrentDate() { return DateTime.Now.ToString("(HH:mm:ss) "); }

        static void LogMessage(LogType type, string message) {
            if (!Server.Config.ConsoleLogging[(int)type]) return;
            
            switch (type) {
                case LogType.Error:
                    Write("&c!!!Error" + ExtractErrorMessage(message)
                          + " - See " + FileLogger.ErrorLogPath + " for more details.");
                    break;
                case LogType.BackgroundActivity:
                    // ignore these messages
                    break;
                case LogType.Warning:
                    Write("&e" + CurrentDate() + message);
                    break;
                default:
                    Write(CurrentDate() + message);
                    break;
            }
        }
        
        static string msgPrefix = Environment.NewLine + "Message: ";
        static string ExtractErrorMessage(string raw) {
            // Error messages are usually structured like so:
            //   Type: whatever
            //   Message: whatever
            //   Something: whatever
            // this code extracts the Message line from the raw message
            int beg = raw.IndexOf(msgPrefix);
            if (beg == -1) return "";
            
            beg += msgPrefix.Length;
            int end = raw.IndexOf(Environment.NewLine, beg);
            if (end == -1) return "";
            
            return " (" + raw.Substring(beg, end - beg) + ")";
        }
        

        static void LogNewerVersionDetected(object sender, EventArgs e) {
            Write("&cMCGalaxy update available! Update by replacing with the files from " + Updater.UploadsURL);
        }
        
        static void ConsoleLoop() {
            int eofs = 0;
            while (true) {
                try {
                    string msg = Console.ReadLine();
                    // null msg is triggered in two cases:
                    //   a) when pressing Ctrl+C to shutdown CLI on Windows
                    //   b) underlying terminal provides a bogus EOF
                    // b) actually happens very rarely (e.g. a few times on startup with wine mono),
                    // so ignore the first few EOFs to workaround this case
                    if (msg == null) {
                        eofs++;
                        if (eofs >= 15) { Write("&e** EOF, console no longer accepts input **"); break; }
                        continue;
                    }
                    
                    msg = msg.Trim();
                    if (msg == "/") {
                        UIHelpers.RepeatCommand();
                    } else if (msg.Length > 0 && msg[0] == '/') {
                        UIHelpers.HandleCommand(msg.Substring(1));
                    } else {
                        UIHelpers.HandleChat(msg);
                    }
                } catch (Exception ex) {
                    // ArgumentException is raised on Mono when you:
                    //  1) Type a message into a large CLI window
                    //  2) Resize the CLI window to be smaller
                    //  3) Try to backspace when the message is bigger than the smaller resized CLI window
                    Logger.LogError(ex);
                }
            }
        }
        
        static void Write(string message) {
            int index = 0;
            char col = 'S';
            message = UIHelpers.Format(message);
            
            while (index < message.Length) {
                char curCol = col;
                string part = UIHelpers.OutputPart(ref col, ref index, message);
                if (part.Length > 0) {
                    Console.ForegroundColor = GetConsoleCol(curCol);
                    Console.Write(part);
                }
            }
            
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine();
        }

        static ConsoleColor GetConsoleCol(char c) {
            if (c == 'S') return ConsoleColor.White;
            Colors.Map(ref c);
            
            switch (c) {
                    case '0': return ConsoleColor.DarkGray; // black text on black background is unreadable
                    case '1': return ConsoleColor.DarkBlue;
                    case '2': return ConsoleColor.DarkGreen;
                    case '3': return ConsoleColor.DarkCyan;
                    case '4': return ConsoleColor.DarkRed;
                    case '5': return ConsoleColor.DarkMagenta;
                    case '6': return ConsoleColor.DarkYellow;
                    case '7': return ConsoleColor.Gray;
                    case '8': return ConsoleColor.DarkGray;
                    case '9': return ConsoleColor.Blue;
                    case 'a': return ConsoleColor.Green;
                    case 'b': return ConsoleColor.Cyan;
                    case 'c': return ConsoleColor.Red;
                    case 'd': return ConsoleColor.Magenta;
                    case 'e': return ConsoleColor.Yellow;
                    case 'f': return ConsoleColor.White;
                    
                default:
                    if (!Colors.IsDefined(c)) return ConsoleColor.White;
                    return GetConsoleCol(Colors.Get(c).Fallback);
            }
        }
    }
}
