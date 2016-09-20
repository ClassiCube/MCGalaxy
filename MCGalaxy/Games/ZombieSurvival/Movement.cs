/*
    Copyright 2015 MCGalaxy
    
    Dual-licensed under the Educational Community License, Version 2.0 and
    the GNU General Public License, Version 3 (the "Licenses"); you may
    not use this file except in compliance with the Licenses. You may
    obtain a copy of the Licenses at
    
    http://www.osedu.org/licenses/ECL-2.0
    http://www.gnu.org/licenses/gpl-3.0.html
    
    Unless required by applicable law or agreed to in writing,
    software distributed under the Licenses are distributed on an "AS IS"
    BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express
    or implied. See the Licenses for the specific language governing
    permissions and limitations under the Licenses.
 */
using System;

namespace MCGalaxy.Games.ZS {   
    internal static class Movement {
        
        public static bool DetectNoclip(Player p, ushort x, ushort y, ushort z) {
            if (p.Game.Referee) return false;
            
            /*if (p.CheckIfInsideBlock()) {
                p.SendPos(0xFF, p.pos[0], p.pos[1], p.pos[2], p.rot[0], p.rot[1]);
                return true;
            }*/
            return false;
        }
        
        public static bool DetectSpeedhack(Player p, ushort x, ushort y, ushort z) {
            if (p.Game.Referee) return false;
            
            int dx = Math.Abs(p.pos[0] - x), dz = Math.Abs(p.pos[2] - z);
            if (dx >= ZombieGameProps.MaxMoveDistance || dz >= ZombieGameProps.MaxMoveDistance) {
                p.SendPos(0xFF, p.pos[0], p.pos[1], p.pos[2], p.rot[0], p.rot[1]);
                return true;
            }
            return false;
        }
    }
}
