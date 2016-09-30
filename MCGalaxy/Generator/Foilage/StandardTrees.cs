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
// Copyright 2009, 2010 Matvei Stefarov <me@matvei.org>
/*
This generator was developed by Neko_baron.

Ideas, concepts, and code were used from the following two sources:
1) Isaac McGarvey's 'perlin noise generator' code
2) http://www.lighthouse3d.com/opengl/terrain/index.php3?introduction

 */
using System;
using System.Collections.Generic;
using MCGalaxy.Drawing.Brushes;

namespace MCGalaxy.Generator.Foilage {
    public sealed class CactusTree : Tree {
        
        public override void SetData(Random rnd) {
            height = (byte)rnd.Next(3, 6);
            top = 0;
            size = 0;
            this.rnd = rnd;
        }
        
        public override void Output(ushort x, ushort y, ushort z, TreeOutput output) {
            for (ushort dy = 0; dy <= height; dy++) {
                output(x, (ushort)(y + dy), z, Block.green);
            }

            int value = rnd.Next(1, 3);
            int inX = value == 1 ? -1 : 0;
            int inZ = value == 2 ? -1 : 0;

            for (ushort dy = height; dy <= rnd.Next(height + 2, height + 5); dy++) {
                output((ushort)(x + inX), (ushort)(y + dy), (ushort)(z + inZ), Block.green);
            }
            for (ushort dy = height; dy <= rnd.Next(height + 2, height + 5); dy++) {
                output((ushort)(x - inX), (ushort)(y + dy), (ushort)(z - inZ), Block.green);
            }
        }
    }

    public sealed class NormalTree : Tree {
        
        public override void SetData(Random rnd) {
            height = (byte)rnd.Next(5, 8);
            top = (byte)(height - rnd.Next(2, 4));
            size = top;
            this.rnd = rnd;
        }
        
        public override void Output(ushort x, ushort y, ushort z, TreeOutput output) {
            for (ushort dy = 0; dy < top + height - 1; dy++)
                output(x, (ushort)(y + dy), z, Block.trunk);
            
            for (short dy = (short)-top; dy <= top; ++dy)
                for (short dz = (short)-top; dz <= top; ++dz)
                    for (short dx = (short)-top; dx <= top; ++dx)
            {
                short dist = (short)(Math.Sqrt(dx * dx + dy * dy + dz * dz));
                if ((dist < top + 1) && rnd.Next(dist) < 2) {
                    ushort xx = (ushort)(x + dx), yy = (ushort)(y + dy + height), zz = (ushort)(z + dz);

                    if (xx != x || zz != z || dy >= top - 1)
                        output(xx, yy, zz, Block.leaf);
                }
            }
        }
    }

    public sealed class ClassicTree : Tree {
        
        public override void SetData(Random rnd) {
            height = (byte)rnd.Next(3, 7);
            top = (byte)(height - 2);
            size = 2;
            this.rnd = rnd;
        }
        
        public override void Output(ushort x, ushort y, ushort z, TreeOutput output) {
            for (ushort dy = 0; dy <= height; dy++)
                output(x, (ushort)(y + dy), z, Block.trunk);

            for (int dy = top; dy <= height + 1; dy++) {
                int dist = dy > height - 1 ? 1 : 2;
                for (int dz = -dist; dz <= dist; dz++)
                    for (int dx = -dist; dx <= dist; dx++)
                {
                    ushort xx = (ushort)(x + dx), yy = (ushort)(y + dy), zz = (ushort)(z + dz);
                    if (xx == x && zz == z && dy <= height) continue;

                    if (Math.Abs(dx) == dist && Math.Abs(dz) == dist) {
                        if (dy > height) continue;
                        if (rnd.Next(2) == 0) output(xx, yy, zz, Block.leaf);
                    } else {
                        output(xx, yy, zz, Block.leaf);
                    }
                }
            }
        }
    }
    
    public sealed class SwampTree : Tree {
        
        public override void SetData(Random rnd) {
            height = (byte)rnd.Next(4, 8);
            top = (byte)(height - 2);
            size = 3;
            this.rnd = rnd;
        }
        
        public override void Output(ushort x, ushort y, ushort z, TreeOutput output) {
            for (ushort dy = 0; dy <= height; dy++)
                output(x, (ushort)(y + dy), z, Block.trunk);

            for (int dy = top; dy <= height + 1; dy++) {
                int dist = dy > height - 1 ? 2 : 3;
                for (int dz = (short)-dist; dz <= dist; dz++)
                    for (int dx = (short)-dist; dx <= dist; dx++)
                {
                    ushort xx = (ushort)(x + dx), yy = (ushort)(y + dy), zz = (ushort)(z + dz);
                    if (xx == x && zz == z && dy <= height) continue;

                    if (Math.Abs(dz) == dist && Math.Abs(dx) == dist) {
                        if (dy > height) continue;
                        if (rnd.Next(2) == 0) output(xx, yy, zz, Block.leaf);
                    } else {
                        output(xx, yy, zz, Block.leaf);
                    }
                }
            }
        }
    }
}
