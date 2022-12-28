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

namespace MCGalaxy.Network
{
    /// <summary> Heartbeat to ClassiCube.net's web server. </summary>
    public sealed class ClassiCubeBeat : Heartbeat
    {
        string proxyUrl;
        public string LastResponse;
        bool checkedAddr;
        
        void CheckAddress() {
            string hostUrl = "";
            checkedAddr    = true;
            
            try {
                hostUrl = GetHost();
                IPAddress[] addresses = Dns.GetHostAddresses(hostUrl);
                EnsureIPv4Url(addresses);
            } catch (Exception ex) {
                Logger.LogError("Error retrieving DNS information for " + hostUrl, ex);
            }
            
            // Replace www, as otherwise the 'Finding www.classicube.net url..'
            //  message appears as a clickable link in the Logs textbox in GUI
            hostUrl = hostUrl.Replace("www.", "");
            Logger.Log(LogType.SystemActivity, "Finding " + hostUrl + " url..");
        }
        
        // classicube.net only supports ipv4 servers, so we need to make
        // sure we are using its ipv4 address when POSTing heartbeats
        void EnsureIPv4Url(IPAddress[] addresses) {
            bool hasIPv6 = false;
            IPAddress firstIPv4 = null;
            
            // proxying doesn't work properly with https:// URLs
            if (URL.CaselessStarts("https://")) return;
            
            foreach (IPAddress ip in addresses) {
                AddressFamily family = ip.AddressFamily;
                if (family == AddressFamily.InterNetworkV6)
                    hasIPv6 = true;
                if (family == AddressFamily.InterNetwork && firstIPv4 == null)
                    firstIPv4 = ip;
            }
            
            if (!hasIPv6 || firstIPv4 == null) return;
            proxyUrl = "http://"  + firstIPv4 + ":80";
        }

        protected override string GetHeartbeatData()  {
            string name = Server.Config.Name;
            OnSendingHeartbeatEvent.Call(this, ref name);
            name = Colors.StripUsed(name);
            
            return
                "&port="     + Server.Config.Port +
                "&max="      + Server.Config.MaxPlayers +
                "&name="     + Uri.EscapeDataString(name) +
                "&public="   + Server.Config.Public +
                "&version=7" +
                "&salt="     + Salt +
                "&users="    + PlayerInfo.NonHiddenUniqueIPCount() +
                "&software=" + Uri.EscapeDataString(Server.SoftwareNameVersioned) +
                "&web="      + Server.Config.WebClient;
        }
        
        protected override void OnRequest(HttpWebRequest request) {
            if (!checkedAddr) CheckAddress();
            
            if (proxyUrl == null) return;
            request.Proxy = new WebProxy(proxyUrl);
        }
        
        protected override void OnResponse(WebResponse response) {
            string text = HttpUtil.GetResponseText(response);
            if (!NeedsProcessing(text)) return;
            
            if (!text.Contains("\"errors\":")) {
                OnSuccess(text);
            } else {
                string error = GetError(text);
                if (error == null) error = "Error while finding URL. Is the port open?";
                OnError(error);
            }
        }
        
        protected override void OnFailure(string response) {
            if (NeedsProcessing(response)) OnError(response);
        }
        
        
        bool NeedsProcessing(string text) {
            if (String.IsNullOrEmpty(text)) return false;
            if (text == LastResponse)       return false;
            
            // only need to process responses that have changed
            LastResponse = text;
            return true;
        }
        
        static void OnSuccess(string text) {
            text = Truncate(text);
            Server.UpdateUrl(text);
            File.WriteAllText("text/externalurl.txt", text);
            Logger.Log(LogType.SystemActivity, "Server URL found: " + text);
        }
        
        static void OnError(string error) {
            error = Truncate(error);
            Server.UpdateUrl(error);
            Logger.Log(LogType.Warning, error);
        }
        
        
        static string GetError(string json) {
            JsonReader reader = new JsonReader(json);
            // silly design, but form of json is:
            // {
            //   "errors": [ ["Error 1"], ["Error 2"] ],
            //   "response": "",
            //   "status": "fail"
            // }
            JsonObject obj = reader.Parse() as JsonObject;
            if (obj == null || !obj.ContainsKey("errors")) return null;
            
            JsonArray errors = obj["errors"] as JsonArray;
            if (errors == null) return null;

            foreach (object raw in errors) {
                JsonArray err = raw as JsonArray;
                if (err != null && err.Count > 0) return (string)err[0];
            }
            return null;
        }
        
        static string Truncate(string text) {
            if (text.Length < 256) return text;
            
            return text.Substring(0, 256) + "..";
        }
    }
}
