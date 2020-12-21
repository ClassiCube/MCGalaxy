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

namespace MCGalaxy.Util {
    /// <summary> Sparsely represents 1 bit of data per voxel for a 3D volume. </summary>
    /// <remarks> Typically this means 1 bit per block for a level. </remarks>
    /// <remarks> Does NOT perform any bounds checking. </remarks>
    public sealed class SparseBitSet {
        
        int chunksX, chunksY, chunksZ;
        byte[][] bits;
        
        /// <summary> Initialises a sparse bit set for the given 3D volume. </summary>
        public SparseBitSet(int width, int height, int length) {
            chunksX = Utils.CeilDiv16(width);
            chunksY = Utils.CeilDiv16(height);
            chunksZ = Utils.CeilDiv16(length);
            bits = new byte[chunksX * chunksY * chunksZ][];
        }
        
        /// <summary> Returns the 1 bit of data associated with the given coordinates. </summary>
        /// <remarks> If Set() was never called before at the given coordinates, returns false. </remarks>
        public bool Get(int x, int y, int z) {
            int index = (x >> 4) + chunksX * ((z >> 4) + (y >> 4) * chunksZ);
            byte[] chunk = bits[index];
            if (chunk == null) return false;
            
            index = (x & 0xF) | (z & 0xF) << 4 | (y & 0xF) << 8;
            return (chunk[index >> 3] & (1 << (index & 0x7))) != 0;
        }
        
        /// <summary> Sets the 1 bit of data associated with the given coordinates. </summary>
        public void Set(int x, int y, int z, bool bit) {
            int index = (x >> 4) + chunksX * ((z >> 4) + (y >> 4) * chunksZ);
            byte[] chunk = bits[index];
            if (chunk == null) {
                chunk = new byte[(16 * 16 * 16) / 8];
                bits[index] = chunk;
            }
            
            index = (x & 0xF) | (z & 0xF) << 4 | (y & 0xF) << 8;
            chunk[index >> 3] &= (byte)(~(1 << (index & 0x7))); // reset bit
            chunk[index >> 3] |= (byte)((bit ? 1 : 0) << (index & 0x7)); // set new bit
        }
        
        /// <summary> Attempts to sets the 1 bit of data associated with the given coordinates to true. </summary>
        /// <remarks> true if the 1 bit of data was not already set to true, false if it was. </remarks>
        public bool TrySetOn(int x, int y, int z) {
            int index = (x >> 4) + chunksX * ((z >> 4) + (y >> 4) * chunksZ);
            byte[] chunk = bits[index];
            if (chunk == null) {
                chunk = new byte[(16 * 16 * 16) / 8];
                bits[index] = chunk;
            }
            
            index = (x & 0xF) | (z & 0xF) << 4 | (y & 0xF) << 8;
            if ((chunk[index >> 3] & (1 << (index & 0x7))) != 0) return false;
            
            chunk[index >> 3] |= (byte)(1 << (index & 0x7)); // set new bit
            return true;
        }
        
        /// <summary> Resets all bits of data to false. </summary>
        public void Clear() {
            byte[][] bits_ = bits; // local var to avoid JIT bounds check
            for (int i = 0; i < bits_.Length; i++)
                bits_[i] = null;
        }
    }
}
