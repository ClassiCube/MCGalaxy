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
    
    public unsafe sealed class BlockDBFile_V1 : BlockDBFile {
        
        public override void WriteEntries(Stream s, FastList<BlockDBEntry> entries) {
            byte[] bulk = new byte[BulkEntries * EntrySize];
            for (int i = 0; i < entries.Count; i += BulkEntries) {
                int bulkCount = Math.Min(BulkEntries, entries.Count - i);
                for (int j = 0; j < bulkCount; j++) {
                    WriteEntry(entries.Items[i + j], bulk, j * EntrySize);
                }
                s.Write(bulk, 0, bulkCount * EntrySize);
            }
        }

        public override void WriteEntries(Stream s, BlockDBCache cache) {
            byte[] bulk = new byte[BulkEntries * EntrySize];
            BlockDBCacheNode node = cache.Tail;
            
            while (node != null) {
                int count = node.Count;
                for (int i = 0; i < count; i += BulkEntries) {
                    int bulkCount = Math.Min(BulkEntries, count - i);
                    for (int j = 0; j < bulkCount; j++) {
                        BlockDBEntry entry = node.Unpack(node.Entries[i + j]);
                        WriteEntry(entry, bulk, j * EntrySize);
                    }
                    s.Write(bulk, 0, bulkCount * EntrySize);
                }
                
                lock (cache.Locker)
                    node = node.Next;
            }
        }

        public override long CountEntries(Stream s) {
            return (s.Length / BlockDBFile.EntrySize) - BlockDBFile.HeaderEntries;
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
            long remaining = (s.Length - s.Position) / EntrySize;
            int count = (int)Math.Min(remaining, BulkEntries);
            
            if (count > 0) {
                BlockDBFile.ReadFully(s, bulk, 0, count * EntrySize);
            }
            return count;
        }
         
        public unsafe override int ReadBackward(Stream s, byte[] bulk, BlockDBEntry* entriesPtr) {
            long pos = s.Position;
            long remaining = (pos / EntrySize) - HeaderEntries;
            int count = (int)Math.Min(remaining, BulkEntries);
            
            if (count > 0) {
                pos -= count * EntrySize;
                s.Position = pos;
                BlockDBFile.ReadFully(s, bulk, 0, count * EntrySize);
                s.Position = pos; // set correct position for next backward read
            }
            return count;
        }
    }
}