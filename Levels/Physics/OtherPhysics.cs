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
using MCGalaxy.Drawing;
using MCGalaxy.Drawing.Ops;

namespace MCGalaxy.BlockPhysics {

    public static class OtherPhysics {
        
        public static void DoFalling(Level lvl, ref Check C) {
            if (lvl.physics == 0 || lvl.physics == 5) { C.data.Data = 255; return; }
            ushort x, y, z;
            lvl.IntToPos(C.b, out x, out y, out z);
            int index = C.b;
            bool movedDown = false;
            byte block = lvl.blocks[C.b];
            
            do {
                index = lvl.IntOffset(index, 0, -1, 0); //Get block below each loop
                if (lvl.GetTile(index) == Block.Zero) break;
                bool hitBlock = false;
                
                switch (lvl.blocks[index]) {
                    case Block.air:
                    case Block.water:
                    case Block.lava:
                        movedDown = true;
                        break;
                        //Adv physics crushes plants with sand
                    case Block.shrub:
                    case Block.yellowflower:
                    case Block.redflower:
                    case Block.mushroom:
                    case Block.redmushroom:
                        if (lvl.physics > 1) movedDown = true;
                        break;
                    default:
                        hitBlock = true;
                        break;
                }
                if (hitBlock || lvl.physics > 1) break;
            } while (true);

            if (movedDown) {
                lvl.AddUpdate(C.b, Block.air);
                if (lvl.physics > 1)
                    lvl.AddUpdate(index, block);
                else
                    lvl.AddUpdate(lvl.IntOffset(index, 0, 1, 0), block);
                
                AirPhysics.PhysAir(lvl, lvl.PosToInt((ushort)(x + 1), y, z));
                AirPhysics.PhysAir(lvl, lvl.PosToInt((ushort)(x - 1), y, z));
                AirPhysics.PhysAir(lvl, lvl.PosToInt(x, y, (ushort)(z + 1)));
                AirPhysics.PhysAir(lvl, lvl.PosToInt(x, y, (ushort)(z - 1)));
                AirPhysics.PhysAir(lvl, lvl.PosToInt(x, (ushort)(y + 1), z));
            }
            C.data.Data = 255;
        }

        public static void DoStairs(Level lvl, ref Check C) {
            int bBelow = lvl.IntOffset(C.b, 0, -1, 0);
            byte tile = lvl.GetTile(bBelow);
            
            if (tile == Block.staircasestep) {
                lvl.AddUpdate(C.b, Block.air);
                lvl.AddUpdate(bBelow, Block.staircasefull);
            } else if (tile == Block.cobblestoneslab) {
                lvl.AddUpdate(C.b, Block.air);
                lvl.AddUpdate(bBelow, Block.stone);
            }
            C.data.Data = 255;
        }
        
        public static void DoFloatwood(Level lvl, ref Check C) {
            int index = lvl.IntOffset(C.b, 0, -1, 0);
            if (lvl.GetTile(index) == Block.air) {
                lvl.AddUpdate(C.b, Block.air);
                lvl.AddUpdate(index, Block.wood_float);
            } else {
                index = lvl.IntOffset(C.b, 0, 1, 0);
                if (Block.Convert(lvl.GetTile(index)) == Block.water) {
                    lvl.AddUpdate(C.b, lvl.blocks[index]);
                    lvl.AddUpdate(index, Block.wood_float);
                }
            }
            C.data.Data = 255;
        }
        
        public static void DoShrub(Level lvl, ref Check C) {
            Random rand = lvl.physRandom;			
            ushort x, y, z;
            lvl.IntToPos(C.b, out x, out y, out z);
            if (lvl.physics > 1) { //Adv physics kills flowers and mushroos in water/lava
                AirPhysics.PhysAir(lvl, lvl.PosToInt((ushort)(x + 1), y, z));
                AirPhysics.PhysAir(lvl, lvl.PosToInt((ushort)(x - 1), y, z));
                AirPhysics.PhysAir(lvl, lvl.PosToInt(x, y, (ushort)(z + 1)));
                AirPhysics.PhysAir(lvl, lvl.PosToInt(x, y, (ushort)(z - 1)));
                AirPhysics.PhysAir(lvl, lvl.PosToInt(x, (ushort)(y + 1), z));
            }

            if (!lvl.growTrees) { C.data.Data = 255; return; }
            if (C.data.Data < 20) {
                if (rand.Next(20) == 0) C.data.Data++;
                return;
            }
            
            TreeDrawOp op = new TreeDrawOp();
            op.Level = lvl;
            op.random = rand;
            Vec3S32[] marks = new [] { new Vec3S32(x, y, z) };
            op.SetMarks(marks);
            
            foreach (var block in op.Perform(marks, null, lvl, null)) {
                lvl.Blockchange(block.X, block.Y, block.Z, block.Block);
            }
            C.data.Data = 255;
        }
        
        public static void DoDirt(Level lvl, ref Check C) {
            if (!lvl.GrassGrow) { C.data.Data = 255; return; }
            ushort x, y, z;
            lvl.IntToPos(C.b, out x, out y, out z);
            
            if (C.data.Data > 20) {
                byte block = lvl.GetTile(x, (ushort)(y + 1), z), extBlock = 0;
                if (block == Block.custom_block)
                    extBlock = lvl.GetExtTile(x, (ushort)(y + 1), z);
                
                if (Block.LightPass(block, extBlock, lvl.CustomBlockDefs))
                    lvl.AddUpdate(C.b, Block.grass);
                C.data.Data = 255;
            } else {
                C.data.Data++;
            }
        }
		
        public static void DoSponge(Level lvl, ref Check C, bool lava) {
            for (int y = -2; y <= +2; ++y)
                for (int z = -2; z <= +2; ++z)
                    for (int x = -2; x <= +2; ++x)
            {
                int index = lvl.IntOffset(C.b, x, y, z);
                byte block = lvl.GetTile(index);
                if (block == Block.Zero) continue;
                
                if ((!lava && Block.Convert(block) == Block.water) || (lava && Block.Convert(block) == Block.lava))
                    lvl.AddUpdate(index, Block.air);
            }
            C.data.Data = 255;
        }
		
		public static void DoSpongeRemoved(Level lvl, int b, bool lava = false) {
            for (int y = -3; y <= +3; ++y)
                for (int z = -3; z <= +3; ++z)
                    for (int x = -3; x <= +3; ++x)
            {
                if (Math.Abs(x) == 3 || Math.Abs(y) == 3 || Math.Abs(z) == 3) //Calc only edge
                {
                    int index = lvl.IntOffset(b, x, y, z);
                    byte block = lvl.GetTile(index);
                    if (block == Block.Zero) continue;
                    
                    if ((!lava && Block.Convert(block) == Block.water) || (lava && Block.Convert(block) == Block.lava))
                        lvl.AddCheck(index);
                }
            }
        }
        
        public static void DoOther(Level lvl, ref Check C) {
            if (lvl.physics <= 1) { C.data.Data = 255; return; }
            ushort x, y, z;
            lvl.IntToPos(C.b, out x, out y, out z);
            
            //Adv physics kills flowers and mushroos in water/lava
            AirPhysics.PhysAir(lvl, lvl.PosToInt((ushort)(x + 1), y, z));
            AirPhysics.PhysAir(lvl, lvl.PosToInt((ushort)(x - 1), y, z));
            AirPhysics.PhysAir(lvl, lvl.PosToInt(x, y, (ushort)(z + 1)));
            AirPhysics.PhysAir(lvl, lvl.PosToInt(x, y, (ushort)(z - 1)));
            AirPhysics.PhysAir(lvl, lvl.PosToInt(x, (ushort)(y + 1), z));
            C.data.Data = 255;
        }
    }
}
