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
    public unsafe sealed class BlockDBFile_V1 : BlockDBFile 
    {
        const int V1_ENTRY_SIZE = 16; // Same size as RAW_ENTRY_SIZE (size of a BlockDB record)
        // TODO V1_ENTRY_SHIFT
        
        public override byte Version { get { return 1; } }
        
        public override void WriteHeader(Stream s, Vec3U16 dims) {
            byte[] header = new byte[HEADER_SIZE];
            FillHeader(s, header, dims);
            s.Write(header, 0, HEADER_SIZE);
        }
        
        public override long CountEntries(Stream s) {
            return (s.Length - HEADER_SIZE) / V1_ENTRY_SIZE;
        }
        
        
        public override void WriteEntries(Stream s, BlockDBEntry[] entries, int count) {
            byte[] bulk = new byte[BULK_BUFFER_SIZE];
            for (int i = 0; i < count; i += BULK_ENTRIES) 
            {
                int bulkCount = Math.Min(BULK_ENTRIES, count - i);
                // TODO BlockCopy
                for (int j = 0; j < bulkCount; j++) 
                {
                    WriteEntry(entries[i + j], bulk, j * V1_ENTRY_SIZE);
                }
                s.Write(bulk, 0, bulkCount * V1_ENTRY_SIZE);
            }
        }

        public override void WriteEntries(Stream s, BlockDBCache cache) {
            byte[] bulk = new byte[BULK_BUFFER_SIZE];
            BlockDBCacheNode node = cache.Tail;
            
            while (node != null) {
                int count = node.Count;
                for (int i = 0; i < count; i += BULK_ENTRIES) 
                {
                    int bulkCount = Math.Min(BULK_ENTRIES, count - i);
                    for (int j = 0; j < bulkCount; j++) 
                    {
                        BlockDBEntry entry = node.Unpack(node.Entries[i + j]);
                        WriteEntry(entry, bulk, j * V1_ENTRY_SIZE);
                    }
                    s.Write(bulk, 0, bulkCount * V1_ENTRY_SIZE);
                }
                
                lock (cache.Locker)
                    node = node.Next;
            }
        }
        
        public override void WriteRaw(Stream s, byte[] bulk, BlockDBEntry* entryPtr, int count) {
            s.Write(bulk, 0, count * V1_ENTRY_SIZE);
        }
        
        // Inlined WriteI32/WriteU16 for better performance
        static void WriteEntry(BlockDBEntry entry, byte[] bulk, int index) {
            bulk[index + 0 ] = (byte)(entry.PlayerID);
            bulk[index + 1 ] = (byte)(entry.PlayerID >> 8);
            bulk[index + 2 ] = (byte)(entry.PlayerID >> 16);
            bulk[index + 3 ] = (byte)(entry.PlayerID >> 24);
            
            bulk[index + 4 ] = (byte)(entry.TimeDelta);
            bulk[index + 5 ] = (byte)(entry.TimeDelta >> 8);
            bulk[index + 6 ] = (byte)(entry.TimeDelta >> 16);
            bulk[index + 7 ] = (byte)(entry.TimeDelta >> 24);
            
            bulk[index + 8 ] = (byte)(entry.Index);
            bulk[index + 9 ] = (byte)(entry.Index >> 8);
            bulk[index + 10] = (byte)(entry.Index >> 16);
            bulk[index + 11] = (byte)(entry.Index >> 24);
            
            bulk[index + 12] = entry.OldRaw;
            bulk[index + 13] = entry.NewRaw;
            bulk[index + 14] = (byte)(entry.Flags);
            bulk[index + 15] = (byte)(entry.Flags >> 8);
        }
        
        
        public unsafe override int ReadForward(Stream s, byte[] bulk, BlockDBEntry* entriesPtr) {
            long remaining = (s.Length - s.Position) / V1_ENTRY_SIZE; // TODO mask instead
            int count = (int)Math.Min(remaining, BULK_ENTRIES);
            
            if (count > 0) {
                ReadFully(s, bulk, 0, count * V1_ENTRY_SIZE);
            }
            return count;
        }
         
        public unsafe override int ReadBackward(Stream s, byte[] bulk, BlockDBEntry* entriesPtr) {
            long pos = s.Position;
            long remaining = (pos - HEADER_SIZE) / V1_ENTRY_SIZE;
            int count = (int)Math.Min(remaining, BULK_ENTRIES);
            // TODO rethink this s.Position and don't set twice
            
            if (count > 0) {
                pos -= count * V1_ENTRY_SIZE;
                s.Position = pos;
                ReadFully(s, bulk, 0, count * V1_ENTRY_SIZE);
                s.Position = pos; // set correct position for next backward read
            }
            return count;
        }
    }
}