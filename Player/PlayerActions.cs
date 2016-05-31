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
namespace MCGalaxy {
    public static class PlayerActions {

        /// <summary> Moves the player to the specified block coordinates. </summary>
        public static void MoveCoords(Player p, int bX, int bY, int bZ) {
            MoveCoords(p, bX, bY, bZ, p.rot[0], p.rot[1]);
        }
        
        /// <summary> Moves the player to the specified block coordinates. </summary>
        public static void MoveCoords(Player p, int bX, int bY, int bZ, 
                                      byte rotX, byte rotY) {
            ushort x = (ushort)(bX * 32 + 16);
            ushort y = (ushort)(bY * 32);
            ushort z = (ushort)(bZ * 32 + 16);
            p.SendOwnFeetPos(x, y, z, rotX, rotY);
        }
        
        /// <summary> Moves the player to the specified map. </summary>
        public static void ChangeMap(Player p, string name) {
            Command.all.Find("goto").Use(p, name);
        }
    }
}
