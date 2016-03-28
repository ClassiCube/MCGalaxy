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

    public class AdvConeDrawOp : AdvDrawOp {
        
        public override string Name { get { return "Adv Cone"; } }
        
        public override int GetBlocksAffected(Level lvl, Vec3U16[] marks) {
            int R = Radius, H = Height;
            return (int)(Math.PI / 3 * (R * R * H));
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
                int dist = xx * xx + zz * zz;
                if (dist > curRadius * curRadius) continue;
                
                byte ctile = p.level.GetTile((ushort)cx, (ushort)cy, (ushort)cz);
                if (ctile != 0) continue;
                PlaceBlock(p, lvl, (ushort)cx, (ushort)cy, (ushort)cz, brush);
            }
        }
    }
    
    public class AdvHollowConeDrawOp : AdvDrawOp {
        
        public override string Name { get { return "Adv Hollow Cone"; } }
        
        public override int GetBlocksAffected(Level lvl, Vec3U16[] marks) {
            int R = Radius, H = Height;
            double outer = (int)(Math.PI / 3 * (R * R * H));
            double inner = (int)(Math.PI / 3 * ((R - 1) * (R - 1) * (H - 1)));
            return (int)(outer - inner);
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
                int dist = xx * xx + zz * zz;
                if (dist > curRadius * curRadius || dist < (curRadius - 1) * (curRadius - 1))
                    continue;
                
                byte ctile = p.level.GetTile((ushort)cx, (ushort)cy, (ushort)cz);
                if (ctile != 0) continue;
                PlaceBlock(p, lvl, (ushort)cx, (ushort)cy, (ushort)cz, brush);
            }
        }
    }
    
    public class AdvVolcanoDrawOp : AdvDrawOp {
        
        public override string Name { get { return "Adv Volcano"; } }
        
        public override int GetBlocksAffected(Level lvl, Vec3U16[] marks) {
            int R = Radius, H = Height;
            double outer = (int)(Math.PI / 3 * (R * R * H));
            double inner = (int)(Math.PI / 3 * ((R - 1) * (R - 1) * (H - 1)));
            return (int)(outer - inner);
        }
        
        public override void Perform(Vec3U16[] marks, Player p, Level lvl, Brush brush) {
            Vec3U16 P = marks[0];
            for (short yy = 0; yy <= Height; yy++)
                for (short zz = (short)-Radius; zz <= Radius; zz++)
                    for (short xx = (short)-Radius; xx <= Radius; xx++)
            {
                int cx = (P.X + xx), cy = (P.Y + yy), cz = (P.Z + zz);
                int curHeight = Height - yy;
                if (curHeight == 0) continue;
                
                double curRadius = Radius * ((double)curHeight / (double)Height);
                int dist = xx * xx + zz * zz;
                if (dist > curRadius * curRadius)continue;
                
                byte ctile = p.level.GetTile((ushort)cx, (ushort)cy, (ushort)cz);
                if (ctile != 0) continue;
                bool layer = dist >= (curRadius - 1) * (curRadius - 1);
                byte type = layer ? Block.grass : Block.lavastill;
                PlaceBlock(p, lvl, (ushort)cx, (ushort)cy, (ushort)cz, type, 0);
            }
        }
    }
}