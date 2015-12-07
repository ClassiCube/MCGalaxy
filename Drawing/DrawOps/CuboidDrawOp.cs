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

namespace MCGalaxy {

    public sealed class CuboidDrawOp : DrawOp {
        
        public override string Name {
            get { return "cuboid"; }
        } 
        
        public override int GetBlocksAffected(ushort x1, ushort y1, ushort z1, ushort x2, ushort y2, ushort z2) {
            return (x2 - x1 + 1) * (y2 - y2 + 1) * (z2 - z1 + 1);
        }
        
        public override void Perform(ushort x1, ushort y1, ushort z1, ushort x2, 
                                     ushort y2, ushort z2, Player p, Level lvl, Brush brush) {
            for (ushort y = y1; y <= y2; y++)
                for (ushort z = z1; z <= z2; z++)
                    for (ushort x = x1; x <= x2; x++)
            {
                PlaceBlock(p, lvl, x, y, z, brush.NextBlock());
            }
        }
    }
}
