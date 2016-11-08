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
using System.Runtime.InteropServices;

namespace MCGalaxy.DB {

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BlockDBEntry {
        /// <summary> ID within Players table of player who made the change. </summary>
        public int PlayerID;
        
        /// <summary> Seconds since BlockDB.Epoch that this change occured at. </summary>
        public int TimeDelta;
        
        /// <summary> Packed coordinates of where the change occured at. </summary>
        public int Index;
        
        /// <summary> Raw block that was previously there before the change. </summary>
        public byte OldRaw;
        
        /// <summary> Raw block that is now there due to the change. </summary>
        public byte NewRaw;
        
        /// <summary> Flags for the block change. </summary>
        public ushort Flags;
    }

    public static class BlockDBFlags {
    	public const ushort ManualPlace = 0x0001;
    	public const ushort Painted     = 0x0002;
    	public const ushort Drawn       = 0x0004;
    	public const ushort Replaced    = 0x0008;
    	public const ushort Pasted      = 0x0010;
    	public const ushort Cut         = 0x0020;
    	public const ushort Filled      = 0x0040;
    	public const ushort Restored    = 0x0080;
    	public const ushort UndoOther   = 0x0100;
    	public const ushort UndoSelf    = 0x0200;
    	public const ushort RedoSelf    = 0x0400;
    	public const ushort Unused1     = 0x0800;
    	public const ushort Unused2     = 0x1000;
    	public const ushort Unused3     = 0x2000;
    	public const ushort OldCustom   = 0x4000;
    	public const ushort NewCustom   = 0x8000;
    }
}
