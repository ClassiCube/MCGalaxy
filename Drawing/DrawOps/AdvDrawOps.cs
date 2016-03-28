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
    
    public abstract class AdvDrawOp : DrawOp {
        public int Radius, Height;
        public bool Invert;
    }
    
    public class AdvSphereDrawOp : AdvDrawOp {
        
        public override string Name { get { return "Adv Sphere"; } }
        
        public override int GetBlocksAffected(Level lvl, Vec3U16[] marks) {
            int R = Radius;
            return (int)(Math.PI * 4.0 / 3.0 * (R * R * R));
        }
        
        public override void Perform(Vec3U16[] marks, Player p, Level lvl, Brush brush) {
            Vec3U16 P = marks[0];
            int upper = (Radius + 1) * (Radius + 1);
            for (short yy = (short)-Radius; yy <= Radius; yy++)
                for (short zz = (short)-Radius; zz <= Radius; zz++)
                    for (short xx = (short)-Radius; xx <= Radius; xx++)
            {
                int curDist = xx * xx + yy * yy + zz * zz;
                if (curDist < upper)
                    PlaceBlock(p, lvl, (ushort)(P.X + xx), (ushort)(P.Y + yy), (ushort)(P.Z + zz), brush);
            }
        }
    }
    
    public class AdvHollowSphereDrawOp : DrawOp {
        
        public int Radius;
        public override string Name { get { return "Adv Hollow Sphere"; } }
        
        public override int GetBlocksAffected(Level lvl, Vec3U16[] marks) {
            int R = Radius;
            double outer = (int)(Math.PI * 4.0 / 3.0 * (R * R * R));
            double inner = (int)(Math.PI * 4.0 / 3.0 * ((R - 1) * (R - 1) * (R - 1)));
            return (int)(outer - inner);
        }
        
        public override void Perform(Vec3U16[] marks, Player p, Level lvl, Brush brush) {
            Vec3U16 P = marks[0];
            int upper = (Radius + 1) * (Radius + 1), inner = (Radius - 1) * (Radius - 1);
            for (short yy = (short)-Radius; yy <= Radius; yy++)
                for (short zz = (short)-Radius; zz <= Radius; zz++)
                    for (short xx = (short)-Radius; xx <= Radius; xx++)
            {
                int curDist = xx * xx + yy * yy + zz * zz;
                if (curDist < upper && curDist >= inner) {
                    PlaceBlock(p, lvl, (ushort)(P.X + xx), (ushort)(P.Y + yy), (ushort)(P.Z + zz), brush);
                }
            }
        }
    }
}