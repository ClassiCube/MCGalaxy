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

namespace MCGalaxy {
    
    public sealed partial class ZombieGame {
        
        void MainLoop() {
            if (Status == ZombieGameStatus.NotStarted) return;
            if (!initialChangeLevel) {
                ChangeLevel();
                initialChangeLevel = true;
            }

            while (true) {
                RoundInProgress = false;
                RoundsDone++;
                
                if (Status == ZombieGameStatus.NotStarted) {
                    return;
                } else if (Status == ZombieGameStatus.InfiniteRounds) {
                    DoRound();
                    if (ChangeLevels) ChangeLevel();
                } else if (Status == ZombieGameStatus.SingleRound) {
                    DoRound();
                    ResetState(); return;
                } else if (Status == ZombieGameStatus.VariableRounds) {
                    if (RoundsDone == MaxRounds) {
                        ResetState(); return;
                    } else {
                        DoRound();
                        if (ChangeLevels) ChangeLevel();
                    }
                } else if (Status == ZombieGameStatus.LastRound) {
                    ResetState(); return;
                }
            }
        }

        void DoRound() {
            if (Status == ZombieGameStatus.NotStarted) return;
            List<Player> players = DoRoundCountdown();

        theEnd:
            Random random = new Random();
            int firstinfect = random.Next(players.Count());
            Player player = null;
            if (queZombie) player = PlayerInfo.Find(nextZombie);
            else player = players[firstinfect];

            if (player.level.name != currentLevelName) goto theEnd;

            Player.GlobalMessage(player.color + player.name + " %Sstarted the infection!");
            player.infected = true;
            UpdatePlayerColor(player, Colors.red);

            RoundInProgress = true;
            int roundMins = random.Next(5, 8);
            Player.GlobalMessage("The round will last for " + roundMins + " minutes!");
            timer = new System.Timers.Timer(roundMins * 60 * 1000);
            timer.Elapsed += new ElapsedEventHandler(EndRound);
            timer.Enabled = true;

            Player[] online = PlayerInfo.Online;
            foreach (Player p in online) {
                if (p != player)
                    alive.Add(p);
            }

            infectd.Clear();
            if (queZombie) infectd.Add(PlayerInfo.Find(nextZombie));
            else infectd.Add(player);
            aliveCount = alive.Count;
            DoCoreGame(players, random);
            
            if (Status == ZombieGameStatus.NotStarted) {
                Status = ZombieGameStatus.LastRound; return;
            } else {
                HandOutRewards();
            }
        }
        
        List<Player> DoRoundCountdown() {
            while (true) {
                string logMessage = Convert.ToString(ChangeLevels) + " " + Convert.ToString(Server.ZombieOnlyServer) +
                    " " + Convert.ToString(UseLevelList) + " " + string.Join(",", LevelList.ToArray());
                Server.s.Log(logMessage);
                
                Player.GlobalMessage("%4Round Start:%f 30...");
                Thread.Sleep(45000); if (!Server.ZombieModeOn) return null;
                Player.GlobalMessage("%4Round Start:%f 10...");
                Thread.Sleep(10000); if (!Server.ZombieModeOn) return null;
                Player.GlobalMessage("%4Round Start:%f 5...");
                Thread.Sleep(1000); if (!Server.ZombieModeOn) return null;
                Player.GlobalMessage("%4Round Start:%f 4...");
                Thread.Sleep(1000); if (!Server.ZombieModeOn) return null;
                Player.GlobalMessage("%4Round Start:%f 3...");
                Thread.Sleep(1000); if (!Server.ZombieModeOn) return null;
                Player.GlobalMessage("%4Round Start:%f 2...");
                Thread.Sleep(1000); if (!Server.ZombieModeOn) return null;
                Player.GlobalMessage("%4Round Start:%f 1...");
                Thread.Sleep(1000); if (!Server.ZombieModeOn) return null;
                RoundInProgress = true;
                int nonRefPlayers = 0;
                List<Player> players = new List<Player>();
                
                Player[] online = PlayerInfo.Online;
                foreach (Player p in online) {
                    if (p.referee) {
                        p.color = p.group.color;
                    } else if (p.level.name == currentLevelName) {
                        p.color = p.group.color;
                        players.Add(p);
                        nonRefPlayers++;
                    }
                }
                
                if (nonRefPlayers >= 2) return players;
                Player.GlobalMessage(Colors.red + "ERROR: Need 2 or more players to play");
            }
        }
        
        void DoCoreGame(List<Player> players, Random random) {
            while (aliveCount > 0) {
                aliveCount = alive.Count;
                infectd.ForEach(
                    delegate(Player pKiller)
                    {
                        UpdatePlayerColor(pKiller, Colors.red);
                        alive.ForEach(
                            delegate(Player pAlive)
                            {
                                UpdatePlayerColor(pAlive, pAlive.group.color);
                                if (Math.Abs(pAlive.pos[0] - pKiller.pos[0]) > HitboxPrecision
                                    || Math.Abs(pAlive.pos[1] - pKiller.pos[1]) > HitboxPrecision
                                    || Math.Abs(pAlive.pos[2] - pKiller.pos[2]) > HitboxPrecision)
                                    return;
                                
                                if (!pAlive.infected && pKiller.infected && !pAlive.referee && !pKiller.referee && 
                                    pKiller != pAlive && pKiller.level.name == currentLevelName && pAlive.level.name == currentLevelName)
                                {
                                    pAlive.infected = true;
                                    infectd.Add(pAlive);
                                    alive.Remove(pAlive);
                                    players.Remove(pAlive);
                                    pAlive.blockCount = 25;
                                    if (lastPlayerToInfect == pKiller.name) {
                                        infectCombo++;
                                        if (infectCombo >= 2) {
                                            pKiller.SendMessage("You gained " + (4 - infectCombo) + " " + Server.moneys);
                                            pKiller.money += 4 - infectCombo;
                                            Player.GlobalMessage(pKiller.FullName + " is on a rampage! " + (infectCombo + 1) + " infections in a row!");
                                        }
                                    } else {
                                        infectCombo = 0;
                                    }
                                    lastPlayerToInfect = pKiller.name;
                                    pKiller.playersInfected++;
                                    Player.GlobalMessage(String.Format(
                                        messages[random.Next(messages.Length)],
                                        Colors.red + pKiller.DisplayName + Colors.yellow,
                                        Colors.red + pAlive.DisplayName + Colors.yellow));
                                    
                                    BountyData bounty;
                                    if (Bounties.TryGetValue(pAlive.name, out bounty))
                                        Bounties.Remove(pAlive.name);
                                    if (bounty != null) {
                                        Player.GlobalMessage(pKiller.FullName + " %Scollected the bounty of &a" +
                                                             bounty.Amount + " %S" + Server.moneys + " on " + pAlive.FullName + "%S.");
                                        bounty.Origin.money = Math.Max(0, bounty.Origin.money - bounty.Amount);
                                        pKiller.money += bounty.Amount;
                                    }
                                    UpdatePlayerColor(pAlive, Colors.red);
                                }
                            });
                    });
                Thread.Sleep(50);
            }
        }

        static void UpdatePlayerColor(Player p, string color) {
            if (p.color == color) return;
            p.color = color;
            Player.GlobalDespawn(p, false);
            Player.GlobalSpawn(p, p.pos[0], p.pos[1], p.pos[2], p.rot[0], p.rot[1], false);
        }
        
        public void EndRound(object sender, ElapsedEventArgs e) {
            if (Status == ZombieGameStatus.NotStarted) return;
            Player.GlobalMessage("%4Round End:%f 5"); Thread.Sleep(1000);
            Player.GlobalMessage("%4Round End:%f 4"); Thread.Sleep(1000);
            Player.GlobalMessage("%4Round End:%f 3"); Thread.Sleep(1000);
            Player.GlobalMessage("%4Round End:%f 2"); Thread.Sleep(1000);
            Player.GlobalMessage("%4Round End:%f 1"); Thread.Sleep(1000);
            HandOutRewards();
        }

        public void HandOutRewards() {
            RoundInProgress = false;
            Bounties.Clear();
            if (Status == ZombieGameStatus.NotStarted) return;
            Player.GlobalMessage(Colors.lime + "The game has ended!");
            if (aliveCount == 0) Player.GlobalMessage(Colors.maroon + "Zombies have won this round.");
            else if (aliveCount == 1) Player.GlobalMessage(Colors.green + "Congratulations to the sole survivor:");
            else Player.GlobalMessage(Colors.green + "Congratulations to the survivors:");
            
            timer.Enabled = false;
            string playersString = "";
            Player[] online = null;
            
            if (aliveCount == 0) {
                online = PlayerInfo.Online;
                foreach (Player pl in online)
                    ResetPlayer(pl, ref playersString);
            } else {
                alive.ForEach(pl => ResetPlayer(pl, ref playersString));
            }
            
            Player.GlobalMessage(playersString);
            online = PlayerInfo.Online;
            Random rand = new Random();
            foreach (Player pl in online) {
                int money = 0;
                if (pl.level.name != currentLevelName) continue;
                bool inBlock = pl.CheckIfInsideBlock();
                
                if (!inBlock && aliveCount == 0) {
                    money = rand.Next(1, 5 + pl.playersInfected);
                } else if (!inBlock && (aliveCount == 1 && !pl.infected)) {
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
                pl.color = pl.group.color;
                if (pl.referee) {
                    pl.SendMessage("You gained one " + Server.moneys + " because you're a ref. Would you like a medal as well?");
                    pl.money++;
                }
            }
            try {alive.Clear(); infectd.Clear(); } catch{ }
        }

        void ResetPlayer(Player p, ref string playersString) {
            p.blockCount = 50;
            p.infected = false;
            p.playersInfected = 0;
            
            if (p.level.name == currentLevelName) {
                p.color = p.group.color;
                playersString += p.group.color + p.DisplayName + Colors.white + ", ";
            }
        }
        
        public void ChangeLevel()
        {
            if (queLevel)
            {
                ChangeLevel(nextLevel, Server.ZombieOnlyServer);
            }
            try
            {
                if (ChangeLevels)
                {
                    ArrayList al = new ArrayList();
                    DirectoryInfo di = new DirectoryInfo("levels/");
                    FileInfo[] fi = di.GetFiles("*.lvl");
                    foreach (FileInfo fil in fi)
                    {
                        al.Add(fil.Name.Split('.')[0]);
                    }

                    if (al.Count <= 2 && !UseLevelList) { Server.s.Log("You must have more than 2 levels to change levels in Zombie Survival"); return; }

                    if (LevelList.Count < 2 && UseLevelList) { Server.s.Log("You must have more than 2 levels in your level list to change levels in Zombie Survival"); return; }

                    string selectedLevel1 = "";
                    string selectedLevel2 = "";

                LevelChoice:
                    Random r = new Random();
                    int x = 0;
                    int x2 = 1;
                    string level = ""; string level2 = "";
                    if (!UseLevelList)
                    {
                        x = r.Next(0, al.Count);
                        x2 = r.Next(0, al.Count);
                        level = al[x].ToString();
                        level2 = al[x2].ToString();
                    }
                    else
                    {
                        x = r.Next(0, LevelList.Count());
                        x2 = r.Next(0, LevelList.Count());
                        level = LevelList[x].ToString();
                        level2 = LevelList[x2].ToString();
                    }
                    Level current = Server.mainLevel;

                    if (lastLevelVote1 == level || lastLevelVote2 == level2 || lastLevelVote1 == level2 || lastLevelVote2 == level || current == LevelInfo.Find(level) || currentZombieLevel == level || current == LevelInfo.Find(level2) || currentZombieLevel == level2)
                        goto LevelChoice;
                    else if (selectedLevel1 == "") { selectedLevel1 = level; goto LevelChoice; }
                    else
                        selectedLevel2 = level2;

                    Level1Vote = 0; Level2Vote = 0; Level3Vote = 0;
                    lastLevelVote1 = selectedLevel1; lastLevelVote2 = selectedLevel2;

                    if (Status == ZombieGameStatus.NotStarted || Status == ZombieGameStatus.LastRound)
                        return;

                    if (initialChangeLevel)
                    {
                        Server.votingforlevel = true;
                        Player.GlobalMessage(" " + Colors.black + "Level Vote: %S" + selectedLevel1 + ", " + selectedLevel2 +
                                             " or random " + "(" + Colors.lime + "1%S/" + Colors.red + "2%S/" + Colors.blue + "3%S)");
                        System.Threading.Thread.Sleep(15000);
                        Server.votingforlevel = false;
                    }
                    else { Level1Vote = 1; Level2Vote = 0; Level3Vote = 0; }

                    if (Status == ZombieGameStatus.NotStarted || Status == ZombieGameStatus.LastRound)
                        return;

                    if (Level1Vote >= Level2Vote)
                    {
                        if (Level3Vote > Level1Vote && Level3Vote > Level2Vote)
                        {
                            r = new Random();
                            int x3 = r.Next(0, al.Count);
                            ChangeLevel(al[x3].ToString(), Server.ZombieOnlyServer);
                        }
                        ChangeLevel(selectedLevel1, Server.ZombieOnlyServer);
                    }
                    else
                    {
                        if (Level3Vote > Level1Vote && Level3Vote > Level2Vote)
                        {
                            r = new Random();
                            int x4 = r.Next(0, al.Count);
                            ChangeLevel(al[x4].ToString(), Server.ZombieOnlyServer);
                        }
                        ChangeLevel(selectedLevel2, Server.ZombieOnlyServer);
                    }
                    Player[] online = PlayerInfo.Online;
                    foreach (Player winners in online) {
                        winners.voted = false;
                    }
                }
            }
            catch { }
        }
    }
}
