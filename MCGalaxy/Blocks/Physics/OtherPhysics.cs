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
using MCGalaxy.Generator.Foliage;
using BlockID = System.UInt16;

namespace MCGalaxy.Blocks.Physics {

    public static class OtherPhysics {
        
        public static void DoFalling(Level lvl, ref Check C) {
            ushort x, y, z;
            lvl.IntToPos(C.b, out x, out y, out z);
            int index = C.b;
            bool movedDown = false;
            byte block = lvl.blocks[C.b];
            ushort yCur = y;
            
            do {
                index = lvl.IntOffset(index, 0, -1, 0); yCur--;// Get block below each loop
                BlockID cur = lvl.GetBlock(x, yCur, z);
                if (cur == Block.Invalid) break;
                bool hitBlock = false;
                
                switch (cur) {
                    case Block.Air:
                    case Block.Water:
                    case Block.Lava:
                        movedDown = true;
                        break;
                        //Adv physics crushes plants with sand
                    case Block.Sapling:
                    case Block.Dandelion:
                    case Block.Rose:
                    case Block.Mushroom:
                    case Block.RedMushroom:
                        if (lvl.physics > 1) movedDown = true;
                        break;
                    default:
                        hitBlock = true;
                        break;
                }
                if (hitBlock || lvl.physics > 1) break;
            } while (true);

            if (movedDown) {
                lvl.AddUpdate(C.b, Block.Air, default(PhysicsArgs));
                if (lvl.physics > 1)
                    lvl.AddUpdate(index, block);
                else
                    lvl.AddUpdate(lvl.IntOffset(index, 0, 1, 0), block);
                
                ActivateablePhysics.CheckNeighbours(lvl, x, y, z);
            }
            C.data.Data = PhysicsArgs.RemoveFromChecks;
        }
        
        public static void DoFloatwood(Level lvl, ref Check C) {
            ushort x, y, z;
            lvl.IntToPos(C.b, out x, out y, out z);
            int index;
            
            if (lvl.GetBlock(x, (ushort)(y - 1), z, out index) == Block.Air) {
                lvl.AddUpdate(C.b, Block.Air, default(PhysicsArgs));
                lvl.AddUpdate(index, Block.FloatWood, default(PhysicsArgs));
            } else {
                BlockID above = lvl.GetBlock(x, (ushort)(y + 1), z, out index);
                if (above == Block.StillWater || Block.Convert(above) == Block.Water) {
                    lvl.AddUpdate(C.b, lvl.blocks[index]);
                    lvl.AddUpdate(index, Block.FloatWood, default(PhysicsArgs));
                }
            }
            C.data.Data = PhysicsArgs.RemoveFromChecks;
        }
        
        public static void DoShrub(Level lvl, ref Check C) {
            Random rand = lvl.physRandom;            
            ushort x, y, z;
            lvl.IntToPos(C.b, out x, out y, out z);
            if (lvl.physics > 1) { //Adv physics kills flowers and mushroos in water/lava
                ActivateablePhysics.CheckNeighbours(lvl, x, y, z);
            }

            if (!lvl.Config.GrowTrees) { C.data.Data = PhysicsArgs.RemoveFromChecks; return; }
            if (C.data.Data < 20) {
                if (rand.Next(20) == 0) C.data.Data++;
                return;
            }
            
            lvl.SetTile(x, y, z, Block.Air);        
            Tree tree = Tree.Find(lvl.Config.TreeType);
            if (tree == null) tree = new NormalTree();
            
            tree.SetData(rand, tree.DefaultSize(rand));
            tree.Generate(x, y, z, (xT, yT, zT, bT) =>
                        {
                            if (!lvl.IsAirAt(xT, yT, zT)) return;
                            lvl.Blockchange(xT, yT, zT, (ushort)bT);
                        });
            
            C.data.Data = PhysicsArgs.RemoveFromChecks;
        }
        
        public static void DoDirtGrow(Level lvl, ref Check C) {
            if (!lvl.Config.GrassGrow) { C.data.Data = PhysicsArgs.RemoveFromChecks; return; }
            ushort x, y, z;
            lvl.IntToPos(C.b, out x, out y, out z);
            
            if (C.data.Data > 20) {                
                BlockID above = lvl.GetBlock(x, (ushort)(y + 1), z);
                if (lvl.LightPasses(above)) {
                    BlockID block = lvl.GetBlock(x, y, z);
                    BlockID grass = lvl.Props[block].GrassBlock;
                    lvl.AddUpdate(C.b, grass);
                }
                C.data.Data = PhysicsArgs.RemoveFromChecks;
            } else {
                C.data.Data++;
            }
        }
        
        public static void DoGrassDie(Level lvl, ref Check C) {
            if (!lvl.Config.GrassGrow) { C.data.Data = PhysicsArgs.RemoveFromChecks; return; }
            ushort x, y, z;
            lvl.IntToPos(C.b, out x, out y, out z);
            
            if (C.data.Data > 20) {
                BlockID above = lvl.GetBlock(x, (ushort)(y + 1), z);
                if (!lvl.LightPasses(above)) {
                    BlockID block = lvl.GetBlock(x, y, z);
                    BlockID dirt = lvl.Props[block].DirtBlock;
                    lvl.AddUpdate(C.b, dirt);
                }
                C.data.Data = PhysicsArgs.RemoveFromChecks;
            } else {
                C.data.Data++;
            }
        }
        
        
        public static void DoSponge(Level lvl, ref Check C, bool lava) {
            byte target = lava ? Block.Lava : Block.Water;
            byte alt    = lava ? Block.StillLava : Block.StillWater;
            ushort x, y, z;
            lvl.IntToPos(C.b, out x, out y, out z);
            
            for (int yy = y - 2; yy <= y + 2; ++yy)
                for (int zz = z - 2; zz <= z + 2; ++zz)
                    for (int xx = x - 2; xx <= x + 2; ++xx)
            {
                int index;
                BlockID block = lvl.GetBlock((ushort)xx, (ushort)yy, (ushort)zz, out index);
                if (block == Block.Invalid) continue;
                
                if (Block.Convert(block) == target || Block.Convert(block) == alt) {
                    lvl.AddUpdate(index, Block.Air, default(PhysicsArgs));
                }
            }
            C.data.Data = PhysicsArgs.RemoveFromChecks;
        }
        
        public static void DoSpongeRemoved(Level lvl, int b, bool lava) {
            byte target = lava ? Block.Lava : Block.Water;
            byte alt    = lava ? Block.StillLava : Block.StillWater;
            ushort x, y, z;
            lvl.IntToPos(b, out x, out y, out z);
            
            for (int yy = -3; yy <= +3; ++yy)
                for (int zz = -3; zz <= +3; ++zz)
                    for (int xx = -3; xx <= +3; ++xx)
            {
                if (Math.Abs(xx) == 3 || Math.Abs(yy) == 3 || Math.Abs(zz) == 3) { // Calc only edge
                    int index;
                    BlockID block = lvl.GetBlock((ushort)(x + xx), (ushort)(y + yy), (ushort)(z + zz), out index);
                    if (block == Block.Invalid) continue;
                    
                    if (Block.Convert(block) == target || Block.Convert(block) == alt)
                        lvl.AddCheck(index);
                }
            }
        }
        
        public static void DoOther(Level lvl, ref Check C) {
            if (lvl.physics <= 1) { C.data.Data = PhysicsArgs.RemoveFromChecks; return; }
            ushort x, y, z;
            lvl.IntToPos(C.b, out x, out y, out z);
            
            //Adv physics kills flowers and mushroos in water/lava
            ActivateablePhysics.CheckNeighbours(lvl, x, y, z);
            C.data.Data = PhysicsArgs.RemoveFromChecks;
        }
    }
}
