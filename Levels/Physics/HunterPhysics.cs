/*
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

namespace MCGalaxy.BlockPhysics {
    
    public static class HunterPhysics {
        
        public static void DoKiller(Level lvl, ref Check C, byte target) {
            Random rand = lvl.physRandom;			
            Player closest = AIPhysics.ClosestPlayer(lvl, ref C);
            ushort x, y, z;
            lvl.IntToPos(C.b, out x, out y, out z);
            
            if (closest != null && rand.Next(1, 20) < 19) {
                int index = 0, dirsVisited = 0;

                switch (rand.Next(1, 10)) {
                    case 1:
                    case 2:
                    case 3:
                        if ((closest.pos[0] / 32) - x != 0) {
                            index = lvl.PosToInt((ushort)(x + Math.Sign((closest.pos[0] / 32) - x)), y, z);
                            if (MoveFish(lvl, ref C, index, target)) return;
                        }                    
                        dirsVisited++;
                        if (dirsVisited >= 3) break;
                        goto case 4;
                    case 4:
                    case 5:
                    case 6:
                        if ((closest.pos[1] / 32) - y != 0) {
                            index = lvl.PosToInt(x, (ushort)(y + Math.Sign((closest.pos[1] / 32) - y)), z);
                            if (MoveFish(lvl, ref C, index, target)) return;
                        }
                        dirsVisited++;
                        if (dirsVisited >= 3) break;
                        goto case 7;
                    case 7:
                    case 8:
                    case 9:
                        if ((closest.pos[2] / 32) - z != 0) {
                            index = lvl.PosToInt(x, y, (ushort)(z + Math.Sign((closest.pos[2] / 32) - z)));
                            if (MoveFish(lvl, ref C, index, target)) return;
                        }
                        dirsVisited++;
                        if (dirsVisited >= 3) break;
                        goto case 1;
                }
            }
            RandomlyMove(lvl, ref C, rand, x, y, z, target);
        }
        
        public static void DoFlee(Level lvl, ref Check C, byte target) {
            Random rand = lvl.physRandom;			
            Player closest = AIPhysics.ClosestPlayer(lvl, ref C);
            ushort x, y, z;
            lvl.IntToPos(C.b, out x, out y, out z);
            
            if (closest != null && rand.Next(1, 20) < 19) {
                int index = 0, dirsVisited = 0;

                switch (rand.Next(1, 10)) {
                    case 1:
                    case 2:
                    case 3:
                        if ((closest.pos[0] / 32) - x != 0) {
                            index = lvl.PosToInt((ushort)(x - Math.Sign((closest.pos[0] / 32) - x)), y, z);
                            if (MoveFish(lvl, ref C, index, target)) return;
                        }                        
                        dirsVisited++;
                        if (dirsVisited >= 3) break;
                        goto case 4;
                    case 4:
                    case 5:
                    case 6:
                        if ((closest.pos[1] / 32) - y != 0) {
                            index = lvl.PosToInt(x, (ushort)(y - Math.Sign((closest.pos[1] / 32) - y)), z);
                            if (MoveFish(lvl, ref C, index, target)) return;
                        }
                        dirsVisited++;
                        if (dirsVisited >= 3) break;
                        goto case 7;
                    case 7:
                    case 8:
                    case 9:
                        if ((closest.pos[2] / 32) - z != 0) {
                            index = lvl.PosToInt(x, y, (ushort)(z - Math.Sign((closest.pos[2] / 32) - z)));
                            if (MoveFish(lvl, ref C, index, target)) return;
                        }
                        dirsVisited++;
                        if (dirsVisited >= 3) break;
                        goto case 1;
                }
            }
            RandomlyMove(lvl, ref C, rand, x, y, z, target);
        }
        
        static bool MoveFish(Level lvl, ref Check C, int index, byte target) {
            if (index >= 0 && lvl.blocks[index] == target && lvl.AddUpdate(index, lvl.blocks[C.b])) {
                lvl.AddUpdate(C.b, target);
                return true;
            }
            return false;
        }
        
        static void RandomlyMove(Level lvl, ref Check C, Random rand, ushort x, ushort y, ushort z, byte target ) {
            switch (rand.Next(1, 15)) {
                case 1:
                    if (MoveFish(lvl, ref C, lvl.PosToInt(x, (ushort)(y - 1), z), target)) return;
                    goto case 3;
                case 2:
                    if (MoveFish(lvl, ref C, lvl.PosToInt(x, (ushort)(y + 1), z), target)) return;
                    goto case 6;
                case 3:
                case 4:
                case 5:
                    if (MoveFish(lvl, ref C, lvl.PosToInt((ushort)(x - 1), y, z), target)) return;
                    goto case 9;
                case 6:
                case 7:
                case 8:
                    if (MoveFish(lvl, ref C, lvl.PosToInt((ushort)(x + 1), y, z), target)) return;
                    goto case 12;
                case 9:
                case 10:
                case 11:
                    MoveFish(lvl, ref C, lvl.PosToInt(x, y, (ushort)(z - 1)), target);
                    break;
                case 12:
                case 13:
                case 14:
                    MoveFish(lvl, ref C, lvl.PosToInt(x, y, (ushort)(z + 1)), target);
                    break;
            }
        }
    }
}