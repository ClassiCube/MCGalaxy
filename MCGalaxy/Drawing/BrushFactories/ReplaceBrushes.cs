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
using MCGalaxy.Commands;
using MCGalaxy.Commands.Building;
using BlockID = System.UInt16;

namespace MCGalaxy.Drawing.Brushes {  
    public sealed class ReplaceBrushFactory : BrushFactory {       
        public override string Name { get { return "Replace"; } }       
        public override string[] Help { get { return HelpString; } }
        
        static string[] HelpString = new string[] {
            "&TArguments: [block1] [block2].. [new]",
            "&HDraws by replacing existing blocks that are in the given [blocks] with [new]",
            "&H  If only [block] is given, replaces with your held block.",
        };
        
        public override Brush Construct(BrushArgs args) { return ProcessReplace(args, false); }
        
        internal static Brush ProcessReplace(BrushArgs args, bool not) {
            string[] parts = args.Message.SplitSpaces();
            if (args.Message.Length == 0) {
                args.Player.Message("You need at least one block to replace."); return null;
            }
            
            int count = parts.Length == 1 ? 1 : parts.Length - 1;
            BlockID[] toAffect = GetBlocks(args.Player, 0, count, parts);
            if (toAffect == null) return null;
            
            BlockID target;
            if (!GetTargetBlock(args, parts, out target)) return null;
            
            if (not) return new ReplaceNotBrush(toAffect, target);
            return new ReplaceBrush(toAffect, target);
        }
        
        internal static BlockID[] GetBlocks(Player p, int start, int max, string[] parts) {
            BlockID[] blocks = new BlockID[max - start];
            for (int i = 0; i < blocks.Length; i++)
                blocks[i] = Block.Invalid;
            
            for (int i = 0; start < max; start++, i++ ) {
                BlockID block;
                if (!CommandParser.GetBlockIfAllowed(p, parts[start], out block)) return null;

                blocks[i] = block;
            }
            return blocks;
        }
        
        static bool GetTargetBlock(BrushArgs args, string[] parts, out BlockID target) {
            target = 0;
            if (parts.Length == 1) {
                if (!CommandParser.IsBlockAllowed(args.Player, "draw with", args.Block)) return false;
                
                target = args.Block; return true;
            }            
            return CommandParser.GetBlockIfAllowed(args.Player, parts[parts.Length - 1], out target);
        }
    }
    
    public sealed class ReplaceNotBrushFactory : BrushFactory {    
        public override string Name { get { return "ReplaceNot"; } }        
        public override string[] Help { get { return HelpString; } }
        
        static string[] HelpString = new string[] {
            "&TArguments: [block1] [block2].. [new]",
            "&HDraws by replacing existing blocks that not are in the given [blocks] with [new]",
            "&H  If only [block] is given, replaces with your held block.",
        };
        
        public override Brush Construct(BrushArgs args) { return ReplaceBrushFactory.ProcessReplace(args, true); }
    }
}
