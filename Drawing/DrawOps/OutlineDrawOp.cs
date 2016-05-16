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

    public class OutlineDrawOp : CuboidDrawOp {
        
        public override string Name { get { return "Outline"; } }
        public byte Type, NewType;    
        
        public override void Perform(Vec3U16[] marks, Player p, Level lvl, Brush brush) {
            Vec3U16 p1 = marks[0], p2 = marks[1];
            for (ushort y = p1.Y; y <= p2.Y; y++)
                for (ushort z = p1.Z; z <= p2.Z; z++)
                    for (ushort x = p1.X; x <= p2.X; x++)
            {
                bool outline = false;
                if (lvl.GetTile((ushort)(x - 1), y, z) == Type) outline = true;
                else if (lvl.GetTile((ushort)(x + 1), y, z) == Type) outline = true;
                else if (lvl.GetTile(x, (ushort)(y - 1), z) == Type) outline = true;
                else if (lvl.GetTile(x, (ushort)(y + 1), z) == Type) outline = true;
                else if (lvl.GetTile(x, y, (ushort)(z - 1)) == Type) outline = true;
                else if (lvl.GetTile(x, y, (ushort)(z + 1)) == Type) outline = true;

                if (outline && p.level.GetTile(x, y, z) != Type)
                    PlaceBlock(p, lvl, x, y, z, NewType, 0);
            }
        }
    }
}
