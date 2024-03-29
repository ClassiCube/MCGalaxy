/*
    Copyright 2012 MCForge
    
    Dual-licensed under the Educational Community License, Version 2.0 and
    the GNU General Public License, Version 3 (the "Licenses"); you may
    not use this file except in compliance with the Licenses. You may
    obtain a copy of the Licenses at
    
    https://opensource.org/license/ecl-2-0/
    https://www.gnu.org/licenses/gpl-3.0.html
    
    Unless required by applicable law or agreed to in writing,
    software distributed under the Licenses are distributed on an "AS IS"
    BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express
    or implied. See the Licenses for the specific language governing
    permissions and limitations under the Licenses.
 */
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Cache;
using System.Net.Sockets;
using System.Text;
using MCGalaxy.Authentication;
using MCGalaxy.Tasks;

namespace MCGalaxy.Network 
{
    /// <summary> Repeatedly sends a heartbeat request every certain interval to a web server. </summary>
    public abstract class Heartbeat 
    {
        /// <summary> The max number of retries attempted for a request </summary>
        public const int MAX_RETRIES = 3;
        
        /// <summary> List of all heartbeats to pump </summary>
        public static List<Heartbeat> Heartbeats = new List<Heartbeat>();
        
        
        /// <summary> The URL this heartbeat is sent to </summary
        public string URL;
        /// <summary> Salt used for verifying player names </summary>
        public string Salt = "";

        public string GetHost() {
            try {
                return new Uri(URL).Host;
            } catch (Exception ex) {
                Logger.LogError("Getting host of " + URL, ex);
                return URL;
            }
        }
        
        /// <summary> Gets the data to be sent for the next heartbeat </summary>
        protected abstract string GetHeartbeatData();
        /// <summary> Called when a heartbeat is about to be sent to the web server </summary>
        protected abstract void OnRequest(HttpWebRequest request);
        /// <summary> Called when a response is received from the web server </summary>
        protected abstract void OnResponse(WebResponse response);
        /// <summary> Called when a failure HTTP response is received from the web server </summary>
        protected abstract void OnFailure(string response);
        

        /// <summary> Sends a heartbeat to the web server and then reads the response </summary>
        public void Pump() {
            byte[] data = Encoding.ASCII.GetBytes(GetHeartbeatData());
            Exception lastEx = null;
            string lastResp  = null;

            for (int i = 0; i < MAX_RETRIES; i++) 
            {
                try {
                    HttpWebRequest req = HttpUtil.CreateRequest(URL);
                    req.Method      = "POST";
                    req.ContentType = "application/x-www-form-urlencoded";
                    req.CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore);
                    req.Timeout     = 10000;
                    
                    OnRequest(req);
                    HttpUtil.SetRequestData(req, data);
                    WebResponse res = req.GetResponse();
                    OnResponse(res);
                    return;
                } catch (Exception ex) {
                    lastResp = HttpUtil.GetErrorResponse(ex);
                    HttpUtil.DisposeErrorResponse(ex);
                    lastEx   = ex;
                    continue;
                }
            }
            
            OnFailure(lastResp);
            Logger.Log(LogType.Warning, "Failed to send heartbeat to {0} ({1})", GetHost(), lastEx.Message);
        }
        
        
        /// <summary> Adds the given heartbeat to the list of automatically pumped heartbeats </summary>
        public static void Register(Heartbeat beat) {
            Heartbeats.Add(beat);
        }
        
        /// <summary> Starts pumping heartbeats </summary>
        public static void Start() {
            string hosts = Heartbeats.Join(hb => hb.GetHost().Replace("www.", ""));
            Server.UpdateUrl("Finding " + hosts + " url..");

            OnBeat(null); // immedately call so URL is shown as soon as possible in console
            Server.Heartbeats.QueueRepeat(OnBeat, null, TimeSpan.FromSeconds(30));
        }
        
        static void OnBeat(SchedulerTask task) {
            // no point if can't accept connections anyways
            if (!Server.Listener.Listening) return;
            
            foreach (Heartbeat beat in Heartbeats) { beat.Pump(); }
        }
        
        
        static string lastUrls;
        internal static void ReloadDefault() {
            string urls = Server.Config.HeartbeatURL;
            // don't reload heartbeats unless absolutely have to
            if (urls == lastUrls) return;
            
            lastUrls = urls;
            // TODO only reload default heartbeats, don't clear all
            Heartbeats.Clear();
            
            foreach (string url in urls.SplitComma())
            {
                AuthService service = AuthService.GetOrCreate(url);
                
                Heartbeat beat = new ClassiCubeBeat() { URL = url };
                beat.Salt = service.Salt; // TODO: Just reference Service instead of copying salt?
                Register(beat);
            }
        }
        

        // e.g. classicube.net only supports ipv4 servers, so we need to make
        // sure we are using its ipv4 address when POSTing heartbeats there
        protected string EnsureIPv4Url(string hostUrl) {
            bool hasIPv6 = false;
            IPAddress firstIPv4 = null;
            
            // proxying doesn't work properly with https:// URLs
            if (URL.CaselessStarts("https://")) return null;
            IPAddress[] addresses = Dns.GetHostAddresses(hostUrl);
            
            foreach (IPAddress ip in addresses) {
                AddressFamily family = ip.AddressFamily;
                if (family == AddressFamily.InterNetworkV6)
                    hasIPv6 = true;
                if (family == AddressFamily.InterNetwork && firstIPv4 == null)
                    firstIPv4 = ip;
            }
            
            if (!hasIPv6 || firstIPv4 == null) return null;
            return "http://"  + firstIPv4 + ":80";
        }
    }
}
