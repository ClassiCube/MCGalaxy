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
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using MCGalaxy.Util;

namespace MCGalaxy {
    
    public unsafe partial class BlockDB {
        
        const byte version = 1;
        const int entrySize = 16;
        const int bulkEntries = 256;
        
        public ushort Width, Height, Length;
        
        /// <summary> The map/level name associated with this BlockDB. </summary>
        public string MapName;
        
        /// <summary> The path of this BlockDB's backing file on disc. </summary>
        public string FilePath { get { return "blockdefs/" + MapName + ".cbdb"; } }

        /// <summary> Base point in time that all time deltas are offset from.</summary>
        public static DateTime Epoch = new DateTime(2000, 1, 1, 1, 1, 1, DateTimeKind.Utc);
        
        readonly ReaderWriterLockSlim locker;
        bool resizeRequired;
        public BlockDB(Level lvl) {
            MapName = lvl.name;
            Width = lvl.Width; Height = lvl.Height; Length = lvl.Length;
            locker = new ReaderWriterLockSlim();
        }
        
        /// <summary> Checks if the backing file exists on disc, and if not, creates it.
        /// Also recreates the backing file if dimensions on disc are less than those in memory. </summary>
        public void ValidateBackingFile(Level lvl) {
            using (IDisposable writeLock = locker.AccquireWriteLock()) {
                if (!File.Exists(FilePath)) {
                    WriteHeader(this);
                } else {
                    Vec3U16 dims;
                    using (Stream s = File.OpenRead(FilePath))
                        ReadHeader(s, out dims);
                    
                    if (dims.X < Width || dims.Y < Height || dims.Z < Length) {
                        ResizeBackingFile();
                    }
                }
            }
        }
        
        public void ResizeBackingFile() {
            Server.s.Log("Resizing BlockDB for " + MapName, true);
            throw new NotImplementedException(); // TODO: resize backing file
        }
        
        
        static void WriteHeader(Stream s, Vec3U16 dims) {
            byte[] header = new byte[entrySize];
            NetUtils.WriteAscii("CBDB_MCG", header, 0);
            WriteU16(version, header, 8);
            WriteU16(dims.X, header, 10);
            WriteU16(dims.Y, header, 12);
            WriteU16(dims.Z, header, 14);
            s.Write(header, 0, header.Length);
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