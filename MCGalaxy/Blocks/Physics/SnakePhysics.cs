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
using BlockID = System.UInt16;

namespace MCGalaxy.Blocks.Physics {
    
    public static class SnakePhysics {
        
        public static void Do(Level lvl, ref PhysInfo C) {
            Random rand = lvl.physRandom;
            ushort x = C.X, y = C.Y, z = C.Z;
            int dirsVisited = 0;
            Player closest = HunterPhysics.ClosestPlayer(lvl, x, y, z);
            
            if (closest != null && rand.Next(1, 20) < 19) {
                switch (rand.Next(1, 10)) {
                    case 1:
                    case 2:
                    case 3:
                        ushort xx = (ushort)(x + Math.Sign(closest.Pos.BlockX - x));
                        if (xx != x && MoveSnake(lvl, ref C, xx, y, z)) return;
                        
                        dirsVisited++;
                        if (dirsVisited >= 3) break;
                        goto case 4;
                    case 4:
                    case 5:
                    case 6:
                        ushort yy = (ushort)(y + Math.Sign(closest.Pos.BlockY - y));
                        if (yy != y && MoveSnakeY(lvl, ref C, x, yy, z)) return;
                        
                        dirsVisited++;
                        if (dirsVisited >= 3) break;
                        goto case 7;
                    case 7:
                    case 8:
                    case 9:
                        ushort zz = (ushort)(z + Math.Sign(closest.Pos.BlockZ - z));
                        if (zz != z && MoveSnake(lvl, ref C, x, y, zz)) return;
                        
                        dirsVisited++;
                        if (dirsVisited >= 3) break;
                        goto case 1;
                }
            }
            
            dirsVisited = 0;
            switch (rand.Next(1, 13)) {
                case 1:
                case 2:
                case 3:
                    if (MoveSnake(lvl, ref C, (ushort)(x - 1), y, z)) return;

                    dirsVisited++;
                    if (dirsVisited >= 4) return;
                    goto case 4;
                case 4:
                case 5:
                case 6:
                    if (MoveSnake(lvl, ref C, (ushort)(x + 1), y, z)) return;

                    dirsVisited++;
                    if (dirsVisited >= 4) return;
                    goto case 7;
                case 7:
                case 8:
                case 9:
                    if (MoveSnake(lvl, ref C, x, y, (ushort)(z + 1))) return;

                    dirsVisited++;
                    if (dirsVisited >= 4) return;
                    goto case 10;
                case 10:
                case 11:
                case 12:
                default:
                    if (MoveSnake(lvl, ref C, x, y, (ushort)(z - 1))) return;
                    
                    dirsVisited++;
                    if (dirsVisited >= 4) return;
                    goto case 1;
            }
        }
        
        public static void DoTail(Level lvl, ref PhysInfo C) {
            ushort x = C.X, y = C.Y, z = C.Z;
            bool revert =
                lvl.GetBlock((ushort)(x - 1), y, z) != Block.Snake ||
                lvl.GetBlock((ushort)(x + 1), y, z) != Block.Snake ||
                lvl.GetBlock(x, y, (ushort)(z - 1)) != Block.Snake ||
                lvl.GetBlock(x, y, (ushort)(z + 1)) != Block.Snake;
            if (revert) {
                C.Data.Type1 = PhysicsArgs.Revert; C.Data.Value1 = Block.Air;
            }
        }
        
        static bool MoveSnake(Level lvl, ref PhysInfo C, ushort x, ushort y, ushort z) {
            int index;
            
            // Move snake up or down blocks
            if (       lvl.IsAirAt(x, (ushort)(y - 1), z, out index) && lvl.IsAirAt(x, y,               z)) {
            } else if (lvl.IsAirAt(x, y,               z, out index) && lvl.IsAirAt(x, (ushort)(y + 1), z)) {
            } else if (lvl.IsAirAt(x, (ushort)(y + 1), z, out index) && lvl.IsAirAt(x, (ushort)(y + 2), z)) {
            } else {
                return false;
            }

            if (lvl.AddUpdate(index, C.Block)) {
                PhysicsArgs args = default(PhysicsArgs);
                args.Type1 = PhysicsArgs.Wait; args.Value1 = 5;
                args.Type2 = PhysicsArgs.Revert; args.Value2 = Block.Air;
                lvl.AddUpdate(C.Index, Block.SnakeTail, args, true);
                return true;
            }
            return false;
        }
        
        static bool MoveSnakeY(Level lvl, ref PhysInfo C, ushort x, ushort y, ushort z) {
            int index;
            BlockID block  = lvl.GetBlock(x, y, z, out index);
            BlockID above  = lvl.GetBlock(x, (ushort)(y + 1), z);
            BlockID above2 = lvl.GetBlock(x, (ushort)(y + 2), z);
            
            if (block == Block.Air && (above == Block.Grass || above == Block.Dirt && above2 == Block.Air)) {
                if (lvl.AddUpdate(index, C.Block)) {
                    PhysicsArgs args = default(PhysicsArgs);
                    args.Type1 = PhysicsArgs.Wait; args.Value1 = 5;
                    args.Type2 = PhysicsArgs.Revert; args.Value2 = Block.Air;
                    lvl.AddUpdate(C.Index, Block.SnakeTail, args, true);
                    return true;
                }
            }
            return false;
        }
    }
}
