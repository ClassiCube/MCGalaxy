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
using System.IO;
using System.Threading;
using MCGalaxy.DB;
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
        
        public ZSGame() { Picker = new ZSLevelPicker(); }
        public DateTime RoundEnd;
        public VolatileArray<Player> Alive = new VolatileArray<Player>();
        public VolatileArray<Player> Infected = new VolatileArray<Player>();
        public string QueuedZombie;
        public VolatileArray<BountyData> Bounties = new VolatileArray<BountyData>();
        List<string> infectMessages = new List<string>();
        
        const string zsExtrasKey = "MCG_ZS_DATA";
        internal static ZSData Get(Player p) {
            object data;
            if (!p.Extras.TryGet(zsExtrasKey, out data)) {
                data = new ZSData();
                // TODO: Is this even thread-safe
                InitData((ZSData)data, p);
                p.Extras.Put(zsExtrasKey, data);
            }
            return (ZSData)data;
        }

        internal static ZSData TryGet(Player p) {
            object data;
            bool success = p.Extras.TryGet(zsExtrasKey, out data);
            return success ? (ZSData)data : null;
        }
        
        static void InitData(ZSData data, Player p) {
            data.InfectMessages = PlayerDB.GetInfectMessages(p);
            ZombieStats stats = LoadStats(p.name);
            data.MaxInfected = stats.MaxInfected;     data.TotalInfected = stats.TotalInfected;
            data.MaxRoundsSurvived = stats.MaxRounds; data.TotalRoundsSurvived = stats.TotalRounds;
        }
        
        protected override List<Player> GetPlayers() {
            Player[] players = PlayerInfo.Online.Items;
            List<Player> playing = new List<Player>();
            
            foreach (Player pl in players) {
                if (pl.level != Map || pl.Game.Referee) continue;
                playing.Add(pl);
            }
            return playing;
        }
        
        public override void OutputStatus(Player p) {
            Player.Message(p, "{0} out of {1} players are alive",
                           Alive.Count, Alive.Count + Infected.Count);
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
            
            Running = true;
            Database.Backend.CreateTable("ZombieStats", createSyntax);
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
            UpdateAllStatus1();
            UpdateStatus3(p, infected);
        }
        
        void ResetInvisibility(Player p, ZSData data) {
            if (!data.Invisible) return;
            p.SendCpeMessage(CpeMessageType.BottomRight2, "");
            
            data.ResetInvisibility();
            Entities.GlobalSpawn(p, false);
        }

        public override void End() {
            if (!Running) return;
            Running = false;
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
                SaveStats(pl);
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
            p.SendCpeMessage(CpeMessageType.Status1, FormatStatus1());
            p.SendCpeMessage(CpeMessageType.Status2, FormatStatus2());
            UpdateStatus3(p, data.Infected);
            
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
            RemoveAssociatedBounties(p);
            
            if (!Running || !RoundInProgress || Infected.Count > 0) return;
            Random random = new Random();
            Player[] alive = Alive.Items;
            if (alive.Length == 0) return;
            
            Player zombie = alive[random.Next(alive.Length)];
            Map.Message("&c" + zombie.DisplayName + " %Scontinued the infection!");
            InfectPlayer(zombie, null);
        }
        
        public override bool HandlesChatMessage(Player p, string message) {
            if (!Running || p.level != Map) return false;
            return Picker.HandlesMessage(p, message);
        }
        
        void RemoveAssociatedBounties(Player p) {
            BountyData[] bounties = Bounties.Items;
            foreach (BountyData b in bounties) {
                if (!(b.Origin.CaselessEq(p.name) || b.Target.CaselessEq(p.name))) continue;
                
                string target = PlayerInfo.GetColoredName(p, b.Target);
                Map.Message("Bounty on " + target + " %Sis no longer active.");
                Bounties.Remove(b);
                
                Player setter = PlayerInfo.FindExact(b.Origin);
                if (setter != null) setter.SetMoney(setter.money + b.Amount);
            }
        }

        public override void AdjustPrefix(Player p, ref string prefix) {
            if (!Running) return;
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
        
        void UpdateAllStatus1() {
            MessageMap(CpeMessageType.Status1, FormatStatus1());
        }

        internal void UpdateAllStatus2() {
            MessageMap(CpeMessageType.Status2, FormatStatus2());
        }
        
        void UpdateStatus3(Player p, bool infected) {
            string status = FormatStatus3(p, infected);
            p.SendCpeMessage(CpeMessageType.Status3, status);
        }       
        
        static string GetTimeLeft(int seconds) {
            if (seconds < 0) return "";
            if (seconds <= 10) return "10s left";
            if (seconds <= 30) return "30s left";
            if (seconds <= 60) return "1m left";
            return ((seconds + 59) / 60) + "m left";
        }
        
        string FormatStatus1() {
            int left = (int)(RoundEnd - DateTime.UtcNow).TotalSeconds;
            string timespan = GetTimeLeft(left);
            
            string format = timespan.Length == 0 ? "&a{0} %Salive %S(map: {1})" :
                "&a{0} %Salive %S({2}, map: {1})";         
            return string.Format(format, Alive.Count, Map.MapName, timespan);
        }
        
        string FormatStatus2() {
            string pillar = "%SPillaring " + (Map.Config.Pillaring ? "&aYes" : "&cNo");
            string type = "%S, Type is &a" + Map.Config.BuildType;
            return pillar + type;
        }

        static string FormatStatus3(Player p, bool infected) {
            string money = "&a" + p.money + " %S" + ServerConfig.Currency;
            string state = ", you are " + (infected ? "&cdead" : "&aalive");
            return money + state;
        }
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
