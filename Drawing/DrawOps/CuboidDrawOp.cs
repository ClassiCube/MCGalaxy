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

    public class CuboidDrawOp : DrawOp {        
        public override string Name { get { return "Cuboid"; } }
        
        public override long GetBlocksAffected(Level lvl, Vec3S32[] marks) {
            return (Max.X - Min.X + 1) * (Max.Y - Min.Y + 1) * (Max.Z - Min.Z + 1);
        }
        
        public override IEnumerable<DrawOpBlock> Perform(Vec3S32[] marks, Player p, Level lvl, Brush brush) {
            Vec3U16 p1 = Clamp(Min), p2 = Clamp(Max);
            for (ushort y = p1.Y; y <= p2.Y; y++)
                for (ushort z = p1.Z; z <= p2.Z; z++)
                    for (ushort x = p1.X; x <= p2.X; x++)
            {
                yield return Place(x, y, z, brush);
            }
        }
    }
    
    public class CuboidHollowsDrawOp : DrawOp {       
        public override string Name { get { return "Cuboid Hollow"; } }
        
        public override long GetBlocksAffected(Level lvl, Vec3S32[] marks) {
            int lenX = (Max.X - Min.X + 1), lenY = (Max.Y - Min.Y + 1), lenZ = (Max.Z - Min.Z + 1);
            int xQuadsVol = Math.Min(lenX, 2) * (lenY * lenZ);
            int yQuadsVol = Math.Max(0, Math.Min(lenY, 2) * ((lenX - 2) * lenZ)); // we need to avoid double counting overlaps
            int zQuadzVol = Math.Max(0, Math.Min(lenZ, 2) * ((lenX - 2) * (lenY - 2)));
            return xQuadsVol + yQuadsVol + zQuadzVol;
        }
        
        public override IEnumerable<DrawOpBlock> Perform(Vec3S32[] marks, Player p, Level lvl, Brush brush) {
            Vec3U16 p1 = Clamp(Min), p2 = Clamp(Max);
            int lenX = (p2.X - p1.X + 1), lenY = (p2.Y - p1.Y + 1);
            foreach (var block in QuadY(p1.Y, p1.X, p1.Z, p2.X, p2.Z, brush))
                yield return block;
            foreach (var block in QuadY(p2.Y, p1.X, p1.Z, p2.X, p2.Z, brush))
                yield return block;
            if (lenY > 2) {
                foreach (var block in QuadX(p1.X, (ushort)(p1.Y + 1), p1.Z, (ushort)(p2.Y - 1), p2.Z, brush))
                    yield return block;
                foreach (var block in QuadX(p2.X, (ushort)(p1.Y + 1), p1.Z, (ushort)(p2.Y - 1), p2.Z, brush))
                    yield return block;
            }
            if (lenX > 2 && lenY > 2) {
                foreach (var block in QuadZ(p1.Z, (ushort)(p1.Y + 1), (ushort)(p1.X + 1),
                                            (ushort)(p2.Y - 1), (ushort)(p2.X - 1), brush))
                    yield return block;
                foreach (var block in QuadZ(p2.Z, (ushort)(p1.Y + 1), (ushort)(p1.X + 1),
                                            (ushort)(p2.Y - 1), (ushort)(p2.X - 1), brush))
                    yield return block;
            }
        }
        
        protected IEnumerable<DrawOpBlock> QuadX(ushort x, ushort y1, ushort z1, ushort y2, ushort z2, Brush brush) {
            for (ushort y = y1; y <= y2; y++)
                for (ushort z = z1; z <= z2; z++)
            {
                yield return Place(x, y, z, brush);
            }
        }
        
        protected IEnumerable<DrawOpBlock> QuadY(ushort y, ushort x1, ushort z1, ushort x2, ushort z2, Brush brush) {
            for (ushort z = z1; z <= z2; z++)
                for (ushort x = x1; x <= x2; x++)
            {
                yield return Place(x, y, z, brush);
            }
        }
        
        protected IEnumerable<DrawOpBlock> QuadZ(ushort z, ushort y1, ushort x1, ushort y2, ushort x2, Brush brush) {
            for (ushort y = y1; y <= y2; y++)
                for (ushort x = x1; x <= x2; x++)
            {
                yield return Place(x, y, z, brush);
            }
        }
    }
    
	public class CuboidWallsDrawOp : CuboidHollowsDrawOp {
        public override string Name { get { return "Cuboid Walls"; } }
        
        public override long GetBlocksAffected(Level lvl, Vec3S32[] marks) {
            int lenX = (Max.X - Min.X + 1), lenY = (Max.Y - Min.Y + 1), lenZ = (Max.Z - Min.Z + 1);
            int xQuadsVol = Math.Min(lenX, 2) * (lenY * lenZ);
            int zQuadsVol = Math.Max(0, Math.Min(lenZ, 2) * ((lenX - 2) * lenY)); // we need to avoid double counting overlaps
            return xQuadsVol + zQuadsVol;
        }
        
        public override IEnumerable<DrawOpBlock> Perform(Vec3S32[] marks, Player p, Level lvl, Brush brush) {
            Vec3U16 p1 = Clamp(Min), p2 = Clamp(Max);
            int lenX = (p2.X - p1.X + 1);
            foreach (var block in QuadX(p1.X, p1.Y, p1.Z, p2.Y, p2.Z, brush))
                yield return block;
            foreach (var block in QuadX(p2.X, p1.Y, p1.Z, p2.Y, p2.Z, brush))
                yield return block;
            
            if (lenX <= 2) yield break;
            foreach (var block in QuadZ(p1.Z, p1.Y, (ushort)(p1.X + 1), p2.Y, (ushort)(p2.X - 1), brush))
                yield return block;
            foreach (var block in QuadZ(p2.Z, p1.Y, (ushort)(p1.X + 1), p2.Y, (ushort)(p2.X - 1), brush))
                yield return block;
        }
    }
    
    public class CuboidWireframeDrawOp : CuboidHollowsDrawOp {        
        public override string Name { get { return "Cuboid Wireframe"; } }
        
        public override long GetBlocksAffected(Level lvl, Vec3S32[] marks) {
            int lenX = (Max.X - Min.X + 1), lenY = (Max.Y - Min.Y + 1), lenZ = (Max.Z - Min.Z + 1);
            int horSidesvol = 2 * (lenX * 2 + lenZ * 2); // TODO: slightly overestimated by at most four blocks.
            int verSidesVol = Math.Max(0, lenY - 2) * 4;
            return horSidesvol + verSidesVol;
        }
        
        public override IEnumerable<DrawOpBlock> Perform(Vec3S32[] marks, Player p, Level lvl, Brush brush) {
            Vec3U16 p1 = Clamp(Min), p2 = Clamp(Max);
            for (ushort y = p1.Y; y <= p2.Y; y++ ) {
                yield return Place(p1.X, y, p1.Z, brush);
                yield return Place(p2.X, y, p1.Z, brush);
                yield return Place(p1.X, y, p2.Z, brush);
                yield return Place(p2.X, y, p2.Z, brush);
            }

            for (ushort z = p1.Z; z <= p2.Z; z++) {
                yield return Place(p1.X, p1.Y, z, brush);
                yield return Place(p2.X, p1.Y, z, brush);
                yield return Place(p1.X, p2.Y, z, brush);
                yield return Place(p2.X, p2.Y, z, brush);
            }
            
            for (ushort x = p1.X; x <= p2.X; x++) {
                yield return Place(x, p1.Y, p1.Z, brush);
                yield return Place(x, p1.Y, p2.Z, brush);
                yield return Place(x, p2.Y, p1.Z, brush);
                yield return Place(x, p2.Y, p2.Z, brush);
            }
        }
    }
}
