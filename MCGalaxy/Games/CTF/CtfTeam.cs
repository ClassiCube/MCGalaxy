/*
    Copyright 2011 MCForge
    
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
using MCGalaxy.Maths;
using BlockID = System.UInt16;

namespace MCGalaxy.Games {
    
    /// <summary> Represents a team in Capture the Flag. </summary>
    public sealed class CtfTeam2 {
        
        /// <summary> The name of this team. </summary>
        public string Name;
        
        /// <summary> The color code of this team. </summary>
        public string Color;
        
        public string ColoredName { get { return Color + Name; } }
        
        /// <summary> Total points this team has collected. </summary>
        public int Points;
        
        /// <summary> Players on this team. </summary>
        public VolatileArray<Player> Members = new VolatileArray<Player>();
        
        
        /// <summary> Position in the world the flag is located at. </summary>
        public Vec3U16 FlagPos;
        
        /// <summary> Position in the world members of this team spawn at. </summary>
        public Position SpawnPos;
        
        /// <summary> Block type of this team's flag. </summary>
        public BlockID FlagBlock;
                
        public CtfTeam2(string name, string color) { Name = name; Color = color; }

        
        /// <summary> Removes a player from this team. </summary>
        public bool Remove(Player p) { return Members.Remove(p); }
    }
}