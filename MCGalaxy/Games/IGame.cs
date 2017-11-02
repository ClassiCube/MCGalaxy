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
        public string MapName;
        public Level Map;
        public abstract bool Running { get; }
        
        /// <summary> Whether players are allowed to teleport to others when not in referee mode. </summary>
        public virtual bool TeleportAllowed { get { return true; } }    
        /// <summary> Returns whether this game handled the player sending a chat message. </summary>
        public virtual bool HandlesChatMessage(Player p, string message) {  return false; }
        
        public virtual void PlayerJoinedServer(Player p) { }
        public virtual void PlayerJoinedGame(Player p) { }
        public virtual void PlayerLeftGame(Player p) { }
        public virtual void PlayerJoinedLevel(Player p, Level lvl, Level oldLvl) { }
        public virtual void OnHeartbeat(ref string name) { }
        
        /// <summary> Adjusts the prefix (e.g. title) shown before the player's name in chat. </summary>
        public virtual void AdjustPrefix(Player p, ref string prefix) { }
        
        public abstract void EndRound();
    }
}
