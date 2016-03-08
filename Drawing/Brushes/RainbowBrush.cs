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
using MCGalaxy.Commands;
using MCGalaxy.Drawing.Ops;

namespace MCGalaxy.Drawing.Brushes {
    
    public sealed class RainbowBrush : Brush {
        
        public override byte NextBlock(DrawOp op) {
            int offset = (op.Coords.X + op.Coords.Y + op.Coords.Z) % 13;
            if (offset < 0) offset += 13;
            return (byte)(Block.red + offset);
        }
        
        public override byte NextExtBlock(DrawOp op) { return 0; }
        
        public static Brush Process(BrushArgs args) {
            if (args.Message.StartsWith("random")) {
                string[] parts = args.Message.Split(' ');
                int seed;
                if (parts.Length > 1 && Int32.TryParse(parts[1], out seed))
                    return new RandomRainbowBrush(seed);
                return new RandomRainbowBrush();           
            }

            if (args.Message == "bw")
                return new BWRainbowBrush();
            return new RainbowBrush();
        }
    }
    
    public sealed class BWRainbowBrush : Brush {
        
        static byte[] blocks = { Block.iron, Block.white, Block.lightgrey,
            Block.darkgrey, Block.obsidian, Block.darkgrey, Block.lightgrey, Block.white };
        public override byte NextBlock(DrawOp op) {
            int offset = (op.Coords.X + op.Coords.Y + op.Coords.Z) % 8;
            if (offset < 0) offset += 8;
            return blocks[offset];
        }
        
        public override byte NextExtBlock(DrawOp op) { return 0; }
        
        public static Brush Process(BrushArgs args) {
            return new BWRainbowBrush();
        }
    }
    
    public sealed class RandomRainbowBrush : Brush {
        readonly Random rnd;
        
        public RandomRainbowBrush() {
            rnd = new Random();
        }
        
        public RandomRainbowBrush(int seed) {
            rnd = new Random(seed);
        }
        
        public override byte NextBlock(DrawOp op) {
            return (byte)rnd.Next(Block.red, Block.darkgrey);
        }
        
        public override byte NextExtBlock(DrawOp op) { return 0; }
    }
}
