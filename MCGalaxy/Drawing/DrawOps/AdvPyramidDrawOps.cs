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
//StormCom Object Generator
//
//Full use to all StormCom Server System codes (in regards to minecraft classic) have been granted to MCForge without restriction.
//
// ~Merlin33069
using System;
using MCGalaxy.Drawing.Brushes;
using MCGalaxy.Maths;

namespace MCGalaxy.Drawing.Ops {
    public class AdvPyramidDrawOp : AdvDrawOp {        
        public override string Name { get { return "Adv Pyramid"; } }
        public AdvPyramidDrawOp(bool invert = false) { Invert = invert; }
        
        public override long BlocksAffected(Level lvl, Vec3S32[] marks) {
            long R = Radius, H = Max.Y - Min.Y;
            return (R * R * H) / 3;
        }
        
        public override void Perform(Vec3S32[] marks, Brush brush, DrawOpOutput output) {
            Vec3U16 p1 = Clamp(Min), p2 = Clamp(Max);
            Vec3S32 C = (Min + Max) / 2;

            for (ushort y = p1.Y; y <= p2.Y; y++)
                for (ushort z = p1.Z; z <= p2.Z; z++)
                    for (ushort x = p1.X; x <= p2.X; x++)
            {
                int height = Max.Y - Min.Y;
                int xx = C.X - x, yy = y - Min.Y, zz = C.Z - z;
                int curHeight = Invert ? yy : height - yy;
                if (curHeight == 0) continue;              
                
                double curRadius = Radius * ((double)curHeight / (double)height);
                if (Math.Abs(xx) > curRadius || Math.Abs(zz) > curRadius) continue;
                
                if (Level.IsAirAt(x, y, z)) output(Place(x, y, z, brush));
            }
        }
    }
    
    public class AdvHollowPyramidDrawOp : AdvDrawOp {      
        public override string Name { get { return "Adv Hollow Pyramid"; } }
        public AdvHollowPyramidDrawOp(bool invert = false) { Invert = invert; }
        
        public override long BlocksAffected(Level lvl, Vec3S32[] marks) {
            long R = Radius, H = Max.Y - Min.Y;
            long outer = (R * R * H) / 3;
            long inner = ((R - 1) * (R - 1) * (H - 1)) / 3;
            return outer - inner;
        }
        
        public override void Perform(Vec3S32[] marks, Brush brush, DrawOpOutput output) {
            Vec3U16 p1 = Clamp(Min), p2 = Clamp(Max);
            Vec3S32 C = (Min + Max) / 2;
            int height = Max.Y - Min.Y;

            for (ushort y = p1.Y; y <= p2.Y; y++)
                for (ushort z = p1.Z; z <= p2.Z; z++)
                    for (ushort x = p1.X; x <= p2.X; x++)
            {
                int xx = C.X - x, yy = y - Min.Y, zz = C.Z - z;
                int curHeight = Invert ? yy : height - yy;
                if (curHeight == 0) continue;           
                
                double curRadius = Radius * ((double)curHeight / (double)height);
                int absx = Math.Abs(xx), absz = Math.Abs(zz);
                if (absx > curRadius || absz > curRadius) continue;
                if (absx < (curRadius - 1) && absz < (curRadius - 1)) continue;

                if (Level.IsAirAt(x, y, z)) output(Place(x, y, z, brush));
            }
        }
    }
}