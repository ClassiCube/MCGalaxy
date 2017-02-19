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
using MCGalaxy.Drawing;

namespace MCGalaxy.Commands.Building {
    public sealed class CmdPalette : Command {
        public override string name { get { return "palette"; } }
        public override string shortcut { get { return "imgpalette"; } }
        public override string type { get { return CommandTypes.Building; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Admin; } }

        public override void Use(Player p, string message) {
            string[] args = message.SplitSpaces(4);
            if (message == "") { Help(p); return; }
            
            if (args[0].CaselessEq("create")) {
                HandleCreate(p, args);
            } else if (args[0].CaselessEq("delete")) {
                HandleDelete(p, args);
            } else if (args[0].CaselessEq("add")) {
                Player.Message(p, "?????");
            } else if (args[0].CaselessEq("remove")) {
                Player.Message(p, "?????");
            }
        }
        
        void HandleCreate(Player p, string[] args) {
            if (args.Length != 2) { Help(p); return; }
            if (!Formatter.ValidName(p, args[1], "palette")) return;
            
            ImagePalette palette = ImagePalette.Find(args[1]);
            if (palette != null) {
                Player.Message(p, "Palette {0} already exists.", args[1]);
            } else {
                ImagePalette.Add(args[1]);
                Player.Message(p, "Created palette {0}", args[1]);
            }
        }
        
        void HandleDelete(Player p, string[] args) {
            if (args.Length != 2) { Help(p); return; }
            
            ImagePalette palette = ImagePalette.Find(args[1]);
            if (palette == null) {
                Player.Message(p, "Palette {0} does not exist.", args[1]);
            } else {
                ImagePalette.Remove(palette);
                Player.Message(p, "Removed palette {0}", args[1]);
            }
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/palette create/delete [name]");
            Player.Message(p, "%HCreates or deletes a palette for %T/imageprint");
            Player.Message(p, "%T/palette add [name] [block] [hex color] <back>");
            Player.Message(p, "???");
            Player.Message(p, "%T/palette remove [name] [block]");
            Player.Message(p, "???");
            Player.Message(p, "%HPalettes: &f{0}", ImagePalette.Palettes.Join(pal => pal.Name));
        }
    }
}

