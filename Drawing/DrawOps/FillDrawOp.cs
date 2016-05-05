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

namespace MCGalaxy.Drawing.Ops {
    
    public class FillDrawOp : DrawOp {
        
        public List<int> Positions;
        
        public override string Name { get { return "Fill"; } }
        
        public override long GetBlocksAffected(Level lvl, Vec3U16[] marks) {
            return Positions.Count;
        }
        
        public override bool CanDraw(Vec3U16[] marks, Player p, out long affected) {
            affected = GetBlocksAffected(p.level, marks);
            if (affected > p.group.maxBlocks) {
                Player.Message(p, "You tried to fill over " + p.group.maxBlocks + " blocks.");
                Player.Message(p, "You cannot fill more than " + p.group.maxBlocks + ".");
                return false;
            }
            return true;
        }
        
        public override void Perform(Vec3U16[] marks, Player p, Level lvl, Brush brush) {
            ushort x, y, z;
            foreach (int pos in Positions) {
                lvl.IntToPos(pos, out x, out y, out z);
                PlaceBlock(p, lvl, x, y, z, brush);
            }
        }
    }
}
