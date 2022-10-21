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
using MCGalaxy.DB;
using MCGalaxy.Drawing.Ops;
using BlockID = System.UInt16;

namespace MCGalaxy.Drawing.Brushes 
{
    public class ReplaceBrushBrush : Brush 
    {
        protected readonly BlockID target;
        protected readonly Brush replacement;
        
        public ReplaceBrushBrush(BlockID include, Brush replacement) {
            this.target = include; this.replacement = replacement;
        }
        
        public override string Name { get { return "ReplaceBrush"; } }
        
        public override void Configure(DrawOp op, Player p) {
            op.Flags = BlockDBFlags.Replaced;
            replacement.Configure(op, p);
        }
        
        public override BlockID NextBlock(DrawOp op) {
            ushort x = op.Coords.X, y = op.Coords.Y, z = op.Coords.Z;
            BlockID block = op.Level.GetBlock(x, y, z); // TODO FastGetBlock
            
            if (block != target) return Block.Invalid;
            return replacement.NextBlock(op);
        }
    }
    
    public class ReplaceNotBrushBrush : ReplaceBrushBrush 
    {
        public ReplaceNotBrushBrush(BlockID exclude, Brush replacement) 
            : base(exclude, replacement) { }
        
        public override string Name { get { return "ReplaceNotBrush"; } }
        
        public override BlockID NextBlock(DrawOp op) {
            ushort x = op.Coords.X, y = op.Coords.Y, z = op.Coords.Z;
            BlockID block = op.Level.GetBlock(x, y, z); // TODO FastGetBlock
            
            if (block == target) return Block.Invalid;
            return replacement.NextBlock(op);
        }
    }
}
