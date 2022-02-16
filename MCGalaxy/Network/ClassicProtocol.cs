﻿/*
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
using System.Threading;
using MCGalaxy.Events.PlayerEvents;
using MCGalaxy.Events.ServerEvents;
using BlockID = System.UInt16;

namespace MCGalaxy.Network
{
    public abstract class IGameProtocol : INetProtocol
    {
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
        public abstract void Disconnect();
        public abstract void Send(byte[] data);


        public abstract void SendMotd(string motd);
        public abstract void SendPing();
        public abstract void SendSpawnEntity(byte id, string name, string skin, Position pos, Orientation rot);
        public abstract void SendLevel(Level prev, Level level);

        public abstract void SendTeleport(byte id, Position pos, Orientation rot);
        public abstract void SendRemoveEntity(byte id);
        public abstract void SendChat(string message);
        public abstract void SendMessage(CpeMessageType type, string message);
        public abstract void SendKick(string reason, bool sync);
        public abstract bool SendSetUserType(byte type);

        public abstract bool SendSetReach(float reach);
        public abstract bool SendHoldThis(BlockID block, bool locked);
        public abstract bool SendSetEnvColor(byte type, string hex);
        public abstract void SendChangeModel(byte id, string model);
        public abstract bool SendSetWeather(byte weather);
        public abstract void SendDefineBlock(BlockDefinition def);
        public abstract void SendUndefineBlock(BlockDefinition def);
        public abstract bool SendSetTextColor(ColorDesc color);

        /// <summary> Returns an appropriate name for the associated player's client </summary>
        /// <remarks> Determines name based on appname or protocol version supported </remarks>
        public abstract string ClientName();


        public abstract BlockID ConvertBlock(BlockID block);

        public abstract void SendBlockchange(ushort x, ushort y, ushort z, BlockID block);
    }

    public class ClassicProtocol : IGameProtocol
    {
        Player player;
        INetSocket socket;
        int extensionCount;
        bool finishedCpeLogin;
        // these are checked very frequently, so avoid overhead of .Supports(
        bool hasEmoteFix, hasTwoWayPing, hasExtTexs;

        public ClassicProtocol(INetSocket s) {
            socket = s;
            player = new Player(s, this);
        }

        public override void Send(byte[] data) { socket.Send(data, SendFlags.None); }

        protected override int HandlePacket(byte[] buffer, int offset, int left) {
            switch (buffer[offset]) {
                case Opcode.Ping:              return 1;
                case Opcode.Handshake:         return HandleLogin(buffer, offset, left);
                case Opcode.SetBlockClient:    return HandleBlockchange(buffer, offset, left);
                case Opcode.EntityTeleport:    return HandleMovement(buffer, offset, left);
                case Opcode.Message:           return HandleChat(buffer, offset, left);
                case Opcode.CpeExtInfo:        return HandleExtInfo(buffer, offset, left);
                case Opcode.CpeExtEntry:       return HandleExtEntry(buffer, offset, left);
                case Opcode.CpePlayerClick:    return HandlePlayerClicked(buffer, offset, left);
                case Opcode.CpeTwoWayPing:     return HandleTwoWayPing(buffer, offset, left);   
                case Opcode.CpePluginMessage:  return HandlePluginMessage(buffer, offset, left);   
                
                case Opcode.CpeCustomBlockSupportLevel:
                    return left < 2 ? 0 : 2; // only ever one level anyways
                default:
                    player.Leave("Unhandled opcode \"" + buffer[offset] + "\"!", true);
                    return -1;
            }
        }
        
        #if TEN_BIT_BLOCKS
        BlockID ReadBlock(byte[] buffer, int offset) {
            BlockID block;
            if (player.hasExtBlocks) {
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

        public override void Disconnect() { player.Disconnect(); }


#region Classic processing
        int HandleLogin(byte[] buffer, int offset, int left) {
            // protocol versions < 6 didn't have the usertype field,
            //  hence this two-packet-size-handling monstrosity
            const int old_size = 1 + 1 + 64 + 64;
            const int new_size = 1 + 1 + 64 + 64 + 1;
            // the packet must be at least old_size long
            if (left < old_size) return 0;  
            
            player.ProtocolVersion = buffer[offset + 1];
            // check size now that know whether usertype field is included or not
            int size = player.ProtocolVersion >= Server.VERSION_0020 ? new_size : old_size;
            if (left < size) return 0;
            if (player.loggedIn)  return size;

            // usertype field has different meanings depending on protocol version
            //  Version 7 - 0x42 for CPE supporting client, should be 0 otherwise
            //  Version 6 - should be 0
            //  Version 5 - field does not exist
            if (player.ProtocolVersion >= Server.VERSION_0030) {
                player.hasCpe = buffer[offset + 130] == 0x42 && Server.Config.EnableCPE;
            }
            
            string name   = NetUtils.ReadString(buffer, offset +  2);
            string mppass = NetUtils.ReadString(buffer, offset + 66);
            return player.ProcessLogin(name, mppass) ? size : -1;
        }
        
        int HandleBlockchange(byte[] buffer, int offset, int left) {
            int size = 1 + 6 + 1 + (player.hasExtBlocks ? 2 : 1);
            if (left < size) return 0;
            if (!player.loggedIn) return size;

            ushort x = NetUtils.ReadU16(buffer, offset + 1);
            ushort y = NetUtils.ReadU16(buffer, offset + 3);
            ushort z = NetUtils.ReadU16(buffer, offset + 5);

            byte action = buffer[offset + 7];
            if (action > 1) {
                player.Leave("Unknown block action!", true); return -1;
            }

            BlockID held = ReadBlock(buffer, offset + 8);
            player.ProcessBlockchange(x, y, z, action, held);
            return size;
        }
        
        int HandleMovement(byte[] buffer, int offset, int left) {
            int size = 1 + 6 + 2 + (player.hasExtPositions ? 6 : 0) + (player.hasExtBlocks ? 2 : 1);
            if (left < size) return 0;
            if (!player.loggedIn) return size;

            int held = -1;
            if (player.Supports(CpeExt.HeldBlock)) {
                held = ReadBlock(buffer, offset + 1);
                if (player.hasExtBlocks) offset++; // correct offset for position later
            }
            
            int x, y, z;
            if (player.hasExtPositions) {
                x = NetUtils.ReadI32(buffer, offset + 2);
                y = NetUtils.ReadI32(buffer, offset + 6);
                z = NetUtils.ReadI32(buffer, offset + 10);
                offset += 6; // for yaw/pitch offset below
            } else {
                x = NetUtils.ReadI16(buffer, offset + 2);
                y = NetUtils.ReadI16(buffer, offset + 4);
                z = NetUtils.ReadI16(buffer, offset + 6);
            }

            byte yaw   = buffer[offset + 8];
            byte pitch = buffer[offset + 9];
            player.ProcessMovement(x, y, z, yaw, pitch, held);
            return size;
        }
        
        int HandleChat(byte[] buffer, int offset, int left) {
            const int size = 1 + 1 + 64;
            if (left < size) return 0;
            if (!player.loggedIn) return size;

            // In original clasic, this field is 'player ID' and so useless
            // With LongerMessages extension, this field has been repurposed
            bool continued = false;
            if (player.Supports(CpeExt.LongerMessages))
                continued  = buffer[offset + 1] != 0;

            string text = NetUtils.ReadString(buffer, offset + 2);
            player.ProcessChat(text, continued);
            return size;
        }
#endregion


#region CPE processing
        void CheckReadAllExtensions() {
            if (extensionCount <= 0 && !finishedCpeLogin) {
                player.CompleteLoginProcess();
                finishedCpeLogin = true;
            }
        }

        int HandleExtInfo(byte[] buffer, int offset, int left) {
            const int size = 1 + 64 + 2;
            if (left < size) return 0;
            
            player.appName      = NetUtils.ReadString(buffer, offset + 1);
            extensionCount = buffer[offset + 66];
            CheckReadAllExtensions(); // in case client supports 0 CPE packets
            return size;
        }

        int HandleExtEntry(byte[] buffer, int offset, int left) {
            const int size = 1 + 64 + 4;
            if (left < size) return 0;
            
            string extName = NetUtils.ReadString(buffer, offset + 1);
            int extVersion = NetUtils.ReadI32(buffer,    offset + 65);
            AddExtension(extName, extVersion);
            extensionCount--;
            CheckReadAllExtensions();
            return size;
        }

        int HandlePlayerClicked(byte[] buffer, int offset, int left) {
            const int size = 1 + 1 + 1 + 2 + 2 + 1 + 2 + 2 + 2 + 1;
            if (left < size) return 0;
            
            MouseButton Button = (MouseButton)buffer[offset + 1];
            MouseAction Action = (MouseAction)buffer[offset + 2];
            ushort yaw    = NetUtils.ReadU16(buffer, offset + 3);
            ushort pitch  = NetUtils.ReadU16(buffer, offset + 5);
            byte entityID = buffer[offset + 7];
            ushort x = NetUtils.ReadU16(buffer, offset + 8);
            ushort y = NetUtils.ReadU16(buffer, offset + 10);
            ushort z = NetUtils.ReadU16(buffer, offset + 12);
            
            TargetBlockFace face = (TargetBlockFace)buffer[offset + 14];
            if (face > TargetBlockFace.None) face = TargetBlockFace.None;
            OnPlayerClickEvent.Call(player, Button, Action, yaw, pitch, entityID, x, y, z, face);
            return size;
        }
        
        int HandleTwoWayPing(byte[] buffer, int offset, int left) {
            const int size = 1 + 1 + 2;
            if (left < size) return 0;
            
            bool serverToClient = buffer[offset + 1] != 0;
            ushort data = NetUtils.ReadU16(buffer, offset + 2);
            
            if (!serverToClient) {
                // Client-> server ping, immediately send reply.
                Send(Packet.TwoWayPing(false, data));
            } else {
                // Server -> client ping, set time received for reply.
                player.Ping.Update(data);
            }
            return size;
        }

        int HandlePluginMessage(byte[] buffer, int offset, int left) {
            const int size = 1 + 1 + Packet.PluginMessageDataLength;
            if (left < size) return 0;

            byte channel = buffer[offset + 1];
            byte[] data  = new byte[Packet.PluginMessageDataLength];
            Array.Copy(buffer, offset + 2, data, 0, Packet.PluginMessageDataLength);
            OnPluginMessageReceivedEvent.Call(player, channel, data);

            return size;
        }

        internal void AddExtension(string extName, int version) {
            Player p   = player;
            CpeExt ext = p.FindExtension(extName);
            if (ext == null) return;
            ext.ClientVersion = (byte)version;
            
            if (ext.Name == CpeExt.CustomBlocks) {
                if (version == 1) Send(Packet.CustomBlockSupportLevel(1));
                p.hasCustomBlocks = true;

                p.UpdateFallbackTable();
                if (p.MaxRawBlock < Block.CPE_MAX_BLOCK) p.MaxRawBlock = Block.CPE_MAX_BLOCK;
            } else if (ext.Name == CpeExt.ChangeModel) {
                p.hasChangeModel = true;
            } else if (ext.Name == CpeExt.EmoteFix) {
                hasEmoteFix = true;
            } else if (ext.Name == CpeExt.FullCP437) {
                p.hasCP437 = true;
            } else if (ext.Name == CpeExt.ExtPlayerList) {
                p.hasExtList = true;
            } else if (ext.Name == CpeExt.BlockDefinitions) {
                p.hasBlockDefs = true;
                if (p.MaxRawBlock < 255) p.MaxRawBlock = 255;
            } else if (ext.Name == CpeExt.TextColors) {
                p.hasTextColors = true;
                for (int i = 0; i < Colors.List.Length; i++) {
                    if (!Colors.List[i].IsModified()) continue;
                    Send(Packet.SetTextColor(Colors.List[i]));
                }
            } else if (ext.Name == CpeExt.ExtEntityPositions) {
                p.hasExtPositions = true;
            } else if (ext.Name == CpeExt.TwoWayPing) {
                hasTwoWayPing = true;
            } else if (ext.Name == CpeExt.BulkBlockUpdate) {
                p.hasBulkBlockUpdate = true;
            } else if (ext.Name == CpeExt.ExtTextures) {
                hasExtTexs = true;
            }
            #if TEN_BIT_BLOCKS
            else if (ext.Name == CpeExt.ExtBlocks) {
                p.hasExtBlocks = true;
                if (p.MaxRawBlock < 767) p.MaxRawBlock = 767;
            }
            #endif
        }
#endregion


#region Classic packet sending
        public override void SendTeleport(byte id, Position pos, Orientation rot) {
            // NOTE: Classic clients require offseting own entity by 22 units vertically
            if (id == Entities.SelfID) pos.Y -= 22;

            Send(Packet.Teleport(id, pos, rot, player.hasExtPositions));
        }

        public override void SendRemoveEntity(byte id) {
            Send(Packet.RemoveEntity(id));
        }

        public override void SendChat(string message) {
            List<string> lines = LineWrapper.Wordwrap(message, hasEmoteFix);

            // Need to combine chat line packets into one Send call, so that
            // multi-line messages from multiple threads don't interleave
            for (int i = 0; i < lines.Count;) 
            {
                // Send buffer max size is 4096 bytes
                // Divide by 66 (size of chat packet) gives ~62 lines
                int count   = Math.Min(62, lines.Count - i);
                byte[] data = new byte[count * 66];
                
                for (int j = 0; j < count; i++, j++) 
                {
                    Packet.WriteMessage(lines[i], 0, player.hasCP437, data, j * 66);
                }
                Send(data);
            }
        }

        public override void SendMessage(CpeMessageType type, string message) {
            Send(Packet.Message(message, type, player.hasCP437));
        }
        
        public override void SendKick(string reason, bool sync) {
            byte[] buffer = Packet.Kick(reason, player.hasCP437);
            socket.Send(buffer, sync ? SendFlags.Synchronous : SendFlags.None);
        }

        public override bool SendSetUserType(byte type) {
            // this packet doesn't exist before protocol version 7
            if (player.ProtocolVersion < Server.VERSION_0030) return false;

            Send(Packet.UserType(type));
            return true;
        }
#endregion


#region CPE packet sending
        public override bool SendSetReach(float reach) {
            if (!player.Supports(CpeExt.HeldBlock)) return false;

            Send(Packet.ClickDistance((short)(reach * 32)));
            return true;
        }

        public override bool SendHoldThis(BlockID block, bool locked) {
            if (!player.Supports(CpeExt.HeldBlock)) return false;

            BlockID raw = ConvertBlock(block);
            Send(Packet.HoldThis(raw, locked, player.hasExtBlocks));
            return true;
        }

        public override bool SendSetEnvColor(byte type, string hex) {
            if (!player.Supports(CpeExt.EnvColors)) return false;

            ColorDesc c;
            if (Colors.TryParseHex(hex, out c)) {
                Send(Packet.EnvColor(type, c.R, c.G, c.B));
            } else {
                Send(Packet.EnvColor(type, -1, -1, -1));
            }
            return true;
        }

        public override void SendChangeModel(byte id, string model) {
            BlockID raw;
            if (BlockID.TryParse(model, out raw) && raw > player.MaxRawBlock) {
                BlockID block = Block.FromRaw(raw);
                if (block >= Block.ExtendedCount) {
                    model = "humanoid"; // invalid block ids
                } else {
                    model = ConvertBlock(block).ToString();
                }                
            }
            Send(Packet.ChangeModel(id, model, player.hasCP437));
        }

        public override bool SendSetWeather(byte weather) {
            if (!player.Supports(CpeExt.EnvWeatherType)) return false;

            Send(Packet.EnvWeatherType(weather));
            return true;
        }

        public override bool SendSetTextColor(ColorDesc color) {
            if (!player.Supports(CpeExt.TextColors)) return false;

            Send(Packet.SetTextColor(color));
            return true;
        }

        public override void SendDefineBlock(BlockDefinition def) {
            byte[] packet;

            if (player.Supports(CpeExt.BlockDefinitionsExt, 2) && def.Shape != 0) {
                packet = Packet.DefineBlockExt(def, true, player.hasCP437, player.hasExtBlocks, hasExtTexs);
            } else if (player.Supports(CpeExt.BlockDefinitionsExt) && def.Shape != 0) {
                packet = Packet.DefineBlockExt(def, false, player.hasCP437, player.hasExtBlocks, hasExtTexs);
            } else {
                packet = Packet.DefineBlock(def, player.hasCP437, player.hasExtBlocks, hasExtTexs);
            }
            Send(packet);
        }

        public override void SendUndefineBlock(BlockDefinition def) {
            Send(Packet.UndefineBlock(def, player.hasExtBlocks));
        }
#endregion


#region Higher level sending
        public override void SendMotd(string motd) {
            byte[] packet = Packet.Motd(player, motd);
            Send(packet);
            
            if (!player.Supports(CpeExt.HackControl)) return;
            Send(Hacks.MakeHackControl(player, motd));
        }

        public override void SendPing() {
            if (hasTwoWayPing) {
                Send(Packet.TwoWayPing(true, player.Ping.NextTwoWayPingData()));
            } else {
                Send(Packet.Ping());
            }
        }

        public override void SendSpawnEntity(byte id, string name, string skin, Position pos, Orientation rot) {
            // NOTE: Classic clients require offseting own entity by 22 units vertically
            if (id == Entities.SelfID) pos.Y -= 22;

            if (player.Supports(CpeExt.ExtPlayerList, 2)) {
                Send(Packet.ExtAddEntity2(id, skin, name, pos, rot, player.hasCP437, player.hasExtPositions));
            } else if (player.hasExtList) {
                Send(Packet.ExtAddEntity(id, skin, name, player.hasCP437));
                Send(Packet.Teleport(id, pos, rot, player.hasExtPositions));
            } else {
                Send(Packet.AddEntity(id, name, pos, rot, player.hasCP437, player.hasExtPositions));
            }
        }

        public override void SendLevel(Level prev, Level level) {
            int volume = level.blocks.Length;
            if (player.Supports(CpeExt.FastMap)) {
                Send(Packet.LevelInitaliseExt(volume));
            } else {
                Send(Packet.LevelInitalise());
            }
            
            if (player.hasBlockDefs) {
                if (prev != null && prev != level) {
                    RemoveOldLevelCustomBlocks(prev);
                }
                BlockDefinition.SendLevelCustomBlocks(player);
                
                if (player.Supports(CpeExt.InventoryOrder)) {
                    BlockDefinition.SendLevelInventoryOrder(player);
                }
            }
            
            using (LevelChunkStream dst = new LevelChunkStream(player))
                using (Stream stream = LevelChunkStream.CompressMapHeader(player, volume, dst))
            {
                if (level.MightHaveCustomBlocks()) {
                    LevelChunkStream.CompressMap(player, stream, dst);
                } else {
                    LevelChunkStream.CompressMapSimple(player, stream, dst);
                }
            }
            
            // Force players to read the MOTD (clamped to 3 seconds at most)
            if (level.Config.LoadDelay > 0)
                Thread.Sleep(level.Config.LoadDelay);
            
            Send(Packet.LevelFinalise(level.Width, level.Height, level.Length));
        }

        void RemoveOldLevelCustomBlocks(Level oldLevel) {
            BlockDefinition[] defs = oldLevel.CustomBlockDefs;
            for (int i = 0; i < defs.Length; i++)
            {
                BlockDefinition def = defs[i];
                if (def == BlockDefinition.GlobalDefs[i] || def == null) continue;

                if (def.RawID > player.MaxRawBlock) continue;
                SendUndefineBlock(def);
            }
        }
#endregion


        /// <summary> Returns an appropriate name for the associated player's client </summary>
        /// <remarks> Determines name based on appname or protocol version supported </remarks>
        public override string ClientName() {
            if (!string.IsNullOrEmpty(player.appName)) return player.appName;
            byte version = player.ProtocolVersion;
                  
            if (version == Server.VERSION_0016) return "Classic 0.0.16";
            if (version == Server.VERSION_0017) return "Classic 0.0.17-0.0.18";
            if (version == Server.VERSION_0019) return "Classic 0.0.19";
            if (version == Server.VERSION_0020) return "Classic 0.0.20-0.0.23";
            
            // Might really be Classicube in Classic Mode, Charged Miners, etc though
            return "Classic 0.28-0.30";
        }


        public override void SendBlockchange(ushort x, ushort y, ushort z, BlockID block) {
            byte[] buffer = new byte[player.hasExtBlocks ? 9 : 8];
            buffer[0] = Opcode.SetBlock;
            NetUtils.WriteU16(x, buffer, 1);
            NetUtils.WriteU16(y, buffer, 3);
            NetUtils.WriteU16(z, buffer, 5);
            
            BlockID raw = ConvertBlock(block);
            NetUtils.WriteBlock(raw, buffer, 7, player.hasExtBlocks);
            socket.Send(buffer, SendFlags.LowPriority);
        }
        
        
        /// <summary> Converts the given block ID into a raw block ID that can be sent to this player </summary>
        public override BlockID ConvertBlock(BlockID block) {
            BlockID raw;
            if (block >= Block.Extended) {
                raw = Block.ToRaw(block);
            } else {
                raw = Block.Convert(block);
                // show invalid physics blocks as Orange
                if (raw >= Block.CPE_COUNT) raw = Block.Orange;
            }
            if (raw > player.MaxRawBlock) raw = player.level.GetFallback(block);
            
            // Check if a custom block replaced a core block
            //  If so, assume fallback is the better block to display
            if (!player.hasBlockDefs && raw < Block.CPE_COUNT) {
                BlockDefinition def = player.level.CustomBlockDefs[raw];
                if (def != null) raw = def.FallBack;
            }
            
            if (!player.hasCustomBlocks) raw = player.fallback[(byte)raw];
            return raw;
        }
    }
}
