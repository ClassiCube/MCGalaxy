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
using MCGalaxy.Maths;

namespace MCGalaxy.Drawing.Ops 
{
    public class LineDrawOp : DrawOp 
    {
        public override string Name { get { return "Line"; } }
        public bool WallsMode;
        public int MaxLength = int.MaxValue;
        
        public override long BlocksAffected(Level lvl, Vec3S32[] marks) {
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
        
        public override void Perform(Vec3S32[] marks, Brush brush, DrawOpOutput output) {
            Vec3U16 p1 = Clamp(marks[0]), p2 = Clamp(marks[1]);
            List<Vec3S32> buffer = new List<Vec3S32>();
            DrawLine(p1.X, p1.Y, p1.Z, MaxLength, p2.X, p2.Y, p2.Z, buffer);
            if (WallsMode) {
                ushort yy1 = p1.Y, yy2 = p2.Y;
                p1.Y = Math.Min(yy1, yy2); p2.Y = Math.Max(yy1, yy2);
            }
            
            for (int i = 0; i < buffer.Count; i++) {
                Vec3U16 pos = (Vec3U16)buffer[i];
                if (WallsMode) {
                    for (ushort yy = p1.Y; yy <= p2.Y; yy++)
                        output(Place(pos.X, yy, pos.Z, brush));
                } else {
                    output(Place(pos.X, pos.Y, pos.Z, brush));
                }
            }
        }
        
        internal static void DrawLine(int x1, int y1, int z1, int maxLen,
                                      int x2, int y2, int z2, List<Vec3S32> buffer) {
            Line lx, ly, lz;
            int[] pixel = new int[] { x1, y1, z1 };
            int dx = x2 - x1, dy = y2 - y1, dz = z2 - z1;
            lx.dir = Math.Sign(dx); ly.dir = Math.Sign(dy); lz.dir = Math.Sign(dz);

            int xLen = Math.Abs(dx), yLen = Math.Abs(dy), zLen = Math.Abs(dz);
            lx.len2 = xLen << 1; ly.len2 = yLen << 1; lz.len2 = zLen << 1;
            lx.axis = 0; ly.axis = 1; lz.axis = 2;

            if (xLen >= yLen && xLen >= zLen)
                DoLine(ly, lz, lx, xLen, pixel, maxLen, buffer);
            else if (yLen >= xLen && yLen >= zLen)
                DoLine(lx, lz, ly, yLen, pixel, maxLen, buffer);
            else
                DoLine(ly, lx, lz, zLen, pixel, maxLen, buffer);
            
            Vec3S32 pos;
            pos.X = pixel[0]; pos.Y = pixel[1]; pos.Z = pixel[2];
            buffer.Add(pos);
        }
        
        struct Line { public int len2, dir, axis; }
        
        static void DoLine(Line l1, Line l2, Line l3, int len, 
                           int[] pixel, int maxLen, List<Vec3S32> buffer) {
            int err_1 = l1.len2 - len, err_2 = l2.len2 - len;
            Vec3S32 pos;
            for (int i = 0; i < len && i < (maxLen - 1); i++) {
                pos.X = pixel[0]; pos.Y = pixel[1]; pos.Z = pixel[2];
                buffer.Add(pos);

                if (err_1 > 0) {
                    pixel[l1.axis] += l1.dir;
                    err_1 -= l3.len2;
                }
                if (err_2 > 0) {
                    pixel[l2.axis] += l2.dir;
                    err_2 -= l3.len2;
                }
                err_1 += l1.len2; err_2 += l2.len2;
                pixel[l3.axis] += l3.dir;
            }
        }
    }
}
