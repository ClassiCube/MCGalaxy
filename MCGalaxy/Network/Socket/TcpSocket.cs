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

namespace MCGalaxy.Network {
    
    /// <summary> Abstracts sending to/receiving from a TCP socket. </summary>
    public sealed class TcpSocket : INetworkSocket {
        readonly Player player;
        readonly Socket socket;
        
        byte[] unprocessed = new byte[352];
        byte[] recvBuffer = new byte[256];        
        int unprocessedLen;
        
        byte[] sendBuffer = new byte[1536];
        readonly object sendLock = new object();
        readonly Queue<byte[]> sendQueue = new Queue<byte[]>(64);
        volatile bool sendInProgress;
        
        public TcpSocket(Player p, Socket s) {
            player = p; socket = s;
        }
        
        public string RemoteIP {
            get { return ((IPEndPoint)socket.RemoteEndPoint).Address.ToString(); }
        }
        
        public bool LowLatency {
            set { socket.NoDelay = value; }
        }
        
        
        static AsyncCallback recvCallback = new AsyncCallback(ReceiveCallback);
        public void ReceiveNextAsync() {
            socket.BeginReceive(recvBuffer, 0, recvBuffer.Length, SocketFlags.None, recvCallback, this);
        }
        
        static void ReceiveCallback(IAsyncResult result) {
            TcpSocket s = (TcpSocket)result.AsyncState;
            Player p = s.player;
            if (p.disconnected) return;
            
            try {
                int recvLen = s.socket.EndReceive(result);
                if (recvLen == 0) { p.Disconnect(); return; }

                // Packets may not always be fully received in a Receive call
                // As such, we may need to retain a little bit of partial packet data
                Buffer.BlockCopy(s.recvBuffer, 0, s.unprocessed, s.unprocessedLen, recvLen);
                s.unprocessedLen += recvLen;
                int processedLen = p.ProcessReceived(s.unprocessed, s.unprocessedLen);
                
                // Disconnect invalid clients
                if (p.nonPlayerClient && processedLen == -1) { s.Close(); p.disconnected = true; }
                if (processedLen == -1) return;
                
                // move remaining partial packet data back to start of unprocessed buffer
                for (int i = processedLen; i < s.unprocessedLen; i++) {
                    s.unprocessed[i - processedLen] = s.unprocessed[i];
                }
                s.unprocessedLen -= processedLen;
                
                if (!p.disconnected) s.ReceiveNextAsync();
            } catch (SocketException) {
                p.Disconnect();
            }  catch (ObjectDisposedException) {
                // Socket was closed by another thread, mark as disconnected
                Player.connections.Remove(p);
                p.RemoveFromPending();
                p.disconnected = true;
            } catch (Exception e) {
                Logger.LogError(e);
                p.Leave("Error!");
            }
        }
        
        
        static AsyncCallback sendCallback = new AsyncCallback(SendCallback);
        public void Send(byte[] buffer, bool sync = false) {
            // Abort if socket has been closed
            if (player.disconnected || !socket.Connected) return;

            try {
                if (sync) {
                    socket.Send(buffer, 0, buffer.Length, SocketFlags.None);
                    return;
                }
                
                lock (sendLock) {
                    if (sendInProgress) { sendQueue.Enqueue(buffer); }
                    else { DoSendAsync(buffer); }
                }
            } catch (SocketException) {
                player.Disconnect();
            } catch (ObjectDisposedException) {
                // Socket was already closed by another thread
            }
        }
        
        // TODO: do this seprately
        public void SendLowPriority(byte[] buffer) { Send(buffer, false); }
        
        void DoSendAsync(byte[] buffer) {
            sendInProgress = true;
            // BlockCopy has some overhead, not worth using for very small data
            if (buffer.Length <= 16) {
                for (int i = 0; i < buffer.Length; i++) {
                    sendBuffer[i] = buffer[i];
                }
            } else {
                Buffer.BlockCopy(buffer, 0, sendBuffer, 0, buffer.Length);
            }
            
            socket.BeginSend(sendBuffer, 0, buffer.Length, SocketFlags.None, sendCallback, this);
        }
        
        static void SendCallback(IAsyncResult result) {
            TcpSocket s = (TcpSocket)result.AsyncState;
            try {
                // TODO: Need to check if all data was sent or not?
                int sent = s.socket.EndSend(result);
                lock (s.sendLock) {
                    s.sendInProgress = false;
                    if (s.sendQueue.Count > 0) {
                        s.DoSendAsync(s.sendQueue.Dequeue());
                    }
                }
            } catch (SocketException) {
                s.player.Disconnect();
            } catch (ObjectDisposedException) {
                // Socket was already closed by another thread
            }
        }
        
        
        public void Close() {
            // Try to close the socket. Sometimes socket is already closed, so just hide this.
            try { socket.Shutdown(SocketShutdown.Both); } catch { }
            try { socket.Close(); } catch { }
            lock (sendLock) { sendQueue.Clear(); }
        }
    }
}
