/*
    Copyright 2015 MCGalaxy
        
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

namespace MCGalaxy.BlockPhysics {
    
    public static class ExtLiquidPhysics {
        
        public static void DoMagma(Level lvl, Check C, Random rand) {
            C.time++;
            if (C.time < 3) return;
            ushort x, y, z;
            lvl.IntToPos(C.b, out x, out y, out z);
            byte below = lvl.GetTile(x, (ushort)(y - 1), z);
            
            if (below == Block.air) {
                lvl.AddUpdate(lvl.PosToInt(x, (ushort)(y - 1), z), Block.magma);
            } else if (below != Block.magma) {
                byte block = lvl.blocks[C.b];
                lvl.PhysLava(lvl.PosToInt((ushort)(x + 1), y, z), block);
                lvl.PhysLava(lvl.PosToInt((ushort)(x - 1), y, z), block);
                lvl.PhysLava(lvl.PosToInt(x, y, (ushort)(z + 1)), block);
                lvl.PhysLava(lvl.PosToInt(x, y, (ushort)(z - 1)), block);
            }

            if (lvl.physics <= 1 || C.time <= 10) return;
            C.time = 0;
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
            int index = lvl.PosToInt((ushort)x, (ushort)y, (ushort)z);
            if (index >= 0 && Block.LavaKill(lvl.blocks[index])) {
                lvl.AddUpdate(index, Block.magma);
                flowUp = true;
            }
        }
        
        public static void DoGeyser(Level lvl, Check C, Random rand) {
            C.time++;
            ushort x, y, z;
            lvl.IntToPos(C.b, out x, out y, out z);
            byte below = lvl.GetTile(x, (ushort)(y - 1), z);
            
            if (below == Block.air) {
                lvl.AddUpdate(lvl.PosToInt(x, (ushort)(y - 1), z), Block.geyser);
            } else if (below != Block.geyser) {
                byte block = lvl.blocks[C.b];
                lvl.PhysWater(lvl.PosToInt((ushort)(x + 1), y, z), block);
                lvl.PhysWater(lvl.PosToInt((ushort)(x - 1), y, z), block);
                lvl.PhysWater(lvl.PosToInt(x, y, (ushort)(z + 1)), block);
                lvl.PhysWater(lvl.PosToInt(x, y, (ushort)(z - 1)), block);
            }

            if (lvl.physics <= 1 || C.time <= 10) return;
            C.time = 0;
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
            int index = lvl.PosToInt((ushort)x, (ushort)y, (ushort)z);
            if (index >= 0 && Block.WaterKill(lvl.blocks[index])) {
                lvl.AddUpdate(index, Block.geyser);
                flowUp = true;
            }
        }
        
        public static void DoWaterfall(Level lvl, Check C, Random rand) {
            ushort x, y, z;
            lvl.IntToPos(C.b, out x, out y, out z);
            byte below = lvl.GetTile(x, (ushort)(y - 1), z);
            
            switch (below)
            {
                case Block.air:
                    lvl.AddUpdate(lvl.PosToInt(x, (ushort)(y - 1), z), Block.WaterDown);
                    if (C.extraInfo.IndexOf("wait") == -1)
                        C.time = 255;
                    break;
                case Block.air_flood_down:
                case Block.lavastill:
                case Block.waterstill:
                case Block.WaterDown:
                    break;
                default:
                    byte block = lvl.blocks[C.b];
                    lvl.PhysWater(lvl.PosToInt((ushort)(x + 1), y, z), block);
                    lvl.PhysWater(lvl.PosToInt((ushort)(x - 1), y, z), block);
                    lvl.PhysWater(lvl.PosToInt(x, y, (ushort)(z + 1)),block);
                    lvl.PhysWater(lvl.PosToInt(x, y, (ushort)(z - 1)), block);
                    if (C.extraInfo.IndexOf("wait") == -1)
                        C.time = 255;
                    break;
            }
        }
        
        public static void DoLavafall(Level lvl, Check C, Random rand) {
            ushort x, y, z;
            lvl.IntToPos(C.b, out x, out y, out z);
            byte below = lvl.GetTile(x, (ushort)(y - 1), z);
            
            switch (below)
            {
                case Block.air:
                    lvl.AddUpdate(lvl.PosToInt(x, (ushort)(y - 1), z), Block.LavaDown);
                    if (C.extraInfo.IndexOf("wait") == -1)
                        C.time = 255;
                    break;
                case Block.air_flood_down:
                case Block.lavastill:
                case Block.waterstill:
                case Block.LavaDown:
                    break;
                default:
                    byte block = lvl.blocks[C.b];
                    lvl.PhysLava(lvl.PosToInt((ushort)(x + 1), y, z), block);
                    lvl.PhysLava(lvl.PosToInt((ushort)(x - 1), y, z), block);
                    lvl.PhysLava(lvl.PosToInt(x, y, (ushort)(z + 1)),block);
                    lvl.PhysLava(lvl.PosToInt(x, y, (ushort)(z - 1)), block);
                    if (C.extraInfo.IndexOf("wait") == -1)
                        C.time = 255;
                    break;
            }
        }
        
        public static void DoFaucet(Level lvl, Check C, Random rand, byte target) {
            C.time++;
            if (C.time < 2) return;
            C.time = 0;
            
            ushort x, y, z;
            lvl.IntToPos(C.b, out x, out y, out z);
            int index = lvl.PosToInt(x, (ushort)(y - 1), z);
            if (index < 0) return;
            
            byte block = lvl.blocks[index];
            if (block == Block.air || block == target) {
                if (rand.Next(1, 10) > 7)
                    lvl.AddUpdate(index, Block.air_flood_down);
            } else if (block == Block.air_flood_down) {
                if (rand.Next(1, 10) > 4)
                    lvl.AddUpdate(index, target);
            }
        }
    }
}
