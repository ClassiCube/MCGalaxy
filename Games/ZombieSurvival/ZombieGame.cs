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
	
	public class BountyData {
		public Player Origin;
		public int Amount;
		
		public BountyData(Player origin, int amount) {
			Origin = origin; Amount = amount;
		}
	}
	
    public sealed partial class ZombieGame
    {
        public int amountOfRounds = 0;
        public int limitRounds = 0;
        public int aliveCount = 0;
        public string currentZombieLevel = "";
        public static System.Timers.Timer timer;
        public bool initialChangeLevel = false;
        public string currentLevelName = "";
        public static List<Player> alive = new List<Player>();
        public static List<Player> infectd = new List<Player>();
        string[] infectMessages = new string[] { " WIKIWOO'D ", " stuck their teeth into ", " licked ", " danubed ", " made ", " tripped ", " made some zombie babies with ", " made ", " tweeted ", " made ", " infected ", " iDotted ", "", "transplanted " };
        string[] infectMessages2 = new string[] { "", "", "'s brain", "", " meet their maker", "", "", " see the dark side", "", " open source", "", "", " got nommed on", "'s living brain" };

        internal bool noRespawn = true, noLevelSaving = true, noPillaring = true;
        internal string ZombieName = "";
        internal int gameStatus = 0; //0 = not started, 1 = always on, 2 = one time, 3 = certain amount of rounds, 4 = stop game next round
        internal bool queLevel = false, queZombie = false;
        internal string nextZombie = "", nextLevel = "";
        internal bool zombieRound = false;
        internal bool ChangeLevels = true, UseLevelList = false;
        
        internal List<string> LevelList = new List<string>();
        string lastLevelVote1 = "", lastLevelVote2 = "";

        int Level1Vote = 0, Level2Vote = 0, Level3Vote = 0;
        
        string lastPlayerToInfect = "";
        int infectCombo = 0;
        public Dictionary<string, BountyData> Bounties = new Dictionary<string, BountyData>();
        
        public void StartGame(int status, int amount)
        {
            //status: 0 = not started, 1 = always on, 2 = one time, 3 = certain amount of rounds, 4 = stop round next round

            if (status == 0) return;

            //SET ALL THE VARIABLES!
            if (UseLevelList && LevelList == null)
                ChangeLevels = false;
            Server.ZombieModeOn = true;
            gameStatus = status;
            zombieRound = false;
            initialChangeLevel = false;
            limitRounds = amount + 1;
            amountOfRounds = 0;
            //SET ALL THE VARIABLES?!?

             //Start the main Zombie thread
             Thread t = new Thread(MainLoop);
             t.Name = "MCG_ZombieGame";
             t.Start();
        }

        public void InfectedPlayerDC()
        {
            if (gameStatus == 0) return;
            //This is for when the first zombie disconnects
            Random random = new Random();
            if ((gameStatus != 0 && zombieRound) && infectd.Count <= 0)
            {
                int firstinfect = random.Next(alive.Count);
                firstinfect = firstinfect - 1;
                while (alive[firstinfect].referee || alive[firstinfect].level.name == Server.zombie.currentLevelName)
                {
                    if (firstinfect == alive.Count)
                    {
                        firstinfect = 0;
                    }
                    else
                    {
                        firstinfect++;
                    }
                }
                Player.GlobalMessage(alive[firstinfect].color + alive[firstinfect].name + Server.DefaultColor + " continued the infection!");
                alive[firstinfect].color = Colors.red;
                Player.GlobalDespawn(alive[firstinfect], false);
                Player.GlobalSpawn(alive[firstinfect], alive[firstinfect].pos[0], alive[firstinfect].pos[1], alive[firstinfect].pos[2], alive[firstinfect].rot[0], alive[firstinfect].rot[1], false);
                infectd.Add(alive[firstinfect]);
                alive.Remove(alive[firstinfect]);
            }
            return;
        }

        public bool InfectedPlayerLogin(Player p)
        {
            if (gameStatus == 0) return false;
            if (p == null) return false;
            if (p.level.name != Server.zombie.currentLevelName) return false;
            p.SendMessage("You have joined in the middle of a round. You are now infected!");
            p.blockCount = 50;
            try
            {
                Server.zombie.InfectPlayer(p);
            }
            catch { }
            return true;
        }

        public int ZombieStatus()
        {
            return gameStatus;
        }

        public bool GameInProgess()
        {
            return zombieRound;
        }

        public void InfectPlayer(Player p)
        {
            if (!zombieRound || p == null) return;
            infectd.Add(p);
            alive.Remove(p);
            p.infected = true;
            p.color = Colors.red;
            Player.GlobalDespawn(p, false);
            Player.GlobalSpawn(p, p.pos[0], p.pos[1], p.pos[2], p.rot[0], p.rot[1], false);
            aliveCount = alive.Count;
        }

        public void DisinfectPlayer(Player p)
        {
            if (!zombieRound || p == null) return;
            infectd.Remove(p);
            alive.Add(p);
            p.infected = false;
            p.color = p.group.color;
            Player.GlobalDespawn(p, false);
            Player.GlobalSpawn(p, p.pos[0], p.pos[1], p.pos[2], p.rot[0], p.rot[1], false);
            aliveCount = alive.Count;
        }

        public void ChangeLevel(string LevelName, bool changeMainLevel)
        {
            String next = LevelName;
            currentLevelName = next;
            queLevel = false;
            nextLevel = "";
            Command.all.Find("load").Use(null, next.ToLower() + " 0");
            Player.GlobalMessage("The next map has been chosen - " + Colors.red + next.ToLower());
            Player.GlobalMessage("Please wait while you are transfered.");
            String oldLevel = Server.mainLevel.name;
            if (changeMainLevel)
            {
                Server.mainLevel = LevelInfo.Find(next.ToLower());
                Player[] online = PlayerInfo.Online; 
                foreach (Player player in online) {
                    if (player.level.name != next && player.level.name == currentLevelName)
                    {
                        player.SendMessage("Going to the next map!");
                        Command.all.Find("goto").Use(player, next);
                    }
                }
                Command.all.Find("unload").Use(null, oldLevel);
            }
            else
            {
                Player.GlobalMessage("Type /goto " + next + " to play the next round of Zombie Survival");
            }
            return;
        }

        public bool IsInZombieGameLevel(Player p) {
            return p.level.name == currentLevelName;
        }

        public void ResetState() {
            gameStatus = 0; 
            limitRounds = 0; 
            initialChangeLevel = false; 
            Server.ZombieModeOn = false; 
            zombieRound = false;
        }
    }
}
