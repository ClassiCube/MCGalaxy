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
    
    public sealed class ReplaceBrush : Brush {
        readonly ExtBlock[] include;
        readonly ExtBlock target;
        
        public ReplaceBrush(ExtBlock[] include, ExtBlock target) {
            this.include = include; this.target = target;
        }
        
        public override string Name { get { return "Replace"; } }
        
        public static Brush Process(BrushArgs args) {
            string[] parts = args.Message.Split(' ');
            if (parts.Length < 2) {
                args.Player.SendMessage("You need to provide a target block, and at least one block to replace."); return null;
            }

            ExtBlock[] toAffect = GetBlocks(args.Player, 0, parts.Length - 1, parts);
            ExtBlock target;
            target.Type = DrawCmd.GetBlock(args.Player, parts[parts.Length - 1], out target.ExtType);
            if (target.Type == Block.Zero) return null;
            return target.Type == Block.Zero ? null : new ReplaceBrush(toAffect, target);
        }
        
        internal static ExtBlock[] GetBlocks(Player p, int start, int max, string[] parts) {
            ExtBlock[] blocks = new ExtBlock[max - start];
            for (int j = 0; j < blocks.Length; j++)
                blocks[j].Type = Block.Zero;
            for (int j = 0; start < max; start++, j++ ) {
                byte extType = 0;
                byte type = DrawCmd.GetBlock(p, parts[start], out extType);
                if (type == Block.Zero) continue;
                blocks[j].Type = type; blocks[j].ExtType = extType;
            }
            return blocks;
        }
        
        public override byte NextBlock(DrawOp op) {
            ushort x = op.Coords.X, y = op.Coords.Y, z = op.Coords.Z;
            byte tile = op.Level.GetTile(x, y, z), extTile = 0;
            if (tile == Block.custom_block) extTile = op.Level.GetExtTile(x, y, z);
            
            for (int i = 0; i < include.Length; i++) {
                ExtBlock block = include[i];
                if (tile == block.Type && (tile != Block.custom_block || extTile == block.ExtType))
                    return target.Type;
            }
            return Block.Zero;
        }
        
        public override byte NextExtBlock(DrawOp op) {
            return target.ExtType;
        }
    }
    
    public sealed class ReplaceNotBrush : Brush {
        readonly ExtBlock[] exclude;
        readonly ExtBlock target;
        
        public ReplaceNotBrush(ExtBlock[] include, ExtBlock target) {
            this.exclude = include; this.target = target;
        }
        
        public override string Name { get { return "Replace"; } }
        
        public static Brush Process(BrushArgs args) {
            string[] parts = args.Message.Split(' ');
            if (parts.Length < 2) {
                args.Player.SendMessage("You need to provide a target block, and at least one block to replace."); return null;
            }

            ExtBlock[] toAffect = ReplaceBrush.GetBlocks(args.Player, 0, parts.Length - 1, parts);
            ExtBlock target;
            target.Type = DrawCmd.GetBlock(args.Player, parts[parts.Length - 1], out target.ExtType);
            if (target.Type == Block.Zero) return null;
            return target.Type == Block.Zero ? null : new ReplaceNotBrush(toAffect, target);
        }
        
        public override byte NextBlock(DrawOp op) {
            ushort x = op.Coords.X, y = op.Coords.Y, z = op.Coords.Z;
            byte tile = op.Level.GetTile(x, y, z), extTile = 0;
            if (tile == Block.custom_block) extTile = op.Level.GetExtTile(x, y, z);
            
            for (int i = 0; i < exclude.Length; i++) {
                ExtBlock block = exclude[i];
                if (tile == block.Type && (tile != Block.custom_block || extTile == block.ExtType))
                    return Block.Zero;
            }
            return target.Type;
        }
        
        public override byte NextExtBlock(DrawOp op) {
            return target.ExtType;
        }
    }
}
