/*
    Copyright 2011 MCForge
        
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
using MCGalaxy.Maths;

namespace MCGalaxy.Drawing.Ops { 
    public class SPlaceDrawOp : DrawOp {
        public override string Name { get { return "SPlace"; } }
        public int Distance, Interval;
        
        public override long BlocksAffected(Level lvl, Vec3S32[] marks) {
            if (Interval <= 0) return 2;
            
            // divide Distance by Interval rounding up
            return 1 + (Distance + (Interval - 1)) / Interval;
        }
        
        public override void Perform(Vec3S32[] m, Brush brush, DrawOpOutput output) {
            int dirX = 0, dirY = 0, dirZ = 0;
            int dx = Math.Abs(m[1].X - m[0].X), dy = Math.Abs(m[1].Y - m[0].Y), dz = Math.Abs(m[1].Z - m[0].Z);
            if (dy > dx && dy > dz) {
                dirY = m[1].Y > m[0].Y ? 1 : -1;
            } else if (dx > dz) {
                dirX = m[1].X > m[0].X ? 1 : -1;
            } else {
                dirZ = m[1].Z > m[0].Z ? 1 : -1;
            } 
            
            ushort endX = (ushort)(m[0].X + dirX * Distance);
            ushort endY = (ushort)(m[0].Y + dirY * Distance);
            ushort endZ = (ushort)(m[0].Z + dirZ * Distance);            
            output(Place(endX, endY, endZ, brush));
            
            if (Interval > 0) {
                int x = m[0].X, y = m[0].Y, z = m[0].Z;
                int delta = 0;
                while (Level.IsValidPos(x, y, z) && delta < Distance) {
                    output(Place((ushort)x, (ushort)y, (ushort)z, brush));
                    x += dirX * Interval; y += dirY * Interval; z += dirZ * Interval;
                    delta += Interval;
                }
            } else {
                output(Place((ushort)m[0].X, (ushort)m[0].Y, (ushort)m[0].Z, brush));
            }
        }
    }
}
