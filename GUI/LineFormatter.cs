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
using System.Collections.Generic;
using System.Drawing;
using MCGalaxy.Gui.Components;

namespace MCGalaxy.Gui {
    public static class LineFormatter {
        
        public static void Format(string message, Action<char, string> output) {
            int index = 0;
            char col = 'S';
            message = Colors.Escape(message);
            
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
                if (message[i] != '&') continue;
                // No colour code follows this
                if (i == message.Length - 1) return -1;
                
                // Check following character is an actual colour code
                char col = message[i + 1];
                if (Colors.Map(ref col)) return i;
            }
            return -1;
        }
        
        public static void FormatConsole(char col, string message) {
            Console.ForegroundColor = GetConsoleCol(col);
            Console.Write(message);
        }

        static ConsoleColor GetConsoleCol(char c) {
            if (c == 'S') return ConsoleColor.White;
            Colors.Map(ref c);
            
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
                    if (!Colors.IsDefined(c)) return ConsoleColor.White;
                    return GetConsoleCol(Colors.List[c].Fallback);
            }
        }
        
        public static void FormatGui(char col, string message,
                                     ColoredTextBox box, Color foreCol) {
            box.AppendColoredText(message, GetGuiCol(col, foreCol));
        }
        
        static Color GetGuiCol(char c, Color foreCol) {
            if (c == 'S' || c == 'f' || c == 'F' || c == '0') return foreCol;
            Colors.Map(ref c);

            if (!Colors.IsDefined(c)) return foreCol;
            ColorDesc col = Colors.List[c];
            return Color.FromArgb(col.R, col.G, col.B);
        }
        
        public static List<string> GetColorsList() {
            List<string> colors = new List<string>();
            for (int i = 0; i < Colors.List.Length; i++) {
                if (Colors.List[i].Undefined) continue;
                colors.Add(Colors.List[i].Name);
            }
            return colors;
        }
    }
}
