/*
	Copyright 2011 MCForge
		
	Dual-licensed under the	Educational Community License, Version 2.0 and
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
namespace MCGalaxy.Commands {
	
    public sealed class CmdPlace : Command {
		
        public override string name { get { return "place"; } }
        public override string shortcut { get { return "pl"; } }
        public override string type { get { return CommandTypes.Building; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Guest; } }
        public CmdPlace() { }

        public override void Use(Player p, string message) {
            byte type = Block.Zero, extType = 0;
            ushort x = (ushort)(p.pos[0] / 32);
            ushort y = (ushort)((p.pos[1] / 32) - 1);
            ushort z = (ushort)(p.pos[2] / 32);

            try {
            	string[] parts = message.Split(' ');
                switch (parts.Length) {
                    case 1: type = message == "" ? Block.rock :
                        DrawCmd.GetBlock(p, parts[0], out extType); break;
                    case 3:
                        type = Block.rock;
                        x = Convert.ToUInt16(parts[0]);
                        y = Convert.ToUInt16(parts[1]);
                        z = Convert.ToUInt16(parts[2]);
                        break;
                    case 4:
                        type = DrawCmd.GetBlock(p, parts[0], out extType);
                        x = Convert.ToUInt16(parts[1]);
                        y = Convert.ToUInt16(parts[2]);
                        z = Convert.ToUInt16(parts[3]);
                        break;
                    default: Player.SendMessage(p, "Invalid number of parameters"); return;
                }
            } catch { 
            	Player.SendMessage(p, "Invalid parameters"); return; 
            }

            if (type == Block.Zero) return;
            if (!Block.canPlace(p, type)) { Player.SendMessage(p, "Cannot place that block type."); return; }
            if (y >= p.level.Height) y = (ushort)(p.level.Height - 1);

            p.level.UpdateBlock(p, x, y, z, type, extType);
            Player.SendMessage(p, "A block was placed at (" + x + ", " + y + ", " + z + ").");
        }
        
        public override void Help(Player p) {
            Player.SendMessage(p, "/place [block] <x y z> - Places block at your feet or <x y z>");
        }
    }
}
