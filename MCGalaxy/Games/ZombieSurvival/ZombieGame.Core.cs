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
using MCGalaxy.Network;

namespace MCGalaxy.Games {
    
    public sealed partial class ZombieGame {
        
        void MainLoop() {
            // Make sure that in the worst case, we do not crash the entire server.
            try {
                MainLoopCore();
            } catch (Exception ex) {
                Server.ErrorLog(ex);
                Chat.MessageGlobal("&cZombie survival disabled due to an error.");
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
            CurLevel.ChatLevel("&c" + first.DisplayName + " %Sstarted the infection!");
            InfectPlayer(first, null);

            DoCoreGame(random);
            if (!Running) { Status = ZombieGameStatus.LastRound; return; }
            
            EndRound();
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
            string lastTimeLeft = null;
            int lastCountdown = -1;
            
            while (alive.Length > 0 && Running) {
                Player[] infected = Infected.Items;
                // Do round end.
                int seconds = (int)(RoundEnd - DateTime.UtcNow).TotalSeconds;
                if (seconds <= 0) { 
                    SendLevelRaw("", true); 
                    EndRound();
                    return; 
                }                
                if (seconds <= 5 && seconds != lastCountdown) {
                    string suffix = seconds == 1 ? " &4second" : " &4seconds";
                    SendLevelRaw("&4Ending in &f" + seconds + suffix, true);
                    lastCountdown = seconds;
                }
                
                // Update the round time left shown in the top right
                string timeLeft = HUD.GetTimeLeft(seconds);
                if (lastTimeLeft != timeLeft) {
                    HUD.UpdateAllPrimary(this);
                    lastTimeLeft = timeLeft;
                }
                
                DoCollisions(alive, infected, random);
                CheckInvisibilityTime();
                Thread.Sleep(200);
                alive = Alive.Items;
            }
        }
        
        void DoCollisions(Player[] aliveList, Player[] deadList, Random random) {
            int dist = ZombieGameProps.HitboxPrecision;
            foreach (Player killer in deadList) {
                killer.Game.Infected = true;
                UpdatePlayerColor(killer, InfectCol);
                aliveList = Alive.Items;

                foreach (Player alive in aliveList) {
                    if (alive == killer) continue;
                    UpdatePlayerColor(alive, alive.color);
                    if (!MovementCheck.InRange(alive, killer, dist)) continue;
                    
                    if (killer.Game.Infected && !alive.Game.Infected
                        && !alive.Game.Referee && !killer.Game.Referee
                        && killer.level.name.CaselessEq(CurLevelName)
                        && alive.level.name.CaselessEq(CurLevelName))
                    {
                        InfectPlayer(alive, killer);
                        alive.Game.LastInfecter = killer.name;
                        
                        if (lastPlayerToInfect == killer.name) {
                            infectCombo++;
                            if (infectCombo >= 2) {
                                killer.SendMessage("You gained " + (2 + infectCombo) + " " + Server.moneys);
                                killer.SetMoney(killer.money + (2 + infectCombo));
                                CurLevel.ChatLevel("&c" + killer.DisplayName + " %Sis on a rampage! " + (infectCombo + 1) + " infections in a row!");
                            }
                        } else {
                            infectCombo = 0;
                        }
                        
                        lastPlayerToInfect = killer.name;
                        killer.Game.CurrentInfected++;
                        killer.Game.TotalInfected++;
                        killer.Game.MaxInfected = Math.Max(killer.Game.CurrentInfected, killer.Game.MaxInfected);
                        
                        ShowInfectMessage(random, alive, killer);
                        UpdatePlayerColor(alive, InfectCol);
                        Thread.Sleep(50);
                    }
                }
            }
        }
        
        void SendLevelRaw(string message, bool announce = false) {
            Player[] players = PlayerInfo.Online.Items;
            foreach (Player p in players) {
                if (p.level != CurLevel) continue;
                CpeMessageType type = announce && p.HasCpeExt(CpeExt.MessageTypes)
                    ? CpeMessageType.Announcement : CpeMessageType.Normal;
                
                p.Send(Packet.Message(message, type, p.hasCP437));
            }
        }
        
        void CheckInvisibilityTime() {
            DateTime now = DateTime.UtcNow;
            Player[] players = PlayerInfo.Online.Items;
            foreach (Player p in players) {
                if (!p.Game.Invisible || p.level != CurLevel) continue;
                DateTime end = p.Game.InvisibilityEnd;
                if (now >= end) {
                    Player.Message(p, "&cYou are &bvisible &cagain");
                    ResetInvisibility(p); 
                    continue;
                }
                
                int left = (int)Math.Ceiling((end - now).TotalSeconds);
                if (left == p.Game.InvisibilityTime) continue;
                p.Game.InvisibilityTime = left;
                
                string msg = "&bInvisibility for &a" + left;
                if (p.HasCpeExt(CpeExt.MessageTypes)) {
                    p.SendCpeMessage(CpeMessageType.BottomRight2, msg);
                } else {
                    Player.Message(p, msg);
                }
            }
        }
        
        void CheckHumanPledge(Player p, Player killer) {
            if (!p.Game.PledgeSurvive) return;
            p.Game.PledgeSurvive = false;
            CurLevel.ChatLevel("&c" + p.DisplayName + " %Sbroke their pledge of not being infected.");
            
            if (killer == null) {
                Player.Message(p, "As this was an automatic infection, you have not lost any &3" + Server.moneys);
            } else {
                p.SetMoney(Math.Max(p.money - 2, 0));
            }
        }
        
        void CheckBounty(Player p, Player pKiller) {
            BountyData bounty = FindBounty(p.name);
            if (bounty == null) return;
            Bounties.Remove(bounty);
            
            Player setter = PlayerInfo.FindExact(bounty.Origin);
            if (pKiller == null) {
                CurLevel.ChatLevel("Bounty on " + p.ColoredName + " %Sis no longer active.");
                if (setter != null) setter.SetMoney(setter.money + bounty.Amount);
            } else if (setter == null) {
                Player.Message(pKiller, "Cannot collect the bounty, as the player who set it is offline.");
            } else {
                CurLevel.ChatLevel("&c" + pKiller.DisplayName + " %Scollected the bounty of &a" +
                                   bounty.Amount + " %S" + Server.moneys + " on " + p.ColoredName + "%S.");
                pKiller.SetMoney(pKiller.money + bounty.Amount);
            }
        }
        
        void ShowInfectMessage(Random random, Player pAlive, Player pKiller) {
            string text = null;
            List<string> infectMsgs = pKiller.Game.InfectMessages;
            if (infectMsgs != null && infectMsgs.Count > 0 && random.Next(0, 10) < 5) {
                text = infectMsgs[random.Next(infectMsgs.Count)];
            } else {
                text = infectMessages[random.Next(infectMessages.Count)];
            }
            
            CurLevel.ChatLevel(String.Format(text,
                                             "&c" + pKiller.DisplayName + "%S",
                                             pAlive.ColoredName + "%S"));
        }

        static void UpdatePlayerColor(Player p, string color) {
            if (p.Game.lastSpawnColor == color) return;
            p.Game.lastSpawnColor = color;
            Entities.GlobalDespawn(p, false);
            Entities.GlobalSpawn(p, false);
            TabList.Add(p, p, Entities.SelfID);
        }

        public void EndRound() {
            if (!RoundInProgress) return;
            RoundInProgress = false;
            RoundStart = DateTime.MinValue;
            RoundEnd = DateTime.MinValue;
            HUD.UpdateAllPrimary(this);
            
            if (!Running) return;
            Rewards.HandOut(this);
        }
    }
}
