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
using MCGalaxy.Maths;

namespace MCGalaxy.Drawing.Ops {    
    public class TriangleDrawOp : DrawOp {        
        public override string Name { get { return "Triangle"; } }
        
        public override long BlocksAffected(Level lvl, Vec3S32[] marks) {
            // Applying Heron's Formula
            float a = (marks[0] - marks[2]).Length;
            float b = (marks[1] - marks[2]).Length;
            float c = (marks[0] - marks[1]).Length;
            float s = (a + b + c) / 2;
            return (int)Math.Sqrt(s * (s - a) * (s - b) * (s - c));
        }
        
        public override void Perform(Vec3S32[] marks, Brush brush, DrawOpOutput output) {
            Vec3F32 V1 = marks[0], V2 = marks[1], V3 = marks[2];
            Vec3F32 N = Vec3F32.Cross(V2 - V1, V3 - V1);
            N = Vec3F32.Normalise(N);
            Vec3U16 min = Clamp(Min), max = Clamp(Max);
            
            for (ushort yy = min.Y; yy <= max.Y; yy++)
                for (ushort zz = min.Z; zz <= max.Z; zz++)
                    for (ushort xx = min.X; xx <= max.X; xx++)
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

                if (u >= 0 && v >= 0 && u + v <= 1) {
                    output(Place(xx, yy, zz, brush));
                } else if (Axis(P, V1, V2) || Axis(P, V1, V3) || Axis(P, V2, V3)) {
                    output(Place(xx, yy, zz, brush));
                }
            }
        }
        
        bool Axis(Vec3F32 P, Vec3F32 P1, Vec3F32 P2) {
            // Point to line segment test
            float bottom = (P2 - P1).LengthSquared;
            if (bottom == 0) return (P1 - P).Length <= 0.5f;

            float t = Vec3F32.Dot(P2 - P1, P - P1) / bottom;
            if (t < 0 || t > 1) return false;
            Vec3F32 proj = P1 + t * (P2 - P1);
            return (P - proj).Length <= 0.5f;
        }
    }
}
