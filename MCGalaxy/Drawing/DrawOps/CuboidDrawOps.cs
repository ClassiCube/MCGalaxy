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

namespace MCGalaxy.Drawing.Ops {

    public class HollowDrawOp : CuboidDrawOp {      
        public override string Name { get { return "Hollow"; } }
        public byte Skip;
        
        public override void Perform(Vec3S32[] marks, Brush brush, Action<DrawOpBlock> output) {
            Vec3U16 p1 = Clamp(Min), p2 = Clamp(Max);
            
            for (ushort y = p1.Y; y <= p2.Y; y++)
                for (ushort z = p1.Z; z <= p2.Z; z++)
                    for (ushort x = p1.X; x <= p2.X; x++)
            {
                bool hollow = true;
                byte tile = Level.GetTile(x, y, z);
                if (!Block.RightClick(Block.Convert(tile), true) && tile != Skip) {
                    CheckTile(x - 1, y, z, ref hollow);
                    CheckTile(x + 1, y, z, ref hollow);
                    CheckTile(x, y - 1, z, ref hollow);
                    CheckTile(x, y + 1, z, ref hollow);
                    CheckTile(x, y, z - 1, ref hollow);
                    CheckTile(x, y, z + 1, ref hollow);
                } else {
                    hollow = false;
                }
                if (hollow)
                    output(Place(x, y, z, Block.air, 0));
            }
        }
        
        void CheckTile(int x, int y, int z, ref bool hollow) {
            byte tile = Level.GetTile((ushort)x, (ushort)y, (ushort)z);
            if (Block.RightClick(Block.Convert(tile)) || tile == Skip)
                hollow = false;
        }
    }
    
    public class OutlineDrawOp : CuboidDrawOp {        
        public override string Name { get { return "Outline"; } }
        public byte Block, ExtBlock;
        
        public override void Perform(Vec3S32[] marks, Brush brush, Action<DrawOpBlock> output) {
            Vec3U16 p1 = Clamp(Min), p2 = Clamp(Max);
            for (ushort y = p1.Y; y <= p2.Y; y++)
                for (ushort z = p1.Z; z <= p2.Z; z++)
                    for (ushort x = p1.X; x <= p2.X; x++)
            {
                bool outline = false;
                outline |= Check((ushort)(x - 1), y, z);
                outline |= Check((ushort)(x + 1), y, z);
                outline |= Check(x, y, (ushort)(z - 1));
                outline |= Check(x, y, (ushort)(z + 1));
                outline |= Check(x, (ushort)(y - 1), z);
                outline |= Check(x, (ushort)(y + 1), z);

                if (outline && !Check(x, y, z))
                    output(Place(x, y, z, brush));
            }
        }
        
        bool Check(ushort x, ushort y, ushort z) {
            byte block = Level.GetTile(x, y, z);
            if (block != Block) return false;
            return block != MCGalaxy.Block.custom_block || Level.GetExtTile(x, y, z) == ExtBlock;
        }
    }
    
    public class RainbowDrawOp : CuboidDrawOp {
        
        public override string Name { get { return "Rainbow"; } }
        
        public override void Perform(Vec3S32[] marks, Brush brush, Action<DrawOpBlock> output) {
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
            
            int i = 12;
            for (ushort y = p1.Y; y <= p2.Y; y++) {
                i = (i + stepY) % 13;
                int startZ = i;
                for (ushort z = p1.Z; z <= p2.Z; z++) {
                    i = (i + stepZ) % 13;
                    int startX = i;
                    for (ushort x = p1.X; x <= p2.X; x++) {
                        i = (i + stepX) % 13;
                        if (Level.GetTile(x, y, z) != Block.air)
                            output(Place(x, y, z, (byte)(Block.red + i), 0));
                    }
                    i = startX;
                }
                i = startZ;
            }
        }
    }
}
