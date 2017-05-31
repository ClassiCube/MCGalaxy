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
    
    /// <summary> Abstracts listening on a TCP socket. </summary>
    public sealed class TcpListen : INetworkListen {
        Socket socket;
        IPAddress ip;
        ushort port;
        
        public IPAddress LocalIP { get { return ip; } }
        public ushort LocalPort { get { return port; } }
        
        public void Listen(IPAddress ip, ushort port) {
            try {
        	    this.ip = ip; this.port = port;
                IPEndPoint ep = new IPEndPoint(ip, port);
                socket = new Socket(ep.Address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                
                socket.Bind(ep);
                socket.Listen((int)SocketOptionName.MaxConnections);
                AcceptNextAsync();
            } catch (Exception ex) {
                Server.ErrorLog(ex);
                Server.s.Log("Failed to start listening on port " + port + " (" + ex.Message + ")");
                socket = null;
            }
        }
        
        static AsyncCallback acceptCallback = new AsyncCallback(AcceptCallback);
        public void AcceptNextAsync() {
            socket.BeginAccept(acceptCallback, this);
        }
        
        static void AcceptCallback(IAsyncResult result) {
            if (Server.shuttingDown) return;
            TcpListen listen = (TcpListen)result.AsyncState;
            Player p = null;
            bool accepted = false;
            
            try {
                p = new Player(listen.socket.EndAccept(result));
                listen.AcceptNextAsync();
                accepted = true;
            } catch (Exception ex) {
                if (!(ex is SocketException)) Server.ErrorLog(ex);
                
                if (p != null) p.Disconnect();
                if (!accepted) listen.AcceptNextAsync();
            }
        }

        public void Close() {
            if (socket != null) socket.Close();
        }
    }
}
