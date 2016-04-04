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
    
    public static class RocketPhysics {
        
        public static void Do(Level lvl, Check C) {
            Random rand = lvl.physRandom;			
            int dirX = rand.Next(1, 10) <= 5 ? 1 : -1;
            int dirY = rand.Next(1, 10) <= 5 ? 1 : -1;
            int dirZ = rand.Next(1, 10) <= 5 ? 1 : -1;
            ushort x, y, z;
            lvl.IntToPos(C.b, out x, out y, out z);

            for (int cx = -dirX; cx != 2 * dirX; cx += dirX)
                for (int cy = -dirY; cy != 2 * dirY; cy += dirY)
                    for (int cz = -dirZ; cz != 2 * dirZ; cz += dirZ)
            {                
                byte rocketTail = lvl.GetTile((ushort)(x + cx), (ushort)(y + cy), (ushort)(z + cz));
                if (rocketTail != Block.fire) continue;
                
                int headIndex = lvl.PosToInt((ushort)(x - cx), (ushort)(y - cy), (ushort)(z - cz));
                byte rocketHead = headIndex < 0 ? Block.Zero : lvl.blocks[headIndex];                
                bool unblocked = !lvl.listUpdateExists.Get(x, y, z) && 
                	(headIndex < 0 || !lvl.listUpdateExists.Get(x - cx, y - cy, z - cz));
                
                if (unblocked && (rocketHead == Block.air || rocketHead == Block.rocketstart)) {
                    lvl.AddUpdate(headIndex, Block.rockethead);
                    lvl.AddUpdate(C.b, Block.fire);
                } else if (rocketHead == Block.fire) {
                } else {
                    if (lvl.physics > 2)
                        lvl.MakeExplosion(x, y, z, 2);
                    else
                        lvl.AddUpdate(C.b, Block.fire);
                }
            }
        }
    }
}