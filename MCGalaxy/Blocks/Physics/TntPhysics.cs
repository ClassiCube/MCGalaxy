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
using System.Collections.Generic;
using MCGalaxy.Games;
using BlockID = System.UInt16;

namespace MCGalaxy.Blocks.Physics {
    
    public static class TntPhysics {
        
        internal static void ToggleFuse(Level lvl, ushort x, ushort y, ushort z) {
            if (lvl.GetBlock(x, y, z) == Block.StillLava) {
                lvl.Blockchange(x, y, z, Block.Air);
            } else {
                lvl.Blockchange(x, y, z, Block.StillLava);
            }
        }
        
        public static void DoTntExplosion(Level lvl, ref PhysInfo C) {
            Random rand = lvl.physRandom;
            if (rand.Next(1, 11) <= 7)
                lvl.AddUpdate(C.Index, Block.Air, default(PhysicsArgs));
        }
        
        public static void DoSmallTnt(Level lvl, ref PhysInfo C) {
            ushort x = C.X, y = C.Y, z = C.Z;
            if (lvl.physics < 3) {
                lvl.Blockchange(x, y, z, Block.Air);
            } else {
                if (C.Data.Data < 5 && lvl.physics == 3) {
                    C.Data.Data++;
                    ToggleFuse(lvl, x, (ushort)(y + 1), z);
                    return;
                }
                MakeExplosion(lvl, x, y, z, 0);
            }
        }
        
        public static void DoBigTnt(Level lvl, ref PhysInfo C) { DoLargeTnt(lvl, ref C, 1); }     
        public static void DoNukeTnt(Level lvl, ref PhysInfo C) { DoLargeTnt(lvl, ref C, 4); }
        
        public static void DoLargeTnt(Level lvl, ref PhysInfo C, int power) {
            ushort x = C.X, y = C.Y, z = C.Z;
            
            if (lvl.physics < 3) {
                lvl.Blockchange(x, y, z, Block.Air);
            } else {
                if (C.Data.Data < 5 && lvl.physics == 3) {
                    C.Data.Data++;
                    
                    ToggleFuse(lvl, x, (ushort)(y + 1), z);
                    ToggleFuse(lvl, x, (ushort)(y - 1), z);
                    ToggleFuse(lvl, (ushort)(x + 1), y, z);
                    ToggleFuse(lvl, (ushort)(x - 1), y, z);
                    ToggleFuse(lvl, x, y, (ushort)(z + 1));
                    ToggleFuse(lvl, x, y, (ushort)(z - 1));
                    return;
                }
                MakeExplosion(lvl, x, y, z, power);
            }
        }
        
        public static void MakeExplosion(Level lvl, ushort x, ushort y, ushort z, int size,
                                         bool force = false, TWGame game = null) {
            Random rand = new Random();
            if ((lvl.physics < 2 || lvl.physics == 5) && !force) return;
            
            int index;
            BlockID block = lvl.GetBlock(x, y, z, out index);
            if (index >= 0 && !lvl.Props[block].OPBlock) {
                lvl.AddUpdate(index, Block.TNT_Explosion, default(PhysicsArgs), true);
            }

            Explode(lvl, x, y, z, size + 1, rand, -1, game);
            Explode(lvl, x, y, z, size + 2, rand, 7, game);
            Explode(lvl, x, y, z, size + 3, rand, 3, game);
        }
        
        static bool IsFuse(BlockID b, int dx, int dy, int dz) {
            return dx == 0 && dy == 1 && dz == 0 && b == Block.StillLava;
        }
        
        static void Explode(Level lvl, ushort x, ushort y, ushort z,
                            int size, Random rand, int prob, TWGame game) {
            for (int xx = (x - size); xx <= (x + size ); ++xx)
                for (int yy = (y - size); yy <= (y + size); ++yy)
                    for (int zz = (z - size); zz <= (z + size); ++zz)
            {
                int index;
                BlockID b = lvl.GetBlock((ushort)xx, (ushort)yy, (ushort)zz, out index);
                if (b == Block.Invalid) continue;
                
                bool doDestroy = prob < 0 || rand.Next(1, 10) < prob;
                if (doDestroy && Block.Convert(b) != Block.TNT) {
                    if (game != null && b != Block.Air && !IsFuse(b, xx - x, yy - y, zz - z)) {
                        if (game.InZone((ushort)xx, (ushort)yy, (ushort)zz, game.tntImmuneZones))
                            continue;
                    }
                    
                    int mode = rand.Next(1, 11);
                    if (mode <= 4) {
                        lvl.AddUpdate(index, Block.TNT_Explosion, default(PhysicsArgs));
                    } else if (mode <= 8) {
                        lvl.AddUpdate(index, Block.Air, default(PhysicsArgs));
                    } else {
                        PhysicsArgs args = default(PhysicsArgs);
                        args.Type1 = PhysicsArgs.Drop;      args.Value1 = 50;
                        args.Type2 = PhysicsArgs.Dissipate; args.Value2 = 8;
                        lvl.AddCheck(index, false, args);
                    }
                } else if (b == Block.TNT) {
                    lvl.AddUpdate(index, Block.TNT_Small, default(PhysicsArgs));
                } else if (b == Block.TNT_Small || b == Block.TNT_Big || b == Block.TNT_Nuke) {
                    lvl.AddCheck(index);
                }
            }
        }
    }
}
