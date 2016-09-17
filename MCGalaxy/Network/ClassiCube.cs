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

namespace MCGalaxy {
    
    public sealed class ClassiCubeBeat : IBeat {
        
		string url = "http://www.classicube.net/heartbeat.jsp";
        public string URL { get { return url; } }

        public bool Persistance { get { return true; } }
        
        public void Init() {
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
            url = "http://"  + firstIPv4 + ":80/heartbeat.jsp";
        }

        public string PrepareBeat()  {
            string name = Server.name;
            Server.zombie.OnHeartbeat(ref name);
            Server.lava.OnHeartbeat(ref name);
            name = Colors.StripColors(name);
            
            return "&port=" + Server.port +
                "&max=" + Server.players +
                "&name=" + Uri.EscapeDataString(name) +
                "&public=" + Server.pub +
                "&version=7" +
                "&salt=" + Server.salt +
                "&users=" + PlayerCount() + "&software=MCGalaxy";
        }
        
        static int PlayerCount() {
            Player[] players = PlayerInfo.Online.Items;
            int count = 0;
            foreach (Player p in players) {
                if (!p.hidden) count++;
            }
            return count;
        }
        
        public void OnRequest(HttpWebRequest request) {
            request.Host = "www.classicube.net";
        }   
        
        bool foundUrl = false;
        public void OnResponse(string line) {
            if (String.IsNullOrEmpty(line.Trim())) return;
            string newHash = line.Substring(line.LastIndexOf('/') + 1);

            // Run this code if we don't already have a hash or if the hash has changed
            if (String.IsNullOrEmpty(Server.Hash) || !newHash.Equals(Server.Hash)) {
                Server.Hash = newHash;
                Server.URL = line;
                if (!Server.URL.Contains("\"errors\": [")) {
                    Server.s.UpdateUrl(Server.URL);
                    File.WriteAllText("text/externalurl.txt", Server.URL);
                    if (!foundUrl) {
                        Server.s.Log("ClassiCube URL found: " + Server.URL);
                        foundUrl = true;
                    }
                } else {
                    Response resp = JsonConvert.DeserializeObject<Response>(Server.URL);
                    if (resp.errors != null && resp.errors.Length > 0 && resp.errors[0].Length > 0)
                        Server.URL = resp.errors[0][0];
                    else
                        Server.URL = "Error while finding URL. Is the port open?";
                    Server.s.UpdateUrl(Server.URL);
                    Server.s.Log(Server.URL);
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
