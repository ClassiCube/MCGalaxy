/*
    Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCForge)
    
    Dual-licensed under the Educational Community License, Version 2.0 and
    the GNU General Public License, Version 3 (the "Licenses"); you may
    not use this file except in compliance with the Licenses. You may
    obtain a copy of the Licenses at
    
    https://opensource.org/license/ecl-2-0/
    https://www.gnu.org/licenses/gpl-3.0.html
    
    Unless required by applicable law or agreed to in writing,
    software distributed under the Licenses are distributed on an "AS IS"
    BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express
    or implied. See the Licenses for the specific language governing
    permissions and limitations under the Licenses.
 */
using System;
using System.IO;
using System.Reflection;
using System.Linq;
using System.Threading;
using System.Collections.Generic;
using MCGalaxy.UI;

namespace MCGalaxy.Cli {
    public static class Program {
        static bool interactive;

        [STAThread]
        public static void Main(string[] args) {
            interactive = args.Contains("--interactive");

            SetCurrentDirectory();

            // If MCGalaxy_.dll is missing, a FileNotFoundException will get thrown for MCGalaxy dll
            try {
                EnableCLIMode();
            } catch (FileNotFoundException ex) {
                Console.WriteLine("Cannot start server as {0} is missing from {1}",
                                  GetFilename(ex.FileName), Environment.CurrentDirectory);
                Console.WriteLine("Download from " + Updater.UploadsURL);
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey(true);
                return;
            }
            
            // separate method, in case MCGalaxy_.dll is missing
            StartCLI();
        }
        
        static string GetFilename(string rawName) {
            try {
                return new AssemblyName(rawName).Name + ".dll";
            } catch {
                return rawName;
            }
        }
        
        static void SetCurrentDirectory() {
#if !MCG_STANDALONE
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
#endif
        }
        
        static void EnableCLIMode() {
            try {
                Server.CLIMode = true;
            } catch {
                // in case user is running CLI with older MCGalaxy dll which lacked CLIMode field
            }
        	
#if !MCG_STANDALONE
            Server.RestartPath = Assembly.GetEntryAssembly().Location;
#endif
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

                CheckNameVerification();
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
        

        static void CheckNameVerification() {
            if (Server.Config.VerifyNames) return;
            Write("&e==============================================");
            Write("&eWARNING: Name verification is disabled! This means players can login as anyone, including YOU");
            Write("&eUnless you know EXACTLY what you are doing, you should change verify-names to true in server.properties");
            Write("&e==============================================");
        }

        static void LogNewerVersionDetected(object sender, EventArgs e) {
            Write("&cMCGalaxy update available! Update by replacing with the files from " + Updater.UploadsURL);
        }

        static int cursortY = 0;

        static void RedrawLine(string buffer, int position, int trailingSpaces) {
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write($"{buffer}{new string(' ', Math.Max(trailingSpaces, 0))}");
            Console.SetCursorPosition(position, Console.CursorTop);
        }

        static void MoveCursor(int direction) {
            Console.SetCursorPosition(Console.CursorLeft + direction, Console.CursorTop);
        }

        // [current input] [history]
        static List<string> commandHistory = new List<string>();
        static int maxHistory = 50;
        static int historyPosition = 0;

        static string ReadLine() {
            if(commandHistory.Count == 0) {
                commandHistory.Add("");
            } else {
                commandHistory[0] = "";
            }

            int position = 0;
            ConsoleKeyInfo keyInfo;

            while (true) {
                keyInfo = Console.ReadKey(true);

                switch(keyInfo.Key) {
                    case ConsoleKey.Enter:
                        Console.WriteLine();
                        if(commandHistory[0].Trim().Length != 0) {
                            string buffer = commandHistory[0];
                            commandHistory[0] = "";
                            commandHistory.Remove(buffer);
                            commandHistory.Insert(1, buffer);
                            if(commandHistory.Count > maxHistory){
                                commandHistory.RemoveAt(commandHistory.Count - 1);
                            }
                            return buffer;
                        }
                        return commandHistory[0];
                    case ConsoleKey.Backspace:
                        if(historyPosition != 0) {
                            commandHistory[0] = commandHistory[historyPosition];
                            historyPosition = 0;
                        }
                        if (position > 0) {
                            commandHistory[0] = commandHistory[0].Remove(position - 1, 1);
                            position--;
                            RedrawLine(commandHistory[0], position, 1);
                        }
                        break;
                    case ConsoleKey.LeftArrow:
                        if (position > 0) {
                            position--;
                            Console.SetCursorPosition(position, Console.CursorTop);
                        }
                        break;
                    case ConsoleKey.RightArrow:
                        if (position < commandHistory[0].Length) {
                            position++;
                            Console.SetCursorPosition(position, Console.CursorTop);
                        }
                        break;
                    case ConsoleKey.UpArrow:
                        if (historyPosition < commandHistory.Count - 1) {
                            int oldLength = commandHistory[historyPosition].Length;
                            historyPosition++;
                            position = commandHistory[historyPosition].Length;
                            RedrawLine(commandHistory[historyPosition], position, oldLength - position);
                        }
                        break;
                    case ConsoleKey.DownArrow:
                        if (historyPosition > 0) {
                            int oldLength = commandHistory[historyPosition].Length;
                            historyPosition--;
                            position = commandHistory[historyPosition].Length;
                            RedrawLine(commandHistory[historyPosition], position, oldLength - position);
                        }
                        break;
                    default:
                        if(historyPosition != 0) {
                            commandHistory[0] = commandHistory[historyPosition];
                            historyPosition = 0;
                        }
                        if(position++ == commandHistory[0].Length) {
                            commandHistory[0] += keyInfo.KeyChar;
                            Console.Write(keyInfo.KeyChar);
                        } else {
                            commandHistory[0] = commandHistory[0].Insert(position - 1, keyInfo.KeyChar.ToString());
                            RedrawLine(commandHistory[0], position, 0);
                        }
                        break;
                }
            }
        }

        static void ConsoleLoop() {
            int eofs = 0;
            while (true) {
                try {
                    string msg;

                    if(interactive) {
                        msg = ReadLine();
                    } else {
                        msg = Console.ReadLine();
                    }
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
                } catch (UnauthorizedAccessException) {
                    // UnauthorizedAccessException can get thrown when stdin is unreadable
                    // See https://github.com/dotnet/runtime/issues/21913 for instance
                     Write("&e** Access denied to stdin, console no longer accepts input **"); 
                     Write("&e** If nohup is being used, remove that to avoid this issue **"); 
                     break;
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
            
            while (index < message.Length) 
            {
                char curCol = col;
                string part = UIHelpers.OutputPart(ref col, ref index, message);

                if (part.Length == 0) continue;
                ConsoleColor color = GetConsoleColor(curCol);

                if (color == ConsoleColor.White) {
                    // show in user's preferred console text color
                    Console.ResetColor(); 
                } else { 
                    Console.ForegroundColor = color; 
                }
                Console.Write(part);
            }

            Console.ResetColor();
            Console.WriteLine();
        }

        static ConsoleColor GetConsoleColor(char c) {
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
                    return GetConsoleColor(Colors.Get(c).Fallback);
            }
        }
    }
}
