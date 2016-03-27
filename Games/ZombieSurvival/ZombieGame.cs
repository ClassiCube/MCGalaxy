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
    
    public class BountyData {
        public Player Origin;
        public int Amount;
        
        public BountyData(Player origin, int amount) {
            Origin = origin; Amount = amount;
        }
    }
    
    public enum ZombieGameStatus { NotStarted, InfiniteRounds, SingleRound, VariableRounds, LastRound }
    
    public sealed partial class ZombieGame {
    	
    	public const string InfectCol = "&infect";
        
        /// <summary> The number of rounds that have been played in this game so far. </summary>
        public int RoundsDone = 0;
        
        /// <summary> The maximum number of rounds that can be played before the game ends. </summary>
        public int MaxRounds = 0;
        
        /// <summary> How precise collision detection is between alive and dead players. (Where 1 block = 32 units) </summary>
        public int HitboxPrecision = 24;
        
        /// <summary> Current round status of the game. </summary>
        public ZombieGameStatus Status = ZombieGameStatus.NotStarted;
        
        /// <summary> Whether a round is currently in progress. </summary>
        public bool RoundInProgress = false;
        
        /// <summary> Time at which the next round is scheduled to start. </summary>
        public DateTime RoundStart;
        
        /// <summary> Time at which the next round is scheduled to end. </summary>
        public DateTime RoundEnd;
        
        public static System.Timers.Timer timer;
        public bool initialChangeLevel = false;
        
        /// <summary> The name of the level that the last round of zombie survival was played on. </summary>
        public string LastLevelName = "";
        
        /// <summary> The name of the level that the current round of zombie survival is being played on. </summary>
        public string CurLevelName = "";
        
        /// <summary> The level that the current round of zombie survival is being played on. </summary>
        public Level CurLevel = null;
        
        /// <summary> List of alive/human players. </summary>
        public VolatileArray<Player> Alive = new VolatileArray<Player>(false);
        
        /// <summary> List of dead/infected players. </summary>
        public VolatileArray<Player> Infected = new VolatileArray<Player>(false);
        
        /// <summary> Name of the player queued to be the first zombie in the next round. </summary>
        public string QueuedZombie;
        
        /// <summary> Name of the level queued to be used for the next round. </summary>
        public string QueuedLevel;
        
        static string[] messages = new string[] { "{0} WIKIWOO'D {1}", "{0} stuck their teeth into {1}",
            "{0} licked {1}'s brain ", "{0} danubed {1}", "{0} made {1} meet their maker", "{0} tripped {1}",
            "{0} made some zombie babies with {1}", "{0} made {1} see the dark side", "{0} tweeted {1}",
            "{0} made {1} open source", "{0} infected {1}", "{0} iDotted {1}", "{1} got nommed on",
            "{0} transplanted {1}'s living brain" };
        
        internal bool noRespawn = true, noPillaring = true;
        internal string ZombieName = "";
        internal bool ChangeLevels = true, UseLevelList = false;
        
        internal List<string> LevelList = new List<string>();
        string lastLevel1 = "", lastLevel2 = "";

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

        /// <summary> If there are no infected players left, randomly selected one of the alive players to continue the infection. </summary>
        public void AssignFirstZombie() {
            if (Status == ZombieGameStatus.NotStarted || !RoundInProgress || Infected.Count > 0) return;
            Random random = new Random();
            Player[] alive = Alive.Items;
            if (alive.Length == 0) return;
            int index = random.Next(alive.Length);
            
            while (alive[index].referee || !alive[index].level.name.CaselessEq(CurLevelName)) {
                if (index >= alive.Length - 1) {
                    index = 0;
                    alive = Alive.Items;
                    if (alive.Length == 0) return;
                } else {
                    index++;
                }
            }
            
            Player zombie = alive[index];
            CurLevel.ChatLevel(zombie.FullName + " %Scontinued the infection!");
            InfectPlayer(zombie);
        }

        public void InfectPlayer(Player p) {
            if (!RoundInProgress || p == null) return;
            Infected.Add(p);
            Alive.Remove(p);
            
            p.infected = true;
            UpdatePlayerColor(p, Colors.red);
            UpdateAllPlayerStatus();
            PlayerMoneyChanged(p);
        }

        public void DisinfectPlayer(Player p) {
            if (!RoundInProgress || p == null) return;
            Infected.Remove(p);
            Alive.Add(p);
            
            p.infected = false;
            UpdatePlayerColor(p, p.color);
            UpdateAllPlayerStatus();
            PlayerMoneyChanged(p);
        }

        void ChangeLevel(string next) {
            Player[] online = PlayerInfo.Online.Items;
            if (CurLevel != null) {
                bool saveSettings = false;
                foreach (Player pl in online)
                    saveSettings |= pl.ratedMap;
                if (saveSettings) Level.SaveSettings(CurLevel);
                
                CurLevel.ChatLevel("The next map has been chosen - " + Colors.red + next.ToLower());
                CurLevel.ChatLevel("Please wait while you are transfered.");
            }
            
            CurLevelName = next;
            QueuedLevel = null;
            Command.all.Find("load").Use(null, next.ToLower() + " 0");
            CurLevel = LevelInfo.Find(next);
            if (Server.ZombieOnlyServer)
                Server.mainLevel = CurLevel;
            
            online = PlayerInfo.Online.Items;
            foreach (Player pl in online) {
                pl.ratedMap = false;
                pl.pledgeSurvive = false;
                if (!pl.level.name.CaselessEq(next) && pl.level.name.CaselessEq(LastLevelName)) {
                    pl.SendMessage("Going to the next map - &a" + next);
                    Command.all.Find("goto").Use(pl, next);
                }
            }
            if (LastLevelName != "")
                Command.all.Find("unload").Use(null, LastLevelName);
            LastLevelName = next;
        }

        public void ResetState() {
            Status = ZombieGameStatus.NotStarted;
            MaxRounds = 0;
            initialChangeLevel = false;
            Server.ZombieModeOn = false;
            RoundInProgress = false;
            RoundStart = DateTime.MinValue;
            RoundEnd = DateTime.MinValue;
            LastLevelName = "";
            CurLevelName = "";
            CurLevel = null;
            
            Player[] online = PlayerInfo.Online.Items;
            foreach (Player pl in online) {
                pl.ratedMap = false;
                pl.pledgeSurvive = false;
            }
        }
        
        void UpdatePlayerStatus(Player p) {
            int seconds = (int)(RoundEnd - DateTime.UtcNow).TotalSeconds;
            string status = GetStatusMessage(GetTimespan(seconds));
            p.SendCpeMessage(CpeMessageType.Status1, status, true);
        }
        
        internal void UpdateAllPlayerStatus() {
            int seconds = (int)(RoundEnd - DateTime.UtcNow).TotalSeconds;
            UpdateAllPlayerStatus(GetTimespan(seconds));
        }
        
        internal void UpdateAllPlayerStatus(string timespan) {
            string message = GetStatusMessage(timespan);
            Player[] players = Alive.Items;
            foreach (Player p in players)
                p.SendCpeMessage(CpeMessageType.Status1, message, true);
            players = Infected.Items;
            foreach (Player p in players)
                p.SendCpeMessage(CpeMessageType.Status1, message, true);
        }

        string GetStatusMessage(string timespan) {
            if (timespan.Length > 0) {
                const string format = "&a{0} %Salive, &c{1} %Sinfected ({2})";                
                return String.Format(format, Alive.Count, Infected.Count, timespan);
            } else {
                const string format = "&a{0} %Salive, &c{1} %Sinfected";                
                return String.Format(format, Alive.Count, Infected.Count);
            }
        }

        string GetTimespan(int seconds) {
            if (seconds < 0) return "";           
            if (seconds <= 10) return "10 secs left";
            if (seconds <= 30) return "30 secs left";
            if (seconds <= 60) return "1 min left";
            return ((seconds + 59) / 60) + " mins left";
        }
    }
}
