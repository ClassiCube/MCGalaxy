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
    
    public static class LiquidPhysics {
        
        public static void PhysWater(Level lvl, ushort x, ushort y, ushort z, byte type) {
            int b = lvl.PosToInt(x, y, z);
            if (b == -1) return;
            if (Server.lava.active && Server.lava.map == lvl && Server.lava.InSafeZone(x, y, z))
                return;

            switch (lvl.blocks[b]) {
                case Block.air:
                    if (!lvl.CheckSpongeWater(x, y, z)) lvl.AddUpdate(b, type);
                    break;

                case Block.lava:
                case Block.lava_fast:
                case Block.activedeathlava:
                    if (!lvl.CheckSpongeWater(x, y, z)) lvl.AddUpdate(b, Block.rock);
                    break;

                case Block.sand:
                case Block.gravel:
                case Block.wood_float:
                    lvl.AddCheck(b); break;
                    
                default:
                    // //Adv physics kills flowers and mushrooms in water
                    byte block = lvl.blocks[b];
                    if (block != Block.custom_block) {
                        if (!Block.Props[block].WaterKills) break;
                    } else {
                        block = lvl.GetExtTile(b);
                        if (!lvl.CustomBlockProps[block].WaterKills) break;
                    }
                    
                    if (lvl.physics > 1 && !lvl.CheckSpongeWater(x, y, z)) 
                        lvl.AddUpdate(b, Block.air);
                    break;
            }
        }
        
        public static void PhysLava(Level lvl, ushort x, ushort y, ushort z, byte type) {
            int b = lvl.PosToInt(x, y, z);
            if (b == -1) return;
            if (Server.lava.active && Server.lava.map == lvl && Server.lava.InSafeZone(x, y, z))
                return;

            switch (lvl.blocks[b]) {
                case Block.air:
                    if (!lvl.CheckSpongeLava(x, y, z)) lvl.AddUpdate(b, type);
                    break;
                    
                case Block.water:
                case Block.activedeathwater:
                    if (!lvl.CheckSpongeLava(x, y, z)) lvl.AddUpdate(b, Block.rock); 
                    break;
                    
                case Block.sand:
                    if (lvl.physics > 1) { //Adv physics changes sand to glass next to lava
                        if (lvl.physics != 5) lvl.AddUpdate(b, Block.glass);
                    } else {
                        lvl.AddCheck(b);
                    } break;
                    
                case Block.gravel:
                    lvl.AddCheck(b); break;

                default:
                    //Adv physics kills flowers, wool, mushrooms, and wood type blocks in lava
                    byte block = lvl.blocks[b];
                    if (block != Block.custom_block) {
                        if (!Block.Props[block].LavaKills) break;
                    } else {
                        block = lvl.GetExtTile(b);
                        if (!lvl.CustomBlockProps[block].LavaKills) break;
                    }
                    
                    if (lvl.physics > 1 && !lvl.CheckSpongeLava(x, y, z)) 
                        lvl.AddUpdate(b, Block.air);
                    break;
            }
        }
    }
}
