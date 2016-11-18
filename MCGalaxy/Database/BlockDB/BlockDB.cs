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
        
        public ushort Width, Height, Length;
        
        /// <summary> The map/level name associated with this BlockDB. </summary>
        public string MapName;
        
        /// <summary> Base point in time that all time deltas are offset from.</summary>
        public static DateTime Epoch = new DateTime(2010, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        /// <summary> The path of this BlockDB's backing file on disc. </summary>
        public string FilePath { get { return "blockdb/" + MapName + ".cbdb"; } }

        /// <summary> The path of this BlockDB's temp backing file on disc for resizing. </summary>
        public string TempPath { get { return "blockdb/" + MapName + ".temp"; } }
        
        readonly ReaderWriterLockSlim locker;
        public BlockDB(Level lvl) {
            MapName = lvl.name;
            Width = lvl.Width; Height = lvl.Height; Length = lvl.Length;
            locker = new ReaderWriterLockSlim();
        }

        
        public void AppendEntries(ref FastList<BlockDBEntry> entries, object lockObj) {
            using (IDisposable writeLock = locker.AccquireWriteLock()) {
                if (entries.Count == 0) return;
                
                ValidateBackingFile();
                using (Stream s = File.OpenWrite(FilePath)) {
                    s.Position = s.Length;
                    BlockDBFile.WriteEntries(s, entries);
                    
                    lock (lockObj)
                        entries = new FastList<BlockDBEntry>();
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
            Vec3U16 dims;

            if (!File.Exists(FilePath)) {
                using (Stream s = File.OpenWrite(FilePath)) {
                    dims = new Vec3U16(Width, Height, Length);
                    BlockDBFile.WriteHeader(s, dims);
                }
            } else {
                using (Stream s = File.OpenRead(FilePath)) {
                    BlockDBFile.ReadHeader(s, out dims);
                }
                if (dims.X < Width || dims.Y < Height || dims.Z < Length) {
                    BlockDBFile.ResizeBackingFile(this);
                }
            }
        }
    }
}
