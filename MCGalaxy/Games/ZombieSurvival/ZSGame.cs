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
    
    public class BountyData {
        public string Origin, Target;
        public int Amount;
        
        public BountyData(string origin, string target, int amount) {
            Origin = origin; Target = target; Amount = amount;
        }
    }  
    public struct ZombieStats { public int TotalRounds, MaxRounds, TotalInfected, MaxInfected; }
    
    internal sealed class ZSData {
        public int BlocksLeft = 50, BlocksStacked;
        internal int LastX, LastY, LastZ;
        
        public bool Infected, AkaMode, Invisible;
        public DateTime TimeInfected, InvisibilityEnd;
        public List<string> InfectMessages;
        
        public int TotalRoundsSurvived, MaxRoundsSurvived, CurrentRoundsSurvived;
        public int TotalInfected, MaxInfected, CurrentInfected;
        public int InvisibilityTime = -1, InvisibilityPotions, RevivesUsed;
        
        public DateTime LastPillarWarn;
        public bool PillarFined;
        
        public void ResetInvisibility() {
            Invisible = false;
            InvisibilityEnd = DateTime.MinValue;
            InvisibilityTime = -1;
        }

        public void ResetState() {
            BlocksLeft = 50;
            CurrentInfected = 0;
            Infected = false;           
            InvisibilityPotions = 0;
            RevivesUsed = 0;
            TimeInfected = DateTime.MinValue;
        }
    }
    
    public sealed partial class ZSGame : RoundsGame {
        public override string GameName { get { return "Zombie survival"; } }
        public override bool TeleportAllowed { get { return !RoundInProgress; } }
        public override bool Running { get { return running; } }
        
        public ZSGame() { Picker = new ZSLevelPicker(); }
        public DateTime RoundStart, RoundEnd;
        public VolatileArray<Player> Alive = new VolatileArray<Player>();
        public VolatileArray<Player> Infected = new VolatileArray<Player>();
        public string QueuedZombie;
        public VolatileArray<BountyData> Bounties = new VolatileArray<BountyData>();

        List<string> infectMessages = new List<string>();
        bool running;
        
        const string zsExtrasKey = "MCG_ZS_DATA";
        internal ZSData Get(Player p) {
            object data;
            if (!p.Extras.TryGet("MCG_ZS_DATA", out data)) {
                data = new ZSData();
                // TODO: Is this even thread-safe
                InitData((ZSData)data, p);
                p.Extras.Put("MCG_ZS_DATA", data);
            }
            return (ZSData)data;
        }

        internal ZSData TryGet(Player p) {
            object data;
            bool success = p.Extras.TryGet("MCG_ZS_DATA", out data);
            return success ? (ZSData)data : null;
        }
        
        void InitData(ZSData data, Player p) {
            data.InfectMessages = PlayerDB.GetInfectMessages(p);
            ZombieStats stats = LoadZombieStats(p.name);
            data.MaxInfected = stats.MaxInfected;     data.TotalInfected = stats.TotalInfected;
            data.MaxRoundsSurvived = stats.MaxRounds; data.TotalRoundsSurvived = stats.TotalRounds;
        }
        
        public override void Start(Player p, string map, int rounds) {
            // ZS starts on current map by default
            if (!Player.IsSuper(p) && map.Length == 0) map = p.level.name;
            
            map = GetStartMap(map);
            if (map == null) {
                Player.Message(p, "No maps have been setup for Zombie Survival yet"); return;
            }
            if (!SetMap(map)) {
                Player.Message(p, "Failed to load initial map!"); return;
            }
            
            Chat.MessageGlobal("A game of zombie survival is starting on: {0}", Map.ColoredName);
            Player[] players = PlayerInfo.Online.Items;
            foreach (Player pl in players) {
                if (pl.level == Map) PlayerJoinedGame(pl);
            }
            
            RoundsLeft = rounds;
            HookStats();
            
            running = true;
            HookEventHandlers();
            
            Thread t = new Thread(RunGame);
            t.Name = "MCG_ZSGame";
            t.Start();
        }

        public void InfectPlayer(Player p, Player killer) {
            if (!RoundInProgress || p == null) return;
            Infected.Add(p);
            Alive.Remove(p);
            
            ZSData data = Get(p);
            data.CurrentRoundsSurvived = 0;
            data.TimeInfected = DateTime.UtcNow;
            
            p.SetPrefix();
            ResetPlayerState(p, data, true);
            RespawnPlayer(p);
            
            CheckHumanPledge(p, killer);
            CheckBounty(p, killer);
        }

        public void DisinfectPlayer(Player p) {
            if (!RoundInProgress || p == null) return;
            Infected.Remove(p);
            Alive.Add(p);
            
            ZSData data = Get(p);
            ResetPlayerState(p, data, false);
            RespawnPlayer(p);
        }
        
        void ResetPlayerState(Player p, ZSData data, bool infected) {
            data.Infected = infected;
            data.BlocksLeft = infected ? 25 : 50;
            
            ResetInvisibility(p, data);
            HUD.UpdateAllPrimary(this);
            HUD.UpdateTertiary(p, infected);
        }
        
        void ResetInvisibility(Player p, ZSData data) {
            if (!data.Invisible) return;
            p.SendCpeMessage(CpeMessageType.BottomRight2, "");
            
            data.ResetInvisibility();
            Entities.GlobalSpawn(p, false);
        }

        public override void End() {
            if (!running) return;
            running = false;
            UnhookEventHandlers();
            
            RoundStart = DateTime.MinValue;
            RoundEnd = DateTime.MinValue;
            
            Alive.Clear();
            Infected.Clear();
            Bounties.Clear();
            
            Player[] online = PlayerInfo.Online.Items;
            foreach (Player pl in online) {
                pl.Game.Referee = false;
                pl.Game.RatedMap = false; 
                pl.Game.PledgeSurvive = false;
                
                pl.SetPrefix();
                if (pl.level == null || pl.level != Map) continue;
                
                ZSData data = Get(pl);
                data.ResetState();
                ResetInvisibility(pl, data);
                HUD.Reset(pl);
            }
            
            EndCommon();
            UnhookStats();
        }
        
        public BountyData FindBounty(string target) {
            BountyData[] bounties = Bounties.Items;
            foreach (BountyData bounty in bounties) {
                if (bounty.Target.CaselessEq(target)) return bounty;
            }
            return null;
        }
        
        public override void PlayerJoinedGame(Player p) {
            ZSData data = Get(p); // usually this Get() performs the expensive DB stats read
            p.SetPrefix();
            HUD.UpdatePrimary(this, p);
            HUD.UpdateSecondary(this, p);
            HUD.UpdateTertiary(p, data.Infected);
            
            if (RoundInProgress) {
                Player.Message(p, "You joined in the middle of a round. &cYou are now infected!");
                data.BlocksLeft = 25;
                InfectPlayer(p, null);
            }

            double startLeft = (RoundStart - DateTime.UtcNow).TotalSeconds;
            if (startLeft >= 0) {
                Player.Message(p, "&a{0}%Sseconds left until the round starts. &aRun!", (int)startLeft);
            }
            Player.Message(p, "This map has &a{0} likes %Sand &c{1} dislikes", 
                           Map.Config.Likes, Map.Config.Dislikes);
            Player.Message(p, "This map's win chance is &a{0}%S%", Map.WinChance);
            
            if (Map.Config.Authors.Length == 0) return;
            string[] authors = Map.Config.Authors.Replace(" ", "").Split(',');
            Player.Message(p, "It was created by {0}",
                           authors.Join(n => PlayerInfo.GetColoredName(p, n)));
        }

        public override void PlayerLeftGame(Player p) {
            Alive.Remove(p);
            Infected.Remove(p);
            Get(p).Infected = false;
            RemoveBounties(p);
            
            if (!running || !RoundInProgress || Infected.Count > 0) return;
            Random random = new Random();
            Player[] alive = Alive.Items;
            if (alive.Length == 0) return;
            
            Player zombie = alive[random.Next(alive.Length)];
            Map.ChatLevel("&c" + zombie.DisplayName + " %Scontinued the infection!");
            InfectPlayer(zombie, null);
        }
        
        public override bool HandlesChatMessage(Player p, string message) {
            if (!Running || (p.level == null || p.level != Map)) return false;
            if (Picker.Voting && Picker.HandlesMessage(p, message)) return true;
            
            if (message[0] == '~' && message.Length > 1) {
                Player[] players = PlayerInfo.Online.Items;
                bool infected = Get(p).Infected;
                string type = infected ? " &cto zombies%S: " : " &ato humans%S: ";
                string msg = p.ColoredName + type + message.Substring(1);
                
                foreach (Player pl in players) {
                    if (pl.level != Map || !Chat.NotIgnoring(pl, p)) continue;
                    if (pl.Game.Referee || Get(pl).Infected == infected) {
                        pl.SendMessage(msg);
                    }
                }
                return true;
            } else if (message[0] == '`' && message.Length > 1) {
                if (p.Game.Team == null) {
                    Player.Message(p, "You are not on a team, so cannot send a team message."); return true;
                }
                p.Game.Team.Chat(p, message.Substring(1));
                return true;
            }
            return false;
        }
        
        void RemoveBounties(Player p) {
            BountyData[] bounties = Bounties.Items;
            foreach (BountyData b in bounties) {
                if (!(b.Origin.CaselessEq(p.name) || b.Target.CaselessEq(p.name))) continue;
                
                string target = PlayerInfo.GetColoredName(p, b.Target);
                Map.ChatLevel("Bounty on " + target + " %Sis no longer active.");
                Bounties.Remove(b);
                
                Player setter = PlayerInfo.FindExact(b.Origin);
                if (setter != null) setter.SetMoney(setter.money + b.Amount);
            }
        }

        public override void AdjustPrefix(Player p, ref string prefix) {
            if (!running) return;
            int winStreak = Get(p).CurrentRoundsSurvived;
            
            if      (winStreak == 1) prefix += "&4*" + p.color;
            else if (winStreak == 2) prefix += "&7*" + p.color;
            else if (winStreak == 3) prefix += "&6*" + p.color;
            else if (winStreak > 0)  prefix += "&6"  + winStreak + p.color;
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
            if (ZSConfig.IgnoredLevelList.CaselessContains(name)) return false;
            
            return ZSConfig.LevelList.Count == 0 || ZSConfig.LevelList.CaselessContains(name);
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
        
        TopStat statMostInfected, statMaxInfected, statMostSurvived, statMaxSurvived;
        OfflineStatPrinter offlineZSStats;
        OnlineStatPrinter onlineZSStats;
        ChatToken infectedToken, survivedToken;
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
        
        void UnhookStats() {
            OfflineStat.Stats.Remove(offlineZSStats);
            OnlineStat.Stats.Remove(onlineZSStats);
            ChatTokens.Standard.Remove(infectedToken);
            ChatTokens.Standard.Remove(survivedToken);
            
            TopStat.Stats.Remove(statMostInfected);
            TopStat.Stats.Remove(statMostSurvived);
            TopStat.Stats.Remove(statMaxInfected);
            TopStat.Stats.Remove(statMaxSurvived);
        }
        
        void PrintOnlineZSStats(Player p, Player who) {
            ZSData data = Get(who);
            PrintZSStats(p, data.TotalRoundsSurvived, data.TotalInfected,
                         data.MaxRoundsSurvived, data.MaxInfected);
        }
        
        void PrintOfflineZSStats(Player p, PlayerData who) {
            ZombieStats stats = LoadZombieStats(who.Name);
            PrintZSStats(p, stats.TotalRounds, stats.TotalInfected,
                         stats.MaxRounds, stats.MaxInfected);
        }
        
        static void PrintZSStats(Player p, int rounds, int infected, int roundsMax, int infectedMax) {
            Player.Message(p, "  Survived &a{0} %Srounds (max &e{1}%S)", rounds, roundsMax);
            Player.Message(p, "  Infected &a{0} %Splayers (max &e{1}%S)", infected, infectedMax);
        }
        #endregion
    }
    
    internal class ZSLevelPicker : LevelPicker {
        
        public override List<string> GetCandidateMaps() {
            List<string> maps = null;
            bool useLevelList = ZSConfig.LevelList.Count > 0;
            
            if (useLevelList) {
                maps = new List<string>(ZSConfig.LevelList);
            } else {
                string[] allMaps = LevelInfo.AllMapNames();
                maps = new List<string>(allMaps);
            }
            
            foreach (string ignore in ZSConfig.IgnoredLevelList) { maps.Remove(ignore); }
            if (maps.Count < 3) {
                string group = useLevelList ? "in your level list " : "";
                Logger.Log(LogType.Warning, "You must have more than 3 levels {0}to change levels in Zombie Survival", group);
                return null;
            }
            return maps;
        }
    }
}
