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

namespace MCGalaxy {
    
    public unsafe class BlockDB {
        
        const byte version = 1;
        const int entrySize = 16;
        const int bulkEntries = 256;
        
        public void WriteHeader(Stream stream, Level lvl) {
            byte[] header = new byte[4 * entrySize];
            NetUtils.WriteAscii("CBDB", header, 0);
            header[4] = version;
            header[5] = 0; // ref year
            WriteU16(lvl.Width, header, 6);
            WriteU16(lvl.Height, header, 8);
            WriteU16(lvl.Length, header, 10);
            
            NetUtils.WriteAscii("MCGalaxy", header, entrySize);
            stream.Write(header, 0, header.Length);
        }
        
        public void WriteEntries(Stream stream, List<BlockDBEntry> entries) {
            byte[] bulk = new byte[bulkEntries * entrySize];
            
            for (int i = 0; i < entries.Count; i += bulkEntries) {
                int count = Math.Min(bulkEntries, entries.Count - i);
                for (int j = 0; j < count; j++) {
                    BlockDBEntry entry = entries[i + j];
                    WriteI32(entry.PlayerID, bulk, j * entrySize);
                    WriteI32(entry.TimeDelta, bulk, j * entrySize + 4);
                    WriteI32(entry.Index, bulk, j * entrySize + 8);
                    bulk[j * entrySize + 12] = entry.Old;
                    bulk[j * entry + 13] = entry.New;
                    WriteU16(entry.Flags, bulk, j * entrySize + 14);
                }
                stream.Write(bulk, 0, count * entrySize);
            }
        }
        
        public List<BlockDBEntry> ReadEntries(int x, int y, int z, Stream stream) {
            byte[] bulk = new byte[bulkEntries * entrySize];
            ReadFully(stream, bulk, 4 * entrySize);
            List<BlockDBEntry> matches = new List<BlockDBEntry>();
            
            ushort width = ReadU16(bulk, 6);
            ushort height = ReadU16(bulk, 8);
            ushort length = ReadU16(bulk, 10);
            int index = x + width * (z + y * length);
            
            fixed (byte* ptr = bulk) {
                int entries = (int)(stream.Length / entrySize);
                while (entries > 0) {
                    int read = Math.Min(entries, bulkEntries);
                    ReadFully(stream, bulk, read * entrySize);
                    BlockDBEntry* entryPtr = (BlockDBEntry*)ptr;
                    
                    for (int i = 0; i < read; i++) {
                        if (entryPtr->Index != index) continue;
                        matches.Add(*entryPtr);
                        entryPtr++;
                    }
                    entries -= read;
                }
            }
            return matches;
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
    
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BlockDBEntry {
        public int PlayerID;
        public int TimeDelta;
        public int Index;
        public byte Old, New;
        public ushort Flags;
    }
}
