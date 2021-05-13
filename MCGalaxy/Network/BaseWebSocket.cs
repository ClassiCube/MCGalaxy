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
    
    /// <summary> Abstracts WebSocket handling </summary>
    public abstract class BaseWebSocket : INetSocket, INetProtocol {        
        protected bool conn, upgrade;
        protected bool readingHeaders = true;
        
        /// <summary> Computes a base64-encoded handshake verification key </summary>
        protected static string ComputeKey(string rawKey) {
            string key = rawKey + "258EAFA5-E914-47DA-95CA-C5AB0DC85B11";
            SHA1   sha = SHA1.Create();
            byte[] raw = sha.ComputeHash(Encoding.ASCII.GetBytes(key));
            return Convert.ToBase64String(raw);
        }
        
        protected abstract void OnGotAllHeaders();
        protected abstract void OnGotHeader(string key, string val);

        void ProcessHeader(string raw) {
            // end of all headers
            if (raw.Length == 0) OnGotAllHeaders();
            
            // check that got a proper header
            int sep = raw.IndexOf(':');
            if (sep == -1) return;
            
            string key = raw.Substring(0, sep);
            string val = raw.Substring(sep + 1).Trim();
            
            if (key.CaselessEq("Connection")) {
                conn    = val.CaselessContains("Upgrade");
            } else if (key.CaselessEq("Upgrade")) {
                upgrade = val.CaselessEq("websocket");
            } else {
                OnGotHeader(key, val);
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
        
        protected const int OPCODE_CONTINUED  = 0;
        protected const int OPCODE_TEXT       = 1;
        protected const int OPCODE_BINARY     = 2;
        protected const int OPCODE_DISCONNECT = 8;        
        protected const int FIN = 0x80;
        
        protected const int REASON_NORMAL         = 1000;
        protected const int REASON_INVALID_DATA   = 1003;
        protected const int REASON_EXCESSIVE_SIZE = 1009;
        
        int GetDisconnectReason() {
            if (frameLen < 2) return REASON_NORMAL;
            
            // See section 5.5.1 of websockets specification:
            //  "... If there is a body, the first two bytes of the body MUST 
            //   be a 2-byte unsigned integer (in network byte order)..."
            return (frame[0] << 8) | frame[1];
        }
        
        void DecodeFrame() {
            for (int i = 0; i < frameLen; i++) {
                frame[i] ^= mask[i & 3];
            }
            
            switch (opcode) {
                    // TODO: reply to ping frames
                case OPCODE_CONTINUED:
                case OPCODE_BINARY:
                case OPCODE_TEXT:
                    if (frameLen == 0) return;
                    HandleData(frame, frameLen);
                    break;
                case OPCODE_DISCONNECT:
                    // Connection is getting closed
                    Disconnect(GetDisconnectReason()); break;
                default:
                    Disconnect(REASON_INVALID_DATA); break;
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
                    int flags = data[offset] & 0x7F;
                    // if mask bit is     zero: maskRead is set to 0x80 (therefore skipping reading the 4 bytes)
                    // if mask bit is non-zero: maskRead is set to 0x00 (therefore actually reading the data)
                    maskRead  = 0x80 - (data[offset] & 0x80);                    
                    offset++;
                    
                    if (flags == 127) {
                        // unsupported 8 byte extended length
                        Disconnect(REASON_EXCESSIVE_SIZE);
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
                    
                    maskRead = 0;
                    state    = state_data;
                    goto case state_data;
                    
                case state_data:
                    if (frame == null || frameLen > frame.Length) frame = new byte[frameLen];
                    int copy = Math.Min(len - offset, frameLen - frameRead);
                    
                    Buffer.BlockCopy(data, offset, frame, frameRead, copy);
                    offset += copy; frameRead += copy;
                    
                    if (frameRead == frameLen) {
                        DecodeFrame();
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
        
        protected static byte[] WrapDisconnect(int reason) {
            byte[] packet = new byte[4];
            packet[0] = OPCODE_DISCONNECT | FIN;
            packet[1] = 2;
            packet[2] = (byte)(reason >> 8);
            packet[3] = (byte)reason;
            return packet;
        }
        
        protected virtual void Disconnect(int reason) {
            SendRaw(WrapDisconnect(reason), SendFlags.Synchronous);
        }
        
        protected abstract void HandleData(byte[] data, int len);
        
        /// <summary> Sends data to the underlying socket without wrapping the data in a websocket frame </summary>
        protected abstract void SendRaw(byte[] data, SendFlags flags);
        
        public void Disconnect() { Disconnect(REASON_NORMAL); }
    }
    
    /// <summary> Abstracts a server side WebSocket </summary>
    public abstract class ServerWebSocket : BaseWebSocket {
        bool version;
        string verKey;
        
        void AcceptConnection() {
            const string fmt =
                "HTTP/1.1 101 Switching Protocols\r\n" +
                "Upgrade: websocket\r\n" +
                "Connection: Upgrade\r\n" +
                "Sec-WebSocket-Accept: {0}\r\n" +
                "Sec-WebSocket-Protocol: ClassiCube\r\n" +
                "\r\n";
            
            string key = ComputeKey(verKey);
            string headers = String.Format(fmt, key);
            SendRaw(Encoding.ASCII.GetBytes(headers), SendFlags.None);
            readingHeaders = false;
        }
        
        protected override void OnGotAllHeaders() {
            if (conn && upgrade && version && verKey != null) {
                AcceptConnection();
            } else {
                // don't pretend to be a http server (so IP:port isn't marked as one by bots)
                Close();
            }
        }
        
        protected override void OnGotHeader(string key, string val) {
            if (key.CaselessEq("Sec-WebSocket-Version")) {
                version = val.CaselessEq("13");
            } else if (key.CaselessEq("Sec-WebSocket-Key")) {
                verKey  = val;
            }
        }
        
        /// <summary> Wraps the given data in a websocket frame </summary>
        protected static byte[] WrapData(byte[] data) {
            int headerLen = data.Length >= 126 ? 4 : 2;
            byte[] packet = new byte[headerLen + data.Length];
            packet[0] = OPCODE_BINARY | FIN;
            
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
    }
    
    /// <summary> Abstracts a client side WebSocket </summary>
    public abstract class ClientWebSocket : BaseWebSocket {
        protected string path = "/";
        string verKey;
        // TODO: use a random securely generated key
        const string key = "xTNDiuZRoMKtxrnJDWyLmA==";
        
        void AcceptConnection() {
            readingHeaders = false;
        }
        
        protected override void OnGotAllHeaders() {
            if (conn && upgrade && verKey == ComputeKey(key)) {
                AcceptConnection();
            } else {
                // don't pretend to be a http server (so IP:port isn't marked as one by bots)
                Close();
            }
        }
        
        protected override void OnGotHeader(string key, string val) {
            if (key.CaselessEq("Sec-WebSocket-Accept")) {
                verKey = val;
            }
        }
        
        /// <summary> Wraps the given data in a websocket frame </summary>
        protected static byte[] WrapData(byte[] data) {
            int headerLen = data.Length >= 126 ? 4 : 2;
            byte[] packet = new byte[headerLen + 4 + data.Length];
            packet[0] = OPCODE_TEXT | FIN;
            
            if (headerLen > 2) {
                packet[1] = 126;
                packet[2] = (byte)(data.Length >> 8);
                packet[3] = (byte)data.Length;
            } else {
                packet[1] = (byte)data.Length;
            }
            packet[1] |= 0x80;
            Buffer.BlockCopy(data, 0, packet, headerLen + 4, data.Length);
            return packet;
        }
        
        public override void Send(byte[] buffer, SendFlags flags) {
            SendRaw(WrapData(buffer), flags);
        }
        
        
        protected void WriteHeader(string header) {
            SendRaw(Encoding.ASCII.GetBytes(header + "\r\n"), SendFlags.None);
        }
        
        protected virtual void WriteCustomHeaders() { }
        
        public override void Init() {
            WriteHeader("GET " + path + " HTTP/1.1");
            WriteHeader("Upgrade: websocket");
            WriteHeader("Connection: Upgrade");
            WriteHeader("Sec-WebSocket-Version: 13");
            WriteHeader("Sec-WebSocket-Key: " + key);
            WriteCustomHeaders();
            WriteHeader("");
        }
    }
}
