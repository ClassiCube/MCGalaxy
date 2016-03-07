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
using MCGalaxy.Drawing.Brushes;

namespace MCGalaxy.Drawing.Ops {
    
    public class ReplaceDrawOp : DrawOp {
        
        public ExtBlock[] ToReplace;
        public ExtBlock Target;
        
        public override string Name { get { return "Replace"; } }
        
        public override int GetBlocksAffected(Level lvl, ushort x1, ushort y1, ushort z1, ushort x2, ushort y2, ushort z2) {
            return (x2 - x1 + 1) * (y2 - y1 + 1) * (z2 - z1 + 1);
        }
        
        public override void Perform(ushort x1, ushort y1, ushort z1, ushort x2,
                                     ushort y2, ushort z2, Player p, Level lvl, Brush brush) {
            ExtBlock[] toReplace = ToReplace;
            ExtBlock target = Target;
            for (ushort y = y1; y <= y2; y++)
                for (ushort z = z1; z <= z2; z++)
                    for (ushort x = x1; x <= x2; x++)
            {
                byte tile = lvl.GetTile(x, y, z), extTile = 0;
                if (tile == Block.custom_block) extTile = lvl.GetExtTile(x, y, z);
                
                for (int i = 0; i < toReplace.Length; i++) {
                    ExtBlock block = toReplace[i];
                    if (tile == block.Type && (tile != Block.custom_block || extTile == block.ExtType)) {
                        PlaceBlock(p, lvl, x, y, z, target.Type, target.ExtType); break;
                    }
                }
            }
        }
    }
    
    public class ReplaceNotDrawOp : DrawOp {
        
        public ExtBlock[] ToReplace;
        public ExtBlock Target;
        
        public override string Name { get { return "ReplaceNot"; } }
        
        public override int GetBlocksAffected(Level lvl, ushort x1, ushort y1, ushort z1, ushort x2, ushort y2, ushort z2) {
            return (x2 - x1 + 1) * (y2 - y1 + 1) * (z2 - z1 + 1);
        }
        
        public override void Perform(ushort x1, ushort y1, ushort z1, ushort x2,
                                     ushort y2, ushort z2, Player p, Level lvl, Brush brush) {
            ExtBlock[] toReplace = ToReplace;
            ExtBlock target = Target;
            for (ushort y = y1; y <= y2; y++)
                for (ushort z = z1; z <= z2; z++)
                    for (ushort x = x1; x <= x2; x++)
            {
                byte tile = lvl.GetTile(x, y, z), extTile = 0;
                if (tile == Block.custom_block) extTile = lvl.GetExtTile(x, y, z);
                
                bool place = true;
                for (int i = 0; i < toReplace.Length; i++) {
                    ExtBlock block = toReplace[i];
                    if (tile == block.Type || (tile == Block.custom_block && extTile == block.ExtType)) {
                        place = false; break;
                    }
                }
                if (place)
                    PlaceBlock(p, lvl, x, y, z, target.Type, target.ExtType);
            }
        }
    }
}
