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
	
	public enum ZombieGameStatus { NotStarted, InfiniteRounds, SingleRound, VariableRounds, LastRound }
	
    public sealed partial class ZombieGame {
		
		/// <summary> The number of rounds that have been played in this game so far. </summary>
        public int RoundsDone = 0;
        
        /// <summary> The maximum number of rounds that can be played before the game ends. </summary>
        public int MaxRounds = 0;
        
        /// <summary> How precise collision detection is between alive and dead players. (Where 1 block = 32 units) </summary>
        public int HitboxPrecision = 24;
        
        /// <summary> Current round status of the game. </summary>
        public ZombieGameStatus Status = ZombieGameStatus.NotStarted;
        
        public bool RoundInProgress = false;
        
        public int aliveCount = 0;
        public string currentZombieLevel = "";
        public static System.Timers.Timer timer;
        public bool initialChangeLevel = false;
        public string currentLevelName = "";
        public static List<Player> alive = new List<Player>();
        public static List<Player> infectd = new List<Player>();
        static string[] messages = new string[] { "{0} WIKIWOO'D {1}", "{0} stuck their teeth into {1}", 
            "{0} licked {1}'s brain ", "{0} danubed {1}", "{0} made {1} meet their maker", "{0} tripped {1}", 
            "{0} made some zombie babies with {1}", "{0} made {1} see the dark side", "{0} tweeted {1}", 
            "{0} made {1} open source", "{0} infected {1}", "{0} iDotted {1}", "{1} got nommed on",
            "{0} transplanted {1}'s living brain" };
        
        internal bool noRespawn = true, noPillaring = true;
        internal string ZombieName = "";
        internal bool queLevel = false, queZombie = false;
        internal string nextZombie = "", nextLevel = "";
        internal bool ChangeLevels = true, UseLevelList = false;
        
        internal List<string> LevelList = new List<string>();
        string lastLevelVote1 = "", lastLevelVote2 = "";

        int Level1Vote = 0, Level2Vote = 0, Level3Vote = 0;
        
        string lastPlayerToInfect = "";
        int infectCombo = 0;
        public Dictionary<string, BountyData> Bounties = new Dictionary<string, BountyData>();
        
        public void Start(ZombieGameStatus status, int amount) {
            if (UseLevelList && LevelList == null)
                ChangeLevels = false;
            Server.ZombieModeOn = true;
            Status = status;
            RoundInProgress = false;
            initialChangeLevel = false;
            MaxRounds = amount + 1;
            RoundsDone = 0;

            Thread t = new Thread(MainLoop);
            t.Name = "MCG_ZombieGame";
            t.Start();
        }

        public void InfectedPlayerDC() {
            if (Status == ZombieGameStatus.NotStarted) return;
            //This is for when the first zombie disconnects
            Random random = new Random();
            if ((Status != ZombieGameStatus.NotStarted && RoundInProgress) && infectd.Count <= 0) {
                if (alive.Count == 0) return;
                int index = random.Next(alive.Count);
                
                while (alive[index].referee || alive[index].level.name == Server.zombie.currentLevelName) {
                    if (index >= alive.Count - 1) index = 0;
                    else index++;
                }
                
                Player zombie = alive[index];
                Player.GlobalMessage(zombie.FullName + " %Scontinued the infection!");
                InfectPlayer(zombie);
            }
        }

        public bool InfectedPlayerLogin(Player p)  {
            if (Status == ZombieGameStatus.NotStarted || p == null) return false;
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

        public void InfectPlayer(Player p)
        {
            if (!RoundInProgress || p == null) return;
            infectd.Add(p);
            alive.Remove(p);
            p.infected = true;
            UpdatePlayerColor(p, Colors.red);
            aliveCount = alive.Count;
        }

        public void DisinfectPlayer(Player p)
        {
            if (!RoundInProgress || p == null) return;
            infectd.Remove(p);
            alive.Add(p);
            p.infected = false;
            UpdatePlayerColor(p, p.group.color);
            aliveCount = alive.Count;
        }

        public void ChangeLevel(string LevelName, bool changeMainLevel)
        {
            string next = LevelName;
            currentLevelName = next;
            queLevel = false;
            nextLevel = "";
            Command.all.Find("load").Use(null, next.ToLower() + " 0");
            Player.GlobalMessage("The next map has been chosen - " + Colors.red + next.ToLower());
            Player.GlobalMessage("Please wait while you are transfered.");
            string oldLevel = Server.mainLevel.name;
            if (changeMainLevel) {
                Server.mainLevel = LevelInfo.Find(next.ToLower());
                Player[] online = PlayerInfo.Online.Items; 
                foreach (Player player in online) {
                    if (player.level.name != next && player.level.name == currentLevelName)
                    {
                        player.SendMessage("Going to the next map!");
                        Command.all.Find("goto").Use(player, next);
                    }
                }
                Command.all.Find("unload").Use(null, oldLevel);
            } else {
                Player.GlobalMessage("Type /goto " + next + " to play the next round of Zombie Survival");
            }
            return;
        }

        public bool IsInZombieGameLevel(Player p) {
            return p.level.name == currentLevelName;
        }

        public void ResetState() {
            Status = ZombieGameStatus.NotStarted;
            MaxRounds = 0; 
            initialChangeLevel = false; 
            Server.ZombieModeOn = false; 
            RoundInProgress = false;
        }
    }
}
