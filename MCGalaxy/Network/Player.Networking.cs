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
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using MCGalaxy.Events;
using MCGalaxy.Events.PlayerEvents;
using MCGalaxy.Network;
using BlockID = System.UInt16;
using BlockRaw = System.Byte;

namespace MCGalaxy {
    public partial class Player : IDisposable, INetProtocol {

        public bool hasCpe, finishedCpeLogin = false;
        public string appName;
        int extensionCount;
        
        int INetProtocol.ProcessReceived(byte[] buffer, int bufferLen) {
            int processedLen = 0;
            try {
                while (processedLen < bufferLen) {
                    int packetLen = PacketSize(buffer[processedLen]);
                    if (packetLen == -1) return bufferLen;
                    
                    // Partial packet data received
                    if (processedLen + packetLen > bufferLen) return processedLen;
                    HandlePacket(buffer, processedLen);
                    processedLen += packetLen;
                }
            } catch (Exception ex) {
                Logger.LogError(ex);
            }
            return processedLen;
        }
        
        int PacketSize(byte opcode) {
            switch (opcode) {
                case Opcode.Handshake:      return 1 + 1 + 64 + 64 + 1;
                case Opcode.SetBlockClient: return 1 + 6 + 1 + (hasExtBlocks ? 2 : 1);
                case Opcode.EntityTeleport: return 1 + 6 + 2 + (hasExtPositions ? 6 : 0) + (hasExtBlocks ? 2 : 1);
                case Opcode.Message:        return 1 + 1 + 64;
                case Opcode.CpeExtInfo:     return 1 + 64 + 2;
                case Opcode.CpeExtEntry:    return 1 + 64 + 4;
                case Opcode.CpeCustomBlockSupportLevel: return 1 + 1;
                case Opcode.CpePlayerClick: return 1 + 1 + 1 + 2 + 2 + 1 + 2 + 2 + 2 + 1;
                case Opcode.Ping:           return 1;
                case Opcode.CpeTwoWayPing:  return 1 + 1 + 2;

                default:
                    Leave("Unhandled opcode \"" + opcode + "\"!", true);
                    return -1;
            }
        }
        
        void HandlePacket(byte[] buffer, int offset) {
            switch (buffer[offset]) {
                case Opcode.Ping: break;
                case Opcode.Handshake:
                    HandleLogin(buffer, offset); break;
                case Opcode.SetBlockClient:
                    HandleBlockchange(buffer, offset); break;
                case Opcode.EntityTeleport:
                    HandleMovement(buffer, offset); break;
                case Opcode.Message:
                    HandleChat(buffer, offset); break;
                case Opcode.CpeExtInfo:
                    HandleExtInfo(buffer, offset); break;
                case Opcode.CpeExtEntry:
                    HandleExtEntry(buffer, offset); break;
                case Opcode.CpeCustomBlockSupportLevel:
                    break; // only ever one level
                case Opcode.CpePlayerClick:
                    HandlePlayerClicked(buffer, offset); break;
                case Opcode.CpeTwoWayPing:
                    HandleTwoWayPing(buffer, offset); break;
            }
        }
        
        #if TEN_BIT_BLOCKS
        BlockID ReadBlock(byte[] buffer, int offset) {
            BlockID block;
            if (hasExtBlocks) {
                block = NetUtils.ReadU16(buffer, offset);
            } else {
                block = buffer[offset];
            }
            
            if (block > Block.MaxRaw) block = Block.MaxRaw;
            return Block.FromRaw(block);
        }
        #else
        BlockID ReadBlock(byte[] buffer, int offset) { return Block.FromRaw(buffer[offset]); }
        #endif
        
        void HandleExtInfo(byte[] buffer, int offset) {
            appName = NetUtils.ReadString(buffer, offset + 1);
            extensionCount = buffer[offset + 66];
            CheckReadAllExtensions(); // in case client supports 0 CPE packets
        }

        void HandleExtEntry(byte[] buffer, int offset) {
            string extName = NetUtils.ReadString(buffer, offset + 1);
            int extVersion = NetUtils.ReadI32(buffer, offset + 65);
            AddExtension(extName, extVersion);
            extensionCount--;
            CheckReadAllExtensions();
        }

        void CheckReadAllExtensions() {
            if (extensionCount <= 0 && !finishedCpeLogin) {
                CompleteLoginProcess();
                finishedCpeLogin = true;
            }
        }
        
        
        public void Send(byte[] buffer)  { Socket.Send(buffer, SendFlags.None); }
        public void Send(byte[] buffer, bool sync) { 
            Socket.Send(buffer, sync ? SendFlags.Synchronous : SendFlags.None);
        }
        
        public void MessageLines(IEnumerable<string> lines) {
            foreach (string line in lines) { Message(line); }
        }
        
        [Obsolete("Use p.Message(message) instead", true)]
        public static void Message(Player p, string message) {
            if (p == null) p = Player.Console;
            p.Message(0, message);
        }
        [Obsolete("Use p.Message(message) instead", true)]
        public static void SendMessage(Player p, string message) { Message(p, message); }
        
        public void Message(string message, object a0) { Message(string.Format(message, a0)); }  
        public void Message(string message, object a0, object a1) { Message(string.Format(message, a0, a1)); }       
        public void Message(string message, object a0, object a1, object a2) { Message(string.Format(message, a0, a1, a2)); }       
        public void Message(string message, params object[] args) { Message(string.Format(message, args)); }
        
        [Obsolete("Use Message(message) instead", true)]
        public void SendMessage(string message) { Message(0, message); } 
        [Obsolete("Use Message(id, message) instead", true)]
        public void SendMessage(byte id, string message) { Message(id, message); }
        public void Message(string message) { Message(0, message); }
        
        // Need to combine chat line packets into one Send call, so that
        // multi-line messages from multiple threads don't interleave
        void SendLines(List<string> lines, byte type) {
            for (int i = 0; i < lines.Count;) {
                // Send buffer max size is 4096 bytes
                // Divide by 66 (size of chat packet) gives ~62 lines
                int count   = Math.Min(62, lines.Count - i);
                byte[] data = new byte[count * 66];
                
                for (int j = 0; j < count; i++, j++) {
                    Packet.WriteMessage(lines[i], type, hasCP437, data, j * 66);
                }
                Send(data);
            }
        }
        
        public virtual void Message(byte type, string message) {
            // Message should start with server color if no initial color
            if (message.Length > 0 && !(message[0] == '&' || message[0] == '%')) message = "&S" + message;
            message = Chat.Format(message, this);
            
            bool cancel = false;
            OnMessageRecievedEvent.Call(this, ref message, ref cancel);
            if (cancel) return;
            
            try {
                message = LineWrapper.CleanupColors(message, this);
                SendLines(LineWrapper.Wordwrap(message, hasEmoteFix), type);
            } catch (Exception e) {
                Logger.LogError(e);
            }
        }
        
        public void SendCpeMessage(CpeMessageType type, string message) {
            if (type != CpeMessageType.Normal && !Supports(CpeExt.MessageTypes)) {
                if (type == CpeMessageType.Announcement) type = CpeMessageType.Normal;
                else return;
            }
            
            message = Chat.Format(message, this);
            message = LineWrapper.CleanupColors(message, this);
            Send(Packet.Message(message, type, hasCP437));
        }

        public void SendMapMotd() {
            string motd = GetMotd();
            motd = Chat.Format(motd, this);
            OnSendingMotdEvent.Call(this, ref motd);
            
            // Change -hax into +hax etc when in Referee mode
            //  (can't just do Replace('-', '+') though, that breaks -push)
            if (Game.Referee) {
                motd = motd
                    .Replace("-hax",  "+hax"  ).Replace("-noclip",  "+noclip")
                    .Replace("-speed","+speed").Replace("-respawn", "+respawn")
                    .Replace("-fly",  "+fly"  ).Replace("-thirdperson", "+thirdperson");
            }
            byte[] packet = Packet.Motd(this, motd);
            Send(packet);
            
            if (!Supports(CpeExt.HackControl)) return;
            Send(Hacks.MakeHackControl(this, motd));
        }

        readonly object joinLock = new object();
        public bool SendRawMap(Level oldLevel, Level level) {
            lock (joinLock)
                return SendRawMapCore(oldLevel, level);
        }
        
        bool SendRawMapCore(Level prev, Level level) {
            if (level.blocks == null) return false;
            bool success = true;
            useCheckpointSpawn = false;
            lastCheckpointIndex = -1;
            
            AFKCooldown = DateTime.UtcNow.AddSeconds(2);
            ZoneIn = null;
            SendMapMotd();
            AllowBuild = level.BuildAccess.CheckAllowed(this);
            
            try {
                int volume = level.blocks.Length;
                if (Supports(CpeExt.FastMap)) {
                    Send(Packet.LevelInitaliseExt(volume));
                } else {
                    Send(Packet.LevelInitalise());
                }
                
                if (hasBlockDefs) {
                    if (prev != null && prev != level) {
                        RemoveOldLevelCustomBlocks(prev);
                    }
                    BlockDefinition.SendLevelCustomBlocks(this);
                    
                    if (Supports(CpeExt.InventoryOrder)) {
                        BlockDefinition.SendLevelInventoryOrder(this);
                    }
                }
                
                using (LevelChunkStream dst = new LevelChunkStream(this))
                    using (Stream stream = LevelChunkStream.CompressMapHeader(this, volume, dst))
                {
                    if (level.MightHaveCustomBlocks()) {
                        LevelChunkStream.CompressMap(this, stream, dst);
                    } else {
                        LevelChunkStream.CompressMapSimple(this, stream, dst);
                    }
                }
                
                // Force players to read the MOTD (clamped to 3 seconds at most)
                if (level.Config.LoadDelay > 0)
                    System.Threading.Thread.Sleep(level.Config.LoadDelay);
                
                byte[] buffer = Packet.LevelFinalise(level.Width, level.Height, level.Length);
                Send(buffer);
                Loading = false;
                
                OnSentMapEvent.Call(this, prev, level);
            } catch (Exception ex) {
                success = false;
                PlayerActions.ChangeMap(this, Server.mainLevel);
                Message("&WThere was an error sending the map, you have been sent to the main level.");
                Logger.LogError(ex);
            } finally {
                Server.DoGC();
            }
            return success;
        }
        
        void RemoveOldLevelCustomBlocks(Level oldLevel) {
            BlockDefinition[] defs = oldLevel.CustomBlockDefs;
            for (int i = 0; i < defs.Length; i++) {
                BlockDefinition def = defs[i];
                if (def == BlockDefinition.GlobalDefs[i] || def == null) continue;
                
                if (def.RawID > MaxRawBlock) continue;
                Send(Packet.UndefineBlock(def, hasExtBlocks));
            }
        }
        
        /// <summary> Sends a packet indicating an absolute position + orientation change for an enity. </summary>
        public void SendPos(byte id, ushort x, ushort y, ushort z, byte rotx, byte roty) {
            SendPos(id, new Position(x, y, z), new Orientation(rotx, roty));
        }
        
        /// <summary> Sends a packet indicating an absolute position + orientation change for an enity. </summary>
        public void SendPos(byte id, Position pos, Orientation rot) {
            if (id == Entities.SelfID) {
                Pos = pos; SetYawPitch(rot.RotY, rot.HeadX);
                pos.Y -= 22;  // NOTE: Fix for standard clients
            }           
            Send(Packet.Teleport(id, pos, rot, hasExtPositions));
        }
        
        public void SendBlockchange(ushort x, ushort y, ushort z, BlockID block) {
            //if (x < 0 || y < 0 || z < 0) return;
            if (x >= level.Width || y >= level.Height || z >= level.Length) return;

            byte[] buffer = new byte[hasExtBlocks ? 9 : 8];
            buffer[0] = Opcode.SetBlock;
            NetUtils.WriteU16(x, buffer, 1);
            NetUtils.WriteU16(y, buffer, 3);
            NetUtils.WriteU16(z, buffer, 5);
            
            BlockID raw = ConvertBlock(block);
            NetUtils.WriteBlock(raw, buffer, 7, hasExtBlocks);
            Socket.Send(buffer, SendFlags.LowPriority);
        }
        
        
        public BlockID ConvertBlock(BlockID block) {
            BlockID raw;
            if (block >= Block.Extended) {
                raw = Block.ToRaw(block);
            } else {
                raw = Block.Convert(block);
                if (raw >= Block.CpeCount) raw = Block.Orange;
            }
            if (raw > MaxRawBlock) raw = level.GetFallback(block);
            
            // Custom block replaced a core block
            if (!hasBlockDefs && raw < Block.CpeCount) {
                BlockDefinition def = level.CustomBlockDefs[raw];
                if (def != null) raw = def.FallBack;
            }
            
            if (!hasCustomBlocks) raw = Block.ConvertCPE((BlockRaw)raw);
            return raw;
        }
    }
}
