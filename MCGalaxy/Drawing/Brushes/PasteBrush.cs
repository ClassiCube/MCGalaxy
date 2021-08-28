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

namespace MCGalaxy.Drawing.Brushes {
    public sealed class SimplePasteBrush : Brush {
        readonly CopyState state;
        
        public SimplePasteBrush(CopyState state) { this.state = state; }
        
        public override string Name { get { return "Paste"; } }

        public override void Configure(DrawOp op, Player p) {
            op.Flags = BlockDBFlags.Pasted;
        }
        
        public override BlockID NextBlock(DrawOp op) {
            // Figure out local coords for this block
            int x = (op.Coords.X - op.Min.X) % state.Width;
            if (x < 0) x += state.Width;
            int y = (op.Coords.Y - op.Min.Y) % state.Height;
            if (y < 0) y += state.Height;
            int z = (op.Coords.Z - op.Min.Z) % state.Length;
            if (z < 0) z += state.Length;
            
            int index = (y * state.Length + z) * state.Width + x;
            return state.Get(index);
        }
    }
    
	// TODO merge with SimplePasteBrush
    public sealed class PasteBrush : Brush {
        readonly CopyState state;
        public BlockID[] Include;
        
        public PasteBrush(CopyState state) { this.state = state; }
        
        public override string Name { get { return "Paste"; } }

        public override void Configure(DrawOp op, Player p) {
            op.Flags = BlockDBFlags.Pasted;
        }
        
        public override BlockID NextBlock(DrawOp op) {
            // Figure out local coords for this block
            int x = (op.Coords.X - op.Min.X) % state.Width;
            if (x < 0) x += state.Width;
            int y = (op.Coords.Y - op.Min.Y) % state.Height;
            if (y < 0) y += state.Height;
            int z = (op.Coords.Z - op.Min.Z) % state.Length;
            if (z < 0) z += state.Length;
            
            int index = (y * state.Length + z) * state.Width + x;
            BlockID block = state.Get(index);
            for (int i = 0; i < Include.Length; i++) {
                if (block == Include[i]) return block;
            }
            return Block.Invalid;
        }
    }
    
    
    public sealed class PasteNotBrush : Brush {
        readonly CopyState state;
        public BlockID[] Exclude;
        
        public PasteNotBrush(CopyState state) { this.state = state; }
        
        public override string Name { get { return "Paste"; } }

        public override void Configure(DrawOp op, Player p) {
            op.Flags = BlockDBFlags.Pasted;
        }
        
        public override BlockID NextBlock(DrawOp op) {
            // Figure out local coords for this block
            int x = (op.Coords.X - op.Min.X) % state.Width;
            if (x < 0) x += state.Width;
            int y = (op.Coords.Y - op.Min.Y) % state.Height;
            if (y < 0) y += state.Height;
            int z = (op.Coords.Z - op.Min.Z) % state.Length;
            if (z < 0) z += state.Length;
            
            int index = (y * state.Length + z) * state.Width + x;
            BlockID block = state.Get(index);
            for (int i = 0; i < Exclude.Length; i++) {
                if (block == Exclude[i]) return Block.Invalid;
            }
            return block;
        }
    }
}
