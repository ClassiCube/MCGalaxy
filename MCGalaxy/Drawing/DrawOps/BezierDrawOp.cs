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
using System.Collections.Generic;
using MCGalaxy.Drawing.Brushes;
using MCGalaxy.Maths;

namespace MCGalaxy.Drawing.Ops {
    public class BezierDrawOp : DrawOp {
        public override string Name { get { return "Bezier"; } }
        public bool WallsMode;
        public int MaxLength = int.MaxValue;
        
        public override long BlocksAffected(Level lvl, Vec3S32[] marks) {
            Vec3S32 p0 = marks[0], p2 = marks[1], p1 = marks[2];
            return (long)((p1 - p0).Length + (p1 - p2).Length);
        }

        static Vec3F32 offset = new Vec3F32(0.5f);
        public override void Perform(Vec3S32[] marks, Brush brush, DrawOpOutput output) {
            points.Clear();
        	
            points.Add(marks[0]);
            // Want to rasterize bezier curve from centre of 'grid'
            TesselateCurve(marks[0] + offset, marks[2] + offset, marks[1] + offset, 0);
            
            List<Vec3S32> buffer = new List<Vec3S32>();
            for (int i = 0; i < points.Count - 1; i++) {
                LineDrawOp.DrawLine(points[i].X, points[i].Y, points[i].Z, 1000000,
                                    points[i+1].X, points[i+1].Y, points[i+1].Z, buffer);
                
                foreach (Vec3S32 P in buffer) {
                    output(Place((ushort)P.X, (ushort)P.Y, (ushort)P.Z, brush));
                }
            }
        }
        
        List<Vec3S32> points = new List<Vec3S32>();
        const float objspace_flatness_squared = 0.35f * 0.35f;
        
        // Based off stbtt__tesselate_curve from https://github.com/nothings/stb/blob/master/stb_truetype.h
        void TesselateCurve(Vec3F32 p0, Vec3F32 p1, Vec3F32 p2, int n) {
            // midpoint
            Vec3F32 m;
            m.X = (p0.X + 2 * p1.X + p2.X) * 0.25f;
            m.Y = (p0.Y + 2 * p1.Y + p2.Y) * 0.25f;
            m.Z = (p0.Z + 2 * p1.Z + p2.Z) * 0.25f;
            
            // versus directly drawn line
            Vec3F32 d;
            d.X = (p0.X + p2.X) * 0.5f - m.X;
            d.Y = (p0.Y + p2.Y) * 0.5f - m.Y;
            d.Z = (p0.Z + p2.Z) * 0.5f - m.Z;
            
            if (n > 16) return; // 65536 segments on one curve better be enough!
            
            // half-pixel error allowed... need to be smaller if AA
            if (d.X * d.X + d.Y * d.Y + d.Z * d.Z > objspace_flatness_squared) { 
                Vec3F32 p0_p1 = new Vec3F32((p0.X + p1.X) * 0.5f, (p0.Y + p1.Y) * 0.5f, (p0.Z + p1.Z) * 0.5f);
                TesselateCurve(p0, p0_p1, m, n + 1);
                
                Vec3F32 p1_p2 = new Vec3F32((p1.X + p2.X) * 0.5f, (p1.Y + p2.Y) * 0.5f, (p1.Z + p2.Z) * 0.5f);
                TesselateCurve(m,  p1_p2, p2, n + 1);
            } else {
                // TODO: do we need to round properly here or not
                points.Add(new Vec3S32((int)p2.X, (int)p2.Y, (int)p2.Z));
            }
        }

        /*static ushort Round(float value) {
            int valueI = (int)value;
            int floored = value < valueI ? valueI - 1 : valueI;
            float frac = (value % 1.0f);
            return (ushort)(floored + (frac > 0.5f ? 1 : 0));
        }*/        
    }
}
