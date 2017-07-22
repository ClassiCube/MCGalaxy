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
using MCGalaxy.Maths;

namespace MCGalaxy.DB {
    
	public unsafe sealed class BlockDBFile_V1 : BlockDBFile {
                
        public override void WriteEntries(Stream s, FastList<BlockDBEntry> entries) {
            byte[] bulk = new byte[BulkEntries * EntrySize];
            WriteEntries(s, bulk, entries.Items, entries.Count);
        }

        public override void WriteEntries(Stream s, BlockDBCache cache) {
            byte[] bulk = new byte[BulkEntries * EntrySize];
            BlockDBCacheNode node = cache.Tail;
            
            while (node != null) {
                WriteEntries(s, bulk, node);
                lock (cache.Locker)
                    node = node.Next;
            }
        }
        
        static void WriteEntries(Stream s, byte[] bulk, BlockDBEntry[] entries, int count) {
            for (int i = 0; i < count; i += BulkEntries) {
                int bulkCount = Math.Min(BulkEntries, count - i);
                for (int j = 0; j < bulkCount; j++) {
                    WriteEntry(entries[i + j], bulk, j * EntrySize);
                }
                s.Write(bulk, 0, bulkCount * EntrySize);
            }
        }
        
        static void WriteEntries(Stream s, byte[] bulk, BlockDBCacheNode node) {
            int count = node.Count;
            for (int i = 0; i < count; i += BulkEntries) {
                int bulkCount = Math.Min(BulkEntries, count - i);
                for (int j = 0; j < bulkCount; j++) {
                    BlockDBEntry entry = node.Unpack(node.Entries[i + j]);
                    WriteEntry(entry, bulk, j * EntrySize);
                }
                s.Write(bulk, 0, bulkCount * EntrySize);
            }
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

        
        public override void FindChangesAt(Stream s, int index, Action<BlockDBEntry> output) {
            byte[] bulk = new byte[BulkEntries * EntrySize];
            fixed (byte* ptr = bulk) {
                int dbEntries = (int)(s.Length / EntrySize) - HeaderEntries;
                while (dbEntries > 0) {
                    int count = Math.Min(dbEntries, BulkEntries);
                    ReadFully(s, bulk, count * EntrySize);
                    BlockDBEntry* entryPtr = (BlockDBEntry*)ptr;
                    
                    for (int i = 0; i < count; i++) {
                        if (entryPtr->Index == index) {
                            output(*entryPtr);
                        }
                        entryPtr++;
                    }
                    dbEntries -= count;
                }
            }
        }
        
        public override bool FindChangesBy(Stream s, int[] ids, int start, int end, Action<BlockDBEntry> output) {
            byte[] bulk = new byte[BulkEntries * EntrySize];
            fixed (byte* ptr = bulk) {
                int dbEntries = (int)(s.Length / EntrySize) - HeaderEntries;
                
                while (dbEntries > 0) {
                    int count = Math.Min(dbEntries, BulkEntries);
                    // find the correct position for the start of this bulk read
                    s.Position = (dbEntries - count + HeaderEntries) * (long)EntrySize;
                    
                    ReadFully(s, bulk, count * EntrySize);
                    BlockDBEntry* entryPtr = (BlockDBEntry*)ptr;
                    entryPtr += (count - 1);
                    
                    for (int i = count - 1; i >= 0; i--) {
                        if (entryPtr->TimeDelta < start) return true;
                        
                        if (entryPtr->TimeDelta <= end) {
                            for (int j = 0; j < ids.Length; j++) {
                                if (entryPtr->PlayerID != ids[j]) continue;
                                output(*entryPtr); break;
                            }
                        }
                        entryPtr--;
                    }
                    dbEntries -= count;
                }
            }
            return false;
        }
        
        static ushort ReadU16(byte[] array, int offset) {
            return (ushort)(array[offset] | array[offset + 1] << 8);
        }
        
        static void WriteU16(ushort value, byte[] array, int index) {
            array[index++] = (byte)(value);
            array[index++] = (byte)(value >> 8);
        }
        
        static void ReadFully(Stream stream, byte[] dst, int count) {
            int total = 0;
            do {
                int read = stream.Read(dst, total, count - total);
                if (read == 0) throw new EndOfStreamException();
                total += read;
            } while (total < count);
        }
    }
}