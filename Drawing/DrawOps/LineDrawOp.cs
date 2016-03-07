/*
    Copyright 2011 MCGalaxy
        
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

namespace MCGalaxy.Drawing.Ops {
    
    public class LineDrawOp : DrawOp {
        
        public bool WallsMode;
        public int MaxLength = int.MaxValue;
        
        public override string Name { get { return "Line"; } }
        
        public override bool MinMaxCoords { get { return false; } }
        
        public override int GetBlocksAffected(Level lvl, ushort x1, ushort y1, ushort z1, ushort x2, ushort y2, ushort z2) {
            double dx = Math.Abs(x2 - x1) + 0.25, dy = Math.Abs(y2 - y1) + 0.25, dz = Math.Abs(z2 - z1) + 0.25;
            if (WallsMode) {
                int baseLen = (int)Math.Ceiling(Math.Sqrt(dx * dx + dz * dz));
                return Math.Min(baseLen, MaxLength) * (Math.Abs(y2 - y1) + 1);
            } else {
                int baseLen = (int)Math.Ceiling(Math.Sqrt(dx * dx + dy * dy + dz * dz));
                return Math.Min(baseLen, MaxLength);
            }
        }
        
        public override void Perform(ushort x1, ushort y1, ushort z1, ushort x2,
                                     ushort y2, ushort z2, Player p, Level lvl, Brush brush) {
            List<FillPos> buffer = new List<FillPos>();
            DrawLine(x1, y1, z1, MaxLength, x2, y2, z2, buffer);
            if (WallsMode) {
                ushort yy1 = y1, yy2 = y2;
                y1 = Math.Min(yy1, yy2); y2 = Math.Max(yy1, yy2);
            }
            
            for (int i = 0; i < buffer.Count; i++) {
                FillPos pos = buffer[i];
                if (WallsMode) {
                    for (ushort yy = y1; yy <= y2; yy++)
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
