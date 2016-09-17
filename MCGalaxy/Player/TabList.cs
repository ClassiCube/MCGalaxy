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
using MCGalaxy.Games;

namespace MCGalaxy {

    /// <summary> Contains methods related to the management of tab list of player names. </summary>
    public static class TabList {
        
    	 // Want nobody to be at top of list, banned to be bottom of list.
        const LevelPermission offset = LevelPermission.Nobody;
        
        /// <summary> Adds the given player to that player's tab list (if their client supports it). </summary>
        public static void Add(Player dst, Player p, byte id) {
            if (!dst.hasExtList) return;
            byte grpPerm = (byte)(offset - p.Rank);
            if (!Server.TablistRankSorted) grpPerm = 0;
            
            string name, group;
            GetEntry(p, dst, out name, out group);
            dst.SendExtAddPlayerName(id, p.truename, name, group, grpPerm);
        }
        
        /// <summary> Gets the name and the group name for the given player. </summary>
        public static void GetEntry(Player p, Player dst, out string name, out string group) {
        	string col = Entities.GetSupportedCol(dst, p.color);          
            group = Server.TablistGlobal ? "On " + p.level.name : "&fPlayers";    	
            name = col + p.truename;
            
            if (p.hidden && p.IsAfk) { name += " &f(Hid, &7AFK)"; return; }
            if (p.hidden) name += " &f(Hid)";
            if (p.IsAfk) { name += " &7(AFK)"; return; }
            
            IGame game = p.level.CurrentGame();
            if (game != null) game.GetTabName(p, dst, ref name, ref group);
        }
        
        /// <summary> Adds the given bot to that player's tab list (if their client support it). </summary>
        public static void Add(Player dst, PlayerBot b) {
            if (!dst.hasExtList) return;
            dst.SendExtAddPlayerName(b.id, b.SkinName, b.color + b.name, "Bots", 0);
        }
        
        /// <summary> Removes the given player from player's tab list (if their client supports it). </summary>
        public static void Remove(Player dst, byte id) {
            if (!dst.hasExtList) return;
            dst.SendExtRemovePlayerName(id);
        }
        
        /// <summary> Updates the tab list entry for this player to all other players 
        /// (whose clients support it) who can see the player in the tab list. </summary>
        internal static void Update(Player p, bool self) {
        	if (Server.TablistGlobal) UpdateAll(p, self);
        	else UpdateToLevel(p, self);
        }
        
        
    	/// <summary> Updates the tab list entry for this player to all other players 
        /// (whose clients support it) in the server. </summary>
        internal static void UpdateAll(Player p, bool self) {
        	if (!Server.TablistGlobal) return;
            Player[] players = PlayerInfo.Online.Items;
            foreach (Player other in players) {
            	if (p == other) {
            		if (self) Add(other, p, 0xFF);
            		continue;
            	}
            	
                if (Entities.CanSeeEntity(other, p))
                    Add(other, p, p.id);
                if (Entities.CanSeeEntity(p, other))
                    Add(p, other, other.id);
            }
        }
        
        /// <summary> Updates the tab list entry for this player to all other players 
        /// (whose clients support it) in the server. </summary>
        internal static void RemoveAll(Player p, bool self, bool toVisible) {
        	if (!Server.TablistGlobal) return;
            Player[] players = PlayerInfo.Online.Items;
            foreach (Player other in players) {               
            	if (p == other) {
            		if (self) Remove(other, 0xFF); 
            		continue;
            	}
            	
                bool despawn = !Entities.CanSeeEntity(other, p);
                if (toVisible) despawn = !despawn;
                if (despawn) Remove(other, p.id);
                
                despawn = !Entities.CanSeeEntity(p, other);
                if (toVisible) despawn = !despawn;
                if (despawn) Remove(p, other.id);
            }
        }
        
        /// <summary> Updates the tab list entry for this player to all other players 
        /// (whose clients support it) in the player's world. </summary>
        internal static void UpdateToLevel(Player p, bool self) {
        	if (Server.TablistGlobal) return;
            Player[] players = PlayerInfo.Online.Items;          
            foreach (Player other in players) {
                if ((other.Loading && p != other) || p.level != other.level) continue;
                
                if (p != other && Entities.CanSeeEntity(other, p)) {
                    Add(other, p, p.id);
                } else if (p == other && self) {
                    Add(other, p, 0xFF);
                }
            }
        }
    }
}
