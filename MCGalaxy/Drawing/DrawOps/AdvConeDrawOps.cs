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
using BlockID = System.UInt16;

namespace MCGalaxy.Drawing.Ops {
    public class AdvConeDrawOp : AdvDrawOp {
        public override string Name { get { return "Adv Cone"; } }
        public AdvConeDrawOp(bool invert = false) { Invert = invert; }
        
        public override long BlocksAffected(Level lvl, Vec3S32[] marks) {
            long R = Radius, H = Max.Y - Min.Y;
            return (long)(Math.PI / 3 * (R * R * H));
        }
        
        public override void Perform(Vec3S32[] marks, Brush brush, DrawOpOutput output) {
            Vec3U16 p1 = Clamp(Min), p2 = Clamp(Max);
            Vec3S32 C  = (Min + Max) / 2;
            int height = Max.Y - Min.Y;

            for (ushort y = p1.Y; y <= p2.Y; y++)
            {
                int dy = y - Min.Y;
                int curHeight = Invert ? dy : height - dy;
                if (curHeight == 0) continue;
                int curRadius = Radius * curHeight / height;
                
                for (ushort z = p1.Z; z <= p2.Z; z++)
                    for (ushort x = p1.X; x <= p2.X; x++)
                {
                    int dx   = C.X - x, dz = C.Z - z;
                    int dist = dx * dx + dz * dz;
                    if (dist > curRadius * curRadius) continue;
                    output(Place(x, y, z, brush));
                }
            }
        }
    }
    
    public class AdvHollowConeDrawOp : AdvDrawOp {
        public override string Name { get { return "Adv Hollow Cone"; } }
        public AdvHollowConeDrawOp(bool invert = false) { Invert = invert; }
        
        public override long BlocksAffected(Level lvl, Vec3S32[] marks) {
            long R = Radius, H = Max.Y - Min.Y;
            double outer = (int)(Math.PI / 3 * (R * R * H));
            double inner = (int)(Math.PI / 3 * ((R - 1) * (R - 1) * (H - 1)));
            return (long)(outer - inner);
        }
        
        public override void Perform(Vec3S32[] marks, Brush brush, DrawOpOutput output) {
            Vec3U16 p1 = Clamp(Min), p2 = Clamp(Max);
            Vec3S32 C  = (Min + Max) / 2;
            int height = Max.Y - Min.Y;

            for (ushort y = p1.Y; y <= p2.Y; y++)
            {
                int dy = y - Min.Y;
                int curHeight = Invert ? dy : height - dy;
                if (curHeight == 0) continue;
                
                int curRadius  = Radius * curHeight       / height;
                int curRadius2 = Radius * (curHeight - 1) / height;
                
                for (ushort z = p1.Z; z <= p2.Z; z++)
                    for (ushort x = p1.X; x <= p2.X; x++)
                {
                    int dx   = C.X - x, dz = C.Z - z;
                    int dist = dx * dx + dz * dz;
                    if (dist > curRadius * curRadius) continue;
                    
                    if (dist <= (curRadius - 1) * (curRadius - 1) &&
                        dist <= (curRadius2)    * (curRadius2)  ) continue;
                    output(Place(x, y, z, brush));
                }
            }
        }
    }
    
    public class AdvVolcanoDrawOp : AdvDrawOp {
        public override string Name { get { return "Adv Volcano"; } }
        
        public override long BlocksAffected(Level lvl, Vec3S32[] marks) {
            long R = Radius, H = Max.Y - Min.Y;
            return (long)(Math.PI / 3 * (R * R * H));
        }
        
        public override void Perform(Vec3S32[] marks, Brush brush, DrawOpOutput output) {
            Vec3U16 p1 = Clamp(Min), p2 = Clamp(Max);
            Vec3S32 C  = (Min + Max) / 2;
            int height = Max.Y - Min.Y;

            for (ushort y = p1.Y; y <= p2.Y; y++)
            {
                int yy = y - Min.Y;
                int curHeight = height - yy;
                if (curHeight == 0) continue;
                
                int curRadius  = Radius * curHeight       / height;
                int curRadius2 = Radius * (curHeight - 1) / height;
                
                for (ushort z = p1.Z; z <= p2.Z; z++)
                    for (ushort x = p1.X; x <= p2.X; x++)
                {
                    int dx   = C.X - x, dz = C.Z - z;
                    int dist = dx * dx + dz * dz;
                    if (dist > curRadius * curRadius) continue;
                    
                    bool layer = curRadius == 0 ||
                        !(dist <= (curRadius - 1) * (curRadius - 1) &&
                          dist <= (curRadius2   ) * (curRadius2   ) );

                    BlockID block = layer ? Block.Grass : Block.StillLava;
                    output(Place(x, y, z, block));
                }
            }
        }
    }
}
