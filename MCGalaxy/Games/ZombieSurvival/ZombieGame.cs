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
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Threading;
using MCGalaxy.DB;
using MCGalaxy.Commands.World;
using MCGalaxy.Games.ZS;
using MCGalaxy.SQL;

namespace MCGalaxy.Games {
    
    public struct ZombieStats {
        public int TotalRounds, MaxRounds, TotalInfected, MaxInfected;
    }
    
    public sealed partial class ZSGame {
        ZSPlugin plugin = new ZSPlugin();
        
        public void Start(ZombieGameStatus status, Level level, int rounds) {
            Status = status;
            RoundInProgress = false;
            MaxRounds = rounds + 1;
            RoundsDone = 0;
            if (!SetStartLevel(level)) return;
            
            HookStats();
            if (plugin.Game == null) { plugin.Game = this; plugin.Load(false); }
            
            Thread t = new Thread(MainLoop);
            t.Name = "MCG_ZombieGame";
            t.Start();
        }
        
        bool SetStartLevel(Level level) {
            if (level == null) {
                List<string> levels = LevelPicker.GetCandidateLevels();
                if (levels == null) return false;
                
                CurLevelName = LevelPicker.GetRandomLevel(new Random(), levels);
                CurLevel = LevelInfo.FindExact(CurLevelName)
                    ?? CmdLoad.LoadLevel(null, CurLevelName);
                if (CurLevel == null) return false;
            } else {
                CurLevelName = level.name;
                CurLevel = level;
            }
            
            Chat.MessageGlobal("A game of zombie survival is starting on: {0}", CurLevelName);
            Player[] players = PlayerInfo.Online.Items;
            foreach (Player p in players) {
                if (p.level != CurLevel) continue;
                PlayerJoinedLevel(p, p.level, p.level);
            }
            
            if (ZSConfig.SetMainLevel)
                Server.mainLevel = CurLevel;
            return true;
        }

        /// <summary> If there are no infected players left, randomly selected one of the alive players to continue the infection. </summary>
        public void AssignFirstZombie() {
            if (!Running || !RoundInProgress || Infected.Count > 0) return;
            Random random = new Random();
            Player[] alive = Alive.Items;
            if (alive.Length == 0) return;
            int index = random.Next(alive.Length);
            
            while (alive[index].Game.Referee || !alive[index].level.name.CaselessEq(CurLevelName)) {
                if (index >= alive.Length - 1) {
                    index = 0;
                    alive = Alive.Items;
                    if (alive.Length == 0) return;
                } else {
                    index++;
                }
            }
            
            Player zombie = alive[index];
            CurLevel.ChatLevel("&c" + zombie.DisplayName + " %Scontinued the infection!");
            InfectPlayer(zombie, null);
        }

        public void InfectPlayer(Player p, Player killer) {
            if (!RoundInProgress || p == null) return;
            Infected.Add(p);
            Alive.Remove(p);
            
            p.Game.CurrentRoundsSurvived = 0;
            p.Game.TimeInfected = DateTime.UtcNow;
            p.SetPrefix();
            ResetPlayerState(p, true);
            
            CheckHumanPledge(p, killer);
            CheckBounty(p, killer);
        }

        public void DisinfectPlayer(Player p) {
            if (!RoundInProgress || p == null) return;
            Infected.Remove(p);
            Alive.Add(p);
            ResetPlayerState(p, false);
        }
        
        void ResetPlayerState(Player p, bool infected) {
            p.Game.Infected = infected;
            p.Game.BlocksLeft = infected ? 25 : 50;
            string col = infected ? Colors.red : p.color;
            
            ResetInvisibility(p);
            UpdatePlayerColor(p, col);
            HUD.UpdateAllPrimary(this);
            HUD.UpdateTertiary(p);
        }
        
        void ResetInvisibility(Player p) {
            if (!p.Game.Invisible) return;
            p.SendCpeMessage(CpeMessageType.BottomRight2, "");
            p.Game.ResetInvisibility();
            Entities.GlobalSpawn(p, false);
        }

        internal void ChangeLevel(string next) {
            Player[] online = PlayerInfo.Online.Items;
            if (CurLevel != null) {
                Level.SaveSettings(CurLevel);
                CurLevel.ChatLevel("The next map has been chosen - " + Colors.red + next.ToLower());
                CurLevel.ChatLevel("Please wait while you are transfered.");
            }
            string lastLevel = CurLevelName;
            
            CurLevelName = next;
            QueuedLevel = null;
            CmdLoad.LoadLevel(null, next);
            CurLevel = LevelInfo.FindExact(next);
            if (ZSConfig.SetMainLevel)
                Server.mainLevel = CurLevel;
            
            online = PlayerInfo.Online.Items;
            List<Player> players = new List<Player>(online.Length);
            foreach (Player pl in online) {
                pl.Game.RatedMap = false;
                pl.Game.PledgeSurvive = false;
                if (!pl.level.name.CaselessEq(next) && pl.level.name.CaselessEq(lastLevel)) {
                    players.Add(pl);
                }
            }
            JoinInRandomOrder(players, next);
            
            if (LastLevelName != "")
                Command.all.Find("unload").Use(null, LastLevelName);
            LastLevelName = next;
        }
        
        static void JoinInRandomOrder(List<Player> players, string next) {
            Random rnd = new Random();
            while (players.Count > 0) {
                int index = rnd.Next(0, players.Count);
                Player pl = players[index];
                
                pl.SendMessage("Going to the next map - &a" + next);
                PlayerActions.ChangeMap(pl, next);
                players.RemoveAt(index);
            }
        }

        public void End() {
            Status = ZombieGameStatus.NotStarted;
            MaxRounds = 0;
            RoundInProgress = false;
            RoundStart = DateTime.MinValue;
            RoundEnd = DateTime.MinValue;
            if (plugin.Game != null) { plugin.Game = null; plugin.Unload(false); }
            
            Player[] online = PlayerInfo.Online.Items;           
            Alive.Clear();
            Infected.Clear();
            
            Lottery.Clear();
            Bounties.Clear();
            RecentMaps.Clear();
            
            foreach (Player pl in online) {
                pl.Game.Referee = false;
                pl.Game.RatedMap = false;
                pl.Game.ResetZombieState();
                ResetInvisibility(pl);
                pl.SetPrefix();
                
                if (pl.level == null || !pl.level.name.CaselessEq(CurLevelName))
                    continue;
                HUD.Reset(pl);
            }
            
            LastLevelName = "";
            CurLevelName = "";
            CurLevel = null;
            UnhookStats();
        }
        
        public BountyData FindBounty(string target) {
            BountyData[] bounties = Bounties.Items;
            foreach (BountyData bounty in bounties) {
                if (bounty.Target.CaselessEq(target)) return bounty;
            }
            return null;
        }
        
        static string[] defMessages = new string[] { "{0} WIKIWOO'D {1}", "{0} stuck their teeth into {1}",
            "{0} licked {1}'s brain ", "{0} danubed {1}", "{0} made {1} meet their maker", "{0} tripped {1}",
            "{0} made some zombie babies with {1}", "{0} made {1} see the dark side", "{0} tweeted {1}",
            "{0} made {1} open source", "{0} infected {1}", "{0} iDotted {1}", "{1} got nommed on",
            "{0} transplanted {1}'s living brain" };
        
        public void LoadInfectMessages() {
            infectMessages.Clear();
            try {
                if (!File.Exists("text/infectmessages.txt"))
                    File.WriteAllLines("text/infectmessages.txt", defMessages);

                infectMessages = Utils.ReadAllLinesList("text/infectmessages.txt");
            } catch (Exception ex) {
                Logger.LogError(ex);
            }
            if (infectMessages.Count == 0)
                infectMessages = new List<string>(defMessages);
        }
        
        public bool IsZombieMap(string name) {
            if (!Running) return false;
            if (ZSConfig.IgnorePersonalWorlds && name.IndexOf('+') >= 0)
                return false;
            if (ZSConfig.IgnoredLevelList.CaselessContains(name))
                return false;
            
            return ZSConfig.LevelList.Count == 0 ?
                true : ZSConfig.LevelList.CaselessContains(name);
        }
        
        #region Database
        
        static ColumnDesc[] createSyntax = new ColumnDesc[] {
            new ColumnDesc("ID", ColumnType.Integer, priKey: true, autoInc: true, notNull: true),
            new ColumnDesc("Name", ColumnType.Char, 20),
            new ColumnDesc("TotalRounds", ColumnType.Int32),
            new ColumnDesc("MaxRounds", ColumnType.Int32),
            new ColumnDesc("TotalInfected", ColumnType.Int32),
            new ColumnDesc("MaxInfected", ColumnType.Int32),
            // reserve space for possible future additions
            new ColumnDesc("Additional1", ColumnType.Int32),
            new ColumnDesc("Additional2", ColumnType.Int32),
            new ColumnDesc("Additional3", ColumnType.Int32),
            new ColumnDesc("Additional4", ColumnType.Int32),
        };
        
        public void CheckTableExists() {
            Database.Backend.CreateTable("ZombieStats", createSyntax);
        }
        
        public ZombieStats LoadZombieStats(string name) {
            DataTable table = Database.Backend.GetRows("ZombieStats", "*", "WHERE Name=@0", name);
            ZombieStats stats = default(ZombieStats);
            
            if (table.Rows.Count > 0) {
                DataRow row = table.Rows[0];
                stats.TotalRounds = int.Parse(row["TotalRounds"].ToString());
                stats.MaxRounds = int.Parse(row["MaxRounds"].ToString());
                stats.TotalInfected = int.Parse(row["TotalInfected"].ToString());
                stats.MaxInfected = int.Parse(row["MaxInfected"].ToString());
            }
            table.Dispose();
            return stats;
        }
        
        public void SaveZombieStats(Player p) {
            if (p.Game.TotalRoundsSurvived == 0 && p.Game.TotalInfected == 0) return;
            int count = 0;
            using (DataTable table = Database.Backend.GetRows("ZombieStats", "*", "WHERE Name=@0", p.name)) {
                count = table.Rows.Count;
            }

            if (count == 0) {
                Database.Backend.AddRow("ZombieStats", "TotalRounds, MaxRounds, TotalInfected, MaxInfected, Name",
                                        p.Game.TotalRoundsSurvived, p.Game.MaxRoundsSurvived,
                                        p.Game.TotalInfected, p.Game.MaxInfected, p.name);
            } else {
                Database.Backend.UpdateRows("ZombieStats", "TotalRounds=@0, MaxRounds=@1, TotalInfected=@2, MaxInfected=@3",
                                            "WHERE Name=@4", p.Game.TotalRoundsSurvived, p.Game.MaxRoundsSurvived,
                                            p.Game.TotalInfected, p.Game.MaxInfected, p.name);
            }
        }
        
        TopStat statMostInfected, statMaxInfected, statMostSurvived, statMaxSurvived;
        OfflineStatPrinter offlineZSStats;
        OnlineStatPrinter onlineZSStats;
        void HookStats() {
            if (TopStat.Stats.Contains(statMostInfected)) return; // don't duplicate
            
            statMostInfected = new TopStat("Infected", "ZombieStats", "TotalInfected",
                                           () => "Most players infected", TopStat.FormatInteger);
            statMaxInfected = new TopStat("Survived", "ZombieStats", "TotalRounds",
                                          () => "Most rounds survived", TopStat.FormatInteger);
            statMostSurvived = new TopStat("ConsecutiveInfected", "ZombieStats", "MaxInfected",
                                           () => "Most consecutive infections", TopStat.FormatInteger);
            statMaxSurvived = new TopStat("ConsecutiveSurvived", "ZombieStats", "MaxRounds",
                                          () => "Most consecutive rounds survived", TopStat.FormatInteger);
            
            offlineZSStats = PrintOfflineZSStats;
            onlineZSStats = PrintOnlineZSStats;
            OfflineStat.Stats.Add(offlineZSStats);
            OnlineStat.Stats.Add(onlineZSStats);

            TopStat.Stats.Add(statMostInfected);
            TopStat.Stats.Add(statMostSurvived);
            TopStat.Stats.Add(statMaxInfected);
            TopStat.Stats.Add(statMaxSurvived);
        }
        
        void UnhookStats() {
            OfflineStat.Stats.Remove(offlineZSStats);
            OnlineStat.Stats.Remove(onlineZSStats);
            
            TopStat.Stats.Remove(statMostInfected);
            TopStat.Stats.Remove(statMostSurvived);
            TopStat.Stats.Remove(statMaxInfected);
            TopStat.Stats.Remove(statMaxSurvived);
        }
        
        static void PrintOnlineZSStats(Player p, Player who) {
            PrintZSStats(p, who.Game.TotalRoundsSurvived, who.Game.TotalInfected,
                         who.Game.MaxRoundsSurvived, who.Game.MaxInfected);
        }
        
        static void PrintOfflineZSStats(Player p, PlayerData who) {
            ZombieStats stats = Server.zombie.LoadZombieStats(who.Name);
            PrintZSStats(p, stats.TotalRounds, stats.TotalInfected,
                         stats.MaxRounds, stats.MaxInfected);
        }
        
        static void PrintZSStats(Player p, int rounds, int infected, int roundsMax, int infectedMax) {
            Player.Message(p, "  Survived &a{0} %Srounds (max &e{1}%S)", rounds, roundsMax);
            Player.Message(p, "  Infected &a{0} %Splayers (max &e{1}%S)", infected, infectedMax);
        }
        #endregion
    }
}
