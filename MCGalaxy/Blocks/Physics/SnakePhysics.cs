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

namespace MCGalaxy.Blocks.Physics {
    
    public static class SnakePhysics {
        
        public static void Do(Level lvl, ref Check C) {
            Random rand = lvl.physRandom;            
            ushort x, y, z;
            lvl.IntToPos(C.b, out x, out y, out z);
            int dirsVisited = 0, index = 0;
            Player closest = AIPhysics.ClosestPlayer(lvl, x, y, z);
            
            if (closest != null && rand.Next(1, 20) < 19) {
                switch (rand.Next(1, 10)) {
                    case 1:
                    case 2:
                    case 3:
                        index = lvl.PosToInt((ushort)(x + Math.Sign(closest.Pos.BlockX - x)), y, z);
                        if (index != C.b && MoveSnake(lvl, ref C, index)) return;
                        
                        dirsVisited++;
                        if (dirsVisited >= 3) break;
                        goto case 4;
                    case 4:
                    case 5:
                    case 6:
                        index = lvl.PosToInt(x, (ushort) (y + Math.Sign(closest.Pos.BlockY - y)), z);
                        if (index != C.b && MoveSnakeY(lvl, ref C, index)) return;
                        
                        dirsVisited++;
                        if (dirsVisited >= 3) break;
                        goto case 7;
                    case 7:
                    case 8:
                    case 9:
                        index = lvl.PosToInt(x, y, (ushort)(z + Math.Sign(closest.Pos.BlockZ - z)));
                        if (index != C.b && MoveSnake(lvl, ref C, index)) return;
                        
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
                    index = lvl.IntOffset(C.b, -1, 0, 0);
                    if (MoveSnake(lvl, ref C, index)) return;

                    dirsVisited++;
                    if (dirsVisited >= 4) return;
                    goto case 4;
                case 4:
                case 5:
                case 6:
                    index = lvl.IntOffset(C.b, 1, 0, 0);
                    if (MoveSnake(lvl, ref C, index)) return;

                    dirsVisited++;
                    if (dirsVisited >= 4) return;
                    goto case 7;
                case 7:
                case 8:
                case 9:
                    index = lvl.IntOffset(C.b, 0, 0, 1);
                    if (MoveSnake(lvl, ref C, index)) return;

                    dirsVisited++;
                    if (dirsVisited >= 4) return;
                    goto case 10;
                case 10:
                case 11:
                case 12:
                default:
                    index = lvl.IntOffset(C.b, 0, 0, -1);
                    if (MoveSnake(lvl, ref C, index)) return;
                    
                    dirsVisited++;
                    if (dirsVisited >= 4) return;
                    goto case 1;
            }
        }
        
        public static void DoTail(Level lvl, ref Check C) {
            if (lvl.GetTile(lvl.IntOffset(C.b, -1, 0, 0)) != Block.Snake
                || lvl.GetTile(lvl.IntOffset(C.b, 1, 0, 0)) != Block.Snake
                || lvl.GetTile(lvl.IntOffset(C.b, 0, 0, 1)) != Block.Snake
                || lvl.GetTile(lvl.IntOffset(C.b, 0, 0, -1)) != Block.Snake) {
                C.data.Type1 = PhysicsArgs.Revert; C.data.Value1 = Block.Air;
            }
        }
        
        static bool MoveSnake(Level lvl, ref Check C, int index) {
            if (
                lvl.GetTile(lvl.IntOffset(index, 0, -1, 0)) == Block.Air &&
                lvl.GetTile(index) == Block.Air) {
                index = lvl.IntOffset(index, 0, -1, 0);
            } else if (
                lvl.GetTile(index) == Block.Air &&
                lvl.GetTile(lvl.IntOffset(index, 0, 1, 0)) == Block.Air) {
            } else if (
                lvl.GetTile(lvl.IntOffset(index, 0, 2, 0)) == Block.Air &&
                lvl.GetTile(lvl.IntOffset(index, 0, 1, 0)) == Block.Air) {
                index = lvl.IntOffset(index, 0, 1, 0);
            } else {
                return false;
            }

            if (lvl.AddUpdate(index, lvl.blocks[C.b])) {
                PhysicsArgs args = default(PhysicsArgs);
                args.Type1 = PhysicsArgs.Wait; args.Value1 = 5;
                args.Type2 = PhysicsArgs.Revert; args.Value2 = Block.Air;
                lvl.AddUpdate(C.b, Block.SnakeTail, true, args);
                return true;
            }
            return false;
        }
        
        static bool MoveSnakeY(Level lvl, ref Check C, int index ) {
            byte block = lvl.GetTile(index);
            byte blockAbove = lvl.GetTile(lvl.IntOffset(index, 0, 1, 0));
            byte block2Above = lvl.GetTile(lvl.IntOffset(index, 0, 2, 0));
            
            if (block == Block.Air &&
                (blockAbove == Block.Grass ||
                 blockAbove == Block.Dirt && block2Above == Block.Air)) {
                if (lvl.AddUpdate(index, lvl.blocks[C.b])) {
                    PhysicsArgs args = default(PhysicsArgs);
                    args.Type1 = PhysicsArgs.Wait; args.Value1 = 5;
                    args.Type2 = PhysicsArgs.Revert; args.Value2 = Block.Air;
                    lvl.AddUpdate(C.b, Block.SnakeTail, true, args);
                    return true;
                }            
            }
            return false;
        }
    }
}
