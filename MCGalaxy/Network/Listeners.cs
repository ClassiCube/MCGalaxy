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
    
    /// <summary> Abstracts listening on network socket. </summary>
    public abstract class INetListen {
        /// <summary> The IP address this network socket is listening on. </summary>
        public IPAddress IP;
        /// <summary> The port this network socket is listening on. </summary>
        public int Port;

        /// <summary> Begins listening for connections on the given IP and port. </summary>
        /// <remarks> Client connections are asynchronously accepted. </remarks>
        public abstract void Listen(IPAddress ip, int port);
        
        /// <summary> Closes this network listener. </summary>
        public abstract void Close();
    }
    
    /// <summary> Abstracts listening on a TCP socket. </summary>
    public sealed class TcpListen : INetListen {
        Socket socket;
        
        void AcceptIPV4OnIPV6Listener() {
            if (socket.AddressFamily != AddressFamily.InterNetworkV6) return;
            
#if !NET_20
            try {
                socket.SetSocketOption(SocketOptionLevel.IPv6, SocketOptionName.IPv6Only, false);
            } catch (Exception ex) {
                Logger.LogError(ex);
                Logger.Log(LogType.Warning, "Failed to allow IPv4 connections to IPv6 listener ({0})", ex.Message);
            }
#endif
        }
        
        public override void Listen(IPAddress ip, int port) {
            if (IP == ip && Port == port) return;
            Close();
            IP = ip; Port = port;
            
            try {
                socket = new Socket(ip.AddressFamily, SocketType.Stream, ProtocolType.Tcp); 
                AcceptIPV4OnIPV6Listener();                
                socket.Bind(new IPEndPoint(ip, port));
                
                socket.Listen((int)SocketOptionName.MaxConnections);
                AcceptNextAsync();
            } catch (Exception ex) {
                Logger.LogError(ex);
                Logger.Log(LogType.Warning, "Failed to start listening on port {0} ({1})", port, ex.Message);
                socket = null; return;
            }
            Logger.Log(LogType.SystemActivity, "Started listening on port {0}... ", port);
        }
        
        void AcceptNextAsync() {
            // retry, because if we don't call BeginAccept, no one can connect anymore
            for (int i = 0; i < 3; i++) {
                try {
                    socket.BeginAccept(acceptCallback, this); return;
                } catch (Exception ex) {
                    Logger.LogError(ex);
                }
            }
        }
        
        static AsyncCallback acceptCallback = new AsyncCallback(AcceptCallback);
        static void AcceptCallback(IAsyncResult result) {
            if (Server.shuttingDown) return;
            TcpListen listen = (TcpListen)result.AsyncState;
            TcpSocket s = null;
            
            try {
                Socket raw = listen.socket.EndAccept(result);
                s = new TcpSocket(raw);
                
                Logger.Log(LogType.UserActivity, s.IP + " connected to the server.");
                s.Init();                
            } catch (Exception ex) {
                if (!(ex is SocketException)) Logger.LogError(ex);
                if (s != null) s.Close();
            }
            listen.AcceptNextAsync();
        }

        public override void Close() {
            if (socket != null) socket.Close();
        }
    }
}
