/*
    Copyright 2015 MCGalaxy
        
    Dual-licensed under the Educational Community License, Version 2.0 and
    the GNU General Public License, Version 3 (the "Licenses"); you may
    not use this file except in compliance with the Licenses. You may
    obtain a copy of the Licenses at
    
    https://opensource.org/license/ecl-2-0/
    https://www.gnu.org/licenses/gpl-3.0.html
    
    Unless required by applicable law or agreed to in writing,
    software distributed under the Licenses are distributed on an "AS IS"
    BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express
    or implied. See the Licenses for the specific language governing
    permissions and limitations under the Licenses.
 */
using System;
using System.IO;
using MCGalaxy.Maths;
using MCGalaxy.Util;

namespace MCGalaxy.DB 
{
    public unsafe sealed class BlockDBFile_V2 : BlockDBFile 
    {
        const int CHUNK_SIZE = BlockDBFile.BULK_ENTRIES;
        
        public override byte Version { get { return 2; } }
        
        public override void WriteHeader(Stream s, Vec3U16 dims) {
            throw new NotImplementedException();
            // TODO: Write a full chunk
        }

        public override long CountEntries(Stream s) {
            byte[] data = new byte[8];
            s.Position  = 16;
            ReadFully(s, data, 0, data.Length);
            
            uint lo = (uint)ReadI32(data, 0);
            uint hi = (uint)ReadI32(data, 4);
            return (long)((ulong)lo | ((ulong)hi << 32));
        }
        
        
        /* TODO: Last chunk in file may only be partially filled. need to prepend these entries when compressing more. */
        public override void WriteEntries(Stream s, BlockDBEntry[] entries, int count) {
            throw new NotImplementedException();
        }

        public override void WriteEntries(Stream s, BlockDBCache cache) {
            throw new NotImplementedException();
        }
        
        public override void WriteRaw(Stream s, byte[] bulk, BlockDBEntry* entryPtr, int count) {
            throw new NotImplementedException();
        }
        
        
        public unsafe override int ReadForward(Stream s, byte[] bulk, BlockDBEntry* entriesPtr) {
            long pos = s.Position;
            // TODO: Align position to CHUNK_SIZE. and read partial if necessary
            // Version 2 expects all chunks to be aligned to 4096 bytes
            if (pos < CHUNK_SIZE) { s.Position = CHUNK_SIZE; pos = CHUNK_SIZE; }
            long remaining = s.Length - pos;
            if (remaining == 0) return 0;
            
            int bytes = (int)Math.Min(remaining, CHUNK_SIZE);
            int offset = bulk.Length - CHUNK_SIZE;
            
            // NOTE: bulk and entriesPtr point to same thing
            // But we read into the end of the bulk array, thus the entriesPtr pointing
            // to start of array never ends up overlapping with the data being read
            ReadFully(s, bulk, offset, bytes);
            return DecompressChunk(bulk, offset, entriesPtr);
        }
        
        public unsafe override int ReadBackward(Stream s, byte[] bulk, BlockDBEntry* entriesPtr) {
            long pos = s.Position;
            if (pos > CHUNK_SIZE) {
                int bytes = (int)Math.Min(pos - CHUNK_SIZE, CHUNK_SIZE);
                int offset = bulk.Length - CHUNK_SIZE;
                
                pos -= bytes;
                s.Position = pos;
                ReadFully(s, bulk, offset, bytes);
                s.Position = pos; // set correct position for next backward read
                return DecompressChunk(bulk, offset, entriesPtr);
            }
            return 0;
        }
        
        unsafe static int DecompressChunk(byte[] bulk, int idx, BlockDBEntry* ptr) {
            int comp  = bulk[idx + 0] | (bulk[idx + 1] << 8);
            int count = bulk[idx + 2] | (bulk[idx + 3] << 8); // TODO mask to bulk_entries
            idx += 4;
            
            // TODO byte ushort modes for playerID
            // TODO: always read these fields... ?
            int playerID = 0;
            if ((comp & 0x03) < 0x03) { playerID = ReadI32(bulk, idx); idx += 4; }
            int time = 0;
            if ((comp & 0x0C) < 0x0C) { time = ReadI32(bulk, idx); idx += 4; }
            int index = 0;
            if ((comp & 0x30) < 0x30) { index = ReadI32(bulk, idx); idx += 4; }
            
            byte oldRaw = 0;
            if ((comp &  0x40) < 0x20)  { oldRaw = bulk[idx]; idx++; }
            byte newRaw = 0;
            if ((comp &  0x80) < 0x40)  { newRaw = bulk[idx]; idx++; }
            ushort flags = 0;
            if ((comp & 0x100) < 0x100) { flags = (ushort)(bulk[idx] | (bulk[idx + 1] << 8)); idx += 2; }

            for (int i = 0; i < count; i++)
            {
                switch (comp & 0x03) {
                    case 0: ptr->PlayerID = playerID; break;
                    case 1: ptr->PlayerID = bulk[idx]; idx++; break;
                    case 2: ptr->PlayerID = bulk[idx] | (bulk[idx + 1] << 8); idx += 2; break;
                    case 3: ptr->PlayerID = ReadI32(bulk, idx); idx += 4; break;
                }
                
                switch ((comp & 0x0C) >> 2) {
                    case 0: ptr->TimeDelta = time; break;
                    case 1: ptr->TimeDelta = time + bulk[idx]; idx++; break;
                    case 2: ptr->TimeDelta = time + (bulk[idx] | (bulk[idx + 1] << 8)); idx += 2; break;
                    case 3: ptr->TimeDelta = ReadI32(bulk, idx); idx += 4; break;
                }
                
                switch ((comp & 0x30) >> 4) {
                    case 0: ptr->Index = index; break;
                    case 1: ptr->Index = index + (sbyte)bulk[idx]; idx++; break;
                    case 2: ptr->Index = index + (short)(bulk[idx] | (bulk[idx + 1] << 8)); idx += 2; break;
                    case 3: ptr->Index = ReadI32(bulk, idx); idx += 4; break;
                }
                
                switch (comp & 0x40) {
                    case 0x00: ptr->OldRaw = oldRaw; break;
                    default:   ptr->OldRaw = bulk[idx]; idx++; break;
                }
                
                switch (comp & 0x80) {
                    case 0x00: ptr->NewRaw = newRaw; break;
                    default:   ptr->NewRaw = bulk[idx]; idx++; break;
                }
                
            	// TODO more flags modes (same base mode, differing old/new
                switch (comp & 0x100) {
                    case 0x00: ptr->Flags = flags; break;
                    default:   ptr->Flags = (ushort)(bulk[idx] | (bulk[idx + 1] << 8)); idx += 2; break;
                }
            }
            return count;
        }
    }
}