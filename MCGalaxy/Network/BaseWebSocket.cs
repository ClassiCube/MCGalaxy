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
using System.Security.Cryptography;
using System.Text;

namespace MCGalaxy.Network {
    
    /// <summary> Abstracts WebSocket handling. </summary>
    public abstract class BaseWebSocket : INetSocket, INetProtocol {        
        bool readingHeaders = true;
        bool conn, upgrade, version, proto;
        string verKey;
        
        void AcceptConnection() {
            const string fmt =
                "HTTP/1.1 101 Switching Protocols\r\n" +
                "Upgrade: websocket\r\n" +
                "Connection: Upgrade\r\n" +
                "Sec-WebSocket-Accept: {0}\r\n" +
                "Sec-WebSocket-Protocol: ClassiCube\r\n" +
                "\r\n";
            
            string key = verKey + "258EAFA5-E914-47DA-95CA-C5AB0DC85B11";
            SHA1   sha = SHA1.Create();
            byte[] raw = sha.ComputeHash(Encoding.ASCII.GetBytes(key));
            
            string headers = String.Format(fmt, Convert.ToBase64String(raw));
            SendRaw(Encoding.ASCII.GetBytes(headers), false);
            readingHeaders = false;
        }
        
        void ProcessHeader(string raw) {
            // end of headers
            if (raw.Length == 0) {
                if (conn && upgrade && version && proto && verKey != null) {
                    AcceptConnection();
                } else {
                    // don't pretend to be a http server (so IP:port isn't marked as one by bots)
                    Close();
                }
            }
            
            int sep = raw.IndexOf(':');
            if (sep == -1) return; // not a proper header
            string key = raw.Substring(0, sep);
            string val = raw.Substring(sep + 1).Trim();
            
            if (key.CaselessEq("Connection")) {
                conn    = val.CaselessContains("Upgrade");
            } else if (key.CaselessEq("Upgrade")) {
                upgrade = val.CaselessEq("websocket");
            } else if (key.CaselessEq("Sec-WebSocket-Version")) {
                version = val.CaselessEq("13");
            } else if (key.CaselessEq("Sec-WebSocket-Key")) {
                verKey  = val;
            } else if (key.CaselessEq("Sec-WebSocket-Protocol")) {
                proto   = val.CaselessEq("ClassiCube");
            }
        }
        
        int ReadHeaders(byte[] buffer, int bufferLen) {
            int i;
            for (i = 0; i < bufferLen - 1; ) {
                int end = -1;
                
                // find end of header
                for (int j = i; j < bufferLen - 1; j++) {
                    if (buffer[j] != '\r' || buffer[j + 1] != '\n') continue;
                    end = j; break;
                }
                
                if (end == -1) break;
                string value = Encoding.ASCII.GetString(buffer, i, end - i);
                ProcessHeader(value);
                i = end + 2;
            }
            return i;
        }
        
        int state, opcode, frameLen, maskRead, frameRead;
        byte[] mask = new byte[4], frame;
        
        const int state_header1 = 0;
        const int state_header2 = 1;
        const int state_extLen1 = 2;
        const int state_extLen2 = 3;
        const int state_mask    = 4;
        const int state_data    = 5;
        
        void DecodeFrame() {
            for (int i = 0; i < frameLen; i++) {
                frame[i] ^= mask[i & 3];
            }
            
            switch (opcode) {
                    // TODO: reply to ping frames
                case 0x00:
                case 0x02:
                    if (frameLen == 0) return;
                    HandleData(frame, frameLen);
                    break;
                case 0x08:
                    // Connection is getting closed
                    Disconnect(1000); break;
                default:
                    Disconnect(1003); break;
            }
        }
        
        int ProcessData(byte[] data, int offset, int len) {
            switch (state) {
                case state_header1:
                    if (offset >= len) break;
                    opcode = data[offset++] & 0x0F;
                    state  = state_header2;
                    goto case state_header2;
                    
                case state_header2:
                    if (offset >= len) break;
                    int flags = data[offset++] & 0x7F;
                    
                    if (flags == 127) {
                        // unsupported 8 byte extended length
                        Disconnect(1009);
                        return len;
                    } else if (flags == 126) {
                        // two byte extended length
                        state = state_extLen1;
                        goto case state_extLen1;
                    } else {
                        // length is inline
                        frameLen = flags;
                        state    = state_mask;
                        goto case state_mask;
                    }
                    
                case state_extLen1:
                    if (offset >= len) break;
                    frameLen = data[offset++] << 8;
                    state    = state_extLen2;
                    goto case state_extLen2;
                    
                case state_extLen2:
                    if (offset >= len) break;
                    frameLen |= data[offset++];
                    state     = state_mask;
                    goto case state_mask;
                    
                case state_mask:
                    for (; maskRead < 4; maskRead++) {
                        if (offset >= len) return offset;
                        mask[maskRead] = data[offset++];
                    }
                    
                    state = state_data;
                    goto case state_data;
                    
                case state_data:
                    if (frame == null || frameLen > frame.Length) frame = new byte[frameLen];
                    int copy = Math.Min(len - offset, frameLen - frameRead);
                    
                    Buffer.BlockCopy(data, offset, frame, frameRead, copy);
                    offset += copy; frameRead += copy;
                    
                    if (frameRead == frameLen) {
                        DecodeFrame();
                        maskRead  = 0;
                        frameRead = 0;
                        state     = state_header1;
                    }
                    break;
            }
            return offset;
        }
        
        int INetProtocol.ProcessReceived(byte[] buffer, int bufferLen) {
            int offset = 0;
            if (readingHeaders) {
                offset = ReadHeaders(buffer, bufferLen);
                if (readingHeaders) return offset;
            }
            
            while (offset < bufferLen) {
                offset = ProcessData(buffer, offset, bufferLen);
            }
            return offset;
        }
        
        protected static byte[] WrapData(byte[] data) {
            int headerLen = 2 + (data.Length >= 126 ? 2 : 0);
            byte[] packet = new byte[headerLen + data.Length];
            packet[0] = 0x82; // FIN bit, binary opcode
            
            if (headerLen > 2) {
                packet[1] = 126;
                packet[2] = (byte)(data.Length >> 8);
                packet[3] = (byte)data.Length;
            } else {
                packet[1] = (byte)data.Length;
            }
            Buffer.BlockCopy(data, 0, packet, headerLen, data.Length);
            return packet;
        }
        
        protected virtual void Disconnect(int reason) {
            byte[] packet = new byte[4];
            packet[0] = 0x88; // FIN BIT, close opcode
            packet[1] = 2;
            packet[2] = (byte)(reason >> 8);
            packet[3] = (byte)reason;           
            SendRaw(packet, true);
        }
        
        protected abstract void HandleData(byte[] data, int len);
        
        protected abstract void SendRaw(byte[] data, bool sync);
        
        public void Disconnect() { Disconnect(1000); }
    }
}
