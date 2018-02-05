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

namespace MCGalaxy.Network {
    /// <summary> Combines block changes and sends them as either a single CPE BulkBlockUpdate packet,
    /// or 256 SetBlock packets combined as a single byte array to reduce overhead. </summary>
    public sealed class BufferedBlockSender {
        
        int[] indices = new int[256];
        byte[] types = new byte[256];
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
            this.level = player.level;
        }
        
        /// <summary> Adds a block change, and potentially sends block change packets if 
        /// number of buffered block changes has reached the limit. </summary>
        /// <returns> Whether block change packets were actually sent. </returns>
        public bool Add(int index, BlockID block) {
            indices[count] = index;
            if (block == Block.custom_block) types[count] = extBlock;
            else types[count] = Block.Convert(block);
            count++;
            return Send(false);
        }
        
        /// <summary> Sends the block change packets if either 'force' is true, 
        /// or the number of buffered block changes has reached the limit. </summary>
        /// <returns> Whether block change packets were actually sent. </returns>
        public bool Send(bool force) {
            if (count > 0 && (force || count == 256)) {
                if (player != null) SendPlayer();
                else SendLevel();
                count = 0;
                return true;
            }
            return false;
        }
        
        void SendLevel() {
            byte[] bulk = null, normal = null, noBlockDefs = null, original = null;
            Player[] players = PlayerInfo.Online.Items;
            foreach (Player p in players) {
                if (p.level != level) continue;
                byte[] packet = MakePacket(p, ref bulk, ref normal,
                                           ref noBlockDefs, ref original);
                p.Socket.SendLowPriority(packet);
            }
        }
        
        void SendPlayer() {
            byte[] bulk = null, normal = null, noBlockDefs = null, original = null;
            byte[] packet = MakePacket(player, ref bulk, ref normal,
                                       ref noBlockDefs, ref original);
            player.Socket.SendLowPriority(packet);
        }
        
        #region Packet construction
        
        byte[] MakePacket(Player p, ref byte[] bulk, ref byte[] normal,
                          ref byte[] noBlockDefs, ref byte[] original) {
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
        
        byte[] MakeBulk() {
            byte[] data = new byte[2 + 256 * 5];
            data[0] = Opcode.CpeBulkBlockUpdate;
            data[1] = (byte)(count - 1);
            for (int i = 0, j = 2; i < count; i++) {
                int index = indices[i];
                data[j++] = (byte)(index >> 24); data[j++] = (byte)(index >> 16);
                data[j++] = (byte)(index >> 8); data[j++] = (byte)index;
            }
            for (int i = 0, j = 2 + 256 * sizeof(int); i < count; i++)
                data[j++] = types[i];
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
                data[j++] = types[i];
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
                data[j++] = level.RawFallback(types[i]);
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
                data[j++] = Block.ConvertCPE(level.RawFallback(types[i]));
            }
            return data;
        }
        #endregion
    }
}
