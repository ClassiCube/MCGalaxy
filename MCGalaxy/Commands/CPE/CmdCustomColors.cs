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

namespace MCGalaxy.Commands.CPE {    
    public sealed class CmdCustomColors : Command2 {        
        public override string name { get { return "CustomColors"; } }
        public override string shortcut { get { return "ccols"; } }
        public override string type { get { return CommandTypes.Chat; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Admin; } }

        public override void Use(Player p, string message, CommandData data) {
            string[] args = message.SplitSpaces();
            if (message.Length == 0) { Help(p); return; }
            string cmd = args[0];
            
            if (IsCreateCommand(cmd)) {
                AddHandler(p, args);
            } else if (IsDeleteCommand(cmd)) {
                RemoveHandler(p, args);
            } else if (IsEditCommand(cmd)) {
                EditHandler(p, args);
            } else if (IsListCommand(cmd)) {
                string modifer = args.Length > 1 ? args[1] : "";
                ListHandler(p, "ccols list", modifer);
            } else {
                Help(p);
            }
        }
        
        void AddHandler(Player p, string[] args) {
            if (args.Length <= 4) { Help(p); return; }         
            char code = args[1][0];
            if (code >= 'A' && code <= 'F') code += ' ';
            
            if (code == ' ' || code == '\0' || code == '\u00a0' || code == '%' || code == '&') {
                p.Message("&WColor code cannot be a space, percentage, or ampersand.");
                return;
            }            
            if (Colors.IsSystem(code)) {
                p.Message("&WCannot change system defined color codes using %T/CustomColors");
                return;
            }
            
            char fallback;
            if (!CheckName(p, args[2]) || !CheckFallback(p, args[3], code, out fallback)) return;
            ColorDesc col = default(ColorDesc);
            if (!CommandParser.GetHex(p, args[4], ref col)) return;
            
            col.Code = code; col.Fallback = fallback; col.Name = args[2];
            Colors.Update(col);
            p.Message("Successfully added '{0}' color", code);
        }
        
        void RemoveHandler(Player p, string[] args) {
            if (args.Length < 2) { Help(p); return; }
            
            char code = ParseColor(p, args[1]);
            if (code == '\0') return;
            
            Colors.Update(Colors.DefaultCol(code));
            p.Message("Successfully removed '{0}' color", code);
        }
        
        static void ListHandler(Player p, string cmd, string modifier) {
            List<ColorDesc> validCols = new List<ColorDesc>(Colors.List.Length);
            foreach (ColorDesc col in Colors.List) {
                if (col.IsModified()) validCols.Add(col);
            }
            MultiPageOutput.Output(p, validCols, FormatColor, cmd, "Colors", modifier, true);
        }
        
        // Not very elegant, because we don't want the % to be escaped like everywhere else
        internal static string FormatColor(ColorDesc col) {
            string format = "{0} &{1}({2})&S - %&S{1}, falls back to &{3}%&{3}{3}";
            if (col.Code == col.Fallback) format = "{0} &{1}({2})&S - %&S{1}";

            return string.Format(format, col.Name, col.Code, Utils.Hex(col.R, col.G, col.B), col.Fallback);
        }
        
        void EditHandler(Player p, string[] args) {
            if (args.Length < 4) { Help(p); return; }
            
            char code = ParseColor(p, args[1]);
            if (code == '\0') return;
            ColorDesc col = Colors.Get(code);
            
            if (args[2].CaselessEq("name")) {
                if (!CheckName(p, args[3])) return;

                p.Message("Set name of {0} to {1}", col.Name, args[3]);
                col.Name = args[3];
            } else if (args[2].CaselessEq("fallback")) {
                char fallback;
                if (!CheckFallback(p, args[3], code, out fallback)) return;
                
                p.Message("Set fallback of {0} to %&S{1}", col.Name, fallback);
                col.Fallback = fallback;
            } else if (args[2].CaselessEq("hex") || args[2].CaselessEq("color")) {
                ColorDesc rgb = default(ColorDesc);
                if (!CommandParser.GetHex(p, args[3], ref rgb)) return;
                
                p.Message("Set hex color of {0} to {1}", col.Name, Utils.Hex(rgb.R, rgb.G, rgb.B));
                col.R = rgb.R; col.G = rgb.G; col.B = rgb.B;
            } else {
                Help(p); return;
            }
            
            Colors.Update(col);
        }
        
        
        static bool CheckName(Player p, string arg) {
            if (Colors.Parse(arg).Length > 0) {
                p.Message("There is already an existing color named \"{0}\".", arg);
                return false;
            }
            return true;
        }
        
        static char ParseColor(Player p, string arg) {
            if (arg.Length != 1) {
                string colCode = Matcher.FindColor(p, arg);
                if (colCode != null) return colCode[1];
            } else {
                char code = arg[0];
                if (Colors.IsDefined(code)) return code;
                
                p.Message("There is no color with the code {0}.", code);
                p.Message("Use &T/CustomColors list &Sto see a list of colors.");
            }
            return '\0';
        }
        
        static bool CheckFallback(Player p, string arg, char code, out char fallback) {
            fallback = arg[0];
            if (!Colors.IsStandard(fallback)) {
                p.Message("{0} must be a standard color code.", fallback); return false;
            }
            // Can't change fallback of standard colour code
            if (Colors.IsStandard(code)) fallback = code;
            
            if (fallback >= 'A' && fallback <= 'F') fallback += ' ';
            return true;
        }
        
        public override void Help(Player p) {
            p.Message("&T/CustomColors add [code] [name] [fallback] [hex]");
            p.Message("&H  code is a single character.");
            p.Message("&H  fallback is the color code shown to non-supporting clients.");
            p.Message("&T/CustomColors remove [code] &H- Removes that custom color.");
            p.Message("&T/CustomColors list [offset] &H- lists all custom colors.");
            p.Message("&T/CustomColors edit [code] [name/fallback/hex] [value]");
        }
    }
}
