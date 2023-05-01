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
using MCGalaxy.Eco;
using MCGalaxy.Games;
using MCGalaxy.SQL;

namespace MCGalaxy.Modules.Games.ZS
{    
    public sealed partial class ZSGame : RoundsGame 
    {       
        struct ZombieStats { public int TotalRounds, MaxRounds, TotalInfected, MaxInfected; }
        
        static TopStat statMostInfected, statMaxInfected, statMostSurvived, statMaxSurvived;
        static OfflineStatPrinter offlineZSStats;
        static OnlineStatPrinter onlineZSStats;
        static ChatToken infectedToken, survivedToken;
        
        static void HookStats() {
            statMostInfected = new DBTopStat("Infected", "Most players infected",
        	                                 "ZombieStats", "TotalInfected", TopStat.FormatInteger);
            statMaxInfected  = new DBTopStat("Survived", "Most rounds survived",
        	                                 "ZombieStats", "TotalRounds",   TopStat.FormatInteger);
            statMostSurvived = new DBTopStat("ConsecutiveInfected", "Most consecutive infections",
        	                                 "ZombieStats", "MaxInfected",   TopStat.FormatInteger);
            statMaxSurvived  = new DBTopStat("ConsecutiveSurvived", "Most consecutive rounds survived", 
        	                                 "ZombieStats", "MaxRounds",     TopStat.FormatInteger);
            
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
            
            TopStat.Register(statMostInfected);
            TopStat.Register(statMostSurvived);
            TopStat.Register(statMaxInfected);
            TopStat.Register(statMaxSurvived);
        }
        
        static void UnhookStats() {
            OfflineStat.Stats.Remove(offlineZSStats);
            OnlineStat.Stats.Remove(onlineZSStats);
            ChatTokens.Standard.Remove(infectedToken);
            ChatTokens.Standard.Remove(survivedToken);
            
            TopStat.Unregister(statMostInfected);
            TopStat.Unregister(statMostSurvived);
            TopStat.Unregister(statMaxInfected);
            TopStat.Unregister(statMaxSurvived);
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
        
        static ZombieStats ParseStats(ISqlRecord record) {
            ZombieStats stats;
            stats.TotalRounds   = record.GetInt("TotalRounds");
            stats.MaxRounds     = record.GetInt("MaxRounds");
            stats.TotalInfected = record.GetInt("TotalInfected");
            stats.MaxInfected   = record.GetInt("MaxInfected");
            return stats;
        }
        
        static ZombieStats LoadStats(string name) {
            ZombieStats stats = default(ZombieStats);
            Database.ReadRows("ZombieStats", "*", 
                                record => stats = ParseStats(record), 
                                "WHERE Name=@0", name);
            return stats;
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
        
        
        static void HookCommands() {
            Command.TryRegister(true, cmdAka, cmdAlive, cmdBounties, cmdBounty,
                                cmdDisinfect, cmdHuman, cmdInfect, cmdInfected,
                                cmdLastLevels, cmdQueue, cmdShowQueue);
        }
        
        static void UnhookCommands() {
            Command.Unregister(cmdAka, cmdAlive, cmdBounties, cmdBounty,
                               cmdDisinfect, cmdHuman, cmdInfect, cmdInfected,
                               cmdLastLevels, cmdQueue, cmdShowQueue);
        }
        
        static Command cmdAka        = new CmdAka();
        static Command cmdAlive      = new CmdAlive();
        static Command cmdBounties   = new CmdBounties();
        static Command cmdBounty     = new CmdBounty();
        static Command cmdDisinfect  = new CmdDisInfect();
        static Command cmdHuman      = new CmdHuman();
        static Command cmdInfect     = new CmdInfect();
        static Command cmdInfected   = new CmdInfected();
        static Command cmdLastLevels = new CmdLastLevels();
        static Command cmdQueue      = new CmdQueue();
        static Command cmdShowQueue  = new CmdShowQueue();
        
        
        static void HookItems() {
            Economy.RegisterItem(itemQueue);
            Economy.RegisterItem(itemBlocks);
            Economy.RegisterItem(itemRevive);
            Economy.RegisterItem(itemInfectMsg);
            Economy.RegisterItem(itemInv);
        }
        
        static void UnhookItems() {
            Economy.Items.Remove(itemQueue);
            Economy.Items.Remove(itemBlocks);
            Economy.Items.Remove(itemRevive);
            Economy.Items.Remove(itemInfectMsg);
            Economy.Items.Remove(itemInv);
        }       
        
        static Item itemQueue     = new QueueLevelItem();
        static Item itemBlocks    = new BlocksItem();
        static Item itemRevive    = new ReviveItem();
        static Item itemInfectMsg = new InfectMessageItem();
        static Item itemInv       = new InvisibilityItem();
    }
}
