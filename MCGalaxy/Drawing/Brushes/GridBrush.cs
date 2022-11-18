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
    public class GridBrush : Brush
    {
        readonly BlockID gridBlock, cellBlock, borderBlock;
        readonly int gridSize, blocksCount;

        public GridBrush(BlockID[] blocks, int[] counts) {
            gridBlock   = blocks[0];
            cellBlock   = blocks[1];
            borderBlock = blocks.Length > 2 ? blocks[2] : Block.Invalid;

            gridSize    = counts[0];
            blocksCount = counts[0] + counts[1];
        }

        public override string Name { get { return "Grid"; } }

        public override BlockID NextBlock(DrawOp op) {
            int dx = (op.Coords.X - op.Min.X) % blocksCount; if (dx < 0) dx += blocksCount;
            int dy = (op.Coords.Y - op.Min.Y) % blocksCount; if (dy < 0) dy += blocksCount;
            int dz = (op.Coords.Z - op.Min.Z) % blocksCount; if (dz < 0) dz += blocksCount;

            // On the grid boundary planes
            if (dx < gridSize || dy < gridSize || dz < gridSize) 
            {
                if (dx < gridSize && dz < gridSize) return gridBlock;
                if (dx < gridSize && dy < gridSize) return gridBlock;
                if (dy < gridSize && dz < gridSize) return gridBlock;

                return borderBlock;
            }
            return cellBlock;
        }
    }
}
