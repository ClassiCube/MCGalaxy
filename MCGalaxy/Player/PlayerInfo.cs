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
        
        public static string GetColoredName(Player p, string name) {
            Player target = FindExact(name);
            // TODO: select color from database?
            return target != null && p.CanSee(target) ? target.ColoredName
                : Group.GroupIn(name).Color + name.RemoveLastPlus();
        }
        
        /// <summary> Calculates default color for the given player. </summary>
        public static string DefaultColor(Player p) {
            string col = PlayerDB.FindColor(p);
            return col.Length > 0 ? col : p.group.Color;
        }
        
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
                if (!p.hidden) { 
                    uniqueIPs[p.ip] = true;
                }
            }
            return uniqueIPs.Count;
        }
        
        public static Player FindMatches(Player pl, string name, bool onlyCanSee = true) {
            int matches; return FindMatches(pl, name, out matches, onlyCanSee);
        }
        
        public static Player FindMatches(Player pl, string name,
                                         out int matches, bool onlyCanSee = true) {
            matches = 0;
            if (!Formatter.ValidName(pl, name, "player")) return null;
            
            return Matcher.Find(pl, name, out matches, Online.Items,
                                p => pl.CanSee(p) || !onlyCanSee,
                                p => p.name, "online players");
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

            foreach (Player p in players) {
                if (p.name.CaselessEq(name)) return p;
            }
            return null;
        }
        
        
        public static PlayerData FindData(string name) {
            string suffix = Database.Backend.CaselessWhereSuffix;
            object raw = Database.Backend.ReadRows("Players", "*",
                                                   null, PlayerData.Read,
                                                   "WHERE Name=@0" + suffix, name);
            return (PlayerData)raw;
        }

        public static string FindName(string name) {
            string suffix = Database.Backend.CaselessWhereSuffix;
            return Database.ReadString("Players", "Name", "WHERE Name=@0" + suffix, name);
        }
        
        public static string FindIP(string name) {
            string suffix = Database.Backend.CaselessWhereSuffix;
            return Database.ReadString("Players", "IP", "WHERE Name=@0" + suffix, name);
        }
        
        public static string FindOfflineIPMatches(Player p, string name, out string ip) {
            string[] match = PlayerDB.MatchValues(p, name, "Name,IP");
            ip   = match == null ? null : match[1];
            return match == null ? null : match[0];
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
            Database.Backend.ReadRows("Players", "Name", names, ReadAccounts, "WHERE IP=@0", ip);
            
            // TODO: should we instead do save() when the player logs in
            // by checking online players we avoid a DB write though
            Player[] players = PlayerInfo.Online.Items;
            foreach (Player p in players) {
                if (p.ip != ip) continue;
                if (!names.CaselessContains(p.name)) names.Add(p.name);
            }
            return names;
        }
    }
}
