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
using MCGalaxy.Commands.Building;
using MCGalaxy.Drawing.Ops;

namespace MCGalaxy.Drawing.Brushes {    
    public sealed class SolidBrush : Brush {
        readonly byte block, extBlock;
        
        public SolidBrush(byte type, byte extType) {
            this.block = type;
            this.extBlock = extType;
        }
        
        public override string Name { get { return "Normal"; } }
        
        public override byte NextBlock(DrawOp op) { return block; }
        
        public override byte NextExtBlock(DrawOp op) { return extBlock; }
    }
    
    public sealed class StripedBrush : Brush {
        readonly byte block1, extBlock1, block2, extBlock2;
        
        public StripedBrush(byte block1, byte extBlock1, byte block2, byte extBlock2) {
            this.block1 = block1; this.extBlock1 = extBlock1;
            this.block2 = block2; this.extBlock2 = extBlock2;
        }
        
        public override string Name { get { return "Striped"; } }
        
        public override byte NextBlock(DrawOp op) {
            return ((op.Coords.X + op.Coords.Y + op.Coords.Z) & 3) <= 1 ? block1 : block2;
        }
        
        public override byte NextExtBlock(DrawOp op) {
            return ((op.Coords.X + op.Coords.Y + op.Coords.Z) & 3) <= 1 ? extBlock1 : extBlock2;
        }
    }
    
    public sealed class CheckeredBrush : Brush {
        readonly byte block1, extBlock1, block2, extBlock2;
        
        public CheckeredBrush(byte block1, byte extBlock1, byte block2, byte extBlock2) {
            this.block1 = block1; this.extBlock1 = extBlock1;
            this.block2 = block2; this.extBlock2 = extBlock2;
        }
        
        public override string Name { get { return "Checkered"; } }
        
        public override byte NextBlock(DrawOp op) {
            return ((op.Coords.X + op.Coords.Y + op.Coords.Z) & 1) == 0 ? block1 : block2;
        }
        
        public override byte NextExtBlock(DrawOp op) {
            return ((op.Coords.X + op.Coords.Y + op.Coords.Z) & 1) == 0 ? extBlock1 : extBlock2;
        }
    }
}
