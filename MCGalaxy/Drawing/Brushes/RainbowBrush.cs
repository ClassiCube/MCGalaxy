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

namespace MCGalaxy.Drawing.Brushes {
    
    public sealed class RainbowBrush : Brush {
        
        ExtBlock block = default(ExtBlock);
        public override string Name { get { return "Rainbow"; } }
        
        public override ExtBlock NextBlock(DrawOp op) {
            int offset = (op.Coords.X + op.Coords.Y + op.Coords.Z) % 13;
            if (offset < 0) offset += 13;
            
            block.BlockID = (byte)(Block.red + offset);
            return block;
        }
    }
    
    public sealed class BWRainbowBrush : Brush {
        
        ExtBlock block = default(ExtBlock);
        public override string Name { get { return "BWRainbow"; } }
        
        static byte[] blocks = new byte[] { Block.iron, Block.white, Block.lightgrey,
            Block.darkgrey, Block.obsidian, Block.darkgrey, Block.lightgrey, Block.white };
        
        public override ExtBlock NextBlock(DrawOp op) {
            int offset = (op.Coords.X + op.Coords.Y + op.Coords.Z) % 8;
            if (offset < 0) offset += 8;
            
            block.BlockID = blocks[offset];
            return block;
        }
    }
    
    internal sealed class RandomRainbowBrush : Brush {
        readonly Random rnd;
        ExtBlock block = default(ExtBlock);
        
        public override string Name { get { return "RandomRainbow"; } }
        
        public RandomRainbowBrush() { rnd = new Random(); }
        
        public RandomRainbowBrush(int seed) { rnd = new Random(seed); }
        
        public override ExtBlock NextBlock(DrawOp op) {
            block.BlockID = (byte)rnd.Next(Block.red, Block.darkgrey);
            return block;
        }
    }
}
