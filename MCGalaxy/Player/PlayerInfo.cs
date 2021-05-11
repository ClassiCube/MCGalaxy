/*
Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCGalaxy)
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
using System.Data;
using MCGalaxy.DB;
using MCGalaxy.SQL;

namespace MCGalaxy {
    public static class PlayerInfo {
        
        /// <summary> Array of all currently online players. </summary>
        /// <remarks> Note this field is highly volatile, you should cache references to the items array. </remarks>
        public static VolatileArray<Player> Online = new VolatileArray<Player>(true);
        
        public static Group GetGroup(string name) {
            Player target = FindExact(name);
            return target != null ? target.group : Group.GroupIn(name);
        }
        
        [Obsolete("Use p.FormatNick instead")]
        public static string GetColoredName(Player p, string name) { return p.FormatNick(name); }
        
        /// <summary> Calculates default color for the given player. </summary>
        public static string DefaultColor(Player p) {
            string col = PlayerDB.FindColor(p);
            return col.Length > 0 ? col : p.group.Color;
        }
        
        /// <summary> Returns the number of non-hidden players that are currently online </summary>
        public static int NonHiddenCount() {
            Player[] players = Online.Items;
            int count = 0;
            foreach (Player p in players) { if (!p.hidden) count++; }
            return count;
        }
        
        public static int NonHiddenUniqueIPCount() {
            Player[] players = Online.Items;
            Dictionary<string, bool> uniqueIPs = new Dictionary<string, bool>();
            
            foreach (Player p in players) {
                if (!p.hidden) uniqueIPs[p.ip] = true;
            }
            return uniqueIPs.Count;
        }
        // TODO: remove _useless parameter. but this breaks backwards compatibility with plugins
        
        /// <summary> Matches given name against the names of all online players that the given player can see </summary>
        /// <returns> A Player instance if exactly one match was found </returns>
        public static Player FindMatches(Player pl, string name, bool _useless = false) {
            int matches; return FindMatches(pl, name, out matches);
        }
        
        /// <summary> Matches given name against the names of all online players that the given player can see </summary>
        /// <param name="matches"> Outputs the number of matching players </param>
        /// <returns> A Player instance if exactly one match was found </returns>
        public static Player FindMatches(Player pl, string name, out int matches, bool _useless = false) {
            matches = 0;
            if (!Formatter.ValidName(pl, name, "player")) return null;
            
            // Try to exactly match name first (because names have + at end)
            Player exact = FindExact(name);
            if (exact != null && pl.CanSee(exact)) { matches = 1; return exact; }
            
            return Matcher.Find(pl, name, out matches, Online.Items,
                                p => pl.CanSee(p), p => p.name, "online players");
        }
        
        public static string FindMatchesPreferOnline(Player p, string name) {
            if (!Formatter.ValidName(p, name, "player")) return null;
            int matches;
            Player target = FindMatches(p, name, out matches);
            
            if (matches > 1) return null;
            if (target != null) return target.name;
            p.Message("Searching PlayerDB for \"{0}\"..", name);
            return PlayerDB.MatchNames(p, name);
        }
        
        /// <summary> Finds the online player whose name caselessly exactly matches the given name. </summary>
        /// <returns> Player instance if an exact match is found, null if not. </returns>
        public static Player FindExact(string name) {
            Player[] players = PlayerInfo.Online.Items;
            name = name.RemoveLastPlus();
            
            foreach (Player p in players) {
                if (p.truename.CaselessEq(name)) return p;
            }
            return null;
        }

        
        static object ReadAccounts(IDataRecord record, object arg) {
            List<string> names = (List<string>)arg;
            string name = record.GetText(0);
            
            if (!names.CaselessContains(name)) names.Add(name);
            return arg;
        }
        
        /// <summary> Retrieves names of all players whose IP address matches the given IP address. </summary>
        /// <remarks> This is current IP for online players, last IP for offline players from the database. </remarks>
        public static List<string> FindAccounts(string ip) {
            List<string> names = new List<string>();
            Database.ReadRows("Players", "Name", names, ReadAccounts, "WHERE IP=@0", ip);
            
            // TODO: should we instead do save() when the player logs in
            // by checking online players we avoid a DB write though
            Player[] players = PlayerInfo.Online.Items;
            foreach (Player p in players) {
                if (p.ip != ip) continue;
                if (!names.CaselessContains(p.name)) names.Add(p.name);
            }
            return names;
        }
        
        /// <summary> Filters input list to only players that the source player can see. </summary>
        internal static List<Player> OnlyCanSee(Player p, LevelPermission plRank, 
                                                IEnumerable<Player> players) {
            List<Player> list = new List<Player>();
            foreach (Player pl in players) {
                if (p.CanSee(pl, plRank)) list.Add(pl);
            }
            return list;
        }
        
        
        /// <summary> Returns all online players that the given player can see, ordered by rank </summary>
        public static List<OnlineListEntry> GetOnlineList(Player p, LevelPermission plRank, out int total) {
            List<OnlineListEntry> all = new List<OnlineListEntry>();
            total = 0;
            
            foreach (Group group in Group.GroupList) {
                OnlineListEntry e = OnlineOfRank(p, plRank, group);
                total += e.players.Count;
                all.Add(e);
            }
            
            // Highest ranks first
            all.Reverse();
            return all;
        }
           
        static OnlineListEntry OnlineOfRank(Player p, LevelPermission plRank, Group group) {
            OnlineListEntry entry = new OnlineListEntry();
            entry.group   = group;
            entry.players = new List<Player>();
            
            Player[] online = PlayerInfo.Online.Items;
            foreach (Player pl in online) {
                if (pl.group != group || !p.CanSee(pl, plRank)) continue;
                entry.players.Add(pl);
            }
            return entry;
        }
    }
    
    public class OnlineListEntry { 
        public Group group; public List<Player> players;
        
        public static string GetFlags(Player p) {
            string flags = "";
                        
            if (p.hidden)       flags += "-hidden";
            if (p.muted)        flags += "-muted";
            if (p.frozen)       flags += "-frozen";
            if (p.Game.Referee) flags += "-ref";
            if (p.IsAfk)        flags += "-afk";
            if (p.Unverified)   flags += "-unverified";
            return flags;
        }
    }
}
