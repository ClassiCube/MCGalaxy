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

namespace MCGalaxy.Games {
    
    public sealed partial class ZombieGame {
        
        void MainLoop() {
            // Make sure that in the worst case, we do not crash the entire server.
            try {
                MainLoopCore();
            } catch (Exception ex) {
                Server.ErrorLog(ex);
                Player.GlobalMessage("&cZombie survival disabled due to an error.");
                try {
                    ResetState();
                } catch (Exception ex2) {
                    Server.ErrorLog(ex2);
                }
            }
        }
        
        void MainLoopCore() {
            if (Status == ZombieGameStatus.NotStarted) return;
            if (!initialChangeLevel) {
                ChooseNextLevel();
                initialChangeLevel = true;
            }

            while (true) {
                RoundInProgress = false;
                RoundsDone++;
                
                if (!Running) {
                    return;
                } else if (Status == ZombieGameStatus.InfiniteRounds) {
                    DoRound();
                    if (ChangeLevels) ChooseNextLevel();
                } else if (Status == ZombieGameStatus.SingleRound) {
                    DoRound();
                    ResetState(); return;
                } else if (Status == ZombieGameStatus.VariableRounds) {
                    if (RoundsDone == MaxRounds) {
                        ResetState(); return;
                    } else {
                        DoRound();
                        if (ChangeLevels) ChooseNextLevel();
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
            RoundInProgress = true;
            Random random = new Random();
            Player first = PickFirstZombie(random, players);

            CurLevel.ChatLevel(first.color + first.name + " %Sstarted the infection!");
            first.Game.Infected = true;
            PlayerMoneyChanged(first);
            UpdatePlayerColor(first, InfectCol);

            RoundInProgress = true;
            int roundMins = random.Next(CurLevel.MinRoundTime, CurLevel.MaxRoundTime);
            string suffix = roundMins == 1 ? " %Sminute!" : " %Sminutes!";
            CurLevel.ChatLevel("This round will last for &a" + roundMins + suffix);
            RoundEnd = DateTime.UtcNow.AddMinutes(roundMins);
            
            Player[] online = PlayerInfo.Online.Items;
            foreach (Player p in online) {
                if (p.level == null || p.level != CurLevel || p.Game.Referee) continue;
                if (p != first) Alive.Add(p);
            }

            Infected.Clear();
            Infected.Add(first);
            UpdateAllPlayerStatus();
            DoCoreGame(random);
            
            if (!Running) {
                Status = ZombieGameStatus.LastRound; return;
            } else {
                HandOutRewards();
            }
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
                SendLevelRaw("&4Round Start:&f 30...");
                Thread.Sleep(20000); if (!Running) return null;
                SendLevelRaw("&4Round Start:&f 10...");
                Thread.Sleep(5000); if (!Running) return null;
                SendLevelRaw("&4Round Start:&f 5...");
                Thread.Sleep(1000); if (!Running) return null;
                SendLevelRaw("&4Round Start:&f 4...");
                Thread.Sleep(1000); if (!Running) return null;
                SendLevelRaw("&4Round Start:&f 3...");
                Thread.Sleep(1000); if (!Running) return null;
                SendLevelRaw("&4Round Start:&f 2...");
                Thread.Sleep(1000); if (!Running) return null;
                SendLevelRaw("&4Round Start:&f 1...");
                Thread.Sleep(1000); if (!Running) return null;
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
            Player[] alive = null;
            string lastTimespan = null;
            int lastTime = -1;
            
            while ((alive = Alive.Items).Length > 0 && Running) {
                Player[] infected = Infected.Items;
                // Do round end.
                int seconds = (int)(RoundEnd - DateTime.UtcNow).TotalSeconds;
                if (seconds <= 0) { HandOutRewards(); return; }
                if (seconds <= 5 && seconds != lastTime) {
                     SendLevelRaw("&4Round End:&f " + seconds);
                     lastTime = seconds;
                }
                
                // Update the round time left shown in the top right
                string timespan = GetTimespan(seconds);
                if (lastTimespan != timespan) {
                    UpdateAllPlayerStatus(timespan);
                    lastTimespan = timespan;
                }
                
                foreach (Player pKiller in infected) {
                    pKiller.Game.Infected = true;
                    UpdatePlayerColor(pKiller, InfectCol);
                    bool aliveChanged = false;
                    foreach (Player pAlive in alive) {
                        UpdatePlayerColor(pAlive, pAlive.color);
                        if (Math.Abs(pAlive.pos[0] - pKiller.pos[0]) > HitboxPrecision
                            || Math.Abs(pAlive.pos[1] - pKiller.pos[1]) > HitboxPrecision
                            || Math.Abs(pAlive.pos[2] - pKiller.pos[2]) > HitboxPrecision)
                            continue;
                        
                        if (!pAlive.Game.Infected && pKiller.Game.Infected && !pAlive.Game.Referee 
                            && !pKiller.Game.Referee && pKiller != pAlive
                            && pKiller.level.name.CaselessEq(CurLevelName) 
                            && pAlive.level.name.CaselessEq(CurLevelName))
                        {
                            if (pAlive.Game.Invisible)
                                ZombieAwards.Give(pKiller, ZombieAwards.killInvisHuman, this);
                            InfectPlayer(pAlive);
                            aliveChanged = true;
                            pAlive.Game.BlocksLeft = 25;
                            
                            if (lastPlayerToInfect == pKiller.name) {
                                infectCombo++;
                                if (infectCombo >= 2) {
                                    pKiller.SendMessage("You gained " + (2 + infectCombo) + " " + Server.moneys);
                                    pKiller.money += 2 + infectCombo;
                                    pKiller.OnMoneyChanged();
                                    CurLevel.ChatLevel(pKiller.ColoredName + " is on a rampage! " + (infectCombo + 1) + " infections in a row!");
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
                        }
                    }
                    if (aliveChanged) alive = Alive.Items;
                }
                
                CheckInvisibilityTime(Alive.Items);
                Thread.Sleep(25);
            }
        }
        
        void SendLevelRaw(string message) {
            Player[] players = PlayerInfo.Online.Items;
            foreach (Player p in players) {
                if (p.level != CurLevel) continue;
                p.SendRawMessage(CpeMessageType.Normal, message);
            }
        }
        
        void CheckInvisibilityTime(Player[] alive) {
            DateTime now = DateTime.UtcNow;
            foreach (Player p in alive) {
                if (!p.Game.Invisible) continue;
                DateTime end = p.Game.InvisibilityEnd;
                
                if (now >= end) {
                    p.SendCpeMessage(CpeMessageType.BottomRight2, "", false);
                    p.Game.ResetInvisibility();
                    Entities.GlobalSpawn(p, false);
                    continue;
                }
                
                int left = (int)Math.Ceiling((end - now).TotalSeconds);
                if (left == p.Game.InvisibilityTime) continue;
                p.Game.InvisibilityTime = left;
                
                string msg = "&bInvisibility for &a" + left;
                if (p.HasCpeExt(CpeExt.MessageTypes))
                    p.SendCpeMessage(CpeMessageType.BottomRight2, msg, true);
                else
                    p.SendMessage(msg, true);
            }
        }
        
        void CheckHumanPledge(Player pAlive) {
            if (!pAlive.Game.PledgeSurvive) return;
            pAlive.Game.PledgeSurvive = false;
            CurLevel.ChatLevel(pAlive.ColoredName + "%Sbroke their pledge of not being infected.");
            pAlive.money = Math.Max(pAlive.money - 2, 0);
            pAlive.OnMoneyChanged();
        }
        
        void ShowInfectMessage(Random random, Player pAlive, Player pKiller) {
            string text = null;
            List<string> infectMsgs = pKiller.Game.InfectMessages;
            if (infectMsgs != null && random.Next(0, 10) < 5)
                text = infectMsgs[random.Next(infectMsgs.Count)];
            else
                text = messages[random.Next(messages.Count)];
            
            CurLevel.ChatLevel(String.Format(text,
                                             Colors.red + pKiller.DisplayName + Colors.yellow,
                                             Colors.red + pAlive.DisplayName + Colors.yellow));
        }
        
        void CheckBounty(Player pAlive, Player pKiller) {
            BountyData bounty;
            if (Bounties.TryGetValue(pAlive.name, out bounty))
                Bounties.Remove(pAlive.name);
            if (bounty != null) {
                CurLevel.ChatLevel(pKiller.ColoredName + " %Scollected the bounty of &a" +
                                   bounty.Amount + " %S" + Server.moneys + " on " + pAlive.ColoredName + "%S.");
                bounty.Origin.money = Math.Max(0, bounty.Origin.money - bounty.Amount);
                bounty.Origin.OnMoneyChanged();
                pKiller.money += bounty.Amount;
                pKiller.OnMoneyChanged();
            }
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
            
            Player[] alive = Alive.Items;
            CurLevel.ChatLevel("&aThe game has ended!");
            if (alive.Length == 0) CurLevel.ChatLevel("&4Zombies have won this round.");
            else if (alive.Length == 1) CurLevel.ChatLevel("&2Congratulations to the sole survivor:");
            else CurLevel.ChatLevel("&2Congratulations to the survivors:");
            
            string playersString = "";
            Player[] online = null;
            CurLevel.RoundsPlayed++;
            
            if (alive.Length == 0) {
                online = PlayerInfo.Online.Items;
                foreach (Player pl in online)
                    ResetPlayer(pl, ref playersString);
            } else {
               CurLevel.RoundsHumanWon++;
                foreach (Player pl in alive) {
                    if (pl.Game.PledgeSurvive) {
                        pl.SendMessage("You received &a5 %3" + Server.moneys +
                                       "%s for successfully pledging that you would survive.");
                        pl.money += 5;
                        pl.OnMoneyChanged();
                    }
                    pl.Game.CurrentRoundsSurvived++;
                    pl.Game.TotalRoundsSurvived++;
                    pl.Game.MaxRoundsSurvived = Math.Max(pl.Game.CurrentRoundsSurvived, pl.Game.MaxRoundsSurvived);
                    ResetPlayer(pl, ref playersString);
                    pl.SetPrefix();
                }
            }
            
            CurLevel.ChatLevel(playersString);
            online = PlayerInfo.Online.Items;
            Random rand = new Random();
            foreach (Player pl in online) {
                pl.Game.ResetInvisibility();
                if (!pl.level.name.CaselessEq(CurLevelName)) continue;
                int money = GetMoney(pl, alive, rand);
                
                Entities.GlobalDespawn(pl, true);
                Entities.GlobalSpawn(pl, true);
                if (money == -1) {
                    pl.SendMessage("You may not hide inside a block! No " + Server.moneys + " for you."); money = 0;
                } else if (money > 0) {
                    pl.SendMessage( Colors.gold + "You gained " + money + " " + Server.moneys);
                }
                
                pl.Game.BlocksLeft = 50;
                pl.Game.CurrentInfected = 0;
                pl.money += money;
                pl.Game.Infected = false;
                pl.Game.InvisibilityPotions = 0;
                if (pl.Game.Referee) {
                    pl.SendMessage("You gained one " + Server.moneys + " because you're a ref. Would you like a medal as well?");
                    pl.money++;
                }
                pl.OnMoneyChanged();
            }
            
            DoLottery();
            UpdateAllPlayerStatus();
            Alive.Clear();
            Infected.Clear();
        }
        
        void DoLottery() {
            string[] players = Lottery.Items;
            if (players.Length == 0) return;
            
            // Ensure the players are actually online
            List<Player> online = new List<Player>(players.Length);
            foreach (string name in players) {
                Player pl = PlayerInfo.FindExact(name);
                if (pl == null) continue;
                online.Add(pl);
            }
            if (online.Count == 0) return;
            
            int amount = 10;
            Player winner = online[0];
            if (online.Count == 1) {
                winner.SendMessage("Your money was refunded as you were " +
                                      "the only player still in the lottery.");
            } else {
                Random rand = new Random();
                winner = online[rand.Next(online.Count)];
                amount = 9 * online.Count;
                CurLevel.ChatLevel(winner.ColoredName + " %Swon the lottery for &6" 
                                   + amount + " " + Server.moneys);
            }
            Lottery.Clear();
            winner.money += 10;
            winner.OnMoneyChanged();
        }
        
        int GetMoney(Player pl, Player[] alive, Random rand) {
            if (pl.CheckIfInsideBlock()) return -1;
            
            if (alive.Length == 0) {
                return rand.Next(1 + pl.Game.CurrentInfected, 5 + pl.Game.CurrentInfected);
            } else if (alive.Length == 1 && !pl.Game.Infected) {
                return rand.Next(5, 10);
            } else if (alive.Length > 1 && !pl.Game.Infected) {
                return rand.Next(2, 6);
            }
            return 0;
        }

        void ResetPlayer(Player p, ref string playersString) {
            p.Game.BlocksLeft = 50;
            p.Game.Infected = false;
            
            if (p.level.name.CaselessEq(CurLevelName))
                playersString += p.ColoredName + Colors.white + ", ";
        }
        
        void ChooseNextLevel() {
            if (QueuedLevel != null) { ChangeLevel(QueuedLevel); return; }
            if (!ChangeLevels) return;
            
            try
            {
                bool useLevelList = LevelList.Count > 0;
                List<string> levels = GetCandidateLevels();
                foreach (string ignore in IgnoredLevelList)
                    levels.Remove(ignore);
                
                if (levels.Count <= 2 && !useLevelList) { Server.s.Log("You must have more than 2 levels to change levels in Zombie Survival"); return; }
                if (levels.Count <= 2 && useLevelList) { Server.s.Log("You must have more than 2 levels in your level list to change levels in Zombie Survival"); return; }

                string picked1 = "", picked2 = "";
                Random r = new Random();

            LevelChoice:
                string level = levels[r.Next(0, levels.Count)];
                string level2 = levels[r.Next(0, levels.Count)];

                if (level == lastLevel1 || level == lastLevel2 || level == CurLevelName ||
                    level2 == lastLevel1 || level2 == lastLevel2 || level2 == CurLevelName ||
                    level == picked1) {
                    goto LevelChoice;
                } else if (picked1 == "") {
                    picked1 = level; goto LevelChoice;
                } else {
                    picked2 = level2;
                }

                Level1Vote = 0; Level2Vote = 0; Level3Vote = 0;
                lastLevel1 = picked1; lastLevel2 = picked2;
                if (!Running || Status == ZombieGameStatus.LastRound) return;

                if (initialChangeLevel) {
                    Server.votingforlevel = true;
                    Player[] players = PlayerInfo.Online.Items;
                    foreach (Player pl in players) {
                        if (pl.level != CurLevel) continue;
                        SendVoteMessage(pl, picked1, picked2);
                    }
                    System.Threading.Thread.Sleep(15000);
                    Server.votingforlevel = false;
                } else { Level1Vote = 1; Level2Vote = 0; Level3Vote = 0; }

                if (!Running || Status == ZombieGameStatus.LastRound) return;
                MoveToNextLevel(r, levels, picked1, picked2);
            } catch (Exception ex) {
                Server.ErrorLog(ex);
            }
        }
        
        void MoveToNextLevel(Random r, List<string> levels, string picked1, string picked2) {
            if (Level1Vote >= Level2Vote) {
                if (Level3Vote > Level1Vote && Level3Vote > Level2Vote) {
                    ChangeLevel(GetRandomLevel(r, levels));
                } else {
                    ChangeLevel(picked1);
                }
            } else {
                if (Level3Vote > Level1Vote && Level3Vote > Level2Vote) {
                    ChangeLevel(GetRandomLevel(r, levels));
                } else {
                    ChangeLevel(picked2);
                }
            }
            Player[] online = PlayerInfo.Online.Items;
            foreach (Player pl in online)
                pl.voted = false;
        }
        
        string GetRandomLevel(Random r, List<string> levels) {
            for (int i = 0; i < 100; i++) {
                string lvl = levels[r.Next(0, levels.Count)];
                if (!lvl.CaselessEq(CurLevelName)) return lvl;
            }
            return levels[r.Next(0, levels.Count)];
        }
        
        List<string> GetCandidateLevels() {
            if (LevelList.Count > 0) return LevelList;           
            List<string> maps = new List<string>();
            DirectoryInfo di = new DirectoryInfo("levels/");
            FileInfo[] fi = di.GetFiles("*.lvl");
            
            foreach (FileInfo fil in fi) {
                string mapName = Path.GetFileNameWithoutExtension(fil.Name);
                if (mapName.IndexOf('+') >= 0 && IgnorePersonalWorlds) 
                    continue;
                maps.Add(mapName);
            }
            return maps;
        }
        
        void SendVoteMessage(Player p, string lvl1, string lvl2) {
            const string line1 = "&eLevel vote - type &a1&e, &c2&e or &93";
            string line2 = "&a" + lvl1 + "&e, &c" + lvl2 + "&e, &9random";
            if (p.HasCpeExt(CpeExt.MessageTypes)) {
                p.SendCpeMessage(CpeMessageType.BottomRight2, line1, true);
                p.SendCpeMessage(CpeMessageType.BottomRight1, line2, true);
            } else {
                p.SendMessage(line1, true);
                p.SendMessage(line2, true);
            }
        }
    }
}
