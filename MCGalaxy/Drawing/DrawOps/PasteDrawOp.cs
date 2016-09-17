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
using MCGalaxy.Drawing.Brushes;

namespace MCGalaxy.Drawing.Ops {    
    public class SimplePasteDrawOp : DrawOp {
        public override string Name { get { return "Paste"; } }        
        public CopyState CopyState;
        
        public override long BlocksAffected(Level lvl, Vec3S32[] marks) {
            return CopyState.UsedBlocks;
        }
        
        public override void SetMarks(Vec3S32[] m) {
            Origin = m[0];
            CopyState cState = CopyState;
            if (cState.X != cState.OriginX) m[0].X -= (cState.Width - 1);
            if (cState.Y != cState.OriginY) m[0].Y -= (cState.Height - 1);
            if (cState.Z != cState.OriginZ) m[0].Z -= (cState.Length - 1);
            
            Min = m[0]; Max = m[0];
            Max.X += cState.Width; Max.Y += cState.Height; Max.Z += cState.Length;
        }
        
        public override void Perform(Vec3S32[] marks, Player p, Level lvl, Brush brush, Action<DrawOpBlock> output) {
            CopyState state = CopyState;
            bool pasteAir = state.PasteAir;
            // Adjust for the fact that paste origin may be outside the map.
            int x1 = marks[0].X, y1 = marks[0].Y, z1 = marks[0].Z;
            
            for (int i = 0; i < state.Blocks.Length; i++ ) {
                ushort locX, locY, locZ;
                byte b = state.Blocks[i], extB = state.ExtBlocks[i];
                state.GetCoords(i, out locX, out locY, out locZ);
                
                ushort x = (ushort)(locX + x1), y = (ushort)(locY + y1), z = (ushort)(locZ + z1);
                if ((b != Block.air || pasteAir) && lvl.InBound(x, y, z))
                    output(Place(x, y, z, b, extB));
            }
        }
    }
    
    public class PasteDrawOp : SimplePasteDrawOp {
        
        public ExtBlock[] Include, Exclude;
        
        public override string Name { get { return "Paste"; } }
        
        public override void Perform(Vec3S32[] marks, Player p, Level lvl, Brush brush, Action<DrawOpBlock> output) {
            CopyState state = CopyState;
            bool pasteAir = state.PasteAir;
            ExtBlock[] include = Include, exclude = Exclude;
            // Adjust for the fact that paste origin may be outside the map.
            int x1 = marks[0].X, y1 = marks[0].Y, z1 = marks[0].Z;
            
            for (int i = 0; i < state.Blocks.Length; i++ ) {
                ushort locX, locY, locZ;
                byte b = state.Blocks[i], extB = state.ExtBlocks[i];
                state.GetCoords(i, out locX, out locY, out locZ);
                
                ushort x = (ushort)(locX + x1), y = (ushort)(locY + y1), z = (ushort)(locZ + z1);
                if ((b == Block.air && !pasteAir) || !lvl.InBound(x, y, z)) continue;
                
                if (exclude != null) {
                    bool place = true;
                    for (int j = 0; j < exclude.Length; j++) {
                        ExtBlock block = exclude[j];
                        if (b == block.Block && (b != Block.custom_block || extB == block.Ext)) {
                            place = false; break;
                        }
                    }
                    if (!place) continue;
                    output(Place(x, y, z, b, extB));
                }
                
                if (include != null) {
                    for (int j = 0; j < include.Length; j++) {
                        ExtBlock block = include[j];
                        if (b == block.Block && (b != Block.custom_block || extB == block.Ext)) {
                            output(Place(x, y, z, b, extB)); break;
                        }
                    }
                }
            }
        }
    }
}
