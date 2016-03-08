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
    
    public sealed class PasteBrush : Brush {
        readonly CopyState state;
        
        public PasteBrush(CopyState state) {
            this.state = state;
        }
        
        public static Brush Process(BrushArgs args) {
            if (args.Player.CopyBuffer == null) {
                args.Player.SendMessage("You haven't copied anything yet.");
                return null;
            }
            return new PasteBrush(args.Player.CopyBuffer);
        }
        
        public override byte NextBlock(DrawOp op) { 
            Vector3U16 p = LocalCoords(op);
            return state.Blocks[state.GetIndex(p.X, p.Y, p.Z)];
        }
        
        public override byte NextExtBlock(DrawOp op) {
            Vector3U16 p = LocalCoords(op);
            return state.ExtBlocks[state.GetIndex(p.X, p.Y, p.Z)];
        }
        
        Vector3U16 LocalCoords(DrawOp op) {
            int x = (op.Coords.X - op.Min.X) % state.Width;
            if (x < 0) x += state.Width;
            int y = (op.Coords.Y - op.Min.Y) % state.Height;
            if (y < 0) y += state.Height;
            int z = (op.Coords.Z - op.Min.Z) % state.Length;
            if (z < 0) z += state.Length;
            return new Vector3U16((ushort)x, (ushort)y, (ushort)z);
        }       
    }
}
