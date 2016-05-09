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
        
        /// <summary> Updates the tab list entry for this player to all other players 
        /// (whose clients support it) in the player's world. </summary>
        internal static void UpdateToLevel(Player p, bool self) {
            Player[] players = PlayerInfo.Online.Items;
            p.Game.lastSpawnColor = p.Game.Infected ? ZombieGame.InfectCol : p.color;          
            
            foreach (Player other in players) {
                if ((other.Loading && p != other) || p.level != other.level) continue;
                
                if (p != other && Entities.CanSeeEntity(other, p)) {
                    Add(other, p, p.id);
                } else if (p == other && self) {
                    Add(other, p, 0xFF);
                }
            }
        }
        
        // Want nobody to be at top of list, banned to be bottom of list.
        const LevelPermission offset = LevelPermission.Nobody;
        
        /// <summary> Adds the given player to that player's tab list (if their client supports it). </summary>
        public static void Add(Player dst, Player p, byte id) {
            if (!dst.hasExtList) return;
            byte grpPerm = (byte)(offset - p.group.Permission);
            if (!Server.TablistRankSorted) grpPerm = 0;
            
            if (p.IsAfk) {
                string col = Entities.GetSupportedCol(dst, p.color);
                dst.SendExtAddPlayerName(id, p.truename, col + p.truename, "&7AFK", grpPerm);
                return;
            }            
            
            if (!Server.zombie.Running || !p.Game.Infected) {
                string col = Entities.GetSupportedCol(dst, p.color);
                string group = p.Game.Referee ? "&2Referees" : "&fPlayers";
                dst.SendExtAddPlayerName(id, p.truename, col + p.truename, group, grpPerm);
                return;
            }
            
            string name = p.truename;
            if (ZombieGame.ZombieName != "" && !dst.Game.Aka)
                name = ZombieGame.ZombieName;           
            dst.SendExtAddPlayerName(id, p.truename, Colors.red + name, "&cZombies", grpPerm);
        }
        
        /// <summary> Adds the given bot to that player's tab list (if their client support it). </summary>
        public static void Add(Player dst, PlayerBot b) {
            if (!dst.hasExtList) return;
            dst.SendExtAddPlayerName(b.id, b.skinName, b.color + b.name, "Bots", 0);
        }
        
        /// <summary> Removes the given player from player's tab list (if their client supports it). </summary>
        public static void Remove(Player dst, byte id) {
            if (!dst.hasExtList) return;
            dst.SendExtRemovePlayerName(id);
        }
    }
}
