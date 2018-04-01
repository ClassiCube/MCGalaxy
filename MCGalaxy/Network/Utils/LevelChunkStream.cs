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

namespace MCGalaxy.Network {
    
    /// <summary> Streams the compressed form of a map directly to a player's network socket. </summary>
    public sealed class LevelChunkStream : Stream {
        public override bool CanRead { get { return false; } }
        public override bool CanSeek { get { return false; } }
        public override bool CanWrite { get { return true; } }
        
        static Exception ex = new NotSupportedException("Stream does not support length/seeking.");
        public override void Flush() { }
        public override long Length { get { throw ex; } }
        public override long Position { get { throw ex; } set { throw ex; } }
        public override int Read(byte[] buffer, int offset, int count) { throw ex; }
        public override long Seek(long offset, SeekOrigin origin) { throw ex; }
        public override void SetLength(long length) { throw ex; }
        
        int index;
        byte chunkValue;
        Player p;
        byte[] data = new byte[chunkSize + 4];
        const int chunkSize = 1024;
        public LevelChunkStream(Player p) { this.p = p; }
        
        public override void Close() {
            if (index > 0) WritePacket();
            p = null;
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
            p.Send(data);
            index = 0;
        }
        
        
        static Stream CompressMapHeader(Player player, int volume, LevelChunkStream dst) {
            Stream stream = null;
            if (player.Supports(CpeExt.FastMap)) {
                stream = new DeflateStream(dst, CompressionMode.Compress, true);
            } else {
                stream = new GZipStream(dst, CompressionMode.Compress, true);
                byte[] buffer = new byte[4]; NetUtils.WriteI32(volume, buffer, 0);
                stream.Write(buffer, 0, sizeof(int));
            }
            return stream;
        }
        
        public unsafe static void CompressMap(Player p, LevelChunkStream dst) {
            const int bufferSize = 64 * 1024;
            byte[] buffer = new byte[bufferSize];
            int bIndex = 0;
            
            // Store on stack instead of performing function call for every block in map
            byte* conv = stackalloc byte[Block.ExtendedCount];
            byte* convExt = conv + Block.Count;
            #if TEN_BIT_BLOCKS
            byte* convExt2 = conv + Block.Count * 2;
            byte* convExt3 = conv + Block.Count * 3;
            #endif

            for (int b = 0; b < Block.ExtendedCount; b++) {
                BlockID block = (BlockID)b;
                if (Block.IsPhysicsType(block)) {
                    conv[b] = (byte)Block.Convert(block);
                    if (conv[b] > Block.CpeCount) conv[b] = Block.Orange;
                    continue;
                }
                
                // Map [256, 256 + 65] to [0, 65]
                if (block >= Block.Count && block < Block.Count + Block.CpeCount) block -= Block.Count;
                
                // Players with block defs don't need fallbacks for first 256 blocks
                if (block < Block.Count && p.hasBlockDefs) { conv[b] = (byte)block; continue; }
                    
                // Use custom block fallback, if possible
                BlockDefinition def = p.level.CustomBlockDefs[block];
                if (def == null) {
                    conv[b] = block < Block.CpeCount ? (byte)block : Block.Air;
                } else {
                    conv[b] = def.FallBack;
                }
            }
            
            // Convert CPE blocks to their fallbacks
            if (!p.hasCustomBlocks) {
                for (int b = 0; b < Block.ExtendedCount; b++) {
                    conv[b] = Block.ConvertCPE(conv[b]);
                }
            }
            
            Level lvl = p.level;
            bool hasBlockDefs = p.hasBlockDefs;
            byte[] blocks = lvl.blocks;
            float progScale = 100.0f / blocks.Length;
            
            using (Stream stream = CompressMapHeader(p, blocks.Length, dst)) {
                // compress the map data in 64 kb chunks
                #if TEN_BIT_BLOCKS
                if (p.hasExtBlocks) {
                    for (int i = 0; i < blocks.Length; ++i) {
                        byte block = blocks[i];
                        if (block == Block.custom_block) {
                            buffer[bIndex] = lvl.GetExtTile(i);
                            buffer[bIndex + 1] = 0;
                        } else if (block == Block.custom_block_2) {
                            buffer[bIndex] = lvl.GetExtTile(i);
                            buffer[bIndex + 1] = 1;
                        } else if (block == Block.custom_block_3) {
                            buffer[bIndex] = lvl.GetExtTile(i);
                            buffer[bIndex + 1] = 2;
                        } else {
                            buffer[bIndex] = conv[block];
                            buffer[bIndex + 1] = 0;
                        }
                        
                        bIndex += 2;
                        if (bIndex == bufferSize) {
                            dst.chunkValue = (byte)(i * progScale);
                            stream.Write(buffer, 0, bufferSize); bIndex = 0;
                        }
                    }
                } else if (p.hasBlockDefs) {
                    for (int i = 0; i < blocks.Length; ++i) {
                        byte block = blocks[i];
                        if (block == Block.custom_block) {
                            buffer[bIndex] = lvl.GetExtTile(i);
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
                } else {
                    for (int i = 0; i < blocks.Length; ++i) {
                        byte block = blocks[i];
                        if (block == Block.custom_block) {
                            block = convExt[lvl.GetExtTile(i)];
                        } else if (block == Block.custom_block_2) {
                            block = convExt2[lvl.GetExtTile(i)];
                        } else if (block == Block.custom_block_3) {
                            block = convExt3[lvl.GetExtTile(i)];
                        }
                        buffer[bIndex] = conv[block];
                        
                        bIndex++;
                        if (bIndex == bufferSize) {
                            dst.chunkValue = (byte)(i * progScale);
                            stream.Write(buffer, 0, bufferSize); bIndex = 0;
                        }
                    }
                }
                #else
                if (p.hasBlockDefs) {
                    for (int i = 0; i < blocks.Length; ++i) {
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
                    for (int i = 0; i < blocks.Length; ++i) {
                        byte block = blocks[i];
                        if (block == Block.custom_block) {
                            block = convExt[lvl.GetExtTile(i)];
                        }
                        buffer[bIndex] = conv[block];
                        
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
}
