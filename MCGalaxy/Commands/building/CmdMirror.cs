/*
    Copyright 2011 MCForge
        
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
using System.Collections.Generic;
using MCGalaxy.Drawing;

namespace MCGalaxy.Commands.Building {
    public sealed class CmdMirror : Command2 {
        public override string name { get { return "Mirror"; } }
        public override string type { get { return CommandTypes.Building; } }
        public override LevelPermission defaultRank { get { return LevelPermission.AdvBuilder; } }
        public override bool SuperUseable { get { return false; } }
        public override CommandAlias[] Aliases {
            get { return new [] { new CommandAlias("Flip") }; }
        }

        public override void Use(Player p, string message, CommandData data) {
            if (message.Length == 0) { Help(p); return; }
            if (p.CurrentCopy == null) {
                p.Message("You haven't copied anything yet"); return;
            }
            
            CopyState cState = p.CurrentCopy;
            BlockDefinition[] defs = p.level.CustomBlockDefs;
            
            foreach (string arg in message.SplitSpaces())
            {
                if (arg.CaselessEq("x")) {
                    Flip.MirrorX(cState, defs);
                    p.Message("Flipped copy across the X (east/west) axis.");
                } else if (arg.CaselessEq("y") || arg.CaselessEq("u")) {
                    Flip.MirrorY(cState, defs);
                    p.Message("Flipped copy across the Y (vertical) axis.");
                } else if (arg.CaselessEq("z")) {
                    Flip.MirrorZ(cState, defs);
                    p.Message("Flipped copy across the Z (north/south) axis.");
                }
            }
        }
        
        public override void Help(Player p) {
            p.Message("&T/Mirror X/Y/Z");
            p.Message("&HFlips/Mirrors the copied object around that axis.");
            p.Message("  &HX = horizontal axis (east-west)");           
            p.Message("  &HY = vertical axis");
            p.Message("  &HZ = horizontal axis (north-south)");
        }
    }
}
