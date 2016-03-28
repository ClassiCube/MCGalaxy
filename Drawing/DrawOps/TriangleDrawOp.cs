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
    
    public class TriangleDrawOp : DrawOp {
        
        public override string Name { get { return "Triangle"; } }
        
        public override long GetBlocksAffected(Level lvl, Vec3U16[] marks) {
            // Applying Heron's Formula
            double a = (marks[0] - marks[2]).Length;
            double b = (marks[1] - marks[2]).Length;
            double c = (marks[0] - marks[1]).Length;
            double s = (a + b + c) / 2;
            return (int)Math.Sqrt(s * (s - a) * (s - b) * (s - c));
        }
        
        public override void Perform(Vec3U16[] marks, Player p, Level lvl, Brush brush) {
        	Vec3U16 p1 = Min, p2 = Max, a = marks[0];
            Vec3S16 v0 = marks[1] - a, v1 = marks[2] - a;
            float d00 = v0.Dot(v0), d01 = v0.Dot(v1), d11 = v1.Dot(v1);
            float invDenom = 1f / (d00 * d11 - d01 * d01);
            
            for (ushort yy = p1.Y; yy <= p2.Y; yy++)
                for (ushort zz = p1.Z; zz <= p2.Z; zz++)
                    for (ushort xx = p1.X; xx <= p2.X; xx++)
            {
                // Compute the barycentric coordinates of the point
                Vec3S16 v2 = new Vec3U16(xx, yy, zz) - a;
                float d20 = v2.Dot(v0), d21 = v2.Dot(v1);                
                float v = (d11 * d20 - d01 * d21) * invDenom;
                float w = (d00 * d21 - d01 * d20) * invDenom;
                float u = 1.0f - v - w;
                
                if (u >= 0 && u <= 1 && v >= 0 && v <= 1 && w >= 0 && w <= 1)
                    PlaceBlock(p, lvl, xx, yy, zz, brush);
            }
        }
    }
}
