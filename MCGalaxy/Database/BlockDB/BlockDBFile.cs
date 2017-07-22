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
    
    public unsafe abstract class BlockDBFile {
        
        public const byte Version = 1;
        public const int EntrySize = 16;
        public const int HeaderEntries = 1;
        public const int BulkEntries = 256;
        
        public static BlockDBFile V1 = new BlockDBFile_V1();
        
        public static string FilePath(string map) { return "blockdb/" + map + ".cbdb"; }
        public static string DumpPath(string map) { return "blockdb/" + map + ".dump"; }
        public static string TempPath(string map) { return "blockdb/" + map + ".temp"; }
        
        public static void WriteHeader(Stream s, Vec3U16 dims) {
            byte[] header = new byte[EntrySize * HeaderEntries * 4];
            NetUtils.Write("CBDB_MCG", header, 0, false);
            WriteU16(Version, header, 8);
            WriteU16(dims.X, header, 10);
            WriteU16(dims.Y, header, 12);
            WriteU16(dims.Z, header, 14);
            s.Write(header, 0, EntrySize);
        }
        
        public static BlockDBFile ReadHeader(Stream s, out Vec3U16 dims) {
            dims = default(Vec3U16);
            byte[] header = new byte[EntrySize * HeaderEntries];
            ReadFully(s, header, header.Length);
            
            // Check constants are expected
            // TODO: check 8 byte string identifier
            ushort fileVersion = ReadU16(header, 8);
            if (fileVersion != Version)
                throw new NotSupportedException("only version 1 is supported");
            
            dims.X = ReadU16(header, 10);
            dims.Y = ReadU16(header, 12);
            dims.Z = ReadU16(header, 14);
            return V1;
        }
        
        
        public abstract void WriteEntries(Stream s, FastList<BlockDBEntry> entries);

        public abstract void WriteEntries(Stream s, BlockDBCache cache);
        
        /// <summary> Iterates from the very oldest to newest entry in the BlockDB. </summary>
        public abstract void FindChangesAt(Stream s, int index, Action<BlockDBEntry> output);
        
        /// <summary> Iterates from the very newest to oldest entry in the BlockDB. </summary>
        /// <returns> whether an entry before start time was reached. </returns>
        public abstract bool FindChangesBy(Stream s, int[] ids, int start, int end, Action<BlockDBEntry> output);
        

        /// <summary> Deletes the backing file on disc if it exists. </summary>
        public static void DeleteBackingFile(string map) {
            string path = FilePath(map);
            if (!File.Exists(path)) return;
            File.Delete(path);
        }
        
        /// <summary> Moves the backing file on disc if it exists. </summary>
        public static void MoveBackingFile(string srcMap, string dstMap) {
            string srcPath = FilePath(srcMap), dstPath = FilePath(dstMap);
            if (!File.Exists(srcPath)) return;
            if (File.Exists(dstPath)) File.Delete(dstPath);
            File.Move(srcPath, dstPath);
        }
        
        /// <summary> Returns number of entries in the backing file on disc if it exists. </summary>
        public static long CountEntries(string map) {
            string path = FilePath(map);
            if (!File.Exists(path)) return 0;

            using (Stream src = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Read, FileShare.ReadWrite))
                return (src.Length / 16) - 1;
        }
        
        
        public static void ResizeBackingFile(BlockDB db) {
            Logger.Log(LogType.BackgroundActivity, "Resizing BlockDB for " + db.MapName);
            string filePath = FilePath(db.MapName);
            string tempPath = TempPath(db.MapName);
            
            using (Stream src = File.OpenRead(filePath), dst = File.Create(tempPath)) {
                Vec3U16 dims;
                ReadHeader(src, out dims);
                WriteHeader(dst, db.Dims);
                int width = db.Dims.X, length = db.Dims.Z;
                
                byte[] bulk = new byte[BulkEntries * EntrySize];
                fixed (byte* ptr = bulk) {
                    int entries = (int)(src.Length / EntrySize) - 1;
                    BlockDBEntry* entryPtr = (BlockDBEntry*)ptr;
                    
                    while (entries > 0) {
                        int read = Math.Min(entries, BulkEntries);
                        ReadFully(src, bulk, read * EntrySize);
                        
                        for (int i = 0; i < read; i++) {
                            int index = entryPtr[i].Index;
                            int x = index % dims.X;
                            int y = (index / dims.X) / dims.Z;
                            int z = (index / dims.X) % dims.Z;
                            entryPtr[i].Index = (y * length + z) * width + x;
                        }
                        
                        dst.Write(bulk, 0, read * EntrySize);
                        entries -= read;
                    }
                }
            }
            
            File.Delete(filePath);
            File.Move(tempPath, filePath);
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