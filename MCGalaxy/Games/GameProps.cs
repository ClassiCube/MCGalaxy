/*
    Copyright 2015 MCGalaxy
        
    Dual-licensed under the Educational Community License, Version 2.0 and
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
using System.Collections.Generic;

namespace MCGalaxy.Games {

    public class GameProps {
        
        /// <summary> Team the player is currently in. </summary>
        public Team Team;
        
        /// <summary> Last team the player was invited to. </summary>
        public string TeamInvite; 
        
        /// <summary> Whether the player has liked or disliked the map in this round. </summary>
        internal bool RatedMap = false, LikedMap = false;
        
        /// <summary> Whether the player has pledged that they will survive/win in this round. </summary>
        internal bool PledgeSurvive = false;

        //Zombie
        public bool Referee = false;
        
        /// <summary> Remaining number of blocks the player can place this round. </summary>
        public int BlocksLeft = 50;
        
        /// <summary> Number of blocks the player has sequentially pillared up. </summary>
        internal int BlocksStacked = 0;
        internal int LastX, LastY, LastZ;
        
        /// <summary> Whether this player is currently infected/dead. </summary>
        public bool Infected = false;
        
        /// <summary> Point in time this player was infected at. </summary>
        public DateTime TimeInfected;
        
        /// <summary> Whether the real names of zombies are always shown to the player. </summary>
        public bool Aka = false;
        
        /// <summary> List of custom infect messages this player has. </summary>
        internal List<string> InfectMessages = null;
        
        /// <summary> Whether this player is currently using an invisibility potion. </summary>
        public bool Invisible;
        
        /// <summary> Point in time at which the invisibility potion expires. </summary>
        public DateTime InvisibilityEnd;
        
        /// <summary> Last 'invisible for X more seconds' time sent to the player. </summary>
        public int InvisibilityTime = -1;
        
        /// <summary> Number of invisibility potions bought this round. </summary>
        public int InvisibilityPotions;
        
        /// <summary> Number of successful revives this round. </summary>
        public int RevivesUsed;
        
        /// <summary> Last time a message was sent to ops warning about pillaring. </summary>
        public DateTime LastPillarWarn;
        
        /// <summary> Whether the player has already been fined for pillaring. </summary>
        public bool PillarFined;
        
        /// <summary> Resets all the invisibility variables back to default. </summary>
        public void ResetInvisibility() {
            Invisible = false;
            InvisibilityEnd = DateTime.MinValue;
            InvisibilityTime = -1;
        }
        
        /// <summary> Resets all the zombie game round variables back to default. </summary>
        public void ResetZombieState() {
            BlocksLeft = 50;
            CurrentInfected = 0;
            Infected = false;
            PledgeSurvive = false;
            InvisibilityPotions = 0;
            RevivesUsed = 0;
            TimeInfected = DateTime.MinValue;
        }
                
        public int TotalRoundsSurvived;
        public int MaxRoundsSurvived;
        public int CurrentRoundsSurvived;
        public int TotalInfected;
        public int MaxInfected;
        public int CurrentInfected;
        
        internal List<DateTime> NoclipLog = new List<DateTime>(5);
        internal List<DateTime> SpeedhackLog = new List<DateTime>(5);
        internal DateTime LastNoclipWarn, LastSpeedhackWarn;
    }
}
