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
using MCGalaxy.Events.EntityEvents;
using MCGalaxy.Network;

namespace MCGalaxy {

    /// <summary> Contains methods related to the management of tab list of player names. </summary>
    public static class TabList {
         // Want nobody to be at top of list, banned to be bottom of list.
        const LevelPermission offset = LevelPermission.Nobody;
        
        /// <summary> Adds the given player to that player's tab list (if their client supports it). </summary>
        public static void Add(Player dst, Player p, byte id) {
            if (!dst.hasExtList) return;
            byte grpPerm = (byte)(offset - p.Rank);
            if (!ServerConfig.TablistRankSorted) grpPerm = 0;
            
            string name, group;
            GetEntry(p, dst, out name, out group);
            dst.Send(Packet.ExtAddPlayerName(id, p.truename, name, group, grpPerm, dst.hasCP437));
        }
        
        static void GetEntry(Player p, Player dst, out string name, out string group) {
            string col = Entities.GetSupportedCol(dst, p.color);          
            group = ServerConfig.TablistGlobal ? "On " + p.level.name : "&fPlayers";
            name = col + p.truename;
            OnTabListEntryAddedEvent.Call(p, ref name, ref group, dst);

            if (p.hidden && p.IsAfk) { name += " &f(Hid, &7AFK)"; return; }
            if (p.hidden) name += " &f(Hid)";
            if (p.IsAfk) name += " &7(AFK)";
        }
        
        /// <summary> Adds the given bot to that player's tab list (if their client support it). </summary>
        public static void Add(Player dst, PlayerBot b) {
            if (!dst.hasExtList) return;
            
            string name = b.color + b.name, group = "Bots";
            OnTabListEntryAddedEvent.Call(b, ref name, ref group, dst);
            dst.Send(Packet.ExtAddPlayerName(b.id, b.SkinName, name, group, 0, dst.hasCP437));
        }
        
        /// <summary> Removes the given player from player's tab list (if their client supports it). </summary>
        public static void Remove(Player dst, Entity entity) {
            if (!dst.hasExtList) return;
            
            OnTabListEntryRemovedEvent.Call(entity, dst);
            byte id = dst == entity ? Entities.SelfID : entity.EntityID;
            dst.Send(Packet.ExtRemovePlayerName(id));
        }
        
        
        /// <summary> Updates the tab list entry for this player to all other players 
        /// (whose clients support it) who can see the player in the tab list. </summary>
        internal static void Update(Player p, bool self) {
            Player[] players = PlayerInfo.Online.Items;
            foreach (Player other in players) {
                if (p == other) {
                    if (self) Add(other, p, Entities.SelfID);
                    continue;
                }
                if (!ServerConfig.TablistGlobal && p.level != other.level) continue;
                
                if (other.CanSeeEntity(p))
                    Add(other, p, p.id);
                if (p.CanSeeEntity(other))
                    Add(p, other, other.id);
            }
        }
        
        /// <summary> Updates the tab list entry for this player to all other players 
        /// (whose clients support it) in the server. </summary>
        internal static void RemoveAll(Player p, bool self, bool toVisible) {
            if (!ServerConfig.TablistGlobal) return;
            Player[] players = PlayerInfo.Online.Items;
            foreach (Player other in players) {               
                if (p == other) {
                    if (self) Remove(other, p); 
                    continue;
                }
                
                bool despawn = other.CanSeeEntity(p);
                if (!toVisible) despawn = !despawn;
                if (despawn) Remove(other, p);
                
                despawn = p.CanSeeEntity(other);
                if (!toVisible) despawn = !despawn;
                if (despawn) Remove(p, other);
            }
        }
    }
}
