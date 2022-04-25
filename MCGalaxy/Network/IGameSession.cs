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

namespace MCGalaxy.Network
{
    /// <summary> Abstracts a network session with a client supporting a particular game protocol </summary>
    /// <remarks> By default only the Minecraft Classic game protocol is supported </remarks>
    public abstract class IGameSession
    {
        public byte ProtocolVersion;
        internal byte[] fallback = new byte[256]; // fallback for classic+CPE block IDs
        public BlockID MaxRawBlock = Block.CLASSIC_MAX_BLOCK;
        public bool hasCpe;

        // these are checked very frequently, so avoid overhead of .Supports(
        public bool hasCustomBlocks, hasExtBlocks, hasBlockDefs, hasBulkBlockUpdate;
        protected INetSocket socket;

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
        
        
        /// <summary> Sends raw data to the client </summary>
        public void Send(byte[] data) { socket.Send(data, SendFlags.None); }
        /// <summary> Attempts to process the next packet received from the client </summary>
        /// <returns><br/>
        /// -1 = the client got disconnected (e.g. sent invalid packet)<br/>
        ///  0 = insufficient data left to fully process the next packet<br/>
        /// Otherwise returns the number of bytes processed
        /// </returns>
        protected abstract int HandlePacket(byte[] buffer, int offset, int left);
        /// <summary> Whether the client supports the given CPE extension </summary>
        public abstract bool Supports(string extName, int version);
        
        /// <summary> Sends a ping packet to the client </summary>
        public abstract void SendPing();
        public abstract void SendMotd(string motd);
        public abstract void SendChat(string message);
        public abstract void SendMessage(CpeMessageType type, string message);
        /// <summary> Sends a kick/disconnect packet with the given reason to the client </summary>
        public abstract void SendKick(string reason, bool sync);
        public abstract bool SendSetUserType(byte type);
        
        public abstract void SendTeleport(byte id, Position pos, Orientation rot);
        public abstract void SendSpawnEntity(byte id, string name, string skin, Position pos, Orientation rot);
        public abstract void SendRemoveEntity(byte id);
        public abstract void SendSetSpawnpoint(Position pos, Orientation rot);
        
        public abstract void SendAddTabEntry(byte id, string name, string nick, string group, byte groupRank);
        public abstract void SendRemoveTabEntry(byte id);
        public abstract bool SendSetReach(float reach);
        public abstract bool SendHoldThis(ushort block, bool locked);
        public abstract bool SendSetEnvColor(byte type, string hex);
        public abstract void SendChangeModel(byte id, string model);
        public abstract bool SendSetWeather(byte weather);
        public abstract bool SendSetTextColor(ColorDesc color);
        public abstract bool SendDefineBlock(BlockDefinition def);
        public abstract bool SendUndefineBlock(BlockDefinition def);

        public abstract void SendLevel(Level prev, Level level);
        public abstract void SendBlockchange(ushort x, ushort y, ushort z, ushort block);
        
        public abstract byte[] MakeBulkBlockchange(BufferedBlockSender buffer);
        /// <summary> Converts the given block ID into a raw block ID that the client supports </summary>
        public abstract BlockID ConvertBlock(BlockID block);
        /// <summary> Gets the name of the software the client is using </summary>
        /// <example> ClassiCube, Classic 0.0.16, etc </example>
        public abstract string ClientName();
        public abstract void UpdatePlayerPositions();
    }
}
