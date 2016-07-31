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
using System.Data;
using MCGalaxy.SQL;

namespace MCGalaxy {   
    public class PlayerData {
        public string Name, Color, Title, TitleColor;
        public string Money, Deaths, Blocks, Cuboided, Logins, Kicks;
        public string TotalTime, FirstLogin, LastLogin, IP, UserID;
        
        internal static void Create(Player p) {
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
            p.TotalBlocksDrawn = 0;
            string now = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            
            const string query = "INSERT INTO Players (Name, IP, FirstLogin, LastLogin, totalLogin, Title, totalDeaths" +
                ", Money, totalBlocks, totalKicked, TimeSpent) VALUES (@0, @1, @2, @2, @3, @4, @5, @5, @5, @5, @6)";
            Database.Execute(query, 
                             p.name, p.ip, now, 1, "", 0, p.time.ToDBTime());
            
            const string ecoQuery = "INSERT INTO Economy (player, money, total, purchase, payment, salary, fine) " +
                "VALUES (@0, @1, @2, @3, @3, @3, @3)";
            Database.Execute(ecoQuery, 
                             p.name, p.money, 0, "%cNone");
        }
        
        internal static void Load(DataTable playerDb, Player p) {
            PlayerData data = PlayerData.Fill(playerDb.Rows[0]);
            p.totalLogins = int.Parse(data.Logins) + 1;
            p.time = data.TotalTime.ParseDBTime();
            p.DatabaseID = int.Parse(data.UserID);
            p.firstLogin = DateTime.Parse(data.FirstLogin);
            p.lastLogin = DateTime.Parse(data.LastLogin);
            
            p.title = data.Title;
            if (p.title != "") p.title = p.title.Replace("[", "").Replace("]", "");
            
            p.titlecolor = data.TitleColor;
            p.color = data.Color;
            if (p.color == "") p.color = p.group.color;
            
            p.overallDeath = int.Parse(data.Deaths);
            p.overallBlocks = long.Parse(data.Blocks);
            p.TotalBlocksDrawn = long.Parse(data.Cuboided);
            
            //money = int.Parse(data.money);
            p.money = Economy.RetrieveEcoStats(p.name).money;
            p.loginMoney = p.money;
            p.totalKicked = int.Parse(data.Kicks);
        }
        
        public static PlayerData Fill(DataRow row) {
            PlayerData pl = new PlayerData();
            pl.Name = row["Name"].ToString().Trim();
            pl.IP = row["IP"].ToString().Trim();
            pl.UserID = row["ID"].ToString().Trim();
            
            pl.TotalTime = row["TimeSpent"].ToString();
            pl.FirstLogin = row["FirstLogin"].ToString();
            pl.LastLogin = row["LastLogin"].ToString();
            
            pl.Title = row["Title"].ToString().Trim();
            pl.TitleColor = ParseColor(row["title_color"]);
            pl.Color = ParseColor(row["color"]);
            
            pl.Money = row["Money"].ToString();
            pl.Deaths = row["TotalDeaths"].ToString();
            pl.Blocks = row["totalBlocks"].ToString();
            pl.Cuboided = row["totalCuboided"].ToString();
            pl.Logins = row["totalLogin"].ToString();
            pl.Kicks = row["totalKicked"].ToString();
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
}
