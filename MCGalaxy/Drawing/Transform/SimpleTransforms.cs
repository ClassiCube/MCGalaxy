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
using MCGalaxy.Drawing.Ops;
using MCGalaxy.Maths;

namespace MCGalaxy.Drawing.Transforms {
    public sealed class NoTransform : Transform {
        
        public override string Name { get { return "None"; } }
        public static NoTransform Instance = new NoTransform();
        
        public override void Perform(Vec3S32[] marks, Player p, Level lvl, 
                                     DrawOp op, Brush brush, DrawOpOutput output) {
            op.Perform(marks, brush, output);
        }
    }
    
    public sealed class ScaleTransform : Transform {
        
        public override string Name { get { return "Scale"; } }
        public bool CentreOrigin;
        public int XMul, XDiv, YMul, YDiv, ZMul, ZDiv;
        int dirX, dirY, dirZ;
        int width, height, length;
        Vec3S32 P;
        
        public override void GetBlocksAffected(ref long affected) {
            // NOTE: We do not the actual size of the drawop on each axis, so we take
            // the overly conversative case and use the maximum scale for all three axes.
            long x = affected * XMul / XDiv, y = affected * YMul / YDiv, z = affected * ZMul / ZDiv;
            affected = Math.Max(x, Math.Max(y, z));
        }
        
        public override void Perform(Vec3S32[] marks, Player p, Level lvl, 
                                     DrawOp op, Brush brush, DrawOpOutput output) {
            P = (op.Min + op.Max) / 2;
            dirX = 1; dirY = 1; dirZ = 1;
            width = lvl.Width; height = lvl.Height; length = lvl.Length;
            
            if (!CentreOrigin) {
                // Guess the direction in which we should be scaling -
                // for simplicity we assume we are scaling in positive direction
                P = op.Origin;
                dirX = op.Min.X == op.Max.X ? 1 : (P.X == op.Max.X ? -1 : 1);
                dirY = op.Min.Y == op.Max.Y ? 1 : (P.Y == op.Max.Y ? -1 : 1);
                dirZ = op.Min.Z == op.Max.Z ? 1 : (P.Z == op.Max.Z ? -1 : 1);
            }           
            op.Perform(marks, brush, b => OutputBlock(b, output));
        }
        
        void OutputBlock(DrawOpBlock b, DrawOpOutput output) {
            int dx = b.X - P.X, dy = b.Y - P.Y, dz = b.Z - P.Z;
            
            // Scale out until we hit the next block
            for (int y = P.Y + dy * YMul / YDiv; y != P.Y + (dy + dirY) * YMul / YDiv; y += dirY)
                for (int z = P.Z + dz * ZMul / ZDiv; z != P.Z + (dz + dirZ) * ZMul / ZDiv; z += dirZ)
                    for (int x = P.X + dx * XMul / XDiv; x != P.X + (dx + dirX) * XMul / XDiv; x += dirX)
            {
                if (x < 0 || y < 0 || z < 0 || x >= width || y >= height || z >= length) continue;
                b.X = (ushort)x; b.Y = (ushort)y; b.Z = (ushort)z;
                output(b);
            }
        }
    }
}
