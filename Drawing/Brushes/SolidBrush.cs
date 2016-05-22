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
        readonly byte type, extType;
        
        public SolidBrush(byte type, byte extType) {
            this.type = type;
            this.extType = extType;
        }
        
        public override string Name { get { return "Normal"; } }
        
        public override string[] Help { get { return HelpString; } }
        
        public static string[] HelpString = new [] {
            "%TArguments: [block]",
            "%HDraws using the specified block.",
            "%H   If block is not given, the currently held block is used.",
        };
        
        public static Brush Process(BrushArgs args) {
            if (args.Message == "")
                return new SolidBrush(args.Type, args.ExtType);
            byte extType;
            byte type = DrawCmd.GetBlock(args.Player, args.Message, out extType);
            if (type == Block.Zero) return null;
            return new SolidBrush(type, extType);
        }
        
        public override byte NextBlock(DrawOp op) { return type; }
        
        public override byte NextExtBlock(DrawOp op) { return extType; }
    }
}
