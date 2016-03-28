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

    public abstract class PyramidDrawOp : DrawOp {
        protected DrawOp baseOp;
        protected int yDir;
        
        public PyramidDrawOp(DrawOp baseOp, int yDir) {
            this.baseOp = baseOp;
            this.yDir = yDir;
        }
        
        public override bool DetermineDrawOpMethod(Level lvl, long affected) {
            return baseOp.DetermineDrawOpMethod(lvl, affected);
        }
        
        public override long GetBlocksAffected(Level lvl, Vec3U16[] marks) {
            Vec3U16 origP1 = marks[0], origP2 = marks[1];
            Vec3U16 p1 = marks[0], p2 = marks[1];
            long total = 0;
            while (true) {
                total += baseOp.GetBlocksAffected(lvl, marks);
                if (p1.Y >= lvl.Height || Math.Abs(p2.X - p1.X) <= 1 || Math.Abs(p2.Z - p1.Z) <= 1)
                    break;            
                p1.X++; p2.X--;
                p1.Z++; p2.Z--;
                p1.Y = (ushort)(p1.Y + yDir); p2.Y = p1.Y;
                marks[0] = p1; marks[1] = p2;
            }
            marks[0] = origP1; marks[1] = origP2;
            return total;
        }
        
        public override void Perform(Vec3U16[] marks, Player p, Level lvl, Brush brush) {
            Vec3U16 p1 = marks[0], p2 = marks[1];
            while (true) {
                baseOp.Perform(marks, p, lvl, brush);
                if (p1.Y >= lvl.Height || Math.Abs(p2.X - p1.X) <= 1 || Math.Abs(p2.Z - p1.Z) <= 1)
                    break;
                p1.X++; p2.X--;
                p1.Z++; p2.Z--;
                p1.Y = (ushort)(p1.Y + yDir); p2.Y = p1.Y;
                marks[0] = p1; marks[1] = p2;
            }
        }
    }
    
    public class PyramidSolidDrawOp : PyramidDrawOp {      

        public PyramidSolidDrawOp() : base(new CuboidDrawOp(), 1) {
        }
        
        public override string Name { get { return "Pyramid solid"; } }
    }
    
    public class PyramidHollowDrawOp : PyramidDrawOp {      

        public PyramidHollowDrawOp() : base(new CuboidWallsDrawOp(), 1) {
        }
        
        public override string Name { get { return "Pyramid hollow"; } }
    }
    
    public class PyramidReverseDrawOp : PyramidDrawOp {

        DrawOp wallOp;
        Brush airBrush;
        public PyramidReverseDrawOp() : base(new CuboidDrawOp(), -1) {
            wallOp = new CuboidWallsDrawOp();
            airBrush = new SolidBrush(Block.air, 0);
        }
        
        public override string Name { get { return "Pyramid reverse"; } }
        
        public override void Perform(Vec3U16[] marks, Player p, Level lvl, Brush brush) {
            Vec3U16 p1 = marks[0], p2 = marks[1];
            while (true) {
                wallOp.Perform(marks, p, lvl, brush);
                if (p1.Y >= lvl.Height || Math.Abs(p2.X - p1.X) <= 1 || Math.Abs(p2.Z - p1.Z) <= 1)
                    break;
                p1.X++; p2.X--;
                p1.Z++; p2.Z--;
                marks[0] = p1; marks[1] = p2;
                
                baseOp.Perform(marks, p, lvl, airBrush);
                p1.Y = (ushort)(p1.Y + yDir); p2.Y = p1.Y;
                marks[0] = p1; marks[1] = p2;
            }
        }
    }
}
