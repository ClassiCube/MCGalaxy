/*
    Copyright 2011 MCForge
    
    Dual-licensed under the    Educational Community License, Version 2.0 and
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

namespace MCGalaxy.Commands.Building {
    public sealed class CmdDrill : Command {        
        public override string name { get { return "drill"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.Building; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }

        public override void Use(Player p, string message) {
            int dist = 20;
            if (message != "" && !int.TryParse(message, out dist)) { Help(p); return; }
            Player.Message(p, "Destroy the block you wish to drill.");
            p.MakeSelection(1, dist, DoDrill);
        }
        
        bool DoDrill(Player p, Vec3S32[] marks, object state, byte type, byte extType) {
            ushort x = (ushort)marks[0].X, y = (ushort)marks[0].Y, z = (ushort)marks[0].Z;
            type = p.level.GetTile(x, y, z); extType = 0;
            if (type == Block.custom_block) 
                extType = p.level.GetExtTile(x, y, z);
            int dist = (int)state;

            int dx = 0, dz = 0;
            if (p.rot[0] <= 32 || p.rot[0] >= 224) { dz = -1; }
            else if (p.rot[0] <= 96) { dx = 1; }
            else if (p.rot[0] <= 160) { dz = 1; }
            else dx = -1;

            List<int> buffer = new List<int>();
            int depth = 0;
            Level lvl = p.level;

            if (dx != 0) {
                for (ushort xx = x; depth < dist; xx += (ushort)dx)
                {
                    for (ushort yy = (ushort)(y - 1); yy <= (ushort)(y + 1); yy++)
                        for (ushort zz = (ushort)(z - 1); zz <= (ushort)(z + 1); zz++)
                        {
                            buffer.Add(lvl.PosToInt(xx, yy, zz));
                        }
                    depth++;
                }
            } else {
                for (ushort zz = z; depth < dist; zz += (ushort)dz)
                {
                    for (ushort yy = (ushort)(y - 1); yy <= (ushort)(y + 1); yy++)
                        for (ushort xx = (ushort)(x - 1); xx <= (ushort)(x + 1); xx++)
                        {
                            buffer.Add(lvl.PosToInt(xx, yy, zz));
                        }
                    depth++;
                }
            }

            if (buffer.Count > p.group.maxBlocks) {
                Player.Message(p, "You tried to drill " + buffer.Count + " blocks.");
                Player.Message(p, "You cannot drill more than " + p.group.maxBlocks + ".");
                return false;
            }

            foreach (int index in buffer) {
                if (index < 0) continue;
                lvl.IntToPos(index, out x, out y, out z);
                byte tile = lvl.blocks[index], extTile = 0;
                if (tile == Block.custom_block) extTile = lvl.GetExtTile(x, y, z);
                
                bool sameBlock = type == Block.custom_block ? extType == extTile : type == tile;
                if (sameBlock) p.level.UpdateBlock(p, x, y, z, Block.air, 0);
            }
            Player.Message(p, "Drilled " + buffer.Count + " blocks.");
            return true;
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/drill [distance]");
            Player.Message(p, "%HDrills a hole, destroying all similar blocks in a 3x3 rectangle ahead of you.");
        }
    }
}
