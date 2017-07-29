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

namespace MCGalaxy.Commands.CPE {
    
    public sealed class CmdCustomColors : Command {
        
        public override string name { get { return "customcolors"; } }
        public override string shortcut { get { return "ccols"; } }
        public override string type { get { return CommandTypes.Chat; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Admin; } }

        public override void Use(Player p, string message) {
            string[] args = message.SplitSpaces();
            if (message == "") { Help(p); return; }
            
            switch (args[0].ToLower()) {
                case "add":
                case "create":
                    AddHandler(p, args); break;
                case "remove":
                case "delete":
                    RemoveHandler(p, args); break;
                case "list":
                    string modifer = args.Length > 1 ? args[1] : "";
                    ListHandler(p, "ccols list", modifer); break;
                case "edit":
                case "modify":
                    EditHandler(p, args); break;
                default:
                    Help(p); break;
            }
        }
        
        void AddHandler(Player p, string[] args) {
            if (args.Length <= 4) { Help(p); return; }
            
            char code = args[1][0];
            if (Colors.IsStandard(code)) {
                Player.Message(p, "You may only edit standard codes using %T/ccols edit"); return;
            }
            if (code <= ' ' || code > '~' || code == '%' || code == '&') {
                Player.Message(p, "{0} must be a standard ASCII character.", code);
                Player.Message(p, "It also cannot be a space, percentage, or ampersand.");
                return;
            }
            
            char code2 = code;
            if (Colors.Map(ref code2)) {
                Player.Message(p, "There is already an existing or server defined color with the code " + code +
                               ", you must either use a different code or use \"%T/ccols remove " + code + "%S\"");
                return;
            }
            
            char fallback;
            if (!CheckName(p, args[2]) || !CheckFallback(p, args[3], code, out fallback)) return;
            ColorDesc col = default(ColorDesc);
            if (!CommandParser.GetHex(p, args[4], ref col)) return;
            
            col.Code = code; col.Fallback = fallback; col.Name = args[2];
            Colors.Update(col);
            Player.Message(p, "Successfully added a color.");
        }
        
        void RemoveHandler(Player p, string[] args) {
            if (args.Length < 2) { Help(p); return; }
            
            char code = ParseColor(p, args[1]);
            if (code == '\0') return;
            
            Colors.Update(Colors.DefaultCol(code));
            Player.Message(p, "Successfully removed a color.");
        }
        
        static void ListHandler(Player p, string cmd, string modifier) {
            List<ColorDesc> validCols = new List<ColorDesc>(Colors.List.Length);
            foreach (ColorDesc col in Colors.List) {
                if (!col.Undefined) validCols.Add(col);
            }
            MultiPageOutput.Output(p, validCols, FormatColor, cmd, "colors", modifier, true);
        }
        
        // Not very elegant, because we don't want the % to be escaped like everywhere else
        internal static string FormatColor(ColorDesc col) {
            string format = "{0} &{1}({2})%S - %&S{1}, falls back to &{3}%&{3}{3}";
            if (col.Code == col.Fallback) format = "{0} &{1}({2})%S - %&S{1}";

            return String.Format(format, col.Name, col.Code, Utils.Hex(col.R, col.G, col.B), col.Fallback);
        }
        
        void EditHandler(Player p, string[] args) {
            if (args.Length < 4) { Help(p); return; }
            
            char code = ParseColor(p, args[1]);
            if (code == '\0') return;            
            ColorDesc col = Colors.List[code];
            
            switch (args[2]) {
                case "name":
                    if (!CheckName(p, args[3])) return;                  

                    Player.Message(p, "Set name of {0} to {1}", col.Name, args[3]);
                    col.Name = args[3];break;
                case "fallback":
                    char fallback;
                    if (!CheckFallback(p, args[3], code, out fallback)) return;                    
                    
                    Player.Message(p, "Set fallback of {0} to %&S{1}", col.Name, fallback);
                    col.Fallback = fallback; break;
                case "hex":
                case "color":
                    ColorDesc rgb = default(ColorDesc);
                    if (!CommandParser.GetHex(p, args[3], ref rgb)) return;
                    
                    Player.Message(p, "Set hex color of {0} to {1}", col.Name, Utils.Hex(rgb.R, rgb.G, rgb.B));
                    col.R = rgb.R; col.G = rgb.G; col.B = rgb.B; break;
                default:
                    Help(p); return;
            }
            
            Colors.Update(col);
        }
        
        
        static bool CheckName(Player p, string arg) {
            if (Colors.Parse(arg) != "") {
                Player.Message(p, "There is already an existing color with the name \"{0}\".", arg);
                return false;
            }
            return true;
        }
        
        static char ParseColor(Player p, string arg) {
            if (arg.Length != 1) {
                string colCode = Colors.Parse(arg);
                if (colCode != "") return colCode[1];
                
                Player.Message(p, "There is no color \"" + arg + "\".");
            } else {
                char code = arg[0];
                if (Colors.IsDefined(code)) return code;
                
                Player.Message(p, "There is no color with the code {0}.", code);
                Player.Message(p, "Use \"%T/ccols list\" %Sto see a list of colors.");
            }
            return '\0';
        }
        
        static bool CheckFallback(Player p, string arg, char code, out char fallback) {
            fallback = arg[0];
            if (!Colors.IsStandard(fallback)) {
                Player.Message(p, "{0} must be a standard color code.", fallback); return false;
            }
            if (Colors.IsStandard(code)) {
                Player.Message(p, "Cannot change fallback of a standard color code."); return false;
            }
            
            if (fallback >= 'A' && fallback <= 'F') fallback += ' ';
            return true;
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/customcolors add [code] [name] [fallback] [hex]");
            Player.Message(p, "%H  code is a single ascii character.");
            Player.Message(p, "%H  fallback is the color code shown to non-supporting clients.");
            Player.Message(p, "%T/customcolors remove [code] %H- Removes that custom color.");
            Player.Message(p, "%T/customcolors list [offset] %H- lists all custom colors.");
            Player.Message(p, "%T/customcolors edit [code] [name/fallback/hex] [value]");
        }
    }
}
