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
        
        public override byte NextExtBlock(DrawOp op) { return target.Ext; }
    }
    
    public sealed class ReplaceNotBrush : Brush {
        readonly ExtBlock[] exclude;
        readonly ExtBlock target;
        
        public ReplaceNotBrush(ExtBlock[] include, ExtBlock target) {
            this.exclude = include; this.target = target;
        }
        
        public override string Name { get { return "ReplaceNot"; } }
        
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
        
        public override byte NextExtBlock(DrawOp op) { return target.Ext; }
    }
}
