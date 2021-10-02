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
using System.Net;
using System.Net.Cache;
using System.Text;
using MCGalaxy.Tasks;

namespace MCGalaxy.Network 
{
    /// <summary> Repeatedly sends a heartbeat request every certain interval to a web server. </summary>
    public abstract class Heartbeat 
    {
        /// <summary> The max number of retries attempted for a requests. </summary>
        public const int MAX_RETRIES = 3;
        
        /// <summary> List of all heartbeats to pump. </summary>
        public static List<Heartbeat> Heartbeats = new List<Heartbeat>() { new ClassiCubeBeat() };
        
        
        /// <summary> Gets the URL the heartbeat is sent to </summary
        public abstract string URL { get; }
        
        /// <summary> Salt used for verifying player names </summary>
        public string Salt = "";
        
        /// <summary> Initialises data for this heartbeat </summary>
        protected abstract void Init();
        
        /// <summary> Gets the data to be sent for the next heartbeat </summary>
        protected abstract string GetHeartbeatData();
        
        /// <summary> Called when a heartbeat is about to be sent to the web server </summary>
        protected abstract void OnRequest(HttpWebRequest request);
        
        /// <summary> Called when a response is received from the web server </summary>
        protected abstract void OnResponse(string response);
        

        /// <summary> Pumps this heartbeat </summary>
        /// <remarks> i.e. Sends a heartbeat and then reads the response </remarks>
        public void Pump() {
            byte[] data = Encoding.ASCII.GetBytes(GetHeartbeatData());
            Exception lastEx = null;

            for (int i = 0; i < MAX_RETRIES; i++) {
                try {
                    HttpWebRequest req = HttpUtil.CreateRequest(URL);
                    req.Method      = "POST";
                    req.ContentType = "application/x-www-form-urlencoded";
                    req.CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore);
                    req.Timeout     = 10000;
                    
                    OnRequest(req);
                    HttpUtil.SetRequestData(req, data);
                    WebResponse res = req.GetResponse();
                    
                    string response = HttpUtil.GetResponseText(res);
                    OnResponse(response);
                    return;
                } catch (Exception ex) {
                    HttpUtil.DisposeErrorResponse(ex);
                    lastEx = ex;
                    continue;
                }
            }
            
            string hostUrl = new Uri(URL).Host;
            Logger.Log(LogType.Warning, "Failed to send heartbeat to {0} ({1})", hostUrl, lastEx.Message);
        }
        
        
        /// <summary> Initialises all heartbeats </summary>
        public static void InitHeartbeats() {
            foreach (Heartbeat beat in Heartbeats) {
                beat.Init();
                beat.Salt = Server.GenerateSalt();
                beat.Pump();
            }
            
            if (heartbeatTask != null) return;
            heartbeatTask = Server.Background.QueueRepeat(OnBeat, null, 
                                                          TimeSpan.FromSeconds(30));
        }
        
        static SchedulerTask heartbeatTask;
        static void OnBeat(SchedulerTask task) {
            foreach (Heartbeat beat in Heartbeats) { beat.Pump(); }
        }
    }
}
