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
    public sealed class ReplaceBrushFactory : BrushFactory {       
        public override string Name { get { return "Replace"; } }       
        public override string[] Help { get { return HelpString; } }
        
        public static string[] HelpString = new [] {
            "%TArguments: [block1] [block2].. [new]",
            "%HDraws by replacing existing blocks that are in the given [blocks] with [new]",
            "%H  If only [block] is given, replaces with your held block.",
        };
        
        public override Brush Construct(BrushArgs args) { return ProcessReplace(args, false); }
        
        internal static Brush ProcessReplace(BrushArgs args, bool not) {
            string[] parts = args.Message.Split(' ');
            if (args.Message == "") {
                args.Player.SendMessage("You need at least one block to replace."); return null;
            }
            
            int count = parts.Length == 1 ? 1 : parts.Length - 1;
            ExtBlock[] toAffect = GetBlocks(args.Player, 0, count, parts);
            if (toAffect == null) return null;
            
            ExtBlock target;
            if (!GetTargetBlock(args, parts, out target)) return null;
            
            if (not) return new ReplaceNotBrush(toAffect, target);
            return new ReplaceBrush(toAffect, target);
        }
        
        internal static ExtBlock[] GetBlocks(Player p, int start, int max, string[] parts) {
            ExtBlock[] blocks = new ExtBlock[max - start];
            for (int i = 0; i < blocks.Length; i++)
                blocks[i].Block = Block.Invalid;
            for (int i = 0; start < max; start++, i++ ) {
                byte extBlock = 0;
                int block = DrawCmd.GetBlock(p, parts[start], out extBlock);
                if (block == -1) return null;
                blocks[i].Block = (byte)block; blocks[i].Ext = extBlock;
            }
            return blocks;
        }
        
        static bool GetTargetBlock(BrushArgs args, string[] parts, out ExtBlock target) {
            if (parts.Length == 1) {
                target = new ExtBlock(args.Block, args.ExtBlock);
                return true;
            }
            
            target = default(ExtBlock);
            int block = DrawCmd.GetBlock(args.Player, parts[parts.Length - 1], out target.Ext);
            if (block == -1) return false;
            target.Block = (byte)block;
            return true;
        }
    }
    
    public sealed class ReplaceNotBrushFactory : BrushFactory {    
        public override string Name { get { return "ReplaceNot"; } }        
        public override string[] Help { get { return HelpString; } }
        
        public static string[] HelpString = new [] {
            "%TArguments: [block1] [block2].. [new]",
            "%HDraws by replacing existing blocks that not are in the given [blocks] with [new]",
            "%H  If only [block] is given, replaces with your held block.",
        };
        
        public override Brush Construct(BrushArgs args) { return ReplaceBrushFactory.ProcessReplace(args, true); }
    }
}
