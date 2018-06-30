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
        public static ZSConfig Config = new ZSConfig();
        public override string GameName { get { return "Zombie Survival"; } }
        public override RoundsGameConfig GetConfig() { return Config; }
        
        public ZSGame() { Picker = new LevelPicker(); }
        public DateTime RoundEnd;
        public VolatileArray<Player> Alive = new VolatileArray<Player>();
        public VolatileArray<Player> Infected = new VolatileArray<Player>();
        public string QueuedZombie;
        public VolatileArray<BountyData> Bounties = new VolatileArray<BountyData>();
        internal List<string> infectMessages = new List<string>();
        
        const string zsExtrasKey = "MCG_ZS_DATA";
        internal static ZSData Get(Player p) {
            ZSData data = TryGet(p);
            if (data != null) return data;
            data = new ZSData();
            
            // TODO: Is this even thread-safe
            data.InfectMessages = ZSConfig.LoadPlayerInfectMessages(p.name);
            ZombieStats s = LoadStats(p.name);
            data.MaxInfected = s.MaxInfected;     data.TotalInfected = s.TotalInfected;
            data.MaxRoundsSurvived = s.MaxRounds; data.TotalRoundsSurvived = s.TotalRounds;
            
            p.Extras.Put(zsExtrasKey, data);
            return data;
        }

        internal static ZSData TryGet(Player p) {
            object data; p.Extras.TryGet(zsExtrasKey, out data); return (ZSData)data;
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
            base.Start(p, map, rounds);
        }
        
        protected override void StartGame() { 
            HookStats();
            Database.Backend.CreateTable("ZombieStats", createSyntax);
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

        protected override void EndGame() {
            RoundEnd = DateTime.MinValue;
            UnhookStats();
            
            Alive.Clear();
            Infected.Clear();
            Bounties.Clear();
            
            Player[] players = PlayerInfo.Online.Items;
            foreach (Player pl in players) {
                if (pl.level != Map) continue;              
                ZSData data = Get(pl);
                
                data.ResetState();
                ResetInvisibility(pl, data);
            }
        }
        
        public BountyData FindBounty(string target) {
            BountyData[] bounties = Bounties.Items;
            foreach (BountyData bounty in bounties) {
                if (bounty.Target.CaselessEq(target)) return bounty;
            }
            return null;
        }
        
        public override void PlayerJoinedGame(Player p) {
            bool announce = false;
            HandleJoinedLevel(p, Map, Map, ref announce);
        }

        public override void PlayerLeftGame(Player p) {
            Alive.Remove(p);
            Infected.Remove(p);
            Get(p).Infected = false;
            RemoveAssociatedBounties(p);
            
            if (!Running || !RoundInProgress || Infected.Count > 0) return;
            Random rnd = new Random();
            Player[] alive = Alive.Items;
            if (alive.Length == 0) return;
            
            // Auto continue infection
            Player zombie = alive[rnd.Next(alive.Length)];
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
        

        public bool HasMap(string name) {
            return Running && Config.Maps.CaselessContains(name);
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
}
