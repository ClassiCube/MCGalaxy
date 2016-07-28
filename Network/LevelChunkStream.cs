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
using System.Threading;

namespace MCGalaxy {
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
        
        internal int index, position, length;
        Player p;
        byte[] data = new byte[chunkSize + 4];        
        const int chunkSize = 1024;
        
        public LevelChunkStream(Player p) {
            this.p = p;
        }
        
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
            data[1027] = (byte)(100 * (float)position / length);
            p.SendRaw(data);
            index = 0;
        }
    }
}
