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
        
        void DisableIPV6OnlyListener() {
            if (socket.AddressFamily != AddressFamily.InterNetworkV6) return;
            // TODO: Make windows only?

            // NOTE: SocketOptionName.IPv6Only is not defined in Mono, but apparently
            //  macOS and Linux default to dual stack by default already
            const SocketOptionName ipv6Only = (SocketOptionName)27;
            try {
                socket.SetSocketOption(SocketOptionLevel.IPv6, ipv6Only, false);
            } catch (Exception ex) {
                Logger.LogError("Failed to disable IPv6 only listener setting", ex);
            }
        }
        
        void EnableAddressReuse() {
        	// https://stackoverflow.com/questions/3229860/what-is-the-meaning-of-so-reuseaddr-setsockopt-option-linux
        	// http://www.softlab.ntua.gr/facilities/documentation/unix/unix-socket-faq/unix-socket-faq-4.html#ss4.2
        	// https://docs.microsoft.com/en-us/windows/win32/winsock/graceful-shutdown-linger-options-and-socket-closure-2
        	// https://stackoverflow.com/questions/45535077/address-in-use-on-tcp-socket-bind
        	// https://stackoverflow.com/questions/3757289/when-is-tcp-option-so-linger-0-required
        	// https://superuser.com/questions/173535/what-are-close-wait-and-time-wait-states
        	// http://www.serverframework.com/asynchronousevents/2011/01/time-wait-and-its-design-implications-for-protocols-and-scalable-servers.html
        	// https://superuser.com/questions/173535/what-are-close-wait-and-time-wait-states
        	// https://stackoverflow.com/questions/35322550/is-there-a-way-to-enable-the-so-reuseaddr-socket-option-when-using-system-net-ht
        	try {
        		socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, 1);
            } catch {
                // not really a critical issue if this fails to work
                // TODO only do this on Mono
            }
        }
        
        public override void Listen(IPAddress ip, int port) {
            if (IP == ip && Port == port) return;
            Close();
            IP = ip; Port = port;
            
            try {
                socket = new Socket(ip.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                DisableIPV6OnlyListener();
                EnableAddressReuse();
                
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
