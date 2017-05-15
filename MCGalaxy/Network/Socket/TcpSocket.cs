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
        byte[] leftBuffer = new byte[0];
        byte[] tempbuffer = new byte[0xFF];
        
        public TcpSocket(Player p, Socket s) {
            player = p; socket = s;
        }
        
        public string RemoteIP {
            // TODO: This is very icky
            get { return socket.RemoteEndPoint.ToString().Split(':')[0]; }
        }
        
        
        public void ReceiveNextAsync() {
            socket.BeginReceive(tempbuffer, 0, tempbuffer.Length, SocketFlags.None, new AsyncCallback(Receive), this);
        }
        
        static void Receive(IAsyncResult result) {
            TcpSocket s = (TcpSocket)result.AsyncState;
            Player p = s.player;
            if (p.disconnected) return;
            
            try {
                int length = s.socket.EndReceive(result);
                if (length == 0) { p.Disconnect(); return; }

                byte[] allData = new byte[s.leftBuffer.Length + length];
                Buffer.BlockCopy(s.leftBuffer, 0, allData, 0, s.leftBuffer.Length);
                Buffer.BlockCopy(s.tempbuffer, 0, allData, s.leftBuffer.Length, length);
                s.leftBuffer = p.ProcessReceived(allData);
                
                if (p.dontmindme && s.leftBuffer.Length == 0) {
                    Server.s.Log("Disconnected");
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
                Server.ErrorLog(e);
                p.Leave("Error!");
            }
        }
        
        public void Send(byte[] buffer, bool sync = false) {
            // Abort if socket has been closed
            if (player.disconnected || !socket.Connected) return;
            
            try {
                if (sync)
                    socket.Send(buffer, 0, buffer.Length, SocketFlags.None);
                else
                    socket.BeginSend(buffer, 0, buffer.Length, SocketFlags.None, delegate(IAsyncResult result) { }, null);
                buffer = null;
            } catch (SocketException e) {
                buffer = null;
                player.Disconnect();
                #if DEBUG
                Server.ErrorLog(e);
                #endif
            } catch (ObjectDisposedException) {
                // socket was already closed by another thread.
                buffer = null;
            }
        }
        
        public void Close() {
            // Try to close the socket.
            // Sometimes its already closed so these lines will cause an error
            // We just trap them and hide them from view :P
            try {
                // Close the damn socket connection!
                socket.Shutdown(SocketShutdown.Both);
                #if DEBUG
                Server.s.Log("Socket was shutdown for " + name ?? ip);
                #endif
            } catch (Exception e) {
                #if DEBUG
                Exception ex = new Exception("Failed to shutdown socket for " + name ?? ip, e);
                Server.ErrorLog(ex);
                #endif
            }

            try {
                socket.Close();
                #if DEBUG
                Server.s.Log("Socket was closed for " + name ?? ip);
                #endif
            } catch (Exception e) {
                #if DEBUG
                Exception ex = new Exception("Failed to close socket for " + name ?? ip, e);
                Server.ErrorLog(ex);
                #endif
            }
        }
    }
}
