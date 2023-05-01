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
using MCGalaxy.Games;
using MCGalaxy.SQL;

namespace MCGalaxy.Modules.Games.ZS 
{    
    public class BountyData 
    {
        public string Origin, Target;
        public int Amount;
        
        public BountyData(string origin, string target, int amount) {
            Origin = origin; Target = target; Amount = amount;
        }
            
        
        public static VolatileArray<BountyData> Bounties = new VolatileArray<BountyData>();
        
        public static BountyData Find(string target) {
            BountyData[] bounties = Bounties.Items;
            foreach (BountyData bounty in bounties) 
            {
                if (bounty.Target.CaselessEq(target)) return bounty;
            }
            return null;
        }
    }
    
    internal sealed class ZSData 
    {
        public int BlocksLeft = 50, BlocksStacked;
        internal int LastX, LastY, LastZ;
        
        public bool AkaMode, Invisible;
        public DateTime TimeInfected, InvisibilityEnd;
        public List<string> InfectMessages;
        
        public int TotalRoundsSurvived, MaxRoundsSurvived, CurrentRoundsSurvived;
        public int TotalInfected, MaxInfected, CurrentInfected;
        public int InvisibilityTime = -1, InvisibilityPotions, RevivesUsed;
        
        public DateTime LastPillarWarn;
        public bool PillarFined;
        /// <summary> Whether the player has pledged that they will survive this round. </summary>
        public bool PledgeSurvive;
        
        public void ResetInvisibility() {
            Invisible = false;
            InvisibilityEnd = DateTime.MinValue;
            InvisibilityTime = -1;
        }
    }
    
    public sealed partial class ZSGame : RoundsGame 
    {
        public ZSConfig Config = new ZSConfig();
        public override string GameName { get { return "Zombie Survival"; } }
        public override RoundsGameConfig GetConfig() { return Config; }
        
        public static ZSGame Instance = new ZSGame();
        public ZSGame() { Picker = new LevelPicker(); }
        
        protected override string WelcomeMessage {
            get { return "&2Zombie Survival &Sis running! Type &T/ZS go &Sto join"; }
		}
        
        public DateTime RoundEnd;
        public VolatileArray<Player> Alive = new VolatileArray<Player>();
        public VolatileArray<Player> Infected = new VolatileArray<Player>();
        public string QueuedZombie;
        internal List<string> infectMessages = new List<string>();
        static bool hooked;
        
        const string zsExtrasKey = "MCG_ZS_DATA";
        internal static ZSData Get(Player p) {
            ZSData data = TryGet(p);
            if (data != null) return data;
            data = new ZSData();
            
            // TODO: Is this even thread-safe
            // TODO don't load here, add a LoadInfectMessages method
            data.InfectMessages = ZSConfig.LoadPlayerInfectMessages(p.name);
            ZombieStats s = LoadStats(p.name);
            data.MaxInfected = s.MaxInfected;     data.TotalInfected = s.TotalInfected;
            data.MaxRoundsSurvived = s.MaxRounds; data.TotalRoundsSurvived = s.TotalRounds;
            
            p.Extras[zsExtrasKey] = data;
            return data;
        }

        internal static ZSData TryGet(Player p) {
            object data; p.Extras.TryGet(zsExtrasKey, out data); return (ZSData)data;
        }
        
        // TODO: Move ZS map config to per-game properties
        public override void UpdateMapConfig() { }
        
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
            p.Message("{0} out of {1} players are alive",
                           Alive.Count, Alive.Count + Infected.Count);
        }
        
        public override void Start(Player p, string map, int rounds) {
            // ZS starts on current map by default
            if (!p.IsSuper && map.Length == 0) map = p.level.name;
            base.Start(p, map, rounds);
        }
        
        protected override void StartGame() {
            Database.CreateTable("ZombieStats", zsTable); 
            if (hooked) return;
            
            hooked = true;
            HookStats();
            HookCommands();
            HookItems();
        }

        protected override void EndGame() {
            RoundEnd = DateTime.MinValue;
            hooked   = false;
            UnhookStats();
            UnhookCommands();
            UnhookItems();
            
            Alive.Clear();
            Infected.Clear();
            BountyData.Bounties.Clear(); // TODO only do it when all games end
            
            Player[] players = PlayerInfo.Online.Items;
            foreach (Player pl in players) 
            {
                if (pl.level != Map) continue;
                ZSData data = Get(pl);
                
                ResetRoundState(pl, data);
                ResetInvisibility(pl, data);
            }
        }
        
        public static bool IsInfected(Player p) { return p.infected; }

        public void InfectPlayer(Player p, Player killer) {
            if (!RoundInProgress) return;
            Infected.Add(p);
            Alive.Remove(p);
            
            ZSData data = Get(p);
            data.CurrentRoundsSurvived = 0;
            data.TimeInfected = DateTime.UtcNow;
            
            p.SetPrefix();
            UpdatePlayer(p, data, true);
            RespawnPlayer(p);
            
            CheckHumanPledge(p, killer);
            CheckBounty(p, killer);
        }

        public void DisinfectPlayer(Player p) {
            if (!RoundInProgress) return;
            Infected.Remove(p);
            Alive.Add(p);
            
            ZSData data = Get(p);
            UpdatePlayer(p, data, false);
            RespawnPlayer(p);
        }
        
        static void ResetRoundState(Player p, ZSData data) {
            p.infected           = false;
            data.BlocksLeft      = 50;
            data.CurrentInfected = 0;
            
            data.InvisibilityPotions = 0;
            data.RevivesUsed = 0;
            data.TimeInfected = DateTime.MinValue;
        }
        
        void UpdatePlayer(Player p, ZSData data, bool infected) {
            p.infected      = infected;
            data.BlocksLeft = infected ? 25 : 50;
            
            ResetInvisibility(p, data);
            UpdateAllStatus1();
            UpdateStatus3(p);
        }
        
        static void ResetInvisibility(Player p, ZSData data) {
            if (!data.Invisible) return;
            p.SendCpeMessage(CpeMessageType.BottomRight2, "");
            
            data.ResetInvisibility();
            Entities.GlobalSpawn(p, false);
        }
        
        public override void PlayerJoinedGame(Player p) {
            bool announce = false;
            HandleJoinedLevel(p, Map, Map, ref announce);
        }

        public override void PlayerLeftGame(Player p) {
            Alive.Remove(p);
            Infected.Remove(p);
            p.infected = false;
            RemoveAssociatedBounties(p);
            
            if (!Running || !RoundInProgress || Infected.Count > 0) return;
            Random rnd = new Random();
            Player[] alive = Alive.Items;
            if (alive.Length == 0) return;
            
            // Auto continue infection
            Player zombie = alive[rnd.Next(alive.Length)];
            Map.Message("&c" + zombie.DisplayName + " &Scontinued the infection!");
            InfectPlayer(zombie, null);
        }
        
        void RemoveAssociatedBounties(Player p) {
            BountyData[] bounties = BountyData.Bounties.Items;
            foreach (BountyData b in bounties) 
            {
                if (!(b.Origin.CaselessEq(p.name) || b.Target.CaselessEq(p.name))) continue;
                
                string nick = p.FormatNick(b.Target);
                Map.Message("Bounty on " + nick + " &Sis no longer active.");
                BountyData.Bounties.Remove(b);
                
                Player setter = PlayerInfo.FindExact(b.Origin);
                if (setter != null) setter.SetMoney(setter.money + b.Amount);
            }
        }

        public override string GetPrefix(Player p) {
            if (!Running) return "";
            int winStreak = Get(p).CurrentRoundsSurvived;
            
            if      (winStreak == 1) return "&4*" + p.color;
            else if (winStreak == 2) return "&7*" + p.color;
            else if (winStreak == 3) return "&6*" + p.color;
            else if (winStreak > 0)  return "&6"  + winStreak + p.color;
            return "";
        }
        
        public void GoInvisible(Player p, int duration) {
            ZSData data    = Get(p);
            data.Invisible = true;
            data.InvisibilityEnd = DateTime.UtcNow.AddSeconds(duration);

            Map.Message(p.ColoredName + " &Svanished. &a*POOF*");
            Entities.GlobalDespawn(p, false, false);
        }
        
        public void AttemptRevive(Player p) {
            int chance = new Random().Next(1, 101);
            if (chance <= Config.ReviveChance) {
                DisinfectPlayer(p);
                Map.Message(p.ColoredName + " &Sused a revive potion. &aIt was super effective!");
            } else {
                Map.Message(p.ColoredName + " &Stried using a revive potion. &cIt was not very effective..");
            }
        }

        
        public override void OutputMapInfo(Player p, string map, LevelConfig cfg) {
            int winChance = cfg.RoundsPlayed == 0 ? 100 : (cfg.RoundsHumanWon * 100) / cfg.RoundsPlayed;
            p.Message("&a{0} &Srounds played total, &a{1}% &Swin chance for humans.",
                      cfg.RoundsPlayed, winChance);
        }
        
        static string GetTimeLeft(int seconds) {
            if (seconds < 0) return "";
            if (seconds <= 10) return "10s left";
            if (seconds <= 30) return "30s left";
            if (seconds <= 60) return "1m left";
            return ((seconds + 59) / 60) + "m left";
        }
        
        protected override string FormatStatus1(Player p) {
            int left = (int)(RoundEnd - DateTime.UtcNow).TotalSeconds;
            string timespan = GetTimeLeft(left);
            
            string format = timespan.Length == 0 ? "&a{0} &Salive &S(map: {1})" :
                "&a{0} &Salive &S({2}, map: {1})";
            return string.Format(format, Alive.Count, Map.MapName, timespan);
        }
        
        protected override string FormatStatus2(Player p) {
            string pillar = "&SPillaring " + (Map.Config.Pillaring ? "&aYes" : "&cNo");
            string type = "&S, Type is &a" + Map.Config.BuildType;
            return pillar + type;
        }

        protected override string FormatStatus3(Player p) {
            string money = "&a" + p.money + " &S" + Server.Config.Currency;
            string state = ", you are " + (IsInfected(p) ? "&cdead" : "&aalive");
            return money + state;
        }
        
        
        public bool SetQueuedZombie(Player p, string name) {
            Player target = PlayerInfo.FindMatches(p, name);
            if (target == null) return false;
            
            p.Message("{0} &Swas queued.", p.FormatNick(target));
            QueuedZombie = target.name;
            
            if (Map != null) Map.Message(target.ColoredName + " &Swas queued as the next zombie.");
            return true;
        }
        
        public bool SetQueuedLevel(Player p, string name) {
            string map = Matcher.FindMaps(p, name);
            if (map == null) return false;
                
            p.Message(map + " was queued.");
            Picker.QueuedMap = map.ToLower();
            
            if (Map != null) Map.Message(map + " was queued as the next map.");
            return true;
        }
    }
}
