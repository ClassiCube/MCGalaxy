﻿/*
    Copyright 2010 MCLawl Team -
    Created by Snowl (David D.) and Cazzar (Cayde D.)

    Dual-licensed under the Educational Community License, Version 2.0 and
    the GNU General Public License, Version 3 (the "Licenses"); you may
    not use this file except in compliance with the Licenses. You may
    obtain a copy of the Licenses at
    
    https://opensource.org/license/ecl-2-0/
    https://www.gnu.org/licenses/gpl-3.0.html
    
    Unless required by applicable law or agreed to in writing,
    software distributed under the Licenses are distributed on an "AS IS"
    BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express
    or implied. See the Licenses for the specific language governing
    permissions and limitations under the Licenses.
 */
using System;
using System.Collections.Generic;
using System.Threading;
using MCGalaxy.Games;

namespace MCGalaxy.Modules.Games.ZS
{    
    public partial class ZSGame : RoundsGame 
    {
        string lastKiller = "";
        int infectCombo = 0;
        
        protected override void DoRound() {
            if (!Running) return;

            ResetPledges();
            List<Player> players = DoRoundCountdown(Config.InfectionCountdown);
            if (players == null) return;

            if (!Running) return;
            RoundInProgress = true;
            StartRound(players);
            
            if (!Running) return;
            DoCoreGame();
        }

        void ResetPledges() {
            foreach (Player pl in GetPlayers())
            {
                Get(pl).PledgeSurvive = false;
            }
        }
        
        void StartRound(List<Player> players) {
            TimeSpan duration = Map.Config.RoundTime;
            Map.Message("This round will last for &a" + duration.Shorten(true, true));
            RoundEnd = DateTime.UtcNow.Add(duration);
            
            Player[] online = PlayerInfo.Online.Items;
            foreach (Player p in online) {
                if (p.level != Map || p.Game.Referee) continue;
                Alive.Add(p);
            }
            Infected.Clear();
            
            Random rnd = new Random();
            Player first;
            do {
                first = QueuedZombie != null ? PlayerInfo.FindExact(QueuedZombie) : players[rnd.Next(players.Count)];
                QueuedZombie = null;
            } while (first == null || first.level != Map);
            
            Map.Message("&c" + first.DisplayName + " &Sstarted the infection!");
            InfectPlayer(first, null);
        }
        
        void DoCoreGame() {
            Player[] alive = Alive.Items;
            string lastTimeLeft = null;
            int lastCountdown = -1;
            Random random = new Random();
            
            while (alive.Length > 0 && Running && RoundInProgress) {
                Player[] infected = Infected.Items;
                // Round ended with some players still alive
                int seconds = (int)(RoundEnd - DateTime.UtcNow).TotalSeconds;
                if (seconds <= 0) {
                    MessageMap(CpeMessageType.Announcement, ""); return;
                }
                
                if (seconds <= 5 && seconds != lastCountdown) {
                    string suffix = seconds == 1 ? " &4second" : " &4seconds";
                    MessageMap(CpeMessageType.Announcement, 
                               "&4Ending in &f" + seconds + suffix);
                    lastCountdown = seconds;
                }
                
                // Update the round time left shown in the top right
                string timeLeft = GetTimeLeft(seconds);
                if (lastTimeLeft != timeLeft) {
                    UpdateAllStatus1();
                    lastTimeLeft = timeLeft;
                }
                
                DoCollisions(alive, infected, random);
                CheckInvisibilityTime();
                Thread.Sleep(Config.CollisionsCheckInterval);
                alive = Alive.Items;
            }
        }
        
        void DoCollisions(Player[] aliveList, Player[] deadList, Random random) {
            int dist = (int)(Config.HitboxDist * 32);
            foreach (Player killer in deadList)
            {
                ZSData killerData = Get(killer);
                killer.infected = true;
                aliveList = Alive.Items;

                foreach (Player alive in aliveList) 
                {
                    if (alive == killer) continue;
                    if (!InRange(alive, killer, dist)) continue;
                    
                    if (killer.infected && !IsInfected(alive)
                        && !alive.Game.Referee && !killer.Game.Referee
                        && killer.level == Map && alive.level == Map)
                    {
                        InfectPlayer(alive, killer);
                        
                        if (lastKiller == killer.name) {
                            infectCombo++;
                            if (infectCombo >= 2) {
                                killer.Message("You gained " + (2 + infectCombo) + " " + Server.Config.Currency);
                                killer.SetMoney(killer.money + (2 + infectCombo));
                                Map.Message("&c" + killer.DisplayName + " &Sis on a rampage! " + (infectCombo + 1) + " infections in a row!");
                            }
                        } else {
                            infectCombo = 0;
                        }
                        
                        lastKiller = killer.name;
                        killerData.CurrentInfected++;
                        killerData.TotalInfected++;
                        killerData.MaxInfected = Math.Max(killerData.CurrentInfected, killerData.MaxInfected);
                        
                        ShowInfectMessage(random, alive, killer);
                        Thread.Sleep(50);
                    }
                }
            }
        }
        
        void CheckInvisibilityTime() {
            DateTime now = DateTime.UtcNow;
            Player[] players = PlayerInfo.Online.Items;
            foreach (Player p in players) 
            {
                if (p.level != Map) continue;
                ZSData data = Get(p);
                if (!data.Invisible) continue;
                
                DateTime end = data.InvisibilityEnd;
                if (now >= end) {
                    p.Message("&cYou are &bvisible &cagain");
                    ResetInvisibility(p, data);
                    continue;
                }
                
                int left = (int)Math.Ceiling((end - now).TotalSeconds);
                if (left == data.InvisibilityTime) continue;
                data.InvisibilityTime = left;
                
                string msg = "&bInvisibility for &a" + left;
                if (p.Supports(CpeExt.MessageTypes)) {
                    p.SendCpeMessage(CpeMessageType.BottomRight2, msg);
                } else {
                    p.Message(msg);
                }
            }
        }
        
        void CheckHumanPledge(Player p, Player killer) {
            ZSData data = Get(p);
            if (!data.PledgeSurvive) return;
            data.PledgeSurvive = false;
            Map.Message("&c" + p.DisplayName + " &Sbroke "+p.pronouns.Object+" pledge of not being infected.");
            
            if (killer == null) {
                p.Message("As this was an automatic infection, you have not lost any &3" + Server.Config.Currency);
            } else {
                p.SetMoney(Math.Max(p.money - 2, 0));
            }
        }
        
        void CheckBounty(Player p, Player pKiller) {
            BountyData bounty = BountyData.Find(p.name);
            if (bounty == null) return;
            BountyData.Bounties.Remove(bounty);
            
            Player setter = PlayerInfo.FindExact(bounty.Origin);
            if (pKiller == null) {
                Map.Message("Bounty on " + p.ColoredName + " &Sis no longer active");
                if (setter != null) setter.SetMoney(setter.money + bounty.Amount);
            } else if (setter == null) {
                pKiller.Message("Cannot collect the bounty, as the player who set it is offline.");
            } else {
                Map.Message("&c" + pKiller.DisplayName + " &Scollected the bounty of &a" +
                              bounty.Amount + " &S" + Server.Config.Currency + " on " + p.ColoredName);
                pKiller.SetMoney(pKiller.money + bounty.Amount);
            }
        }
        
        void ShowInfectMessage(Random random, Player pAlive, Player pKiller) {
            string text = null;
            List<string> infectMsgs = Get(pKiller).InfectMessages;
            
            if (infectMsgs != null && infectMsgs.Count > 0 && random.Next(0, 10) < 5) {
                text = infectMsgs[random.Next(infectMsgs.Count)];
            } else {
                text = infectMessages[random.Next(infectMessages.Count)];
            }

            Map.Message(ZSConfig.FormatInfectMessage(text, pKiller, pAlive));
        }

        internal static void RespawnPlayer(Player p) {
            Entities.GlobalRespawn(p, false);
            TabList.Add(p, p, Entities.SelfID);
        }

        public override void EndRound() {
            if (!RoundInProgress) return;
            RoundInProgress = false;
            RoundStart = DateTime.MinValue;
            RoundEnd = DateTime.MinValue;
            UpdateAllStatus1();
            
            if (!Running) return;
            Player[] alive = Alive.Items, dead = Infected.Items;
            Map.Message("&aThe game has ended!");
            
            if (alive.Length == 0) Map.Message("&4Zombies have won this round.");
            else if (alive.Length == 1) Map.Message("&2Congratulations to the sole survivor:");
            else Map.Message("&2Congratulations to the survivors:");
            AnnounceWinners(alive, dead);
            
            Map.Config.RoundsPlayed++;
            if (alive.Length > 0) {
                Map.Config.RoundsHumanWon++;
                foreach (Player p in alive) { IncreaseAliveStats(p); }
            }
            
            GiveMoney(alive);
            Map.SaveSettings();
        }

        void AnnounceWinners(Player[] alive, Player[] dead) {
            if (alive.Length > 0) {
                Map.Message(alive.Join(p => p.ColoredName)); return;
            }
            
            int maxKills = 0, count = 0;
            for (int i = 0; i < dead.Length; i++) 
            {
                maxKills = Math.Max(maxKills, Get(dead[i]).CurrentInfected);
            }
            for (int i = 0; i < dead.Length; i++) 
            {
                if (Get(dead[i]).CurrentInfected == maxKills) count++;
            }
            
            string group = count == 1 ? " zombie " : " zombies ";
            string suffix = maxKills == 1 ? " &Skill" : " &Skills";
            StringFormatter<Player> formatter = p => Get(p).CurrentInfected == maxKills ? p.ColoredName : null;
            Map.Message("&8Best" + group + "&S(&b" + maxKills + suffix + "&S)&8: " + dead.Join(formatter));
        }

        void IncreaseAliveStats(Player p) {
            ZSData data = Get(p);

            if (data.PledgeSurvive) {
                p.Message("You received &a5 &3" + Server.Config.Currency +
                          " &Sfor successfully pledging that you would survive.");
                p.SetMoney(p.money + 5);
            }
            
            data.CurrentRoundsSurvived++;
            data.TotalRoundsSurvived++;
            data.MaxRoundsSurvived = Math.Max(data.CurrentRoundsSurvived, data.MaxRoundsSurvived);
            p.SetPrefix(); // stars before name
        }

        void GiveMoney(Player[] alive) {
            Player[] online = PlayerInfo.Online.Items;
            Random rand = new Random();
            
            foreach (Player pl in online) 
            {
                if (pl.level != Map) continue;
                ZSData data = Get(pl);
                data.ResetInvisibility();
                RewardMoney(pl, data, alive, rand);
                
                ResetRoundState(pl, data);
                data.PledgeSurvive = false;
                
                if (pl.Game.Referee) {
                    pl.Message("You gained one " + Server.Config.Currency + " because you're a ref. Would you like a medal as well?");
                    pl.SetMoney(pl.money + 1);
                }
                
                RespawnPlayer(pl);
                UpdateStatus3(pl);
            }
        }

        void RewardMoney(Player p, ZSData data, Player[] alive, Random rnd) {
            if (p.IsLikelyInsideBlock()) {
                p.Message("You may not hide inside a block! No " + Server.Config.Currency + " for you.");
                return;
            }
            
            if (alive.Length == 0) {
                AwardMoney(p, Config.ZombiesRewardMin, Config.ZombiesRewardMax,
                           rnd, data.CurrentInfected * Config.ZombiesRewardMultiplier);
            } else if (alive.Length == 1 && !p.infected) {
                AwardMoney(p, Config.SoleHumanRewardMin, Config.SoleHumanRewardMax,
                           rnd, 0);
            } else if (alive.Length > 1 && !p.infected) {
                AwardMoney(p, Config.HumansRewardMin, Config.HumansRewardMax,
                           rnd, 0);
            }
        }
        
        
        public override void OutputTimeInfo(Player p) {
            TimeSpan delta = RoundEnd - DateTime.UtcNow;
            if (delta.TotalSeconds > 0) {
                p.Message("&a{0} &Suntil the round ends.", delta.Shorten(true));
            } else {
                delta = RoundStart - DateTime.UtcNow;
                if (delta.TotalSeconds > 0)
                    p.Message("&a{0} &Suntil the round starts.", delta.Shorten(true));
            }
        }
    }
}
