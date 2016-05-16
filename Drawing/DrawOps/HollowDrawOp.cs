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
using MCGalaxy.Drawing.Brushes;

namespace MCGalaxy.Drawing.Ops {

    public class HollowDrawOp : DrawOp {
        
        public override string Name { get { return "Hollow"; } }
        public byte Skip;
        
        public override long GetBlocksAffected(Level lvl, Vec3U16[] marks) {
            Vec3U16 p1 = marks[0], p2 = marks[1];
            return (p2.X - p1.X + 1) * (p2.Y - p1.Y + 1) * (p2.Z - p1.Z + 1);
        }
        
        public override void Perform(Vec3U16[] marks, Player p, Level lvl, Brush brush) {
            Vec3U16 p1 = marks[0], p2 = marks[1];
            for (ushort y = p1.Y; y <= p2.Y; y++)
                for (ushort z = p1.Z; z <= p2.Z; z++)
                    for (ushort x = p1.X; x <= p2.X; x++)
            {
                bool hollow = true;
                byte tile = lvl.GetTile(x, y, z);
                if (!Block.RightClick(Block.Convert(tile), true) && tile != Skip) {
                    CheckTile(lvl, x - 1, y, z, ref hollow);
                    CheckTile(lvl, x + 1, y, z, ref hollow);
                    CheckTile(lvl, x, y - 1, z, ref hollow);
                    CheckTile(lvl, x, y + 1, z, ref hollow);
                    CheckTile(lvl, x, y, z - 1, ref hollow);
                    CheckTile(lvl, x, y, z + 1, ref hollow);
                } else {
                    hollow = false;
                }
                if (hollow) 
                    PlaceBlock(p, lvl, x, y, z, Block.air, 0);
            }
        }
        
        void CheckTile(Level lvl, int x, int y, int z, ref bool hollow) {
            byte tile = lvl.GetTile((ushort)x, (ushort)y, (ushort)z);
            if (Block.RightClick(Block.Convert(tile)) || tile == Skip)
                hollow = false;
        }
    }
}
