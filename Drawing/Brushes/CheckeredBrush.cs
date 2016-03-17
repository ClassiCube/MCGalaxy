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
    
    public sealed class CheckeredBrush : Brush {
        readonly byte type1, extType1, type2, extType2;
        
        public CheckeredBrush(byte type1, byte extType1, byte type2, byte extType2) {
            this.type1 = type1; this.extType1 = extType1;
            this.type2 = type2; this.extType2 = extType2;
        }
        
        public override string Name { get { return "Checkered"; } }
        
        public override string[] Help { get { return HelpString; } }
        
        public static string[] HelpString = new [] {
            "%TArguments: [block1] [block2]",
            "%HDraws an alternating pattern of block1 and block2.",
            "%H   If block2 is not given, air is used.",
            "%H   If block1 is not given, the currently held block is used.",
        };
        
        public static Brush Process(BrushArgs args) {
            if (args.Message == "")
                return new CheckeredBrush(args.Type, args.ExtType, 0, 0);
            string[] parts = args.Message.Split(' ');
            byte extType1;
            byte type1 = DrawCmd.GetBlock(args.Player, parts[0], out extType1);
            if (type1 == Block.Zero) return null;
            if (parts.Length == 1)
                return new CheckeredBrush(type1, extType1, Block.Zero, 0);
            
            byte extType2;
            byte type2 = DrawCmd.GetBlock(args.Player, parts[1], out extType2);
            if (type2 == Block.Zero) return null;
            return new CheckeredBrush(type1, extType1, type2, extType2);
        }
        
        public override byte NextBlock(DrawOp op) {
            return ((op.Coords.X + op.Coords.Y + op.Coords.Z) & 1) == 0 ? type1 : type2;
        }
        
        public override byte NextExtBlock(DrawOp op) {
            return ((op.Coords.X + op.Coords.Y + op.Coords.Z) & 1) == 0 ? extType1 : extType2;
        }
    }
}
