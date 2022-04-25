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
using System.IO.Compression;
using BlockID = System.UInt16;

namespace MCGalaxy.Network 
{
    /// <summary> Streams the compressed form of a map directly to a Minecraft Classic client </summary>
    public sealed class LevelChunkStream : Stream 
    {
        public override bool CanRead { get { return false; } }
        public override bool CanSeek { get { return false; } }
        public override bool CanWrite { get { return true; } }
        
        static Exception ex = new NotSupportedException();
        public override void Flush() { }
        public override long Length { get { throw ex; } }
        public override long Position { get { throw ex; } set { throw ex; } }
        public override int Read(byte[] buffer, int offset, int count) { throw ex; }
        public override long Seek(long offset, SeekOrigin origin) { throw ex; }
        public override void SetLength(long length) { throw ex; }
        
        int index;
        byte chunkValue;
        ClassicProtocol session;
        byte[] data = new byte[chunkSize + 4];
        const int chunkSize = 1024;
        public LevelChunkStream(ClassicProtocol s) { session = s; }
        
        public override void Close() {
            if (index > 0) WritePacket();
            session = null;
            base.Close();
        }
        
        public override void Write(byte[] buffer, int offset, int count) {
            while (count > 0) {
                int copy = Math.Min(chunkSize - index, count);
                if (copy <= 8) {
                    for (int i = 0; i < copy; i++)
                        data[index + i + 3] = buffer[offset + i];
                } else {
                    Buffer.BlockCopy(buffer, offset, data, index + 3, copy);
                }
                offset += copy; index += copy; count -= copy;
                
                if (index != chunkSize) continue;
                WritePacket();
                data = new byte[chunkSize + 4];
            }
        }
        
        public override void WriteByte(byte value) {
            data[index + 3] = value;
            index++;
            
            if (index != chunkSize) return;
            WritePacket();
            data = new byte[chunkSize + 4];
        }
        
        void WritePacket() {
            data[0] = Opcode.LevelDataChunk;
            NetUtils.WriteU16((ushort)index, data, 1);
            data[1027] = chunkValue;

            session.Send(data);
            index = 0;
        }


        public static void SendLevel(ClassicProtocol session, Level level, int volume) {
            using (LevelChunkStream dst = new LevelChunkStream(session))
                using (Stream stream = dst.CompressMapHeader(volume))
            {
                if (level.MightHaveCustomBlocks()) {
                    CompressMap(level, stream, dst);
                } else {
                    CompressMapSimple(level, stream, dst);
                }
            }
        }
        
        Stream CompressMapHeader(int volume) {
            // FastMap sends volume in LevelInit packet instead
            if (session.Supports(CpeExt.FastMap)) {
               return new DeflateStream(this, CompressionMode.Compress, true);
            }
           
            Stream stream = new GZipStream(this, CompressionMode.Compress, true);
            byte[] buffer = new byte[4];

            NetUtils.WriteI32(volume, buffer, 0);
            stream.Write(buffer, 0, 4);
            return stream;
        }
        
        unsafe static void CompressMapSimple(Level lvl, Stream stream, LevelChunkStream dst) {
            const int bufferSize = 64 * 1024;
            byte[] buffer = new byte[bufferSize];
            int bIndex = 0;

            ClassicProtocol s = dst.session;
            byte[] blocks     = lvl.blocks;
            float progScale   = 100.0f / blocks.Length;

            // Store on stack instead of performing function call for every block in map
            byte* conv = stackalloc byte[256];
            for (int i = 0; i < 256; i++) 
            {
                conv[i] = (byte)s.ConvertBlock((BlockID)i);
            }
            
            // compress the map data in 64 kb chunks
            for (int i = 0; i < blocks.Length; ++i) {
                buffer[bIndex] = conv[blocks[i]];
                bIndex++;
                
                if (bIndex == bufferSize) {
                    // '0' to indicate this chunk has lower 8 bits of block ids
                    dst.chunkValue = s.hasExtBlocks ? (byte)0 : (byte)(i * progScale);
                    stream.Write(buffer, 0, bufferSize); bIndex = 0;
                }
            }
            if (bIndex > 0) stream.Write(buffer, 0, bIndex);
        }
        
        unsafe static void CompressMap(Level lvl, Stream stream, LevelChunkStream dst) {
            const int bufferSize = 64 * 1024;
            byte[] buffer = new byte[bufferSize];
            int bIndex = 0;

            ClassicProtocol s = dst.session;
            byte[] blocks     = lvl.blocks;
            float progScale   = 100.0f / blocks.Length;

            // Store on stack instead of performing function call for every block in map
            byte* conv = stackalloc byte[Block.SUPPORTED_COUNT];
            byte* convExt  = conv + 256; // 256 blocks per group/class
            #if TEN_BIT_BLOCKS
            byte* convExt2 = conv + 256 * 2;
            byte* convExt3 = conv + 256 * 3;
            #endif

            for (int j = 0; j < Block.SUPPORTED_COUNT; j++) {
                conv[j] = (byte)s.ConvertBlock((BlockID)j);
            }
            
            // compress the map data in 64 kb chunks
            #if TEN_BIT_BLOCKS
            if (s.hasExtBlocks) {
                // Initially assume all custom blocks are <= 255
                int i;
                for (i = 0; i < blocks.Length; i++) 
                {
                    byte block = blocks[i];
                    if (block == Block.custom_block) {
                        buffer[bIndex] = lvl.GetExtTile(i);
                    } else if (block == Block.custom_block_2) {
                        break;
                    } else if (block == Block.custom_block_3) {
                        break;
                    } else {
                        buffer[bIndex] = conv[block];
                    }
                    
                    bIndex++;
                    if (bIndex == bufferSize) {
                        stream.Write(buffer, 0, bufferSize);
                        bIndex = 0;
                    }
                }
                
                // Check if map only used custom blocks <= 255
                if (bIndex > 0) stream.Write(buffer, 0, bIndex);                
                if (i == blocks.Length) return;
                bIndex = 0;
                
                // Nope - have to go slower path now                
                using (LevelChunkStream dst2 = new LevelChunkStream(s))
                    using (Stream stream2 = dst2.CompressMapHeader(blocks.Length))
                {
                    dst2.chunkValue = 1; // 'extended' blocks
                    byte[] buffer2 = new byte[bufferSize];
                    
                    // Need to fill in all the upper 8 bits of blocks before this one with 0
                    for (int j = 0; j < i; j += bufferSize) {
                        int len = Math.Min(bufferSize, i - j);
                        stream2.Write(buffer2, 0, len);
                    }
                    
                    for (; i < blocks.Length; i++) 
                    {
                        byte block = blocks[i];
                        if (block == Block.custom_block) {
                            buffer[bIndex]  = lvl.GetExtTile(i);
                            buffer2[bIndex] = 0;
                        } else if (block == Block.custom_block_2) {
                            buffer[bIndex]  = lvl.GetExtTile(i);
                            buffer2[bIndex] = 1;
                        } else if (block == Block.custom_block_3) {
                            buffer[bIndex]  = lvl.GetExtTile(i);
                            buffer2[bIndex] = 2;
                        } else {
                            buffer[bIndex]  = conv[block];
                            buffer2[bIndex] = 0;
                        }
                        
                        bIndex++;
                        if (bIndex == bufferSize) {
                            stream.Write(buffer,   0, bufferSize);
                            stream2.Write(buffer2, 0, bufferSize);
                            bIndex = 0;
                        }
                    }
                    if (bIndex > 0) stream2.Write(buffer2, 0, bIndex);
                }
            } else {
                for (int i = 0; i < blocks.Length; i++) 
                {
                    byte block = blocks[i];
                    if (block == Block.custom_block) {
                        buffer[bIndex] = convExt[lvl.GetExtTile(i)];
                    } else if (block == Block.custom_block_2) {
                        buffer[bIndex] = convExt2[lvl.GetExtTile(i)];
                    } else if (block == Block.custom_block_3) {
                        buffer[bIndex] = convExt3[lvl.GetExtTile(i)];
                    } else {
                        buffer[bIndex] = conv[block];
                    }
                    
                    bIndex++;
                    if (bIndex == bufferSize) {
                        dst.chunkValue = (byte)(i * progScale);
                        stream.Write(buffer, 0, bufferSize); bIndex = 0;
                    }
                }
            }
            #else
            if (s.hasBlockDefs) {
                for (int i = 0; i < blocks.Length; i++) 
                {
                    byte block = blocks[i];
                    if (block == Block.custom_block) {
                        buffer[bIndex] = lvl.GetExtTile(i);
                    } else {
                        buffer[bIndex] = conv[block];
                    }
                    
                    bIndex++;
                    if (bIndex == bufferSize) {
                        dst.chunkValue = (byte)(i * progScale);
                        stream.Write(buffer, 0, bufferSize); bIndex = 0;
                    }
                }
            } else {
                for (int i = 0; i < blocks.Length; i++) 
                {
                    byte block = blocks[i];
                    if (block == Block.custom_block) {
                        buffer[bIndex] = convExt[lvl.GetExtTile(i)];
                    } else {
                        buffer[bIndex] = conv[block];
                    }
                    
                    bIndex++;
                    if (bIndex == bufferSize) {
                        dst.chunkValue = (byte)(i * progScale);
                        stream.Write(buffer, 0, bufferSize); bIndex = 0;
                    }
                }
            }
            #endif
            
            if (bIndex > 0) stream.Write(buffer, 0, bIndex);
        }
    }
}
