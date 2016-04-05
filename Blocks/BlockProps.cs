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
	
	public struct BlockProps {
		
		/// <summary> Standard block id sent to clients in map and block update packets. </summary>
		public byte ConvertId;
		
		/// <summary> Block id converted to when the map is saved to a .lvl file. </summary>
		public byte SaveConvertId;
		
		/// <summary> Block name used for in commands. </summary>
		public string Name;
		
		/// <summary> Whether this block is considered a tdoor. </summary>
		public bool IsTDoor;
		/// <summary> Whether this block is considered a message block. </summary>
		public bool IsMessageBlock;
		/// <summary> Whether this block is considered a portal. </summary>
		public bool IsPortal;
		
		/// <summary> Whether walkinhg through this block causes the death of that player. </summary>
		public bool CausesDeath;
		
		/// <summary> Whether light passes through this block. </summary>
		public bool LightPasses;
		
		/// <summary> Whether this block is an OP block (cannot be replaced by physics changes). </summary>
		public bool OPBlock;
		
		public BlockProps(byte type) {
			this = default(BlockProps);
			ConvertId = type;
			SaveConvertId = type;
			Name = "unknown";
		}
	}
}
