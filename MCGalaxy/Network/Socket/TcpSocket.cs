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
using System.Net;
using System.Net.Sockets;

namespace MCGalaxy.Network {
    
    /// <summary> Abstracts sending to/receiving from a TCP socket. </summary>
    public sealed class TcpSocket : INetworkSocket {
        readonly Player player;
        readonly Socket socket;
        byte[] unprocessed = new byte[0];
        byte[] recvBuffer = new byte[256];
        
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
                int length = s.socket.EndReceive(result);
                if (length == 0) { p.Disconnect(); return; }

                byte[] allData = new byte[s.unprocessed.Length + length];
                Buffer.BlockCopy(s.unprocessed, 0, allData, 0, s.unprocessed.Length);
                Buffer.BlockCopy(s.recvBuffer, 0, allData, s.unprocessed.Length, length);
                s.unprocessed = p.ProcessReceived(allData);
                
                if (p.nonPlayerClient && s.unprocessed.Length == 0) {
                    s.Close();
                    p.disconnected = true;
                    return;
                }
                if (!p.disconnected) s.ReceiveNextAsync();
            } catch (SocketException) {
                p.Disconnect();
            }  catch (ObjectDisposedException) {
                // Player is no longer connected, socket was closed
                // Mark this as disconnected and remove them from active connection list
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
                if (sync)
                    socket.Send(buffer, 0, buffer.Length, SocketFlags.None);
                else
                    socket.BeginSend(buffer, 0, buffer.Length, SocketFlags.None, sendCallback, this);
                buffer = null;
            } catch (SocketException e) {
                buffer = null;
                player.Disconnect();
                #if DEBUG
                Logger.LogError(e);
                #endif
            } catch (ObjectDisposedException) {
                // socket was already closed by another thread.
                buffer = null;
            }
        }
        
        // TODO: do this seprately
        public void SendLowPriority(byte[] buffer, bool sync = false) {
            Send(buffer, sync);
        }
        
        static void SendCallback(IAsyncResult result) {
            // TODO: call EndSend, need to check if all data was sent or not!
            /*TcpSocket s = (TcpSocket)result.AsyncState;
            
            try {
                int sent = s.socket.EndSend(result);
            } catch (SocketException e) {
                s.player.Disconnect();
                #if DEBUG
                Logger.LogError(e);
                #endif
            } catch (ObjectDisposedException) {
                // socket was already closed by another thread.
            }*/
        }
        
        
        public void Close() {
            // Try to close the socket. Sometimes socket is already closed, so just hide this.
            #if !DEBUG
            try { socket.Shutdown(SocketShutdown.Both); } catch { }
            try { socket.Close(); } catch { }
            
            #else
            try {
                socket.Shutdown(SocketShutdown.Both);
                Logger.Log(LogType.Debug, "Socket was shutdown for " + name ?? ip);
            } catch (Exception e) {
                Exception ex = new Exception("Failed to shutdown socket for " + name ?? ip, e);
                Logger.LogError(ex);
            }
            
            try {
                socket.Close();
                Logger.Log(LogType.Debug, "Socket was closed for " + name ?? ip);
            } catch (Exception e) {
                Exception ex = new Exception("Failed to close socket for " + name ?? ip, e);
                Logger.LogError(ex);
            }
            #endif
        }
    }
}
