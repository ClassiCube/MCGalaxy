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

namespace MCGalaxy {
    
    /// <summary> Describes an extended block. (Meaning either a core, physics, or custom block) </summary>
    public struct ExtBlock : IEquatable<ExtBlock> {
        public byte BlockID, ExtID;

        /// <summary> Constructs an extended block. </summary>
        public ExtBlock(byte block, byte extBlock) {
            BlockID = block; ExtID = extBlock;
        }
        
        public static ExtBlock Air = new ExtBlock(Block.air, 0);
        public static ExtBlock Invalid = new ExtBlock(Block.Invalid, 0);
        
        
        /// <summary> Returns whether the type of this extended block is a physics block. </summary>
        public bool IsPhysicsType {
            get { return BlockID >= Block.CpeCount && BlockID != Block.custom_block; } 
        }
        
        /// <summary> Returns whether the type of this extended block is an invalid block. </summary>
        public bool IsInvalidType { get { return BlockID == Block.Invalid; } }
        
        
        /// <summary> Returns the raw (for client side) block ID of this block. </summary>
        public byte RawID {
            get { return BlockID == Block.custom_block ? ExtID : BlockID; }
        }
        
        public static ExtBlock FromRaw(byte raw) {
            if (raw < Block.CpeCount) return (ExtBlock)raw;
            return new ExtBlock(Block.custom_block, raw);
        }
        
        public static ExtBlock FromRaw(byte raw, bool customBit) {
            if (!customBit) return (ExtBlock)raw;
            return new ExtBlock(Block.custom_block, raw);
        }
        
        /// <summary> Constructs an extended block. </summary>
        public static explicit operator ExtBlock(byte block) { return new ExtBlock(block, 0); }
        
        
        public override bool Equals(object obj) {
            return (obj is ExtBlock) && Equals((ExtBlock)obj);
        }
        
        public override int GetHashCode() {
            return BlockID | (ExtID << 8);
        }

        /// <summary> Returns whether the two extended blocks visually appear the same to clients. </summary>
        public bool VisuallyEquals(ExtBlock other) {
            return (BlockID == Block.custom_block && other.BlockID == Block.custom_block) 
                ? ExtID == other.ExtID : Block.Convert(BlockID) == Block.Convert(other.BlockID);
        }        
        
        /// <summary> Returns whether the two extended blocks are the same extended block. </summary>
        public bool Equals(ExtBlock other) {
            return (BlockID == Block.custom_block && other.BlockID == Block.custom_block) 
                ? ExtID == other.ExtID : BlockID == other.BlockID;
        }
        
        public static bool operator == (ExtBlock a, ExtBlock b) {
            return (a.BlockID == Block.custom_block && b.BlockID == Block.custom_block) 
                ? a.ExtID == b.ExtID : a.BlockID == b.BlockID;
        }
        
        public static bool operator != (ExtBlock a, ExtBlock b) {
            return (a.BlockID == Block.custom_block && b.BlockID == Block.custom_block) 
                ? a.ExtID != b.ExtID : a.BlockID != b.BlockID;
        }
    }
}
