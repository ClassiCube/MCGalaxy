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
using MCGalaxy.Maths;

namespace MCGalaxy.Blocks.Physics {

    internal static class PlayerPhysics {

        internal static void Fall(Player p, AABB bb) {
            bb.Min.Y -= 2; // test block below player feet
            Vec3S32 min = bb.BlockMin;
            if (min.Y == p.oldFallY) return;
            
            Vec3S32 max = bb.BlockMax;
            bool allGas = true;
            
            for (int z = min.Z; z <= max.Z; z++)
                for (int x = min.X; x <= max.X; x++)
            {
                ExtBlock block = GetSurvivalBlock(p, x, min.Y, z);
                byte collideType = p.level.CollideType(block);
                allGas = allGas && collideType == CollideType.WalkThrough;
                
                if (collideType != CollideType.Solid) continue;
                if (p.fallCount > p.level.Config.FallHeight)
                    p.HandleDeath(ExtBlock.Air, null, false, true);
                
                p.fallCount = 0;
                p.drownCount = 0;
                return;
            }
            
            if (!allGas) return;
            if (min.Y < p.oldFallY) p.fallCount++;
            else if (min.Y > p.oldFallY) p.fallCount = 0; // e.g. flying up
            
            p.oldFallY = min.Y;
            p.drownCount = 0;
        }
        
        static ExtBlock GetSurvivalBlock(Player p, int x, int y, int z) {
            if (y < 0) return (ExtBlock)Block.blackrock;
            if (y >= p.level.Height) return ExtBlock.Air;
            return p.level.GetBlock((ushort)x, (ushort)y, (ushort)z);
        }
    }
}