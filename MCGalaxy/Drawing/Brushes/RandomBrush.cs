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
using MCGalaxy.Drawing.Ops;
using BlockID = System.UInt16;

namespace MCGalaxy.Drawing.Brushes 
{   
    public sealed class RandomBrush : Brush 
    {
        readonly BlockID[] blocks;
        readonly int seed;
        
        public RandomBrush(BlockID[] blocks) {
            this.blocks = blocks;
            seed = new Random().Next();
        }
        
        public override string Name { get { return "Random"; } }
        
        int next;
        const int mask = 0x7fffffff;
        public override BlockID NextBlock(DrawOp op) {
            // Sourced from http://freespace.virgin.net/hugo.elias/models/m_perlin.htm
            int n = (op.Coords.X + 1217 * op.Coords.Y + 4751 * op.Coords.Z + 673 * seed) & mask;
            n = (n >> 13) ^ n;
            int raw = (n * (n * n * 60493 + 19990303) + 1376312589) & mask;
            next = (int)Math.Floor((raw / (double)mask) * blocks.Length);
            return blocks[next];
        }
    }
}
