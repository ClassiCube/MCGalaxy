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
using MCGalaxy.Games;
using BlockID = System.UInt16;

namespace MCGalaxy.Blocks.Physics {
    
    public static class LiquidPhysics {
        
        public static void PhysWater(Level lvl, ushort x, ushort y, ushort z, BlockID type) {
            int index;
            BlockID block = lvl.GetBlock(x, y, z, out index);
            if (LSGame.Instance.Running && LSGame.Instance.Map == lvl && LSGame.Instance.InSafeZone(x, y, z))
                return;

            switch (block) {
                case Block.Air:
                    if (!lvl.CheckSpongeWater(x, y, z)) {
                        lvl.AddUpdate(index, type);
                    }
                    break;

                case Block.Lava:
                case Block.FastLava:
                case Block.Deadly_ActiveLava:
                    if (!lvl.CheckSpongeWater(x, y, z)) {
                        lvl.AddUpdate(index, Block.Stone, default(PhysicsArgs));
                    }
                    break;

                case Block.Sand:
                case Block.Gravel:
                case Block.FloatWood:
                    lvl.AddCheck(index); break;
                    
                default:
                    // Adv physics kills flowers and mushrooms in water
                    if (!lvl.Props[block].WaterKills) break;
                    
                    if (lvl.physics > 1 && !lvl.CheckSpongeWater(x, y, z)) {
                        lvl.AddUpdate(index, Block.Air, default(PhysicsArgs));
                    }
                    break;
            }
        }
        
        public static void PhysLava(Level lvl, ushort x, ushort y, ushort z, BlockID type) {
            int index;
            BlockID block = lvl.GetBlock(x, y, z, out index);
            if (LSGame.Instance.Running && LSGame.Instance.Map == lvl && LSGame.Instance.InSafeZone(x, y, z))
                return;

            switch (block) {
                case Block.Air:
                    if (!lvl.CheckSpongeLava(x, y, z)) {
                        lvl.AddUpdate(index, type);
                    }
                    break;
                    
                case Block.Water:
                case Block.Deadly_ActiveWater:
                    if (!lvl.CheckSpongeLava(x, y, z)) {
                        lvl.AddUpdate(index, Block.Stone, default(PhysicsArgs));
                    }
                    break;
                    
                case Block.Sand:
                    if (lvl.physics > 1) { //Adv physics changes sand to glass next to lava
                        lvl.AddUpdate(index, Block.Glass, default(PhysicsArgs));
                    } else {
                        lvl.AddCheck(index);
                    } break;
                    
                case Block.Gravel:
                    lvl.AddCheck(index); break;

                default:
                    //Adv physics kills flowers, wool, mushrooms, and wood type blocks in lava
                    if (!lvl.Props[block].LavaKills) break;
                    
                    if (lvl.physics > 1 && !lvl.CheckSpongeLava(x, y, z)) {
                        lvl.AddUpdate(index, Block.Air, default(PhysicsArgs));
                    }
                    break;
            }
        }
    }
}
