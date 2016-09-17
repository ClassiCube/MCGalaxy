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
            gs.WriteByte(0xBD);
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
        
    	
        public static void LoadDimensions(string file, out ushort width, out ushort height, out ushort length) {
            using (Stream fs = File.OpenRead(file), gs = new GZipStream(fs, CompressionMode.Decompress, true)) {
                byte[] header = new byte[16];
                int offset = 0;
                Vec3U16 dims = ReadHeader(gs, header, out offset);
                width = dims.X; height = dims.Y; length = dims.Z;
            }
        }
    	
        public static Level Load(string name, string file) {
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
    }
}