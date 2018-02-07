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
using BlockID = System.UInt16;

namespace MCGalaxy.Blocks.Physics {
    
    public static class RocketPhysics {
        
        public static void Do(Level lvl, ref PhysInfo C) {
            Random rand = lvl.physRandom;            
            int dirX = rand.Next(1, 10) <= 5 ? 1 : -1;
            int dirY = rand.Next(1, 10) <= 5 ? 1 : -1;
            int dirZ = rand.Next(1, 10) <= 5 ? 1 : -1;
            ushort x = C.X, y = C.Y, z = C.Z;

            for (int cx = -dirX; cx != 2 * dirX; cx += dirX)
                for (int cy = -dirY; cy != 2 * dirY; cy += dirY)
                    for (int cz = -dirZ; cz != 2 * dirZ; cz += dirZ)
            {                
                BlockID rocketTail = lvl.GetBlock((ushort)(x + cx), (ushort)(y + cy), (ushort)(z + cz));
                if (rocketTail != Block.LavaFire) continue;
                
                int headIndex;
                BlockID rocketHead = lvl.GetBlock((ushort)(x - cx), (ushort)(y - cy), (ushort)(z - cz), out headIndex);
                bool unblocked = !lvl.listUpdateExists.Get(x, y, z) && (headIndex < 0 || !lvl.listUpdateExists.Get(x - cx, y - cy, z - cz));
                
                if (unblocked && (rocketHead == Block.Air || rocketHead == Block.RocketStart)) {
                    lvl.AddUpdate(headIndex, Block.RocketHead, default(PhysicsArgs));
                    lvl.AddUpdate(C.Index, Block.LavaFire, default(PhysicsArgs));
                } else if (rocketHead == Block.LavaFire) {
                } else {
                    if (lvl.physics > 2)
                        lvl.MakeExplosion(x, y, z, 2);
                    else
                        lvl.AddUpdate(C.Index, Block.LavaFire, default(PhysicsArgs));
                }
            }
        }
    }
}