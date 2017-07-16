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
    public static class BirdPhysics {
        
        public static void Do(Level lvl, ref Check C) {
            Random rand = lvl.physRandom;            
            ushort x, y, z;
            lvl.IntToPos(C.b, out x, out y, out z);

            switch (rand.Next(1, 15)) {
                case 1:
                    if (lvl.IsAirAt(x, (ushort)(y - 1), z))
                        lvl.AddUpdate(lvl.PosToInt(x, (ushort)(y - 1), z), lvl.blocks[C.b]);
                    else goto case 3;
                    break;
                case 2:
                    if (lvl.IsAirAt(x, (ushort)(y + 1), z))
                        lvl.AddUpdate(lvl.PosToInt(x, (ushort)(y + 1), z), lvl.blocks[C.b]);
                    else goto case 6;
                    break;
                case 3:
                case 4:
                case 5:
                    FlyTo(lvl, ref C, x - 1, y, z);
                    break;
                case 6:
                case 7:
                case 8:
                    FlyTo(lvl, ref C, x + 1, y, z);
                    break;
                case 9:
                case 10:
                case 11:
                    FlyTo(lvl, ref C, x, y, z - 1);
                    break;
                default:
                    FlyTo(lvl, ref C, x, y, z + 1);
                    break;
            }
            lvl.AddUpdate(C.b, Block.Air);
            C.data.Data = PhysicsArgs.RemoveFromChecks;
        }
        
        static void FlyTo(Level lvl, ref Check C, int x, int y, int z) {
            int index = lvl.PosToInt((ushort)x, (ushort)y, (ushort)z);
            if (index < 0) return;
            
            switch (lvl.blocks[index]) {
                case Block.Air:
                    lvl.AddUpdate(index, lvl.blocks[C.b]);
                    break;
                case Block.Op_Air:
                    break;
                default:
                    PhysicsArgs args = default(PhysicsArgs);
                    args.Type1 = PhysicsArgs.Dissipate; args.Value1 = 25;
                    lvl.AddUpdate(C.b, Block.Red, false, args);
                    break;
            }
        }
    }
}
