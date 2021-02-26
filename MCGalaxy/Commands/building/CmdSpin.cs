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
    public sealed class CmdSpin : Command2 {
        public override string name { get { return "Spin"; } }
        public override string type { get { return CommandTypes.Building; } }
        public override LevelPermission defaultRank { get { return LevelPermission.AdvBuilder; } }
        public override bool SuperUseable { get { return false; } }
        public override CommandAlias[] Aliases {
            get { return new [] { new CommandAlias("Mirror", "mirror"), new CommandAlias("Rotate") }; }
        }

        public override void Use(Player p, string message, CommandData data) {
            if (message.Length == 0) message = "y";
            if (p.CurrentCopy == null) { 
                p.Message("You haven't copied anything yet"); return; 
            }
            
            CopyState cState = p.CurrentCopy;
            string opt = message.ToLower();
            BlockDefinition[] defs = p.level.CustomBlockDefs;
            
            // Mirroring
            if (opt == "mirrorx" || opt == "mirror x") {
                Flip.MirrorX(cState, defs);
                p.Message("Flipped copy across the X (east/west) axis.");
            } else if (opt == "mirrory" || opt == "mirror y" || opt == "u") {
                Flip.MirrorY(cState, defs);
                p.Message("Flipped copy across the Y (vertical) axis.");
            } else if (opt == "mirrorz" || opt == "mirror z" || opt == "m") {
                Flip.MirrorZ(cState, defs);
                p.Message("Flipped copy across the Z (north/south) axis.");
            } else {
                string[] args = opt.SplitSpaces();
                char axis = 'Y';
                int angle = 90;
                if (!Handle(ref axis, ref angle, args[0])) { Help(p); return; }
                if (args.Length > 1 && !Handle(ref axis, ref angle, args[1])) { Help(p); return; }
                
                CopyState newState = cState;
                if (angle == 0) {
                } else if (axis == 'X') {
                    newState = Flip.RotateX(cState, angle, defs);
                } else if (axis == 'Y') {
                    newState = Flip.RotateY(cState, angle, defs);
                } else if (axis == 'Z') {
                    newState = Flip.RotateZ(cState, angle, defs);
                }

                newState.CopySource = cState.CopySource;
                newState.CopyTime   = cState.CopyTime;
                p.CurrentCopy = newState;
                p.Message("Rotated copy {0} degrees around the {1} axis", angle, axis);
            }            
        }
        
        bool Handle(ref char axis, ref int angle, string arg) {
            int value;
            if (arg == "x" || arg == "y" || arg == "z") {
                axis = char.ToUpper(arg[0]); return true;
            } else if (int.TryParse(arg, out value)) {
                // Clamp to [0, 360)
                value %= 360;
                if (value < 0) value += 360;
                angle = value;
                return angle == 0 || angle == 90 || angle == 180 || angle == 270;
            }
            return false;
        }
        
        public override void Help(Player p) {
            p.Message("&T/Spin mirrorx/mirrory/mirrorz");
            p.Message("&HFlips/Mirrors the copied object around that axis.");
            p.Message("&T/Spin X/Y/Z 90/180/270");
            p.Message("&HRotates the copied object around that axis by the given angle. " +
                           "If no angle is given, 90 degrees is used.");
        }
    }
}
