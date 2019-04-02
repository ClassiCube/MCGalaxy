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
    public interface INetworkListen {

        /// <summary> Gets the IP address this network socket is listening on. </summary>
        IPAddress IP { get; }
        
        /// <summary> Gets the port this network socket is listening on. </summary>
        int Port { get; }

        /// <summary> Begins listening for connections on the given IP and port. </summary>
        /// <remarks> Client connections are asynchronously accepted. </remarks>
        void Listen(IPAddress ip, int port);
        
        /// <summary> Closes this network listener. </summary>
        void Close();
    }
    
    /// <summary> Abstracts listening on a TCP socket. </summary>
    public sealed class TcpListen : INetworkListen {
        Socket socket;
        IPAddress ip;
        int port;
        
        public IPAddress IP { get { return ip; } }
        public int Port     { get { return port; } }
        
        public void Listen(IPAddress ip, int port) {
            try {
                this.ip = ip; this.port = port;
                IPEndPoint ep = new IPEndPoint(ip, port);
                socket = new Socket(ep.Address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                
                socket.Bind(ep);
                socket.Listen((int)SocketOptionName.MaxConnections);
                AsyncAcceptNext();
            } catch (Exception ex) {
                Logger.LogError(ex);
                Logger.Log(LogType.Warning, "Failed to start listening on port {0} ({1})", port, ex.Message);
                socket = null;
            }
        }
        
        static AsyncCallback acceptCallback = new AsyncCallback(AcceptCallback);
        void AsyncAcceptNext() {
            socket.BeginAccept(acceptCallback, this);
        }
        
        static void AcceptCallback(IAsyncResult result) {
            if (Server.shuttingDown) return;
            TcpListen listen = (TcpListen)result.AsyncState;
            Player p = null;
            bool accepted = false;
            
            try {
                p = new Player();
                p.Connect(listen.socket.EndAccept(result));
                listen.AsyncAcceptNext();
                accepted = true;
            } catch (Exception ex) {
                if (!(ex is SocketException)) Logger.LogError(ex);
                
                if (p != null) p.Disconnect();
                if (!accepted) listen.AsyncAcceptNext();
            }
        }

        public void Close() {
            if (socket != null) socket.Close();
        }
    }
}
