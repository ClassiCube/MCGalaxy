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
using MCGalaxy;
using MCGalaxy.Drawing.Brushes;
 
namespace MCGalaxy.Commands {
    
    public sealed class CmdReplaceBrush : Command {
        public override string name { get { return "replacebrush"; } }
        public override string shortcut { get { return "rb"; } }
        public override string type { get { return CommandTypes.Building; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.AdvBuilder; } }
        static char[] trimChars = {' '};

        public override void Use(Player p, string message) {
        	// TODO: make sure can use or brush first.
            if (message == "") { Help(p); return; }
            if (p == null) { MessageInGameOnly(p); return; }
            string[] args = message.Split(trimChars, 3);
            if (args.Length < 2) { Help(p); return }
            
            byte extType = 0;
            byte type = DrawCmd.GetBlock(p, args[0], out extType);
            string brush = CmdBrush.FindBrush(args[1]);
            if (brush == null) {
                Player.SendMessage(p, "No brush found with name \"" + message + "\".");
                Player.SendMessage(p, "Available brushes: " + CmdBrush.AvailableBrushes);
                return;
            }
        }
        
        public override void Help(Player p) {
            Player.SendMessage(p, "/replace [block] [block2].. [new] - replace block with new inside a selected cuboid");
            Player.SendMessage(p, "If more than one [block] is specified, they will all be replaced.");
        }
    }
}
