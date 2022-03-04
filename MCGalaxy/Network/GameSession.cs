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
using MCGalaxy;
using BlockID = System.UInt16;

namespace MCGalaxy.Network
{
    public abstract class GameSession
    {
        public byte ProtocolVersion;
        public BlockID MaxRawBlock = Block.CLASSIC_MAX_BLOCK;

        // these are checked very frequently, so avoid overhead of .Supports(
        public bool hasCustomBlocks, hasExtBlocks, hasBlockDefs, hasBulkBlockUpdate;

        internal byte[] fallback = new byte[256]; // fallback for classic+CPE block IDs
        protected Player player;
        protected INetSocket socket;

        public abstract string ClientName();
        public abstract ushort ConvertBlock(BlockID block);
        public abstract void Disconnect();
        public abstract byte[] MakeBulkBlockchange(BufferedBlockSender buffer);

        public void Send(byte[] data) { socket.Send(data, SendFlags.None); }

        public int ProcessReceived(byte[] buffer, int bufferLen) {
            int read = 0;
            try {
                while (read < bufferLen) {
                    int packetLen = HandlePacket(buffer, read, bufferLen - read);
                    // Partial packet received
                    if (packetLen == 0) break;
                    
                    // Client was forced disconnected
                    if (packetLen == -1) return bufferLen;
                    
                    // Packet processed, onto next
                    read += packetLen;
                }
            } catch (Exception ex) {
                Logger.LogError(ex);
            }
            return read;
        }

        protected abstract int HandlePacket(byte[] buffer, int offset, int left);

        public abstract void SendAddTabEntry(byte id, string name, string nick, string group, byte groupRank);
        public abstract void SendBlockchange(ushort x, ushort y, ushort z, BlockID block);
        public abstract void SendChangeModel(byte id, string model);
        public abstract void SendChat(string message);
        public virtual bool SendDefineBlock(BlockDefinition def) { return false; }
        public virtual bool SendHoldThis(BlockID block, bool locked) { return false; }
        public abstract void SendKick(string reason, bool sync);
        public abstract void SendLevel(Level prev, Level level);
        public abstract void SendMessage(CpeMessageType type, string message);
        public abstract void SendMotd(string motd);
        public abstract void SendPing();
        public abstract void SendRemoveEntity(byte id);
        public abstract void SendRemoveTabEntry(byte id);
        public virtual bool SendSetEnvColor(byte type, string hex) { return false; }
        public virtual bool SendSetReach(float reach) { return false; }
        public abstract void SendSetSpawnpoint(Position pos, Orientation rot);
        public virtual bool SendSetTextColor(ColorDesc color) { return false; }
        public virtual bool SendSetUserType(byte type) { return false; }
        public virtual bool SendSetWeather(byte weather) { return false; }
        public abstract void SendSpawnEntity(byte id, string name, string skin, Position pos, Orientation rot);
        public abstract void SendTeleport(byte id, Position pos, Orientation rot);
        public virtual bool SendUndefineBlock(BlockDefinition def) { return false; }
        public virtual bool Supports(string extName, int version = 1) { return false; }
        public abstract void UpdatePlayerPositions();
    }
}