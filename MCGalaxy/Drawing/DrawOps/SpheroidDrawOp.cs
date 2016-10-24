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

namespace MCGalaxy.Drawing.Ops {
    public class EllipsoidDrawOp : DrawOp {        
        public override string Name { get { return "Ellipsoid"; } }
        
        public override long BlocksAffected(Level lvl, Vec3S32[] marks) {
            double rx = (Max.X - Min.X) / 2.0 + 0.25, ry = (Max.Y - Min.Y) / 2.0 + 0.25, rz = (Max.Z - Min.Z) / 2.0 + 0.25;
            return (int)(Math.PI * 4.0/3.0 * rx * ry * rz);
        }
        
        public override void Perform(Vec3S32[] marks, Player p, Level lvl, Brush brush, Action<DrawOpBlock> output) {
            /* Courtesy of fCraft's awesome Open-Source'ness :D */
            double cx = (Min.X + Max.X) / 2.0, cy = (Min.Y + Max.Y) / 2.0, cz = (Min.Z + Max.Z) / 2.0;
            double rx = (Max.X - Min.X) / 2.0 + 0.25, ry = (Max.Y - Min.Y) / 2.0 + 0.25, rz = (Max.Z - Min.Z) / 2.0 + 0.25;
            double rx2 = 1 / (rx * rx), ry2 = 1 / (ry * ry), rz2 = 1 / (rz * rz);
            Vec3U16 min = Clamp(Min), max = Clamp(Max);
            
            for (ushort yy = min.Y; yy <= max.Y; yy++)
                for (ushort zz = min.Z; zz <= max.Z; zz++)
                    for (ushort xx = min.X; xx <= max.X; xx++)
            {
                double dx = xx - cx, dy = yy - cy, dz = zz - cz;
                if ((dx * dx) * rx2 + (dy * dy) * ry2 + (dz * dz) * rz2 <= 1)
                    output(Place(xx, yy, zz, brush));
            }
        }
    }
    
    public class EllipsoidHollowDrawOp : DrawOp {
        
        public override string Name { get { return "Ellipsoid Hollow"; } }
        
        public override long BlocksAffected(Level lvl, Vec3S32[] marks) {
            double rx = (Max.X - Min.X) / 2.0 + 0.25, ry = (Max.Y - Min.Y) / 2.0 + 0.25, rz = (Max.Z - Min.Z) / 2.0 + 0.25;
            return (int)(Math.PI * 4.0/3.0 * rx * ry * rz);
        }
        
        public override void Perform(Vec3S32[] marks, Player p, Level lvl, Brush brush, Action<DrawOpBlock> output) {
            /* Courtesy of fCraft's awesome Open-Source'ness :D */
            double cx = (Min.X + Max.X) / 2.0, cy = (Min.Y + Max.Y) / 2.0, cz = (Min.Z + Max.Z) / 2.0;
            double rx = (Max.X - Min.X) / 2.0 + 0.25, ry = (Max.Y - Min.Y) / 2.0 + 0.25, rz = (Max.Z - Min.Z) / 2.0 + 0.25;
            double rx2 = 1 / (rx * rx), ry2 = 1 / (ry * ry), rz2 = 1 / (rz * rz);
            
            double smallrx2 = 1 / ((rx - 1) * (rx - 1));
            double smallry2 = 1 / ((ry - 1) * (ry - 1));
            double smallrz2 = 1 / ((rz - 1) * (rz - 1));
            Vec3U16 min = Clamp(Min), max = Clamp(Max);
            
            for (ushort yy = min.Y; yy <= max.Y; yy++)
                for (ushort zz = min.Z; zz <= max.Z; zz++)
                    for (ushort xx = min.X; xx <= max.X; xx++)
            {
                double dx = xx - cx, dy = yy - cy, dz = zz - cz;
                dx *= dx; dy *= dy; dz *= dz;
                bool inRange = dx * rx2 + dy * ry2 + dz * rz2 <= 1;
                if (inRange && (dx * smallrx2 + dy * smallry2 + dz * smallrz2 > 1))
                    output(Place(xx, yy, zz, brush));
            }
        }
    }
    
    public class CylinderDrawOp : DrawOp {       
        public override string Name { get { return "Cylinder"; } }
        
        public override long BlocksAffected(Level lvl, Vec3S32[] marks) {
            double rx = (Max.X - Min.X) / 2.0 + 0.25, rz = (Max.Z - Min.Z) / 2.0 + 0.25;
            int height = (Max.Y - Min.Y + 1);
            return (int)(Math.PI * rx * rz * height);
        }
        
        public override void Perform(Vec3S32[] marks, Player p, Level lvl, Brush brush, Action<DrawOpBlock> output) {            
            /* Courtesy of fCraft's awesome Open-Source'ness :D */
            double cx = (Min.X + Max.X) / 2.0, cz = (Min.Z + Max.Z) / 2.0;
            double rx = (Max.X - Min.X) / 2.0 + 0.25, rz = (Max.Z - Min.Z) / 2.0 + 0.25;
            double rx2 = 1 / (rx * rx), rz2 = 1 / (rz * rz);
            double smallrx2 = 1 / ((rx - 1) * (rx - 1));
            double smallrz2 = 1 / ((rz - 1) * (rz - 1));
            Vec3U16 p1 = Clamp(Min), p2 = Clamp(Max);
            
            for (ushort yy = p1.Y; yy <= p2.Y; yy++)
                for (ushort zz = p1.Z; zz <= p2.Z; zz++)
                    for (ushort xx = p1.X; xx <= p2.X; xx++)
            {
                double dx = xx - cx, dz = zz - cz;
                dx *= dx; dz *= dz;
                bool inRange = dx * rx2 + dz * rz2 <= 1;
                if (inRange && (dx * smallrx2 + dz * smallrz2 > 1))
                    output(Place(xx, yy, zz, brush));
            }
        }
    }
}
