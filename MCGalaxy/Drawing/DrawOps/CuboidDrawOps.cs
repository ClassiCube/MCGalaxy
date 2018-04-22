/*
    Copyright 2015 MCGalaxy
        
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
using System.Collections.Generic;
using MCGalaxy.Drawing.Brushes;
using MCGalaxy.Maths;
using BlockID = System.UInt16;

namespace MCGalaxy.Drawing.Ops {

    public class HollowDrawOp : CuboidDrawOp {      
        public override string Name { get { return "Hollow"; } }
        public BlockID Skip;
        
        static bool CanHollow(BlockID block, bool andAir = false) {
            block = Block.Convert(block);
            if (andAir && block == Block.Air) return true;
            return block >= Block.Water && block <= Block.StillLava;
        }
        
        public override void Perform(Vec3S32[] marks, Brush brush, DrawOpOutput output) {
            Vec3U16 p1 = Clamp(Min), p2 = Clamp(Max);
            
            for (ushort y = p1.Y; y <= p2.Y; y++)
                for (ushort z = p1.Z; z <= p2.Z; z++)
                    for (ushort x = p1.X; x <= p2.X; x++)
            {
                bool hollow = true;
                BlockID block = Level.GetBlock(x, y, z);
                if (!CanHollow(block, true) && block != Skip) {
                    CheckTile(x - 1, y, z, ref hollow);
                    CheckTile(x + 1, y, z, ref hollow);
                    CheckTile(x, y - 1, z, ref hollow);
                    CheckTile(x, y + 1, z, ref hollow);
                    CheckTile(x, y, z - 1, ref hollow);
                    CheckTile(x, y, z + 1, ref hollow);
                } else {
                    hollow = false;
                }
                
                if (hollow) output(Place(x, y, z, Block.Air));
            }
        }
        
        void CheckTile(int x, int y, int z, ref bool hollow) {
            BlockID block = Level.GetBlock((ushort)x, (ushort)y, (ushort)z);
            if (CanHollow(block) || block == Skip) hollow = false;
        }
    }
    
    public class OutlineDrawOp : CuboidDrawOp {        
        public override string Name { get { return "Outline"; } }
        public BlockID Target;
        public bool Above = true, Layer = true, Below = true;
        
        public override void Perform(Vec3S32[] marks, Brush brush, DrawOpOutput output) {
            Vec3U16 p1 = Clamp(Min), p2 = Clamp(Max);
            for (ushort y = p1.Y; y <= p2.Y; y++)
                for (ushort z = p1.Z; z <= p2.Z; z++)
                    for (ushort x = p1.X; x <= p2.X; x++)
            {
                bool outline = false;
                outline |= Layer && Level.GetBlock((ushort)(x - 1), y, z) == Target;
                outline |= Layer && Level.GetBlock((ushort)(x + 1), y, z) == Target;
                outline |= Layer && Level.GetBlock(x, y, (ushort)(z - 1)) == Target;
                outline |= Layer && Level.GetBlock(x, y, (ushort)(z + 1)) == Target;
                outline |= Below && Level.GetBlock(x, (ushort)(y - 1), z) == Target;
                outline |= Above && Level.GetBlock(x, (ushort)(y + 1), z) == Target;

                if (outline && Level.GetBlock(x, y, z) != Target)
                    output(Place(x, y, z, brush));
            }
        }
    }
    
    public class RainbowDrawOp : CuboidDrawOp {
        
        public bool AllowAir;
        public override string Name { get { return "Rainbow"; } }
        
        public override void Perform(Vec3S32[] marks, Brush brush, DrawOpOutput output) {
            Vec3U16 p1 = Clamp(Min), p2 = Clamp(Max);
            int dx = Math.Abs(p1.X - p2.X), dy = Math.Abs(p1.Y - p2.Y), dz = Math.Abs(p1.Z - p2.Z);
            byte stepX = 0, stepY = 0, stepZ = 0;
            
            if (dx >= dy && dx >= dz) {
                stepX = 1;
            } else if (dy > dx && dy > dz) {
                stepY = 1;
            } else if (dz > dy && dz > dx) {
                stepZ = 1;
            }
            
            int repeat = RainbowBrush.blocks.Length;
            int i = repeat - 1;
            brush = new RainbowBrush();            
            
            for (ushort y = p1.Y; y <= p2.Y; y++) {
                i = (i + stepY) % repeat;
                int startZ = i;
                for (ushort z = p1.Z; z <= p2.Z; z++) {
                    i = (i + stepZ) % repeat;
                    int startX = i;
                    for (ushort x = p1.X; x <= p2.X; x++) {
                        i = (i + stepX) % repeat;
                        if (AllowAir || !Level.IsAirAt(x, y, z)) {
                            // Need this because RainbowBrush works on world coords
                            Coords.X = (ushort)i; Coords.Y = 0; Coords.Z = 0;
                            BlockID block = brush.NextBlock(this);
                            output(Place(x, y, z, block));
                        }
                    }
                    i = startX;
                }
                i = startZ;
            }
        }
    }
}
