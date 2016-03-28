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
//Full use to all StormCom Server System codes (in regards to minecraft classic) have been granted to MCGalaxy without restriction.
//
// ~Merlin33069
using System;
using MCGalaxy.Drawing.Brushes;

namespace MCGalaxy.Drawing.Ops {

    public class AdvPyramidDrawOp : AdvDrawOp {
        
        public override string Name { get { return "Adv Pyramid"; } }
        
        public override int GetBlocksAffected(Level lvl, Vec3U16[] marks) {
            int R = Radius, H = Height;
            return (R * R * H) / 3;
        }
        
        public override void Perform(Vec3U16[] marks, Player p, Level lvl, Brush brush) {
            Vec3U16 P = marks[0];
            for (short yy = 0; yy <= Height; yy++)
                for (short zz = (short)-Radius; zz <= Radius; zz++)
                    for (short xx = (short)-Radius; xx <= Radius; xx++)
            {
                int curHeight = Invert ? yy : Height - yy;
                if (curHeight == 0) continue;
                int cx = P.X + xx, cy = P.Y + + (Height - curHeight), cz = P.Z + zz;                
                
                double curRadius = Radius * ((double)curHeight / (double)Height);
                if (Math.Abs(xx) > curRadius || Math.Abs(zz) > curRadius)
                    continue;
                byte ctile = p.level.GetTile((ushort)cx, (ushort)cy, (ushort)cz);
                if (ctile != 0) continue;
                PlaceBlock(p, lvl, (ushort)cx, (ushort)cy, (ushort)cz, brush);
            }
        }
    }
    
    public class AdvHollowPyramidDrawOp : AdvDrawOp {
        
        public override string Name { get { return "Adv Hollow Pyramid"; } }
        
        public override int GetBlocksAffected(Level lvl, Vec3U16[] marks) {
            int R = Radius, H = Height;
            int outer = (R * R * H) / 3;
            int inner = ((R - 1) * (R - 1) * (H - 1)) / 3;
            return outer - inner;
        }
        
        public override void Perform(Vec3U16[] marks, Player p, Level lvl, Brush brush) {
            Vec3U16 P = marks[0];
            for (short yy = 0; yy <= Height; yy++)
                for (short zz = (short)-Radius; zz <= Radius; zz++)
                    for (short xx = (short)-Radius; xx <= Radius; xx++)
            {
                int curHeight = Invert ? yy : Height - yy;
                if (curHeight == 0) continue;
                int cx = P.X + xx, cy = P.Y + (Height - curHeight), cz = P.Z + zz;                
                
                double curRadius = Radius * ((double)curHeight / (double)Height);
                int absx = Math.Abs(xx), absz = Math.Abs(zz);
                if (absx > curRadius || absz > curRadius) continue;
                if (absx < (curRadius - 1) && absz < (curRadius - 1)) continue;

                byte ctile = p.level.GetTile((ushort)cx, (ushort)cy, (ushort)cz);
                if (ctile != 0) continue;
                PlaceBlock(p, lvl, (ushort)cx, (ushort)cy, (ushort)cz, brush);
            }
        }
    }
}