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
    
    public class ReplaceDrawOp : DrawOp {
        
        public ExtBlock Include;
        
        public ReplaceDrawOp(byte type, byte extType) {
            Include = new ExtBlock(type, extType);
        }
        
        public override string Name { get { return "Replace"; } }
        
        public override int GetBlocksAffected(Level lvl, Vec3U16[] marks) {
            Vec3U16 p1 = marks[0], p2 = marks[1];
            return (p2.X - p1.X + 1) * (p2.Y - p1.Y + 1) * (p2.Z - p1.Z + 1);
        }
        
        public override void Perform(Vec3U16[] marks, Player p, Level lvl, Brush brush) {
            Vec3U16 p1 = marks[0], p2 = marks[1];
            for (ushort y = p1.Y; y <= p2.Y; y++)
                for (ushort z = p1.Z; z <= p2.Z; z++)
                    for (ushort x = p1.X; x <= p2.X; x++)
            {
                byte tile = lvl.GetTile(x, y, z), extTile = 0;
                if (tile == Block.custom_block) extTile = lvl.GetExtTile(x, y, z);
                
                if (tile == Include.Type && (tile != Block.custom_block || extTile == Include.ExtType))
                    PlaceBlock(p, lvl, x, y, z, brush);
            }
        }
    }
    
    public class ReplaceNotDrawOp : DrawOp {
        
        public ExtBlock Exclude;
        
        public ReplaceNotDrawOp(byte type, byte extType) {
            Exclude = new ExtBlock(type, extType);
        }
        
        public override string Name { get { return "ReplaceNot"; } }
        
        public override int GetBlocksAffected(Level lvl, Vec3U16[] marks) {
            Vec3U16 p1 = marks[0], p2 = marks[1];
            return (p2.X - p1.X + 1) * (p2.Y - p1.Y + 1) * (p2.Z - p1.Z + 1);
        }
        
        public override void Perform(Vec3U16[] marks, Player p, Level lvl, Brush brush) {
            Vec3U16 p1 = marks[0], p2 = marks[1];
            for (ushort y = p1.Y; y <= p2.Y; y++)
                for (ushort z = p1.Z; z <= p2.Z; z++)
                    for (ushort x = p1.X; x <= p2.X; x++)
            {
                byte tile = lvl.GetTile(x, y, z), extTile = 0;
                if (tile == Block.custom_block) extTile = lvl.GetExtTile(x, y, z);
                
                if (tile != Exclude.Type || (tile == Block.custom_block && extTile != Exclude.ExtType)) {
                    PlaceBlock(p, lvl, x, y, z, brush);
                }
            }
        }
    }
}