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
using System.Threading;
using MCGalaxy.Config;

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
        [ConfigInt("zombie-hitbox-precision", "Zombie", null, 32)]
        public static int HitboxPrecision = 32;
        
        /// <summary> The maximum distance a player is allowed to move between movement packets. </summary>
        [ConfigInt("zombie-maxmove-distance", "Zombie", null, 70)]
        public static int MaxMoveDistance = 70;
        
        /// <summary> Current round status of the game. </summary>
        public ZombieGameStatus Status = ZombieGameStatus.NotStarted;
        
        /// <summary> Gets whether zombie survival is currently running. </summary>
        public bool Running { get { return Status != ZombieGameStatus.NotStarted; } }
        
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
        
        /// <summary> Whether the server's main level should be set to the current level at the end of each round. </summary>
        [ConfigBool("zombie-survival-only-server", "Zombie", null, false)]
        public static bool SetMainLevel;
        
        /// <summary> Whether zombie survival should start upon server startup. </summary>
        [ConfigBool("zombie-on-server-start", "Zombie", null, false)]
        public static bool StartImmediately;
        
        /// <summary> Whether changes made during a round of zombie survival should be permanently saved. </summary>
        [ConfigBool("zombie-save-blockchanges", "Zombie", null, false)]
        public static bool SaveLevelBlockchanges;
        
        /// <summary> Whether maps with '+' in their name are ignored when choosing levels for the next round. </summary>
        [ConfigBool("zombie-ignore-personalworlds", "Zombie", null, true)]
        public static bool IgnorePersonalWorlds = true;
        
        /// <summary> Whether the current level name should be shown in the heartbeats sent. </summary>
        [ConfigBool("zombie-map-inheartbeat", "Zombie", null, false)]
        public static bool IncludeMapInHeartbeat = false;
        
        static string[] messages = new string[] { "{0} WIKIWOO'D {1}", "{0} stuck their teeth into {1}",
            "{0} licked {1}'s brain ", "{0} danubed {1}", "{0} made {1} meet their maker", "{0} tripped {1}",
            "{0} made some zombie babies with {1}", "{0} made {1} see the dark side", "{0} tweeted {1}",
            "{0} made {1} open source", "{0} infected {1}", "{0} iDotted {1}", "{1} got nommed on",
            "{0} transplanted {1}'s living brain" };

        [ConfigBool("no-respawning-during-zombie", "Zombie", null, true)]
        internal static bool noRespawn = true;
        [ConfigBool("no-pillaring-during-zombie", "Zombie", null, true)]
        internal static bool noPillaring = true;
        [ConfigString("zombie-name-while-infected", "Zombie", null, "", true)]
        internal static string ZombieName = "";
        [ConfigString("zombie-model-while-infected", "Zombie", null, "zombie")]
        internal static string ZombieModel = "zombie";
        [ConfigBool("enable-changing-levels", "Zombie", null, true)]
        internal static bool ChangeLevels = true;
        
        /// <summary> List of levels that are randomly picked for zombie survival. 
        /// If this left blank, then all level files are picked from instead. </summary>
        [ConfigStringList("zombie-levels-list", "Zombie", null)]
        internal static List<string> LevelList = new List<string>();
        
        /// <summary> List of levels that are never picked for zombie survival. </summary>
        [ConfigStringList("zombie-ignores-list", "Zombie", null)]
        internal static List<string> IgnoredLevelList = new List<string>();
        
        string lastLevel1 = "", lastLevel2 = "";
        int Level1Vote = 0, Level2Vote = 0, Level3Vote = 0;
        
        string lastPlayerToInfect = "";
        int infectCombo = 0;
        public Dictionary<string, BountyData> Bounties = new Dictionary<string, BountyData>();
    }
}
