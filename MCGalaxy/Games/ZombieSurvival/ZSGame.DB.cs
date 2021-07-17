/*
    Copyright 2010 MCLawl Team -
    Created by Snowl (David D.) and Cazzar (Cayde D.)

    Dual-licensed under the Educational Community License, Version 2.0 and
    the GNU General Public License, Version 3 (the "Licenses"); you may
    not use this file except in compliance with the Licenses. You may
    obtain a copy of the Licenses at
    
    http://www.osedu.org/licenses/ECL-2.0
    http://www.gnu.org/licenses/gpl-3.0.html
    
    Unless required by applicable law or agreed to in writing,
    software distributed under the Licenses are distributed on an "AS IS"
    BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express
    or implied. See the Licenses for the specific language governing
    permissions and limitations under the Licenses.
 */
using System;
using MCGalaxy.DB;
using MCGalaxy.SQL;

namespace MCGalaxy.Games {
    
    public sealed partial class ZSGame : RoundsGame {
        
        struct ZombieStats { public int TotalRounds, MaxRounds, TotalInfected, MaxInfected; }
        
        static TopStat statMostInfected, statMaxInfected, statMostSurvived, statMaxSurvived;
        static OfflineStatPrinter offlineZSStats;
        static OnlineStatPrinter onlineZSStats;
        static ChatToken infectedToken, survivedToken;
        
        static void HookStats() {
            if (TopStat.Stats.Contains(statMostInfected)) return; // don't duplicate
            
            statMostInfected = new TopStat("Infected", "ZombieStats", "TotalInfected",
                                           () => "Most players infected", TopStat.FormatInteger);
            statMaxInfected = new TopStat("Survived", "ZombieStats", "TotalRounds",
                                          () => "Most rounds survived", TopStat.FormatInteger);
            statMostSurvived = new TopStat("ConsecutiveInfected", "ZombieStats", "MaxInfected",
                                           () => "Most consecutive infections", TopStat.FormatInteger);
            statMaxSurvived = new TopStat("ConsecutiveSurvived", "ZombieStats", "MaxRounds",
                                          () => "Most consecutive rounds survived", TopStat.FormatInteger);
            
            infectedToken = new ChatToken("$infected", "Total number of players infected",
                                          p => Get(p).TotalInfected.ToString());
            survivedToken = new ChatToken("$survived", "Total number of rounds survived",
                                          p => Get(p).TotalRoundsSurvived.ToString());
            
            offlineZSStats = PrintOfflineZSStats;
            onlineZSStats = PrintOnlineZSStats;
            OfflineStat.Stats.Add(offlineZSStats);
            OnlineStat.Stats.Add(onlineZSStats);
            ChatTokens.Standard.Add(infectedToken);
            ChatTokens.Standard.Add(survivedToken);
            
            TopStat.Stats.Add(statMostInfected);
            TopStat.Stats.Add(statMostSurvived);
            TopStat.Stats.Add(statMaxInfected);
            TopStat.Stats.Add(statMaxSurvived);
        }
        
        static void UnhookStats() {
            OfflineStat.Stats.Remove(offlineZSStats);
            OnlineStat.Stats.Remove(onlineZSStats);
            ChatTokens.Standard.Remove(infectedToken);
            ChatTokens.Standard.Remove(survivedToken);
            
            TopStat.Stats.Remove(statMostInfected);
            TopStat.Stats.Remove(statMostSurvived);
            TopStat.Stats.Remove(statMaxInfected);
            TopStat.Stats.Remove(statMaxSurvived);
        }
        
        static void PrintOnlineZSStats(Player p, Player who) {
            ZSData data = Get(who);
            PrintZSStats(p, data.TotalRoundsSurvived, data.TotalInfected,
                         data.MaxRoundsSurvived, data.MaxInfected);
        }
        
        static void PrintOfflineZSStats(Player p, PlayerData who) {
            ZombieStats stats = LoadStats(who.Name);
            PrintZSStats(p, stats.TotalRounds, stats.TotalInfected,
                         stats.MaxRounds, stats.MaxInfected);
        }
        
        static void PrintZSStats(Player p, int rounds, int infected, int roundsMax, int infectedMax) {
            p.Message("  Survived &a{0} &Srounds (max &e{1}&S)", rounds, roundsMax);
            p.Message("  Infected &a{0} &Splayers (max &e{1}&S)", infected, infectedMax);
        }        
        
                
        static ColumnDesc[] zsTable = new ColumnDesc[] {
            new ColumnDesc("ID", ColumnType.Integer, priKey: true, autoInc: true, notNull: true),
            new ColumnDesc("Name", ColumnType.Char, 20),
            new ColumnDesc("TotalRounds", ColumnType.Int32),
            new ColumnDesc("MaxRounds", ColumnType.Int32),
            new ColumnDesc("TotalInfected", ColumnType.Int32),
            new ColumnDesc("MaxInfected", ColumnType.Int32),
            // reserve space for possible future additions
            new ColumnDesc("Additional1", ColumnType.Int32),
        };
        
        static object ReadStats(ISqlRecord record, object arg) {
            ZombieStats stats;
            stats.TotalRounds   = record.GetInt("TotalRounds");
            stats.MaxRounds     = record.GetInt("MaxRounds");
            stats.TotalInfected = record.GetInt("TotalInfected");
            stats.MaxInfected   = record.GetInt("MaxInfected");
            return stats;
        }
        
        static ZombieStats LoadStats(string name) {
            ZombieStats stats = default(ZombieStats);
            return (ZombieStats)Database.ReadRows("ZombieStats", "*", stats,
                                                  ReadStats, "WHERE Name=@0", name);
        }
        
        protected override void SaveStats(Player p) {
            ZSData data = TryGet(p);
            if (data == null || (data.TotalRoundsSurvived == 0 && data.TotalInfected == 0)) return;
            
            int count = Database.CountRows("ZombieStats", "WHERE Name=@0", p.name);
            if (count == 0) {
                Database.AddRow("ZombieStats", "TotalRounds, MaxRounds, TotalInfected, MaxInfected, Name",
            	                data.TotalRoundsSurvived, data.MaxRoundsSurvived,
            	                data.TotalInfected,       data.MaxInfected, p.name);
            } else {
                Database.UpdateRows("ZombieStats", "TotalRounds=@0, MaxRounds=@1, TotalInfected=@2, MaxInfected=@3",
            	                    "WHERE Name=@4", data.TotalRoundsSurvived, data.MaxRoundsSurvived,
            	                                     data.TotalInfected,       data.MaxInfected, p.name);
            }
        }
    }
}
