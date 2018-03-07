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
using BlockID = System.UInt16;

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
        
        public BlockID OldBlock { 
            get { return (BlockID)(OldRaw | ((Flags & BlockDBFlags.OldExtended) >> 6) | ((Flags & BlockDBFlags.OldExtended2) >> 3)); }
        }
        public BlockID NewBlock { 
            get { return (BlockID)(NewRaw | ((Flags & BlockDBFlags.NewExtended) >> 7) | ((Flags & BlockDBFlags.NewExtended2) >> 4)); }
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]    
    public struct BlockDBCacheEntry {

        /// <summary> ID within Players table of player who made the change, and block flags </summary>
        /// <remarks> lowest 8 bits for block flags, remaining 24 bits for player id. </remarks>
        public int Packed;
        
        /// <summary> Packed coordinates of where the change occured at. </summary>
        public int Index;
        
        /// <summary> Raw block that was previously there before the change. </summary>
        public byte OldRaw;
        
        /// <summary> Raw block that is now there due to the change. </summary>
        public byte NewRaw;
        
        /// <summary> Time offset for the block change. </summary>
        public ushort TimeDelta;
    }

    public static class BlockDBFlags {
        public const ushort ManualPlace = 1 << 0;
        public const ushort Painted     = 1 << 1;
        public const ushort Drawn       = 1 << 2;
        public const ushort Replaced    = 1 << 3;
        public const ushort Pasted      = 1 << 4;
        public const ushort Cut         = 1 << 5;
        public const ushort Filled      = 1 << 6;
        public const ushort Restored    = 1 << 7;
        public const ushort UndoOther   = 1 << 8;
        public const ushort UndoSelf    = 1 << 9;
        public const ushort RedoSelf    = 1 << 10;        
        public const ushort Unused      = 1 << 11;
        
        public const ushort OldExtended2 = 1 << 12;
        public const ushort NewExtended2 = 1 << 13;        
        public const ushort OldExtended  = 1 << 14;
        public const ushort NewExtended  = 1 << 15;
    }
}
