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
using System.Timers;
using MCGalaxy.Games.ZS;

namespace MCGalaxy.Games {
    
    public sealed partial class ZombieGame {
        
        void MainLoop() {
            // Make sure that in the worst case, we do not crash the entire server.
            try {
                MainLoopCore();
            } catch (Exception ex) {
                Server.ErrorLog(ex);
                Chat.MessageAll("&cZombie survival disabled due to an error.");
                try {
                    ResetState();
                } catch (Exception ex2) {
                    Server.ErrorLog(ex2);
                }
            }
        }
        
        void MainLoopCore() {
            if (Status == ZombieGameStatus.NotStarted) return;
            while (true) {
                RoundInProgress = false;
                RoundsDone++;
                
                if (!Running) {
                    return;
                } else if (Status == ZombieGameStatus.InfiniteRounds) {
                    DoRound();
                    if (ZombieGameProps.ChangeLevels) 
                        LevelPicker.ChooseNextLevel(this);
                } else if (Status == ZombieGameStatus.SingleRound) {
                    DoRound();
                    ResetState(); return;
                } else if (Status == ZombieGameStatus.VariableRounds) {
                    if (RoundsDone == MaxRounds) {
                        ResetState(); return;
                    } else {
                        DoRound();
                        if (ZombieGameProps.ChangeLevels) 
                            LevelPicker.ChooseNextLevel(this);
                    }
                } else if (Status == ZombieGameStatus.LastRound) {
                    ResetState(); return;
                }
            }
        }

        void DoRound() {
            if (!Running) return;
            List<Player> players = DoRoundCountdown();
            if (players == null) return;
            Random random = new Random();

            RoundInProgress = true;
            int roundMins = random.Next(CurLevel.MinRoundTime, CurLevel.MaxRoundTime);
            string suffix = roundMins == 1 ? " %Sminute!" : " %Sminutes!";
            CurLevel.ChatLevel("This round will last for &a" + roundMins + suffix);
            RoundEnd = DateTime.UtcNow.AddMinutes(roundMins);
            
            Player[] online = PlayerInfo.Online.Items;
            foreach (Player p in online) {
                if (p.level == null || p.level != CurLevel || p.Game.Referee) continue;
                Alive.Add(p);
            }
            Infected.Clear();

            Player first = PickFirstZombie(random, players);
            CurLevel.ChatLevel(first.ColoredName + " %Sstarted the infection!");
            InfectPlayer(first);

            DoCoreGame(random); 
            if (!Running) {
                Status = ZombieGameStatus.LastRound; return;
            } else {
                HandOutRewards();
            }
            
            if (RecentMaps.Count > 20)
                RecentMaps.RemoveAt(0);
            RecentMaps.Add(CurLevelName);
        }
        
        Player PickFirstZombie(Random random, List<Player> players) {
            Player first = null;
            do {
                first = QueuedZombie != null ?
                    PlayerInfo.FindExact(QueuedZombie) : players[random.Next(players.Count)];
                QueuedZombie = null;
            } while (first == null || !first.level.name.CaselessEq(CurLevelName));
            return first;
        }
        
        List<Player> DoRoundCountdown() {
            while (true) {
                RoundStart = DateTime.UtcNow.AddSeconds(30);
                if (!Running) return null;
                SendLevelRaw("&4Starting in &f30 &4seconds", true);
                Thread.Sleep(10000); if (!Running) return null;
                SendLevelRaw("&4Starting in &f20 &4seconds", true);
                Thread.Sleep(10000); if (!Running) return null;
                SendLevelRaw("&4Starting in &f10 &4seconds", true);
                Thread.Sleep(5000); if (!Running) return null;
                SendLevelRaw("&4Starting in &f5 &4seconds", true);
                Thread.Sleep(1000); if (!Running) return null;
                SendLevelRaw("&4Starting in &f4 &4seconds", true);
                Thread.Sleep(1000); if (!Running) return null;
                SendLevelRaw("&4Starting in &f3 &4seconds", true);
                Thread.Sleep(1000); if (!Running) return null;
                SendLevelRaw("&4Starting in &f2 &4seconds", true);
                Thread.Sleep(1000); if (!Running) return null;
                SendLevelRaw("&4Starting in &f1 &4second", true);
                Thread.Sleep(1000); if (!Running) return null;
                SendLevelRaw("", true);
                int nonRefPlayers = 0;
                List<Player> players = new List<Player>();
                
                Player[] online = PlayerInfo.Online.Items;
                foreach (Player p in online) {
                    if (!p.Game.Referee && p.level.name.CaselessEq(CurLevelName)) {
                        players.Add(p);
                        nonRefPlayers++;
                    }
                }
                
                if (!Running) return null;
                if (nonRefPlayers >= 2) return players;
                CurLevel.ChatLevel("&cNeed 2 or more non-ref players to start a round.");
            }
        }
        
        void DoCoreGame(Random random) {
            Player[] alive = Alive.Items;
            string lastTimespan = null;
            int lastTime = -1;
            
            while (alive.Length > 0 && Running) {
                Player[] infected = Infected.Items;
                // Do round end.
                int seconds = (int)(RoundEnd - DateTime.UtcNow).TotalSeconds;
                if (seconds <= 0) { HandOutRewards(); return; }
                if (seconds <= 5 && seconds != lastTime) {
                    string suffix = seconds == 1 ? " &4second" : " &4seconds";
                    SendLevelRaw("&4Ending in &f" + seconds + suffix, true);
                    lastTime = seconds;
                }
                
                // Update the round time left shown in the top right
                string timespan = GetTimespan(seconds);
                if (lastTimespan != timespan) {
                    UpdateAllPlayerStatus(timespan);
                    lastTimespan = timespan;
                }
                
                int dist = ZombieGameProps.HitboxPrecision;
                foreach (Player pKiller in infected) {
                    pKiller.Game.Infected = true;
                    UpdatePlayerColor(pKiller, InfectCol);

                    foreach (Player pAlive in alive) {
                        UpdatePlayerColor(pAlive, pAlive.color);
                        int dx = Math.Abs(pAlive.pos[0] - pKiller.pos[0]);
                        int dy = Math.Abs(pAlive.pos[1] - pKiller.pos[1]);
                        int dz = Math.Abs(pAlive.pos[2] - pKiller.pos[2]);                        
                        if (dx > dist || dy > dist || dz > dist) continue;
                        
                        if (!pAlive.Game.Infected && pKiller.Game.Infected && !pAlive.Game.Referee
                            && !pKiller.Game.Referee && pKiller != pAlive
                            && pKiller.level.name.CaselessEq(CurLevelName)
                            && pAlive.level.name.CaselessEq(CurLevelName))
                        {
                            InfectPlayer(pAlive);
                            pAlive.Game.LastInfecter = pKiller.name;
                            pAlive.Game.BlocksLeft = 25;
                            
                            if (lastPlayerToInfect == pKiller.name) {
                                infectCombo++;
                                if (infectCombo >= 2) {
                                    pKiller.SendMessage("You gained " + (2 + infectCombo) + " " + Server.moneys);
                                    pKiller.SetMoney(pKiller.money + (2 + infectCombo));
                                    CurLevel.ChatLevel(pKiller.ColoredName + " %Sis on a rampage! " + (infectCombo + 1) + " infections in a row!");
                                }
                            } else {
                                infectCombo = 0;
                            }
                            
                            lastPlayerToInfect = pKiller.name;
                            pKiller.Game.CurrentInfected++;
                            pKiller.Game.TotalInfected++;
                            pKiller.Game.MaxInfected = Math.Max(pKiller.Game.CurrentInfected, pKiller.Game.MaxInfected);
                            
                            ShowInfectMessage(random, pAlive, pKiller);
                            CheckHumanPledge(pAlive);
                            CheckBounty(pAlive, pKiller);
                            UpdatePlayerColor(pAlive, InfectCol);
                            Thread.Sleep(50);
                        }
                    }
                    alive = Alive.Items;
                }
                
                CheckInvisibilityTime();
                Thread.Sleep(200);
                alive = Alive.Items;
            }
        }
        
        void SendLevelRaw(string message, bool announce = false) {
            Player[] players = PlayerInfo.Online.Items;
            foreach (Player p in players) {
                if (p.level != CurLevel) continue;
                CpeMessageType type = announce && p.HasCpeExt(CpeExt.MessageTypes)
                    ? CpeMessageType.Announcement : CpeMessageType.Normal;
                p.SendRawMessage(type, message);
            }
        }
        
        void CheckInvisibilityTime() {
            DateTime now = DateTime.UtcNow;
            Player[] players = PlayerInfo.Online.Items;
            foreach (Player p in players) {
                if (!p.Game.Invisible || p.level != CurLevel) continue;
                DateTime end = p.Game.InvisibilityEnd;                
                if (now >= end) { ResetInvisibility(p); continue; }
                
                int left = (int)Math.Ceiling((end - now).TotalSeconds);
                if (left == p.Game.InvisibilityTime) continue;
                p.Game.InvisibilityTime = left;
                
                string msg = "&bInvisibility for &a" + left;
                if (p.HasCpeExt(CpeExt.MessageTypes))
                    p.SendCpeMessage(CpeMessageType.BottomRight2, msg);
                else
                    p.SendMessage(msg);
            }
        }
        
        void CheckHumanPledge(Player pAlive) {
            if (!pAlive.Game.PledgeSurvive) return;
            pAlive.Game.PledgeSurvive = false;
            CurLevel.ChatLevel(pAlive.ColoredName + "%Sbroke their pledge of not being infected.");
            pAlive.SetMoney(Math.Max(pAlive.money - 2, 0));
        }
        
        void CheckBounty(Player pAlive, Player pKiller) {
            BountyData bounty;
            if (Bounties.TryGetValue(pAlive.name, out bounty))
                Bounties.Remove(pAlive.name);
            if (bounty != null) {
                CurLevel.ChatLevel(pKiller.ColoredName + " %Scollected the bounty of &a" +
                                   bounty.Amount + " %S" + Server.moneys + " on " + pAlive.ColoredName + "%S.");
                bounty.Origin.SetMoney(Math.Max(0, bounty.Origin.money - bounty.Amount));
                pKiller.SetMoney(pKiller.money + bounty.Amount);
            }
        }
        
        void ShowInfectMessage(Random random, Player pAlive, Player pKiller) {
            string text = null;
            List<string> infectMsgs = pKiller.Game.InfectMessages;
            if (infectMsgs != null && random.Next(0, 10) < 5)
                text = infectMsgs[random.Next(infectMsgs.Count)];
            else
                text = infectMessages[random.Next(infectMessages.Count)];
            
            CurLevel.ChatLevel(String.Format(text,
                                             Colors.red + pKiller.DisplayName + Colors.yellow,
                                             Colors.red + pAlive.DisplayName + Colors.yellow));
        }

        static void UpdatePlayerColor(Player p, string color) {
            if (p.Game.lastSpawnColor == color) return;
            p.Game.lastSpawnColor = color;
            Entities.GlobalDespawn(p, true);
            Entities.GlobalSpawn(p, true);
        }

        public void HandOutRewards() {
            if (!RoundInProgress) return;
            RoundInProgress = false;
            RoundStart = DateTime.MinValue;
            RoundEnd = DateTime.MinValue;
            Bounties.Clear();
            
            if (!Running) return;
            Rewards.HandOut(this);
        }
    }
}
