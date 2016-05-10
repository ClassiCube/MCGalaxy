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

namespace MCGalaxy.Games {

    public abstract class IGame {
		
		/// <summary> Whether players are allowed to teleport to others when not in referee mode. </summary>
		public virtual bool TeleportAllowed { get { return true; } }
        
        /// <summary> Returns whether this game handed the player manually placing a block. </summary>
        public virtual bool HandlesManualChange(Player p, ushort x, ushort y, ushort z, 
                                                byte action, byte tile, byte b) {
            return false;
        }
        
        /// <summary> Returns whether this game handled the player moving to a new position. </summary>
        public virtual bool HandlesMovement(Player p, ushort x, ushort y, ushort z, 
                                                byte rotX, byte rotY) {
            return false;
        }
        
        /// <summary> Returns whether this game handled the player sending a chat message. </summary>
        public virtual bool HandlesChatMessage(Player p, string message) {
            return false;
        }
        
        /// <summary> Raised when a player joins the server. </summary>
        public virtual void PlayerJoinedServer(Player p) { }
        
        /// <summary> Raised when a player leaves the server. </summary>
        public virtual void PlayerLeftServer(Player p) { }
        
        /// <summary> Raised when a player joins this game. </summary>
        public virtual void PlayerJoinedGame(Player p) { }
        
        /// <summary> Raised when a player leaves this game. </summary>
        public virtual void PlayerLeftGame(Player p) { }
        
        /// <summary> Raised when a player moves to a different map/level. </summary>
        public virtual void PlayerJoinedLevel(Player p, Level lvl, Level oldLvl) { }
        
        public virtual bool PlayerCanJoinLevel(Player p, Level lvl, Level oldLvl) {
        	return true;
        }
        
        /// <summary> Raised when a player's money amount changes. </summary>
        public virtual void PlayerMoneyChanged(Player p) { }
        
        /// <summary> Raised when the server is about to send a heartbeat. </summary>
        public virtual void OnHeartbeat(ref string name) { }
        
        /// <summary> Adjusts the prefix (e.g. title) shown before the player's name in chat. </summary>
        public virtual void AdjustPrefix(Player p, ref string prefix) { }
        
        /// <summary> Gets the player's name shown in the tab list 
        /// (for clients that support a separate tab list) that are in this game. </summary>
        public virtual void GetTabName(Player p, Player dst,
                                       ref string name, ref string group) { }
    }
}
