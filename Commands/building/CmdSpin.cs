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
    public sealed class CmdSpin : Command {
        public override string name { get { return "spin"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.Building; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.AdvBuilder; } }
        public CmdSpin() { }
        public override CommandAlias[] Aliases {
            get { return new [] { new CommandAlias("mirror", "mirror") }; }
        }

        public override void Use(Player p, string message) {
            if (message == "") message = "y";
            if (p.CopyBuffer == null) {
                Player.Message(p, "You haven't copied anything yet"); return;
            }
            string opt = message.ToLower();
            
            // Mirroring
            if (opt == "mirrorx" || opt == "mirror x") {
                Flip.MirrorX(p.CopyBuffer);
                Player.Message(p, "Flipped copy across the X (east/west) axis.");
            } else if (opt == "mirrory" || opt == "mirror y" || opt == "u") {
                Flip.MirrorY(p.CopyBuffer);
                Player.Message(p, "Flipped copy across the Y (vertical) axis.");
            } else if (opt == "mirrorz" || opt == "mirror z" || opt == "m") {
                Flip.MirrorZ(p.CopyBuffer);
                Player.Message(p, "Flipped copy across the Z (north/south) axis.");
            } else {
                string[] args = opt.Split(' ');
                char axis = 'Y';
                int angle = 90;
                if (!Handle(ref axis, ref angle, args[0])) { Help(p); return; }
                if (args.Length > 1 && !Handle(ref axis, ref angle, args[1])) { Help(p); return; }
                
                if (angle == 0) {
                } else if (axis == 'X') {
                    p.CopyBuffer = Flip.RotateX(p.CopyBuffer, angle);
                } else if (axis == 'Y') {
                    p.CopyBuffer = Flip.RotateY(p.CopyBuffer, angle);
                } else if (axis == 'Z') {
                    p.CopyBuffer = Flip.RotateZ(p.CopyBuffer, angle);
                }
                Player.Message(p, "Rotated copy {0} degrees around the {1} axis", angle, axis);
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
            Player.Message(p, "/spin <x/y/z/180/mirrorx/mirrory/mirrorz> - Spins the copied object.");
            Player.Message(p, "Shortcuts: u for mirror on y, m for mirror on z");
            Player.Message(p, "x for spin 90 on x, y for spin 90 on y, z for spin 90 on z.");
        }
    }
}
