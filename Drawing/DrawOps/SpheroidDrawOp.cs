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

namespace MCGalaxy.Drawing.Ops {

    public class EllipsoidDrawOp : DrawOp {
        
        public override string Name { get { return "Ellipsoid"; } }
        
        public override int GetBlocksAffected(Level lvl, Vector3U16[] marks) {
            Vector3U16 p1 = marks[0], p2 = marks[1];
            double rx = (p2.X - p1.X) / 2.0 + 0.25, ry = (p2.Y - p1.Y) / 2.0 + 0.25, rz = (p2.Z - p1.Z) / 2.0 + 0.25;
            return (int)(Math.PI * 4.0/3.0 * rx * ry * rz);
        }
        
        public override void Perform(Vector3U16[] marks, Player p, Level lvl, Brush brush) {
            Vector3U16 p1 = marks[0], p2 = marks[1];
            /* Courtesy of fCraft's awesome Open-Source'ness :D */
            double cx = (p1.X + p2.X) / 2.0, cy = (p1.Y + p2.Y) / 2.0, cz = (p1.Z + p2.Z) / 2.0;
            double rx = (p2.X - p1.X) / 2.0 + 0.25, ry = (p2.Y - p1.Y) / 2.0 + 0.25, rz = (p2.Z - p1.Z) / 2.0 + 0.25;
            double rx2 = 1 / (rx * rx), ry2 = 1 / (ry * ry), rz2 = 1 / (rz * rz);
            
            for (ushort yy = p1.Y; yy <= p2.Y; yy++)
                for (ushort zz = p1.Z; zz <= p2.Z; zz++)
                    for (ushort xx = p1.X; xx <= p2.X; xx++)
            {
                double dx = xx - cx, dy = yy - cy, dz = zz - cz;
                if ((dx * dx) * rx2 + (dy * dy) * ry2 + (dz * dz) * rz2 <= 1)
                    PlaceBlock(p, lvl, xx, yy, zz, brush);
            }
        }
    }
    
    public class EllipsoidHollowDrawOp : DrawOp {
        
        public override string Name { get { return "Ellipsoid Hollow"; } }
        
        public override int GetBlocksAffected(Level lvl, Vector3U16[] marks) {
            Vector3U16 p1 = marks[0], p2 = marks[1];
            double rx = (p2.X - p1.X) / 2.0 + 0.25, ry = (p2.Y - p1.Y) / 2.0 + 0.25, rz = (p2.Z - p1.Z) / 2.0 + 0.25;
            return (int)(Math.PI * 4.0/3.0 * rx * ry * rz);
        }
        
        public override void Perform(Vector3U16[] marks, Player p, Level lvl, Brush brush) {
            Vector3U16 p1 = marks[0], p2 = marks[1];
            /* Courtesy of fCraft's awesome Open-Source'ness :D */
            double cx = (p1.X + p2.X) / 2.0, cy = (p1.Y + p2.Y) / 2.0, cz = (p1.Z + p2.Z) / 2.0;
            double rx = (p2.X - p1.X) / 2.0 + 0.25, ry = (p2.Y - p1.Y) / 2.0 + 0.25, rz = (p2.Z - p1.Z) / 2.0 + 0.25;
            double rx2 = 1 / (rx * rx), ry2 = 1 / (ry * ry), rz2 = 1 / (rz * rz);
            
            double smallrx2 = 1 / ((rx - 1) * (rx - 1));
            double smallry2 = 1 / ((ry - 1) * (ry - 1));
            double smallrz2 = 1 / ((rz - 1) * (rz - 1));
            
            for (ushort yy = p1.Y; yy <= p2.Y; yy++)
                for (ushort zz = p1.Z; zz <= p2.Z; zz++)
                    for (ushort xx = p1.X; xx <= p2.X; xx++)
            {
                double dx = xx - cx, dy = yy - cy, dz = zz - cz;
                dx *= dx; dy *= dy; dz *= dz;
                bool inRange = dx * rx2 + dy * ry2 + dz * rz2 <= 1;
                if (inRange && (dx * smallrx2 + dy * smallry2 + dz * smallrz2 > 1))
                    PlaceBlock(p, lvl, xx, yy, zz, brush);
            }
        }
    }
    
    public class CylinderDrawOp : DrawOp {
        
        public override string Name { get { return "Cylinder"; } }
        
        public override int GetBlocksAffected(Level lvl, Vector3U16[] marks) {
            Vector3U16 p1 = marks[0], p2 = marks[1];
            double rx = (p2.X - p1.X) / 2.0 + 0.25, rz = (p2.Z - p1.Z) / 2.0 + 0.25;
            int height = (p2.Y - p1.Y + 1);
            return (int)(Math.PI * rx * rz * height);
        }
        
        public override void Perform(Vector3U16[] marks, Player p, Level lvl, Brush brush) {
            Vector3U16 p1 = marks[0], p2 = marks[1];
            /* Courtesy of fCraft's awesome Open-Source'ness :D */
            double cx = (p1.X + p2.X) / 2.0, cz = (p1.Z + p2.Z) / 2.0;
            double rx = (p2.X - p1.X) / 2.0 + 0.25, rz = (p2.Z - p1.Z) / 2.0 + 0.25;
            double rx2= 1 / (rx * rx), rz2 = 1 / (rz * rz);
            double smallrx2 = 1 / ((rx - 1) * (rx - 1));
            double smallrz2 = 1 / ((rz - 1) * (rz - 1));
            
            for (ushort yy = p1.Y; yy <= p2.Y; yy++)
                for (ushort zz = p1.Z; zz <= p2.Z; zz++)
                    for (ushort xx = p1.X; xx <= p2.X; xx++)
            {
                double dx = xx - cx, dz = zz - cz;
                dx *= dx; dz *= dz;
                bool inRange = dx * rx2 + dz * rz2 <= 1;
                if (inRange && (dx * smallrx2 + dz * smallrz2 > 1))
                    PlaceBlock(p, lvl, xx, yy, zz, brush);
            }
        }
    }
}
