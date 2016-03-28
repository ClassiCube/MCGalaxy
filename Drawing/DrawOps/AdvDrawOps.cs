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
        public ushort Radius, Height;
        public bool Invert;
        public virtual bool UsesHeight { get { return true; } }
    }
    
    public class AdvSphereDrawOp : AdvDrawOp {
        
        public override bool UsesHeight { get { return false; } }
        public override string Name { get { return "Adv Sphere"; } }
        
        public override long GetBlocksAffected(Level lvl, Vec3U16[] marks) {
            long R = Radius;
            return (long)(Math.PI * 4.0 / 3.0 * (R * R * R));
        }
        
        public override void Perform(Vec3U16[] marks, Player p, Level lvl, Brush brush) {
            Vec3U16 P = marks[0];
            int upper = (Radius + 1) * (Radius + 1);
            int minX = Math.Max(P.X - Radius, 0) - P.X, maxX = Math.Min(P.X + Radius, lvl.Width - 1) - P.X;
            int minY = Math.Max(P.Y - Radius, 0) - P.Y, maxY = Math.Min(P.Y + Radius, lvl.Height - 1) - P.Y;
            int minZ = Math.Max(P.Z - Radius, 0) - P.Z, maxZ = Math.Min(P.Z + Radius, lvl.Length - 1) - P.Z;
            for (int yy = minY; yy <= maxY; yy++)
                for (int zz = minZ; zz <= maxZ; zz++)
                    for (int xx = minX; xx <= maxX; xx++)
            {
                int curDist = xx * xx + yy * yy + zz * zz;
                if (curDist < upper)
                    PlaceBlock(p, lvl, (ushort)(P.X + xx), (ushort)(P.Y + yy), (ushort)(P.Z + zz), brush);
            }
        }
    }
    
    public class AdvHollowSphereDrawOp : AdvDrawOp {
        
        public override bool UsesHeight { get { return false; } }
        public override string Name { get { return "Adv Hollow Sphere"; } }
        
        public override long GetBlocksAffected(Level lvl, Vec3U16[] marks) {
            long R = Radius;
            double outer = (int)(Math.PI * 4.0 / 3.0 * (R * R * R));
            double inner = (int)(Math.PI * 4.0 / 3.0 * ((R - 1) * (R - 1) * (R - 1)));
            return (long)(outer - inner);
        }
        
        public override void Perform(Vec3U16[] marks, Player p, Level lvl, Brush brush) {
            Vec3U16 P = marks[0];
            int upper = (Radius + 1) * (Radius + 1), inner = (Radius - 1) * (Radius - 1);
            int minX = Math.Max(P.X - Radius, 0) - P.X, maxX = Math.Min(P.X + Radius, lvl.Width - 1) - P.X;
            int minY = Math.Max(P.Y - Radius, 0) - P.Y, maxY = Math.Min(P.Y + Radius, lvl.Height - 1) - P.Y;
            int minZ = Math.Max(P.Z - Radius, 0) - P.Z, maxZ = Math.Min(P.Z + Radius, lvl.Length - 1) - P.Z;
            for (int yy = minY; yy <= maxY; yy++)
                for (int zz = minZ; zz <= maxZ; zz++)
                    for (int xx = minX; xx <= maxX; xx++)
            {
                int curDist = xx * xx + yy * yy + zz * zz;
                if (curDist < upper && curDist >= inner) {
                    PlaceBlock(p, lvl, (ushort)(P.X + xx), (ushort)(P.Y + yy), (ushort)(P.Z + zz), brush);
                }
            }
        }
    }
}