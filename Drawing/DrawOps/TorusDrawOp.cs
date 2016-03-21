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

    public class TorusDrawOp : DrawOp {
        
        public override string Name { get { return "Torus"; } }
        
        public override int GetBlocksAffected(Level lvl, Vector3U16[] marks) {
            Vector3U16 p1 = marks[0], p2 = marks[1];
            double rx = (p2.X - p1.X) / 2.0 + 0.25, ry = (p2.Y - p1.Y) / 2.0 + 0.25, rz = (p2.Z - p1.Z) / 2.0 + 0.25;
            double rTube = ry, rCentre = Math.Min(rx, rz) - rTube;
            return (int)(2 * Math.PI * Math.PI * rTube * rTube * rCentre);
        }
        
        public override void Perform(Vector3U16[] marks, Player p, Level lvl, Brush brush) {
            Vector3U16 p1 = marks[0], p2 = marks[1];
            double cx = (p1.X + p2.X) / 2.0, cy = (p1.Y + p2.Y) / 2.0, cz = (p1.Z + p2.Z) / 2.0;
            double rx = (p2.X - p1.X) / 2.0 + 0.25, ry = (p2.Y - p1.Y) / 2.0 + 0.25, rz = (p2.Z - p1.Z) / 2.0 + 0.25;
            double rTube = ry, rCentre = Math.Min(rx, rz) - rTube;
            
            for (ushort yy = p1.Y; yy <= p2.Y; yy++)
                for (ushort zz = p1.Z; zz <= p2.Z; zz++)
                    for (ushort xx = p1.X; xx <= p2.X; xx++)
            {
                double dx = xx - cx, dy = yy - cy, dz = zz - cz;
                dx *= dx; dy *= dy; dz *= dz;
                double dInner = rCentre - Math.Sqrt( dx + dz );
                
                if (dInner * dInner + dy <= rTube * rTube * 0.5 + 0.25)
                    PlaceBlock(p, lvl, xx, yy, zz, brush);
            }
        }
    }
}
