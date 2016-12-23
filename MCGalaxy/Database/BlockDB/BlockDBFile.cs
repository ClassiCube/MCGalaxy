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
    
    unsafe static class BlockDBFile {
        
        const byte version = 1;
        const int entrySize = 16;
        const int bulkEntries = 256;
        
        public static string FilePath(string map) { return "blockdb/" + map + ".cbdb"; }
        
        public static void WriteHeader(Stream s, Vec3U16 dims) {
            byte[] header = new byte[entrySize * 4];
            NetUtils.WriteAscii("CBDB_MCG", header, 0);
            WriteU16(version, header, 8);
            WriteU16(dims.X, header, 10);
            WriteU16(dims.Y, header, 12);
            WriteU16(dims.Z, header, 14);
            s.Write(header, 0, entrySize);
        }
        
        public static void ReadHeader(Stream s, out Vec3U16 dims) {
            dims = default(Vec3U16);
            byte[] header = new byte[entrySize];
            ReadFully(s, header, header.Length);
            
            // Check constants are expected
            // TODO: check 8 byte string identifier
            ushort fileVersion = ReadU16(header, 8);
            if (fileVersion != version)
                throw new NotSupportedException("only version 1 is supported");
            
            dims.X = ReadU16(header, 10);
            dims.Y = ReadU16(header, 12);
            dims.Z = ReadU16(header, 14);
        }
        
        
        public static void WriteEntries(Stream s, FastList<BlockDBEntry> entries) {
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
        
        // NOTE: These are duplicated since it is important to be as performant as possible,
        // since the BlockDB can have millions and millions of entries
        public static void FindChangesAt(Stream s, int index, Action<BlockDBEntry> output) {
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
        
        public static void FindChangesBy(Stream s, int id, Action<BlockDBEntry> output) {
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
        
        public static void FindChangesBy(Stream s, int[] ids, Action<BlockDBEntry> output) {
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
        
        public static void ResizeBackingFile(BlockDB db) {
            Server.s.Log("Resizing BlockDB for " + db.MapName, true);
            using (Stream src = File.OpenRead(db.FilePath), dst = File.Create(db.TempPath)) {
                Vec3U16 dims;
                ReadHeader(src, out dims);
                WriteHeader(dst, db.Dims);
                int width = db.Dims.X, length = db.Dims.Z;
                
                byte[] bulk = new byte[bulkEntries * entrySize];
                fixed (byte* ptr = bulk) {
                    int entries = (int)(src.Length / entrySize) - 1;
                    BlockDBEntry* entryPtr = (BlockDBEntry*)ptr;
                    
                    while (entries > 0) {
                        int read = Math.Min(entries, bulkEntries);
                        ReadFully(src, bulk, read * entrySize);
                        
                        for (int i = 0; i < read; i++) {
                            int index = entryPtr[i].Index;
                            int x = index % dims.X;
                            int y = (index / dims.X) / dims.Z;
                            int z = (index / dims.X) % dims.Z;
                            entryPtr[i].Index = (y * length + z) * width + x;
                        }
                        
                        dst.Write(bulk, 0, read * entrySize);
                        entries -= read;
                    }
                }
            }
            
            File.Delete(db.FilePath);
            File.Move(db.TempPath, db.FilePath);
        }
        
        static ushort ReadU16(byte[] array, int offset) {
            return (ushort)(array[offset] | array[offset + 1] << 8);
        }
        
        static void WriteU16(ushort value, byte[] array, int index) {
            array[index++] = (byte)(value);
            array[index++] = (byte)(value >> 8);
        }
        
        static void WriteI32(int value, byte[] array, int index) {
            array[index++] = (byte)(value);
            array[index++] = (byte)(value >> 8);
            array[index++] = (byte)(value >> 16);
            array[index++] = (byte)(value >> 24);
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