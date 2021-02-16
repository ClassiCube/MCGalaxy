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
using BlockID = System.UInt16;
using BlockRaw = System.Byte;

namespace MCGalaxy.Network {
    /// <summary> Helper class for efficiently sending many block changes. </summary>
    /// <remarks> Sends block changes as either a single CPE BulkBlockUpdate packet,
    /// or 256 SetBlock packets combined as a single byte array to reduce overhead. </remarks>
    public sealed class BufferedBlockSender {
        
        int[] indices = new int[256];
        BlockID[] blocks = new BlockID[256];
        int count = 0;
        public Level level;
        public Player player;
        public BufferedBlockSender() { }
        
        /// <summary> Constructs a bulk sender that will send block changes to all players on that level. </summary>
        public BufferedBlockSender(Level level) {
            this.level = level;
        }
        
        /// <summary> Constructs a bulk sender that will only send block changes to that player. </summary>
        public BufferedBlockSender(Player player) {
            this.player = player;
            this.level  = player.level;
        }
        
        /// <summary> Adds a block change to list of buffered changes. </summary>
        /// <remarks> When buffer limit is reached, calls Flush(), resetting buffered list. </remarks>
        public void Add(int index, BlockID block) {
            indices[count] = index;
            if (Block.IsPhysicsType(block)) {
                blocks[count] = Block.Convert(block);
            } else {
                blocks[count] = block;
            }
            
            count++;
            if (count == 256) Flush();
        }
        
        /// <summary> Sends buffered block changes to target player(s). </summary>
        public void Flush() {
            if (count == 0) return;
            
            if (player != null) SendPlayer();
            else SendLevel();
            count = 0;
        }
        
        void SendLevel() {
            byte[] bulk = null, normal = null, noBlockDefs = null, original = null, ext = null, extBulk = null;
            Player[] players = PlayerInfo.Online.Items;
            foreach (Player p in players) {
                if (p.level != level) continue;
                byte[] packet = MakePacket(p, ref bulk, ref normal,
                                           ref noBlockDefs, ref original, ref ext, ref extBulk);
                p.Socket.Send(packet, SendFlags.LowPriority);
            }
        }
        
        void SendPlayer() {
            byte[] bulk = null, normal = null, noBlockDefs = null, original = null, ext = null, extBulk = null;
            byte[] packet = MakePacket(player, ref bulk, ref normal,
                                       ref noBlockDefs, ref original, ref ext, ref extBulk);
            player.Socket.Send(packet, SendFlags.LowPriority);
        }
        
        #region Packet construction
        
        byte[] MakePacket(Player p, ref byte[] bulk, ref byte[] normal,
                          ref byte[] noBlockDefs, ref byte[] original, ref byte[] ext, ref byte[] extBulk) {
            #if TEN_BIT_BLOCKS
            if (p.hasExtBlocks) {
                if (p.hasBulkBlockUpdate && count >= 150) {
                    if (extBulk == null) extBulk = MakeBulkExt();
                    return extBulk;
                } else {
                    if (ext == null) ext = MakeExt();
                    return ext;
                }
            }
            #endif
            
            // Different clients support varying types of blocks
            if (p.hasBulkBlockUpdate && p.hasCustomBlocks && p.hasBlockDefs && count >= 160) {
                if (bulk == null) bulk = MakeBulk();
                return bulk;
            } else if (p.hasCustomBlocks && p.hasBlockDefs) {
                if (normal == null) normal = MakeNormal();
                return normal;
            } else if (p.hasCustomBlocks) {
                if (noBlockDefs == null) noBlockDefs = MakeNoBlockDefs();
                return noBlockDefs;
            } else {
                if (original == null) original = MakeOriginalOnly();
                return original;
            }
        }

        #if TEN_BIT_BLOCKS
        byte[] MakeBulkExt() {
            byte[] data = new byte[2 + 256 * 5 + (256 / 4)];
            data[0] = Opcode.CpeBulkBlockUpdate;
            data[1] = (byte)(count - 1);
            
            for (int i = 0, j = 2; i < count; i++) {
                int index = indices[i];
                data[j++] = (byte)(index >> 24); data[j++] = (byte)(index >> 16);
                data[j++] = (byte)(index >> 8);  data[j++] = (byte)index;
            }
            
            for (int i = 0, j = 2 + 256 * sizeof(int); i < count; i++) {
                data[j++] = (BlockRaw)blocks[i];
            }
            
            for (int i = 0, j = 2 + 256 * (sizeof(int) + 1); i < count; i += 4) {
                int flags = 0;
                flags |= (Block.ToRaw(blocks[i    ]) >> 8) & 0x03;
                flags |= (Block.ToRaw(blocks[i + 1]) >> 6) & 0x0C;
                flags |= (Block.ToRaw(blocks[i + 2]) >> 4) & 0x30;
                flags |= (Block.ToRaw(blocks[i + 3]) >> 2) & 0xC0;
                data[j++] = (byte)flags;
            }
            return data;
        }
        
        byte[] MakeExt() {
            byte[] data = new byte[count * 9];
            for (int i = 0, j = 0; i < count; i++) {
                int index = indices[i];
                int x = (index % level.Width);
                int y = (index / level.Width) / level.Length;
                int z = (index / level.Width) % level.Length;
                
                data[j++] = Opcode.SetBlock;
                data[j++] = (byte)(x >> 8); data[j++] = (byte)x;
                data[j++] = (byte)(y >> 8); data[j++] = (byte)y;
                data[j++] = (byte)(z >> 8); data[j++] = (byte)z;
                BlockID raw = Block.ToRaw(blocks[i]);
                data[j++] = (byte)(raw >> 8);
                data[j++] = (byte)raw;
            }
            return data;
        }
        #endif

        
        byte[] MakeBulk() {
            byte[] data = new byte[2 + 256 * 5];
            data[0] = Opcode.CpeBulkBlockUpdate;
            data[1] = (byte)(count - 1);
            for (int i = 0, j = 2; i < count; i++) {
                int index = indices[i];
                data[j++] = (byte)(index >> 24); data[j++] = (byte)(index >> 16);
                data[j++] = (byte)(index >> 8);  data[j++] = (byte)index;
            }
            for (int i = 0, j = 2 + 256 * sizeof(int); i < count; i++) {
                #if TEN_BIT_BLOCKS
                BlockID block = blocks[i];
                data[j++] = block <= 511 ? (BlockRaw)block : level.GetFallback(block);
                #else
                data[j++] = (BlockRaw)blocks[i];
                #endif
            }
            return data;
        }
        
        byte[] MakeNormal() {
            byte[] data = new byte[count * 8];
            for (int i = 0, j = 0; i < count; i++) {
                int index = indices[i];
                int x = (index % level.Width);
                int y = (index / level.Width) / level.Length;
                int z = (index / level.Width) % level.Length;
                
                data[j++] = Opcode.SetBlock;
                data[j++] = (byte)(x >> 8); data[j++] = (byte)x;
                data[j++] = (byte)(y >> 8); data[j++] = (byte)y;
                data[j++] = (byte)(z >> 8); data[j++] = (byte)z;
                #if TEN_BIT_BLOCKS
                BlockID block = blocks[i];
                data[j++] = block <= 511 ? (BlockRaw)block : level.GetFallback(block);
                #else
                data[j++] = (BlockRaw)blocks[i];
                #endif
            }
            return data;
        }
        
        byte[] MakeNoBlockDefs() {
            byte[] data = new byte[count * 8];
            for (int i = 0, j = 0; i < count; i++) {
                int index = indices[i];
                int x = (index % level.Width);
                int y = (index / level.Width) / level.Length;
                int z = (index / level.Width) % level.Length;
                
                data[j++] = Opcode.SetBlock;
                data[j++] = (byte)(x >> 8); data[j++] = (byte)x;
                data[j++] = (byte)(y >> 8); data[j++] = (byte)y;
                data[j++] = (byte)(z >> 8); data[j++] = (byte)z;
                data[j++] = level.GetFallback(blocks[i]);
            }
            return data;
        }
        
        byte[] MakeOriginalOnly() {
            byte[] data = new byte[count * 8];
            for (int i = 0, j = 0; i < count; i++) {
                int index = indices[i];
                int x = (index % level.Width);
                int y = (index / level.Width) / level.Length;
                int z = (index / level.Width) % level.Length;
                
                data[j++] = Opcode.SetBlock;
                data[j++] = (byte)(x >> 8); data[j++] = (byte)x;
                data[j++] = (byte)(y >> 8); data[j++] = (byte)y;
                data[j++] = (byte)(z >> 8); data[j++] = (byte)z;
                data[j++] = Block.ConvertCPE(level.GetFallback(blocks[i]));
            }
            return data;
        }
        #endregion
    }
}
