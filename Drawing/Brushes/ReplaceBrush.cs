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
using MCGalaxy.Commands.Building;
using MCGalaxy.Drawing.Ops;

namespace MCGalaxy.Drawing.Brushes {
    
    public sealed class ReplaceBrush : Brush {
        readonly ExtBlock[] include;
        readonly ExtBlock target;
        
        public ReplaceBrush(ExtBlock[] include, ExtBlock target) {
            this.include = include; this.target = target;
        }
        
        public override string Name { get { return "Replace"; } }
        
        public override string[] Help { get { return HelpString; } }
        
        public static string[] HelpString = new [] {
            "%TArguments: [block1] [block2].. [new]",
            "%HDraws by replacing existing blocks that are in the given [blocks] with [new]",
            "%H  If only [block] is given, replaces with your held block.",
        };
        
        public static Brush Process(BrushArgs args) {
            return ProcessReplace(args, false);
        }
        
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
                blocks[i].Block = Block.Zero;
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
        
        public override byte NextBlock(DrawOp op) {
            ushort x = op.Coords.X, y = op.Coords.Y, z = op.Coords.Z;
            byte tile = op.Level.GetTile(x, y, z), extTile = 0;
            if (tile == Block.custom_block) extTile = op.Level.GetExtTile(x, y, z);
            
            for (int i = 0; i < include.Length; i++) {
                ExtBlock block = include[i];
                if (tile == block.Block && (tile != Block.custom_block || extTile == block.Ext))
                    return target.Block;
            }
            return Block.Zero;
        }
        
        public override byte NextExtBlock(DrawOp op) {
            return target.Ext;
        }
    }
    
    public sealed class ReplaceNotBrush : Brush {
        readonly ExtBlock[] exclude;
        readonly ExtBlock target;
        
        public ReplaceNotBrush(ExtBlock[] include, ExtBlock target) {
            this.exclude = include; this.target = target;
        }
        
        public override string Name { get { return "ReplaceNot"; } }
        
        public override string[] Help { get { return HelpString; } }
        
        public static string[] HelpString = new [] {
            "%TArguments: [block1] [block2].. [new]",
            "%HDraws by replacing existing blocks that not are in the given [blocks] with [new]",
            "%H  If only [block] is given, replaces with your held block.",
        };
        
        public static Brush Process(BrushArgs args) {
            return ReplaceBrush.ProcessReplace(args, true);
        }
        
        public override byte NextBlock(DrawOp op) {
            ushort x = op.Coords.X, y = op.Coords.Y, z = op.Coords.Z;
            byte tile = op.Level.GetTile(x, y, z), extTile = 0;
            if (tile == Block.custom_block) extTile = op.Level.GetExtTile(x, y, z);
            
            for (int i = 0; i < exclude.Length; i++) {
                ExtBlock block = exclude[i];
                if (tile == block.Block && (tile != Block.custom_block || extTile == block.Ext))
                    return Block.Zero;
            }
            return target.Block;
        }
        
        public override byte NextExtBlock(DrawOp op) {
            return target.Ext;
        }
    }
}
