/*
    Copyright 2012 MCForge
 
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
using System.Net.Sockets;
using Newtonsoft.Json;

namespace MCGalaxy.Network {
    
    /// <summary> Heartbeat to ClassiCube.net's web server. </summary>
    public sealed class ClassiCubeBeat : Heartbeat {
        
        string url = "http://www.classicube.net/heartbeat.jsp";
        #if NET_20
        string proxyUrl;
        #endif
        public override string URL { get { return url; } }
        
        public override void Init() {
            try {
                IPAddress[] addresses = Dns.GetHostAddresses("www.classicube.net");
                EnsureIPv4Url(addresses);
            } catch (Exception ex) {
                Server.s.Log("Error while trying to retrieve DNS information for classicube.net");
                Server.ErrorLog(ex);
            }
        }
        
        // classicube.net only supports ipv4 servers, so we need to make
        // sure we are using its ipv4 address when POSTing heartbeats
        void EnsureIPv4Url(IPAddress[] addresses) {
            bool useIPv6 = false;
            IPAddress firstIPv4 = null;
            
            foreach (IPAddress ip in addresses) {
                AddressFamily family = ip.AddressFamily;
                if (family == AddressFamily.InterNetworkV6)
                    useIPv6 = true;
                if (family == AddressFamily.InterNetwork && firstIPv4 == null)
                    firstIPv4 = ip;
            }
            
            if (!useIPv6 || firstIPv4 == null) return;
            
            #if !NET_20
            url = "http://"  + firstIPv4 + ":80/heartbeat.jsp";
            #else
            proxyUrl = "http://"  + firstIPv4 + ":80";
            #endif
        }

        public override string GetHeartbeatData()  {
            string name = Server.name;
            Server.zombie.OnHeartbeat(ref name);
            Server.lava.OnHeartbeat(ref name);
            name = Colors.StripColors(name);
            
            return "&port="  + Server.port +
                "&max="      + Server.players +
                "&name="     + Uri.EscapeDataString(name) +
                "&public="   + Server.pub +
                "&version=7" +
                "&salt="     + Server.salt +
                "&users="    + PlayerCount() + 
                "&software=" + Uri.EscapeDataString(Server.SoftwareNameVersioned);
        }
        
        static int PlayerCount() {
            Player[] players = PlayerInfo.Online.Items;
            int count = 0;
            foreach (Player p in players) {
                if (!p.hidden) count++;
            }
            // This may happen if a VIP or a dev/mod joins an already full server.
            if (count > Server.players)
                count = Server.players;
            return count;
        }
        
        public override void OnRequest(HttpWebRequest request) {
            #if !NET_20
            request.Host = "www.classicube.net";
            #else
            if (proxyUrl == null) return;
            request.Proxy = new WebProxy(proxyUrl);
            #endif
        }   
        
        public override void OnResponse(string response) {
            if (String.IsNullOrEmpty(response)) return;
            
            // in form of http://www.classicube.net/server/play/<hash>/
            if (response.EndsWith("/")) 
                response = response.Substring(0, response.Length - 1);
            string hash = response.Substring(response.LastIndexOf('/') + 1);

            // Run this code if we don't already have a hash or if the hash has changed
            if (String.IsNullOrEmpty(Server.Hash) || hash != Server.Hash) {
                Server.Hash = hash;
                Server.URL = response;
                
                if (!response.Contains("\"errors\": [")) {
                    Server.s.UpdateUrl(Server.URL);
                    File.WriteAllText("text/externalurl.txt", Server.URL);
                    Server.s.Log("ClassiCube URL found: " + Server.URL);
                } else {
                    Response resp = JsonConvert.DeserializeObject<Response>(Server.URL);
                    if (resp.errors != null && resp.errors.Length > 0 && resp.errors[0].Length > 0)
                        Server.URL = resp.errors[0][0];
                    else
                        Server.URL = "Error while finding URL. Is the port open?";
                    Server.s.UpdateUrl(Server.URL);
                    Server.s.Log(response);
                }
            }
        }        
        
        #pragma warning disable 0649
        class Response {
            public string[][] errors;
            public string response;
            public string status;
        }
        #pragma warning restore 0649
    }
}
