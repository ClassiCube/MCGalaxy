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
        
        internal static void Walkthrough(Player p, AABB bb) {
            Vec3S32 min = bb.BlockMin, max = bb.BlockMax;
            bool hitWalkthrough = false;
            
            for (int y = min.Y; y <= max.Y; y++)
                for (int z = min.Z; z <= max.Z; z++)
                    for (int x = min.X; x <= max.X; x++)
            {
                ushort xP = (ushort)x, yP = (ushort)y, zP = (ushort)z;
                ExtBlock block = p.level.GetBlock(xP, yP, zP);
                if (block.BlockID == Block.Invalid) continue;
                
                AABB blockBB = p.level.blockAABBs[block.Index].Offset(x * 32, y * 32, z * 32);
                if (!bb.Intersects(blockBB)) continue;
                
                // We can activate only one walkthrough block per movement
                if (!hitWalkthrough) {
                    HandleWalkthrough handler = p.level.walkthroughHandlers[block.Index];
                    if (handler != null && handler(p, block, xP, yP, zP)) {
                        p.lastWalkthrough = p.level.PosToInt(xP, yP, zP);
                        hitWalkthrough = true;
                    }
                }
                
                // Some blocks will cause death of players
                if (!p.level.BlockProps[block.Index].KillerBlock) continue;               
                if (block.BlockID == Block.tntexplosion && p.PlayingTntWars) continue; // TODO: hardcoded behaviour is icky
                if (block.BlockID == Block.train && p.trainInvincible) continue;
                p.HandleDeath(block);
            }
            
            if (!hitWalkthrough) p.lastWalkthrough = -1;
        }
        
        internal static void Fall(Player p, AABB bb) {
            bb.Min.Y -= 2; // test block below player feet
            Vec3S32 min = bb.BlockMin, max = bb.BlockMax;
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
        
        internal static void Drown(Player p, AABB bb) {
            Vec3S32 P = bb.Max;
            ExtBlock bHead = GetSurvivalBlock(p, (ushort)P.X, (ushort)P.Y, (ushort)P.Z);
            if (bHead.IsPhysicsType) bHead.BlockID = Block.Convert(bHead.BlockID);
            
            switch (bHead.BlockID) {
                case Block.water:
                case Block.waterstill:
                case Block.lava:
                case Block.lavastill:
                    p.fallCount = 0;
                    p.drownCount++;
                    
                    // level drown is in 10ths of a second, and there are 100 ticks/second
                    if (p.drownCount > p.level.Config.DrownTime * 10) {
                        p.HandleDeath((ExtBlock)Block.water);
                        p.drownCount = 0;
                    }
                    break;
                default:
                    bool isGas = p.level.CollideType(bHead) == CollideType.WalkThrough;
                    if (!isGas) p.fallCount = 0;
                    p.drownCount = 0;
                    break;
            }
        }
        
        static ExtBlock GetSurvivalBlock(Player p, int x, int y, int z) {
            if (y < 0) return (ExtBlock)Block.blackrock;
            if (y >= p.level.Height) return ExtBlock.Air;
            return p.level.GetBlock((ushort)x, (ushort)y, (ushort)z);
        }
    }
}