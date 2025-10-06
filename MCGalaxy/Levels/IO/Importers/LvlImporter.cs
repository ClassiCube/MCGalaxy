﻿/*
    Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCForge)
    
    Dual-licensed under the Educational Community License, Version 2.0 and
    the GNU General Public License, Version 3 (the "Licenses"); you may
    not use this file except in compliance with the Licenses. You may
    obtain a copy of the Licenses at
    
    https://opensource.org/license/ecl-2-0/
    https://www.gnu.org/licenses/gpl-3.0.html
    
    Unless required by applicable law or agreed to in writing,
    software distributed under the Licenses are distributed on an "AS IS"
    BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express
    or implied. See the Licenses for the specific language governing
    permissions and limitations under the Licenses.
 */
using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using MCGalaxy.Maths;

namespace MCGalaxy.Levels.IO 
{
    //WARNING! DO NOT CHANGE THE WAY THE LEVEL IS SAVED/LOADED!
    //You MUST make it able to save and load as a new version other wise you will make old levels incompatible!
    public unsafe sealed class LvlImporter : IMapImporter 
    {
        public override string Extension { get { return ".lvl"; } }
        public override string Description { get { return "MCDzienny/MCForge/MCGalaxy map"; } }
        const int HEADER_SIZE = 18;
        
        public override Vec3U16 ReadDimensions(Stream src) {
            using (Stream gs = new GZipStream(src, CompressionMode.Decompress, true)) {
                byte[] header = new byte[HEADER_SIZE];
                return ReadHeader(gs, header);
            }
        }
        
        public override Level Read(Stream src, string name, bool metadata) {
            using (Stream gs = new GZipStream(src, CompressionMode.Decompress, true)) {
                byte[] header = new byte[HEADER_SIZE];
                Vec3U16 dims  = ReadHeader(gs, header);

                Level lvl  = new Level(name, dims.X, dims.Y, dims.Z);
                lvl.spawnx = BitConverter.ToUInt16(header,  8);
                lvl.spawnz = BitConverter.ToUInt16(header, 10);
                lvl.spawny = BitConverter.ToUInt16(header, 12);
                lvl.rotx   = header[14];
                lvl.roty   = header[15];
                // pervisit/perbuild permission bytes ignored
                
                StreamUtils.ReadFully(gs, lvl.blocks, 0, lvl.blocks.Length);
                ReadCustomBlocksSection(lvl, gs);
                if (!metadata) return lvl;
                
                for (;;) 
                {
                    int section = gs.ReadByte();
                    if (section == 0xFC) { // 'ph'ysics 'c'hecks
                        ReadPhysicsSection(lvl, gs); continue;
                    }
                    if (section == 0x51) { // 'z'one 'l'ist
                        ReadZonesSection(lvl, gs); continue;
                    }
                    return lvl;
                }
            }
        }
        
        static Vec3U16 ReadHeader(Stream gs, byte[] header) {
            StreamUtils.ReadFully(gs, header, 0, HEADER_SIZE);
            int signature = BitConverter.ToUInt16(header, 0);
            if (signature != 1874)
                throw new InvalidDataException("Invalid .lvl map signature");
            
            Vec3U16 dims;
            dims.X = BitConverter.ToUInt16(header, 2);
            dims.Z = BitConverter.ToUInt16(header, 4);
            dims.Y = BitConverter.ToUInt16(header, 6);
            return dims;
        }
        
        static void ReadCustomBlocksSection(Level lvl, Stream gs) {
            byte[] data = new byte[1];
            int read = gs.Read(data, 0, 1);
            if (read == 0 || data[0] != 0xBD) return;
            
            int index = 0;
            for (int y = 0; y < lvl.ChunksY; y++)
                for (int z = 0; z < lvl.ChunksZ; z++)
                    for (int x = 0; x < lvl.ChunksX; x++)
            {
                read = gs.Read(data, 0, 1);
                if (read > 0 && data[0] == 1) {
                    byte[] chunk = new byte[16 * 16 * 16];
                    StreamUtils.ReadFully(gs, chunk, 0, chunk.Length);
                    lvl.CustomBlocks[index] = chunk;
                }
                index++;
            }
        }
        
        
        static void ReadPhysicsSection(Level lvl, Stream gs) {
            byte[] buffer = new byte[sizeof(int)];
            int count = TryRead_I32(buffer, gs);
            if (count == 0) return;
            
            lvl.ListCheck.Count = count;
            lvl.ListCheck.Items = new Check[count];
            ReadPhysicsEntries(lvl, gs, count);
        }
        
        static void ReadPhysicsEntries(Level lvl, Stream gs, int count) {
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
                    C.Index    = *ptrInt;         ptrInt++;
                    C.data.Raw = (uint)(*ptrInt); ptrInt++;
                    lvl.ListCheck.Items[i + j] = C;
                }
            }
        }
        
        static void ReadZonesSection(Level lvl, Stream gs) {
            byte[] buffer = new byte[sizeof(int)];
            int count = TryRead_I32(buffer, gs);
            if (count == 0) return;
            
            for (int i = 0; i < count; i++) {
                try {
                    ParseZone(lvl, ref buffer, gs);
                } catch (Exception ex) {
                    Logger.LogError("Error importing zone #" + i + " from MCSharp map", ex);
                }
            }
        }
        
        static void ParseZone(Level lvl, ref byte[] buffer, Stream gs) {
            Zone z = new Zone();
            z.MinX = Read_U16(buffer, gs); z.MaxX = Read_U16(buffer, gs);
            z.MinY = Read_U16(buffer, gs); z.MaxY = Read_U16(buffer, gs);
            z.MinZ = Read_U16(buffer, gs); z.MaxZ = Read_U16(buffer, gs);
            
            int metaCount = TryRead_I32(buffer, gs);
            ConfigElement[] elems = Server.zoneConfig;
            
            for (int j = 0; j < metaCount; j++) 
            {
                int size = Read_U16(buffer, gs);
                if (size > buffer.Length) buffer = new byte[size + 16];
                StreamUtils.ReadFully(gs, buffer, 0, size);
                
                string line = Encoding.UTF8.GetString(buffer, 0, size), key, value;
                PropertiesFile.ParseLine(line, '=', out key, out value);
                if (key == null) continue;
                
                value = value.Trim();
                ConfigElement.Parse(elems, z.Config, key, value);
            }
            z.AddTo(lvl);
        }
        
        static int TryRead_I32(byte[] buffer, Stream gs) {
            int read = gs.Read(buffer, 0, sizeof(int));
            if (read < sizeof(int)) return 0;
            
            return MemUtils.ReadI32_BE(buffer, 0);
        }
        
        static ushort Read_U16(byte[] buffer, Stream gs) {
            StreamUtils.ReadFully(gs, buffer, 0, sizeof(ushort));
            
            return MemUtils.ReadU16_BE(buffer, 0);
        }
    }
}