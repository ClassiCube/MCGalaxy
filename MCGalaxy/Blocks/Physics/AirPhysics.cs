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
    
    public enum AirFlood { Full, Layer, Down, Up, }
    public static class AirPhysics {
        
        public static void DoAir(Level lvl, ref Check C) {
            ushort x, y, z;
            lvl.IntToPos(C.b, out x, out y, out z);
            
            PhysAir(lvl, lvl.PosToInt((ushort)(x + 1), y, z));
            PhysAir(lvl, lvl.PosToInt((ushort)(x - 1), y, z));
            PhysAir(lvl, lvl.PosToInt(x, y, (ushort)(z + 1)));
            PhysAir(lvl, lvl.PosToInt(x, y, (ushort)(z - 1)));
            PhysAir(lvl, lvl.PosToInt(x, (ushort)(y + 1), z));
            PhysAir(lvl, lvl.PosToInt(x, (ushort)(y - 1), z));

            //Edge of map water
            if (lvl.edgeWater && (y < lvl.Height / 2 && y >= (lvl.Height / 2) - 2)) {
                if (x == 0 || x == lvl.Width - 1 || z == 0 || z == lvl.Length - 1)
                    lvl.AddUpdate(C.b, Block.water);
            }
            if (!C.data.HasWait) C.data.Data = PhysicsArgs.RemoveFromChecks;
        }
        
        public static void DoFlood(Level lvl, ref Check C, AirFlood mode, byte block) {           
            if (C.data.Data >= 1) {
                lvl.AddUpdate(C.b, Block.air);
                C.data.Data = PhysicsArgs.RemoveFromChecks; return;
            }
            ushort x, y, z;
            lvl.IntToPos(C.b, out x, out y, out z);
            
            FloodAir(lvl, lvl.PosToInt((ushort)(x + 1), y, z), block);
            FloodAir(lvl, lvl.PosToInt((ushort)(x - 1), y, z), block);
            FloodAir(lvl, lvl.PosToInt(x, y, (ushort)(z + 1)), block);
            FloodAir(lvl, lvl.PosToInt(x, y, (ushort)(z - 1)), block);
            
            switch (mode) {
                case AirFlood.Full:
                    FloodAir(lvl, lvl.PosToInt(x, (ushort)(y - 1), z), block);
                    FloodAir(lvl, lvl.PosToInt(x, (ushort)(y + 1), z), block);
                    break;
                case AirFlood.Layer:
                    break;
                case AirFlood.Down:
                    FloodAir(lvl, lvl.PosToInt(x, (ushort)(y - 1), z), block);
                    break;
                case AirFlood.Up:
                    FloodAir(lvl, lvl.PosToInt(x, (ushort)(y + 1), z), block);
                    break;
            }
            C.data.Data++;
        }
        
        static void FloodAir(Level lvl, int index, byte block) {
            if (index == -1) return;
            byte curBlock = Block.Convert(lvl.blocks[index]);
            if (curBlock == Block.water || curBlock == Block.lava)
                lvl.AddUpdate(index, block);
        }
        
        internal static void PhysAir(Level lvl, int index) {
            if (index == -1) return;
            byte block = lvl.blocks[index];
            byte convBlock = Block.Convert(block);
            if (convBlock == Block.water || convBlock == Block.lava ||
                (block >= Block.red && block <= Block.white)) {
                lvl.AddCheck(index); return;
            }

            switch (block) {
                //case Block.water:
                //case Block.lava:
                case Block.shrub:
                case Block.sand:
                case Block.gravel:
                case Block.leaf:
                case Block.wood_float:
                    /*case Block.lava_fast:
                    case Block.WaterDown:
                    case Block.LavaDown:
                    case Block.deathlava:
                    case Block.deathwater:
                    case Block.geyser:
                    case Block.magma:*/
                    lvl.AddCheck(index);
                    break;
                default:
                    break;
            }
        }
    }
}
