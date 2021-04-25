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
using System.Collections.Generic;
using System.Data;
using MCGalaxy.DB;
using MCGalaxy.SQL;

namespace MCGalaxy.Eco {
    public static partial class Economy {
        
        static ColumnDesc[] ecoTable = new ColumnDesc[] {
            new ColumnDesc("player", ColumnType.VarChar, 20, priKey: true),
            new ColumnDesc("money", ColumnType.Int32),
            new ColumnDesc("total", ColumnType.Int32),
            new ColumnDesc("purchase", ColumnType.VarChar, 255),
            new ColumnDesc("payment", ColumnType.VarChar, 255),
            new ColumnDesc("salary", ColumnType.VarChar, 255),
            new ColumnDesc("fine", ColumnType.VarChar, 255),
        };
        
        static object ListOld(IDataRecord record, object arg) {
            EcoStats stats = ParseStats(record);
            stats.__unused = record.GetInt("money");            
            ((List<EcoStats>)arg).Add(stats);
            return arg;
        }
        
        public static void LoadDatabase() {
            Database.CreateTable("Economy", ecoTable);
            
            // money used to be in the Economy table, move it back to the Players table
            List<EcoStats> outdated = new List<EcoStats>();
            Database.ReadRows("Economy", "*", outdated, ListOld, "WHERE money > 0");
            
            if (outdated.Count == 0) return;            
            Logger.Log(LogType.SystemActivity, "Upgrading economy stats..");   
            
            foreach (EcoStats stats in outdated) {
                UpdateMoney(stats.Player, stats.__unused);
                UpdateStats(stats);
            }
        }
        
        public static string FindMatches(Player p, string name, out int money) {
            string[] match = PlayerDB.MatchValues(p, name, "Name,Money");
            money = match == null ? 0    : int.Parse(match[1]);
            return  match == null ? null : match[0];
        }
        
        public static void UpdateMoney(string name, int money) {
            PlayerDB.Update(name, PlayerData.ColumnMoney, money.ToString());
        }
        

        public struct EcoStats {
            public string Player, Purchase, Payment, Salary, Fine; public int TotalSpent, __unused;
        }
        
        public static void UpdateStats(EcoStats stats) {
            Database.AddOrReplaceRow("Economy", "player, money, total, purchase, payment, salary, fine",
			                         stats.Player, 0, stats.TotalSpent, stats.Purchase,
			                         stats.Payment, stats.Salary, stats.Fine);
        }
        
        static EcoStats ParseStats(IDataRecord record) {
            EcoStats stats;
            stats.Player = record.GetText("player");
            stats.Payment  = Parse(record.GetText("payment"));
            stats.Purchase = Parse(record.GetText("purchase"));
            stats.Salary   = Parse(record.GetText("salary"));
            stats.Fine     = Parse(record.GetText("fine"));
            
            stats.TotalSpent = record.GetInt("total");
            stats.__unused   = 0;
            return stats;
        }
        
        static string Parse(string raw) {
            if (raw == null || raw.Length == 0 || raw.CaselessEq("NULL")) return null;           
            return raw.CaselessEq("%cNone") ? null : raw;
        }
        
        static object ReadStats(IDataRecord record, object arg) { return ParseStats(record); }
        public static EcoStats RetrieveStats(string name) {
            EcoStats stats = default(EcoStats);
            stats.Player   = name;
            return (EcoStats)Database.ReadRows("Economy", "*", stats, ReadStats,
                                               "WHERE player=@0", name);
        }
    }
}