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
    
    public unsafe sealed class BlockDB {
        
        /// <summary> Dimensions used to pack coordinates into an index. </summary>
        /// <remarks> May be different from actual level's dimensions, such as when the level has been resized. </remarks>
        public Vec3U16 Dims;
        
        /// <summary> The map/level name associated with this BlockDB. </summary>
        public string MapName;
        
        /// <summary> Base point in time that all time deltas are offset from.</summary>
        public static DateTime Epoch = new DateTime(2010, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        /// <summary> The path of this BlockDB's backing file on disc. </summary>
        public string FilePath { get { return BlockDBFile.FilePath(MapName); } }
        
        /// <summary> In-memory list of recent BlockDB changes. </summary>
        public BlockDBCache Cache = new BlockDBCache();
        
        /// <summary> Used to synchronise access to the in-memory and on-disc BlockDB entries. </summary>
        public readonly IReaderWriterLock Locker;
        
        public BlockDB(Level lvl) {
            MapName       = lvl.name;
            Cache.Enabled = lvl.Config.UseBlockDB; 
            ReadDimensions();
            Locker = new IReaderWriterLock();
            
            if (Dims.X < lvl.Width)  Dims.X = lvl.Width;
            if (Dims.Y < lvl.Height) Dims.Y = lvl.Height;
            if (Dims.Z < lvl.Length) Dims.Z = lvl.Length;
            Cache.Dims = Dims;
        }
        
        void ReadDimensions() {
            if (!File.Exists(FilePath)) return;
            using (Stream s = OpenRead())
                BlockDBFile.ReadHeader(s, out Dims);
        }

        /// <summary> Flushes the entries from the in-memory cache to disc. </summary>
        /// <remarks> You must lock using Locker.AccquireWrite() **before** entering this method. </remarks>
        public void FlushCache() {
            if (Cache.Head == null) return;
            
            BlockDBFile format = ValidateBackingFile();
            using (Stream s = OpenWrite()) {
                // This truncates the lower 4 bits off - so e.g. if a power off occurred
                // and 21 bytes were in the file, this sets the position to byte 16
                s.Position = s.Length & ~0x0F;
                format.WriteEntries(s, Cache);
                Cache.Clear();
            }
        }
        
        /// <summary> Counts the total number of entries of BlockDB entries in memory and on disc. </summary>
        public long TotalEntries() {
            using (IDisposable rLock = Locker.AccquireRead(5 * 1000)) {
                if (rLock != null) {
                    return Cache.Count + BlockDBFile.CountEntries(MapName);
                }
                return -1;
            }
        }

        
        /// <summary> Outputs all block changes which affect the given coordinates. </summary>
        /// <remarks> You must lock using Locker.AccquireRead() **before** entering this method. </remarks>
        public void FindChangesAt(ushort x, ushort y, ushort z, Action<BlockDBEntry> output) {
            if (!File.Exists(FilePath)) { FindInMemoryAt(x, y, z, output); return; }
            Vec3U16 dims;
            
            using (Stream s = OpenRead()) {
                BlockDBFile format = BlockDBFile.ReadHeader(s, out dims);
                if (x >= dims.X || y >= dims.Y || z >= dims.Z) return;
                
                int index = (y * dims.Z + z) * dims.X + x;
                format.FindChangesAt(s, index, output);
            }
            FindInMemoryAt(x, y, z, output);
        }
        
        void FindInMemoryAt(ushort x, ushort y, ushort z, Action<BlockDBEntry> output) {
            int index = (y * Dims.Z + z) * Dims.X + x;
            BlockDBCacheNode node = Cache.Tail;
            while (node != null) {
                BlockDBCacheEntry[] entries = node.Entries;
                int count = node.Count;
                
                for (int i = 0; i < count; i++) {
                    if (entries[i].Index != index) continue;
                    BlockDBEntry entry = node.Unpack(entries[i]);
                    output(entry);
                }
                lock (Cache.Locker) node = node.Next;
            }
        }
        
        /// <summary> Outputs all block changes by the given players. </summary>
        /// <remarks> You must lock using Locker.AccquireRead() **before** entering this method. </remarks>
        /// <returns> whether an entry before start time was reached. </returns>
        public bool FindChangesBy(int[] ids, DateTime start, DateTime end,
                                  out Vec3U16 dims, Action<BlockDBEntry> output) {
            int startDelta = ClampDelta(start.Subtract(Epoch));
            int endDelta = ClampDelta(end.Subtract(Epoch));
            
            dims = Dims;
            if (FindInMemoryBy(ids, startDelta, endDelta, output)) return true;
            
            if (!File.Exists(FilePath)) return false;
            using (Stream s = OpenRead()) {
                BlockDBFile format = BlockDBFile.ReadHeader(s, out dims);
                return format.FindChangesBy(s, ids, startDelta, endDelta, output);
            }
        }
        
        bool FindInMemoryBy(int[] ids, int startDelta, int endDelta, Action<BlockDBEntry> output) {
            BlockDBCacheNode node = Cache.Head;
            while (node != null) {
                int count = node.Count;
                BlockDBCacheEntry[] entries = node.Entries;
                
                for (int i = count - 1; i >= 0; i--) {
                    BlockDBEntry entry = node.Unpack(entries[i]);
                    if (entry.TimeDelta < startDelta) return true;
                    if (entry.TimeDelta > endDelta) continue;
                    
                    for (int j = 0; j < ids.Length; j++) {
                        if (entry.PlayerID != ids[j]) continue;
                        output(entry); break;
                    }
                }
                lock (Cache.Locker) node = node.Prev;
            }
            return false;
        }
        
        static int ClampDelta(TimeSpan delta) {
            long secs = (long)delta.TotalSeconds;
            if (secs < int.MinValue) return int.MinValue;
            if (secs > int.MaxValue) return int.MaxValue;
            return (int)secs;
        }
        
        
        /// <summary> Deletes the backing file on disc if it exists. </summary>
        public void DeleteBackingFile() {
            using (IDisposable writeLock = Locker.AccquireWrite()) {
                if (!File.Exists(FilePath)) return;
                File.Delete(FilePath);
            }
        }

        /// <summary> Checks if the backing file exists on disc, and if not, creates it.
        /// Also recreates the backing file if dimensions on disc are less than those in memory. </summary>
        BlockDBFile ValidateBackingFile() {
            Vec3U16 fileDims;

            BlockDBFile format = BlockDBFile.V1;
            if (!File.Exists(FilePath)) {
                using (Stream s = OpenWrite()) {
                    fileDims = Dims;
                    BlockDBFile.WriteHeader(s, fileDims);
                }
            } else {
                using (Stream s = OpenRead()) {
                    format = BlockDBFile.ReadHeader(s, out fileDims);
                }
                if (fileDims.X < Dims.X || fileDims.Y < Dims.Y || fileDims.Z < Dims.Z) {
                    BlockDBFile.ResizeBackingFile(this);
                }
            }
            return format;
        }
        
                
        FileStream OpenWrite() { 
            return new FileStream(FilePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite); 
        }
        
        FileStream OpenRead() {
            return new FileStream(FilePath, FileMode.OpenOrCreate, FileAccess.Read, FileShare.ReadWrite); 
        }
    }
}
