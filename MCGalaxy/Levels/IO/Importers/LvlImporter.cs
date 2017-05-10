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
using MCGalaxy.Maths;

namespace MCGalaxy.Levels.IO {

    //WARNING! DO NOT CHANGE THE WAY THE LEVEL IS SAVED/LOADED!
    //You MUST make it able to save and load as a new version other wise you will make old levels incompatible!
    public sealed class LvlImporter : IMapImporter {

        public override string Extension { get { return ".lvl"; } }
        
        public override Vec3U16 ReadDimensions(Stream src) {
            using (Stream gs = new GZipStream(src, CompressionMode.Decompress, true)) {
                byte[] header = new byte[16];
                int offset = 0;
                return ReadHeader(gs, header, out offset);
            }
        }
        
        public override Level Read(Stream src, string name, bool metadata) {
            using (Stream gs = new GZipStream(src, CompressionMode.Decompress, true)) {
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
                ReadCustomBlocksSection(lvl, gs);
                
                if (!metadata) return lvl;
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
        
        static void ReadCustomBlocksSection(Level lvl, Stream gs) {
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
            byte[] buffer = new byte[Math.Min(count, 1024) * 8];
            Check C;
            
            fixed (byte* ptr = buffer)
                for (int i = 0; i < count; i += 1024)
            {
                int entries = Math.Min(1024, count - i);
                int read = gs.Read(buffer, 0, entries * 8);
                if (read < entries * 8) return;
                
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