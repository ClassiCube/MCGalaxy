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
using MCGalaxy.Commands.Building;
using MCGalaxy.DB;
using MCGalaxy.Drawing.Brushes;
using MCGalaxy.Util;
using MCGalaxy.Maths;
using BlockID = System.UInt16;

namespace MCGalaxy.Drawing.Ops {
    
    public class FillDrawOp : DrawOp {
        
        public List<int> Positions;
        
        public FillDrawOp() {
            Flags = BlockDBFlags.Filled;
            AffectedByTransform = false;
        }
        
        public override string Name { get { return "Fill"; } }
        
        public override long BlocksAffected(Level lvl, Vec3S32[] marks) {
            return Positions.Count;
        }
        
        public override bool CanDraw(Vec3S32[] marks, Player p, long affected) {
            if (affected > p.group.DrawLimit) {
                p.Message("You can only fill up to {0} blocks. " +
                          "This fill would affect more than {0} blocks.", p.group.DrawLimit);
                return false;
            }
            return true;
        }
        
        public override void Perform(Vec3S32[] marks, Brush brush, DrawOpOutput output) {
            ushort x, y, z;
            for (int i = 0; i < Positions.Count; i++) {
                int pos = Positions[i];
                Level.IntToPos(pos, out x, out y, out z);
                output(Place(x, y, z, brush));
            }
        }
        
        
        public unsafe static List<int> FloodFill(Player p, int index, BlockID block, DrawMode mode) {
            Level lvl = p.level;
            SparseBitSet bits = new SparseBitSet(lvl.Width, lvl.Height, lvl.Length);
            List<int> buffer = new List<int>();
            Queue<int> temp = new Queue<int>();
            
            const int max = 65536;
            int count = 0, oneY = lvl.Width * lvl.Length;
            int* pos = stackalloc int[max];
            pos[0] = index; count++;
            
            while (count > 0 && buffer.Count <= p.group.DrawLimit) {
                index = pos[count - 1]; count--;
                ushort x = (ushort)(index % lvl.Width);
                ushort y = (ushort)((index / lvl.Width) / lvl.Length);
                ushort z = (ushort)((index / lvl.Width) % lvl.Length);
                
                if (temp.Count > 0) { pos[count] = temp.Dequeue(); count++; }
                if (!bits.TrySetOn(x, y, z)) continue;
                buffer.Add(index);
                
                if (mode != DrawMode.verticalX) { // x
                    if (lvl.GetBlock((ushort)(x + 1), y, z) == block) {
                        if (count == max) { temp.Enqueue(index + 1); }
                        else { pos[count] = index + 1; count++; }
                    }
                    if (lvl.GetBlock((ushort)(x - 1), y, z) == block) {
                        if (count == max) { temp.Enqueue(index - 1); }
                        else { pos[count] = index - 1; count++; }
                    }
                }

                if (mode != DrawMode.verticalZ) { // z
                    if (lvl.GetBlock(x, y, (ushort)(z + 1)) == block) {
                        if (count == max) { temp.Enqueue(index + lvl.Width); }
                        else { pos[count] = index + lvl.Width; count++; }
                    }
                    if (lvl.GetBlock(x, y, (ushort)(z - 1)) == block) {
                        if (count == max) { temp.Enqueue(index - lvl.Width); }
                        else { pos[count] = index - lvl.Width; count++; }
                    }
                }

                if (!(mode == DrawMode.down || mode == DrawMode.layer)) { // y up
                    if (lvl.GetBlock(x, (ushort)(y + 1), z) == block) {
                        if (count == max) { temp.Enqueue(index + oneY); }
                        else { pos[count] = index + oneY; count++; }
                    }
                }

                if (!(mode == DrawMode.up || mode == DrawMode.layer)) { // y down
                    if (lvl.GetBlock(x, (ushort)(y - 1), z) == block) {
                        if (count == max) { temp.Enqueue(index - oneY); }
                        else { pos[count] = index - oneY; count++; }
                    }
                }
            }
            bits.Clear();
            return buffer;
        }
    }
}
