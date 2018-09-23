// 
//  Authors:
//   *  Tyler Kennedy <tk@tkte.ch>
//   *  Matvei Stefarov <fragmer@gmail.com>
// 
//  Copyright (c) 2010, Tyler Kennedy & Matvei Stefarov
// 
//  All rights reserved.
// 
//  Redistribution and use in source and binary forms, with or without modification,
// are permitted provided that the following conditions are met:
// 
//     * Redistributions of source code must retain the above copyright notice, this
//       list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright notice,
//       this list of conditions and the following disclaimer in
//       the documentation and/or other materials provided with the distribution.
//     * Neither the name of MCC nor the names of its contributors may be
//       used to endorse or promote products derived from this software without
//       specific prior written permission.
// 
//  THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
//  "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
//  LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
//  A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR
//  CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL,
//  EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
//  PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR
//  PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
//  LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
//  NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
//  SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
// 
using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using MCGalaxy.Maths;

namespace MCGalaxy.Levels.IO {
    public sealed class DatImporter : IMapImporter {

        public override string Extension { get { return ".dat"; } }
        public override string Description { get { return "Minecraft Classic map"; } }

        public override Vec3U16 ReadDimensions(Stream src) {
            throw new NotSupportedException();
        }
        
        const byte MAGIC1 = 0xAC, MAGIC2 = 0xED;
        const byte TYPE_END_BLOCK = 0x78, TYPE_NULL = 0x70;
        
        public override Level Read(Stream src, string name, bool metadata) {
            byte[] temp = new byte[8];
            Level lvl = new Level(name, 0, 0, 0);
            src.Seek(-4, SeekOrigin.End);
            src.Read(temp, 0, sizeof(int));
            src.Seek(0, SeekOrigin.Begin);
            
            int length = BitConverter.ToInt32(temp, 0);
            byte[] data = new byte[length];
            using (GZipStream reader = new GZipStream(src, CompressionMode.Decompress, true))
                reader.Read(data, 0, length);

            for (int i = 0; i < length - 1; i++) {
                if (data[i] != MAGIC1 || data[i + 1] != MAGIC2) continue;
                int pointer = 0, headerEnd = 0;
                ReadHeader(data, ref pointer, ref headerEnd, i);
                ReadFields(data, ref pointer, ref headerEnd, lvl);
                
                // find the start of the block array
                int offset = Array.IndexOf<byte>(data, 0x00, headerEnd);
                while (offset != -1 && offset < data.Length - 2) {
                    if (data[offset] == 0x00 && data[offset + 1] == TYPE_END_BLOCK && data[offset + 2] == TYPE_NULL) {
                        pointer = offset + 7;
                        CopyBlocks(lvl, data, pointer);
                        return lvl;
                    }
                    offset = Array.IndexOf<byte>(data, 0x00, offset + 1);
                }
                throw new InvalidDataException("Could not locate block array.");
            }
            return null;
        }
        
        static void ReadHeader(byte[] data, ref int pointer, ref int headerEnd, int i) {
            pointer = i + 6; // skip magic, version, and typecodes
            ushort classNameLen = NetUtils.ReadU16(data, pointer);
            pointer += classNameLen; // skip class name
            pointer += 13; // skip class description

            headerEnd = pointer;
            // find the end of serialization listing
            for (; headerEnd < data.Length - 1; headerEnd++) {
                if (data[headerEnd] == TYPE_END_BLOCK && data[headerEnd + 1] == TYPE_NULL) {
                    headerEnd += 2; break;
                }
            }
        }
        
        static void ReadFields(byte[] data, ref int pointer, ref int headerEnd, Level lvl) {
            // start parsing serialization listing
            int offset = 0;
            while (pointer < headerEnd) {
                byte type = data[pointer]; pointer += 1;
                if (type == 'Z') offset++;
                else if (type == 'I' || type == 'F') offset += 4;
                else if (type == 'J') offset += 8;
                ushort nameLen = NetUtils.ReadU16(data, pointer); pointer += 2;
                
                // look for relevant variables
                int valueOffset = headerEnd + offset - 4;
                if (MemCmp(data, pointer, "width")) {
                    lvl.Width = (ushort)NetUtils.ReadI32(data, valueOffset);
                } else if (MemCmp(data, pointer, "depth")) {
                    lvl.Height = (ushort)NetUtils.ReadI32(data, valueOffset);
                    lvl.Config.EdgeLevel = lvl.Height / 2;
                    lvl.Config.CloudsHeight = lvl.Height + 2;
                } else if (MemCmp(data, pointer, "height")) {
                    lvl.Length = (ushort)NetUtils.ReadI32(data, valueOffset);
                } else if (MemCmp(data, pointer, "xSpawn")) {
                    lvl.spawnx = (ushort)NetUtils.ReadI32(data, valueOffset);
                } else if (MemCmp(data, pointer, "ySpawn")) {
                    lvl.spawny = (ushort)NetUtils.ReadI32(data, valueOffset);
                } else if (MemCmp(data, pointer, "zSpawn")) {
                    lvl.spawnz = (ushort)NetUtils.ReadI32(data, valueOffset);
                }
                pointer += nameLen;
            }
        }

        static bool MemCmp( byte[] data, int offset, string value ) {
            for( int i = 0; i < value.Length; i++ ) {
                if( offset + i >= data.Length || data[offset + i] != value[i] ) return false;
            }
            return true;
        }
        
        static void CopyBlocks(Level lvl, byte[] source, int offset) {
            byte[] blocks = new byte[lvl.Width * lvl.Height * lvl.Length];
            Array.Copy(source, offset, blocks, 0, blocks.Length);

            for (int i = 0; i < blocks.Length; i++) {
                if (blocks[i] >= 50) blocks[i] = 0;
                switch (blocks[i]) {
                    case Block.StillWater:
                        blocks[i] = Block.Water; break;
                    case Block.Water:
                        blocks[i] = Block.StillWater; break;
                    case Block.Lava:
                        blocks[i] = Block.StillLava; break;
                    case Block.StillLava:
                        blocks[i] = Block.Lava; break;
                }
            }
            lvl.blocks = blocks;
        }
    }
}
