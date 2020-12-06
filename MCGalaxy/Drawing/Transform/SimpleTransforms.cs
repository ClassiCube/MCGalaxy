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
        
        public override void Perform(Vec3S32[] marks, DrawOp op, Brush brush, DrawOpOutput output) {
            op.Perform(marks, brush, output);
        }
    }
    
    public sealed class ScaleTransform : Transform {
        
        public override string Name { get { return "Scale"; } }
        public bool CentreOrigin;
        public int XMul, XDiv, YMul, YDiv, ZMul, ZDiv;
        int dirX, dirY, dirZ, signX, signY, signZ;
        int width, height, length;
        Vec3S32 P;
        
        public void CheckScales() {
            // Need to reverse direction for negative scales
            signX = Math.Sign(XMul / XDiv);
            signY = Math.Sign(YMul / YDiv);
            signZ = Math.Sign(ZMul / ZDiv);
            
            XMul = Math.Abs(XMul); XDiv = Math.Abs(XDiv);
            YMul = Math.Abs(YMul); YDiv = Math.Abs(YDiv);
            ZMul = Math.Abs(ZMul); ZDiv = Math.Abs(ZDiv);
        }
        
        public override void GetBlocksAffected(ref long affected) {
            // NOTE: We do not the actual size of the drawop on each axis, so we take
            // the overly conversative case and use the maximum scale for all three axes.
            long x = affected * XMul / XDiv, y = affected * YMul / YDiv, z = affected * ZMul / ZDiv;
            affected = Math.Max(x, Math.Max(y, z));
        }
        
        public override void Perform(Vec3S32[] marks, DrawOp op, Brush brush, DrawOpOutput output) {
            P = (op.Min + op.Max) / 2;
            dirX = 1; dirY = 1; dirZ = 1;
            
            Level lvl = op.Level;
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
            int dx = (b.X - P.X) * signX, dy = (b.Y - P.Y) * signY, dz = (b.Z - P.Z) * signZ;
            
            int begX = P.X + dx * XMul / XDiv, endX = P.X + (dx + dirX) * XMul / XDiv;
            int begY = P.Y + dy * YMul / YDiv, endY = P.Y + (dy + dirY) * YMul / YDiv;
            int begZ = P.Z + dz * ZMul / ZDiv, endZ = P.Z + (dz + dirZ) * ZMul / ZDiv;
            
            // Scale out until we hit the next block
            for (int y = begY; y != endY; y += dirY)
                for (int z = begZ; z != endZ; z += dirZ)
                    for (int x = begX; x != endX; x += dirX)
            {
                if (x < 0 || y < 0 || z < 0 || x >= width || y >= height || z >= length) continue;
                b.X = (ushort)x; b.Y = (ushort)y; b.Z = (ushort)z;
                output(b);
            }
        }
    }
}
