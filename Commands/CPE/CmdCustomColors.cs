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
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }

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
                    ListHandler(p, args); break;
                default:
                    Help(p); break;
            }
        }
        
        void AddHandler(Player p, string[] args) {
            if (args.Length < 4) { Help(p); return; }
            
            char code = args[1][0];
            if (Chat.IsStandardColor(code)) {
                Player.SendMessage(p, code + " is a standard color code, and thus cannot be removed."); return;
            }
            if (code <= ' ' || code > '~' || code == '%' || code == '&') {
                Player.SendMessage(p, code + " must be a standard ASCII character.");
                Player.SendMessage(p, "It also cannot be a space, percentage, or ampersand.");
                return;
            }
            
            char code2 = code;
            if (Chat.Map(ref code2)) {
                Player.SendMessage(p, "There is already a custom or server defined color with the code " + code +
                                   ", you must either use a different code or use \"/ccols remove " + code + "\"");
                return;
            }
            
            char fallback = args[2][0];
            if (!Chat.IsStandardColor(fallback)) {
                Player.SendMessage(p, fallback + " must be a standard color code."); return;
            }
            
            string hex = args[3];
            if (hex.Length > 0 && hex[0] == '#')
                hex = hex.Substring(1);
            if (hex.Length != 6 || !IsValidHex(hex)) {
                Player.SendMessage(p, "\"#" + hex + "\" is not a valid hex color."); return;
            }
            
            CustomColor col = default(CustomColor);
            col.Code = code; col.Fallback = fallback; col.A = 255;
            Color rgb = ColorTranslator.FromHtml("#" + hex);
            col.R = rgb.R; col.G = rgb.G; col.B = rgb.B;
            Chat.AddExtColor(col);
            Player.SendMessage(p, "Successfully added a custom color.");
        }
        
        void RemoveHandler(Player p, string[] args) {
            if (args.Length < 2) { Help(p); return; }
            
            char code = args[1][0];
            if (Chat.IsStandardColor(code)) {
                Player.SendMessage(p, code + " is a standard color, and thus cannot be removed."); return;
            }
            
            if ((int)code >= 256 || Chat.ExtColors[code].Undefined) {
                Player.SendMessage(p, "There is no custom color with the code " + code + ".");
                Player.SendMessage(p, "Use \"%T/ccols list\" %Sto see a list of custom colors.");
                return;
            }
            Chat.RemoveExtColor(code);
            Player.SendMessage(p, "Successfully removed a custom color.");
        }
        
        void ListHandler(Player p, string[] args) {
            int offset = 0, index = 0, count = 0;
            if (args.Length > 1) int.TryParse(args[1], out offset);
            CustomColor[] cols = Chat.ExtColors;
            
            for( int i = 0; i < cols.Length; i++ ) {
                CustomColor col = cols[i];
                if (col.Undefined) continue;
                
                if (index >= offset) {
                    count++;
                    const string format = "%{0} displays as &{0}{1}{3}, and falls back to {2}.";
                    Player.SendMessage(p, String.Format(format, col.Code, Hex(col), col.Fallback, Server.DefaultColor), false);
                    
                    if (count >= 8) {
                        const string helpFormat = "To see the next set of custom colors, type %T/ccols list {0}";
                        Player.SendMessage(p, String.Format(helpFormat, offset + 8));
                        return;
                    }
                }
                index++;
            }
        }
        
        public override void Help(Player p) {
            Player.SendMessage(p, "%T/ccols <add/remove/list>");
            Player.SendMessage(p, "%H/ccols add [code] [fallback] [hex]");
            Player.SendMessage(p, "%H   code is in ASCII. You cannot replace the standard color codes.");
            Player.SendMessage(p, "%H   fallback is the standard color code shown to non-supporting clients.");
            Player.SendMessage(p, "%H/ccols remove [code]");
            Player.SendMessage(p, "%H/ccols list [offset] - lists all custom color codes.");
        }
        
        static bool IsValidHex(string hex) {
            for (int i = 0; i < hex.Length; i++) {
                if (!Chat.IsStandardColor(hex[i])) return false;
            }
            return true;
        }
        
        static string Hex(CustomColor c) {
            return "#" + c.R.ToString("X2") + c.G.ToString("X2") + c.B.ToString("X2");
        }
    }
}
