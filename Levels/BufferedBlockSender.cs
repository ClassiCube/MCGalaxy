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

namespace MCGalaxy {
    
    public sealed class BufferedBlockSender {
        
        int[] indices = new int[256];
        byte[] types = new byte[256];
        int count = 0;
        Level level;
        
        public BufferedBlockSender(Level level) {
            this.level = level;
        }
        
        public void Add(int index, byte type, byte extType) {
            indices[count] = index;
            if (type == Block.custom_block) types[count] = extType;
            else types[count] = Block.Convert(type);
            count++;
        }
        
        public void CheckIfSend(bool force) {
            if (count > 0 && (force || count == 256)) {
                byte[] bulk = null, normal = null, noBlockDefs = null, original = null;
                Player[] players = PlayerInfo.Online;
                
                foreach (Player p in players) {
                    if (p.level != level) continue;
                    
                    // Different clients support varying types of blocks
                    byte[] packet = null;
                    if (p.HasCpeExt(CpeExt.BulkBlockUpdate) && p.hasCustomBlocks && p.hasBlockDefs && count >= 160) {
                        if (bulk == null) bulk = MakeBulkPacket();
                        packet = bulk;
                    } else if (p.hasCustomBlocks && p.hasBlockDefs) {
                        if (normal == null) normal = MakeNormalPacket();
                        packet = normal;
                    } else if (p.hasCustomBlocks) {
                        if (noBlockDefs == null) noBlockDefs = MakeNoBlockDefsPacket();
                        packet = noBlockDefs;
                    } else {
                        if (original == null) original = MakeOriginalOnlyPacket();
                        packet = original;
                    }
                    p.SendRaw(packet);
                }
                count = 0;
            }
        }
        
        #region Packet construction
        
        byte[] MakeBulkPacket() {
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
        
        byte[] MakeNormalPacket() {
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
        
        byte[] MakeNoBlockDefsPacket() {
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
                data[j++] = types[i] < Block.CpeCount ? types[i] : level.GetFallback(types[i]);
            }
            return data;
        }
        
        byte[] MakeOriginalOnlyPacket() {
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
                data[j++] = types[i] < Block.CpeCount ? Block.ConvertCPE(types[i])
                    : Block.ConvertCPE(level.GetFallback(types[i]));
            }
            return data;
        }
        #endregion
    }
}
