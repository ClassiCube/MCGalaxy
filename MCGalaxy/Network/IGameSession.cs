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
    /// <remarks> Generally, you should manipulate a session through wrapper methods in the Player class instead </remarks>
    public abstract class IGameSession : INetProtocol
    {
        public byte ProtocolVersion;
        public byte[] fallback = new byte[256]; // fallback for classic+CPE block IDs
        public BlockID MaxRawBlock = Block.CLASSIC_MAX_BLOCK;
        public bool hasCpe;
        public string appName;

        // these are checked very frequently, so avoid overhead of .Supports(
        public bool hasCustomBlocks, hasExtBlocks, hasBlockDefs, hasBulkBlockUpdate;
        protected INetSocket socket;
        protected Player player;
        /// <summary> Temporary unique ID for this network session </summary>
        public int ID;

        public PingList Ping = new PingList();

        public int ProcessReceived(byte[] buffer, int bufferLen) {
            int read = 0;
            try {
                while (read < bufferLen) {
                    int packetLen = HandlePacket(buffer, read, bufferLen - read);
                    // Partial packet received
                    if (packetLen == 0) break;
                    
                    // Packet processed, onto next
                    read += packetLen;
                }
            } catch (Exception ex) {
                Logger.LogError(ex);
            }
            return read;
        }
        
        public void Disconnect() { player.Disconnect(); }
        
        
        /// <summary> Sends raw data to the client </summary>
        public void Send(byte[] data) { socket.Send(data, SendFlags.None); }
        /// <summary> Whether the client supports the given CPE extension </summary>
        public abstract bool Supports(string extName, int version);
        /// <summary> Attempts to process the next packet received from the client </summary>
        /// <returns> 0 if insufficient data left to fully process the next packet,
        /// otherwise returns the number of bytes processed </returns>
        protected abstract int HandlePacket(byte[] buffer, int offset, int left);
        
        /// <summary> Sends a ping packet to the client </summary>
        public abstract void SendPing();
        public abstract void SendMotd(string motd);
        /// <summary> Sends chat to the client </summary>
        /// <remarks> Performs line wrapping if chat message is too long to fit in a single packet </remarks>
        public abstract void SendChat(string message);
        /// <summary> Sends a message packet to the client </summary>
        public abstract void SendMessage(CpeMessageType type, string message);
        /// <summary> Sends a kick/disconnect packet with the given reason </summary>
        public abstract void SendKick(string reason, bool sync);
        public abstract bool SendSetUserType(byte type);
        
        /// <summary> Sends an entity teleport (absolute location update) packet to the client </summary>
        public abstract void SendTeleport(byte id, Position pos, Orientation rot);
        /// <summary> Sends an ext entity teleport with more control over behavior </summary>
        public virtual bool SendTeleport(byte id, Position pos, Orientation rot,
                                         Packet.TeleportMoveMode moveMode, bool usePos = true, bool interpolateOri = false, bool useOri = true) { return false; }
        /// <summary> Sends a spawn/add entity packet to the client </summary>
        public abstract void SendSpawnEntity(byte id, string name, string skin, Position pos, Orientation rot);
        /// <summary> Sends a despawn/remove entity to the client </summary>
        public abstract void SendRemoveEntity(byte id);
        public abstract void SendSetSpawnpoint(Position pos, Orientation rot);
        
        public abstract void SendAddTabEntry(byte id, string name, string nick, string group, byte groupRank);
        public abstract void SendRemoveTabEntry(byte id);
        /// <summary> Sends a set reach/click distance packet to the client </summary>
        public abstract bool SendSetReach(float reach);
        /// <summary> Sends a set held block packet to the client </summary>
        public abstract bool SendHoldThis(BlockID block, bool locked);
        /// <summary> Sends an update environment color packet to the client </summary>
        public abstract bool SendSetEnvColor(byte type, string hex);
        public abstract void SendChangeModel(byte id, string model);
        /// <summary> Sends an update weather packet </summary>
        public abstract bool SendSetWeather(byte weather);
        /// <summary> Sends an update text color code packet to the client </summary>
        public abstract bool SendSetTextColor(ColorDesc color);
        /// <summary> Sends a define custom block packet to the client </summary>
        public abstract bool SendDefineBlock(BlockDefinition def);
        /// <summary> Sends an undefine custom block packet to the client </summary>
        public abstract bool SendUndefineBlock(BlockDefinition def);

        /// <summary> Sends a level to the client </summary>
        public abstract void SendLevel(Level prev, Level level);
        /// <summary> Sends a block change/update packet to the client </summary>
        public abstract void SendBlockchange(ushort x, ushort y, ushort z, BlockID block);
        
        public abstract byte[] MakeBulkBlockchange(BufferedBlockSender buffer);
        /// <summary> Gets the name of the software the client is using </summary>
        /// <example> ClassiCube, Classic 0.0.16, etc </example>
        public abstract string ClientName();
        public abstract void UpdatePlayerPositions();
        
        /// <summary> Converts the given block ID into a raw block ID that the client supports </summary>
        public virtual BlockID ConvertBlock(BlockID block) {
            BlockID raw;
            Player p = player;

            if (block >= Block.Extended) {
                raw = Block.ToRaw(block);
            } else {
                raw = Block.Convert(block);
                // show invalid physics blocks as Orange
                if (raw >= Block.CPE_COUNT) raw = Block.Orange;
            }
            if (raw > MaxRawBlock) raw = p.level.GetFallback(block);
            
            // Check if a custom block replaced a core block
            //  If so, assume fallback is the better block to display
            if (!hasBlockDefs && raw < Block.CPE_COUNT) {
                BlockDefinition def = p.level.CustomBlockDefs[raw];
                if (def != null) raw = def.FallBack;
            }
            
            if (!hasCustomBlocks) raw = fallback[(byte)raw];
            return raw;
        }
    }
}
