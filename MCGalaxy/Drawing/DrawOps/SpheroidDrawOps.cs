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
using MCGalaxy.Drawing.Brushes;
using MCGalaxy.Maths;

namespace MCGalaxy.Drawing.Ops 
{
    public abstract class ShapedDrawOp : DrawOp
    {
        public double XRadius { get { return (Max.X - Min.X) / 2.0 + 0.25; } }
        public double YRadius { get { return (Max.Y - Min.Y) / 2.0 + 0.25; } }
        public double ZRadius { get { return (Max.Z - Min.Z) / 2.0 + 0.25; } }

        public double XCentre { get { return (Min.X + Max.X) / 2.0; } }
        public double YCentre { get { return (Min.Y + Max.Y) / 2.0; } }
        public double ZCentre { get { return (Min.Z + Max.Z) / 2.0; } }

        public int Height { get { return (Max.Y - Min.Y) + 1; } }


        public static double EllipsoidVolume(double rx, double ry, double rz) {
            return Math.PI * 4.0 / 3.0 * (rx * ry * rz);
        }

        public static double ConeVolume(double rx, double rz, double height) {
            return Math.PI / 3.0 * (rx * rz * height);
        }

        public static double CylinderVolume(double rx, double rz, double height) {
            return Math.PI * (rx * rz * height);
        }
    }

    public class EllipsoidDrawOp : ShapedDrawOp
    {
        public override string Name { get { return "Ellipsoid"; } }
        
        public override long BlocksAffected(Level lvl, Vec3S32[] marks) {
            return (long)EllipsoidVolume(XRadius, YRadius, ZRadius);
        }
        
        public override void Perform(Vec3S32[] marks, Brush brush, DrawOpOutput output) {
            /* Courtesy of fCraft's awesome Open-Source'ness :D */
            double cx = XCentre, cy = YCentre, cz = ZCentre;
            double rx = XRadius, ry = YRadius, rz = ZRadius;
            double rx2 = 1 / (rx * rx), ry2 = 1 / (ry * ry), rz2 = 1 / (rz * rz);
            Vec3U16 min = Clamp(Min), max = Clamp(Max);
            
            for (ushort y = min.Y; y <= max.Y; y++)
                for (ushort z = min.Z; z <= max.Z; z++)
                    for (ushort x = min.X; x <= max.X; x++)
            {
                double dx = x - cx, dy = y - cy, dz = z - cz;
                if ((dx * dx) * rx2 + (dy * dy) * ry2 + (dz * dz) * rz2 <= 1)
                    output(Place(x, y, z, brush));
            }
        }
    }
    
    public class EllipsoidHollowDrawOp : ShapedDrawOp
    {       
        public override string Name { get { return "Ellipsoid Hollow"; } }
        
        public override long BlocksAffected(Level lvl, Vec3S32[] marks) {
            double rx = XRadius, ry = YRadius, rz = ZRadius;
            double outer = EllipsoidVolume(rx,     ry,     rz    );
            double inner = EllipsoidVolume(rx - 1, ry - 1, rz - 1);
            return (long)(outer - inner);
        }
        
        public override void Perform(Vec3S32[] marks, Brush brush, DrawOpOutput output) {
            /* Courtesy of fCraft's awesome Open-Source'ness :D */
            double cx = XCentre, cy = YCentre, cz = ZCentre;
            double rx = XRadius, ry = YRadius, rz = ZRadius;
            double outer_rx2 = 1 / (rx * rx);
            double outer_ry2 = 1 / (ry * ry);
            double outer_rz2 = 1 / (rz * rz);
            double inner_rx2 = 1 / ((rx - 1) * (rx - 1));
            double inner_ry2 = 1 / ((ry - 1) * (ry - 1));
            double inner_rz2 = 1 / ((rz - 1) * (rz - 1));
            Vec3U16 min = Clamp(Min), max = Clamp(Max);
            
            for (ushort y = min.Y; y <= max.Y; y++)
                for (ushort z = min.Z; z <= max.Z; z++)
                    for (ushort x = min.X; x <= max.X; x++)
            {
                double dx = x - cx, dy = y - cy, dz = z - cz;
                dx *= dx; dy *= dy; dz *= dz;

                if (dx * outer_rx2 + dy * outer_ry2 + dz * outer_rz2 > 1)
                    continue; // outside ellipsoid radius
                if (dx * inner_rx2 + dy * inner_ry2 + dz * inner_rz2 > 1)
                    output(Place(x, y, z, brush));
            }
        }
    }
    
    public class CylinderDrawOp : ShapedDrawOp
    {
        public override string Name { get { return "Cylinder"; } }
        
        public override long BlocksAffected(Level lvl, Vec3S32[] marks) {
            double rx = XRadius, rz = ZRadius, h = Height;
            double outer = CylinderVolume(rx,     rz,     h);
            double inner = CylinderVolume(rx - 1, rz - 1, h);
            return (long)(outer - inner);
        }
        
        public override void Perform(Vec3S32[] marks, Brush brush, DrawOpOutput output) {            
            /* Courtesy of fCraft's awesome Open-Source'ness :D */
            double cx = XCentre, cz = ZCentre;
            double rx = XRadius, rz = ZRadius;
            double outer_rx2 = 1 / (rx * rx);
            double outer_rz2 = 1 / (rz * rz);
            double inner_rx2 = 1 / ((rx - 1) * (rx - 1));
            double inner_rz2 = 1 / ((rz - 1) * (rz - 1));
            Vec3U16 p1 = Clamp(Min), p2 = Clamp(Max);
            
            for (ushort y = p1.Y; y <= p2.Y; y++)
                for (ushort z = p1.Z; z <= p2.Z; z++)
                    for (ushort x = p1.X; x <= p2.X; x++)
            {
                double dx = x - cx, dz = z - cz;
                dx *= dx; dz *= dz;

                if (dx * outer_rx2 + dz * outer_rz2 > 1)
                    continue; // outside cylinder radius
                if (dx * inner_rx2 + dz * inner_rz2 > 1)
                    output(Place(x, y, z, brush));
            }
        }
    }

    public class ConeDrawOp : ShapedDrawOp 
    {
        public bool Invert;
        public override string Name { get { return "Cone"; } }
        public ConeDrawOp(bool invert = false) { Invert = invert; }
        
        public override long BlocksAffected(Level lvl, Vec3S32[] marks) {
            return (long)ConeVolume(XRadius, ZRadius, Height);
        }
        
        public override void Perform(Vec3S32[] marks, Brush brush, DrawOpOutput output) {
            Vec3U16 p1 = Clamp(Min), p2 = Clamp(Max);
            double cx  = XCentre, cz = ZCentre;
            int height = Height;

            for (ushort y = p1.Y; y <= p2.Y; y++)
            {
                int dy   = Invert ? y - Min.Y : Max.Y - y;
                double T = (double)(dy + 1) / height;

                double rx  = ((Max.X - Min.X) / 2.0) * T + 0.25;
                double rz  = ((Max.Z - Min.Z) / 2.0) * T + 0.25;
                double rx2 = 1 / (rx * rx), rz2 = 1 / (rz * rz);

                for (ushort z = p1.Z; z <= p2.Z; z++)
                    for (ushort x = p1.X; x <= p2.X; x++)
                {
                    double dx = x - cx, dz = z - cz;

                    if ((dx * dx) * rx2 + (dz * dz) * rz2 <= 1)
                        output(Place(x, y, z, brush));
                 }
            }
        }
    }
}
