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
            string[] args = message.Split(' ');
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
            if (Colors.IsStandardColor(code)) {
                Player.Message(p, code + " is a standard color code, and thus cannot be replaced."); return;
            }
            if (code <= ' ' || code > '~' || code == '%' || code == '&') {
                Player.Message(p, "{0} must be a standard ASCII character.", code);
                Player.Message(p, "It also cannot be a space, percentage, or ampersand.");
                return;
            }
            
            char code2 = code;
            if (Colors.MapColor(ref code2)) {
                Player.Message(p, "There is already a custom or server defined color with the code " + code +
                               ", you must either use a different code or use \"%T/ccols remove " + code + "%S\"");
                return;
            }
            
            char fallback;
            if (!CheckName(p, args[2]) || !CheckFallback(p, args[3], out fallback)) return;
            CustomColor col = Colors.ParseHex(args[4]);
            if (!CommandParser.GetHex(p, args[4], ref col)) return;
            
            col.Code = code; col.Fallback = fallback; col.Name = args[2];
            Colors.AddExtColor(col);
            Player.Message(p, "Successfully added a custom color.");
        }
        
        void RemoveHandler(Player p, string[] args) {
            if (args.Length < 2) { Help(p); return; }
            
            char code = args[1][0];
            if (Colors.IsStandardColor(code)) {
                Player.Message(p,"{0} is a standard color, and thus cannot be removed.", code); return;
            }
            
            if ((int)code >= 256 || Colors.ExtColors[code].Undefined) {
                Player.Message(p, "There is no custom color with the code {0}.", code);
                Player.Message(p, "Use \"%T/ccols list\" %Sto see a list of custom colors.");
                return;
            }
            Colors.RemoveExtColor(code);
            Player.Message(p, "Successfully removed a custom color.");
        }
        
        internal static void ListHandler(Player p, string cmd, string modifier) {
            List<CustomColor> validCols = new List<CustomColor>(Colors.ExtColors.Length);
            foreach (CustomColor col in Colors.ExtColors) {
                if (col.Undefined) continue;
                validCols.Add(col);
            }
            MultiPageOutput.Output(p, validCols, FormatColor, cmd, "colors", modifier, true);
        }
        
        // Not very elegant, because we don't want the % to be escaped like everywhere else
        static string FormatColor(CustomColor col) {
            const string format = "{0} &{1}({2})%S - %&S{1}, falls back to &{3}%&{3}{3}";
            return String.Format(format, col.Name, col.Code, col.Hex(), col.Fallback);
        }
        
        void EditHandler(Player p, string[] args) {
            if (args.Length < 4) { Help(p); return; }
            
            char code = args[1][0];
            if (Colors.IsStandardColor(code)) {
                Player.Message(p, "{0} is a standard color, and thus cannot be edited.", code); return;
            }
            
            if ((int)code >= 256 || Colors.ExtColors[code].Undefined) {
                Player.Message(p, "There is no custom color with the code {0}.", code);
                Player.Message(p, "Use \"%T/ccols list\" %Sto see a list of custom colors.");
                return;
            }
            
            CustomColor col = Colors.ExtColors[code];
            char fallback;
            switch (args[2]) {
                case "name":
                    if (!CheckName(p, args[3])) return;
                    col.Name = args[3]; break;
                case "fallback":
                    if (!CheckFallback(p, args[3], out fallback)) return;
                    col.Fallback = fallback; break;
                case "hex":
                case "color":
                    CustomColor rgb = default(CustomColor);
                    if (!CommandParser.GetHex(p, args[3], ref rgb)) return;
                    col.R = rgb.R; col.G = rgb.G; col.B = rgb.B;
                    break;
                default:
                    Help(p); return;
            }
            
            Colors.AddExtColor(col);
            Player.Message(p, "Successfully edited a custom color.");
        }
        
        static bool CheckName(Player p, string arg) {
            if (Colors.Parse(arg) != "") {
                Player.Message(p, "There is already an existing standard or " +
                               "custom color with the name \"{0}\".", arg);
                return false;
            }
            return true;
        }
        
        static bool CheckFallback(Player p, string arg, out char fallback) {
            fallback = arg[0];
            if (!Colors.IsStandardColor(fallback)) {
                Player.Message(p, "{0} must be a standard color code.", fallback); return false;
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
            Player.Message(p, "%T/customcolors edit [code] [name/fallback/hex]");
        }        
    }
}
