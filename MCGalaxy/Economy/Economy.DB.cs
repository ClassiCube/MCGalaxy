/*
    Copyright 2011 MCForge
        
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
using MCGalaxy.DB;
using MCGalaxy.SQL;

namespace MCGalaxy.Eco {
    public static partial class Economy {
        
        static ColumnDesc[] createEconomy = new ColumnDesc[] {
            new ColumnDesc("player", ColumnType.VarChar, 20, priKey: true),
            new ColumnDesc("money", ColumnType.Int32),
            new ColumnDesc("total", ColumnType.Integer),
            new ColumnDesc("purchase", ColumnType.VarChar, 255),
            new ColumnDesc("payment", ColumnType.VarChar, 255),
            new ColumnDesc("salary", ColumnType.VarChar, 255),
            new ColumnDesc("fine", ColumnType.VarChar, 255),
        };
        
        public static void LoadDatabase() {
            Database.Backend.CreateTable("Economy", createEconomy);
            using (DataTable eco = Database.Backend.GetRows("Economy", "*"))
                foreach (DataRow row in eco.Rows)
            {
                int money = PlayerData.ParseInt(row["money"].ToString());
                if (money == 0) continue;
                
                EcoStats stats = default(EcoStats);
                stats.Player = row["player"].ToString();
                ParseRow(row, ref stats);
                
                UpdateMoney(stats.Player, money);
                UpdateStats(stats);
            }
        }
        
        public static string FindMatches(Player p, string name, out int money) {
            return PlayerInfo.FindOfflineMoneyMatches(p, name, out money);
        }
        
        public static void UpdateMoney(string name, int money) {
            Database.Backend.UpdateRows("Players", "Money = @1", "WHERE Name = @0", name, money);
        }
        

        public struct EcoStats {
            public string Player, Purchase, Payment, Salary, Fine; public int TotalSpent;
        }
        
        public static void UpdateStats(EcoStats stats) {
            Database.Backend.AddOrReplaceRow("Economy", "player, money, total, purchase, payment, salary, fine",
                                             stats.Player, 0, stats.TotalSpent, stats.Purchase,
                                             stats.Payment, stats.Salary, stats.Fine);
        }
        
        static string ParseField(object raw) {
            if (raw == null) return "%cNone";
            string value = raw.ToString();
            return (value.Length == 0 || value.CaselessEq("NULL")) ? "%cNone" : value;
        }
        
        static void ParseRow(DataRow row, ref EcoStats stats) {
            stats.Payment  = ParseField(row["payment"]);
            stats.Purchase = ParseField(row["purchase"]);
            stats.Salary   = ParseField(row["salary"]);
            stats.Fine     = ParseField(row["fine"]);
            stats.TotalSpent = PlayerData.ParseInt(row["total"].ToString());
        }

        public static EcoStats RetrieveStats(string name) {
            EcoStats stats = default(EcoStats);
            stats.Player   = name;
            stats.Purchase = "%cNone";
            stats.Payment  = "%cNone";
            stats.Salary   = "%cNone";
            stats.Fine     = "%cNone";
            
            using (DataTable eco = Database.Backend.GetRows("Economy", "*", "WHERE player=@0", name)) {
                if (eco.Rows.Count > 0) ParseRow(eco.Rows[0], ref stats);
            }
            return stats;
        }
    }
}