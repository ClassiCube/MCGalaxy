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
    
    public enum SendFlags {
        None        = 0x00,
        Synchronous = 0x01,
        LowPriority = 0x02,
    }
    
    /// <summary> Abstracts sending to/receiving from a network socket. </summary>
    public abstract class INetSocket {
        protected INetProtocol protocol;        
        /// <summary> Whether the socket has been closed/disconnected. </summary>
        public bool Disconnected;
        
        /// <summary> Gets the IP address of the remote end (i.e. client) of the socket. </summary>
        public abstract IPAddress IP { get; }      
        /// <summary> Sets whether this socket operates in low-latency mode (e.g. for TCP, disabes nagle's algorithm). </summary>
        public abstract bool LowLatency { set; }
        
        /// <summary> Initialises state to begin asynchronously sending and receiving data. </summary>
        public abstract void Init();        
        /// <summary> Sends a block of data. </summary>
        public abstract void Send(byte[] buffer, SendFlags flags);      
        /// <summary> Closes this network socket. </summary>
        public abstract void Close();
        
        internal static VolatileArray<INetSocket> pending = new VolatileArray<INetSocket>(false);

        INetProtocol IdentifyProtocol(byte opcode) {
            if (opcode == Opcode.Handshake) {
                pending.Remove(this);
                return new Player(this);
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
        
        public override IPAddress IP {
            get { return SocketUtil.GetIP(socket); }
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
        public override void Send(byte[] buffer, SendFlags flags) {
            if (Disconnected || !socket.Connected) return;

            // TODO: Low priority sending support
            try {
            	if ((flags & SendFlags.Synchronous) != 0) {
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
                lock (s.sendLock) {
                    // check if last packet was only partially sent
                    for (;;) {
                        int sent  = e.BytesTransferred;
                        int count = e.Count;
                        if (sent >= count || sent <= 0) break;
                        
                        // last packet was only partially sent - resend rest of packet
                        s.sendArgs.SetBuffer(e.Offset + sent, e.Count - sent);
                        s.sendInProgress = s.socket.SendAsync(s.sendArgs);
                        if (s.sendInProgress) return;
                    }
                    
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
            
            // swallow errors as connection is being closed anyways
            try { socket.Shutdown(SocketShutdown.Both); } catch { }
            try { socket.Close(); } catch { }
            
            lock (sendLock) { sendQueue.Clear(); }
            try { recvArgs.Dispose(); } catch { }
            try { sendArgs.Dispose(); } catch { }
        }
    }
    
    /// <summary> Abstracts a WebSocket on top of a socket. </summary>
    public sealed class WebSocket : ServerWebSocket {
        readonly INetSocket s;
        
        public WebSocket(INetSocket socket) { s  = socket; }
        
        // Init taken care by underlying socket
        public override void Init() { }
        public override IPAddress IP { get { return s.IP; } }
        public override bool LowLatency { set { s.LowLatency = value; } }

        protected override void SendRaw(byte[] data, SendFlags flags) {
            s.Send(data, flags);
        } 
        public override void Send(byte[] buffer, SendFlags flags) {
            s.Send(WrapData(buffer), flags);
        }
        
        protected override void HandleData(byte[] data, int len) {
            HandleReceived(data, len);
        }
        
        protected override void OnDisconnected(int reason) {
            if (protocol != null) protocol.Disconnect();
            s.Close();
        }
        
        public override void Close() { s.Close(); }
    }
    
#if SECURE_WEBSOCKETS
// This code is unfinished and experimental, and is terrible quality. I apologise in advance.
    public sealed class SecureSocket : INetSocket, INetProtocol {
        readonly INetSocket raw;
        WrapperStream wrapper;
        SslStream ssl;
        
        public SecureSocket(INetSocket socket) {
            raw = socket;
            
            wrapper = new WrapperStream();
            wrapper.s = this;
            
            ssl = new SslStream(wrapper);
            new Thread(IOThread).Start();
        }
        
        // Init taken care by underlying socket
        public override void Init() { }        
        public override IPAddress IP { get { return raw.IP; } }
        public override bool LowLatency { set { raw.LowLatency = value; } }
        public override void Close() { raw.Close(); }
        public void Disconnect() { Close(); }
        
        // This is an extremely UGLY HACK 
        readonly object locker = new object();
        public override void Send(byte[] buffer, SendFlags flags) {
            try {
                lock (locker) ssl.Write(buffer);
            } catch (Exception ex) {
                Logger.LogError("Error writing to secure stream", ex);
            }
        }
        
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
