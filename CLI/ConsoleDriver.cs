/*
    Copyright 2015 MCGalaxy
    
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
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using MCGalaxy.UI;
using MCGalaxy.Platform;

namespace MCGalaxy.Cli
{
    public abstract class ConsoleDriver
    {
        public Action<ConsoleCancelEventArgs> OnSpecialInput;
        
        public abstract void Init();
        
        public abstract void SetTitle(string title);
        
        public abstract string ReadLine();
        
        public abstract void WriteColored(string msg);
        

        protected static ConsoleColor GetConsoleColor(char c) {
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
        
        
        public static ConsoleDriver Create() {
            try {
                IOperatingSystem os = IOperatingSystem.DetectOS();
                if (!os.IsWindows) return new UnixConsoleDriver();
            } catch { }
            
            return new DefaultConsoleDriver();
        }
    }
    
    sealed class DefaultConsoleDriver : ConsoleDriver
    {
        public override void Init() {
            Console.CancelKeyPress += OnCancelKeyPress;
        }

        void OnCancelKeyPress(object sender, ConsoleCancelEventArgs e) {
            OnSpecialInput(e);
        }
        
        public override void SetTitle(string title) {
            Console.Title = title;
        }
        
        public override string ReadLine() {
            return Console.ReadLine();
        }
        
        public override void WriteColored(string message) {
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
    }
    
    sealed class UnixConsoleDriver : ConsoleDriver
    {
        Encoding encoding;
        
        public override void Init() {
            encoding = new UTF8Encoding(false);
        }
        
        public override void SetTitle(string title) {
            string msg = "\x1b]0;" + title + "\x7";
            WriteString(msg);
        }      
        
        public override string ReadLine() {
            return Console.ReadLine();
        }
        
        public override void WriteColored(string message) {
            StringBuilder sb = new StringBuilder();
            int index = 0;
            char col  = 'S';
            message   = UIHelpers.Format(message);
            
            while (index < message.Length)
            {
                char curCol = col;
                string part = UIHelpers.OutputPart(ref col, ref index, message);

                if (part.Length == 0) continue;
                ConsoleColor color = GetConsoleColor(curCol);

                if (color == ConsoleColor.White) {
                    // show in user's preferred console text color
                    sb.Append("\x1b[0m"); // reset all attributes
                } else {
                    int code = MapColorCode(color);
                    sb.Append("\x1b[").Append(code).Append("m");
                }
                sb.Append(part);
            }

            sb.Append("\x1b[0m"); // reset all attributes
            sb.Append("\n");
            WriteColored(sb.ToString());
        }
        
        static int MapColorCode(ConsoleColor color) {
            switch (color)
            {
                case ConsoleColor.Black:       return 30;
                case ConsoleColor.DarkRed:     return 31;
                case ConsoleColor.DarkGreen:   return 32;
                case ConsoleColor.DarkYellow:  return 33;
                case ConsoleColor.DarkBlue:    return 34;
                case ConsoleColor.DarkMagenta: return 35;
                case ConsoleColor.DarkCyan:    return 36;
                case ConsoleColor.Gray:        return 37;

                case ConsoleColor.DarkGray: return 90;
                case ConsoleColor.Red:      return 91;
                case ConsoleColor.Green:    return 92;
                case ConsoleColor.Yellow:   return 93;
                case ConsoleColor.Blue:     return 94;
                case ConsoleColor.Magenta:  return 95;
                case ConsoleColor.Cyan:     return 96;
                case ConsoleColor.White:    return 97;
            }
            return 97;
        }
        
        void WriteString(string msg) {
            byte[] data = encoding.GetBytes(msg);
            write(1, data, data.Length);
        }
        
        [DllImport("libc", SetLastError = true)]
        static extern int write(int fd, byte[] data, int len);
    }
}
