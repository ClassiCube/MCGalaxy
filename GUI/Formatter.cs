/*
    Copyright 2015 MCGalaxy
        
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
using System.Drawing;
using MCGalaxy.Gui.Components;

namespace MCGalaxy.Gui {
    public static class Formatter {
        
        public static void Format(string message, Action<char, string> output) {
            int index = 0;
            char col = 'S';
            while (index < message.Length)
                OutputPart(ref col, ref index, message, output);
        }
        
        static void OutputPart(ref char col, ref int start, 
                                   string message, Action<char, string> output) {
            int next = Next(start, message);
            if (next == -1) {
                string part = message.Substring(start);
                if (part.Length > 0) output(col, part);
                start = message.Length;
            } else {
                string part = message.Substring(start, next - start);
                if (part.Length > 0) output(col, part);
                start = next + 2; 
                col = message[next + 1];
            }
        }
        
        static int Next(int start, string message) {
            for (int i = start; i < message.Length; i++) {
				if (!(message[i] == '&' || message[i] == '%')) continue;
				// No colour code follows this
				if (i == message.Length - 1) return -1;
				
                // Check following character is an actual colour code
                char col = message[i + 1];
                if (Colors.MapColor(ref col)) return i;
            }
            return -1;
        }        
        
        public static void FormatConsole(char col, string message) {
            Console.ForegroundColor = GetConsoleCol(col);
            Console.Write(message);
        }

        static ConsoleColor GetConsoleCol(char c) {
            if (c == 's' || c == 'S') 
                return ConsoleColor.White;
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
                case 'a': return ConsoleColor.Green;
                case 'b': return ConsoleColor.Cyan;
                case 'c': return ConsoleColor.Red;
                case 'd': return ConsoleColor.Magenta;
                case 'e': return ConsoleColor.Yellow;
                case 'f': return ConsoleColor.White;
                default:
                    char fallback = Colors.GetFallback(c);
                    return fallback == '\0' ? 
                        ConsoleColor.White : GetConsoleCol(fallback);
            }
        }
        
        public static void FormatGui(char col, string message, 
                                     ColoredTextBox box, Color foreCol) {
            box.Append(message, GetGuiCol(col, foreCol));
        }
        
        static Color GetGuiCol(char c, Color foreCol) {
            if (c == 's' || c == 'S') return foreCol;
            Colors.MapColor(ref c);
            
            switch (c) {
                case '0': return Color.Black;
                case '1': return Color.FromArgb( 255, 0, 0, 161 );
                case '2': return Color.FromArgb( 255, 0, 161, 0 );
                case '3': return Color.FromArgb( 255, 0, 161, 161 );
                case '4': return Color.FromArgb( 255, 161, 0, 0 );
                case '5': return Color.FromArgb( 255, 161, 0, 161 );
                case '6': return Color.FromArgb( 255, 161, 161, 0 );
                case '7': return Color.FromArgb( 255, 161, 161, 161 );
                case '8': return Color.FromArgb( 255, 34, 34, 34 );
                case '9': return Color.FromArgb( 255, 34, 34, 225 );
                case 'a': return Color.FromArgb( 255, 34, 225, 34 );
                case 'b': return Color.FromArgb( 255, 34, 225, 225 );
                case 'c': return Color.FromArgb( 255, 225, 34, 34 );
                case 'd': return Color.FromArgb( 255, 225, 34, 225 );
                case 'e': return Color.FromArgb( 255, 225, 225, 34 );
                case 'f': return Color.Black;
                
                default:
                    char fallback = Colors.GetFallback(c);
                    if (fallback == '\0') return foreCol;
                    
                    CustomColor col = Colors.ExtColors[c];
                    return Color.FromArgb(col.R, col.G, col.B);
            }
        }
    }
}
