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
            float a = (marks[0] - marks[2]).Length;
            float b = (marks[1] - marks[2]).Length;
            float c = (marks[0] - marks[1]).Length;
            float s = (a + b + c) / 2;
            return (int)Math.Sqrt(s * (s - a) * (s - b) * (s - c));
        }
        
        public override void Perform(Vec3U16[] marks, Player p, Level lvl, Brush brush) {
            Vec3F32 V1 = marks[0], V2 = marks[1], V3 = marks[2];
            Vec3F32 N = Vec3F32.Cross(V2 - V1, V3 - V1);
            N = Vec3F32.Normalise(N);
            
            for (ushort yy = Min.Y; yy <= Max.Y; yy++)
                for (ushort zz = Min.Z; zz <= Max.Z; zz++)
                    for (ushort xx = Min.X; xx <= Max.X; xx++)
            {
                // Project point onto the plane
                Vec3F32 P = new Vec3F32(xx, yy, zz);
                float t = Vec3F32.Dot(N, V1) - Vec3F32.Dot(N, P);
                Vec3F32 P0 = P + t * N;
                float dist = (P - P0).Length;
                if (dist > 0.5) continue;
                
                // Check if inside the triangle
                Vec3F32 v0 = V3 - V1, v1 = V2 - V1, v2 = P0 - V1;
                float dot00 = Vec3F32.Dot(v0, v0);
                float dot01 = Vec3F32.Dot(v0, v1);
                float dot02 = Vec3F32.Dot(v0, v2);
                float dot11 = Vec3F32.Dot(v1, v1);
                float dot12 = Vec3F32.Dot(v1, v2);

                // Compute barycentric coordinates
                float invDenom = 1 / (dot00 * dot11 - dot01 * dot01);
                float u = (dot11 * dot02 - dot01 * dot12) * invDenom;
                float v = (dot00 * dot12 - dot01 * dot02) * invDenom;

                if (u >= 0 && v >= 0 && u + v <= 1)
                    PlaceBlock(p, lvl, xx, yy, zz, brush);
            }
        }
    }
}
