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
        
        public override long GetBlocksAffected(Level lvl, Vec3S32[] marks) {
            return CopyState.UsedBlocks;
        }
        
        public override IEnumerable<DrawOpBlock> Perform(Vec3S32[] marks, Player p, Level lvl, Brush brush) {
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
                    yield return Place(x, y, z, b, extB);
            }
        }
    }
    
    public class PasteDrawOp : DrawOp {
        
        public CopyState CopyState;
        public ExtBlock[] Include, Exclude;
        
        public override string Name { get { return "Paste"; } }
        
        public override long GetBlocksAffected(Level lvl, Vec3S32[] marks) {
            return CopyState.UsedBlocks;
        }
        
        public override IEnumerable<DrawOpBlock> Perform(Vec3S32[] marks, Player p, Level lvl, Brush brush) {
        	Vec3U16 p1 = Clamp(marks[0]);
            CopyState state = CopyState;
            bool pasteAir = state.PasteAir;
            ExtBlock[] include = Include, exclude = Exclude;
            // Adjust for the fact that paste origin may be outside the map.
            short offX = (short)p1.X, offY = (short)p1.Y, offZ = (short)p1.Z;
            
            for (int i = 0; i < state.Blocks.Length; i++ ) {
                ushort locX, locY, locZ;
                byte b = state.Blocks[i], extB = state.ExtBlocks[i];
                state.GetCoords(i, out locX, out locY, out locZ);
                
                ushort x = (ushort)(locX + offX), y = (ushort)(locY + offY), z = (ushort)(locZ + offZ);
                byte type = lvl.GetTile(x, y, z), extType = 0;
                if (type == Block.custom_block) extType = lvl.GetExtTile(x, y, z);
                
                bool place = lvl.InBound(x, y, z) && (b != type || (b == Block.custom_block && extB != extType));
                if (!place || (b == Block.air && !pasteAir)) continue;
                
                if (exclude != null) {
                    for (int j = 0; j < exclude.Length; j++) {
                        ExtBlock block = exclude[j];
                        if (b == block.Block && (b != Block.custom_block || extB == block.Ext)) {
                            place = false; break;
                        }
                    }
                    if (!place) continue;
                    yield return Place(x, y, z, b, extB);
                }
                
                if (include != null) {
                    for (int j = 0; j < include.Length; j++) {
                        ExtBlock block = include[j];
                        if (b == block.Block && (b != Block.custom_block || extB == block.Ext)) {
                            yield return Place(x, y, z, b, extB); break;
                        }
                    }
                }
            }
        }
    }
}
