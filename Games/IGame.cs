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

namespace MCGalaxy {

    public abstract class IGame {
        
        public virtual bool HandlesManualChange(Player p, ushort x, ushort y, ushort z, 
                                                byte action, byte tile, byte b) {
            return false;
        }
        
        public virtual bool HandlesMovement(Player p, ushort x, ushort y, ushort z, 
                                                byte rotX, byte rotY) {
            return false;
        }
        
        public virtual bool HandlesChatMessage(Player p, string message) {
            return false;
        }
        
        public virtual void PlayerJoinedServer(Player p) { }
        
        public virtual void PlayerLeftServer(Player p) { }
        
        public virtual void PlayerJoinedGame(Player p) { }
        
        public virtual void PlayerLeftGame(Player p) { }
    }
}
