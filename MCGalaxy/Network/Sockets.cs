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
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.IO;
using System.Net.Security;
using System.Threading;
using System.Security.Cryptography.X509Certificates;

namespace MCGalaxy.Network {
    
    /// <summary> Abstracts sending to/receiving from a network socket. </summary>
    public abstract class INetSocket {
        protected INetProtocol protocol;
        
        /// <summary> Whether the socket has been closed/disconnected. </summary>
        public bool Disconnected;
        /// <summary> The IP address of the remote end (i.e. client) of the socket. </summary>
        public string IP;
        
        /// <summary> Sets whether this socket operates in low-latency mode (e.g. for TCP, disabes nagle's algorithm). </summary>
        public abstract bool LowLatency { set; }
        
        /// <summary> Initialises state to begin asynchronously sending and receiving data. </summary>
        public abstract void Init();
        
        /// <summary> Sends a block of data, either synchronously or asynchronously. </summary>
        public abstract void Send(byte[] buffer, bool sync);
        /// <summary> Asynchronously sends a block of low-priority data. </summary>
        public abstract void SendLowPriority(byte[] buffer);
        
        /// <summary> Closes this network socket. </summary>
        public abstract void Close();
        
        internal static VolatileArray<INetSocket> pending = new VolatileArray<INetSocket>(false);

        INetProtocol IdentifyProtocol(byte opcode) {
            if (opcode == Opcode.Handshake) {
                pending.Remove(this);
                return new Player() { ip = IP, Socket = this };
            } else if (opcode == 'G' && Server.Config.WebClient) {
                pending.Remove(this);
                return new WebSocket(this);
            } 
#if SECURE_WEBSOCKETS
            else if (opcode == 0x16 && Server.Config.WebClient) {
                pending.Remove(this);
                return new SecureSocket(this);
            }
#endif
            else {
                Logger.Log(LogType.UserActivity, "Disconnected {0} (unknown opcode {1})", IP, opcode);
                Close();
                return null;
            }
        }
        
        byte[] leftData;
        int leftLen;
        protected void HandleReceived(byte[] data, int len) {
            // Identify the protocol user is connecting with
            // It could be ClassiCube, Minecraft Modern, WebSocket, etc
            if (protocol == null) {
                protocol = IdentifyProtocol(data[0]);
                if (protocol == null) return;
            }
            byte[] src;
            
            // Is there any data leftover from last Receive call?
            if (leftLen == 0) {
                src     = data;
                leftLen = len;
            } else {
                // Yes, combine it with new data
                int totalLen = leftLen + len;
                if (totalLen > leftData.Length) Array.Resize(ref leftData, totalLen);
                
                Buffer.BlockCopy(data, 0, leftData, leftLen, len);
                src     = leftData;
                leftLen = totalLen;
            }
            
            // Packets may not always be fully received
            // Hence may need to retain partial packet data after processing
            int processedLen = protocol.ProcessReceived(src, leftLen);
            leftLen -= processedLen;
            if (leftLen == 0) return;
            
            if (leftData == null || leftLen > leftData.Length) leftData = new byte[leftLen];
            // move remaining partial packet data back to start of leftover/remaining buffer
            for (int i = 0; i < leftLen; i++) {
                leftData[i] = src[processedLen + i];
            }
        }
    }
    
    public interface INetProtocol {
        int ProcessReceived(byte[] buffer, int length);
        void Disconnect();
    }
    
    /// <summary> Abstracts sending to/receiving from a TCP socket. </summary>
    public sealed class TcpSocket : INetSocket {
        readonly Socket socket;        
        byte[] recvBuffer = new byte[256];
        readonly SocketAsyncEventArgs recvArgs = new SocketAsyncEventArgs();
        
        byte[] sendBuffer = new byte[4096];
        readonly object sendLock = new object();
        readonly Queue<byte[]> sendQueue = new Queue<byte[]>(64);
        volatile bool sendInProgress;
        readonly SocketAsyncEventArgs sendArgs = new SocketAsyncEventArgs();
        
        public TcpSocket(Socket s) {
            socket = s;
            IP = ((IPEndPoint)s.RemoteEndPoint).Address.ToString();
            
            recvArgs.UserToken = this;
            recvArgs.SetBuffer(recvBuffer, 0, recvBuffer.Length);
            sendArgs.UserToken = this;
            sendArgs.SetBuffer(sendBuffer, 0, sendBuffer.Length);
        }
        
        public override void Init() {
            recvArgs.Completed += recvCallback;
            sendArgs.Completed += sendCallback;
            ReceiveNextAsync();
        }
        
        public override bool LowLatency { set { socket.NoDelay = value; } }
        
        
        static EventHandler<SocketAsyncEventArgs> recvCallback = RecvCallback;
        void ReceiveNextAsync() {
            // ReceiveAsync returns false if completed sync
            if (!socket.ReceiveAsync(recvArgs)) RecvCallback(null, recvArgs);
        }
        
        static void RecvCallback(object sender, SocketAsyncEventArgs e) {
            TcpSocket s = (TcpSocket)e.UserToken;
            if (s.Disconnected) return;
            
            try {
                // If received 0, means socket was closed
                int recvLen = e.BytesTransferred;
                if (recvLen == 0) { s.Disconnect(); return; }
                
                s.HandleReceived(s.recvBuffer, recvLen);
                if (!s.Disconnected) s.ReceiveNextAsync();
            } catch (SocketException) {
                s.Disconnect();
            } catch (ObjectDisposedException) {
                // Socket was closed by another thread, mark as disconnected
            } catch (Exception ex) {
                Logger.LogError(ex);
                s.Disconnect();
            }
        }
        
        
        static EventHandler<SocketAsyncEventArgs> sendCallback = SendCallback;
        public override void Send(byte[] buffer, bool sync) {
            if (Disconnected || !socket.Connected) return;

            try {
                if (sync) {
                    socket.Send(buffer, 0, buffer.Length, SocketFlags.None);
                    return;
                }
                
                lock (sendLock) {
                    if (sendInProgress) {
                        sendQueue.Enqueue(buffer);
                    } else {
                        sendInProgress = TrySendAsync(buffer);
                    }
                }
            } catch (SocketException) {
                Disconnect();
            } catch (ObjectDisposedException) {
                // Socket was already closed by another thread
            }
        }
        
        // TODO: do this seprately
        public override void SendLowPriority(byte[] buffer) { Send(buffer, false); }
        
        bool TrySendAsync(byte[] buffer) {
            // BlockCopy has some overhead, not worth it for very small data
            if (buffer.Length <= 16) {
                for (int i = 0; i < buffer.Length; i++) {
                    sendBuffer[i] = buffer[i];
                }
            } else {
                Buffer.BlockCopy(buffer, 0, sendBuffer, 0, buffer.Length);
            }

            sendArgs.SetBuffer(0, buffer.Length);
            // SendAsync returns false if completed sync
            return socket.SendAsync(sendArgs);
        }
        
        static void SendCallback(object sender, SocketAsyncEventArgs e) {
            TcpSocket s = (TcpSocket)e.UserToken;
            try {
                // TODO: Need to check if all data was sent or not?
                int sent = e.BytesTransferred;
                lock (s.sendLock) {
                    s.sendInProgress = false;

                    while (s.sendQueue.Count > 0) {
                        // DoSendAsync returns false if SendAsync completed sync
                        // If that happens, SendCallback isn't called so we need to send data here instead
                        s.sendInProgress = s.TrySendAsync(s.sendQueue.Dequeue());
                        
                        if (s.sendInProgress) return;
                        if (s.Disconnected) s.sendQueue.Clear();
                    }
                }
            } catch (SocketException) {
                s.Disconnect();
            } catch (ObjectDisposedException) {
                // Socket was already closed by another thread
            } catch (Exception ex) {
                Logger.LogError(ex);
            }
        }
        
        // Close while also notifying higher level (i.e. show 'X disconnected' in chat)
        void Disconnect() {
            if (protocol != null) protocol.Disconnect();
            Close();
        }
        
        public override void Close() {
            Disconnected = true;
            pending.Remove(this);
            
            // Try to close the socket. Sometimes socket is already closed, so just hide this.
            try { socket.Shutdown(SocketShutdown.Both); } catch { }
            try { socket.Close(); } catch { }
            
            lock (sendLock) { sendQueue.Clear(); }
            try { recvArgs.Dispose(); } catch { }
            try { sendArgs.Dispose(); } catch { }
        }
    }
    
    /// <summary> Abstracts a WebSocket on top of a socket. </summary>
    public sealed class WebSocket : INetSocket, INetProtocol {
        readonly INetSocket s;
        
        public WebSocket(INetSocket socket) {
            IP = socket.IP;
            s  = socket;
        }
        
        // Init taken care by underlying socket
        public override void Init() { }
        public override bool LowLatency { set { s.LowLatency = value; } }
        
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
            s.Send(Encoding.ASCII.GetBytes(headers), false);
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
                    HandleReceived(frame, frameLen);
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
        
        static byte[] WrapData(byte[] data) {
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
        
        public override void Send(byte[] buffer, bool sync) {
            s.Send(WrapData(buffer), sync);
        }
        public override void SendLowPriority(byte[] buffer) {
            s.SendLowPriority(WrapData(buffer));
        }
        
        void Disconnect(int reason) {
            byte[] packet = new byte[4];
            packet[0] = 0x88; // FIN BIT, close opcode
            packet[1] = 2;
            packet[2] = (byte)(reason >> 8);
            packet[3] = (byte)reason;
            
            if (protocol != null) protocol.Disconnect();
            s.Send(packet, true);
            s.Close();
        }
        
        public void Disconnect() { Disconnect(1000); }
        public override void Close() { s.Close(); }
    }
    
#if SECURE_WEBSOCKETS
// This code is unfinished and experimental, and is terrible quality. I apologise in advance.
    public sealed class SecureSocket : INetSocket, INetProtocol {
        readonly INetSocket raw;
        WrapperStream wrapper;
        SslStream ssl;
        
        public SecureSocket(INetSocket socket) {
            IP  = socket.IP;
            raw = socket;
            
            wrapper = new WrapperStream();
            wrapper.s = this;
            
            ssl = new SslStream(wrapper);
            new Thread(IOThread).Start();
        }
        
        // Init taken care by underlying socket
        public override void Init() { }
        public override bool LowLatency { set { raw.LowLatency = value; } }
        public override void Close() { raw.Close(); }
        public void Disconnect() { Close(); }
        
        // This is an extremely UGLY HACK 
        readonly object locker = new object();
        public override void Send(byte[] buffer, bool sync) {
            try {
                lock (locker) ssl.Write(buffer);
            } catch (Exception ex) {
                Logger.LogError("Error writing to secure stream", ex);
            }
        }
        public override void SendLowPriority(byte[] buffer) { Send(buffer, false); }
        
        public int ProcessReceived(byte[] data, int count) {
            lock (wrapper.locker) {
                for (int i = 0; i < count; i++) {
                    wrapper.input.Add(data[i]);
                }
            }
            return count;
        }
        
        void IOThread() {
            try {
                // UGLY HACK I don't know what this file should even contain??? seems you need public and private key
                X509Certificate2 cert = new X509Certificate2(Server.Config.SslCertPath, Server.Config.SslCertPass);
                ssl.AuthenticateAsServer(cert);
                
                Server.s.Log(".. reading player packets ..");
                byte[] buffer = new byte[4096];
                for (;;) {
                    int read = ssl.Read(buffer, 0, 4096);
                    if (read == 0) break;
                    this.HandleReceived(buffer, read);
                }
            } catch (Exception ex) {
                Logger.LogError("Error reading from secure stream", ex);
            }
        }
        
        // UGLY HACK because can't derive from two base classes
        sealed class WrapperStream : Stream {
            public SecureSocket s;
            public readonly object locker = new object();
            public readonly List<byte> input = new List<byte>();
            
            public override bool CanRead { get { return true; } }
            public override bool CanSeek { get { return false; } }
            public override bool CanWrite { get { return true; } }
            
            static Exception ex = new NotSupportedException();
            public override void Flush() { }
            public override long Length { get { throw ex; } }
            public override long Position { get { throw ex; } set { throw ex; } }
            public override long Seek(long offset, SeekOrigin origin) { throw ex; }
            public override void SetLength(long length) { throw ex; }
            
            public override int Read(byte[] buffer, int offset, int count) {
                // UGLY HACK wait until got some data
                for (;;) {
                    lock (locker) { if (input.Count > 0) break; }
                    Thread.Sleep(1);
                }
                
                // now actually output the data
                lock (locker) {
                    count = Math.Min(count, input.Count);
                    for (int i = 0; i < count; i++) {
                        buffer[offset++] = input[i];
                    }
                    input.RemoveRange(0, count);
                }
                return count;
            }
            
            public override void Write(byte[] buffer, int offset, int count) {
                byte[] data = new byte[count];
                Buffer.BlockCopy(buffer, offset, data, 0, count);
                s.raw.Send(data, false);
            }
        }
    }
#endif
}
