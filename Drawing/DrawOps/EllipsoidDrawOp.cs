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

namespace MCGalaxy {

    public class EllipsoidDrawOp : DrawOp {
        
        public override string Name {
            get { return "Ellipsoid"; }
        }
        
        public override int GetBlocksAffected(ushort x1, ushort y1, ushort z1, ushort x2, ushort y2, ushort z2) {
            double rx = (x2 - x1) / 2.0 + 0.25, ry = (y2 - y1) / 2.0 + 0.25, rz = (z2 - z1) / 2.0 + 0.25;
            return (int)(Math.PI * 4.0/3.0 * rx * ry * rz);
        }
        
        public override void Perform(ushort x1, ushort y1, ushort z1, ushort x2,
                                     ushort y2, ushort z2, Player p, Level lvl, Brush brush) {
            /* Courtesy of fCraft's awesome Open-Source'ness :D */
            double cx = (x1 + x2) / 2.0, cy = (y1 + y2) / 2.0, cz = (z1 + z2) / 2.0;
            double rx = (x2 - x1) / 2.0 + 0.25, ry = (y2 - y1) / 2.0 + 0.25, rz = (z2 - z1) / 2.0 + 0.25;
            double rx2 = 1 / (rx * rx), ry2 = 1 / (ry * ry), rz2 = 1 / (rz * rz);
            
            for (ushort yy = y1; yy <= y2; yy++)
                for (ushort zz = z1; zz <= z2; zz++)
                    for (ushort xx = x1; xx <= x2; xx++)
            {
                double dx = xx - cx, dy = yy - cy, dz = zz - cz;
                if ((dx * dx) * rx2 + (dy * dy) * ry2 + (dz * dz) * rz2 <= 1)
                    PlaceBlock(p, lvl, xx, yy, zz, brush.NextBlock());
            }
        }
    }
    
    public class EllipsoidHollowDrawOp : DrawOp {
        
        public override string Name {
            get { return "Ellipsoid Hollow"; }
        }
        
        public override int GetBlocksAffected(ushort x1, ushort y1, ushort z1, ushort x2, ushort y2, ushort z2) {
            double rx = (x2 - x1) / 2.0 + 0.25, ry = (y2 - y1) / 2.0 + 0.25, rz = (z2 - z1) / 2.0 + 0.25;
            return (int)(Math.PI * 4.0/3.0 * rx * ry * rz);
        }
        
        public override void Perform(ushort x1, ushort y1, ushort z1, ushort x2,
                                     ushort y2, ushort z2, Player p, Level lvl, Brush brush) {
            /* Courtesy of fCraft's awesome Open-Source'ness :D */
            double cx = (x1 + x2) / 2.0, cy = (y1 + y2) / 2.0, cz = (z1 + z2) / 2.0;
            double rx = (x2 - x1) / 2.0 + 0.25, ry = (y2 - y1) / 2.0 + 0.25, rz = (z2 - z1) / 2.0 + 0.25;
            double rx2 = 1 / (rx * rx), ry2 = 1 / (ry * ry), rz2 = 1 / (rz * rz);
            
            double smallrx2 = 1 / ((rx - 1) * (rx - 1));
            double smallry2 = 1 / ((ry - 1) * (ry - 1));
            double smallrz2 = 1 / ((rz - 1) * (rz - 1));
            
            for (ushort yy = y1; yy <= y2; yy++)
                for (ushort zz = z1; zz <= z2; zz++)
                    for (ushort xx = x1; xx <= x2; xx++)
            {
                double dx = xx - cx, dy = yy - cy, dz = zz - cz;
                dx *= dx; dy *= dy; dz *= dz;
                bool inRange = dx * rx2 + dy * ry2 + dz * rz2 <= 1;
                if (inRange && (dx * smallrx2 + dy * smallry2 + dz * smallrz2 > 1))
                    PlaceBlock(p, lvl, xx, yy, zz, brush.NextBlock());
            }
        }
    }
    
    public class CylinderDrawOp : DrawOp {
        
        public override string Name {
            get { return "Cylinder"; }
        }
        
        public override int GetBlocksAffected(ushort x1, ushort y1, ushort z1, ushort x2, ushort y2, ushort z2) {
            double rx = (x2 - x1) / 2.0 + 0.25, rz = (z2 - z1) / 2.0 + 0.25;
            int height = (y2 - y1 + 1);
            return (int)(Math.PI * rx * rz * height);
        }
        
        public override void Perform(ushort x1, ushort y1, ushort z1, ushort x2,
                                     ushort y2, ushort z2, Player p, Level lvl, Brush brush) {
            /* Courtesy of fCraft's awesome Open-Source'ness :D */
            double cx = (x1 + x2) / 2.0, cz = (z1 + z2) / 2.0;
            double rx = (x2 - x1) / 2.0 + 0.25, rz = (z2 - z1) / 2.0 + 0.25;
            double rx2 = 1 / (rx * rx), rz2 = 1 / (rz * rz);
            double smallrx2 = 1 / ((rx - 1) * (rx - 1));
            double smallrz2 = 1 / ((rz - 1) * (rz - 1));
            
            for (ushort yy = y1; yy <= y2; yy++)
                for (ushort zz = z1; zz <= z2; zz++)
                    for (ushort xx = x1; xx <= x2; xx++)
            {
                double dx = xx - cx, dz = zz - cz;
                dx *= dx; dz *= dz;
                bool inRange = dx * rx2 + dz * rz2 <= 1;
                if (inRange && (dx * smallrx2 + dz * smallrz2 > 1))
                    PlaceBlock(p, lvl, xx, yy, zz, brush.NextBlock());
            }
        }
    }
}
