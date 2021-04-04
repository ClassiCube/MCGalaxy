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

    public unsafe static class LeafPhysics {
        
        public static void DoLeaf(Level lvl, ref PhysInfo C) {
            // Decaying disabled? Then just remove from the physics list
            if (!lvl.Config.LeafDecay) {
                C.Data.Data = PhysicsArgs.RemoveFromChecks; return;
            }
            
            // Delay checking for leaf decay for a random amount of time
            if (C.Data.Data < 5) {
                Random rand = lvl.physRandom;
                if (rand.Next(10) == 0) C.Data.Data++;
                return;
            }
            
            // Perform actual leaf decay, then remove from physics list
            if (DoLeafDecay(lvl, ref C)) {
                lvl.AddUpdate(C.Index, Block.Air, default(PhysicsArgs));
                if (lvl.physics > 1) ActivateablePhysics.CheckNeighbours(lvl, C.X, C.Y, C.Z);
            }
            C.Data.Data = PhysicsArgs.RemoveFromChecks;
        }
        
        // radius of box around the given leaf block that is checked for logs
        const int range = 4;
        const int blocksPerAxis = range * 2 + 1;

        const int oneX = 1; // index + oneX = (X + 1, Y, Z)
        const int oneY = blocksPerAxis; // index + oneY = (X, Y + 1, Z)
        const int oneZ = blocksPerAxis * blocksPerAxis;

        static bool DoLeafDecay(Level lvl, ref PhysInfo C) {
            int* dists = stackalloc int[blocksPerAxis * blocksPerAxis * blocksPerAxis];
            ushort x = C.X, y = C.Y, z = C.Z;
            int idx  = 0;
            
            // The general overview of this algorithm is that it finds all log blocks
            //  from (x - range, y - range, z - range) to (x + range, y + range, z + range),
            //  and then tries to find a path from any of those logs to the block at (x, y, z).
            // Note that these paths can only travel through leaf blocks

            for (int xx = -range; xx <= range; xx++)
                for (int yy = -range; yy <= range; yy++)
                    for (int zz = -range; zz <= range; zz++, idx++)
            {
                int index = lvl.PosToInt((ushort)(x + xx), (ushort)(y + yy), (ushort)(z + zz));
                byte type = index < 0 ? Block.Air : lvl.blocks[index];
                
                if (type == Block.Log)
                    dists[idx] = 0;
                else if (type == Block.Leaves)
                    dists[idx] = -2;
                else
                    dists[idx] = -1;
            }

            for (int dist = 1; dist <= range; dist++) {
                idx = 0;
                
                for (int xx = -range; xx <= range; xx++)
                    for (int yy = -range; yy <= range; yy++)
                        for (int zz = -range; zz <= range; zz++, idx++)
                {
                    if (dists[idx] != dist - 1) continue;
                    // if this block is X-1 dist away from a log, potentially update neighbours as X blocks away from a log
                    
                    if (xx > -range) PropagateDist(dists, dist, idx - oneX);
                    if (xx <  range) PropagateDist(dists, dist, idx + oneX);
                    
                    if (yy > -range) PropagateDist(dists, dist, idx - oneY);
                    if (yy <  range) PropagateDist(dists, dist, idx + oneY);
                    
                    if (zz > -range) PropagateDist(dists, dist, idx - oneZ);
                    if (zz <  range) PropagateDist(dists, dist, idx + oneZ);
                }
            }
            
            // calculate index of (0, 0, 0)
            idx = range * oneX + range * oneY + range * oneZ;
            return dists[idx] < 0;
        }
        
        static void PropagateDist(int* dists, int dist, int index) {
            // distances can only propagate through leaf blocks
            if (dists[index] == -2) dists[index] = dist;
        }
        
        
        public static void DoLog(Level lvl, ref PhysInfo C) {
            ushort x = C.X, y = C.Y, z = C.Z;
            
            for (int xx = -range; xx <= range; xx++)
                for (int yy = -range; yy <= range; yy++)
                    for (int zz = -range; zz <= range; zz++)
            {
                int index = lvl.PosToInt((ushort)(x + xx), (ushort)(y + yy), (ushort)(z + zz));              
                if (index < 0 || lvl.blocks[index] != Block.Leaves) continue;
                
                lvl.AddCheck(index);
            }
            C.Data.Data = PhysicsArgs.RemoveFromChecks;
        }
    }
}
