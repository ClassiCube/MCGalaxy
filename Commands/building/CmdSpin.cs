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

			switch (message) {
				case "90":
				case "y":
					p.CopyBuffer = Flip.RotateY(p.CopyBuffer); break;
				case "180":
					Flip.MirrorX(p.CopyBuffer); Flip.MirrorZ(p.CopyBuffer); break;
				case "upsidedown":
				case "u":
				case "mirrory":
				case "mirror y":
					Flip.MirrorY(p.CopyBuffer); break;
				case "mirrorx":
				case "mirror x":
					Flip.MirrorX(p.CopyBuffer); break;
				case "mirror":
				case "m":					
				case "mirrorz":
				case "mirror z":
					Flip.MirrorZ(p.CopyBuffer); break;
				case "z":
					p.CopyBuffer = Flip.RotateZ(p.CopyBuffer); break;
				case "x":
					p.CopyBuffer = Flip.RotateX(p.CopyBuffer); break;

				default:
					Player.Message(p, "Incorrect syntax");
					Help(p); return;
			}
			Player.Message(p, "Spun: &b" + message);
		}
		
		public override void Help(Player p) {
			Player.Message(p, "/spin <x/y/z/180/mirrorx/mirrory/mirrorz> - Spins the copied object.");
			Player.Message(p, "Shortcuts: u for upside down (mirror on y), m for mirror on z");
			Player.Message(p, "x for spin 90 on x, y for spin 90 on y, z for spin 90 on z.");
		}
    }
}
