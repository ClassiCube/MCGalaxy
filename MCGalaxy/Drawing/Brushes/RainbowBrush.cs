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
    
    public sealed class RainbowBrush : CheckeredPaletteBrush {

        public override string Name { get { return "Rainbow"; } }
        public RainbowBrush() : base(blocks) {}
        
        static ExtBlock[] blocks = new ExtBlock[] { 
            (ExtBlock)Block.Red,   (ExtBlock)Block.Orange,  (ExtBlock)Block.Yellow,
            (ExtBlock)Block.Lime,  (ExtBlock)Block.Green,   (ExtBlock)Block.Teal,
            (ExtBlock)Block.Aqua,  (ExtBlock)Block.Cyan,    (ExtBlock)Block.Blue,
            (ExtBlock)Block.Indigo, (ExtBlock)Block.Violet, (ExtBlock)Block.Magenta,
            (ExtBlock)Block.Pink };
    }
    
    public sealed class BWRainbowBrush : CheckeredPaletteBrush {
        
        public override string Name { get { return "BWRainbow"; } }
        public BWRainbowBrush() : base(blocks) {}
        
        static ExtBlock[] blocks = new ExtBlock[] { 
            (ExtBlock)Block.Iron,  (ExtBlock)Block.White,    (ExtBlock)Block.Gray,
            (ExtBlock)Block.Black, (ExtBlock)Block.Obsidian, (ExtBlock)Block.Black, 
            (ExtBlock)Block.Gray,  (ExtBlock)Block.White };
    }
    
    internal sealed class RandomRainbowBrush : Brush {
        readonly Random rnd;
        ExtBlock block = default(ExtBlock);
        
        public override string Name { get { return "RandomRainbow"; } }
        
        public RandomRainbowBrush() { rnd = new Random(); }
        
        public RandomRainbowBrush(int seed) { rnd = new Random(seed); }
        
        public override ExtBlock NextBlock(DrawOp op) {
            block.BlockID = (byte)rnd.Next(Block.Red, Block.Black);
            return block;
        }
    }
}
