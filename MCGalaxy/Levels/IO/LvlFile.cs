/*
    Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCGalaxy)
    
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
using System.IO.Compression;

namespace MCGalaxy.Levels.IO {

    //WARNING! DO NOT CHANGE THE WAY THE LEVEL IS SAVED/LOADED!
    //You MUST make it able to save and load as a new version other wise you will make old levels incompatible!
    public static class LvlFile {
        
        public static void Save(Level lvl, string file) {
            using (Stream fs = File.Create(file), gs = new GZipStream(fs, CompressionMode.Compress, true)) {
                WriteHeader(lvl, gs);
                WriteBlocksSection(lvl, gs);
                WriteBlockDefsSection(lvl, gs);
                WritePhysicsSection(lvl, gs);
            }
        }
        
        static void WriteHeader(Level lvl, Stream gs) {
            byte[] header = new byte[18];
            BitConverter.GetBytes(1874).CopyTo(header, 0);
            BitConverter.GetBytes(lvl.Width).CopyTo(header, 2);
            BitConverter.GetBytes(lvl.Length).CopyTo(header, 4);
            BitConverter.GetBytes(lvl.Height).CopyTo(header, 6);
            BitConverter.GetBytes(lvl.spawnx).CopyTo(header, 8);
            BitConverter.GetBytes(lvl.spawnz).CopyTo(header, 10);
            BitConverter.GetBytes(lvl.spawny).CopyTo(header, 12);
            header[14] = lvl.rotx;
            header[15] = lvl.roty;
            header[16] = (byte)lvl.permissionvisit;
            header[17] = (byte)lvl.permissionbuild;
            gs.Write(header, 0, header.Length);
        }
        
        static void WriteBlocksSection(Level lvl, Stream gs) {
            byte[] blocks = lvl.blocks;
            int start = 0, len = 0;
            
            for (int i = 0; i < blocks.Length; ++i) {
                byte block = blocks[i], convBlock = 0;
                if (block < Block.CpeCount || (convBlock = Block.SaveConvert(block)) == block) {
                    if (len == 0) start = i;
                    len++;
                } else {
                    if (len > 0) gs.Write(blocks, start, len);
                    len = 0;
                    gs.WriteByte(convBlock);
                }
            }
            if (len > 0) gs.Write(blocks, start, len);
        }
        
        static void WriteBlockDefsSection(Level lvl, Stream gs) {
            gs.WriteByte(0xBD); // 'B'lock 'D'efinitions
            int index = 0;
            
            for (int y = 0; y < lvl.ChunksY; y++)
                for (int z = 0; z < lvl.ChunksZ; z++)
                    for (int x = 0; x < lvl.ChunksX; x++)
            {
                byte[] chunk = lvl.CustomBlocks[index];
                if (chunk == null) {
                    gs.WriteByte(0);
                } else {
                    gs.WriteByte(1);
                    gs.Write(chunk, 0, chunk.Length);
                }
                index++;
            }
        }
        
        const int fxBulkCount = 1024, fxEntrySize = 2 * sizeof(int);
        unsafe static void WritePhysicsSection(Level lvl, Stream gs) {
            lock (lvl.physStepLock) {
                // Count the number of physics checks with extra info
                int used = 0, count = lvl.ListCheck.Count;
                Check[] checks = lvl.ListCheck.Items;
                for (int i = 0; i < count; i++) {
                    if (checks[i].data.Raw == 0) continue;
                    used++;
                }
                if (used == 0) return;
                
                gs.WriteByte(0xFC); // 'Ph'ysics 'C'hecks
                int bulkCount = Math.Min(used, fxBulkCount);
                byte[] buffer = new byte[bulkCount * fxEntrySize];
                NetUtils.WriteI32(used, buffer, 0);
                gs.Write(buffer, 0, sizeof(int));
                
                fixed (byte* ptr = buffer) {
                    WritePhysicsEntries(gs, lvl.ListCheck, buffer, ptr);
                }
            }
        }
        
        unsafe static void WritePhysicsEntries(Stream gs, FastList<Check> items,
                                               byte[] buffer, byte* ptr) {
            Check[] checks = items.Items;
            int entries = 0, count = items.Count;
            int* ptrInt = (int*)ptr;
            
            for (int i = 0; i < count; i++) {
                Check C = checks[i];
                // Does this check have extra physics data
                if (C.data.Raw == 0) continue;
                *ptrInt = C.b; ptrInt++;
                *ptrInt = (int)C.data.Raw; ptrInt++;
                entries++;
                
                // Have we filled the temp buffer?
                if (entries != fxBulkCount) continue;
                ptrInt = (int*)ptr;
                gs.Write(buffer, 0, entries * 8);
                entries = 0;
            }
            
            if (entries == 0) return;
            gs.Write(buffer, 0, entries * 8);
        }
        
        public static void LoadDimensions(string file, out ushort width, out ushort height, out ushort length) {
            using (Stream fs = File.OpenRead(file), gs = new GZipStream(fs, CompressionMode.Decompress, true)) {
                byte[] header = new byte[16];
                int offset = 0;
                Vec3U16 dims = ReadHeader(gs, header, out offset);
                width = dims.X; height = dims.Y; length = dims.Z;
            }
        }
        
        public static Level Load(string name, string file, bool loadPhysics = false) {
            using (Stream fs = File.OpenRead(file), gs = new GZipStream(fs, CompressionMode.Decompress, true)) {
                byte[] header = new byte[16];
                int offset = 0;
                Vec3U16 dims = ReadHeader(gs, header, out offset);

                Level lvl = new Level(name, dims.X, dims.Y, dims.Z);
                lvl.spawnx = BitConverter.ToUInt16(header, offset + 4);
                lvl.spawnz = BitConverter.ToUInt16(header, offset + 6);
                lvl.spawny = BitConverter.ToUInt16(header, offset + 8);
                lvl.rotx = header[offset + 10];
                lvl.roty = header[offset + 11];
                
                gs.Read(lvl.blocks, 0, lvl.blocks.Length);
                ReadBlockDefsSection(lvl, gs);
                if (!loadPhysics) return lvl;
                
                ReadPhysicsSection(lvl, gs);
                return lvl;
            }
        }
        
        static Vec3U16 ReadHeader(Stream gs, byte[] header, out int offset) {
            gs.Read(header, 0, 2);
            Vec3U16 dims = default(Vec3U16);
            dims.X = BitConverter.ToUInt16(header, 0);

            if (dims.X == 1874) { // version field, width is next ushort
                gs.Read(header, 0, 16);
                dims.X = BitConverter.ToUInt16(header, 0);
                offset = 2;
            } else {
                gs.Read(header, 0, 12);
                offset = 0;
            }
            
            dims.Z = BitConverter.ToUInt16(header, offset);
            dims.Y = BitConverter.ToUInt16(header, offset + 2);
            return dims;
        }
        
        static void ReadBlockDefsSection(Level lvl, Stream gs) {
            if (gs.ReadByte() != 0xBD) return;
            
            int index = 0;
            for (int y = 0; y < lvl.ChunksY; y++)
                for (int z = 0; z < lvl.ChunksZ; z++)
                    for (int x = 0; x < lvl.ChunksX; x++)
            {
                if (gs.ReadByte() == 1) {
                    byte[] chunk = new byte[16 * 16 * 16];
                    gs.Read(chunk, 0, chunk.Length);
                    lvl.CustomBlocks[index] = chunk;
                }
                index++;
            }
        }
        
        unsafe static void ReadPhysicsSection(Level lvl, Stream gs) {
            if (gs.ReadByte() != 0xFC) return;
            byte[] buffer = new byte[sizeof(int)];
            int read = gs.Read(buffer, 0, sizeof(int));
            if (read < sizeof(int)) return;
            
            int count = NetUtils.ReadI32(buffer, 0);
            lvl.ListCheck.Count = count;
            lvl.ListCheck.Items = new Check[count];
            ReadPhysicsEntries(lvl, gs, count);           
        }
        
        unsafe static void ReadPhysicsEntries(Level lvl, Stream gs, int count) {
            byte[] buffer = new byte[Math.Min(count, fxBulkCount) * fxEntrySize];
            Check C;
            
            fixed (byte* ptr = buffer)
                for (int i = 0; i < count; i += fxBulkCount)
            {
                int entries = Math.Min(count - i, fxBulkCount);
                int read = gs.Read(buffer, 0, entries * fxEntrySize);
                if (read < entries * fxEntrySize) return;
                
                int* ptrInt = (int*)ptr;
                for (int j = 0; j < entries; j++) {
                	C.b = *ptrInt; ptrInt++;
                    C.data.Raw = (uint)(*ptrInt); ptrInt++;
                    lvl.ListCheck.Items[i + j] = C;
                }
            }
        }
    }
}