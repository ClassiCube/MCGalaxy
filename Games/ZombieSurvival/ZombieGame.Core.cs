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
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Timers;

namespace MCGalaxy.Games {
    
    public sealed partial class ZombieGame {
        
        void MainLoop() {
            if (Status == ZombieGameStatus.NotStarted) return;
            if (!initialChangeLevel) {
                ChooseNextLevel();
                initialChangeLevel = true;
            }

            while (true) {
                RoundInProgress = false;
                RoundsDone++;
                
                if (Status == ZombieGameStatus.NotStarted) {
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
            if (Status == ZombieGameStatus.NotStarted) return;
            List<Player> players = DoRoundCountdown();
            RoundInProgress = true;
            Random random = new Random();
            
        pickFirst:
            int firstinfect = random.Next(players.Count());
            Player first = null;
            if (queZombie) first = PlayerInfo.Find(nextZombie);
            else first = players[firstinfect];
            queZombie = false;

            if (!first.level.name.CaselessEq(CurrentLevelName)) goto pickFirst;

            CurrentLevel.ChatLevel(first.color + first.name + " %Sstarted the infection!");
            first.infected = true;
            UpdatePlayerColor(first, Colors.red);

            RoundInProgress = true;
            int roundMins = random.Next(4, 7);
            CurrentLevel.ChatLevel("The round will last for " + roundMins + " minutes!");
            RoundEnd = DateTime.UtcNow.AddMinutes(roundMins);
            timer = new System.Timers.Timer(roundMins * 60 * 1000);
            timer.Elapsed += new ElapsedEventHandler(EndRound);
            timer.Enabled = true;

            Player[] online = PlayerInfo.Online.Items;
            foreach (Player p in online) {
                if (p != first) Alive.Add(p);
            }

            Infected.Clear();
            Infected.Add(first);
            UpdateAllPlayerStatus();
            DoCoreGame(random);
            
            if (Status == ZombieGameStatus.NotStarted) {
                Status = ZombieGameStatus.LastRound; return;
            } else {
                HandOutRewards();
            }
        }
        
        List<Player> DoRoundCountdown() {
            while (true) {
                string logMessage = ChangeLevels + " " + Server.ZombieOnlyServer + " " + UseLevelList;
                Server.s.Log(logMessage);
                
                RoundStart = DateTime.UtcNow.AddSeconds(30);
                CurrentLevel.ChatLevel("%4Round Start:%f 30...");
                Thread.Sleep(20000); if (!Server.ZombieModeOn) return null;
                CurrentLevel.ChatLevel("%4Round Start:%f 10...");
                Thread.Sleep(10000); if (!Server.ZombieModeOn) return null;
                CurrentLevel.ChatLevel("%4Round Start:%f 5...");
                Thread.Sleep(1000); if (!Server.ZombieModeOn) return null;
                CurrentLevel.ChatLevel("%4Round Start:%f 4...");
                Thread.Sleep(1000); if (!Server.ZombieModeOn) return null;
                CurrentLevel.ChatLevel("%4Round Start:%f 3...");
                Thread.Sleep(1000); if (!Server.ZombieModeOn) return null;
                CurrentLevel.ChatLevel("%4Round Start:%f 2...");
                Thread.Sleep(1000); if (!Server.ZombieModeOn) return null;
                CurrentLevel.ChatLevel("%4Round Start:%f 1...");
                Thread.Sleep(1000); if (!Server.ZombieModeOn) return null;
                int nonRefPlayers = 0;
                List<Player> players = new List<Player>();
                
                Player[] online = PlayerInfo.Online.Items;
                foreach (Player p in online) {
                    if (!p.referee && p.level.name.CaselessEq(CurrentLevelName)) {
                        players.Add(p);
                        nonRefPlayers++;
                    }
                }
                
                if (nonRefPlayers >= 2) return players;
                CurrentLevel.ChatLevel(Colors.red + "ERROR: Need 2 or more players to play");
            }
        }
        
        void DoCoreGame(Random random) {
            Player[] alive = null;
            while ((alive = Alive.Items).Length > 0) {
                Player[] infected = Infected.Items;
                foreach (Player pKiller in infected) {
                    pKiller.infected = true;
                    UpdatePlayerColor(pKiller, Colors.red);
                    bool aliveChanged = false;
                    foreach (Player pAlive in alive) {
                        UpdatePlayerColor(pAlive, pAlive.group.color);
                        if (Math.Abs(pAlive.pos[0] - pKiller.pos[0]) > HitboxPrecision
                            || Math.Abs(pAlive.pos[1] - pKiller.pos[1]) > HitboxPrecision
                            || Math.Abs(pAlive.pos[2] - pKiller.pos[2]) > HitboxPrecision)
                            continue;
                        
                        if (!pAlive.infected && pKiller.infected && !pAlive.referee && !pKiller.referee && pKiller != pAlive
                            && pKiller.level.name.CaselessEq(CurrentLevelName) && pAlive.level.name.CaselessEq(CurrentLevelName))
                        {
                            InfectPlayer(pAlive);
                            aliveChanged = true;
                            pAlive.blockCount = 25;
                            
                            if (lastPlayerToInfect == pKiller.name) {
                                infectCombo++;
                                if (infectCombo >= 2) {
                                    pKiller.SendMessage("You gained " + (4 + infectCombo) + " " + Server.moneys);
                                    pKiller.money += 4 + infectCombo;
                                    pKiller.OnMoneyChanged();
                                    CurrentLevel.ChatLevel(pKiller.FullName + " is on a rampage! " + (infectCombo + 1) + " infections in a row!");
                                }
                            } else {
                                infectCombo = 0;
                            }
                            
                            lastPlayerToInfect = pKiller.name;
                            pKiller.playersInfected++;
                            CurrentLevel.ChatLevel(String.Format(
                                messages[random.Next(messages.Length)],
                                Colors.red + pKiller.DisplayName + Colors.yellow,
                                Colors.red + pAlive.DisplayName + Colors.yellow));
                            
                            CheckAssertHuman(pAlive);
                            CheckBounty(pAlive, pKiller);
                            UpdatePlayerColor(pAlive, Colors.red);
                        }
                    }
                    if (aliveChanged) alive = Alive.Items;
                }
                Thread.Sleep(50);
            }
        }
        
        void CheckAssertHuman(Player pAlive) {
            if (!pAlive.pledgeSurvive) return;
            pAlive.pledgeSurvive = false;
            CurrentLevel.ChatLevel(pAlive.FullName + "%Sbroke their pledge of not being infected.");
            pAlive.money = Math.Max(pAlive.money - 2, 0);
            pAlive.OnMoneyChanged();
        }
        
        void CheckBounty(Player pAlive, Player pKiller) {
            BountyData bounty;
            if (Bounties.TryGetValue(pAlive.name, out bounty))
                Bounties.Remove(pAlive.name);
            if (bounty != null) {
                CurrentLevel.ChatLevel(pKiller.FullName + " %Scollected the bounty of &a" +
                                       bounty.Amount + " %S" + Server.moneys + " on " + pAlive.FullName + "%S.");
                bounty.Origin.money = Math.Max(0, bounty.Origin.money - bounty.Amount);
                bounty.Origin.OnMoneyChanged();
                pKiller.money += bounty.Amount;
                pKiller.OnMoneyChanged();
            }
        }

        static void UpdatePlayerColor(Player p, string color) {
            if (p.lastSpawnColor == color) return;
            p.lastSpawnColor = color;
            Player.GlobalDespawn(p, false);
            Player.GlobalSpawn(p, p.pos[0], p.pos[1], p.pos[2], p.rot[0], p.rot[1], false);
        }
        
        public void EndRound(object sender, ElapsedEventArgs e) {
            if (Status == ZombieGameStatus.NotStarted) return;
            CurrentLevel.ChatLevel("%4Round End:%f 5"); Thread.Sleep(1000);
            CurrentLevel.ChatLevel("%4Round End:%f 4"); Thread.Sleep(1000);
            CurrentLevel.ChatLevel("%4Round End:%f 3"); Thread.Sleep(1000);
            CurrentLevel.ChatLevel("%4Round End:%f 2"); Thread.Sleep(1000);
            CurrentLevel.ChatLevel("%4Round End:%f 1"); Thread.Sleep(1000);
            HandOutRewards();
        }

        public void HandOutRewards() {
            RoundInProgress = false;
            RoundStart = DateTime.MinValue;
            RoundEnd = DateTime.MinValue;
            Bounties.Clear();
            if (Status == ZombieGameStatus.NotStarted) return;
            
            Player[] alive = Alive.Items;
            CurrentLevel.ChatLevel(Colors.lime + "The game has ended!");
            if (alive.Length == 0) CurrentLevel.ChatLevel(Colors.maroon + "Zombies have won this round.");
            else if (alive.Length == 1) CurrentLevel.ChatLevel(Colors.green + "Congratulations to the sole survivor:");
            else CurrentLevel.ChatLevel(Colors.green + "Congratulations to the survivors:");
            
            timer.Enabled = false;
            string playersString = "";
            Player[] online = null;
            
            if (alive.Length == 0) {
                online = PlayerInfo.Online.Items;
                foreach (Player pl in online)
                    ResetPlayer(pl, ref playersString);
            } else {
                foreach (Player pl in alive) {
                    if (pl.pledgeSurvive) {
                        pl.SendMessage("You received &a5 %3" + Server.moneys + 
                                       "%s for successfully pledging that you would survive.");
                        pl.money += 5;
                        pl.OnMoneyChanged();
                    }
                    ResetPlayer(pl, ref playersString);
                }
            }
            
            CurrentLevel.ChatLevel(playersString);
            online = PlayerInfo.Online.Items;
            Random rand = new Random();
            foreach (Player pl in online) {
                int money = 0;
                if (!pl.level.name.CaselessEq(CurrentLevelName)) continue;
                bool inBlock = pl.CheckIfInsideBlock();
                
                if (!inBlock && alive.Length == 0) {
                    money = rand.Next(1, 5 + pl.playersInfected);
                } else if (!inBlock && (alive.Length == 1 && !pl.infected)) {
                    money = rand.Next(5, 15);
                } else if (inBlock) {
                    money = -1;
                }
                
                Player.GlobalDespawn(pl, false);
                Player.GlobalSpawn(pl, pl.pos[0], pl.pos[1], pl.pos[2], pl.rot[0], pl.rot[1], false);
                if (money == -1) {
                    pl.SendMessage("You may not hide inside a block! No " + Server.moneys + " for you."); money = 0;
                } else if (money > 0) {
                    pl.SendMessage( Colors.gold + "You gained " + money + " " + Server.moneys);
                }
                
                pl.blockCount = 50;
                pl.playersInfected = 0;
                pl.money += money;
                pl.infected = false;
                if (pl.referee) {
                    pl.SendMessage("You gained one " + Server.moneys + " because you're a ref. Would you like a medal as well?");
                    pl.money++;
                }
                pl.OnMoneyChanged();
            }
            Alive.Clear();
            Infected.Clear();
            UpdateAllPlayerStatus();
        }

        void ResetPlayer(Player p, ref string playersString) {
            p.blockCount = 50;
            p.infected = false;
            p.playersInfected = 0;
            
            if (p.level.name.CaselessEq(CurrentLevelName))
                playersString += p.color + p.DisplayName + Colors.white + ", ";
        }
        
        void ChooseNextLevel() {
            if (queLevel) { ChangeLevel(nextLevel); return; }
            if (!ChangeLevels) return;
            
            try
            {
                List<string> levels = GetCandidateLevels();
                if (levels.Count <= 2 && !UseLevelList) { Server.s.Log("You must have more than 2 levels to change levels in Zombie Survival"); return; }

                if (levels.Count < 2 && UseLevelList) { Server.s.Log("You must have more than 2 levels in your level list to change levels in Zombie Survival"); return; }

                string selectedLevel1 = "", selectedLevel2 = "";
                Random r = new Random();

            LevelChoice:
                string level = levels[r.Next(0, levels.Count)];
                string level2 = levels[r.Next(0, levels.Count)];

                if (lastLevel1 == level || lastLevel2 == level2 || lastLevel1 == level2 ||
                    lastLevel2 == level || CurrentLevelName == level || CurrentLevelName == level2) {
                    goto LevelChoice;
                } else if (selectedLevel1 == "") {
                    selectedLevel1 = level; goto LevelChoice;
                } else {
                    selectedLevel2 = level2;
                }

                Level1Vote = 0; Level2Vote = 0; Level3Vote = 0;
                lastLevel1 = selectedLevel1; lastLevel2 = selectedLevel2;

                if (Status == ZombieGameStatus.NotStarted || Status == ZombieGameStatus.LastRound)
                    return;

                if (initialChangeLevel) {
                    Server.votingforlevel = true;
                    CurrentLevel.ChatLevel(" " + Colors.black + "Level Vote: %S" + selectedLevel1 + ", " + selectedLevel2 +
                                           " or random " + "(" + Colors.lime + "1%S/" + Colors.red + "2%S/" + Colors.blue + "3%S)");
                    System.Threading.Thread.Sleep(15000);
                    Server.votingforlevel = false;
                } else { Level1Vote = 1; Level2Vote = 0; Level3Vote = 0; }

                if (Status == ZombieGameStatus.NotStarted || Status == ZombieGameStatus.LastRound)
                    return;

                if (Level1Vote >= Level2Vote) {
                    if (Level3Vote > Level1Vote && Level3Vote > Level2Vote) {
                        ChangeLevel(levels[r.Next(0, levels.Count)]);
                    } else {
                        ChangeLevel(selectedLevel1);
                    }
                } else {
                    if (Level3Vote > Level1Vote && Level3Vote > Level2Vote) {
                        ChangeLevel(levels[r.Next(0, levels.Count)]);
                    } else {
                        ChangeLevel(selectedLevel2);
                    }
                }
                Player[] online = PlayerInfo.Online.Items;
                foreach (Player pl in online)
                    pl.voted = false;
            } catch (Exception ex) {
                Server.ErrorLog(ex);
            }
        }
        
        List<string> GetCandidateLevels() {
            if (UseLevelList) return LevelList;
            
            List<string> maps = new List<string>();
            DirectoryInfo di = new DirectoryInfo("levels/");
            FileInfo[] fi = di.GetFiles("*.lvl");
            foreach (FileInfo fil in fi)
                maps.Add(fil.Name.Split('.')[0]);
            return maps;
        }
    }
}
