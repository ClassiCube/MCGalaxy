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
            
            ActivateablePhysics.CheckNeighbours(lvl, x, y, z);
            ActivateablePhysics.CheckAt(lvl, lvl.PosToInt(x, (ushort)(y - 1), z)); 

            //Edge of map water
            if (lvl.Config.EdgeWater && (y < lvl.Config.EdgeLevel && y >= (lvl.Config.EdgeLevel + lvl.Config.SidesOffset))) {
                if (x == 0 || x == lvl.Width - 1 || z == 0 || z == lvl.Length - 1) {
                    ExtBlock block = ExtBlock.FromRaw((byte)lvl.Config.HorizonBlock);
                    PhysicsArgs args = default(PhysicsArgs);
                    args.ExtBlock = block.BlockID == Block.custom_block;
                    lvl.AddUpdate(C.b, block.RawID, false, args);
                }
            }
            if (!C.data.HasWait) C.data.Data = PhysicsArgs.RemoveFromChecks;
        }
        
        public static void DoFlood(Level lvl, ref Check C, AirFlood mode, byte block) {           
            if (C.data.Data >= 1) {
                lvl.AddUpdate(C.b, Block.Air);
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
            if (curBlock == Block.Water || curBlock == Block.Lava)
                lvl.AddUpdate(index, block);
        }
    }
}
