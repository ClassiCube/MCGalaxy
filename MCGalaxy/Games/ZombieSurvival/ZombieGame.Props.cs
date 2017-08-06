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
using MCGalaxy.Config;

namespace MCGalaxy.Games {
    
    public class BountyData {
        public string Origin, Target;
        public int Amount;
        
        public BountyData(string origin, string target, int amount) {
            Origin = origin; Target = target; Amount = amount;
        }
    }
    
    public enum ZombieGameStatus { NotStarted, InfiniteRounds, SingleRound, VariableRounds, LastRound }
    
    public sealed partial class ZSGame {
        
        public const string InfectCol = "&infect";
        
        /// <summary> The number of rounds that have been played in this game so far. </summary>
        public int RoundsDone = 0;
        
        /// <summary> The maximum number of rounds that can be played before the game ends. </summary>
        public int MaxRounds = 0;
        
        /// <summary> Current round status of the game. </summary>
        public ZombieGameStatus Status = ZombieGameStatus.NotStarted;
        
        /// <summary> Gets whether zombie survival is currently running. </summary>
        public override bool Running { get { return Status != ZombieGameStatus.NotStarted; } }
        
        /// <summary> Whether a round is currently in progress. </summary>
        public bool RoundInProgress = false;
        
        /// <summary> Time at which the next round is scheduled to start. </summary>
        public DateTime RoundStart;
        
        /// <summary> Time at which the next round is scheduled to end. </summary>
        public DateTime RoundEnd;
        
        /// <summary> The name of the level that the last round of zombie survival was played on. </summary>
        public string LastLevelName = "";
        
        /// <summary> List of alive/human players. </summary>
        public VolatileArray<Player> Alive = new VolatileArray<Player>();
        
        /// <summary> List of dead/infected players. </summary>
        public VolatileArray<Player> Infected = new VolatileArray<Player>();
        
        /// <summary> Name of the player queued to be the first zombie in the next round. </summary>
        public string QueuedZombie;
        
        List<string> infectMessages = new List<string>();

        string lastPlayerToInfect = "";
        int infectCombo = 0;      
        
        /// <summary> List of players who have a bounty on them. </summary>
        public VolatileArray<BountyData> Bounties = new VolatileArray<BountyData>();
    }
}
