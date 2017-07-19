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
using System.Net;

namespace MCGalaxy.Network {
    
    /// <summary> Abstracts sending to/receiving from a network socket. </summary>
    public interface INetworkSocket {
        
        /// <summary> Gets the remote IP of this socket. </summary>
        string RemoteIP { get; }
        
        /// <summary> Sets whether this socket operates in low-latency mode (e.g. for TCP, disabes nagle's algorithm). </summary>
        bool LowLatency { set; }
        
        /// <summary> Receives next block of received data, asynchronously. </summary>
        void ReceiveNextAsync();
        
        /// <summary> Sends a block of data, either synchronously or asynchronously. </summary>
        void Send(byte[] buffer, bool sync = false);
        
        /// <summary> Closes this network socket. </summary>
        void Close();
    }
            
    /// <summary> Abstracts listening on network socket. </summary>
    public interface INetworkListen {

        /// <summary> Gets the IP address this network socket is listening on. </summary>
        IPAddress LocalIP { get; }
        
        /// <summary> Gets the port this network socket is listening on. </summary>
        ushort LocalPort { get; }

        /// <summary> Begins listening on the given local IP and port. </summary>
        void Listen(IPAddress ip, ushort port);
        
        /// <summary> Accepts the next client connection, asynchronously. </summary>
        void AcceptNextAsync();
        
        /// <summary> Closes this network socket. </summary>
        void Close();
    }
}
