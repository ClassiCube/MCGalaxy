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
using MCGalaxy.Drawing.Ops;
using BlockID = System.UInt16;

namespace MCGalaxy.Drawing.Brushes 
{   
    public sealed class GradientBrush : Brush 
    {
        readonly BlockID[] blocks;
        CustomModelAnimAxis axis, _axis;
        int xLen, yLen, zLen;

        public GradientBrush(BlockID[] blocks, CustomModelAnimAxis axis) {
            this.blocks = blocks;
            this.axis   = axis;
        }
        
        public override string Name { get { return "Gradient"; } }

        public override void Configure(DrawOp op, Player p) {
            xLen = op.SizeX; yLen = op.SizeY; zLen = op.SizeZ;

            if (axis <= CustomModelAnimAxis.Z) {
                _axis = axis;
            } else if (xLen >= yLen && xLen >= zLen) {
                _axis = CustomModelAnimAxis.X;
            } else if (yLen >= xLen && yLen >= zLen) {
                _axis = CustomModelAnimAxis.Y;
            } else {
                _axis = CustomModelAnimAxis.Z;
            }
        }

        public override BlockID NextBlock(DrawOp op) {
            int index = 0;

            switch (_axis)
            {
                case CustomModelAnimAxis.X:
                    index = (op.Coords.X - op.Min.X) % xLen;
                    if (index < 0) index += xLen;
                    index = index * blocks.Length / xLen;
                    break;
                case CustomModelAnimAxis.Y: 
                    index = (op.Coords.Y - op.Min.Y) % yLen;
                    if (index < 0) index += yLen;
                    index = index * blocks.Length / yLen;
                    break;
                case CustomModelAnimAxis.Z: 
                    index = (op.Coords.Z - op.Min.Z) % zLen;
                    if (index < 0) index += zLen;
                    index = index * blocks.Length / zLen;
                    break;
            }
            return blocks[index];
        }
    }
}