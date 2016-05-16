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

namespace MCGalaxy.Commands {
    
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
                case "new":
                    AddHandler(p, args); break;
                case "remove":
                case "delete":
                    RemoveHandler(p, args); break;
                case "list":
                    ListHandler(p, args, false); break;
                default:
                    Help(p); break;
            }
        }
        
        void AddHandler(Player p, string[] args) {
            if (args.Length < 4) { Help(p); return; }
            
            char code = args[1][0];
            if (Colors.IsStandardColor(code)) {
                Player.Message(p, code + " is a standard color code, and thus cannot be removed."); return;
            }
            if (code <= ' ' || code > '~' || code == '%' || code == '&') {
                Player.Message(p, code + " must be a standard ASCII character.");
                Player.Message(p, "It also cannot be a space, percentage, or ampersand.");
                return;
            }
            
            char code2 = code;
            if (Colors.MapColor(ref code2)) {
                Player.Message(p, "There is already a custom or server defined color with the code " + code +
                                   ", you must either use a different code or use \"%T/ccols remove " + code + "%S\"");
                return;
            }
            
            string name = args[2];
            if (Colors.Parse(name) != "") {
            	Player.Message(p, "There is already an existing standard or " +
            	                   "custom color with the name \"" + name + "\"."); return;
            }
            
            char fallback = args[3][0];           
            if (!Colors.IsStandardColor(fallback)) {
                Player.Message(p, fallback + " must be a standard color code."); return;
            }
            if (fallback >= 'A' && fallback <= 'F') fallback += ' ';
            
            string hex = args[4];
            if (hex.Length > 0 && hex[0] == '#')
                hex = hex.Substring(1);
            if (hex.Length != 6 || !IsValidHex(hex)) {
                Player.Message(p, "\"#" + hex + "\" is not a valid hex color."); return;
            }
            
            CustomColor col = default(CustomColor);
            col.Code = code; col.Fallback = fallback; col.A = 255;
            col.Name = name;
            Color rgb = ColorTranslator.FromHtml("#" + hex);
            col.R = rgb.R; col.G = rgb.G; col.B = rgb.B;
            Colors.AddExtColor(col);
            Player.Message(p, "Successfully added a custom color.");
        }
        
        void RemoveHandler(Player p, string[] args) {
            if (args.Length < 2) { Help(p); return; }
            
            char code = args[1][0];
            if (Colors.IsStandardColor(code)) {
                Player.Message(p, code + " is a standard color, and thus cannot be removed."); return;
            }
            
            if ((int)code >= 256 || Colors.ExtColors[code].Undefined) {
                Player.Message(p, "There is no custom color with the code " + code + ".");
                Player.Message(p, "Use \"%T/ccols list\" %Sto see a list of custom colors.");
                return;
            }
            Colors.RemoveExtColor(code);
            Player.Message(p, "Successfully removed a custom color.");
        }
        
        internal static void ListHandler(Player p, string[] args, bool all) {
            int offset = 0, index = 0, count = 0;
            if (args != null && args.Length > 1) int.TryParse(args[1], out offset);
            CustomColor[] cols = Colors.ExtColors;
            
            for( int i = 0; i < cols.Length; i++ ) {
                CustomColor col = cols[i];
                if (col.Undefined) continue;
                
                if (index >= offset) {
                    count++;
                    const string format = "{4}{0} &{1}({2}){4} - %{1}, falls back to %{3}.";
                    Player.SendMessage(p, String.Format(format, col.Name, col.Code, Hex(col), col.Fallback, Server.DefaultColor), false);
                    
                    if (count >= 8 && !all) {
                        const string helpFormat = "To see the next set of custom colors, type %T/ccols list {0}";
                        Player.Message(p, helpFormat, offset + 8);
                        return;
                    }
                }
                index++;
            }
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/ccols <add/remove/list>");
            Player.Message(p, "%H/ccols add [code] [name] [fallback] [hex]");
            Player.Message(p, "%H   code is in ASCII. You cannot replace the standard color codes.");
            Player.Message(p, "%H   fallback is the standard color code shown to non-supporting clients.");
            Player.Message(p, "%H/ccols remove [code]");
            Player.Message(p, "%H/ccols list [offset] - lists all custom color codes.");
        }
        
        static bool IsValidHex(string hex) {
            for (int i = 0; i < hex.Length; i++) {
                if (!Colors.IsStandardColor(hex[i])) return false;
            }
            return true;
        }
        
        static string Hex(CustomColor c) {
            return "#" + c.R.ToString("X2") + c.G.ToString("X2") + c.B.ToString("X2");
        }
    }
}
