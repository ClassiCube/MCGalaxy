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
    
    /// <summary> Properties of players that are relevant to zombie survival. </summary>
    public class ZombieGameProps {
        
        /// <summary> Whether this player is currently in referee mode. </summary>
        public bool Referee = false;
        
        /// <summary> Remaining number of blocks that can be manually placed this round. </summary>
        internal int BlockCount = 50;
        
        /// <summary> Number of blocks that have been sequentially vertically pillared. </summary>
        public int BlocksStacked = 0;
        internal int lastYblock = 0, lastXblock = 0, lastZblock = 0;
        
        /// <summary> Whether this player is currently infected. </summary>
        public bool Infected = false;        
        
        /// <summary> Whether the real name of players are always shown over zombie models. </summary>
        public bool AkaMode = false;
        public bool flipHead = false;
        
        /// <summary> Total number of other players infected this round. </summary>
        public int NumberInfected = 0;
        
        internal string lastSpawnColor = "";
        
        /// <summary> Whether either /like or /dislike has been in this round. </summary>
        public bool RatedMap = false;
        
        /// <summary> Whether used /pledge has been used in this round. </summary>
        public bool PledgeSurvive = false;
        
        /// <summary> List of custom infect messages. </summary>
        public List<string> InfectMessages = null;
    }
}
