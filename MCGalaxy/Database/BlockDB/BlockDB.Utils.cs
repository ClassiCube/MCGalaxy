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

namespace MCGalaxy.DB {
    
    public unsafe partial class BlockDB {
        
        const byte version = 1;
        const int entrySize = 16;
        const int bulkEntries = 256;
        
        public ushort Width, Height, Length;
        
        /// <summary> The map/level name associated with this BlockDB. </summary>
        public string MapName;
        
        /// <summary> The path of this BlockDB's backing file on disc. </summary>
        public string FilePath { get { return "blockdb/" + MapName + ".cbdb"; } }

        /// <summary> The path of this BlockDB's temp backing file on disc for resizing. </summary>
        public string TempPath { get { return "blockdb/" + MapName + ".temp"; } }
        
        /// <summary> Base point in time that all time deltas are offset from.</summary>
        public static DateTime Epoch = new DateTime(2000, 1, 1, 1, 1, 1, DateTimeKind.Utc);
        
        readonly ReaderWriterLockSlim locker;
        public BlockDB(Level lvl) {
            MapName = lvl.name;
            Width = lvl.Width; Height = lvl.Height; Length = lvl.Length;
            locker = new ReaderWriterLockSlim();
        }
        
        /// <summary> Checks if the backing file exists on disc, and if not, creates it.
        /// Also recreates the backing file if dimensions on disc are less than those in memory. </summary>
        void ValidateBackingFile() {
            Vec3U16 dims;

            if (!File.Exists(FilePath)) {
                using (Stream s = File.OpenWrite(FilePath)) {
                    dims = new Vec3U16(Width, Height, Length);
                    WriteHeader(s, dims);
                }
            } else {
                using (Stream s = File.OpenRead(FilePath)) {
                    ReadHeader(s, out dims);
                }
                if (dims.X < Width || dims.Y < Height || dims.Z < Length) {
                    ResizeBackingFile();
                }
            }
        }
        
        unsafe void ResizeBackingFile() {
            Server.s.Log("Resizing BlockDB for " + MapName, true);            
            using (Stream src = File.OpenRead(FilePath), dst = File.Create(TempPath)) {
                Vec3U16 dims;
                ReadHeader(src, out dims);
                WriteHeader(dst, new Vec3U16(Width, Height, Length));
                
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
                            entryPtr[i].Index = (y * Length + z) * Width + x;
                        }
                        
                        dst.Write(bulk, 0, read * entrySize);
                        entries -= read;
                    }
                }
            }
            
            File.Delete(FilePath);
            File.Move(TempPath, FilePath);
        }
        
        
        static void WriteHeader(Stream s, Vec3U16 dims) {
            byte[] header = new byte[entrySize * 4];
            NetUtils.WriteAscii("CBDB_MCG", header, 0);
            WriteU16(version, header, 8);
            WriteU16(dims.X, header, 10);
            WriteU16(dims.Y, header, 12);
            WriteU16(dims.Z, header, 14);
            s.Write(header, 0, entrySize);
        }
        
        static void ReadHeader(Stream s, out Vec3U16 dims) {
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