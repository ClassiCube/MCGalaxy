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
        
        public static string GetColoredName(Player p, string name) {
            Player target = FindExact(name);
            return target != null && Entities.CanSee(p, target) ? 
                target.ColoredName : GetColor(name) + name; // TODO: select color from database?
        }
        
        
        public static Player Find(string name) {
            Player[] players = PlayerInfo.Online.Items;
            Player match = null; int matches = 0;
            name = name.ToLower();

            foreach (Player p in players) {
                if (p.name.CaselessEq(name)) return p;
                if (p.name.CaselessContains(name)) {
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
        
        public static string FindMatchesPreferOnline(Player p, string name) {
            if (!Player.ValidName(name)) {
                Player.Message(p, "\"{0}\" is not a valid player name.", name); return null;
            }
            int matches = 0;
            Player target = FindMatches(p, name, out matches);
            
            if (matches > 1) return null;
            if (target != null) return target.name;        
            Player.Message(p, "Searching PlayerDB..");
            return FindOfflineNameMatches(p, name);
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
        
        public static Player FindNick(Player p, string nick) {
            nick = Colors.StripColors(nick);
            Player[] players = PlayerInfo.Online.Items;
            Player match = null; int matches = 0;

            foreach (Player pl in players) {
                if (!Entities.CanSee(p, pl)) continue;
                string name = Colors.StripColors(pl.DisplayName);
                
                if (name.CaselessEq(nick)) return pl;
                if (name.CaselessContains(nick)) {
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
            DataRow row = QueryMulti(p, name, "*");
            return row == null ? null : PlayerData.Fill(row);
        }
        
        public static string FindOfflineNameMatches(Player p, string name) {
            DataRow row = QueryMulti(p, name, "Name");
            return row == null ? null : row["Name"].ToString();
        }
        
        public static string FindOfflineIPMatches(Player p, string name, out string ip) {
            DataRow row = QueryMulti(p, name, "Name, IP");
            ip = row == null ? null : row["IP"].ToString();
            return row == null ? null : row["Name"].ToString();
        }
        
        public static string FindOfflineMoneyMatches(Player p, string name, out int money) {
            DataRow row = QueryMulti(p, name, "Name, Money");
            money = row == null ? 0 : PlayerData.ParseInt(row["Money"].ToString());
            return row == null ? null : row["Name"].ToString();
        }
        
        /// <summary> Retrieves from the database the names of all players whose 
        /// last IP address matches the given IP address. </summary>
        public static List<string> FindAccounts(string ip) {
            DataTable clones = Database.Backend.GetRows("Players", "Name", "WHERE IP=@0", ip);
            List<string> alts = new List<string>();
            
            foreach (DataRow row in clones.Rows) {
                string name = row["Name"].ToString();
                if (!alts.CaselessContains(name))
                    alts.Add(name);
            }
            clones.Dispose();
            return alts;
        }
        
        
        internal static DataTable Query(string name, string columns) {
            string suffix = Server.useMySQL ? " utf8_general_ci" : " NOCASE";
            return Database.Backend.GetRows("Players", columns,
                                            "WHERE Name=@0 COLLATE" + suffix, name);
        }
        
        internal static DataRow QueryMulti(Player p, string name, string columns) {
            string suffix = Server.useMySQL ? "" : " COLLATE NOCASE";
            using (DataTable results = Database.Backend.GetRows("Players", columns,
                                                                "WHERE Name LIKE @0 LIMIT 21" + suffix,
                                                                "%" + name + "%")) {
                int matches = 0;
                return Utils.FindMatches<DataRow>(p, name, out matches, results.Rows,
                                                  r => true, r => r["Name"].ToString(), "players", 20);
            }
        }
    }
}
