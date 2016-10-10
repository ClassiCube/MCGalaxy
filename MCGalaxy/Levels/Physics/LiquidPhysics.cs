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
    
    public static class LiquidPhysics {
        
        public static void PhysWater(Level lvl, ushort x, ushort y, ushort z, byte type) {
            if (x >= lvl.Width || y >= lvl.Height || z >= lvl.Length) return;
            if (Server.lava.active && Server.lava.map == lvl && Server.lava.InSafeZone(x, y, z))
                return;
            
            int b = x + (z * lvl.Width) + (y * lvl.Width * lvl.Length);
            switch (lvl.blocks[b]) {
                case Block.air:
                    if (!lvl.CheckSpongeWater(x, y, z)) lvl.AddUpdate(b, type);
                    break;

                case Block.lava:
                case Block.lava_fast:
                case Block.activedeathlava:
                    if (!lvl.CheckSpongeWater(x, y, z)) lvl.AddUpdate(b, Block.rock);
                    break;

                case Block.shrub:
                case Block.yellowflower:
                case Block.redflower:
                case Block.mushroom:
                case Block.redmushroom:
                    if (lvl.physics > 1 && lvl.physics != 5 && !lvl.CheckSpongeWater(x, y, z))
                        lvl.AddUpdate(b, Block.air); //Adv physics kills flowers and mushrooms in water
                    break;

                case Block.sand:
                case Block.gravel:
                case Block.wood_float:
                    lvl.AddCheck(b); break;
                default:
                    break;
            }
        }
		
		public static void PhysLava(Level lvl, ushort x, ushort y, ushort z, byte type) {
            if (x >= lvl.Width || y >= lvl.Height || z >= lvl.Length) return;
            if (Server.lava.active && Server.lava.map == lvl && Server.lava.InSafeZone(x, y, z))
                return;

            int b = x + lvl.Width * (z + y * lvl.Length);
            // only do expensive sponge check when necessary
            if (lvl.physics > 1 && lvl.physics != 5 &&
                ((lvl.blocks[b] >= Block.red && lvl.blocks[b] <= Block.white) ||
                 (lvl.blocks[b] >= Block.lightpink && lvl.blocks[b] <= Block.turquoise))
                && !lvl.CheckSpongeLava(x, y, z)) {
                lvl.AddUpdate(b, Block.air); return;
            }
            
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

                case Block.wood:
                case Block.shrub:
                case Block.trunk:
                case Block.leaf:
                case Block.yellowflower:
                case Block.redflower:
                case Block.mushroom:
                case Block.redmushroom:
                    if (lvl.physics > 1 && lvl.physics != 5) //Adv physics kills flowers and mushrooms plus wood in lava
                        if (!lvl.CheckSpongeLava(x, y, z)) lvl.AddUpdate(b, Block.air);
                    break;
            }
        }
    }
}
