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
using MCGalaxy.Maths;

namespace MCGalaxy.Drawing.Ops {

    public abstract class PyramidDrawOp : DrawOp {
        protected DrawOp baseOp;
        protected int yDir;
        
        public PyramidDrawOp(DrawOp baseOp, int yDir) {
            this.baseOp = baseOp;
            this.yDir = yDir;
        }
        
        public override long BlocksAffected(Level lvl, Vec3S32[] marks) {
            Vec3S32 p1 = Min, p2 = Max;
            long total = 0;
            
            while (p1.Y >= 0 && p1.Y < lvl.Height && p1.X <= p2.X && p1.Z <= p2.Z) {
                baseOp.Min = p1; baseOp.Max = p2;
                total += baseOp.BlocksAffected(lvl, marks);

                p1.X++; p2.X--; 
                p1.Z++; p2.Z--;
                p1.Y += yDir; p2.Y = p1.Y;
            }
            return total;
        }
        
        public override void Perform(Vec3S32[] marks, Brush brush, DrawOpOutput output) {
            Vec3S32 p1 = Min, p2 = Max;
            baseOp.SetLevel(Level);
            baseOp.Player = Player;
            
            while (p1.Y >= 0 && p1.Y < Level.Height && p1.X <= p2.X && p1.Z <= p2.Z) {
                baseOp.Min = p1; baseOp.Max = p2;
                baseOp.Perform(marks, brush, output);

                p1.X++; p2.X--;
                p1.Z++; p2.Z--;
                p1.Y += yDir; p2.Y = p1.Y;
            }
        }
    }
    
    public class PyramidSolidDrawOp : PyramidDrawOp {

        public PyramidSolidDrawOp() : base(new CuboidDrawOp(), 1) { }
        
        public override string Name { get { return "Pyramid solid"; } }
    }
    
    public class PyramidHollowDrawOp : PyramidDrawOp {      

        public PyramidHollowDrawOp() : base(new CuboidWallsDrawOp(), 1) { }
        
        public override string Name { get { return "Pyramid hollow"; } }
    }
    
    public class PyramidReverseDrawOp : PyramidDrawOp {

        DrawOp wallOp;
        Brush airBrush;
        public PyramidReverseDrawOp() : base(new CuboidDrawOp(), -1) {
            wallOp = new CuboidWallsDrawOp();
            airBrush = new SolidBrush(Block.Air);
        }
        
        public override string Name { get { return "Pyramid reverse"; } }
        
        public override void Perform(Vec3S32[] marks, Brush brush, DrawOpOutput output) {
            Vec3U16 p1 = Clamp(Min), p2 = Clamp(Max);
            wallOp.Min = Min; wallOp.Max = Max;
            baseOp.Min = Min; baseOp.Max = Max;
            wallOp.SetLevel(Level); baseOp.SetLevel(Level);
            wallOp.Player = Player; baseOp.Player = Player;
            
            while (true) {
                wallOp.Perform(marks, brush, output);
                if (p1.Y >= Level.Height || Math.Abs(p2.X - p1.X) <= 1 || Math.Abs(p2.Z - p1.Z) <= 1)
                    return;
                
                p1.X++; p2.X--;
                p1.Z++; p2.Z--;
                wallOp.Min = p1; wallOp.Max = p2;
                baseOp.Min = p1; baseOp.Max = p2;
                
                baseOp.Perform(marks, airBrush, output);
                p1.Y = (ushort)(p1.Y + yDir); p2.Y = p1.Y;
                wallOp.Min = p1; wallOp.Max = p2;
                baseOp.Min = p1; baseOp.Max = p2;
            }
        }
    }
}
