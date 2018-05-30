/*
    Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCGalaxy)
    
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
// Copyright 2009, 2010 Matvei Stefarov <me@matvei.org>
/*
This generator was developed by Neko_baron.

Ideas, concepts, and code were used from the following two sources:
1) Isaac McGarvey's 'perlin noise generator' code
2) http://www.lighthouse3d.com/opengl/terrain/index.php3?introduction

 */
using System;
using System.Collections.Generic;
using MCGalaxy.Drawing.Brushes;
using MCGalaxy.Generator.Foliage;
using MCGalaxy.Maths;

namespace MCGalaxy.Drawing.Ops {
    public class TreeDrawOp : DrawOp {
        public override string Name { get { return "Tree"; } }
        
        public Random random = new Random();
        public Tree Tree;       
        public int Size = -1;
        
        public override long BlocksAffected(Level lvl, Vec3S32[] marks) { return Tree.EstimateBlocksAffected(); }
        
        public override void Perform(Vec3S32[] marks, Brush brush, DrawOpOutput output) {
            Vec3U16 P = Clamp(marks[0]);
            Level lvl = Level;
            
            // Need to make a list of leave coordinates, because otherwise
            // drawing tree with /scale won't work properly
            List<Vec3U16> leaves = new List<Vec3U16>();
            
            Tree.Generate(P.X, P.Y, P.Z, (xT, yT, zT, bT) =>
                          {
                              if (bT != Block.Leaves) {
                                  output(Place(xT, yT, zT, bT));
                              } else if (lvl.IsAirAt(xT, yT, zT)) {
                                  leaves.Add(new Vec3U16(xT, yT, zT));
                              }
                          });
            
            foreach (Vec3U16 pos in leaves) {
                output(Place(pos.X, pos.Y, pos.Z, brush));
            }
        }
        
        public override void SetMarks(Vec3S32[] marks) {
            base.SetMarks(marks);
            int value = Size != -1 ? Size : Tree.DefaultSize(random);
            Tree.SetData(random, value);
            
            Max.Y += Tree.height;
            Min.X -= Tree.size; Min.Z -= Tree.size;
            Max.X += Tree.size; Max.Z += Tree.size;
        }
    }
}
