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

namespace MCGalaxy.Drawing.Brushes {
    public sealed class SolidBrushFactory : BrushFactory {        
        public override string Name { get { return "Normal"; } }
        public override string[] Help { get { return HelpString; } }
        
        public static string[] HelpString = new [] {
            "%TArguments: [block]",
            "%HDraws using the specified block.",
            "%H  If [block] is not given, your currently held block is used.",
        };
        
        public override Brush Construct(BrushArgs args) {
            if (args.Message == "") {
                if (!DrawCmd.CheckBlock(args.Player, args.Block)) return null;
                return new SolidBrush(args.Block, args.ExtBlock);
            }
            
            byte extBlock;
            int block = DrawCmd.GetBlockIfAllowed(args.Player, args.Message, out extBlock);
            if (block == -1) return null;
            return new SolidBrush((byte)block, extBlock);
        }
        
        public override bool Validate(BrushArgs args) {
            if (args.Message == "") return true;
            byte extBlock;
            return DrawCmd.GetBlockIfAllowed(args.Player, args.Message, out extBlock) != -1;
        }
    }
    
    public sealed class CheckeredBrushFactory : BrushFactory {
        public override string Name { get { return "Checkered"; } }
        public override string[] Help { get { return HelpString; } }
        
        public static string[] HelpString = new [] {
            "%TArguments: [block1] [block2]",
            "%HDraws an alternating pattern of block1 and block2.",
            "%H  If [block1] is not given, your currently held block is used.",
            "%H  If [block2] is not given, skip block is used.",
        };
        
        public override Brush Construct(BrushArgs args) {
            if (args.Message == "") {
                if (!DrawCmd.CheckBlock(args.Player, args.Block)) return null;
                return new CheckeredBrush(args.Block, args.ExtBlock, 0, 0);
            }              
            string[] parts = args.Message.Split(' ');
            
            byte extBlock1;
            int block1 = DrawCmd.GetBlockIfAllowed(args.Player, parts[0], out extBlock1);
            if (block1 == -1) return null;
            if (parts.Length == 1)
                return new CheckeredBrush((byte)block1, extBlock1, Block.Invalid, 0);
            
            byte extBlock2;
            int block2 = DrawCmd.GetBlockIfAllowed(args.Player, parts[1], out extBlock2);
            if (block2 == -1) return null;
            return new CheckeredBrush((byte)block1, extBlock1, (byte)block2, extBlock2);
        }
    }
    
    public sealed class PasteBrushFactory : BrushFactory {    
        public override string Name { get { return "Paste"; } }       
        public override string[] Help { get { return HelpString; } }
        
        public static string[] HelpString = new [] {
            "%TArguments: none",
            "%HDraws using blocks from the current copy state.",
        };
        
        public override Brush Construct(BrushArgs args) {
            if (args.Player.CopyBuffer == null) {
                args.Player.SendMessage("You haven't copied anything yet.");
                return null;
            }
            return new PasteBrush(args.Player.CopyBuffer);
        }
    }
    
    public sealed class StripedBrushFactory : BrushFactory {        
        public override string Name { get { return "Striped"; } }        
        public override string[] Help { get { return HelpString; } }
        
        public static string[] HelpString = new [] {
            "%TArguments: [block1] [block2]",
            "%HDraws a diagonally-alternating pattern of block1 and block2.",
            "%H   If block2 is not given, air is used.",
            "%H   If block1 is not given, the currently held block is used.",
        };
        
        public override Brush Construct(BrushArgs args) {
            if (args.Message == "") {
                if (!DrawCmd.CheckBlock(args.Player, args.Block)) return null;
                return new StripedBrush(args.Block, args.ExtBlock, 0, 0);
            }
            string[] parts = args.Message.Split(' ');
            
            byte extBlock1;
            int block1 = DrawCmd.GetBlockIfAllowed(args.Player, parts[0], out extBlock1);
            if (block1 == -1) return null;
            if (parts.Length == 1)
                return new StripedBrush((byte)block1, extBlock1, 0, 0);
            
            byte extBlock2;
            int block2 = DrawCmd.GetBlockIfAllowed(args.Player, parts[1], out extBlock2);
            if (block2 == -1) return null;
            return new StripedBrush((byte)block1, extBlock1, (byte)block2, extBlock2);
        }
    }
    
    
    public sealed class RainbowBrushFactory : BrushFactory {
        public override string Name { get { return "Rainbow"; } }
        public override string[] Help { get { return HelpString; } }
        
        public static string[] HelpString = new [] {
            "%TArguments: <random>",
            "%HIf no arguments are given, draws a diagonally repeating rainbow",
            "%HIf \'random\' is given, draws by randomly selecting blocks from the rainbow pattern.",
        };
        
        public override Brush Construct(BrushArgs args) {
            if (args.Message == "random") return new RandomRainbowBrush();
            if (args.Message == "bw") return new BWRainbowBrush();
            return new RainbowBrush();
        }
    }
    
    public sealed class BWRainbowBrushFactory : BrushFactory {
        public override string Name { get { return "BWRainbow"; } }
        public override string[] Help { get { return HelpString; } }
        
        public static string[] HelpString = new [] {
            "%TArguments: none",
            "%HDraws a diagonally repeating black-white rainbow",
        };
        
        public override Brush Construct(BrushArgs args) { return new BWRainbowBrush(); }
    }
}
