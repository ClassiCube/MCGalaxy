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
    
    public static class SimpleLiquidPhysics {
        
        const StringComparison comp = StringComparison.Ordinal;
        public static void DoWater(Level lvl, Check C) {
            Random rand = lvl.physRandom;        	
            if (lvl.finite) {
                lvl.liquids.Remove(C.b);
                FinitePhysics.DoWaterOrLava(lvl, C);
                return;
            }        
            if (lvl.randomFlow)
                DoWaterRandowFlow(lvl, C);
            else
                DoWaterUniformFlow(lvl, C);
        }
        
        public static void DoLava(Level lvl, Check C) {
            if (C.data.Data < 4) {
                C.data.Data++; return;
            }
            if (lvl.finite) {
                lvl.liquids.Remove(C.b);
                FinitePhysics.DoWaterOrLava(lvl, C);
                return;
            }
            if (lvl.randomFlow)
                DoLavaRandowFlow(lvl, C, true);
            else
                DoLavaUniformFlow(lvl, C, true);
        }
        
        public static void DoFastLava(Level lvl, Check C) {
            if (lvl.randomFlow) {
                byte oldTime = C.data.Data;
                DoLavaRandowFlow(lvl, C, false);
                if (C.data.Data != 255)
                    C.data.Data = oldTime;
            } else {
                DoLavaUniformFlow(lvl, C, false);
            }
        }
        
        static void DoWaterRandowFlow(Level lvl, Check C) {
            Random rand = lvl.physRandom;        	
            bool[] blocked = null;
            ushort x, y, z;
            lvl.IntToPos(C.b, out x, out y, out z);
            
            if (!lvl.CheckSpongeWater(x, y, z)) {
                if (!lvl.liquids.TryGetValue(C.b, out blocked)) {
                    blocked = new bool[5];
                    lvl.liquids.Add(C.b, blocked);
                }

                byte block = lvl.blocks[C.b];
                if (y < lvl.Height - 1)
                    CheckFallingBlocks(lvl, C.b + lvl.Width * lvl.Length);
                
                if (!blocked[0] && rand.Next(4) == 0) {
                    LiquidPhysics.PhysWater(lvl, (ushort)(x + 1), y, z, block);
                    blocked[0] = true;
                }
                if (!blocked[1] && rand.Next(4) == 0) {
                    LiquidPhysics.PhysWater(lvl, (ushort)(x - 1), y, z, block);
                    blocked[1] = true;
                }
                if (!blocked[2] && rand.Next(4) == 0) {
                    LiquidPhysics.PhysWater(lvl, x, y, (ushort)(z + 1), block);
                    blocked[2] = true;
                }
                if (!blocked[3] && rand.Next(4) == 0) {
                    LiquidPhysics.PhysWater(lvl, x, y, (ushort)(z - 1), block);
                    blocked[3] = true;
                }
                if (!blocked[4] && rand.Next(4) == 0) {
                    LiquidPhysics.PhysWater(lvl, x, (ushort)(y - 1), z, block);
                    blocked[4] = true;
                }

                if (!blocked[0] && WaterBlocked(lvl, (ushort)(x + 1), y, z))
                    blocked[0] = true;
                if (!blocked[1] && WaterBlocked(lvl, (ushort)(x - 1), y, z))
                    blocked[1] = true;
                if (!blocked[2] && WaterBlocked(lvl, x, y, (ushort)(z + 1)))
                    blocked[2] = true;
                if (!blocked[3] && WaterBlocked(lvl, x, y, (ushort)(z - 1)))
                    blocked[3] = true;
                if (!blocked[4] && WaterBlocked(lvl, x, (ushort)(y - 1), z))
                    blocked[4] = true;
            } else { //was placed near sponge
                lvl.liquids.TryGetValue(C.b, out blocked);
                lvl.AddUpdate(C.b, Block.air);                
                if (!C.data.HasWait) C.data.Data = 255;
            }

            if (!C.data.HasWait && blocked != null)
                if (blocked[0] && blocked[1] && blocked[2] && blocked[3] && blocked[4])
            {
                lvl.liquids.Remove(C.b);
                C.data.Data = 255;
            }
        }
        
        static void DoWaterUniformFlow(Level lvl, Check C) {
            Random rand = lvl.physRandom;        	
            lvl.liquids.Remove(C.b);
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
            if (!C.data.HasWait) C.data.Data = 255;
        }
        
        static bool WaterBlocked(Level lvl, ushort x, ushort y, ushort z) {
            int b = lvl.PosToInt(x, y, z);
            if (b == -1)
                return true;
            if (Server.lava.active && Server.lava.map == lvl && Server.lava.InSafeZone(x, y, z))
                return true;

            switch (lvl.blocks[b]) {
                case Block.air:
                case Block.lava:
                case Block.lava_fast:
                case Block.activedeathlava:
                    if (!lvl.CheckSpongeWater(x, y, z)) return false;
                    break;

                case Block.shrub:
                case Block.yellowflower:
                case Block.redflower:
                case Block.mushroom:
                case Block.redmushroom:
                    if (lvl.physics > 1 && !lvl.CheckSpongeWater(x, y, z)) return false;
                    break;

                case Block.sand:
                case Block.gravel:
                case Block.wood_float:
                    return false;
            }
            return true;
        }
        
        static void DoLavaRandowFlow(Level lvl, Check C, bool checkWait) {
            Random rand = lvl.physRandom;        	
            bool[] blocked = null;
            ushort x, y, z;
            lvl.IntToPos(C.b, out x, out y, out z);

            if (!lvl.CheckSpongeLava(x, y, z)) {
                C.data.Data = (byte)rand.Next(3);
                if (!lvl.liquids.TryGetValue(C.b, out blocked)) {
                    blocked = new bool[5];
                    lvl.liquids.Add(C.b, blocked);
                }
                byte block = lvl.blocks[C.b];

                if (!blocked[0] && rand.Next(4) == 0) {
                    LiquidPhysics.PhysLava(lvl, (ushort)(x + 1), y, z, block);
                    blocked[0] = true;
                }
                if (!blocked[1] && rand.Next(4) == 0) {
                    LiquidPhysics.PhysLava(lvl, (ushort)(x - 1), y, z, block);
                    blocked[1] = true;
                }
                if (!blocked[2] && rand.Next(4) == 0) {
                    LiquidPhysics.PhysLava(lvl, x, y, (ushort)(z + 1), block);
                    blocked[2] = true;
                }
                if (!blocked[3] && rand.Next(4) == 0) {
                    LiquidPhysics.PhysLava(lvl, x, y, (ushort)(z - 1), block);
                    blocked[3] = true;
                }
                if (!blocked[4] && rand.Next(4) == 0) {
                    LiquidPhysics.PhysLava(lvl, x, (ushort)(y - 1), z, block);
                    blocked[4] = true;
                }

                if (!blocked[0] && LavaBlocked(lvl, (ushort)(x + 1), y, z))
                    blocked[0] = true;
                if (!blocked[1] && LavaBlocked(lvl, (ushort)(x - 1), y, z))
                    blocked[1] = true;
                if (!blocked[2] && LavaBlocked(lvl, x, y, (ushort)(z + 1)))
                    blocked[2] = true;
                if (!blocked[3] && LavaBlocked(lvl, x, y, (ushort)(z - 1)))
                    blocked[3] = true;
                if (!blocked[4] && LavaBlocked(lvl, x, (ushort)(y - 1), z))
                    blocked[4] = true;
            } else { //was placed near sponge
                lvl.liquids.TryGetValue(C.b, out blocked);
                lvl.AddUpdate(C.b, Block.air);
                if (!checkWait || !C.data.HasWait)
                    C.data.Data = 255;
            }

            if (blocked != null && (!checkWait || !C.data.HasWait))
                if (blocked[0] && blocked[1] && blocked[2] && blocked[3] && blocked[4])
            {
                lvl.liquids.Remove(C.b);
                C.data.Data = 255;
            }
        }
        
        static void DoLavaUniformFlow(Level lvl, Check C, bool checkWait) {
            Random rand = lvl.physRandom;
            lvl.liquids.Remove(C.b);
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
            if (!checkWait || !C.data.HasWait) C.data.Data = 255;
        }
        
        static bool LavaBlocked(Level lvl, ushort x, ushort y, ushort z) {
            int b = lvl.PosToInt(x, y, z);
            if (b == -1)
                return true;
            if (Server.lava.active && Server.lava.map == lvl && Server.lava.InSafeZone(x, y, z))
                return true;

            if (lvl.physics > 1 && lvl.blocks[b] >= Block.red && lvl.blocks[b] <= Block.white 
                && !lvl.CheckSpongeLava(x, y, z))
                return false; // Adv physics destroys cloth
            
            switch (lvl.blocks[b]) {
                case Block.air:
                    return false;

                case Block.water:
                case Block.activedeathwater:
                    if (!lvl.CheckSpongeLava(x, y, z))  return false;
                    break;

                case Block.sand:
                case Block.gravel:
                    return false;

                case Block.wood:
                case Block.shrub:
                case Block.trunk:
                case Block.leaf:
                case Block.yellowflower:
                case Block.redflower:
                case Block.mushroom:
                case Block.redmushroom:
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
