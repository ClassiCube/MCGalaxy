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
using MCGalaxy.Drawing.Ops;
using MCGalaxy.Maths;

namespace MCGalaxy.Generator.Foliage {
    public sealed class OakTree : Tree {
        
        int numBranches, maxExtent, maxBranchHeight, trunkHeight;
        List<Vec3S32> branch = new List<Vec3S32>();
        
        public override int MinSize { get { return 0; } }
                
        public override long EstimateBlocksAffected() { return (long)height * height * height; }
                
        public override int DefaultSize(Random rnd) { return rnd.Next(0, 11); }
        
        public override void SetData(Random rnd, int value) {
            numBranches = value;
            this.rnd = rnd;
            
            maxExtent = (int)(numBranches * 0.3f) + 1;
            maxBranchHeight = (int)(numBranches * 0.3f) + 1;
            trunkHeight = rnd.Next(4, 5 + (int)(maxBranchHeight / 1.5f));
            
            // calculate variables
            size = Math.Max(maxExtent, 2); // max of initial cluster and all other clusters
            height = (trunkHeight * 5 / 4) + (maxBranchHeight + 2); // branchEndY
        }
        
        public override void Generate(ushort x, ushort y, ushort z, TreeOutput output) {
            // Generate base tree
            Vec3S32 p1 = new Vec3S32(x, y, z);
            Vec3S32 p2 = new Vec3S32(x, y + trunkHeight, z);
            Line(p1, p2, output);
            GenCluster(x, y + trunkHeight, z, output);
            
            // Generate branches
            for (int i = 0; i < numBranches; i++) {
                MakeBranch(x, y, z, output);
            }
        }

        
        void MakeBranch(int x, int y, int z, TreeOutput output) {
            int branchX = rnd.Next(-maxExtent, maxExtent);
            int branchZ = rnd.Next(-maxExtent, maxExtent);
            int branchStartY = rnd.Next((trunkHeight / 4) + 1, trunkHeight + (trunkHeight / 4));
            int branchEndY = branchStartY + rnd.Next(1, maxBranchHeight + 3);
            
            Vec3S32 p1 = new Vec3S32(x, y + branchStartY, z);
            Vec3S32 p2 = new Vec3S32(x + branchX, y + branchEndY, z + branchZ);
            Line(p1, p2, output);
            
            GenCluster(x + branchX, y + branchEndY, z + branchZ, output);
        }
        
        void GenCluster(int x, int y, int z, TreeOutput output) {
            Vec3S32 p1, p2;
            //cross X
            p1 = new Vec3S32(x - 1, y - 1, z);
            p2 = new Vec3S32(x + 1, y + 3, z);
            Cuboid(p1, p2, output);
            
            //cross z
            p1 = new Vec3S32(x, y - 1, z - 1);
            p2 = new Vec3S32(x, y + 3, z + 1);
            Cuboid(p1, p2, output);
            
            //cuboid x
            p1 = new Vec3S32(x - 2, y, z - 1);
            p2 = new Vec3S32(x + 2, y + 2, z + 1);
            Cuboid(p1, p2, output);
            
            //cuboid z
            p1 = new Vec3S32(x - 1, y, z - 2);
            p2 = new Vec3S32(x + 1, y + 2, z + 2);
            Cuboid(p1, p2, output);
        }
        
        
        void Cuboid(Vec3S32 p1, Vec3S32 p2, TreeOutput output) {
            for (int y = p1.Y; y <= p2.Y; y++)
                for (int z = p1.Z; z <= p2.Z; z++)
                    for (int x = p1.X; x <= p2.X; x++)
            {
                output((ushort)x, (ushort)y, (ushort)z, Block.Leaves);
            }
        }
        
        void Line(Vec3S32 p1, Vec3S32 p2, TreeOutput output) {
            LineDrawOp.DrawLine(p1.X, p1.Y, p1.Z, 10000, p2.X, p2.Y, p2.Z, branch);
            
            foreach (Vec3S32 P in branch) {
                output((ushort)P.X, (ushort)P.Y, (ushort)P.Z, Block.Log);
            }
            branch.Clear();
        }
    }
}