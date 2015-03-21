/*
	Copyright 2012 Snowl (David Diaz) for use with MCForge
		
	Dual-licensed under the	Educational Community License, Version 2.0 and
	the GNU General Public License, Version 3 (the "Licenses"); you may
	not use this file except in compliance with the Licenses. You may
	obtain a copy of the Licenses at
	
	http://www.opensource.org/licenses/ecl2.php
	http://www.gnu.org/licenses/gpl-3.0.html
	
	Unless required by applicable law or agreed to in writing,
	software distributed under the Licenses are distributed on an "AS IS"
	BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express
	or implied. See the Licenses for the specific language governing
	permissions and limitations under the Licenses.
*/
using System;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using System.Threading;
using System.Timers;
using System.Text;
using System.Linq;

namespace MCForge
{
    public class ZombieSurvival
    {
        public bool StartOnStartup;
        public static Random Random;
        public static System.Timers.Timer StartTimer;
        public static System.Timers.Timer EndTimer;
        public static System.Timers.Timer EndRoundTimer;
        public static Level ZombieLevel;
        public static int ElapsedRounds;
        public bool MoreThanTwoPlayers = true;

        // Constructors
        public ZombieSurvival()
        {
            Random = new Random();
            StartOnStartup = true;
            int randomMinutes = Random.Next(1, 2);
        }

        public void Start(int x)
        {
            Server.ZombieMode = x;
            Start();
        }

        public void Start()
        {
            StartTimer = new System.Timers.Timer(1000);
            ZombieLevel = Level.Find("main");
            int loop = 6;
            StartTimer.Start(); StartTimer.Elapsed += delegate
            {
                Server.s.Log(loop + "");
                if (loop != 0)
                {
                    int amountOfPlayers = PlayerCount();
                    Player.players.ForEach(delegate(Player p)
                    {
                        if (p.referee)
                            amountOfPlayers--;
                    });
                    //if (amountOfPlayers >= 2)
                    //{
                        MoreThanTwoPlayers = true;
                        loop--;
                        Player.GlobalMessage(c.gray + " - " + Server.DefaultColor + "Zombie Survival starts in " + loop + " seconds on level " +
                                             ZombieLevel.name + c.gray + " - ");
                    /*}
                    else
                    {
                        if (MoreThanTwoPlayers)
                            Player.GlobalMessage(c.gray + " - " + Server.DefaultColor + "Zombie Survival requires more than 2 non-referee players online to play" + c.gray + " - ");
                        MoreThanTwoPlayers = false;
                    }*/
                }
                else
                {
                    loop = 6;
                    Server.ZombieRound = true;
                    int randomMinutes = Random.Next(1, 4);
                    Player.GlobalMessage(c.gray + " - " + "Zombie Survival has started on level " + ZombieLevel.name + " for " + randomMinutes + " minutes! Type /g " + ZombieLevel.name + " to join! " + c.gray + " - ");
                    EndRoundTimer = new System.Timers.Timer((60000 * randomMinutes) - 30000);
                    EndRoundTimer.Start();
                    EndRoundTimer.Elapsed += delegate { EndRound(); };
                    StartTimer.Stop();
                    StartTimer.Dispose();
                }
            };
        }

        public void EndRound()
        {
            EndRoundTimer.Stop();
            EndRoundTimer.Dispose();
            EndTimer = new System.Timers.Timer(30000);
            int loop = 2;
            EndTimer.Start(); EndTimer.Elapsed += delegate
            {
                if (loop == 2)
                {
                    loop--;
                    Player.GlobalMessage(c.gray + " - " + Server.DefaultColor + "Zombie Survival ends in 30 seconds" + 
                                            c.gray + " - ");
                }
                else if (loop == 1)
                {
                    loop = 2;
                    ElapsedRounds++;
                    Server.ZombieRound = false;
                    Player.GlobalMessage(c.gray + " - " + Server.DefaultColor + "Zombie Survival has ended! " + c.gray + " - ");
                    //DO VOTING
                    ChooseLevel();
                    EndTimer.Stop();
                    EndTimer.Dispose();
                    Start();
                }
            };
        }

        public void ChooseLevel()
        {
            ArrayList al = new ArrayList();
            DirectoryInfo di = new DirectoryInfo("levels/");
            FileInfo[] fi = di.GetFiles("*.lvl");
            foreach (FileInfo fil in fi)
            {
                al.Add(fil.Name.Split('.')[0]);
            }
            if (al.Count <= 2) { Server.s.Log("You must have more than 2 levels to choose levels in Zombie Survival! Choosing last level!"); if (ZombieLevel == null) ZombieLevel = Level.Find("main"); return; }
            int x = 0; string level = "";
            x = Random.Next(0, al.Count);
            level = al[x].ToString();
            ZombieLevel = Level.Find(level);
        }

        public void CheckLocation(Player p)
        {
            if (!Server.ZombieRound) return;
            if (p.level != ZombieLevel || p.referee) return;
            Player.players.ForEach(delegate(Player p2)
            {
                if (p2.level != ZombieLevel || p2.referee) ;
                else
                {
                    if (p2.pos[0] / 32 == p.pos[0] / 32 || p2.pos[0] / 32 == p.pos[0] / 32 + 1 ||
                        p2.pos[0] / 32 == p.pos[0] / 32 - 1 && (p.infected != p2.infected))
                    {
                        if (p2.pos[1] / 32 == p.pos[1] / 32 || p2.pos[1] / 32 == p.pos[1] / 32 - 1 ||
                            p2.pos[1] / 32 == p.pos[1] / 32 + 1)
                        {
                            if (p2.pos[2] / 32 == p.pos[2] / 32 || p2.pos[2] / 32 == p.pos[2] / 32 + 1 ||
                                p2.pos[2] / 32 == p.pos[2] / 32 - 1)
                            {

                            }
                        }
                    }
                }
            });
        }

        public void InfectPlayer(Player p, Player p2, string message1, string message2, string message3 = "", bool announce = true)
        {
            p.infected = false;
            if (announce)
            {
                if (p2 != null)
                    Player.GlobalMessage(c.gray + " - " + Server.DefaultColor + message1 + " " + p.name + " " + message2 +
                                         " " + p2 + " " + message3 + c.gray + " - ");
                else
                    Player.GlobalMessage(c.gray + " - " + Server.DefaultColor + message1 + " " + p.name + " " + message2 +
                                         c.gray + " - ");
            }
            CheckEndRound();
        }

        public bool CheckEndRound()
        {
            int uninfectedPlayers = 0;
            Player.players.ForEach(delegate(Player p2)
            {
                if (!p2.infected)
                {
                    uninfectedPlayers++;
                }
            });
            if (uninfectedPlayers == 0 || PlayerCount() < 2)
            {
                EndRound();
                return true;
            }
            return false;
        }

        public void DisconnectedPlayer(Player p)
        {
            if (CheckEndRound())
            {
                return;
            }
            int infectedPlayers = 0;
            Player.players.ForEach(delegate(Player p2)
            {
                if (p2.infected)
                {
                    infectedPlayers++;
                }
            });
            if (infectedPlayers == 0)
            {
                int randomPlayer = Random.Next(0, PlayerCount() - 1);
                Player player = Player.players[randomPlayer];
                while (player.infected)
                {
                    randomPlayer = Random.Next(0, PlayerCount() - 1);
                    player = Player.players[randomPlayer];
                }
                InfectPlayer(p, null, "" , "got infected because the zombie disconnected!");
            }
        }

        public int PlayerCount()
        {
            return ZombieLevel.getPlayers().Count;
        }

    }
}