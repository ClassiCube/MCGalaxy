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

namespace MCGalaxy.Levels.IO {
    
    public static class DatFile {
        
        public static Level Load(Stream stream, string name)
        {
            byte[] temp = new byte[8];
            using (Level lvl = new Level(name, 0, 0, 0))
            {
                stream.Seek(-4, SeekOrigin.End);
                stream.Read(temp, 0, sizeof(int));
                stream.Seek(0, SeekOrigin.Begin);
                int length = BitConverter.ToInt32(temp, 0);
                byte[] data = new byte[length];
                using (GZipStream reader = new GZipStream(stream, CompressionMode.Decompress, true))
                    reader.Read(data, 0, length);

                for (int i = 0; i < length - 1; i++)
                {
                    if (data[i] == 0xAC && data[i + 1] == 0xED)
                    {

                        // bypassing the header crap
                        int pointer = i + 6;
                        Array.Copy(data, pointer, temp, 0, sizeof(short));
                        pointer += IPAddress.HostToNetworkOrder(BitConverter.ToInt16(temp, 0));
                        pointer += 13;

                        int headerEnd = 0;
                        // find the end of serialization listing
                        for (headerEnd = pointer; headerEnd < data.Length - 1; headerEnd++)
                        {
                            if (data[headerEnd] == 0x78 && data[headerEnd + 1] == 0x70)
                            {
                                headerEnd += 2;
                                break;
                            }
                        }

                        // start parsing serialization listing
                        int offset = 0;
                        while (pointer < headerEnd)
                        {
                            if (data[pointer] == 'Z') offset++;
                            else if (data[pointer] == 'I' || data[pointer] == 'F') offset += 4;
                            else if (data[pointer] == 'J') offset += 8;

                            pointer += 1;
                            Array.Copy(data, pointer, temp, 0, sizeof(short));
                            short skip = IPAddress.HostToNetworkOrder(BitConverter.ToInt16(temp, 0));
                            pointer += 2;

                            // look for relevant variables
                            Array.Copy(data, headerEnd + offset - 4, temp, 0, sizeof(int));
                            if (MemCmp(data, pointer, "width"))
                            {
                                lvl.Width = (ushort)IPAddress.HostToNetworkOrder(BitConverter.ToInt32(temp, 0));
                            }
                            else if (MemCmp(data, pointer, "depth"))
                            {
                                lvl.Height = (ushort)IPAddress.HostToNetworkOrder(BitConverter.ToInt32(temp, 0));
                            }
                            else if (MemCmp(data, pointer, "height"))
                            {
                                lvl.Length = (ushort)IPAddress.HostToNetworkOrder(BitConverter.ToInt32(temp, 0));
                            }

                            pointer += skip;
                        }
                        
                        lvl.spawnx = (ushort)(lvl.Width / 1.3);
                        lvl.spawny = (ushort)(lvl.Height / 1.3);
                        lvl.spawnz = (ushort)(lvl.Length / 1.3);

                        // find the start of the block array
                        bool foundBlockArray = false;
                        offset = Array.IndexOf<byte>(data, 0x00, headerEnd);
                        while (offset != -1 && offset < data.Length - 2)
                        {
                            if (data[offset] == 0x00 && data[offset + 1] == 0x78 && data[offset + 2] == 0x70)
                            {
                                foundBlockArray = true;
                                pointer = offset + 7;
                            }
                            offset = Array.IndexOf<byte>(data, 0x00, offset + 1);
                        }

                        // copy the block array... or fail
                        if (foundBlockArray) {
                            CopyBlocks(lvl, data, pointer);
                            lvl.Save(true);
                        } else {
                            throw new InvalidDataException("Could not locate block array.");
                        }
                        break;
                    }
                }
                return lvl;
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

            for (int i = 0; i < blocks.Length; i++)
            {
                if (blocks[i] >= 50) blocks[i] = 0;
                switch (blocks[i])
                {
                    case Block.waterstill:
                        blocks[i] = Block.water;
                        break;
                    case Block.water:
                        blocks[i] = Block.waterstill;
                        break;
                    case Block.lava:
                        blocks[i] = Block.lavastill;
                        break;
                    case Block.lavastill:
                        blocks[i] = Block.lava;
                        break;
                }
            }
            lvl.blocks = blocks;
        }
    }
}
