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

    public class CuboidDrawOp : DrawOp {
        
        public override string Name {
            get { return "Cuboid"; }
        }
        
        public override int GetBlocksAffected(ushort x1, ushort y1, ushort z1, ushort x2, ushort y2, ushort z2) {
            return (x2 - x1 + 1) * (y2 - y1 + 1) * (z2 - z1 + 1);
        }
        
        public override void Perform(ushort x1, ushort y1, ushort z1, ushort x2,
                                     ushort y2, ushort z2, Player p, Level lvl, Brush brush) {
            for (ushort y = y1; y <= y2; y++)
                for (ushort z = z1; z <= z2; z++)
                    for (ushort x = x1; x <= x2; x++)
            {
                PlaceBlock(p, lvl, x, y, z, brush.NextBlock());
            }
        }
    }
    
    public class CuboidHolesDrawOp : DrawOp {
        
        public override string Name {
            get { return "Cuboid Holes"; }
        }
        
        public override int GetBlocksAffected(ushort x1, ushort y1, ushort z1, ushort x2, ushort y2, ushort z2) {
            return (x2 - x1 + 1) * (y2 - y1 + 1) * (z2 - z1 + 1);
        }
        
        public override void Perform(ushort x1, ushort y1, ushort z1, ushort x2,
                                     ushort y2, ushort z2, Player p, Level lvl, Brush brush) {
            for (ushort y = y1; y <= y2; y++)
                for (ushort z = z1; z <= z2; z++)
            {
                int i = (y & 1) == 0 ? 0 : 1;
                if ((z & 1) == 0) i++;
                
                for (ushort x = x1; x <= x2; x++) {
                    byte block = (i & 1) == 0 ? brush.NextBlock() : Block.air;
                    PlaceBlock(p, lvl, x, y, z, block);
                    i++;
                }
            }
        }
    }
    
    public class CuboidHollowsDrawOp : DrawOp {
        
        public override string Name {
            get { return "Cuboid Hollow"; }
        }
        
        public override int GetBlocksAffected(ushort x1, ushort y1, ushort z1, ushort x2, ushort y2, ushort z2) {
            int lenX = (x2 - x1 + 1), lenY = (y2 - y1 + 1), lenZ = (z2 - z2 + 1);
            int xQuadsVol = Math.Min(lenX, 2) * (lenY * lenZ);
            int yQuadsVol = Math.Max(0, Math.Min(lenY, 2) * ((lenX - 2) * lenZ)); // we need to avoid double counting overlaps
            int zQuadzVol = Math.Max(0, Math.Min(lenZ, 2) * ((lenX - 2) * (lenY - 2)));
            return xQuadsVol + yQuadsVol + zQuadzVol;
        }
        
        public override void Perform(ushort x1, ushort y1, ushort z1, ushort x2,
                                     ushort y2, ushort z2, Player p, Level lvl, Brush brush) {
            int lenX = (x2 - x1 + 1), lenY = (y2 - y1 + 1);
            QuadY(y1, x1, z1, x2, z2, p, lvl, brush);
            QuadY(y2, x1, z1, x2, z2, p, lvl, brush);
            if (lenY > 2) {
                QuadX(x1, (ushort)(y1 + 1), z1, (ushort)(y2 - 1), z2, p, lvl, brush);
                QuadX(x2, (ushort)(y1 + 1), z1, (ushort)(y2 - 1), z2, p, lvl, brush);
            }
            if (lenX > 2 && lenY > 2) {
                QuadZ(z1, (ushort)(x1 + 1), (ushort)(y1 + 1),
                      (ushort)(x2 - 1), (ushort)(y2 - 1), p, lvl, brush);
                QuadZ(z2, (ushort)(x1 + 1), (ushort)(y1 + 1),
                      (ushort)(x2 - 1), (ushort)(y2 - 1), p, lvl, brush);
            }
        }
        
        protected void QuadX(ushort x, ushort y1, ushort z1, ushort y2, ushort z2,
                             Player p, Level lvl, Brush brush) {
            for (ushort y = y1; y <= y2; y++)
                for (ushort z = z1; z <= z2; z++)
            {
                PlaceBlock(p, lvl, x, y, z, brush.NextBlock());
            }
        }
        
        protected void QuadY(ushort y, ushort x1, ushort z1, ushort x2, ushort z2,
                             Player p, Level lvl, Brush brush) {
            for (ushort z = z1; z <= z2; z++)
                for (ushort x = x1; x <= x2; x++)
            {
                PlaceBlock(p, lvl, x, y, z, brush.NextBlock());
            }
        }
        
        protected void QuadZ(ushort z, ushort x1, ushort y1, ushort x2, ushort y2,
                             Player p, Level lvl, Brush brush) {
            for (ushort y = y1; y <= y2; y++)
                for (ushort x = x1; x <= x2; x++)
            {
                PlaceBlock(p, lvl, x, y, z, brush.NextBlock());
            }
        }
    }
    
    public class CuboidWallsDrawOp : CuboidHollowsDrawOp {
        
        public override string Name {
            get { return "Cuboid Walls"; }
        }
        
        public override int GetBlocksAffected(ushort x1, ushort y1, ushort z1, ushort x2, ushort y2, ushort z2) {
            int lenX = (x2 - x1 + 1), lenY = (y2 - y1 + 1), lenZ = (z2 - z2 + 1);
            int xQuadsVol = Math.Min(lenX, 2) * (lenY * lenZ);
            int zQuadsVol = Math.Max(0, Math.Min(lenZ, 2) * ((lenX - 2) * lenY)); // we need to avoid double counting overlaps
            return xQuadsVol + zQuadsVol;
        }
        
        public override void Perform(ushort x1, ushort y1, ushort z1, ushort x2,
                                     ushort y2, ushort z2, Player p, Level lvl, Brush brush) {
            int lenX = (x2 - x1 + 1);
            QuadX(x1, y1, z1, y2, z2, p, lvl, brush);
            QuadX(x2, y1, z1, y2, z2, p, lvl, brush);
            if (lenX > 2) {
                QuadZ(z1, (ushort)(x1 + 1), y1, (ushort)(x2 - 1), y2, p, lvl, brush);
                QuadZ(z2, (ushort)(x1 + 1), y1, (ushort)(x2 - 1), y2, p, lvl, brush);
            }
        }
    }
    
    public class CuboidWireframeDrawOp : CuboidHollowsDrawOp {
        
        public override string Name {
            get { return "Cuboid Wireframe"; }
        }
        
        public override int GetBlocksAffected(ushort x1, ushort y1, ushort z1, ushort x2, ushort y2, ushort z2) {
            int lenX = (x2 - x1 + 1), lenY = (y2 - y1 + 1), lenZ = (z2 - z2 + 1);
            int horSidesvol = 2 * (lenX * 2 + lenZ * 2); // TODO: slightly overestimated by at most four blocks.
            int verSidesVol = Math.Max(0, lenY - 2) * 4;
            return horSidesvol + verSidesVol;
        }
        
        public override void Perform(ushort x1, ushort y1, ushort z1, ushort x2,
                                     ushort y2, ushort z2, Player p, Level lvl, Brush brush) {
            for (ushort y = y1; y <= y2; y++ ) {
                PlaceBlock(p, lvl, x1, y, z1, brush.NextBlock());
                PlaceBlock(p, lvl, x2, y, z1, brush.NextBlock());
                PlaceBlock(p, lvl, x1, y, z2, brush.NextBlock());
                PlaceBlock(p, lvl, x2, y, z2, brush.NextBlock());
            }

            for (ushort z = z1; z <= z2; z++) {
                PlaceBlock(p, lvl, x1, y1, z, brush.NextBlock());
                PlaceBlock(p, lvl, x2, y1, z, brush.NextBlock());
                PlaceBlock(p, lvl, x1, y2, z, brush.NextBlock());
                PlaceBlock(p, lvl, x2, y2, z, brush.NextBlock());
            }
            
            for (ushort x = x1; x <= x2; x++) {
                PlaceBlock(p, lvl, x, y1, z1, brush.NextBlock());
                PlaceBlock(p, lvl, x, y1, z2, brush.NextBlock());
                PlaceBlock(p, lvl, x, y2, z1, brush.NextBlock());
                PlaceBlock(p, lvl, x, y2, z2, brush.NextBlock());
            }
        }
    }
}
