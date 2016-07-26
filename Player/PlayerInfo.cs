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
using System.Text;
using MCGalaxy.SQL;

namespace MCGalaxy {
    
    public static class PlayerInfo {
        
        public static List<Player> players;
        /// <summary> Array of all currently online players. </summary>
        /// <remarks> Note this field is highly volatile, you should cache references to the items array. </remarks>
        public static VolatileArray<Player> Online = new VolatileArray<Player>(true);
        
        public static List<Player> GetPlayers() { return new List<Player>(players); }
        
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
        
        public static Player FindMatches(Player pl, string name, out int matches, bool onlyCanSee = true) {
            matches = 0;
            if (!Player.ValidName(name)) {
                Player.Message(pl, "\"{0}\" is not a valid player name.", name); return null;
            }
            
            return Extensions.FindMatches<Player>(pl, name, out matches, Online.Items,
                                                  p => Entities.CanSee(pl, p) || !onlyCanSee,
                                                  p => p.name, "online players");
        }
        
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
        
        internal static void CreateInfo(Player p) {
            p.prefix = "";
            p.time = new TimeSpan(0, 0, 0, 1);
            p.title = "";
            p.titlecolor = "";
            p.color = p.group.color;
            p.money = 0;
            
            p.firstLogin = DateTime.Now;
            p.totalLogins = 1;
            p.totalKicked = 0;
            p.overallDeath = 0;
            p.overallBlocks = 0;
            
            const string query = "INSERT INTO Players (Name, IP, FirstLogin, LastLogin, totalLogin, Title, totalDeaths, Money, totalBlocks, totalKicked, TimeSpent) " +
                "VALUES ('{0}', '{1}', '{2:yyyy-MM-dd HH:mm:ss}', '{3:yyyy-MM-dd HH:mm:ss}', {4}, '{5}', {6}, {7}, {8}, {9}, '{10}')";
            Database.executeQuery(String.Format(query, p.name, p.ip, p.firstLogin, DateTime.Now, p.totalLogins,
                                                p.title, p.overallDeath, p.money, p.loginBlocks, p.totalKicked, p.time.ToDBTime()));
            string ecoQuery = "INSERT INTO Economy (player, money, total, purchase, payment, salary, fine) VALUES ('" + p.name + "', " + p.money + ", 0, '%cNone', '%cNone', '%cNone', '%cNone')";
            Database.executeQuery(ecoQuery);
        }
        
        internal static void LoadInfo(DataTable playerDb, Player p) {
            DataRow row = playerDb.Rows[0];
            p.totalLogins = int.Parse(row["totalLogin"].ToString()) + 1;
            p.time = row["TimeSpent"].ToString().ParseDBTime();
            p.userID = int.Parse(row["ID"].ToString());
            p.firstLogin = DateTime.Parse(row["firstLogin"].ToString());
            p.lastLogin = DateTime.Parse(row["LastLogin"].ToString());
            
            p.title = row["Title"].ToString().Trim();
            if (p.title != "")
                p.title = p.title.Replace("[", "").Replace("]", "");
            
            p.titlecolor = ParseColor(row["title_color"]);           
            p.color = ParseColor(row["color"]);
            if (p.color == "") p.color = p.group.color;
            
            p.overallDeath = int.Parse(row["TotalDeaths"].ToString());
            p.overallBlocks = long.Parse(row["totalBlocks"].ToString().Trim());
            //money = int.Parse(playerDb.Rows[0]["Money"].ToString());
            p.money = Economy.RetrieveEcoStats(p.name).money;
            p.loginMoney = p.money;
            p.totalKicked = int.Parse(row["totalKicked"].ToString());
        }
        
        public static OfflinePlayer Find(string name, bool fullStats = false) {
            using (DataTable results = Query(name, "*")) {
                if (results.Rows.Count == 0) return null;
                return FillInfo(results.Rows[0], fullStats);
            }
        }
        
        public static string FindName(string name) {
            using (DataTable playerDB = Query(name, "Name")) {
                if (playerDB.Rows.Count == 0) return null;
                return playerDB.Rows[0]["Name"].ToString().Trim();
            }
        }
        
        public static string FindIP(string name) {
            using (DataTable results = Query(name, "IP")) {
                if (results.Rows.Count == 0) return null;
                return results.Rows[0]["IP"].ToString().Trim();
            }
        }
        
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

        public static string FindOfflineNameMatches(Player p, string name) {
            using (DataTable results = QueryMulti(name, "Name")) {
                int matches = 0;
                DataRow row = Extensions.FindMatches<DataRow>(p, name, out matches, results.Rows,
                                                              r => true, r => r["Name"].ToString(), "players", 20);
                return row == null ? null : row["Name"].ToString();
            }
        }
        
        public static OfflinePlayer FindOfflineMatches(Player p, string name) {
            using (DataTable results = QueryMulti(name, "*")) {
                int matches = 0;
                DataRow row = Extensions.FindMatches<DataRow>(p, name, out matches, results.Rows,
                                                              r => true, r => r["Name"].ToString(), "players", 20);
                return row == null ? null : FillInfo(row, true);
            }
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
        
        static OfflinePlayer FillInfo(DataRow row, bool fullStats) {
            OfflinePlayer pl = new OfflinePlayer();
            pl.name = row["Name"].ToString().Trim();
            pl.ip = row["IP"].ToString().Trim();
            
            pl.totalTime = row["TimeSpent"].ToString();
            pl.firstLogin = row["FirstLogin"].ToString();
            pl.lastLogin = row["LastLogin"].ToString();
            if (!fullStats) return pl;
            
            pl.title = row["Title"].ToString().Trim();
            pl.titleColor = ParseColor(row["title_color"]);
            pl.color = ParseColor(row["color"]);
            
            pl.money = row["Money"].ToString();
            pl.deaths = row["TotalDeaths"].ToString();
            pl.blocks = row["totalBlocks"].ToString();
            pl.logins = row["totalLogin"].ToString();
            pl.kicks = row["totalKicked"].ToString();
            return pl;
        }
        
        static string ParseColor(object value) {
            string col = value.ToString().Trim();
            if (col == "") return col;
            
            // Try parse color name, then color code
            string parsed = Colors.Parse(col);
            if (parsed != "") return parsed;        
            return Colors.Name(col) == "" ? "" : col;
        }
    }
    
    public class OfflinePlayer {
        public string name, color, title, titleColor;
        public string money, deaths, blocks, logins, kicks;
        public string totalTime, firstLogin, lastLogin, ip;
    }
}
