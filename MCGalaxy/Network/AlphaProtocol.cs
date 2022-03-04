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
using System.IO.Compression;
using System.Threading;
using MCGalaxy.Events.PlayerEvents;
using MCGalaxy.Events.ServerEvents;
using System.Text;
using BlockID = System.UInt16;

namespace MCGalaxy.Network
{
    class AlphaIndevParser : INetProtocol
    {
        INetSocket socket;

        public AlphaIndevParser(INetSocket s)
        {
            socket = s;
        }

        public void Disconnect() { }

        public int ProcessReceived(byte[] buffer, int length)
        {
            if (length < 4) return 0;

            // indev uses big endian unicode, alpha uses utf18
            if (buffer[3] == 0) {
                socket.protocol = new IndevProtocol(socket);
            } else {
                socket.protocol = new AlphaProtocol(socket);
            }

            return socket.protocol.ProcessReceived(buffer, length);
        }
    }

    public class AlphaProtocol : GameSession, INetProtocol
    {
        const int OPCODE_PING      = 0x00;
        const int OPCODE_LOGIN     = 0x01;
        const int OPCODE_HANDSHAKE = 0x02;
        const int OPCODE_CHAT      = 0x03;

        const int OPCODE_SELF_STATEONLY = 0x0A;
        const int OPCODE_SELF_MOVE      = 0x0B;
        const int OPCODE_SELF_LOOK      = 0x0C;
        const int OPCODE_SELF_MOVE_LOOK = 0x0D;
        const int OPCODE_BLOCK_DIG      = 0x0E;
        const int OPCODE_BLOCK_PLACE    = 0x0F;

        const int OPCODE_ARM_ANIM  = 0x12;
        const int OPCODE_NAMED_ADD = 0x14;
        const int OPCODE_REMOVE_ENTITY = 0x1D;
        const int OPCODE_REL_MOVE  = 0x1F;
        const int OPCODE_LOOK      = 0x20;
        const int OPCODE_REL_MOVE_LOOK = 0x21;
        const int OPCODE_TELEPORT  = 0x22;
        const int OPCODE_PRE_CHUNK = 0x32;
        const int OPCODE_CHUNK     = 0x33;
        const int OPCODE_BLOCK_CHANGE = 0x35;

        const int PROTOCOL_VERSION = 2;

        public AlphaProtocol(INetSocket s) {
            socket = s;
            player = new Player(s, this);
        }

        protected override int HandlePacket(byte[] buffer, int offset, int left) {
            //Console.WriteLine("IN: " + buffer[offset]);
            switch (buffer[offset]) {
                case OPCODE_PING:      return 1; // Ping
                case OPCODE_LOGIN:     return HandleLogin(buffer, offset, left);
                case OPCODE_HANDSHAKE: return HandleHandshake(buffer, offset, left);
                case OPCODE_CHAT:      return HandleChat(buffer, offset, left);
                case OPCODE_SELF_STATEONLY: return HandleSelfStateOnly(buffer, offset, left);
                case OPCODE_SELF_MOVE:      return HandleSelfMove(buffer, offset, left);
                case OPCODE_SELF_LOOK:      return HandleSelfLook(buffer, offset, left);
                case OPCODE_SELF_MOVE_LOOK: return HandleSelfMoveLook(buffer, offset, left);
                case OPCODE_BLOCK_DIG:      return HandleBlockDig(buffer, offset, left);
                case OPCODE_BLOCK_PLACE:    return HandleBlockPlace(buffer, offset, left);
                case OPCODE_ARM_ANIM:       return HandleArmAnim(buffer, offset, left);

                default:
                    player.Leave("Unhandled opcode \"" + buffer[offset] + "\"!", true);
                    return -1;
            }
        }

        static int ReadStringLength(byte[] buffer, int offset) {
            return ReadU16(buffer, offset);
        }

        static string ReadString(byte[] buffer, int offset) {
            int len = ReadStringLength(buffer, offset);
            return Encoding.UTF8.GetString(buffer, offset + 2, len);
        }

        static int CalcStringLength(string value) {
            return Encoding.UTF8.GetByteCount(value);
        }

        static void WriteString(byte[] buffer, int offset, string value) {
            int len = Encoding.UTF8.GetBytes(value, 0, value.Length, buffer, offset + 2);
            WriteU16((ushort)len, buffer, offset);
        }

        static ushort ReadU16(byte[] array, int index) {
            return NetUtils.ReadU16(array, index);
        }

        static void WriteU16(ushort value, byte[] array, int index) {
            NetUtils.WriteU16(value, array, index);
        }

        static int ReadI32(byte[] array, int index) {
            return NetUtils.ReadI32(array, index);
        }

        static void WriteI32(int value, byte[] array, int index) {
            NetUtils.WriteI32(value, array, index);
        }

        unsafe static float ReadF32(byte[] array, int offset) {
            int value = ReadI32(array, offset);
            return *(float*)&value;
        }

        unsafe static double ReadF64(byte[] array, int offset) {
            long hi = ReadI32(array, offset + 0) & 0xFFFFFFFFL;
            long lo = ReadI32(array, offset + 4) & 0xFFFFFFFFL;

            long value = (hi << 32) | lo;
            return *(double*)&value;
        }

        unsafe static void WriteF32(float value, byte[] buffer, int offset) {
            int num = *(int*)&value;
            WriteI32(num, buffer, offset + 0);
        }

        unsafe static void WriteF64(double value, byte[] buffer, int offset) {
            long num = *(long*)&value;
            WriteI32((int)(num >> 32), buffer, offset + 0);
            WriteI32((int)(num >>  0), buffer, offset + 4);
        }

        BlockID ReadBlock(byte[] buffer, int offset) { return Block.FromRaw(buffer[offset]); }

        public override void Disconnect() { player.Disconnect(); }


#region Classic processing
        int HandleLogin(byte[] buffer, int offset, int left) {
            int size = 1 + 4; // opcode + version;
            int strLen;
            if (left < size) return 0;

            int version = ReadI32(buffer, offset + 1);
            if (version != PROTOCOL_VERSION) {
                player.Leave("Unsupported protocol version!"); return -1;
            }

            if (left < size + 2)          return 0;
            strLen = ReadStringLength(buffer, offset + size);
            if (left < size + 2 + strLen) return 0;
            string name = ReadString(buffer,  offset + size);
            size += 2 + strLen;

            if (left < size + 2)          return 0;
            strLen = ReadStringLength(buffer, offset + size);
            if (left < size + 2 + strLen) return 0;
            string pass = ReadString(buffer,  offset + size);
            size += 2 + strLen;

            if (!player.ProcessLogin(name, pass)) return -1;

            player.CompleteLoginProcess();
            return size;
        }

        int HandleHandshake(byte[] buffer, int offset, int left) {
            int size = 1 + 2; // opcode + name length
            if (left < size) return 0;
            
            // enough data for name length?
            size += ReadStringLength(buffer, offset + 1);
            if (left < size) return 0;
            string name = ReadString(buffer, offset + 1);

            // TEMP HACK
            player.name = name; player.truename = name;
            Logger.Log(LogType.SystemActivity, "REA!: " + name);
            // - for no name name verification
            SendHandshake("-");

            return size;
        }

        int HandleChat(byte[] buffer, int offset, int left) {
            int size = 1 + 2; // opcode + text length
            if (left < size) return 0;
            
            // enough data for name length?
            size += ReadStringLength(buffer, offset + 1);
            if (left < size) return 0;
            string text = ReadString(buffer, offset + 1);

            player.ProcessChat(text, false);
            return size;
        }

        int HandleSelfStateOnly(byte[] buffer, int offset, int left) {
            int size = 1 + 1;
            if (left < size) return 0;
            // bool state

            Position pos    = player.Pos;
            Orientation rot = player.Rot;
            player.ProcessMovement(pos.X, pos.Y, pos.Z, rot.RotY, rot.HeadX, 0);
            return size;
        }

        int HandleSelfMove(byte[] buffer, int offset, int left) {
            int size = 1 + 8 + 8 + 8 + 8 + 1;
            if (left < size) return 0;

            double x = ReadF64(buffer, offset + 1);
            double s = ReadF64(buffer, offset + 9); // TODO probably wrong (e.g. when crouching)
            double y = ReadF64(buffer, offset + 17);
            double z = ReadF64(buffer, offset + 25);
            // bool state

            Orientation rot = player.Rot;
            player.ProcessMovement((int)(x * 32), (int)(y * 32), (int)(z * 32),
                              rot.RotY, rot.HeadX, 0);
            return size;
        }

        int HandleSelfLook(byte[] buffer, int offset, int left) {
            int size = 1 + 4 + 4 + 1;
            if (left < size) return 0;

            float yaw   = ReadF32(buffer, offset + 1) + 180.0f;
            float pitch = ReadF32(buffer, offset + 5);
            // bool state

            Position pos = player.Pos;
            player.ProcessMovement(pos.X, pos.Y, pos.Z,
                              (byte)(yaw / 360.0f * 256.0f), (byte)(pitch / 360.0f * 256.0f), 0);
            return size;
        }

        int HandleSelfMoveLook(byte[] buffer, int offset, int left) {
            int size = 1 + 8 + 8 + 8 + 8 + 4 + 4 + 1;
            if (left < size) return 0;

            double x = ReadF64(buffer, offset + 1);
            double s = ReadF64(buffer, offset + 9); // TODO probably wrong (e.g. when crouching)
            double y = ReadF64(buffer, offset + 17);
            double z = ReadF64(buffer, offset + 25);

            float yaw   = ReadF32(buffer, offset + 33) + 180.0f;
            float pitch = ReadF32(buffer, offset + 37);
            // bool state

            player.ProcessMovement((int)(x * 32), (int)(y * 32), (int)(z * 32),
                              (byte)(yaw / 360.0f * 256.0f), (byte)(pitch / 360.0f * 256.0f), 0);
            return size;
        }

        int HandleBlockDig(byte[] buffer, int offset, int left) {
            int size = 1 + 1 + 4 + 1 + 4 + 1;
            if (left < size) return 0;

            byte status = buffer[offset + 1];
            int x    = ReadI32(buffer, offset + 2);
            int y    = buffer[offset + 6];
            int z    = ReadI32(buffer, offset + 7);
            byte dir = buffer[offset + 11];

            if (status == 3)
                player.ProcessBlockchange((ushort)x, (ushort)y, (ushort)z, 0, 0);
            return size;
        }

        int HandleBlockPlace(byte[] buffer, int offset, int left) {
            int size = 1 + 2 + 4 + 1 + 4 + 1;
            if (left < size) return 0;

            BlockID block = ReadU16(buffer, offset + 1);
            int x    = ReadI32(buffer, offset + 3);
            int y    = buffer[offset + 7];
            int z    = ReadI32(buffer, offset + 8);
            byte dir = buffer[offset + 12];

            player.ProcessBlockchange((ushort)x, (ushort)y, (ushort)z, 1, block);
            return size;
        }

        int HandleArmAnim(byte[] buffer, int offset, int left) {
            int size = 1 + 4 + 1;
            if (left < size) return 0;

            // TODO something
            return size;
        }
        #endregion


        void SendHandshake(string serverID) {
            Send(MakeHandshake(serverID));
        }

        public override void SendTeleport(byte id, Position pos, Orientation rot) {
            if (id == Entities.SelfID) {
                Send(MakeSelfMoveLook(pos, rot));
            } else {
                Send(MakeEntityTeleport(id, pos, rot));
            }
        }

        public override void SendRemoveEntity(byte id) {
            byte[] data = new byte[1 + 4];
            data[0] = OPCODE_REMOVE_ENTITY;
            WriteI32(id, data, 1);
            Send(data);
        }

        public override void SendChat(string message) {
            List<string> lines = LineWrapper.Wordwrap(message, true);
            for (int i = 0; i < lines.Count; i++)
            {
                string line = lines[i].Replace('&', '§');
                Send(MakeChat(line));
            }
        }

        public override void SendMessage(CpeMessageType type, string message) {
            if (type != CpeMessageType.Normal) return;
            message = message.Replace('&', '§');
            Send(MakeChat(message));
        }
        
        public override void SendBlockchange(ushort x, ushort y, ushort z, BlockID block) {
            byte[] packet = new byte[1 + 4 + 1 + 4 + 1 + 1];
            byte raw = (byte)ConvertBlock(block);
            WriteBlockChange(packet, 0, raw, x, y, z);
            Send(packet);
        }

        public override void SendKick(string reason, bool sync) {
        }

        public override void SendChangeModel(byte id, string model) {
        }

        bool sentMOTD;
        public override void SendMotd(string motd) {
            if (sentMOTD) return; // TODO work out how to properly resend map
            sentMOTD = true;
            Send(MakeLogin(motd));
        }

        public override void SendPing() {
            Send(new byte[] { OPCODE_PING });
        }

        public override void SendSpawnEntity(byte id, string name, string skin, Position pos, Orientation rot) {
            name = name.Replace('&', '§');
            skin = skin.Replace('&', '§');

            if (id == Entities.SelfID) {
                Send(MakeSelfMoveLook(pos, rot));
            } else {
                Send(MakeNamedAdd(id, name, skin, pos, rot));
            }
        }

        public override void SendLevel(Level prev, Level level) {
            // unload chunks from previous world
            if (prev != null)
            {
                for (int z = 0; z < prev.ChunksZ; z++)
                    for (int x = 0; x < prev.ChunksX; x++)
                    {
                        Send(MakePreChunk(x, z, false));
                    }
            }

            for (int z = 0; z < level.ChunksZ; z++)
                for (int x = 0; x < level.ChunksX; x++)
                {
                    Send(MakePreChunk(x, z, true));
                    Send(MakeChunk(x, z, level));
                }
        }

        byte[] MakeHandshake(string serverID) {
            int dataLen = 1 + 2 + CalcStringLength(serverID);
            byte[] data = new byte[dataLen];

            data[0] = OPCODE_HANDSHAKE;
            WriteString(data, 1, serverID);
            return data;
        }

        byte[] MakeLogin(string motd) {
            int nameLen = CalcStringLength(Server.Config.Name);
            int motdLen = CalcStringLength(motd);
            int dataLen = 1 + 4 + (2 + nameLen) + (2 + motdLen);
            byte[] data = new byte[dataLen];

            data[0] = OPCODE_LOGIN;
            NetUtils.WriteI32(2, data, PROTOCOL_VERSION);
            WriteString(data, 1 + 4,               Server.Config.Name);
            WriteString(data, 1 + 4 + 2 + nameLen, motd);
            return data;
        }

        byte[] MakeChat(string text) {
            int textLen = CalcStringLength(text);
            byte[] data = new byte[1 + 2 + textLen];

            data[0] = OPCODE_CHAT;
            WriteString(data, 1 , text);
            return data;
        }

        byte[] MakeSelfMoveLook(Position pos, Orientation rot) {
            byte[] data = new byte[1 + 8 + 8 + 8 + 8 + 4 + 4 + 1];
            float yaw   = rot.RotY  * 360.0f / 256.0f;
            float pitch = rot.HeadX * 360.0f / 256.0f;
            data[0] = OPCODE_SELF_MOVE_LOOK;

            WriteF64(pos.X / 32.0, data,  1);
            WriteF64(pos.Y / 32.0, data,  9); // stance?
            WriteF64(pos.Y / 32.0, data, 17);
            WriteF64(pos.Z / 32.0, data, 25);

            WriteF32(yaw,   data, 33);
            WriteF32(pitch, data, 37);
            data[41] = 1;
            return data;
        }

        byte[] MakeBlockDig(byte status, int x, int y, int z) {
            byte[] data = new byte[1 + 1 + 4 + 1 + 4 + 1];
            data[0] = OPCODE_BLOCK_DIG;

            data[1] = status;
            WriteI32(x, data, 2);
            data[6] = (byte)y;
            WriteI32(z, data, 7);
            data[11] = 1;
            return data;
        }

        byte[] MakeBlockChange(byte block, int x, int y, int z) {
            byte[] data = new byte[1 + 4 + 1 + 4 + 1 + 1];
            data[0] = OPCODE_BLOCK_CHANGE;
            WriteI32(x, data, 1);
            data[5] = (byte)y;
            WriteI32(z, data, 6);
            data[10] = block;
            data[11] = 0; // metadata
            return data;
        }

        byte[] MakeNamedAdd(byte id, string name, string skin, Position pos, Orientation rot) {
            int nameLen = CalcStringLength(name);
            int dataLen = 1 + 4 + (2 + nameLen) + (4 + 4 + 4) + (1 + 1) + 2;
            byte[] data = new byte[dataLen];

            data[0] = OPCODE_NAMED_ADD;
            WriteI32(id, data, 1);
            WriteString(data, 5, name);

            WriteI32(pos.X, data,  7 + nameLen);
            WriteI32(pos.Y, data, 11 + nameLen);
            WriteI32(pos.Z, data, 15 + nameLen);

            data[19 + nameLen] = rot.RotY;
            data[20 + nameLen] = rot.HeadX;
            WriteU16(0, data, 21 + nameLen); // current item
            return data;
        }

        byte[] MakeEntityTeleport(byte id, Position pos, Orientation rot) {
            int dataLen = 1 + 4 + (4 + 4 + 4) + (1 + 1);
            byte[] data = new byte[dataLen];
            data[0] = OPCODE_TELEPORT;
            // TODO fixes Y kinda
            pos.Y -= 51;

            WriteI32(id, data, 1);
            WriteI32(pos.X, data,  5);
            WriteI32(pos.Y, data,  9);
            WriteI32(pos.Z, data, 13);

            data[17] = (byte)(rot.RotY + 128); // TODO fixed yaw kinda
            data[18] = rot.HeadX;
            return data;
        }

        byte[] MakePreChunk(int x, int z, bool load)
        {
            byte[] data = new byte[1 + 4 + 4 + 1];
            data[0] = OPCODE_PRE_CHUNK;

            WriteI32(x, data, 1);
            WriteI32(z, data, 5);
            data[9] = (byte)(load ? 1 : 0);
            return data;
        }


        byte[] MakeChunk(int x, int z, Level lvl) {
            MemoryStream tmp = new MemoryStream();
            //using (DeflateStream dst = new DeflateStream(tmp, CompressionMode.Compress))
            using (ZLibStream dst = new ZLibStream(tmp))
            {
                byte[] block_data  = new byte[16 * 16 * 128];
                byte[] block_meta  = new byte[(16 * 16 * 128) / 2];
                byte[] block_light = new byte[(16 * 16 * 128) / 2];
                byte[] sky_light   = new byte[(16 * 16 * 128) / 2];

                int height = Math.Min(128, (int)lvl.Height);

                for (int YY = 0; YY < height; YY++)
                    for (int ZZ = 0; ZZ < 16; ZZ++)
                        for (int XX = 0; XX < 16; XX++)
                        {
                            int X = (x * 16) + XX, Y = YY, Z = (z * 16) + ZZ;
                            if (!lvl.IsValidPos(X, Y, Z)) continue;

                            block_data[YY + (ZZ * 128 + (XX * 128 * 16))] = (byte)lvl.GetBlock((ushort)X, (ushort)Y, (ushort)Z);
                        }

                // Make everything insanely bright
                for (int i = 0; i < sky_light.Length; i++) sky_light[i] = 0xFF;

                dst.Write(block_data,  0,  block_data.Length);
                dst.Write(block_meta,  0,  block_meta.Length);
                dst.Write(block_light, 0, block_light.Length);
                dst.Write(sky_light,   0,   sky_light.Length);
            }

            byte[] chunk = tmp.ToArray();
            int dataLen  = 1 + 4 + 2 + 4 + 1 + 1 + 1 + 4 + chunk.Length;
            byte[] data  = new byte[dataLen];

            data[0] = OPCODE_CHUNK;
            WriteI32(x * 16, data, 1); // X/Y/Z chunk origin
            WriteU16(0,      data, 5);
            WriteI32(z * 16, data, 7);
            data[11] = 15;  // X/Y/Z chunk size - 1
            data[12] = 127;
            data[13] = 15;

            WriteI32(chunk.Length, data, 14);
            Array.Copy(chunk, 0, data, 18, chunk.Length);
            return data;
        }

        class ZLibStream : Stream
        {
            Stream underlying;
            DeflateStream dst;
            bool wroteHeader;
            uint s1 = 1, s2 = 0;

            public ZLibStream(Stream tmp)
            {
                underlying = tmp;
                dst = new DeflateStream(tmp, CompressionMode.Compress, true);
            }

            public override bool CanRead { get { return false; } }
            public override bool CanSeek { get { return false; } }
            public override bool CanWrite { get { return true; } }

            static Exception ex = new NotSupportedException();
            public override void Flush() { }
            public override long Length { get { throw ex; } }
            public override long Position { get { throw ex; } set { throw ex; } }
            public override int Read(byte[] buffer, int offset, int count) { throw ex; }
            public override long Seek(long offset, SeekOrigin origin) { throw ex; }
            public override void SetLength(long length) { throw ex; }

            public override void Close() {
                dst.Close();
                WriteFooter();
                base.Close();
            }

            public override void Write(byte[] buffer, int offset, int count) {
                if (!wroteHeader) WriteHeader();

                for (int i = 0; i < count; i++)
                {
                    s1 = (s1 + buffer[offset + i]) % 65521;
                    s2 = (s2 + s1)                 % 65521;
                }
                dst.Write(buffer, offset, count);
            }
            // NOTE: don't call WriteByte because it's imlicitly Write(new byte[] {value}, 0, 1});

            void WriteHeader() {
                byte[] header = new byte[] { 0x78, 0x9C };
                wroteHeader   = true;
                underlying.Write(header, 0, header.Length);
            }

            void WriteFooter() {
                byte[] footer = new byte[4];
                uint adler32 = (s2 << 16) | s1;
                WriteI32((int)adler32, footer, 0);
                underlying.Write(footer, 0, footer.Length);
            }
        }

        public override string ClientName() {
            return "Alpha~";
        }

        public override void SendAddTabEntry(byte id, string name, string nick, string group, byte groupRank)
        {
        }

        public override void SendSetSpawnpoint(Position pos, Orientation rot)
        {
        }
        public override void SendRemoveTabEntry(byte id)
        {
        }
        public override byte[] MakeBulkBlockchange(BufferedBlockSender buffer) {
            int size = 1 + 4 + 1 + 4 + 1 + 1;
            byte[] data = new byte[size * buffer.count];
            Level level = buffer.level;

            for (int i = 0; i < buffer.count; i++)
            {
                int index = buffer.indices[i];
                int x = (index % level.Width);
                int y = (index / level.Width) / level.Length;
                int z = (index / level.Width) % level.Length;

                WriteBlockChange(data, i * size, (byte)buffer.blocks[i], x, y, z);
            }
            return data;
        }

        void WriteBlockChange(byte[] data, int offset, byte block, int x, int y, int z)
        {
            data[offset + 0] = OPCODE_BLOCK_CHANGE;
            WriteI32(x, data, offset + 1);
            data[offset + 5] = (byte)y;
            WriteI32(z, data, offset + 6);
            data[offset + 10] = block;
            data[offset + 11] = 0; // metadata
        }

        public unsafe override void UpdatePlayerPositions() {
            Player[] players = PlayerInfo.Online.Items;
            Player dst = player;
            
            foreach (Player p in players) {
                if (dst == p || dst.level != p.level || !dst.CanSeeEntity(p)) continue;
                
                Orientation rot = p.Rot;
                // TEMP HACK
                Position delta  = Entities.GetDelta(p.tempPos, p.lastPos, p.hasExtPositions);
                bool posChanged = delta.X != 0 || delta.Y != 0 || delta.Z != 0;
                bool oriChanged = rot.RotY != p.lastRot.RotY || rot.HeadX != p.lastRot.HeadX;
                if (posChanged || oriChanged)
                    SendTeleport(p.id, p.tempPos, rot);
            }
        }

        public override ushort ConvertBlock(ushort block)
        {
            return Block.Convert(block);
        }
    }
}