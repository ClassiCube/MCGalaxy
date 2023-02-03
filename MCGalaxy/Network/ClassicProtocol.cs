/*
Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCForge)
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
using System.Threading;
using MCGalaxy.Events.PlayerEvents;
using MCGalaxy.Events.ServerEvents;
using BlockID = System.UInt16;

namespace MCGalaxy.Network
{
    public class ClassicProtocol : IGameSession 
    {
        // these are checked very frequently, so avoid overhead of .Supports(
        bool hasEmoteFix, hasTwoWayPing, hasExtTexs, hasTextColors;
        bool hasHeldBlock, hasLongerMessages;
        
        bool finishedCpeLogin;
        int extensionCount;
        CpeExt[] extensions = CpeExtension.Empty;

        public ClassicProtocol(INetSocket s) {
            socket = s;
            player = new Player(s, this);
        }

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
                    return left;
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


#region Classic processing
        int HandleLogin(byte[] buffer, int offset, int left) {
            // protocol versions < 6 didn't have the usertype field,
            //  hence this two-packet-size-handling monstrosity
            const int old_size = 1 + 1 + 64 + 64;
            const int new_size = 1 + 1 + 64 + 64 + 1;
            // the packet must be at least old_size long
            if (left < old_size) return 0;  
            
            ProtocolVersion = buffer[offset + 1];
            // check size now that know whether usertype field is included or not
            int size = ProtocolVersion >= Server.VERSION_0020 ? new_size : old_size;
            if (left < size) return 0;
            if (player.loggedIn)  return size;

            // usertype field has different meanings depending on protocol version
            //  Version 7 - 0x42 for CPE supporting client, should be 0 otherwise
            //  Version 6 - should be 0
            //  Version 5 - field does not exist
            if (ProtocolVersion >= Server.VERSION_0030) {
                hasCpe = buffer[offset + 130] == 0x42 && Server.Config.EnableCPE;
            }
            
            string name   = NetUtils.ReadString(buffer, offset +  2);
            string mppass = NetUtils.ReadString(buffer, offset + 66);
            if (!player.ProcessLogin(name, mppass)) return left;

            UpdateFallbackTable();
            if (hasCpe) { SendCpeExtensions(); }
            else { player.CompleteLoginProcess(); }
            return size;
        }
        
        int HandleBlockchange(byte[] buffer, int offset, int left) {
            int size = 1 + 6 + 1 + (hasExtBlocks ? 2 : 1);
            if (left < size) return 0;
            if (!player.loggedIn) return size;

            ushort x = NetUtils.ReadU16(buffer, offset + 1);
            ushort y = NetUtils.ReadU16(buffer, offset + 3);
            ushort z = NetUtils.ReadU16(buffer, offset + 5);

            byte action = buffer[offset + 7];
            if (action > 1) {
                player.Leave("Unknown block action!", true); return left;
            }

            BlockID held = ReadBlock(buffer, offset + 8);
            player.ProcessBlockchange(x, y, z, action, held);
            return size;
        }
        
        int HandleMovement(byte[] buffer, int offset, int left) {
            int size = 1 + 6 + 2 + (player.hasExtPositions ? 6 : 0) + (hasExtBlocks ? 2 : 1);
            if (left < size) return 0;
            if (!player.loggedIn) return size;

            int held = -1;
            if (hasHeldBlock) {
                held = ReadBlock(buffer, offset + 1);
                if (hasExtBlocks) offset++; // correct offset for position later
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
            bool continued = hasLongerMessages && buffer[offset + 1] != 0;

            string text = NetUtils.ReadString(buffer, offset + 2);
            player.ProcessChat(text, continued);
            return size;
        }
#endregion


#region CPE processing
        public override bool Supports(string extName, int version = 1) {
            CpeExt ext = FindExtension(extName);
            return ext != null && ext.ClientVersion == version;
        }

        CpeExt FindExtension(string extName) {
            foreach (CpeExt ext in extensions) 
            {
                if (ext.Name.CaselessEq(extName)) return ext;
            }
            return null;
        }

        void SendCpeExtensions() {
            extensions = CpeExtension.GetAllEnabled();
            Send(Packet.ExtInfo((byte)(extensions.Length + 1)));
            // fix for old classicube java client, doesn't reply if only send EnvMapAppearance with version 2
            Send(Packet.ExtEntry(CpeExt.EnvMapAppearance, 1));
            
            foreach (CpeExt ext in extensions) 
            {
                Send(Packet.ExtEntry(ext.Name, ext.ServerVersion));
            }
        }

        void CheckReadAllExtensions() {
            if (extensionCount <= 0 && !finishedCpeLogin) {
                player.CompleteLoginProcess();
                finishedCpeLogin = true;
            }
        }

        int HandleExtInfo(byte[] buffer, int offset, int left) {
            const int size = 1 + 64 + 2;
            if (left < size) return 0;
            
            appName = NetUtils.ReadString(buffer, offset + 1);
            extensionCount = buffer[offset + 66];
            CheckReadAllExtensions(); // in case client supports 0 CPE packets
            return size;
        }

        int HandleExtEntry(byte[] buffer, int offset, int left) {
            const int size = 1 + 64 + 4;
            if (left < size) return 0;
            
            string extName = NetUtils.ReadString(buffer, offset + 1);
            int extVersion = NetUtils.ReadI32(buffer,    offset + 65);

            // TODO: Classic+ client seems to use a custom protocol
            if (extVersion == 0x03110003) {
                player.Leave("Classic+ Client is unsupported");
                return size;
            }

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
                Ping.Update(data);
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

        void AddExtension(string extName, int version) {
            Player p   = player;
            CpeExt ext = FindExtension(extName);
            if (ext == null) return;
            ext.ClientVersion = (byte)version;
            
            if (ext.Name == CpeExt.CustomBlocks) {
                if (version == 1) Send(Packet.CustomBlockSupportLevel(1));
                hasCustomBlocks = true;

                UpdateFallbackTable();
                if (MaxRawBlock < Block.CPE_MAX_BLOCK) MaxRawBlock = Block.CPE_MAX_BLOCK;
            } else if (ext.Name == CpeExt.ChangeModel) {
                p.hasChangeModel = true;
            } else if (ext.Name == CpeExt.EmoteFix) {
                hasEmoteFix = true;
            } else if (ext.Name == CpeExt.FullCP437) {
                p.hasCP437 = true;
            } else if (ext.Name == CpeExt.ExtPlayerList) {
                p.hasExtList = true;
            } else if (ext.Name == CpeExt.BlockDefinitions) {
                hasBlockDefs = true;
                if (MaxRawBlock < 255) MaxRawBlock = 255;
            } else if (ext.Name == CpeExt.TextColors) {
                hasTextColors = true;
                SendGlobalColors();
            } else if (ext.Name == CpeExt.ExtEntityPositions) {
                p.hasExtPositions = true;
            } else if (ext.Name == CpeExt.TwoWayPing) {
                hasTwoWayPing = true;
            } else if (ext.Name == CpeExt.BulkBlockUpdate) {
                hasBulkBlockUpdate = true;
            } else if (ext.Name == CpeExt.ExtTextures) {
                hasExtTexs = true;
            } else if (ext.Name == CpeExt.HeldBlock) {
                hasHeldBlock = true;
            } else if (ext.Name == CpeExt.LongerMessages) {
                hasLongerMessages = true;
            }
            #if TEN_BIT_BLOCKS
            else if (ext.Name == CpeExt.ExtBlocks) {
                hasExtBlocks = true;
                if (MaxRawBlock < 767) MaxRawBlock = 767;
            }
            #endif
        }

        void SendGlobalColors() {
            for (int i = 0; i < Colors.List.Length; i++)
            {
                if (!Colors.List[i].IsModified()) continue;
                Send(Packet.SetTextColor(Colors.List[i]));
            }
        }
#endregion


#region Classic packet sending
        public override void SendTeleport(byte id, Position pos, Orientation rot) {
            // Some classic < 0.0.19 versions have issues with sending teleport packet with ID 255
            //   0.0.16a - does nothing
            //   0.0.17a - does nothing
            //   0.0.18a - works fine (https://minecraft.fandom.com/wiki/Java_Edition_Classic_0.0.18a)
            // Skins weren't implemented until 0.0.19a, so it's fine to spam send SpawnEntity packets
            //  (downside is that client's respawn position is also changed due to using SpawnEntity)
            // Unfortunately, there is no easy way to tell the difference between 0.0.17a and 0.0.18a,
            //  so this workaround still affects 0.0.18 clients even though it is unnecessary
            if (id == Entities.SelfID && ProtocolVersion < Server.VERSION_0019) {
                // TODO keep track of 'last spawn name', in case self entity name was changed by OnEntitySpawnedEvent
                SendSpawnEntity(id, player.color + player.truename, player.SkinName, pos, rot);
                return;
            }

            // NOTE: Classic clients require offseting own entity by 22 units vertically
            if (id == Entities.SelfID) pos.Y -= 22;

            Send(Packet.Teleport(id, pos, rot, player.hasExtPositions));
        }
        public override bool SendTeleport(byte id, Position pos, Orientation rot,
                                          Packet.TeleportMoveMode moveMode, bool usePos = true, bool interpolateOri = false, bool useOri = true) {
            if (!Supports(CpeExt.ExtEntityTeleport)) { return false; }

            // NOTE: Classic clients require offseting own entity by 22 units vertically when using absolute location updates
            if ((moveMode == Packet.TeleportMoveMode.AbsoluteInstant || moveMode == Packet.TeleportMoveMode.AbsoluteSmooth) && id == Entities.SelfID)
                { pos.Y -= 22; }

            Send(Packet.TeleportExt(id, usePos, moveMode, useOri, interpolateOri, pos, rot, player.hasExtPositions));
            return true;
        }

        public override void SendRemoveEntity(byte id) {
            Send(Packet.RemoveEntity(id));
        }

        public override void SendChat(string message) {
            int bufferLen;
            // See comment in CleanupColors
            char[] buffer = LineWrapper.CleanupColors(message, out bufferLen, 
                                                      hasTextColors, hasTextColors);
            List<string> lines = LineWrapper.Wordwrap(buffer, bufferLen, hasEmoteFix);

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
            message = CleanupColors(message);
            Send(Packet.Message(message, type, player.hasCP437));
        }
        
        public override void SendKick(string reason, bool sync) {
            reason = CleanupColors(reason);
            byte[] buffer = Packet.Kick(reason, player.hasCP437);
            socket.Send(buffer, sync ? SendFlags.Synchronous : SendFlags.None);
        }

        public override bool SendSetUserType(byte type) {
            // this packet doesn't exist before protocol version 7
            if (ProtocolVersion < Server.VERSION_0030) return false;

            Send(Packet.UserType(type));
            return true;
        }
        #endregion


#region CPE packet sending
        public override void SendAddTabEntry(byte id, string name, string nick, string group, byte groupRank) {
            nick  = CleanupColors(nick);
            group = CleanupColors(group);
            Send(Packet.ExtAddPlayerName(id, name, nick, group, groupRank, player.hasCP437));
        }

        public override void SendRemoveTabEntry(byte id) {
            Send(Packet.ExtRemovePlayerName(id));
        }
        
        public override bool SendSetReach(float reach) {
            if (!Supports(CpeExt.ClickDistance)) return false;

            Send(Packet.ClickDistance((short)(reach * 32)));
            return true;
        }

        public override bool SendHoldThis(BlockID block, bool locked) {
            if (!hasHeldBlock) return false;

            BlockID raw = ConvertBlock(block);
            Send(Packet.HoldThis(raw, locked, hasExtBlocks));
            return true;
        }

        public override bool SendSetEnvColor(byte type, string hex) {
            if (!Supports(CpeExt.EnvColors)) return false;

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
            if (BlockID.TryParse(model, out raw) && raw > MaxRawBlock) {
                BlockID block = Block.FromRaw(raw);
                if (block >= Block.SUPPORTED_COUNT) {
                    model = "humanoid"; // invalid block ids
                } else {
                    model = ConvertBlock(block).ToString();
                }                
            }
            Send(Packet.ChangeModel(id, model, player.hasCP437));
        }

        public override bool SendSetWeather(byte weather) {
            if (!Supports(CpeExt.EnvWeatherType)) return false;

            Send(Packet.EnvWeatherType(weather));
            return true;
        }

        public override bool SendSetTextColor(ColorDesc color) {
            if (!hasTextColors) return false;

            Send(Packet.SetTextColor(color));
            return true;
        }

        public override bool SendDefineBlock(BlockDefinition def) {
            if (!hasBlockDefs || def.RawID > MaxRawBlock) return false;
            byte[] packet;

            if (Supports(CpeExt.BlockDefinitionsExt, 2) && def.Shape != 0) {
                packet = Packet.DefineBlockExt(def, true, player.hasCP437, hasExtBlocks, hasExtTexs);
            } else if (Supports(CpeExt.BlockDefinitionsExt) && def.Shape != 0) {
                packet = Packet.DefineBlockExt(def, false, player.hasCP437, hasExtBlocks, hasExtTexs);
            } else {
                packet = Packet.DefineBlock(def, player.hasCP437, hasExtBlocks, hasExtTexs);
            }

            Send(packet);
            return true;
        }

        public override bool SendUndefineBlock(BlockDefinition def) {
            if (!hasBlockDefs || def.RawID > MaxRawBlock) return false;

            Send(Packet.UndefineBlock(def, hasExtBlocks));
            return true;
        }
#endregion


#region Higher level sending
        public override void SendMotd(string motd) {
            motd = CleanupColors(motd);
            Send(Packet.Motd(player, motd));
            
            if (!Supports(CpeExt.HackControl)) return;
            Send(Hacks.MakeHackControl(player, motd));
        }

        public override void SendPing() {
            if (hasTwoWayPing) {
                Send(Packet.TwoWayPing(true, Ping.NextTwoWayPingData()));
            } else {
                Send(Packet.Ping());
            }
        }

        public override void SendSetSpawnpoint(Position pos, Orientation rot) {
            if (Supports(CpeExt.SetSpawnpoint)) {
                Send(Packet.SetSpawnpoint(pos, rot, player.hasExtPositions));
            } else {
                // TODO respawn self directly instead of using Entities.Spawn
                Entities.Spawn(player, player, pos, rot);
            }
        }

        public override void SendSpawnEntity(byte id, string name, string skin, Position pos, Orientation rot) {
            name = CleanupColors(name);
            // NOTE: Classic clients require offseting own entity by 22 units vertically
            if (id == Entities.SelfID) pos.Y -= 22;

            // SpawnEntity for self ID behaves differently in Classic 0.0.16a
            //  - yaw and pitch fields are swapped
            //  - pitch is inverted
            // (other entities do NOT require this adjustment however)
            if (id == Entities.SelfID && ProtocolVersion <= Server.VERSION_0016) {
                byte temp = rot.HeadX;
                rot.HeadX = rot.RotY;
                rot.RotY  = (byte)(256 - temp);
            }

            if (Supports(CpeExt.ExtPlayerList, 2)) {
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
            if (Supports(CpeExt.FastMap)) {
                Send(Packet.LevelInitaliseExt(volume));
            } else {
                Send(Packet.LevelInitalise());
            }
            
            if (hasBlockDefs) {
                if (prev != null && prev != level) {
                    RemoveOldLevelCustomBlocks(prev);
                }
                BlockDefinition.SendLevelCustomBlocks(player);
                
                if (Supports(CpeExt.InventoryOrder)) {
                    BlockDefinition.SendLevelInventoryOrder(player);
                }
            }

            LevelChunkStream.SendLevel(this, level, volume);
            
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

                SendUndefineBlock(def);
            }
        }
#endregion

                
        public override void SendBlockchange(ushort x, ushort y, ushort z, BlockID block) {
            byte[] buffer = new byte[hasExtBlocks ? 9 : 8];
            buffer[0] = Opcode.SetBlock;
            NetUtils.WriteU16(x, buffer, 1);
            NetUtils.WriteU16(y, buffer, 3);
            NetUtils.WriteU16(z, buffer, 5);
            
            BlockID raw = ConvertBlock(block);
            NetUtils.WriteBlock(raw, buffer, 7, hasExtBlocks);
            socket.Send(buffer, SendFlags.LowPriority);
        }

        public override byte[] MakeBulkBlockchange(BufferedBlockSender buffer) {
            return buffer.MakeLimited(fallback);
        }
        
        void UpdateFallbackTable() {
            for (byte b = 0; b <= Block.CPE_MAX_BLOCK; b++)
            {
                fallback[b] = hasCustomBlocks ? b : Block.ConvertClassic(b, ProtocolVersion);
            }
        }


        string CleanupColors(string value) {
            // Although ClassiCube in classic mode supports invalid colours,
            //  the original vanilla client crashes with invalid colour codes
            // Since it's impossible to identify which client is being used,
            //  just remove the ampersands to be on the safe side
            //  when text colours extension is not supported
            return LineWrapper.CleanupColors(value, hasTextColors, hasTextColors);
        }

        public override string ClientName() {
            if (!string.IsNullOrEmpty(appName)) return appName;
            byte version = ProtocolVersion;
                  
            if (version == Server.VERSION_0016) return "Classic 0.0.16";
            if (version == Server.VERSION_0017) return "Classic 0.0.17-0.0.18";
            if (version == Server.VERSION_0019) return "Classic 0.0.19";
            if (version == Server.VERSION_0020) return "Classic 0.0.20-0.0.23";
            
            // Might really be Classicube in Classic Mode, Charged Miners, etc though
            return "Classic 0.28-0.30";
        }

        // TODO modularise and move common code back into Entities.cs
        public unsafe override void UpdatePlayerPositions() {
            Player[] players = PlayerInfo.Online.Items;
            byte* src  = stackalloc byte[16 * 256]; // 16 = size of absolute update, with extended positions
            byte* ptr  = src;
            Player dst = player;

            foreach (Player p in players) {
                if (dst == p || dst.level != p.level || !dst.CanSeeEntity(p)) continue;
                
                Orientation rot = p.Rot; byte pitch = rot.HeadX;
                if (Server.flipHead || p.flipHead) pitch = FlippedPitch(pitch);
                
                // flip head when infected in ZS, but doesn't support model
                if (!dst.hasChangeModel && p.infected)
                   pitch = FlippedPitch(pitch);
            
                rot.HeadX = pitch;
                Entities.GetPositionPacket(ref ptr, p.id, p.hasExtPositions, dst.hasExtPositions,
                                           p._tempPos, p._lastPos, rot, p._lastRot);
            }
            
            int count = (int)(ptr - src);
            if (count == 0) return;
            
            byte[] packet = new byte[count];
            for (int i = 0; i < packet.Length; i++) { packet[i] = src[i]; }
            dst.Send(packet);
        }

        static byte FlippedPitch(byte pitch) {
             if (pitch > 64 && pitch < 192) return pitch;
             else return 128;
        }
    }
}
