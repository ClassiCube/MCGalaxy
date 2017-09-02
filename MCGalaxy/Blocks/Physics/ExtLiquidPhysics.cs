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
    
    public static class ExtLiquidPhysics {
        
        public static void DoMagma(Level lvl, ref Check C) {
            C.data.Data++;
            if (C.data.Data < 3) return;
            ushort x, y, z;
            lvl.IntToPos(C.b, out x, out y, out z);
            byte below = lvl.GetTile(x, (ushort)(y - 1), z);
            
            if (below == Block.Air) {
                lvl.AddUpdate(lvl.PosToInt(x, (ushort)(y - 1), z), Block.Magma);
            } else if (below != Block.Magma) {
                byte block = lvl.blocks[C.b];
                LiquidPhysics.PhysLava(lvl, (ushort)(x + 1), y, z, block);
                LiquidPhysics.PhysLava(lvl, (ushort)(x - 1), y, z, block);
                LiquidPhysics.PhysLava(lvl, x, y, (ushort)(z + 1), block);
                LiquidPhysics.PhysLava(lvl, x, y, (ushort)(z - 1), block);
            }

            if (lvl.physics <= 1 || C.data.Data <= 10) return;
            C.data.Data = 0;
            bool flowUp = false;
            
            MagmaFlow(lvl, x - 1, y, z, ref flowUp);
            MagmaFlow(lvl, x + 1, y, z, ref flowUp);
            MagmaFlow(lvl, x, y - 1, z, ref flowUp);
            MagmaFlow(lvl, x, y, z - 1, ref flowUp);
            MagmaFlow(lvl, x, y, z + 1, ref flowUp);
            if (flowUp)
                MagmaFlow(lvl, x, y + 1, z, ref flowUp);
        }
        
        static void MagmaFlow(Level lvl, int x, int y, int z, ref bool flowUp) {
            int index;
            ExtBlock block = lvl.GetBlock((ushort)x, (ushort)y, (ushort)z, out index);
            
            if (index >= 0 && lvl.Props[block.Index].LavaKills) {
                lvl.AddUpdate(index, Block.Magma);
                flowUp = true;
            }
        }
        
        public static void DoGeyser(Level lvl, ref Check C) {
            C.data.Data++;
            ushort x, y, z;
            lvl.IntToPos(C.b, out x, out y, out z);
            byte below = lvl.GetTile(x, (ushort)(y - 1), z);
            
            if (below == Block.Air) {
                lvl.AddUpdate(lvl.PosToInt(x, (ushort)(y - 1), z), Block.Geyser);
            } else if (below != Block.Geyser) {
                byte block = lvl.blocks[C.b];
                LiquidPhysics.PhysWater(lvl, (ushort)(x + 1), y, z, block);
                LiquidPhysics.PhysWater(lvl, (ushort)(x - 1), y, z, block);
                LiquidPhysics.PhysWater(lvl, x, y, (ushort)(z + 1), block);
                LiquidPhysics.PhysWater(lvl, x, y, (ushort)(z - 1), block);
            }

            if (lvl.physics <= 1 || C.data.Data <= 10) return;
            C.data.Data = 0;
            bool flowUp = false;
            
            GeyserFlow(lvl, x - 1, y, z, ref flowUp);
            GeyserFlow(lvl, x + 1, y, z, ref flowUp);
            GeyserFlow(lvl, x, y - 1, z, ref flowUp);
            GeyserFlow(lvl, x, y, z - 1, ref flowUp);
            GeyserFlow(lvl, x, y, z + 1, ref flowUp);
            if (flowUp)
                GeyserFlow(lvl, x, y + 1, z, ref flowUp);
        }
        
        static void GeyserFlow(Level lvl, int x, int y, int z, ref bool flowUp) {
            int index;
            ExtBlock block = lvl.GetBlock((ushort)x, (ushort)y, (ushort)z, out index);
            
            if (index >= 0 && lvl.Props[block.Index].WaterKills) {
                lvl.AddUpdate(index, Block.Geyser);
                flowUp = true;
            }
        }
        
        public static void DoWaterfall(Level lvl, ref Check C) {
            ushort x, y, z;
            lvl.IntToPos(C.b, out x, out y, out z);
            byte below = lvl.GetTile(x, (ushort)(y - 1), z);
            
            switch (below) {
                case Block.Air:
                    lvl.AddUpdate(lvl.PosToInt(x, (ushort)(y - 1), z), Block.WaterDown);
                    if (!C.data.HasWait) C.data.Data = PhysicsArgs.RemoveFromChecks;
                    break;
                case Block.Air_FloodDown:
                case Block.StillLava:
                case Block.StillWater:
                case Block.WaterDown:
                    break;
                    
                default:
                    byte block = lvl.blocks[C.b];
                    LiquidPhysics.PhysWater(lvl, (ushort)(x + 1), y, z, block);
                    LiquidPhysics.PhysWater(lvl, (ushort)(x - 1), y, z, block);
                    LiquidPhysics.PhysWater(lvl, x, y, (ushort)(z + 1),block);
                    LiquidPhysics.PhysWater(lvl, x, y, (ushort)(z - 1), block);
                    if (!C.data.HasWait) C.data.Data = PhysicsArgs.RemoveFromChecks;
                    break;
            }
        }
        
        public static void DoLavafall(Level lvl, ref Check C) {
            ushort x, y, z;
            lvl.IntToPos(C.b, out x, out y, out z);
            byte below = lvl.GetTile(x, (ushort)(y - 1), z);
            
            switch (below)
            {
                case Block.Air:
                    lvl.AddUpdate(lvl.PosToInt(x, (ushort)(y - 1), z), Block.LavaDown);
                    if (!C.data.HasWait) C.data.Data = PhysicsArgs.RemoveFromChecks;
                    break;
                case Block.Air_FloodDown:
                case Block.StillLava:
                case Block.StillWater:
                case Block.LavaDown:
                    break;
                default:
                    byte block = lvl.blocks[C.b];
                    LiquidPhysics.PhysLava(lvl, (ushort)(x + 1), y, z, block);
                    LiquidPhysics.PhysLava(lvl, (ushort)(x - 1), y, z, block);
                    LiquidPhysics.PhysLava(lvl, x, y, (ushort)(z + 1),block);
                    LiquidPhysics.PhysLava(lvl, x, y, (ushort)(z - 1), block);
                    if (!C.data.HasWait) C.data.Data = PhysicsArgs.RemoveFromChecks;
                    break;
            }
        }
        
        public static void DoFaucet(Level lvl, ref Check C, byte target) {
            C.data.Data++;
            if (C.data.Data < 2) return;
            C.data.Data = 0;
            
            ushort x, y, z;
            lvl.IntToPos(C.b, out x, out y, out z);
            int index = lvl.PosToInt(x, (ushort)(y - 1), z);
            if (index < 0) return;

            Random rand = lvl.physRandom;            
            byte block = lvl.blocks[index];
            if (block == Block.Air || block == target) {
                if (rand.Next(1, 10) > 7)
                    lvl.AddUpdate(index, Block.Air_FloodDown);
            } else if (block == Block.Air_FloodDown) {
                if (rand.Next(1, 10) > 4)
                    lvl.AddUpdate(index, target);
            }
        }
    }
}
