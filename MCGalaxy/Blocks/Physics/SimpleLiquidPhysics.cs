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
    
    public static class SimpleLiquidPhysics {
        
        const StringComparison comp = StringComparison.Ordinal;
        public static void DoWater(Level lvl, ref Check C) {
            if (lvl.finite) {
                FinitePhysics.DoWaterOrLava(lvl, ref C);
            } else if (lvl.randomFlow) {
                DoWaterRandowFlow(lvl, ref C);
            } else {
                DoWaterUniformFlow(lvl, ref C);
            }
        }
        
        public static void DoLava(Level lvl, ref Check C) {
            // upper 3 bits are time delay
            if (C.data.Data < (4 << 5)) {
                C.data.Data += (1 << 5); return;
            }
            
            if (lvl.finite) {
                FinitePhysics.DoWaterOrLava(lvl, ref C);
            } else if (lvl.randomFlow) {
                DoLavaRandowFlow(lvl, ref C, true);
            } else {
                DoLavaUniformFlow(lvl, ref C, true);
            }
        }
        
        public static void DoFastLava(Level lvl, ref Check C) {
            if (lvl.randomFlow) {
                DoLavaRandowFlow(lvl, ref C, false);
                if (C.data.Data != PhysicsArgs.RemoveFromChecks)
                    C.data.Data = 0; // no lava delay
            } else {
                DoLavaUniformFlow(lvl, ref C, false);
            }
        }
        
        static void DoWaterRandowFlow(Level lvl, ref Check C) {
            Random rand = lvl.physRandom;
            ushort x, y, z;
            lvl.IntToPos(C.b, out x, out y, out z);
            
            if (!lvl.CheckSpongeWater(x, y, z)) {
                byte flowed = C.data.Data;
                byte block = lvl.blocks[C.b];
                if (y < lvl.Height - 1)
                    CheckFallingBlocks(lvl, C.b + lvl.Width * lvl.Length);
                
                if ((flowed & (1 << 0)) == 0 && rand.Next(4) == 0) {
                    LiquidPhysics.PhysWater(lvl, (ushort)(x + 1), y, z, block);
                    flowed |= (1 << 0);
                }
                if ((flowed & (1 << 1)) == 0 && rand.Next(4) == 0) {
                    LiquidPhysics.PhysWater(lvl, (ushort)(x - 1), y, z, block);
                    flowed |= (1 << 1);
                }
                if ((flowed & (1 << 2)) == 0 && rand.Next(4) == 0) {
                    LiquidPhysics.PhysWater(lvl, x, y, (ushort)(z + 1), block);
                    flowed |= (1 << 2);
                }
                if ((flowed & (1 << 3)) == 0 && rand.Next(4) == 0) {
                    LiquidPhysics.PhysWater(lvl, x, y, (ushort)(z - 1), block);
                    flowed |= (1 << 3);
                }
                if ((flowed & (1 << 4)) == 0 && rand.Next(4) == 0) {
                    LiquidPhysics.PhysWater(lvl, x, (ushort)(y - 1), z, block);
                    flowed |= (1 << 4);
                }

                if ((flowed & (1 << 0)) == 0 && WaterBlocked(lvl, (ushort)(x + 1), y, z)) {
                    flowed |= (1 << 0);
                }
                if ((flowed & (1 << 1)) == 0 && WaterBlocked(lvl, (ushort)(x - 1), y, z)) {
                    flowed |= (1 << 1);
                }
                if ((flowed & (1 << 2)) == 0 && WaterBlocked(lvl, x, y, (ushort)(z + 1))) {
                    flowed |= (1 << 2);
                }
                if ((flowed & (1 << 3)) == 0 && WaterBlocked(lvl, x, y, (ushort)(z - 1))) {
                    flowed |= (1 << 3);
                }
                if ((flowed & (1 << 4)) == 0 && WaterBlocked(lvl, x, (ushort)(y - 1), z)) {
                    flowed |= (1 << 4);
                }
                
                // Have we spread now (or been blocked from spreading) in all directions?
                C.data.Data = flowed;
                if (!C.data.HasWait && (flowed & 0x1F) == 0x1F) {
                    C.data.Data = PhysicsArgs.RemoveFromChecks;
                }
            } else { //was placed near sponge
                lvl.AddUpdate(C.b, Block.air);
                if (!C.data.HasWait) {
                    C.data.Data = PhysicsArgs.RemoveFromChecks;
                }
            }
        }
        
        static void DoWaterUniformFlow(Level lvl, ref Check C) {
            ushort x, y, z;
            lvl.IntToPos(C.b, out x, out y, out z);
            
            if (!lvl.CheckSpongeWater(x, y, z)) {
                byte block = lvl.blocks[C.b];
                if (y < lvl.Height - 1)
                    CheckFallingBlocks(lvl, C.b + lvl.Width * lvl.Length);
                LiquidPhysics.PhysWater(lvl, (ushort)(x + 1), y, z, block);
                LiquidPhysics.PhysWater(lvl, (ushort)(x - 1), y, z, block);
                LiquidPhysics.PhysWater(lvl, x, y, (ushort)(z + 1), block);
                LiquidPhysics.PhysWater(lvl, x, y, (ushort)(z - 1), block);
                LiquidPhysics.PhysWater(lvl, x, (ushort)(y - 1), z, block);
            } else { //was placed near sponge
                lvl.AddUpdate(C.b, Block.air);
            }
            if (!C.data.HasWait) C.data.Data = PhysicsArgs.RemoveFromChecks;
        }
        
        static bool WaterBlocked(Level lvl, ushort x, ushort y, ushort z) {
            int b;
            ExtBlock block = lvl.GetBlock(x, y, z, out b);
            if (b == -1) return true;
            if (Server.lava.active && Server.lava.map == lvl && Server.lava.InSafeZone(x, y, z))
                return true;

            switch (block.BlockID) {
                case Block.air:
                case Block.lava:
                case Block.lava_fast:
                case Block.activedeathlava:
                    if (!lvl.CheckSpongeWater(x, y, z)) return false;
                    break;

                case Block.sand:
                case Block.gravel:
                case Block.wood_float:
                    return false;
                    
                default:
                    // Adv physics kills flowers, mushroom blocks in water
                    if (!lvl.BlockProps[block.Index].WaterKills) break;
                    
                    if (lvl.physics > 1 && !lvl.CheckSpongeWater(x, y, z)) return false;
                    break;
            }
            return true;
        }
        
        static void DoLavaRandowFlow(Level lvl, ref Check C, bool checkWait) {
            Random rand = lvl.physRandom;
            ushort x, y, z;
            lvl.IntToPos(C.b, out x, out y, out z);

            if (!lvl.CheckSpongeLava(x, y, z)) {
                byte flowed = C.data.Data;
                // Upper 3 bits are time flags - reset random delay
                flowed &= 0x1F;
                flowed |= (byte)(rand.Next(3) << 5);
                byte block = lvl.blocks[C.b];

                if ((flowed & (1 << 0)) == 0 && rand.Next(4) == 0) {
                    LiquidPhysics.PhysLava(lvl, (ushort)(x + 1), y, z, block);
                    flowed |= (1 << 0);
                }
                if ((flowed & (1 << 1)) == 0 && rand.Next(4) == 0) {
                    LiquidPhysics.PhysLava(lvl, (ushort)(x - 1), y, z, block);
                    flowed |= (1 << 1);
                }
                if ((flowed & (1 << 2)) == 0 && rand.Next(4) == 0) {
                    LiquidPhysics.PhysLava(lvl, x, y, (ushort)(z + 1), block);
                    flowed |= (1 << 2);
                }
                if ((flowed & (1 << 3)) == 0 && rand.Next(4) == 0) {
                    LiquidPhysics.PhysLava(lvl, x, y, (ushort)(z - 1), block);
                    flowed |= (1 << 3);
                }
                if ((flowed & (1 << 4)) == 0 && rand.Next(4) == 0) {
                    LiquidPhysics.PhysLava(lvl, x, (ushort)(y - 1), z, block);
                    flowed |= (1 << 4);
                }

                if ((flowed & (1 << 0)) == 0 && LavaBlocked(lvl, (ushort)(x + 1), y, z)) {
                    flowed |= (1 << 0);
                }
                if ((flowed & (1 << 1)) == 0 && LavaBlocked(lvl, (ushort)(x - 1), y, z)) {
                    flowed |= (1 << 1);
                }
                if ((flowed & (1 << 2)) == 0 && LavaBlocked(lvl, x, y, (ushort)(z + 1))) {
                    flowed |= (1 << 2);
                }
                if ((flowed & (1 << 3)) == 0 && LavaBlocked(lvl, x, y, (ushort)(z - 1))) {
                    flowed |= (1 << 3);
                }
                if ((flowed & (1 << 4)) == 0 && LavaBlocked(lvl, x, (ushort)(y - 1), z)) {
                    flowed |= (1 << 4);
                }
                
                // Have we spread now (or been blocked from spreading) in all directions?
                C.data.Data = flowed;
                if ((!checkWait || !C.data.HasWait) && (flowed & 0x1F) == 0x1F) {
                    C.data.Data = PhysicsArgs.RemoveFromChecks;
                }
            } else { //was placed near sponge
                lvl.AddUpdate(C.b, Block.air);
                if (!checkWait || !C.data.HasWait) {
                    C.data.Data = PhysicsArgs.RemoveFromChecks;
                }
            }
        }
        
        static void DoLavaUniformFlow(Level lvl, ref Check C, bool checkWait) {
            ushort x, y, z;
            lvl.IntToPos(C.b, out x, out y, out z);
            
            if (!lvl.CheckSpongeLava(x, y, z)) {
                byte block = lvl.blocks[C.b];
                LiquidPhysics.PhysLava(lvl, (ushort)(x + 1), y, z, block);
                LiquidPhysics.PhysLava(lvl, (ushort)(x - 1), y, z, block);
                LiquidPhysics.PhysLava(lvl, x, y, (ushort)(z + 1), block);
                LiquidPhysics.PhysLava(lvl, x, y, (ushort)(z - 1), block);
                LiquidPhysics.PhysLava(lvl, x, (ushort)(y - 1), z, block);
            } else { //was placed near sponge
                lvl.AddUpdate(C.b, Block.air);
            }
            
            if (!checkWait || !C.data.HasWait) {
                C.data.Data = PhysicsArgs.RemoveFromChecks;
            }
        }
        
        static bool LavaBlocked(Level lvl, ushort x, ushort y, ushort z) {
            int b;
            ExtBlock block = lvl.GetBlock(x, y, z, out b);
            if (b == -1) return true;
            if (Server.lava.active && Server.lava.map == lvl && Server.lava.InSafeZone(x, y, z))
                return true;
            
            switch (block.BlockID) {
                case Block.air:
                    return false;

                case Block.water:
                case Block.activedeathwater:
                    if (!lvl.CheckSpongeLava(x, y, z)) return false;
                    break;

                case Block.sand:
                case Block.gravel:
                    return false;

                default:
                    // Adv physics kills flowers, wool, mushrooms, and wood type blocks in lava
                    if (!lvl.BlockProps[block.Index].LavaKills) break;

                    if (lvl.physics > 1 && !lvl.CheckSpongeLava(x, y, z)) return false;
                    break;
            }
            return true;
        }
        
        static void CheckFallingBlocks(Level lvl, int b) {
            switch (lvl.blocks[b]) {
                case Block.sand:
                case Block.gravel:
                case Block.wood_float:
                    lvl.AddCheck(b); break;
                default:
                    break;
            }
        }
    }
}
