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
using System.Threading;
using MCGalaxy.Games;
using BlockID = System.UInt16;
using BlockRaw = System.Byte;

namespace MCGalaxy.Network
{
    class ClassicHandshakeParser : INetProtocol
    {
        INetSocket socket;

        public ClassicHandshakeParser(INetSocket s)
        {
            socket = s;
        }

        public void Disconnect() { }

        public int ProcessReceived(byte[] buffer, int length) {
            if (length < 2) return 0;

            byte version = buffer[1];
            if (version >= 'A' && version <= 'z') {
                socket.protocol = new Classic0015Protocol(socket);
            } else {
                socket.protocol = new ClassicProtocol(socket);
            }
            return socket.protocol.ProcessReceived(buffer, length);
        }
    }

    public class Classic0015Protocol : GameSession, INetProtocol 
    {
        public Classic0015Protocol(INetSocket s) {
            socket = s;
            player = new Player(s, this);
        }

        protected override int HandlePacket(byte[] buffer, int offset, int left) {
            switch (buffer[offset]) {
                case Opcode.Ping:              return 1;
                case Opcode.Handshake:         return HandleLogin(buffer, offset, left);
                case Opcode.SetBlockClient:    return HandleBlockchange(buffer, offset, left);
                case Opcode.EntityTeleport:    return HandleMovement(buffer, offset, left);

                default:
                    player.Leave("Unhandled opcode \"" + buffer[offset] + "\"!", true);
                    return -1;
            }
        }

        BlockID ReadBlock(byte[] buffer, int offset) { return Block.FromRaw(buffer[offset]); }

        public override void Disconnect() { player.Disconnect(); }


#region Classic processing
        int HandleLogin(byte[] buffer, int offset, int left) {
            int size = 1 + 64;
            if (left < size)     return 0;
            if (player.loggedIn) return size;

            ProtocolVersion = Server.VERSION_0016;
            string name = NetUtils.ReadString(buffer, offset + 1);
            if (!player.ProcessLogin(name, "")) return -1;

            UpdateFallbackTable();
            player.CompleteLoginProcess();
            return size;
        }
        
        int HandleBlockchange(byte[] buffer, int offset, int left) {
            int size = 1 + 6 + 1 + 1;
            if (left < size) return 0;
            if (!player.loggedIn) return size;

            ushort x = NetUtils.ReadU16(buffer, offset + 1);
            ushort y = NetUtils.ReadU16(buffer, offset + 3);
            ushort z = NetUtils.ReadU16(buffer, offset + 5);

            byte action = buffer[offset + 7];
            if (action > 1) {
                player.Leave("Unknown block action!", true); return -1;
            }

            BlockID held = Block.FromRaw(buffer[offset + 8]);
            player.ProcessBlockchange(x, y, z, action, held);
            return size;
        }
        
        int HandleMovement(byte[] buffer, int offset, int left) {
            int size = 1 + 6 + 2 + 1;
            if (left < size) return 0;
            if (!player.loggedIn) return size;

            int x = NetUtils.ReadI16(buffer, offset + 2);
            int y = NetUtils.ReadI16(buffer, offset + 4);
            int z = NetUtils.ReadI16(buffer, offset + 6);

            byte yaw   = buffer[offset + 8];
            byte pitch = buffer[offset + 9];
            player.ProcessMovement(x, y, z, yaw, pitch, -1);
            return size;
        }
        #endregion


        #region Classic packet sending
        public override void SendTeleport(byte id, Position pos, Orientation rot)
        {
            if (id == Entities.SelfID)
            {
                // TODO keep track of 'last spawn name', in case self entity name was changed by OnEntitySpawnedEvent
                SendSpawnEntity(id, player.color + player.truename, player.SkinName, pos, rot);
                return;
            }

            // NOTE: Classic clients require offseting own entity by 22 units vertically
            if (id == Entities.SelfID) pos.Y -= 22;

            Send(Packet.Teleport(id, pos, rot, player.hasExtPositions));
        }

        public override void SendRemoveEntity(byte id) {
            // 0.0.15a uses different packet opcode for RemoveEntity
            byte[] packet = new byte[] { 9, id };
            Send(packet);
        }

        public override void SendChat(string message) {
        }

        public override void SendMessage(CpeMessageType type, string message) {
        }
        
        public override void SendKick(string reason, bool sync) {
        }
        #endregion


        #region CPE packet sending
        public override void SendAddTabEntry(byte id, string name, string nick, string group, byte groupRank) {
        }

        public override void SendRemoveTabEntry(byte id) {
        }

        public override void SendChangeModel(byte id, string model) {
        }
#endregion


#region Higher level sending
        public override void SendMotd(string motd) {
            byte[] packet = new byte[1 + 64];
            packet[0] = Opcode.Handshake;
            NetUtils.Write(Server.Config.Name, packet, 1, player.hasCP437);

            Send(packet);
        }

        public override void SendPing() {
            Send(Packet.Ping());
        }

        public override void SendSetSpawnpoint(Position pos, Orientation rot) {
            // TODO respawn self directly instead of using Entities.Spawn
            Entities.Spawn(player, player, pos, rot);
        }

        public override void SendSpawnEntity(byte id, string name, string skin, Position pos, Orientation rot) {
            name = CleanupColors(name);
            // NOTE: Classic clients require offseting own entity by 22 units vertically
            if (id == Entities.SelfID) pos.Y -= 22;

            // SpawnEntity for self ID behaves differently in Classic 0.0.16a
            //  - yaw and pitch fields are swapped
            //  - pitch is inverted
            // (other entities do NOT require this adjustment however)
            if (id == Entities.SelfID && ProtocolVersion == Server.VERSION_0016) {
                byte temp = rot.HeadX;
                rot.HeadX = rot.RotY;
                rot.RotY  = (byte)(256 - temp);
            }
            Send(Packet.AddEntity(id, name, pos, rot, player.hasCP437, player.hasExtPositions));
        }

        public override void SendLevel(Level prev, Level level) {
            int volume = level.blocks.Length;
            Send(Packet.LevelInitalise());
            LevelChunkStream.SendLevel(player, level, volume);
            
            // Force players to read the MOTD (clamped to 3 seconds at most)
            if (level.Config.LoadDelay > 0)
                Thread.Sleep(level.Config.LoadDelay);
            
            Send(Packet.LevelFinalise(level.Width, level.Height, level.Length));
        }
#endregion

                
        public override void SendBlockchange(ushort x, ushort y, ushort z, BlockID block) {
            byte[] buffer = new byte[8];
            buffer[0] = Opcode.SetBlock;
            NetUtils.WriteU16(x, buffer, 1);
            NetUtils.WriteU16(y, buffer, 3);
            NetUtils.WriteU16(z, buffer, 5);
            
            BlockID raw = ConvertBlock(block);
            buffer[7] = (byte)raw;
            socket.Send(buffer, SendFlags.LowPriority);
        }

        public override byte[] MakeBulkBlockchange(BufferedBlockSender buffer) {
            return buffer.MakeLimited(fallback);
        }
        
        /// <summary> Converts the given block ID into a raw block ID that can be sent to this player </summary>
        public override BlockID ConvertBlock(BlockID block) {
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
            
            if (!hasCustomBlocks) raw = fallback[(BlockRaw)raw];
            return raw;
        }
        
        internal void UpdateFallbackTable() {
            for (byte b = 0; b < Block.CPE_COUNT; b++)
            {
                fallback[b] = Block.ConvertLimited(b, this);
            }
        }


        string CleanupColors(string value) {
            return LineWrapper.CleanupColors(value, false, false);
        }

        public override string ClientName() {
            return "Classic 0.15";
        }

        // TODO modularise and move common code back into Entities.c
        public unsafe override void UpdatePlayerPositions()
        {
            Player[] players = PlayerInfo.Online.Items;
            Player dst = player;

            foreach (Player p in players)
            {
                if (dst == p || dst.level != p.level || !dst.CanSeeEntity(p)) continue;

                Orientation rot = p.Rot;
                // TEMP HACK
                Position delta = Entities.GetDelta(p.tempPos, p.lastPos, p.hasExtPositions);
                bool posChanged = delta.X != 0 || delta.Y != 0 || delta.Z != 0;
                bool oriChanged = rot.RotY != p.lastRot.RotY || rot.HeadX != p.lastRot.HeadX;
                if (posChanged || oriChanged)
                    SendTeleport(p.id, p.tempPos, rot);
            }
        }

        static byte FlippedPitch(byte pitch) {
             if (pitch > 64 && pitch < 192) return pitch;
             else return 128;
        }
    }
}
