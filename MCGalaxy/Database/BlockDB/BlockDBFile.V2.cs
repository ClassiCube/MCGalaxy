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
using System.IO;
using MCGalaxy.Util;

namespace MCGalaxy.DB {
    
    public unsafe sealed class BlockDBFile_V2 : BlockDBFile {
        const int BlockSize = 4096;
        
        /* TODO: Last chunk may only be partial. need to prepend these entries when compressing. */
        public override void WriteEntries(Stream s, FastList<BlockDBEntry> entries) {
            throw new NotImplementedException();
        }

        public override void WriteEntries(Stream s, BlockDBCache cache) {
            throw new NotImplementedException();
        }

        public override long CountEntries(Stream s) {
            byte[] data = new byte[8];
            s.Position = 16;
            BlockDBFile.ReadFully(s, data, data.Length);
            
            uint lo = (uint)ReadInt32(data, 0);
            uint hi = (uint)ReadInt32(data, 4);
            return (long)((ulong)lo | ((ulong)hi << 32));
        }
        
        public unsafe override int ReadForward(Stream s, byte[] bulk, BlockDBEntry* entriesPtr) {
            long remaining = s.Length - s.Position;
            if (remaining == 0) return 0;
            int bytes = (int)Math.Min(remaining, BlockSize);
            
            /* TODO: BULK POINTS TO ENTRIESPTR, FIX THIS */
            BlockDBFile.ReadFully(s, bulk, bytes);
            return DecompressChunk(bulk, entriesPtr);
        }
        
        public unsafe override int ReadBackward(Stream s, byte[] bulk, BlockDBEntry* entriesPtr) {
            long pos = s.Position;
            if (pos > BlockSize) {
                int bytes = (int)Math.Min(pos - BlockSize, BlockSize);
                pos -= bytes;
                s.Position = pos;
                BlockDBFile.ReadFully(s, bulk, bytes);
                s.Position = pos; // set correct position for next backward read            
                return DecompressChunk(bulk, entriesPtr);
            }
            return 0;
        }
        
        unsafe static int DecompressChunk(byte[] bulk, BlockDBEntry* ptr) {
            byte comp = bulk[0];
            int count = bulk[1] | (bulk[2] << 8);
            int idx = 3;
            
            int playerID = 0;
            if ((comp & 0x01) < 0x01) { playerID = ReadInt32(bulk, idx); idx += 4; }
            int time = 0;
            if ((comp & 0x06) < 0x06) { time = ReadInt32(bulk, idx); idx += 4; }
            int index = 0;
            if ((comp & 0x18) < 0x18) { index = ReadInt32(bulk, idx); idx += 4; }
            
            byte oldRaw = 0;
            if ((comp & 0x20) < 0x20) { oldRaw = bulk[idx]; idx++; }
            byte newRaw = 0;
            if ((comp & 0x40) < 0x40) { newRaw = bulk[idx]; idx++; }
            ushort flags = 0;
            if ((comp & 0x80) < 0x80) { flags = (ushort)(bulk[idx] | (bulk[idx + 1] << 8)); idx += 2; }

            for (int i = 0; i < count; i++) {
                switch (comp & 0x01) {
                    case 0x00: ptr->PlayerID = playerID; break;
                    default: ptr->PlayerID = ReadInt32(bulk, idx); idx += 4; break;
                }
                
                switch ((comp & 0x06) >> 1) {
                    case 0: ptr->TimeDelta = time; break;
                    case 1: ptr->TimeDelta = time + bulk[idx]; idx++; break;
                    case 2: ptr->TimeDelta = time + (bulk[idx] | (bulk[idx + 1] << 8)); idx += 2; break;
                    case 3: ptr->TimeDelta = ReadInt32(bulk, idx); idx += 4; break;
                }
                
                switch ((comp & 0x18) >> 3) {
                    case 0: ptr->Index = index; break;
                    case 1: ptr->Index = index + (sbyte)bulk[idx]; idx++; break;
                    case 2: ptr->Index = index + (short)(bulk[idx] | (bulk[idx + 1] << 8)); idx += 2; break;
                    case 3: ptr->Index = ReadInt32(bulk, idx); idx += 4; break;
                }
                
                switch (comp & 0x20) {
                    case 0x00: ptr->OldRaw = oldRaw; break;
                    default: ptr->OldRaw = bulk[idx]; idx++; break;
                }
                
                switch (comp & 0x40) {
                    case 0x00: ptr->NewRaw = newRaw; break;
                    default: ptr->NewRaw = bulk[idx]; idx++; break;
                }
                
                switch (comp & 0x80) {
                    case 0x00: ptr->Flags = flags; break;
                    default: ptr->Flags = (ushort)(bulk[idx] | (bulk[idx + 1] << 8)); idx += 2; break;
                }
            }
            return count;
        }
        
        static int ReadInt32(byte[] bulk, int idx) {
            return bulk[idx] | (bulk[idx + 1] << 8) | (bulk[idx + 2] << 16) | (bulk[idx + 3] << 24);
        }
    }
}