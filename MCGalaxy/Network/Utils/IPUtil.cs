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
using System.IO;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;

namespace MCGalaxy.Network {
    /// <summary> Utility methods related to IP addresses </summary>
    public static class IPUtil {
        
        /// <summary> Returns whether the given IP is a loopback or a LAN address </summary>
        public static bool IsPrivate(IPAddress ip) {
            if (IPAddress.IsLoopback(ip)) return true;
            
            if (ip.AddressFamily == AddressFamily.InterNetwork) {
                byte[] addr = ip.GetAddressBytes();
                // range of 172.16.0.0 - 172.31.255.255
                if (addr[0] == 172 && (addr[1] >= 16 && addr[1] <= 31)) return true;
                
                // range of 192.168.0.0 to 192.168.255.255
                if (addr[0] == 192 && addr[1] == 168) return true;
                // range of 10.0.0.0 to 10.255.255.255
                if (addr[0] == 10) return true;
            }
            
            // TODO: Are there more IPv6 address ranges than just this?
            return ip.IsIPv6LinkLocal;
        }

        /// <summary> Returns whether the given IP is a loopback address
        /// or an address mapped to this computer's host </summary>
        public static bool IsLocal(IPAddress ip) {
            try {
                if (IPAddress.IsLoopback(ip)) return true;
                
                IPAddress[] localIPs = Dns.GetHostAddresses(Dns.GetHostName());
                foreach (IPAddress localIP in localIPs) {
                    if (ip.Equals(localIP)) return true;
                }
            }
            catch { }
            return false;
        }
        
        public static bool IsIPv4Mapped(IPAddress ip) {
            if (ip.AddressFamily != AddressFamily.InterNetworkV6) return false;
            byte[] addr = ip.GetAddressBytes();
            
            // IPv4 mapped addresses have the format
            //  0000:0000:0000:0000:0000:FFFF:[ipv4 address]
            for (int i = 0; i < 10; i++) {
                if (addr[i] != 0) return false;
            }
            return addr[10] == 0xFF && addr[11] == 0xFF;
        }
        
        public static IPAddress MapToIPV4(IPAddress ip) {
            byte[] addr = ip.GetAddressBytes();
            
            // lower 32 bits of IPv6 address are the IPV4 address
            byte[] ipv4 = new byte[4];
            Buffer.BlockCopy(addr, 12, ipv4, 0, 4);
            return new IPAddress(ipv4);
        }
    }
}