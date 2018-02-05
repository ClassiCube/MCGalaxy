/*
    Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCGalaxy)
        
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
using BlockID = System.UInt16;

namespace MCGalaxy.Blocks.Physics {
    
    public static class TrainPhysics {
        
        public static void Do(Level lvl, ref Check C) {
            Random rand = lvl.physRandom;
            int dirX = rand.Next(1, 10) <= 5 ? 1 : -1;
            int dirY = rand.Next(1, 10) <= 5 ? 1 : -1;
            int dirZ = rand.Next(1, 10) <= 5 ? 1 : -1;
            ushort x, y, z;
            lvl.IntToPos(C.b, out x, out y, out z);

            for (int dx = -dirX; dx != 2 * dirX; dx += dirX)
                for (int dy = -dirY; dy != 2 * dirY; dy += dirY)
                    for (int dz = -dirZ; dz != 2 * dirZ; dz += dirZ)
            {
                BlockID below = lvl.GetBlock((ushort)(x + dx), (ushort)(y + dy - 1), (ushort)(z + dz));
                BlockID block = lvl.GetBlock((ushort)(x + dx), (ushort)(y + dy),     (ushort)(z + dz));
                bool isRails = lvl.Props[below].IsRails;
                
                if (isRails && (block == Block.Air || block == Block.Water) 
                    && !lvl.listUpdateExists.Get(x + dx, y + dy, z + dz)) {
                    lvl.AddUpdate(lvl.PosToInt((ushort)(x + dx), (ushort)(y + dy), (ushort)(z + dz)), Block.Train);
                    lvl.AddUpdate(C.b, Block.Air, default(PhysicsArgs));                    
                    byte newBlock = below == Block.Op_Air ? Block.Glass : Block.Obsidian;
                    
                    below = lvl.GetBlock(x, (ushort)(y - 1), z);
                    PhysicsArgs args = default(PhysicsArgs);
                    args.Type1 = PhysicsArgs.Wait; args.Value1 = 5;
                    args.Type2 = PhysicsArgs.Revert; args.Value2 = (byte)below;
                    args.ExtBlock = below >= Block.Extended;
                    
                    lvl.AddUpdate(lvl.IntOffset(C.b, 0, -1, 0), newBlock, args, true);
                    return;
                }
            }
        }
    }
}
