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

namespace MCGalaxy {
    
    public abstract class Brush {
        
        public abstract byte NextBlock();
    }
    
    public sealed class SolidBrush : Brush {
        readonly byte block;
        
        public SolidBrush(byte block) {
            this.block = block;
        }
        
        public override byte NextBlock() {
            return block;
        }
    }
    
    public sealed class RainbowBrush : Brush {
        byte curBlock = Block.red;
        
        public override byte NextBlock() {
            byte block = curBlock; 
            curBlock++;
            if (curBlock > Block.darkpink)
                curBlock = Block.red;
            return block;
        }
    }
    
    public sealed class RandomBrush : Brush {
        readonly Random rnd = new Random();
        readonly byte block;
        
        public RandomBrush(byte block) {
            this.block = block;
        }
        
        public override byte NextBlock() {
            return (byte)rnd.Next(1, 11) <= 5 ? block : Block.Zero;
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
        
        public override byte NextBlock() {
            return (byte)rnd.Next(Block.red, Block.darkgrey);
        }
    }
}
