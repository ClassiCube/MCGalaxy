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
        
        public override void Perform(Vec3S32[] marks, Brush brush, DrawOpOutput output) {
            int steps = 20;
            Vec3F32 p0 = marks[0], p2 = marks[1], p1 = marks[2];
            steps *= (int)((p1 - p0).Length + (p1 - p2).Length);
            
            float t = 0, invT = 1, delta = 1.0f / steps;
            for (int i = 0; i <= steps; i++) {
                Vec3F32 B = invT * invT * p0 + 2 * invT * t * p1 + t * t * p2;
                output(Place(Round(B.X), Round(B.Y), Round(B.Z), brush));
                t += delta; invT -= delta;
            }
        }
        
        static ushort Round(float value) {
            int valueI = (int)value;
            int floored = value < valueI ? valueI - 1 : valueI;            
            float frac = (value % 1.0f);
            return (ushort)(floored + (frac > 0.5f ? 1 : 0));
        }
    }
}
