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
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using MCGalaxy.Config;
using MCGalaxy.Events.ServerEvents;

namespace MCGalaxy.Network {
    
    /// <summary> Heartbeat to ClassiCube.net's web server. </summary>
    public sealed class ClassiCubeBeat : Heartbeat {
        string proxyUrl;
        public override string URL { get { return Server.Config.HeartbeatURL; } }
        
        public override void Init() {
            try {
                string hostUrl = new Uri(URL).Host;
                IPAddress[] addresses = Dns.GetHostAddresses(hostUrl);
                EnsureIPv4Url(addresses);
            } catch (Exception ex) {
                Logger.LogError("Error retrieving DNS information for classicube.net", ex);
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
            proxyUrl = "http://"  + firstIPv4 + ":80";
        }

        public override string GetHeartbeatData()  {
            string name = Server.Config.Name;
            OnSendingHeartbeatEvent.Call(this, ref name);
            name = Colors.StripUsed(name);
            
            return
                "&port="     + Server.Config.Port +
                "&max="      + Server.Config.MaxPlayers +
                "&name="     + Uri.EscapeDataString(name) +
                "&public="   + Server.Config.Public +
                "&version=7" +
                "&salt="     + Server.salt +
                "&users="    + PlayerInfo.NonHiddenUniqueIPCount() +
                "&software=" + Uri.EscapeDataString(Server.SoftwareNameVersioned) +
                "&web="      + Server.Config.WebClient;
        }
        
        public override void OnRequest(HttpWebRequest request) {
            if (proxyUrl == null) return;
            request.Proxy = new WebProxy(proxyUrl);
        }
        
        public override void OnResponse(string response) {
            if (String.IsNullOrEmpty(response)) return;
            
            // in form of http://www.classicube.net/server/play/<hash>/
            if (response.EndsWith("/"))
                response = response.Substring(0, response.Length - 1);
            string hash = response.Substring(response.LastIndexOf('/') + 1);

            // only need to do this when contents have changed
            if (hash == Server.Hash) return;
            Server.Hash = hash;
            Server.URL = response;
            
            if (!response.Contains("\"errors\":")) {
                Server.UpdateUrl(Server.URL);
                File.WriteAllText("text/externalurl.txt", Server.URL);
                Logger.Log(LogType.SystemActivity, "ClassiCube URL found: " + Server.URL);
            } else {
                string error = GetError(response);
                if (error == null) error = "Error while finding URL. Is the port open?";
                
                Server.URL = error;
                Server.UpdateUrl(Server.URL);
                Logger.Log(LogType.Warning, response);
            }
        }
        
        static string GetError(string json) {
            JsonReader reader = new JsonReader(json);
            string error = null;
            
            // silly design, but form of json is: "errors": [ ["Error1"], ["Error2"] ]
            reader.OnMember = (obj, key, value) => {
                if (key != "errors") return;                
                JsonArray errors = value as JsonArray;
                if (errors == null) return;
                
                foreach (object raw in errors) {
                    JsonArray err = raw as JsonArray;
                    if (err != null && err.Count > 0) error = (string)err[0];
                }                
            };
            
            reader.Parse();
            return error;
        }
    }
}
