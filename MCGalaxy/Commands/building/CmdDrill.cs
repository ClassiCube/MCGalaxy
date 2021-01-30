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
using MCGalaxy.Maths;
using BlockID = System.UInt16;

namespace MCGalaxy.Commands.Building {
    public sealed class CmdDrill : Command2 {
        public override string name { get { return "Drill"; } }
        public override string type { get { return CommandTypes.Building; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        public override bool SuperUseable { get { return false; } }

        public override void Use(Player p, string message, CommandData data) {
            ushort dist = 20;
            if (message.Length > 0 && !CommandParser.GetUShort(p, message, "Distance", ref dist)) return;
            
            p.Message("Destroy the block you wish to drill.");
            p.MakeSelection(1, "Selecting location for &SDrill", dist, DoDrill);
        }
        
        bool DoDrill(Player p, Vec3S32[] marks, object state, BlockID block) {
            ushort x = (ushort)marks[0].X, y = (ushort)marks[0].Y, z = (ushort)marks[0].Z;
            block = p.level.GetBlock(x, y, z);
            int dist = (ushort)state, numBlocks = (3 * 3) * dist;
            
            if (numBlocks > p.group.DrawLimit) {
                p.Message("You tried to drill " + numBlocks + " blocks.");
                p.Message("You cannot drill more than " + p.group.DrawLimit + ".");
                return false;
            }

            int dx = 0, dz = 0;
            DirUtils.FourYaw(p.Rot.RotY, out dx, out dz);
            Level lvl = p.level;

            if (dx != 0) {
                for (int depth = 0; depth < dist; x += (ushort)dx, depth++) {
                    if (x >= lvl.Width) continue;
                    
                    for (ushort yy = (ushort)(y - 1); yy <= (ushort)(y + 1); yy++)
                        for (ushort zz = (ushort)(z - 1); zz <= (ushort)(z + 1); zz++)
                    {
                        DoBlock(p, lvl, block, x, yy, zz);
                    }
                }
            } else {
                for (int depth = 0; depth < dist; z += (ushort)dz, depth++) {
                    if (z >= lvl.Length) break;
                    
                    for (ushort yy = (ushort)(y - 1); yy <= (ushort)(y + 1); yy++)
                        for (ushort xx = (ushort)(x - 1); xx <= (ushort)(x + 1); xx++)
                    {
                        DoBlock(p, lvl, block, xx, yy, z);
                    }
                }
            }

            p.Message("Drilled " + numBlocks + " blocks.");
            return true;
        }
        
        void DoBlock(Player p, Level lvl, BlockID block, ushort x, ushort y, ushort z) {
            BlockID cur = lvl.GetBlock(x, y, z);
            if (cur == block) {
                p.level.UpdateBlock(p, x, y, z, Block.Air, BlockDBFlags.Drawn, true);
            }
        }
        
        public override void Help(Player p) {
            p.Message("&T/Drill [distance]");
            p.Message("&HDrills a hole, destroying all similar blocks in a 3x3 rectangle ahead of you.");
        }
    }
}
