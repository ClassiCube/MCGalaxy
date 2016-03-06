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

namespace MCGalaxy.Drawing.Ops {
    
    public class SimplePasteDrawOp : DrawOp {
        
        public CopyState CopyState;
        
        public override string Name { get { return "Simple paste"; } }
        
        public override bool MinMaxCoords { get { return false; } }
        
        public override int GetBlocksAffected(Level lvl, ushort x1, ushort y1, ushort z1, ushort x2, ushort y2, ushort z2) {
            return CopyState.Volume;
        }
        
        public override void Perform(ushort x1, ushort y1, ushort z1, ushort x2,
                                     ushort y2, ushort z2, Player p, Level lvl, Brush brush) {
            CopyState state = CopyState;
            bool pasteAir = state.PasteAir;
            // Adjust for the fact that paste origin may be outside the map.
            short offX = (short)x1, offY = (short)y1, offZ = (short)z1;
            
            for (int i = 0; i < state.Blocks.Length; i++ ) {
                ushort locX, locY, locZ;
                byte b = state.Blocks[i], extB = state.ExtBlocks[i];
                state.GetCoords(i, out locX, out locY, out locZ);
                
                ushort x = (ushort)(locX + offX), y = (ushort)(locY + offY), z = (ushort)(locZ + offZ);
                byte type = p.level.GetTile(x, y, z), extType = 0;
                if (type == Block.custom_block) extType = p.level.GetExtTile(x, y, z);
                
                bool place = lvl.InBound(x, y, z) && (b != type || (b == Block.custom_block && extB != extType));
                if ((b != Block.air || pasteAir) && place)
                    PlaceBlock(p, lvl, x, y, z, b, extB);
            }
        }
    }
    
    public class PasteDrawOp : DrawOp {
        
        public CopyState CopyState;
        public ExtBlock[] Inclusive, Exclusive;
        
        public override string Name { get { return "Paste"; } }
        
        public override bool MinMaxCoords { get { return false; } }
        
        public override int GetBlocksAffected(Level lvl, ushort x1, ushort y1, ushort z1, ushort x2, ushort y2, ushort z2) {
            return CopyState.Volume;
        }
        
        public override void Perform(ushort x1, ushort y1, ushort z1, ushort x2,
                                     ushort y2, ushort z2, Player p, Level lvl, Brush brush) {
            CopyState state = CopyState;
            bool pasteAir = state.PasteAir;
            ExtBlock[] include = Inclusive, exclude = Exclusive;
            // Adjust for the fact that paste origin may be outside the map.
            short offX = (short)x1, offY = (short)y1, offZ = (short)z1;
            
            for (int i = 0; i < state.Blocks.Length; i++ ) {
                ushort locX, locY, locZ;
                byte b = state.Blocks[i], extB = state.ExtBlocks[i];
                state.GetCoords(i, out locX, out locY, out locZ);
                
                ushort x = (ushort)(locX + offX), y = (ushort)(locY + offY), z = (ushort)(locZ + offZ);
                byte type = p.level.GetTile(x, y, z), extType = 0;
                if (type == Block.custom_block) extType = p.level.GetExtTile(x, y, z);
                
                bool place = lvl.InBound(x, y, z) && (b != type || (b == Block.custom_block && extB != extType));
                if (!place || (b == Block.air && !pasteAir)) continue;
                
                if (exclude != null) {
                    for (int j = 0; j < exclude.Length; j++) {
                        ExtBlock block = exclude[i];
                        if (b == block.Type || (b == Block.custom_block && extB == block.ExtType)) {
                            place = false; break;
                        }
                    }
                    if (!place) continue;
                    PlaceBlock(p, lvl, x, y, z, b, extB);
                }
                
                if (include != null) {
                    for (int j = 0; j < include.Length; j++) {
                        ExtBlock block = include[j];
                        if (b == block.Type || (b == Block.custom_block && extB == block.ExtType)) {
                            PlaceBlock(p, lvl, x, y, z, b, extB); break;
                        }
                    }
                }
            }
        }
    }
}
