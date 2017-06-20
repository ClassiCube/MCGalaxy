﻿/*
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
using MCGalaxy.Commands;
using MCGalaxy.Commands.Building;

namespace MCGalaxy.Drawing.Brushes {
    public sealed class SolidBrushFactory : BrushFactory {
        public override string Name { get { return "Normal"; } }
        public override string[] Help { get { return HelpString; } }
        
        static string[] HelpString = new string[] {
            "%TArguments: [block]",
            "%HDraws using the specified block.",
            "%H  If [block] is not given, your currently held block is used.",
        };
        
        public override Brush Construct(BrushArgs args) {
            if (args.Message == "") {
                if (!CommandParser.IsBlockAllowed(args.Player, "draw with", args.Block)) return null;
                return new SolidBrush(args.Block);
            }
            
            ExtBlock block;
            if (!CommandParser.GetBlockIfAllowed(args.Player, args.Message, out block)) return null;
            return new SolidBrush(block);
        }
        
        public override bool Validate(BrushArgs args) {
            if (args.Message == "") return true;
            ExtBlock block;
            return CommandParser.GetBlockIfAllowed(args.Player, args.Message, out block);
        }
    }
    
    public sealed class CheckeredBrushFactory : BrushFactory {
        public override string Name { get { return "Checkered"; } }
        public override string[] Help { get { return HelpString; } }
        
        static string[] HelpString = new string[] {
            "%TArguments: [block1] [block2]",
            "%HDraws an alternating pattern of block1 and block2.",
            "%H  If [block1] is not given, your currently held block is used.",
            "%H  If [block2] is not given, skip block is used.",
        };
        
        public override Brush Construct(BrushArgs args) {
            if (args.Message == "") {
                if (!CommandParser.IsBlockAllowed(args.Player, "draw with", args.Block)) return null;
                return new CheckeredBrush(args.Block, ExtBlock.Air);
            }
            string[] parts = args.Message.SplitSpaces();
            
            ExtBlock block1;
            if (!CommandParser.GetBlockIfAllowed(args.Player, parts[0], out block1, true)) return null;
            if (parts.Length == 1)
                return new CheckeredBrush(block1, ExtBlock.Invalid);
            
            ExtBlock block2;
            if (!CommandParser.GetBlockIfAllowed(args.Player, parts[1], out block2, true)) return null;
            return new CheckeredBrush(block1, block2);
        }
    }
    
    public sealed class PasteBrushFactory : BrushFactory {
        public override string Name { get { return "Paste"; } }
        public override string[] Help { get { return HelpString; } }
        
        static string[] HelpString = new string[] {
            "%TArguments: none",
            "%HDraws by pasting blocks from current /copy.",
            "%TArguments: [block1] [block2]..",
            "%HDraws by pasting only the specified blocks from current /copy.",
            "%TArguments: not [block1] [block2]..",
            "%HDraws by pasting blocks from current /copy, except for the specified blocks.",
        };
        
        public override Brush Construct(BrushArgs args) {
            if (args.Player.CopyBuffer == null) {
                args.Player.SendMessage("You haven't copied anything yet.");
                return null;
            }
            
            if (args.Message == "")
                return new SimplePasteBrush(args.Player.CopyBuffer);            
            string[] parts = args.Message.SplitSpaces();
            PasteBrush brush = new PasteBrush(args.Player.CopyBuffer);
            
            if (parts[0].CaselessEq("not")) {
                brush.Exclude = ReplaceBrushFactory.GetBlocks(args.Player, 1, parts.Length, parts);
                if (brush.Exclude == null) return null;
            } else {
                brush.Include = ReplaceBrushFactory.GetBlocks(args.Player, 0, parts.Length, parts);
                if (brush.Include == null) return null;
            }
            return brush;
        }
    }
    
    public sealed class StripedBrushFactory : BrushFactory {
        public override string Name { get { return "Striped"; } }
        public override string[] Help { get { return HelpString; } }
        
        static string[] HelpString = new string[] {
            "%TArguments: [block1] [block2]",
            "%HDraws a diagonally-alternating pattern of block1 and block2.",
            "%H   If block1 is not given, the currently held block is used.",
            "%H   If block2 is not given, air is used.",
        };
        
        public override Brush Construct(BrushArgs args) {
            if (args.Message == "") {
                if (!CommandParser.IsBlockAllowed(args.Player, "draw with", args.Block)) return null;
                return new StripedBrush(args.Block, ExtBlock.Air);
            }
            string[] parts = args.Message.SplitSpaces();
            
            ExtBlock block1;
            if (!CommandParser.GetBlockIfAllowed(args.Player, parts[0], out block1, true)) return null;
            if (parts.Length == 1)
                return new StripedBrush(block1, ExtBlock.Air);
            
            ExtBlock block2;
            if (!CommandParser.GetBlockIfAllowed(args.Player, parts[1], out block2, true)) return null;
            return new StripedBrush(block1, block2);
        }
    }
    
    
    public sealed class RainbowBrushFactory : BrushFactory {
        public override string Name { get { return "Rainbow"; } }
        public override string[] Help { get { return HelpString; } }
        
        static string[] HelpString = new string[] {
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
        
        static string[] HelpString = new string[] {
            "%TArguments: none",
            "%HDraws a diagonally repeating black-white rainbow",
        };
        
        public override Brush Construct(BrushArgs args) { return new BWRainbowBrush(); }
    }
}
