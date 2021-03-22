/*
    Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCGalaxy)
        
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
using BlockID = System.UInt16;

namespace MCGalaxy.Blocks.Physics {
    
    public static class FirePhysics {
        
        static bool ExpandToAir(Level lvl, int x, int y, int z) {
            int index;
            if (lvl.IsAirAt((ushort)x, (ushort)y, (ushort)z, out index)) {
                lvl.AddUpdate(index, Block.Fire, default(PhysicsArgs));
                return true;
            }
            return false;
        }
        
        static void ExpandDiagonal(Level lvl, ushort x, ushort y, ushort z,
                                   int dx, int dy, int dz) {
            BlockID block = lvl.GetBlock((ushort)(x + dx), (ushort)(y + dy), (ushort)(z + dz));
            if (block == Block.Air || !lvl.Props[block].LavaKills) return;
            
            if (dx != 0)
                lvl.AddUpdate(lvl.PosToInt((ushort)(x + dx), y, z), Block.Fire, default(PhysicsArgs));
            if (dy != 0)
                lvl.AddUpdate(lvl.PosToInt(x, (ushort)(y + dy), z), Block.Fire, default(PhysicsArgs));
            if (dz != 0)
                lvl.AddUpdate(lvl.PosToInt(x, y, (ushort)(z + dz)), Block.Fire, default(PhysicsArgs));
        }
        
        static void ExpandAvanced(Level lvl, int x, int y, int z) {
            int index;
            BlockID block = lvl.GetBlock((ushort)x, (ushort)y, (ushort)z, out index);
            
            if (block == Block.TNT) {
                lvl.MakeExplosion((ushort)x, (ushort)y, (ushort)z, -1);
            } else if (block != Block.Air && lvl.Props[block].LavaKills) {
                lvl.AddUpdate(index, Block.Fire, default(PhysicsArgs));
            }
        }
        
        public static void Do(Level lvl, ref PhysInfo C) {
            if (C.Data.Data < 2) {
                C.Data.Data++;
                return;
            }

            ushort x = C.X, y = C.Y, z = C.Z;
            Random rand = lvl.physRandom;
            if (rand.Next(1, 20) == 1 && C.Data.Data % 2 == 0) {
                int max = rand.Next(1, 18);

                if (max <= 3 && ExpandToAir(lvl, x - 1, y, z)) {
                } else if (max <= 6 && ExpandToAir(lvl, x + 1, y, z)) {
                } else if (max <= 9 && ExpandToAir(lvl, x, y - 1, z)) {
                } else if (max <= 12 && ExpandToAir(lvl, x, y + 1, z)) {
                } else if (max <= 15 && ExpandToAir(lvl, x, y, z - 1)) {
                } else if (max <= 18 && ExpandToAir(lvl, x, y, z + 1)) {
                }
            }

            if (lvl.physics >= 2) {
                for (int yy = -1; yy <= 1; yy++ ) {
                    ExpandDiagonal(lvl, x, y, z, -1, yy, -1);
                    ExpandDiagonal(lvl, x, y, z, +1, yy, -1);
                    ExpandDiagonal(lvl, x, y, z, -1, yy, +1);
                    ExpandDiagonal(lvl, x, y, z, +1, yy, +1);
                }
            	
                if (C.Data.Data < 4) {
                    C.Data.Data++;
                    return;
                }
                
                ExpandAvanced(lvl, x - 1, y, z);
                ExpandAvanced(lvl, x + 1, y, z);
                ExpandAvanced(lvl, x, y - 1, z);
                ExpandAvanced(lvl, x, y + 1, z);
                ExpandAvanced(lvl, x, y, z - 1);
                ExpandAvanced(lvl, x, y, z + 1);
            }

            C.Data.Data++;
            if (C.Data.Data > 5) {
                int dropType = rand.Next(1, 10);
                if (dropType <= 2) {
                    lvl.AddUpdate(C.Index, Block.CoalOre, default(PhysicsArgs));
                    C.Data.Type1 = PhysicsArgs.Drop; C.Data.Value1 = 63;
                    C.Data.Type2 = PhysicsArgs.Dissipate; C.Data.Value2 = 10;
                } else if (dropType <= 4) {
                    lvl.AddUpdate(C.Index, Block.Obsidian, default(PhysicsArgs));
                    C.Data.Type1 = PhysicsArgs.Drop; C.Data.Value1 = 63;
                    C.Data.Type2 = PhysicsArgs.Dissipate; C.Data.Value2 = 10;
                } else if (dropType <= 8) {
                    lvl.AddUpdate(C.Index, Block.Air, default(PhysicsArgs));
                } else {
                    C.Data.Data = 3;
                }
            }
        }
    }
}
