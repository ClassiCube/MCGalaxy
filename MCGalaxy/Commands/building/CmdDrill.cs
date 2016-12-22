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
using MCGalaxy.DB;

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
            int dist = (int)state, numBlocks = (3 * 3) * dist;
            
            if (numBlocks > p.group.maxBlocks) {
                Player.Message(p, "You tried to drill " + numBlocks + " blocks.");
                Player.Message(p, "You cannot drill more than " + p.group.maxBlocks + ".");
                return false;
            }

            int dx = 0, dz = 0;
            DirUtils.FourYaw(p.rot[0], out dx, out dz);
            Level lvl = p.level;

            if (dx != 0) {
                for (int depth = 0; depth < dist; x += (ushort)dx, depth++) {
                    if (x >= lvl.Width) continue;
                    
                    for (ushort yy = (ushort)(y - 1); yy <= (ushort)(y + 1); yy++)
                        for (ushort zz = (ushort)(z - 1); zz <= (ushort)(z + 1); zz++)
                    {
                        DoBlock(p, lvl, type, extType, x, yy, zz);
                    }
                }
            } else {
                for (int depth = 0; depth < dist; z += (ushort)dz, depth++) {
                    if (z >= lvl.Length) break;
                    
                    for (ushort yy = (ushort)(y - 1); yy <= (ushort)(y + 1); yy++)
                        for (ushort xx = (ushort)(x - 1); xx <= (ushort)(x + 1); xx++)
                    {
                        DoBlock(p, lvl, type, extType, xx, yy, z);
                    }
                }
            }

            Player.Message(p, "Drilled " + numBlocks + " blocks.");
            return true;
        }
        
        void DoBlock(Player p, Level lvl, byte block, byte ext,
                     ushort x, ushort y, ushort z) {
            int index = lvl.PosToInt(x, y, z);
            if (index == -1) return;
            byte cur = lvl.blocks[index], extCur = 0;
            if (cur == Block.custom_block) extCur = lvl.GetExtTile(x, y, z);
            
            bool same = block == Block.custom_block ? ext == extCur : block == cur;
            if (same) p.level.UpdateBlock(p, x, y, z, Block.air, 0, BlockDBFlags.Drawn);
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/drill [distance]");
            Player.Message(p, "%HDrills a hole, destroying all similar blocks in a 3x3 rectangle ahead of you.");
        }
    }
}
