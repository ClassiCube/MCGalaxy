﻿/*
    Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCGalaxy)
        
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

namespace MCGalaxy.Blocks.Physics {
    
    public static class ZombiePhysics {
        
        public static void Do(Level lvl, ref Check C) {
            Random rand = lvl.physRandom;
            ushort x, y, z;
            lvl.IntToPos(C.b, out x, out y, out z);
            
            // Make zombie fall down
            if (lvl.IsAirAt(x, (ushort)(y - 1), z)) {
                lvl.AddUpdate(C.b, Block.zombiehead);
                lvl.AddUpdate(lvl.IntOffset(C.b, 0, -1, 0), lvl.blocks[C.b]);
                lvl.AddUpdate(lvl.IntOffset(C.b, 0, 1, 0), Block.air);
                return;
            }
            bool checkTime = true;
            int index = 0;
            Player closest = AIPhysics.ClosestPlayer(lvl, x, y, z);

            if (closest != null && rand.Next(1, 20) < 18) {
                if (rand.Next(1, 7) <= 3) {
                    index = lvl.PosToInt((ushort)(x + Math.Sign(closest.Pos.BlockX - x)), y, z);
                    if (index != C.b && MoveZombie(lvl, ref C, index)) return;
                    
                    index = lvl.PosToInt(x, y, (ushort)(z + Math.Sign((closest.Pos.BlockZ - z))));
                    if (index != C.b && MoveZombie(lvl, ref C, index)) return;
                } else {
                    index = lvl.PosToInt(x, y, (ushort)(z + Math.Sign(closest.Pos.BlockZ - z)));
                    if (index != C.b && MoveZombie(lvl, ref C, index)) return;
                    
                    index = lvl.PosToInt((ushort)(x + Math.Sign(closest.Pos.BlockX - x)), y, z);
                    if (index != C.b && MoveZombie(lvl, ref C, index)) return;
                }
                checkTime = false;
            }
            
            if (checkTime && C.data.Data < 3) {
                C.data.Data++;
                return;
            }

            int dirsVisited = 0;
            switch (rand.Next(1, 13))
            {
                case 1:
                case 2:
                case 3:
                    index = lvl.IntOffset(C.b, -1, 0, 0);
                    if (MoveZombie(lvl, ref C, index)) return;

                    dirsVisited++;
                    if (dirsVisited >= 4) return;
                    goto case 4;

                case 4:
                case 5:
                case 6:
                    index = lvl.IntOffset(C.b, 1, 0, 0);
                    if (MoveZombie(lvl, ref C, index)) return;

                    dirsVisited++;
                    if (dirsVisited >= 4) return;
                    goto case 7;

                case 7:
                case 8:
                case 9:
                    index = lvl.IntOffset(C.b, 0, 0, 1);
                    if (MoveZombie(lvl, ref C, index)) return;

                    dirsVisited++;
                    if (dirsVisited >= 4) return;
                    goto case 10;
                case 10:
                case 11:
                case 12:
                    index = lvl.IntOffset(C.b, 0, 0, -1);
                    if (MoveZombie(lvl, ref C, index)) return;

                    dirsVisited++;
                    if (dirsVisited >= 4) return;
                    goto case 1;
            }
            lvl.AddUpdate(C.b, Block.air);
            lvl.AddUpdate(lvl.IntOffset(C.b, 0, 1, 0), Block.air);
        }
        
        public static void DoHead(Level lvl, ref Check C) {
            if (lvl.GetTile(lvl.IntOffset(C.b, 0, -1, 0)) != Block.zombiebody
                && lvl.GetTile(lvl.IntOffset(C.b, 0, -1, 0)) != Block.creeper) {
                C.data.Type1 = PhysicsArgs.Revert; C.data.Value1 = Block.air;
            }
        }
        
        static bool MoveZombie(Level lvl, ref Check C, int index) {
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
                return false;
            }

            if (lvl.AddUpdate(index, lvl.blocks[C.b])) {
                lvl.AddUpdate(lvl.IntOffset(index, 0, 1, 0), Block.zombiehead);
                lvl.AddUpdate(C.b, Block.air);
                lvl.AddUpdate(lvl.IntOffset(C.b, 0, 1, 0), Block.air);
                return true;
            }
            return false;
        }
    }
}