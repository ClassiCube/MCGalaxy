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
using MCGalaxy.SQL;

namespace MCGalaxy {
    public static class PlayerInfo {
		
        /// <summary> Array of all currently online players. </summary>
        /// <remarks> Note this field is highly volatile, you should cache references to the items array. </remarks>
        public static VolatileArray<Player> Online = new VolatileArray<Player>(true);
        [Obsolete("Use PlayerInfo.Online.Items")]
        public static List<Player> players;
        
        public static Group GetGroup(string name) { return Group.findPlayerGroup(name); }
        
        public static string GetColor(string name) { return GetGroup(name).color; }
        
        const StringComparison comp = StringComparison.OrdinalIgnoreCase;
        public static Player Find(string name) {
            Player[] players = PlayerInfo.Online.Items;
            Player match = null; int matches = 0;
            name = name.ToLower();

            foreach (Player p in players) {
                if (p.name.Equals(name, comp)) return p;
                if (p.name.IndexOf(name, comp) >= 0) {
                    match = p; matches++;
                }
            }
            return matches == 1 ? match : null;
        }
        
        public static Player FindMatches(Player pl, string name, bool onlyCanSee = true) {
            int matches = 0; return FindMatches(pl, name, out matches, onlyCanSee);
        }
        
        public static Player FindMatches(Player pl, string name, 
                                         out int matches, bool onlyCanSee = true) {
            matches = 0;
            if (!Player.ValidName(name)) {
                Player.Message(pl, "\"{0}\" is not a valid player name.", name); return null;
            }
            
            return Utils.FindMatches<Player>(pl, name, out matches, Online.Items,
                                             p => Entities.CanSee(pl, p) || !onlyCanSee,
                                             p => p.name, "online players");
        }
        
        /// <summary> Finds the online player whose name caselessly exactly matches the given name. </summary>
        /// <returns> Player instance if an exact match is found, null if not. </returns>
        public static Player FindExact(string name) {
            Player[] players = PlayerInfo.Online.Items;

            foreach (Player p in players) {
                if (p.name.Equals(name, comp)) return p;
            }
            return null;
        }
        
        public static Player FindNick(Player p, string nick) {
            nick = Colors.StripColors(nick);
            Player[] players = PlayerInfo.Online.Items;
            Player match = null; int matches = 0;

            foreach (Player pl in players) {
                if (!Entities.CanSee(p, pl)) continue;
                string name = Colors.StripColors(pl.DisplayName);
                
                if (name.Equals(nick, comp)) return pl;
                if (name.IndexOf(nick, comp) >= 0) {
                    match = pl; matches++;
                }
            }
            return matches == 1 ? match : null;
        }
 
        
        /// <summary> Retrieves from the database the player data for the player 
        /// whose name caselessly exactly matches the given name. </summary>
        /// <returns> PlayerData instance if found, null if not. </returns>
        public static PlayerData FindData(string name) {
            using (DataTable results = Query(name, "*")) {
                if (results.Rows.Count == 0) return null;
                return PlayerData.Fill(results.Rows[0]);
            }
        }
        
        /// <summary> Retrieves from the database the actual name for the player 
        /// whose name caselessly exactly matches the given name. </summary>
        /// <returns> Correctly cased name if found, null if not. </returns>
        public static string FindName(string name) {
            using (DataTable playerDB = Query(name, "Name")) {
                if (playerDB.Rows.Count == 0) return null;
                return playerDB.Rows[0]["Name"].ToString().Trim();
            }
        }

        /// <summary> Retrieves from the database the last IP address for the
        /// player whose name caselessly exactly matches the given name. </summary>
        /// <returns> Last IP address if found, null if not. </returns>
        public static string FindIP(string name) {
            using (DataTable results = Query(name, "IP")) {
                if (results.Rows.Count == 0) return null;
                return results.Rows[0]["IP"].ToString().Trim();
            }
        }
        
        
        public static PlayerData FindOfflineMatches(Player p, string name) {
            using (DataTable results = QueryMulti(name, "*")) {
                int matches = 0;
                DataRow row = Utils.FindMatches<DataRow>(p, name, out matches, results.Rows,
                                                         r => true, r => r["Name"].ToString(), "players", 20);
                return row == null ? null : PlayerData.Fill(row);
            }
        }
        
        public static string FindOfflineNameMatches(Player p, string name) {
            using (DataTable results = QueryMulti(name, "Name")) {
                int matches = 0;
                DataRow row = Utils.FindMatches<DataRow>(p, name, out matches, results.Rows,
                                                         r => true, r => r["Name"].ToString(), "players", 20);
                return row == null ? null : row["Name"].ToString();
            }
        }
        
        public static string FindOfflineIPMatches(Player p, string name) {
            using (DataTable results = QueryMulti(name, "Name, IP")) {
                int matches = 0;
                DataRow row = Utils.FindMatches<DataRow>(p, name, out matches, results.Rows,
                                                         r => true, r => r["Name"].ToString(), "players", 20);
                return row == null ? null : row["IP"].ToString();
            }
        }
        
        /// <summary> Retrieves from the database the names of all players whose 
        /// last IP address matches the given IP address. </summary>
        public static List<string> FindAccounts(string ip) {
            DataTable clones = Database.Fill("SELECT Name FROM Players WHERE IP=@0", ip);
            List<string> alts = new List<string>();
            
            foreach (DataRow row in clones.Rows) {
                string name = row["Name"].ToString();
                if (!alts.CaselessContains(name))
                    alts.Add(name);
            }
            clones.Dispose();
            return alts;
        }
        
        
        static DataTable Query(string name, string selector) {
            string syntax = Server.useMySQL ?
                "SELECT " + selector + " FROM Players WHERE Name=@0 COLLATE utf8_general_ci" :
                "SELECT " + selector + " FROM Players WHERE Name=@0 COLLATE NOCASE";
            return Database.Fill(syntax, name);
        }
        
        static DataTable QueryMulti(string name, string selector) {
            string syntax = Server.useMySQL ?
                "SELECT " + selector + " FROM Players WHERE Name LIKE @0 LIMIT 21" :
                "SELECT " + selector + " FROM Players WHERE Name LIKE @0 LIMIT 21 COLLATE NOCASE";
            return Database.Fill(syntax, "%" + name + "%");
        }
    }
}
