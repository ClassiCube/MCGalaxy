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
using System.Runtime.InteropServices;
using MCGalaxy.Util;

namespace MCGalaxy {
    
    public unsafe partial class BlockDB {
        
        public void AppendEntries(ref FastList<BlockDBEntry> entries, object lockObj) {
            using (IDisposable writeLock = locker.AccquireWriteLock()) {
                if (entries.Count == 0) return;
                
                ValidateBackingFile();
                using (Stream s = File.OpenWrite(FilePath)) {
                    s.Position = s.Length;
                    WriteEntries(s, entries);
                    
                    lock (lockObj)
                        entries = new FastList<BlockDBEntry>();
                }
            }
        }
        
        void WriteEntries(Stream s, FastList<BlockDBEntry> entries) {
            byte[] bulk = new byte[bulkEntries * entrySize];
            
            for (int i = 0; i < entries.Count; i += bulkEntries) {
                int count = Math.Min(bulkEntries, entries.Count - i);
                for (int j = 0; j < count; j++) {
                    BlockDBEntry entry = entries.Items[i + j];
                    WriteI32(entry.PlayerID, bulk, j * entrySize);
                    WriteI32(entry.TimeDelta, bulk, j * entrySize + 4);
                    WriteI32(entry.Index, bulk, j * entrySize + 8);
                    bulk[j * entrySize + 12] = entry.OldRaw;
                    bulk[j * entrySize + 13] = entry.NewRaw;
                    WriteU16(entry.Flags, bulk, j * entrySize + 14);
                }
                s.Write(bulk, 0, count * entrySize);
            }
        }
        
        /// <summary> Finds all block changes which affect the given coordinates. </summary>
        public void FindChangesAt(ushort x, ushort y, ushort z, Action<BlockDBEntry> output) {
            using (IDisposable readLock = locker.AccquireReadLock()) {
                if (!File.Exists(FilePath)) return;
                
                using (Stream s = File.OpenRead(FilePath)) {
                    Vec3U16 dims;
                    ReadHeader(s, out dims);
                    if (x >= dims.X || y >= dims.Y || z >= dims.Z) return;
                    
                    int index = (y * dims.Z + z) * dims.X + x;
                    FindChangesAt(s, index, output);
                }
            }
        }
        
        
        /// <summary> Finds all block changes by the given player. </summary>
        public void FindChangesBy(int id, Action<BlockDBEntry> output, out Vec3U16 dims) {
            dims = default(Vec3U16);
            using (IDisposable readLock = locker.AccquireReadLock()) {
                if (!File.Exists(FilePath)) return;
                
                using (Stream s = File.OpenRead(FilePath)) {
                    ReadHeader(s, out dims);
                    FindChangesBy(s, id, output);
                }
            }
        }
        
        /// <summary> Finds all block changes by the given players. </summary>
        public void FindChangesBy(int[] ids, Action<BlockDBEntry> output, out Vec3U16 dims) {
            dims = default(Vec3U16);
            using (IDisposable readLock = locker.AccquireReadLock()) {
                if (!File.Exists(FilePath)) return;
                
                using (Stream s = File.OpenRead(FilePath)) {
                    ReadHeader(s, out dims);
                    FindChangesBy(s, ids, output);
                }
            }
        }
        
        
        // NOTE: These are duplicated since it is important to be as performant as possible,
        // since the BlockDB can have millions and millions of entries
        static void FindChangesAt(Stream s, int index, Action<BlockDBEntry> output) {
            byte[] bulk = new byte[bulkEntries * entrySize];
            fixed (byte* ptr = bulk) {
                int entries = (int)(s.Length / entrySize) - 1;
                while (entries > 0) {
                    int read = Math.Min(entries, bulkEntries);
                    ReadFully(s, bulk, read * entrySize);
                    BlockDBEntry* entryPtr = (BlockDBEntry*)ptr;
                    
                    for (int i = 0; i < read; i++) {
                        if (entryPtr->Index == index) {
                            output(*entryPtr);
                        }
                        entryPtr++;
                    }
                    entries -= read;
                }
            }
        }
        
        static void FindChangesBy(Stream s, int id, Action<BlockDBEntry> output) {
            byte[] bulk = new byte[bulkEntries * entrySize];
            fixed (byte* ptr = bulk) {
                int entries = (int)(s.Length / entrySize) - 1;
                while (entries > 0) {
                    int read = Math.Min(entries, bulkEntries);
                    ReadFully(s, bulk, read * entrySize);
                    BlockDBEntry* entryPtr = (BlockDBEntry*)ptr;
                    
                    for (int i = 0; i < read; i++) {
                        if (entryPtr->PlayerID == id) {
                            output(*entryPtr);
                        }
                        entryPtr++;
                    }
                    entries -= read;
                }
            }
        }
        
        static void FindChangesBy(Stream s, int[] ids, Action<BlockDBEntry> output) {
            byte[] bulk = new byte[bulkEntries * entrySize];
            fixed (byte* ptr = bulk) {
                int entries = (int)(s.Length / entrySize) - 1;
                while (entries > 0) {
                    int read = Math.Min(entries, bulkEntries);
                    ReadFully(s, bulk, read * entrySize);
                    BlockDBEntry* entryPtr = (BlockDBEntry*)ptr;
                    
                    for (int i = 0; i < read; i++) {
                        for (int j = 0; j < ids.Length; j++) {
                            if (entryPtr->PlayerID == ids[j]) {
                                output(*entryPtr); break;
                            }
                        }
                        entryPtr++;
                    }
                    entries -= read;
                }
            }
        }
    }

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
}
