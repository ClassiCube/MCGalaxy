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

namespace MCGalaxy.Drawing.Transforms {
    public sealed class NoTransform : Transform {
        
        public override string Name { get { return "None"; } }
        public static NoTransform Instance = new NoTransform();
        
        public override IEnumerable<DrawOpBlock> Perform(Vec3S32[] marks, Player p,
                                                         Level lvl, DrawOp op, Brush brush) {
            return op.Perform(marks, p, lvl, brush);
        }
    }
    
    public sealed class ScaleTransform : Transform {
        
        public override string Name { get { return "Scale"; } }
        public bool CentreOrigin;
        public int XMul, XDiv, YMul, YDiv, ZMul, ZDiv;
        
        public override void GetBlocksAffected(ref long affected) {
            // NOTE: We do not the actual size of the drawop on each axis, so we take
            // the overly conversative case and use the maximum scale for all three axes.
            long x = affected * XMul / XDiv, y = affected * YMul / YDiv, z = affected * ZMul / ZDiv;
            affected = Math.Max(x, Math.Max(y, z));
        }
        
        public override IEnumerable<DrawOpBlock> Perform(Vec3S32[] marks, Player p,
                                                         Level lvl, DrawOp op, Brush brush) {
            Vec3S32 P = CentreOrigin ? (op.Min + op.Max) / 2 : op.Origin;
            foreach (DrawOpBlock b in op.Perform(marks, p, lvl, brush)) {
                int dx = b.X - P.X, dy = b.Y - P.Y, dz = b.Z - P.Z;
                DrawOpBlock cur = b;
                
                for (int y = b.Y + dy * YMul / YDiv; y < b.Y + (dy + 1) * YMul / YDiv; y++)
                    for (int z = b.Z + dz * ZMul / ZDiv; b.Z + z < (dz + 1) * ZMul / ZDiv; z++)
                        for (int x = b.X + dx * XMul / XDiv; b.X + x < (dx + 1) * XMul / XDiv; x++)
                {
                    if (!lvl.IsValidPos(x, y, z)) continue;
                    cur.X = (ushort)x; cur.Y = (ushort)y; cur.Z = (ushort)z;
                    yield return cur;
                }
            }
        }
    }
}
