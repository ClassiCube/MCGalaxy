/*
    Copyright 2015 MCGalaxy
        
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
using System;

namespace MCGalaxy.BlockPhysics {
    
    public static class ZombiePhysics {
        
        public static void Do(Level lvl, Check C, Random rand) {
            ushort x, y, z;
            lvl.IntToPos(C.b, out x, out y, out z);
            
            // Make zombie fall down
            if (lvl.GetTile(x, (ushort)(y - 1), z) == Block.air) {
                lvl.AddUpdate(C.b, Block.zombiehead);
                lvl.AddUpdate(lvl.IntOffset(C.b, 0, -1, 0), lvl.blocks[C.b]);
                lvl.AddUpdate(lvl.IntOffset(C.b, 0, 1, 0), Block.air);
                return;
            }
            bool skip = false;
            int index = 0;
            Player foundPlayer = AIPhysics.ClosestPlayer(lvl, C);

            if (foundPlayer != null && rand.Next(1, 20) < 18) {
                if (rand.Next(1, 7) <= 3) {
                    index = lvl.PosToInt((ushort)(x + Math.Sign((foundPlayer.pos[0] / 32) - x)), y, z);
                    if (index != C.b && MoveZombie(lvl, C, index, ref skip)) return;
                    
                    index = lvl.PosToInt(x, y, (ushort)(z + Math.Sign((foundPlayer.pos[2] / 32) - z)));
                    if (index != C.b && MoveZombie(lvl, C, index, ref skip)) return;
                } else {
                    index = lvl.PosToInt(x, y, (ushort)(z + Math.Sign((foundPlayer.pos[2] / 32) - z)));
                    if (index != C.b && MoveZombie(lvl, C, index, ref skip)) return;
                    
                    index = lvl.PosToInt((ushort)(x + Math.Sign((foundPlayer.pos[0] / 32) - x)), y, z);
                    if (index != C.b && MoveZombie(lvl, C, index, ref skip)) return;
                }
                skip = true;
            }
            
            if (!skip && C.time < 3) {
                C.time++;
                return;
            }

            int dirsVisited = 0;
            switch (rand.Next(1, 13))
            {
                case 1:
                case 2:
                case 3:
                    skip = false;
                    index = lvl.IntOffset(C.b, -1, 0, 0);
                    if (MoveZombie(lvl, C, index, ref skip)) return;

                    dirsVisited++;
                    if (dirsVisited >= 4) return;
                    else goto case 4;
                    break;

                case 4:
                case 5:
                case 6:
                    skip = false;
                    index = lvl.IntOffset(C.b, 1, 0, 0);
                    if (MoveZombie(lvl, C, index, ref skip)) return;

                    dirsVisited++;
                    if (dirsVisited >= 4) return;
                    else goto case 7;
                    break;

                case 7:
                case 8:
                case 9:
                    skip = false;
                    index = lvl.IntOffset(C.b, 0, 0, 1);
                    if (MoveZombie(lvl, C, index, ref skip)) return;

                    dirsVisited++;
                    if (dirsVisited >= 4) return;
                    else goto case 10;
                    break;
                case 10:
                case 11:
                case 12:
                    skip = false;
                    index = lvl.IntOffset(C.b, 0, 0, -1);
                    if (MoveZombie(lvl, C, index, ref skip)) return;

                    dirsVisited++;
                    if (dirsVisited >= 4) return;
                    else goto case 1;
                    break;
            }
            lvl.AddUpdate(C.b, Block.air);
            lvl.AddUpdate(lvl.IntOffset(C.b, 0, 1, 0), Block.air);
        }
        
        static bool MoveZombie(Level lvl, Check C, int index, ref bool skip) {
            skip = false;
            
            if(
                lvl.GetTile(lvl.IntOffset(index, 0, -1, 0)) == Block.air &&
                lvl.GetTile(index) == Block.air) {
                index = lvl.IntOffset(index, 0, -1, 0);
            } else if (
                lvl.GetTile(index) == Block.air &&
                lvl.GetTile(lvl.IntOffset(index, 0, 1, 0)) == Block.air) {
            } else if (
                lvl.GetTile(lvl.IntOffset(index, 0, 2, 0)) == Block.air &&
                lvl.GetTile(lvl.IntOffset(index, 0, 1, 0)) == Block.air) {
                index = lvl.IntOffset(index, 0, 1, 0);
            } else {
                skip = true;
            }

            if (!skip && lvl.AddUpdate(index, lvl.blocks[C.b])) {
                lvl.AddUpdate(lvl.IntOffset(index, 0, 1, 0), Block.zombiehead);
                lvl.AddUpdate(C.b, Block.air);
                lvl.AddUpdate(lvl.IntOffset(C.b, 0, 1, 0), Block.air);
                return true;
            }
            return false;
        }
    }
}