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
    public class TorusDrawOp : DrawOp {      
        public override string Name { get { return "Torus"; } }
        
        public override long BlocksAffected(Level lvl, Vec3S32[] marks) {
            double rx = (Max.X - Min.X) / 2.0 + 0.25, ry = (Max.Y - Min.Y) / 2.0 + 0.25, rz = (Max.Z - Min.Z) / 2.0 + 0.25;
            double rTube = ry, rCentre = Math.Min(rx, rz) - rTube;
            return (int)(2 * Math.PI * Math.PI * rTube * rTube * rCentre);
        }
        
        public override void Perform(Vec3S32[] marks, Player p, Level lvl, Brush brush, Action<DrawOpBlock> output) {          
            double cx = (Min.X + Max.X) / 2.0, cy = (Min.Y + Max.Y) / 2.0, cz = (Min.Z + Max.Z) / 2.0;
            double rx = (Max.X - Min.X) / 2.0 + 0.25, ry = (Max.Y - Min.Y) / 2.0 + 0.25, rz = (Max.Z - Min.Z) / 2.0 + 0.25;
            double rTube = ry, rCentre = Math.Min(rx, rz) - rTube;
            Vec3U16 p1 = Clamp(Min), p2 = Clamp(Max);
            
            for (ushort yy = p1.Y; yy <= p2.Y; yy++)
                for (ushort zz = p1.Z; zz <= p2.Z; zz++)
                    for (ushort xx = p1.X; xx <= p2.X; xx++)
            {
                double dx = xx - cx, dy = yy - cy, dz = zz - cz;
                dx *= dx; dy *= dy; dz *= dz;
                double dInner = rCentre - Math.Sqrt( dx + dz );
                
                if (dInner * dInner + dy <= rTube * rTube * 0.5 + 0.25)
                	output(Place(xx, yy, zz, brush));
            }
        }
    }
}
