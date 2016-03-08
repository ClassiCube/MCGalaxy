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
        
        public static List<Player> players;
        /// <summary> Array of all current online players. </summary>
        /// <remarks> Note this field is highly volatile, you should cache references to it. </remarks>
        public volatile static Player[] Online = new Player[0];
        static readonly object listLock = new object();
        
        public static void Add(Player p) {
            lock (listLock) {
                players.Add(p);
                Player[] list = new Player[Online.Length + 1];
                for (int i = 0; i < Online.Length; i++)
                    list[i] = Online[i];
                
                list[Online.Length] = p;
                Online = list;
            }
        }
        
        public static void Remove(Player p) {
            lock (listLock) {
                players.Remove(p);
                if (Online.Length == 0) return;
                
                Player[] list = new Player[Online.Length - 1];
                for (int i = 0, j = 0; i < Online.Length; i++) {
                    if (Online[i] == p) continue;
                    
                    // For some reason player wasn't in the list
                    if (j == list.Length) return;
                    list[j] = Online[i]; j++;
                }
                Online = list;
            }
        }
        
        public static List<Player> GetPlayers() { return new List<Player>(PlayerInfo.players); }
        
        public static Group GetGroup(string name) {
            return Group.findPlayerGroup(name);
        }
        
        public static string GetColor(string name) {
            return GetGroup(name).color;
        }
        
        const StringComparison comp = StringComparison.OrdinalIgnoreCase;
        public static Player Find(string name) {
            Player[] players = PlayerInfo.Online;
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
        
        public static Player FindExact(string name) {
            Player[] players = PlayerInfo.Online;

            foreach (Player p in players) {
                if (p.name.Equals(name, comp)) return p;
            }
            return null;
        }
        
        public static Player FindNick(string nick) {
        	Player[] players = PlayerInfo.Online;
            Player match = null; int matches = 0;

            foreach (Player p in players) {
                if (p.DisplayName.Equals(nick, comp)) return p;
                if (p.DisplayName.IndexOf(nick, comp) >= 0) {
                    match = p; matches++;
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
            p.timeLogged = DateTime.Now;
            
            const string query = "INSERT INTO Players (Name, IP, FirstLogin, LastLogin, totalLogin, Title, totalDeaths, Money, totalBlocks, totalKicked, TimeSpent) " +
                "VALUES ('{0}', '{1}', '{2:yyyy-MM-dd HH:mm:ss}', '{3:yyyy-MM-dd HH:mm:ss}', {4}, '{5}', {6}, {7}, {8}, {9}, '{10}')";
            Database.executeQuery(String.Format(query, p.name, p.ip, p.firstLogin, DateTime.Now, p.totalLogins, 
                                                p.prefix, p.overallDeath, p.money, p.loginBlocks, p.totalKicked, p.time.ToDBTime()));
            string ecoQuery = "INSERT INTO Economy (player, money, total, purchase, payment, salary, fine) VALUES ('" + p.name + "', " + p.money + ", 0, '%cNone', '%cNone', '%cNone', '%cNone')";
            Database.executeQuery(ecoQuery);
        }
        
        internal static void LoadInfo(DataTable playerDb, Player p) {
            DataRow row = playerDb.Rows[0];
            p.totalLogins = int.Parse(row["totalLogin"].ToString()) + 1;
            p.time = row["TimeSpent"].ToString().ParseDBTime();
            p.userID = int.Parse(row["ID"].ToString());
            p.firstLogin = DateTime.Parse(row["firstLogin"].ToString());
            p.timeLogged = DateTime.Now;
            
            p.title = row["Title"].ToString().Trim();
            if (p.title != "")
                p.title = p.title.Replace("[", "").Replace("]", "");
            
            p.titlecolor = row["title_color"].ToString().Trim();
            if (p.titlecolor != "")
                p.titlecolor = Colors.Parse(p.titlecolor);
            else
                p.titlecolor = "";
            
            p.color = row["color"].ToString().Trim();
            if (p.color != "")
                p.color = Colors.Parse(p.color);
            else
                p.color = p.group.color;
            
            p.overallDeath = int.Parse(row["TotalDeaths"].ToString());
            p.overallBlocks = long.Parse(row["totalBlocks"].ToString().Trim());
            //money = int.Parse(playerDb.Rows[0]["Money"].ToString());
            p.money = Economy.RetrieveEcoStats(p.name).money;
            p.totalKicked = int.Parse(row["totalKicked"].ToString());
        }
        
        public static OfflinePlayer FindOffline(string name, bool fullStats = false) {
            OfflinePlayer pl = new OfflinePlayer();
            Database.AddParams("@Name", name);
            string syntax = Server.useMySQL ? "SELECT * FROM Players WHERE Name=@Name COLLATE utf8_general_ci" :
                "SELECT * FROM Players WHERE Name=@Name COLLATE NOCASE";
            using (DataTable playerDB = Database.fillData(syntax)) {
                if (playerDB.Rows.Count == 0) {
                    return null;
                } else {
                    DataRow row = playerDB.Rows[0];
                    pl.name = row["Name"].ToString().Trim();
                    pl.ip = row["IP"].ToString().Trim();   
                    
                    pl.totalTime = row["TimeSpent"].ToString();
                    pl.firstLogin = row["FirstLogin"].ToString();
                    pl.lastLogin = row["LastLogin"].ToString();                    
                    if (!fullStats) return pl;
                    
                    pl.title = row["Title"].ToString().Trim();
                    pl.titleColor = Colors.Parse(row["title_color"].ToString().Trim());
                    pl.color = Colors.Parse(row["color"].ToString().Trim());
                    
                    pl.money = row["Money"].ToString();
                    pl.deaths = row["TotalDeaths"].ToString();
                    pl.blocks = row["totalBlocks"].ToString();
                    pl.logins = row["totalLogin"].ToString();
                    pl.kicks = row["totalKicked"].ToString();
                }
            }
            return pl;
        }
    }
    
    public class OfflinePlayer {
        public string name, color, title, titleColor;
        public string money, deaths, blocks, logins, kicks;
        public string totalTime, firstLogin, lastLogin, ip;
    }
}
