/*
    Copyright 2011 MCForge
        
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
    
    public class LineDrawOp : DrawOp {
        
        public bool WallsMode;
        public int MaxLength = int.MaxValue;
        
        public override string Name { get { return "Line"; } }
        
        public override long GetBlocksAffected(Level lvl, Vec3S32[] marks) {
        	Vec3S32 p1 = marks[0], p2 = marks[1];
            double dx = Math.Abs(p2.X - p1.X) + 0.25, dy = Math.Abs(p2.Y - p1.Y) + 0.25, dz = Math.Abs(p2.Z - p1.Z) + 0.25;
            if (WallsMode) {
                int baseLen = (int)Math.Ceiling(Math.Sqrt(dx * dx + dz * dz));
                return Math.Min(baseLen, MaxLength) * (Math.Abs(p2.Y - p1.Y) + 1);
            } else {
                int baseLen = (int)Math.Ceiling(Math.Sqrt(dx * dx + dy * dy + dz * dz));
                return Math.Min(baseLen, MaxLength);
            }
        }
        
        public override void Perform(Vec3S32[] marks, Player p, Level lvl, Brush brush) {
        	Vec3U16 p1 = Clamp(marks[0]), p2 = Clamp(marks[1]);
            List<FillPos> buffer = new List<FillPos>();
            DrawLine(p1.X, p1.Y, p1.Z, MaxLength, p2.X, p2.Y, p2.Z, buffer);
            if (WallsMode) {
                ushort yy1 = p1.Y, yy2 = p2.Y;
                p1.Y = Math.Min(yy1, yy2); p2.Y = Math.Max(yy1, yy2);
            }
            
            for (int i = 0; i < buffer.Count; i++) {
                FillPos pos = buffer[i];
                if (WallsMode) {
                    for (ushort yy = p1.Y; yy <= p2.Y; yy++)
                        PlaceBlock(p, lvl, pos.X, yy, pos.Z, brush);
                } else {
                    PlaceBlock(p, lvl, pos.X, pos.Y, pos.Z, brush);
                }
            }
        }
        
        internal static void DrawLine(ushort x1, ushort y1, ushort z1, int maxLen,
                                      ushort x2, ushort y2, ushort z2, List<FillPos> buffer) {
            Line lx, ly, lz;
            int[] pixel = { x1, y1, z1 };
            int dx = x2 - x1, dy = y2 - y1, dz = z2 - z1;
            lx.inc = Math.Sign(dx); ly.inc = Math.Sign(dy); lz.inc = Math.Sign(dz);

            int xLen = Math.Abs(dx), yLen = Math.Abs(dy), zLen = Math.Abs(dz);
            lx.dx2 = xLen << 1; ly.dx2 = yLen << 1; lz.dx2 = zLen << 1;
            lx.index = 0; ly.index = 1; lz.index = 2;

            if (xLen >= yLen && xLen >= zLen)
                DoLine(ly, lz, lx, xLen, pixel, maxLen, buffer);
            else if (yLen >= xLen && yLen >= zLen)
                DoLine(lx, lz, ly, yLen, pixel, maxLen, buffer);
            else
                DoLine(ly, lx, lz, zLen, pixel, maxLen, buffer);
            
            FillPos pos;
            pos.X = (ushort)pixel[0]; pos.Y = (ushort)pixel[1]; pos.Z = (ushort)pixel[2];
            buffer.Add(pos);
        }
        
        struct Line { public int dx2, inc, index; }
        
        static void DoLine(Line l1, Line l2, Line l3, int len, 
                           int[] pixel, int maxLen, List<FillPos> buffer) {
            int err_1 = l1.dx2 - len, err_2 = l2.dx2 - len;
            FillPos pos;
            for (int i = 0; i < len && i < (maxLen - 1); i++) {
                pos.X = (ushort)pixel[0]; pos.Y = (ushort)pixel[1]; pos.Z = (ushort)pixel[2];
                buffer.Add(pos);

                if (err_1 > 0) {
                    pixel[l1.index] += l1.inc;
                    err_1 -= l3.dx2;
                }
                if (err_2 > 0) {
                    pixel[l2.index] += l2.inc;
                    err_2 -= l3.dx2;
                }
                err_1 += l1.dx2; err_2 += l2.dx2;
                pixel[l3.index] += l3.inc;
            }
        }
    }
}
