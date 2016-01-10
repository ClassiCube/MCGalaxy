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
    
    public sealed class SparseBitSet {
        
        int chunksX, chunksY, chunksZ;
        byte[][] bits;
        
        public SparseBitSet(int width, int height, int length) {
            chunksX = (width + 15) >> 4;
            chunksY = (height + 15) >> 4;
            chunksZ = (length + 15) >> 4;
            bits = new byte[chunksX * chunksY * chunksZ][];
        }
        
        public bool Get(int x, int y, int z) {
            int index = (x >> 4) + chunksX * 
                ((z >> 4) + (y >> 4) * chunksZ);
            byte[] chunk = bits[index];
            if (chunk == null) return false;
            
            index = (x & 0xF) | (z & 0xF) << 4 | (y & 0xF) << 8;
            return (chunk[index >> 3] & (1 << (index & 0x7))) != 0;
        }
        
        public void Set(int x, int y, int z, bool bit) {
            int index = (x >> 4) + chunksX * 
                ((z >> 4) + (y >> 4) * chunksZ);
            byte[] chunk = bits[index];
            if (chunk == null) {
                chunk = new byte[(16 * 16 * 16) / 8];
                bits[index] = chunk;
            }
            
            index = (x & 0xF) | (z & 0xF) << 4 | (y & 0xF) << 8;
            chunk[index >> 3] &= (byte)(~(1 << (index & 0x7))); // reset bit
            chunk[index >> 3] |= (byte)((bit ? 1 : 0) << (index & 0x7)); // set new bit
        }
        
        public void Clear() {
            for (int i = 0; i < bits.Length; i++)
                bits[i] = null;
        }
    }
}
