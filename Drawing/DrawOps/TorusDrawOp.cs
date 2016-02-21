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

    public class TorusDrawOp : DrawOp {
        
        public override string Name { get { return "Torus"; } }
        
        public override int GetBlocksAffected(Level lvl, ushort x1, ushort y1, ushort z1, ushort x2, ushort y2, ushort z2) {
            double rx = (x2 - x1) / 2.0 + 0.25, ry = (y2 - y1) / 2.0 + 0.25, rz = (z2 - z1) / 2.0 + 0.25;
            double rTube = ry, rCentre = Math.Min(rx, rz) - rTube;
            return (int)(2 * Math.PI * Math.PI * rTube * rTube * rCentre);
        }
        
        public override void Perform(ushort x1, ushort y1, ushort z1, ushort x2,
                                     ushort y2, ushort z2, Player p, Level lvl, Brush brush) {
            double cx = (x1 + x2) / 2.0, cy = (y1 + y2) / 2.0, cz = (z1 + z2) / 2.0;
            double rx = (x2 - x1) / 2.0 + 0.25, ry = (y2 - y1) / 2.0 + 0.25, rz = (z2 - z1) / 2.0 + 0.25;
            double rTube = ry, rCentre = Math.Min(rx, rz) - rTube;
            
            for (ushort yy = y1; yy <= y2; yy++)
                for (ushort zz = z1; zz <= z2; zz++)
                    for (ushort xx = x1; xx <= x2; xx++)
            {
            	double dx = xx - cx, dy = yy - cy, dz = zz - cz;
                dx *= dx; dy *= dy; dz *= dz;
                double dInner = rCentre - Math.Sqrt( dx + dz );
                
                if (dInner * dInner + dy <= rTube * rTube * 0.5)
                    PlaceBlock(p, lvl, xx, yy, zz, brush);
            }
        }
    }
}
