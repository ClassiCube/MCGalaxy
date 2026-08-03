/*
    Copyright 2015-2024 MCGalaxy
    
    Dual-licensed under the Educational Community License, Version 2.0 and
    the GNU General Public License, Version 3 (the "Licenses"); you may
    not use this file except in compliance with the Licenses. You may
    obtain a copy of the Licenses at
    
    https://opensource.org/license/ecl-2-0/
    https://www.gnu.org/licenses/gpl-3.0.html
    
    Unless required by applicable law or agreed to in writing,
    software distributed under the Licenses are distributed on an "AS IS"
    BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express
    or implied. See the Licenses for the specific language governing
    permissions and limitations under the Licenses.
 */
using System;
using MCGalaxy.Events.EntityEvents;
using MCGalaxy.Network;

namespace MCGalaxy 
{
    public interface ITabListEntry
    {
        bool SharesTabListWith(Player target);
        
        string GetTabListName();
        
        string GetTabListNick(Player target);
        string GetTabListSuffix();
        
        string GetTabListGroup();
        byte   GetTabListRank ();
    }
    
    /// <summary> Contains methods related to the management of tab list of player names. </summary>
    public static class TabList 
    {
        /// <summary> Adds the given entry to that player's tab list (if their client support it). </summary>
        public static void Add(Player dst, ITabListEntry e) {
            if (!dst.hasExtList) return;
            
            string name = e.GetTabListName();
            string nick = e.GetTabListNick(dst);
            
            string group = e.GetTabListGroup();
            byte   rank  = e.GetTabListRank();
            
            OnTabListEntryAddedEvent.Call(e, ref nick, ref group, dst);
            string suffix = e.GetTabListSuffix();
            if (!string.IsNullOrEmpty(suffix)) nick += suffix;
            
            dst.EntityList.SendAddTabEntry(e, name, nick, group, rank);
        }
        
        /// <summary> Removes the given entry from player's tab list (if their client supports it). </summary>
        public static void Remove(Player dst, ITabListEntry e) {
            if (!dst.hasExtList) return;
            
            OnTabListEntryRemovedEvent.Call(e, dst);
            dst.EntityList.SendRemoveTabEntry(e);
        }
        
        
        /// <summary> Updates the tab list entry for this player to all other players 
        /// (whose clients support it) who can see the player in the tab list. </summary>
        public static void Update(Player p, bool self) {
            Player[] players = PlayerInfo.Online.Items;
            foreach (Player other in players) {
                if (p == other) {
                    if (self) Add(other, p);
                    continue;
                }
                
                if (p.SharesTabListWith(other)) Add(other, p);
                if (other.SharesTabListWith(p)) Add(p, other);
            }
        }
        
        /// <summary> Removes this tab list entry for this player to all other players 
        /// (whose clients support it) in the server. </summary>
        internal static void RemoveAll(Player p, bool self, bool toVisible) {
            if (!Server.Config.TablistGlobal) return;
            Player[] players = PlayerInfo.Online.Items;
            foreach (Player other in players) {               
                if (p == other) {
                    if (self) Remove(other, p); 
                    continue;
                }
                
                bool despawn = other.CanSee(p);
                if (!toVisible) despawn = !despawn;
                if (despawn) Remove(other, p);
                
                despawn = p.CanSee(other);
                if (!toVisible) despawn = !despawn;
                if (despawn) Remove(p, other);
            }
        }
    }
}
