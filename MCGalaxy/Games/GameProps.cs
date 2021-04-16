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
        
        public NoclipDetector Noclip;
        public SpeedhackDetector Speed;
    }
}
