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
using System;
using MCGalaxy.DB;

namespace MCGalaxy.Commands.Building {
    public sealed class CmdPlace : Command {        
        public override string name { get { return "place"; } }
        public override string shortcut { get { return "pl"; } }
        public override string type { get { return CommandTypes.Building; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Guest; } }
        public CmdPlace() { }

        public override void Use(Player p, string message) {
            byte block, ext = 0;
            block = p.GetActualHeldBlock(out ext);
            ushort x = p.pos[0], y = (ushort)(p.pos[1] - 32), z = p.pos[2];

            try {
                string[] parts = message.Split(' ');
                switch (parts.Length) {
                    case 1:
                        if (message == "") break;
                        
                        if (!CommandParser.GetBlock(p, parts[0], out block, out ext)) return;
                        break;
                    case 3:
                        x = (ushort)(ushort.Parse(parts[0]) * 32);
                        y = (ushort)(ushort.Parse(parts[1]) * 32);
                        z = (ushort)(ushort.Parse(parts[2]) * 32);
                        break;
                    case 4:
                        if (!CommandParser.GetBlock(p, parts[0], out block, out ext)) return;
                        
                        x = (ushort)(ushort.Parse(parts[1]) * 32);
                        y = (ushort)(ushort.Parse(parts[2]) * 32);
                        z = (ushort)(ushort.Parse(parts[3]) * 32);
                        break;
                    default: Player.Message(p, "Invalid number of parameters"); return;
                }
            } catch { 
                Player.Message(p, "Invalid parameters"); return; 
            }

            if (!CommandParser.IsBlockAllowed(p, "place ", block)) return;
            Vec3U16 P = Vec3U16.ClampPos(x, y, z, p.level);
            
            P.X /= 32; P.Y /= 32; P.Z /= 32;
            p.level.UpdateBlock(p, P.X, P.Y, P.Z, (byte)block, ext, BlockDBFlags.ManualPlace);
            string blockName = p.level.BlockName((byte)block, ext);
            Player.Message(p, "{3} block was placed at ({0}, {1}, {2}).", P.X, P.Y, P.Z, blockName);
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/place [block] <x y z>");
            Player.Message(p, "%HPlaces block at your feet or optionally at <x y z>");
        }
    }
}
