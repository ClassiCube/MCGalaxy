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
using System.Threading;
using MCGalaxy.Util;

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
        public string FilePath { get { return "blockdb/" + MapName + ".cbdb"; } }

        /// <summary> The path of this BlockDB's temp backing file on disc for resizing. </summary>
        public string TempPath { get { return "blockdb/" + MapName + ".temp"; } }
        
        /// <summary> Used to synchronise adding to Cache by multiple threads. </summary>
        internal readonly object CacheLock = new object();
        
        /// <summary> In-memory list of recent BlockDB changes. </summary>
        public FastList<BlockDBEntry> Cache = new FastList<BlockDBEntry>();
        
        /// <summary> Whether changes are actually added to the BlockDB. </summary>
        public bool Used;
        
        readonly ReaderWriterLockSlim locker;
        public BlockDB(Level lvl) {
            MapName = lvl.name;
            ReadDimensions();
            locker = new ReaderWriterLockSlim();
            
            if (Dims.X < lvl.Width) Dims.X = lvl.Width;
            if (Dims.Y < lvl.Height) Dims.Y = lvl.Height;
            if (Dims.Z < lvl.Length) Dims.Z = lvl.Length;
        }
        
        void ReadDimensions() {
            if (!File.Exists(FilePath)) return;
            using (Stream s = File.OpenRead(FilePath))
                BlockDBFile.ReadHeader(s, out Dims);
        }

        
        
        public void Add(Player p, ushort x, ushort y, ushort z, ushort flags,
                        byte oldBlock, byte oldExt, byte block, byte ext) {
            if (!Used) return;
            BlockDBEntry entry;
            entry.PlayerID = p.UserID;
            entry.TimeDelta = (int)DateTime.UtcNow.Subtract(BlockDB.Epoch).TotalSeconds;
            entry.Index = x + Dims.X * (z + Dims.Z * y);
            
            entry.OldRaw = oldBlock; entry.NewRaw = block;
            entry.Flags = flags;
            
            if (block == Block.custom_block) {
                entry.Flags |= BlockDBFlags.NewCustom;
                entry.NewRaw = ext;
            }
            if (oldBlock == Block.custom_block) {
                entry.Flags |= BlockDBFlags.OldCustom;
                entry.OldRaw = oldExt;
            }
            
            lock (CacheLock)
                Cache.Add(entry);
        }
        
        public void WriteEntries() {
            using (IDisposable writeLock = locker.AccquireWriteLock()) {
                if (Cache.Count == 0) return;
                
                ValidateBackingFile();
                using (Stream s = File.OpenWrite(FilePath)) {
                    s.Position = s.Length;
                    BlockDBFile.WriteEntries(s, Cache);
                    
                    lock (CacheLock)
                        Cache = new FastList<BlockDBEntry>();
                }
            }
        }

        
        /// <summary> Finds all block changes which affect the given coordinates. </summary>
        public void FindChangesAt(ushort x, ushort y, ushort z, Action<BlockDBEntry> output) {
            using (IDisposable readLock = locker.AccquireReadLock()) {
                if (!File.Exists(FilePath)) return;
                
                using (Stream s = File.OpenRead(FilePath)) {
                    Vec3U16 dims;
                    BlockDBFile.ReadHeader(s, out dims);
                    if (x >= dims.X || y >= dims.Y || z >= dims.Z) return;
                    
                    int index = (y * dims.Z + z) * dims.X + x;
                    BlockDBFile.FindChangesAt(s, index, output);
                }
            }
        }
        
        /// <summary> Finds all block changes by the given player. </summary>
        public void FindChangesBy(int id, Action<BlockDBEntry> output, out Vec3U16 dims) {
            dims = default(Vec3U16);
            using (IDisposable readLock = locker.AccquireReadLock()) {
                if (!File.Exists(FilePath)) return;
                
                using (Stream s = File.OpenRead(FilePath)) {
                    BlockDBFile.ReadHeader(s, out dims);
                    BlockDBFile.FindChangesBy(s, id, output);
                }
            }
        }
        
        /// <summary> Finds all block changes by the given players. </summary>
        public void FindChangesBy(int[] ids, Action<BlockDBEntry> output, out Vec3U16 dims) {
            dims = default(Vec3U16);
            using (IDisposable readLock = locker.AccquireReadLock()) {
                if (!File.Exists(FilePath)) return;
                
                using (Stream s = File.OpenRead(FilePath)) {
                    BlockDBFile.ReadHeader(s, out dims);
                    BlockDBFile.FindChangesBy(s, ids, output);
                }
            }
        }
        
        /// <summary> Deletes the backing file on disc if it exists. </summary>
        public void DeleteBackingFile() {
            using (IDisposable writeLock = locker.AccquireWriteLock()) {
                if (!File.Exists(FilePath)) return;
                File.Delete(FilePath);
            }
        }

        /// <summary> Checks if the backing file exists on disc, and if not, creates it.
        /// Also recreates the backing file if dimensions on disc are less than those in memory. </summary>
        void ValidateBackingFile() {
            Vec3U16 fileDims;

            if (!File.Exists(FilePath)) {
                using (Stream s = File.OpenWrite(FilePath)) {
                    fileDims = Dims;
                    BlockDBFile.WriteHeader(s, fileDims);
                }
            } else {
                using (Stream s = File.OpenRead(FilePath)) {
                    BlockDBFile.ReadHeader(s, out fileDims);
                }
                if (fileDims.X < Dims.X || fileDims.Y < Dims.Y || fileDims.Z < Dims.Z) {
                    BlockDBFile.ResizeBackingFile(this);
                }
            }
        }
    }
}
