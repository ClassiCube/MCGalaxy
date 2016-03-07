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

namespace MCGalaxy.Drawing.Ops {

    public abstract class PyramidDrawOp : DrawOp {
        protected DrawOp baseOp;
        protected int yDir;
        
        public PyramidDrawOp(DrawOp baseOp, int yDir) {
            this.baseOp = baseOp;
            this.yDir = yDir;
        }
        
        public override bool DetermineDrawOpMethod(Level lvl, int affected) {
            return baseOp.DetermineDrawOpMethod(lvl, affected);
        }
        
        public override int GetBlocksAffected(Level lvl, ushort x1, ushort y1, ushort z1, ushort x2, ushort y2, ushort z2) {
            int total = 0;
            while (true) {
                total += baseOp.GetBlocksAffected(lvl, x1, y1, z2, x2, y1, z2);
                if (y1 >= lvl.Height || Math.Abs(x2 - x1) <= 1 || Math.Abs(z2 - z1) <= 1)
                    break;            
                x1++; x2--;
                z1++; z2--;
                y1 = (ushort)(y1 + yDir);
            }
            return total;
        }
        
        public override void Perform(ushort x1, ushort y1, ushort z1, ushort x2,
                                     ushort y2, ushort z2, Player p, Level lvl, Brush brush) {
            while (true) {
                baseOp.Perform(x1, y1, z1, x2, y1, z2, p, lvl, brush);
                if (y1 >= lvl.Height || Math.Abs(x2 - x1) <= 1 || Math.Abs(z2 - z1) <= 1)
                    break;                
                x1++; x2--;
                z1++; z2--;
                y1 = (ushort)(y1 + yDir);
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
        
        public override void Perform(ushort x1, ushort y1, ushort z1, ushort x2,
                                     ushort y2, ushort z2, Player p, Level lvl, Brush brush) {
            while (true) {
                wallOp.Perform(x1, y1, z1, x2, y1, z2, p, lvl, brush);
                if (y1 >= lvl.Height || Math.Abs(x2 - x1) <= 1 || Math.Abs(z2 - z1) <= 1)
                    break;            
                x1++; x2--;
                z1++; z2--;
                baseOp.Perform(x1, y1, z1, x2, y1, z2, p, lvl, airBrush);
                y1 = (ushort)(y1 + yDir);
            }
        }
    }
}
