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

namespace MCGalaxy.Commands.Building {
    public sealed class CmdPlace : Command {        
        public override string name { get { return "place"; } }
        public override string shortcut { get { return "pl"; } }
        public override string type { get { return CommandTypes.Building; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Guest; } }
        public CmdPlace() { }

        public override void Use(Player p, string message)
        {
            byte block, ext = 0;
            block = p.GetActualHeldBlock(out ext);
            int x = p.Pos.BlockX, y = (p.Pos.Y - 32) / 32, z = p.Pos.BlockZ;

            try {
                string[] parts = message.SplitSpaces();
                switch (parts.Length) {
                    case 1:
                        if (message == "") break;
                        
                        if (!CommandParser.GetBlock(p, parts[0], out block, out ext)) return;
                        break;
                    case 3:
                        x = int.Parse(parts[0]);
                        y = int.Parse(parts[1]);
                        z = int.Parse(parts[2]);
                        break;
                    case 4:
                        if (!CommandParser.GetBlock(p, parts[0], out block, out ext)) return;
                        
                        x = int.Parse(parts[1]);
                        y = int.Parse(parts[2]);
                        z = int.Parse(parts[3]);
                        break;
                    default: Player.Message(p, "Invalid number of parameters"); return;
                }
            } catch { 
                Player.Message(p, "Invalid parameters"); return; 
            }

            if (!CommandParser.IsBlockAllowed(p, "place ", block)) return;
            
            x = Clamp(x, p.level.Width);
            y = Clamp(y, p.level.Height);
            z = Clamp(z, p.level.Length);
            
            p.level.UpdateBlock(p, (ushort)x, (ushort)y, (ushort)z, block, ext);
            string blockName = p.level.BlockName(block, ext);
            Player.Message(p, "{3} block was placed at ({0}, {1}, {2}).", x, y, z, blockName);
        }
        
        static int Clamp(int value, int axisLen) {
            if (value < 0) return 0;
            if (value >= axisLen) return axisLen - 1;
            return value;
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/place [block] <x y z>");
            Player.Message(p, "%HPlaces block at your feet or optionally at <x y z>");
        }
    }
}
