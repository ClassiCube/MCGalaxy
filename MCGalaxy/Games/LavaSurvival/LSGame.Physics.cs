/*
    Copyright 2011 MCForge
        
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
using MCGalaxy.Blocks.Physics;
using BlockID = System.UInt16;

namespace MCGalaxy.Games 
{
    public sealed partial class LSGame : RoundsGame 
    {
        void UpdateBlockHandlers() {
            Map.UpdateBlockHandlers(Block.Water);
            Map.UpdateBlockHandlers(Block.Deadly_ActiveWater);
            Map.UpdateBlockHandlers(Block.Lava);
            Map.UpdateBlockHandlers(Block.Deadly_ActiveLava);
        }

        void HandleBlockHandlersUpdated(Level lvl, BlockID block) {
            if (!Running || lvl != Map) return;

            switch (block)
            {
                case Block.Water:
                case Block.Deadly_ActiveWater:
                    lvl.PhysicsHandlers[block] = DoWater; break;
                case Block.Lava:
                case Block.Deadly_ActiveLava:
                    lvl.PhysicsHandlers[block] = DoLava; break;
            }
        }

        void DoWater(Level lvl, ref PhysInfo C) {
            ushort x = C.X, y = C.Y, z = C.Z;
            
            if (!lvl.CheckSpongeWater(x, y, z)) {
                BlockID block = C.Block;
                
                LiquidPhysics.PhysWater(lvl, (ushort)(x + 1), y, z, block);
                LiquidPhysics.PhysWater(lvl, (ushort)(x - 1), y, z, block);
                LiquidPhysics.PhysWater(lvl, x, y, (ushort)(z + 1), block);
                LiquidPhysics.PhysWater(lvl, x, y, (ushort)(z - 1), block);
                LiquidPhysics.PhysWater(lvl, x, (ushort)(y - 1), z, block);

                if (floodUp) LiquidPhysics.PhysWater(lvl, x, (ushort)(y + 1), z, block);
            } else { //was placed near sponge
                lvl.AddUpdate(C.Index, Block.Air, default(PhysicsArgs));
            }
            C.Data.Data = PhysicsArgs.RemoveFromChecks;
        }
        
        void DoLava(Level lvl, ref PhysInfo C) {
            ushort x = C.X, y = C.Y, z = C.Z;

            if (C.Data.Data < spreadDelay) {
                C.Data.Data++; return;
            }
            
            if (!lvl.CheckSpongeLava(x, y, z)) {
                BlockID block = C.Block;
                LiquidPhysics.PhysLava(lvl, (ushort)(x + 1), y, z, block);
                LiquidPhysics.PhysLava(lvl, (ushort)(x - 1), y, z, block);
                LiquidPhysics.PhysLava(lvl, x, y, (ushort)(z + 1), block);
                LiquidPhysics.PhysLava(lvl, x, y, (ushort)(z - 1), block);
                LiquidPhysics.PhysLava(lvl, x, (ushort)(y - 1), z, block);

                if (floodUp) LiquidPhysics.PhysLava(lvl, x, (ushort)(y + 1), z, block);
            } else { //was placed near sponge
                lvl.AddUpdate(C.Index, Block.Air, default(PhysicsArgs));
            }
            C.Data.Data = PhysicsArgs.RemoveFromChecks;
        }
    }
}
