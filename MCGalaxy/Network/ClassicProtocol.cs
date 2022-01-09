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
using MCGalaxy.Events.PlayerEvents;
using MCGalaxy.Events.ServerEvents;
using BlockID = System.UInt16;

namespace MCGalaxy.Network
{
    public class ClassicProtocol : INetProtocol 
    {
        Player p;
        INetSocket socket;
        bool finishedCpeLogin;
        int extensionCount;

        public ClassicProtocol(INetSocket s) {
            socket = s;
            p = new Player(s, this);
        }

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

        int HandlePacket(byte[] buffer, int offset, int left) {
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
                    p.Leave("Unhandled opcode \"" + buffer[offset] + "\"!", true);
                    return -1;
            }
        }
        
        #if TEN_BIT_BLOCKS
        BlockID ReadBlock(byte[] buffer, int offset) {
            BlockID block;
            if (p.hasExtBlocks) {
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

        public void Disconnect() { p.Disconnect(); }


#region Classic processing
        int HandleLogin(byte[] buffer, int offset, int left) {
            // protocol versions < 6 didn't have the usertype field,
            //  hence this two-packet-size-handling monstrosity
            const int old_size = 1 + 1 + 64 + 64;
            const int new_size = 1 + 1 + 64 + 64 + 1;
            // the packet must be at least old_size long
            if (left < old_size) return 0;  
            
            p.ProtocolVersion = buffer[offset + 1];
            // check size now that know whether usertype field is included or not
            int size = p.ProtocolVersion >= Server.VERSION_0020 ? new_size : old_size;
            if (left < size) return 0;
            if (p.loggedIn)  return size;

            // usertype field has different meanings depending on protocol version
            //  Version 7 - 0x42 for CPE supporting client, should be 0 otherwise
            //  Version 6 - should be 0
            //  Version 5 - field does not exist
            if (p.ProtocolVersion >= Server.VERSION_0030) {
                p.hasCpe = buffer[offset + 130] == 0x42 && Server.Config.EnableCPE;
            }
            
            string name   = NetUtils.ReadString(buffer, offset +  2);
            string mppass = NetUtils.ReadString(buffer, offset + 66);
            return p.ProcessLogin(name, mppass) ? size : -1;
        }
        
        int HandleBlockchange(byte[] buffer, int offset, int left) {
            int size = 1 + 6 + 1 + (p.hasExtBlocks ? 2 : 1);
            if (left < size) return 0;
            if (!p.loggedIn) return size;

            ushort x = NetUtils.ReadU16(buffer, offset + 1);
            ushort y = NetUtils.ReadU16(buffer, offset + 3);
            ushort z = NetUtils.ReadU16(buffer, offset + 5);

            byte action = buffer[offset + 7];
            if (action > 1) {
                p.Leave("Unknown block action!", true); return -1;
            }

            BlockID held = ReadBlock(buffer, offset + 8);
            p.ProcessBlockchange(x, y, z, action, held);
            return size;
        }
        
        int HandleMovement(byte[] buffer, int offset, int left) {
            int size = 1 + 6 + 2 + (p.hasExtPositions ? 6 : 0) + (p.hasExtBlocks ? 2 : 1);
            if (left < size) return 0;
            if (!p.loggedIn) return size;

            int held = -1;
            if (p.Supports(CpeExt.HeldBlock)) {
                held = ReadBlock(buffer, offset + 1);
                if (p.hasExtBlocks) offset++; // correct offset for position later
            }
            
            int x, y, z;
            if (p.hasExtPositions) {
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
            p.ProcessMovement(x, y, z, yaw, pitch, held);
            return size;
        }
        
        int HandleChat(byte[] buffer, int offset, int left) {
            const int size = 1 + 1 + 64;
            if (left < size) return 0;
            if (!p.loggedIn) return size;
            
            byte continued = buffer[offset + 1];
            string text    = NetUtils.ReadString(buffer, offset + 2);
            p.ProcessChat(text, continued);
            return size;
        }
#endregion


#region CPE processing
        void CheckReadAllExtensions() {
            if (extensionCount <= 0 && !finishedCpeLogin) {
                p.CompleteLoginProcess();
                finishedCpeLogin = true;
            }
        }

        int HandleExtInfo(byte[] buffer, int offset, int left) {
            const int size = 1 + 64 + 2;
            if (left < size) return 0;
            
            p.appName      = NetUtils.ReadString(buffer, offset + 1);
            extensionCount = buffer[offset + 66];
            CheckReadAllExtensions(); // in case client supports 0 CPE packets
            return size;
        }

        int HandleExtEntry(byte[] buffer, int offset, int left) {
            const int size = 1 + 64 + 4;
            if (left < size) return 0;
            
            string extName = NetUtils.ReadString(buffer, offset + 1);
            int extVersion = NetUtils.ReadI32(buffer,    offset + 65);
            p.AddExtension(extName, extVersion);
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
            OnPlayerClickEvent.Call(p, Button, Action, yaw, pitch, entityID, x, y, z, face);
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
                p.Ping.Update(data);
            }
            return size;
        }

        int HandlePluginMessage(byte[] buffer, int offset, int left) {
            const int size = 1 + 1 + Packet.PluginMessageDataLength;
            if (left < size) return 0;

            byte channel = buffer[offset + 1];
            byte[] data  = new byte[Packet.PluginMessageDataLength];
            Array.Copy(buffer, offset + 2, data, 0, Packet.PluginMessageDataLength);
            OnPluginMessageReceivedEvent.Call(p, channel, data);

            return size;
        }
#endregion


#region Classic packet sending
        public void SendTeleport(byte id, Position pos, Orientation rot) {
            // NOTE: Classic clients require offseting own entity by 22 units vertically
            if (id == Entities.SelfID) pos.Y -= 22;

            Send(Packet.Teleport(Entities.SelfID, pos, rot, p.hasExtPositions));
        }

        public void SendRemoveEntity(byte id) {
            Send(Packet.RemoveEntity(id));
        }

        public void SendChat(byte type, string message) {
            List<string> lines = LineWrapper.Wordwrap(message, p.hasEmoteFix);

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
                    Packet.WriteMessage(lines[i], type, p.hasCP437, data, j * 66);
                }
                Send(data);
            }
        }

        public void SendMessage(CpeMessageType type, string message) {
            Send(Packet.Message(message, type, p.hasCP437));
        }
        
        public void SendKick(string reason, bool sync) {
            byte[] buffer = Packet.Kick(reason, p.hasCP437);
            socket.Send(buffer, sync ? SendFlags.Synchronous : SendFlags.None);
        }

        public bool SendSetUserType(byte type) {
            // this packet doesn't exist before protocol version 7
            if (p.ProtocolVersion < Server.VERSION_0030) return false;

            Send(Packet.UserType(type));
            return true;
        }
#endregion


#region CPE packet sending
        public bool SendSetReach(float reach) {
            if (!p.Supports(CpeExt.HeldBlock)) return false;

            Send(Packet.ClickDistance((short)(reach * 32)));
            return true;
        }

        public bool SendHoldThis(BlockID block, bool locked) {
            if (!p.Supports(CpeExt.HeldBlock)) return false;

            BlockID raw = p.ConvertBlock(block);
            Send(Packet.HoldThis(raw, locked, p.hasExtBlocks));
            return true;
        }

        public bool SendSetEnvColor(byte type, string hex) {
            if (!p.Supports(CpeExt.EnvColors)) return false;

            ColorDesc c;
            if (Colors.TryParseHex(hex, out c)) {
                Send(Packet.EnvColor(type, c.R, c.G, c.B));
            } else {
                Send(Packet.EnvColor(type, -1, -1, -1));
            }
            return true;
        }

        public bool SendSetWeather(byte weather) {
            if (!p.Supports(CpeExt.EnvWeatherType)) return false;

            Send(Packet.EnvWeatherType(weather));
            return true;
        }
#endregion


#region Higher level sending
        public void SendPing() {
            if (p.hasTwoWayPing) {
                Send(Packet.TwoWayPing(true, p.Ping.NextTwoWayPingData()));
            } else {
                Send(Packet.Ping());
            }
        }

        public void SendSpawnEntity(byte id, string name, string skin, Position pos, Orientation rot) {
            // NOTE: Classic clients require offseting own entity by 22 units vertically
            if (id == Entities.SelfID) pos.Y -= 22;

            if (p.Supports(CpeExt.ExtPlayerList, 2)) {
                Send(Packet.ExtAddEntity2(id, skin, name, pos, rot, p.hasCP437, p.hasExtPositions));
            } else if (p.hasExtList) {
                Send(Packet.ExtAddEntity(id, skin, name, p.hasCP437));
                Send(Packet.Teleport(id, pos, rot, p.hasExtPositions));
            } else {
                Send(Packet.AddEntity(id, name, pos, rot, p.hasCP437, p.hasExtPositions));
            }
        }
#endregion


        /// <summary> Returns an appropriate name for the associated player's client </summary>
        /// <remarks> Determines name based on appname or protocol version supported </remarks>
        public string ClientName() {
            if (!string.IsNullOrEmpty(p.appName)) return p.appName;
            byte version = p.ProtocolVersion;
                  
            if (version == Server.VERSION_0016) return "Classic 0.0.16";
            if (version == Server.VERSION_0017) return "Classic 0.0.17-0.0.18";
            if (version == Server.VERSION_0019) return "Classic 0.0.19";
            if (version == Server.VERSION_0020) return "Classic 0.0.20-0.0.23";
            
            // Might really be Classicube in Classic Mode, Charged Miners, etc though
            return "Classic 0.28-0.30";
        }
    }
}
