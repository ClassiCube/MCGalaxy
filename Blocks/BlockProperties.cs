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

namespace MCGalaxy.Blocks {
    
    public struct BlockProperties {
        
        /// <summary> ID of block these properties are associated with. </summary>
        public byte BlockId;
        
        /// <summary> Standard block id sent to clients in map and block update packets. </summary>
        public byte ConvertId;
        
        /// <summary> Block id converted to when the map is saved to a .lvl file. </summary>
        public byte SaveConvertId;
        
        /// <summary> Block name used for in commands. </summary>
        public string Name;
        
        /// <summary> Message shown to the level when the player is killed by this block. Can be null. </summary>
        public string DeathMessage;
        
        /// <summary> Whether colliding with this block kills the player. </summary>
        public bool CollisionDeath;
        
        /// <summary> Whether this block is considered a tdoor. </summary>
        public bool IsTDoor;
        /// <summary> Block id this block is converted to when toggled by a neighbouring door. </summary>
        public byte ODoorId;
        public byte DoorId;
        public byte DoorAirId;
        
        /// <summary> Whether this block is considered a message block. </summary>
        public bool IsMessageBlock;
        /// <summary> Whether this block is considered a portal. </summary>
        public bool IsPortal;
        /// <summary> Whether this block is overwritten/killed by water blocks. </summary>
        public bool WaterKills;
        /// <summary> Whether this block is overwritten/killed by lava blocks. </summary>
        public bool LavaKills;
        
        /// <summary> Whether walkinhg through this block causes the death of that player. </summary>
        public bool CausesDeath;
        
        /// <summary> Whether this block is an OP block (cannot be replaced by physics changes). </summary>
        public bool OPBlock;
        
        /// <summary> Whether this block should allow trains to go over them. </summary>
        public bool IsRails;
        
        public BlockProperties(byte type) {
            this = default(BlockProperties);
            BlockId = type;
            ConvertId = type;
            SaveConvertId = type;
            Name = "unknown";
            ODoorId = Block.Zero;
        }
    }
}
