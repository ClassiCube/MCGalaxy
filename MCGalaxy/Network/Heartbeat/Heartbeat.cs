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
using System.Net.Cache;
using System.Text;
using System.Threading;

namespace MCGalaxy.Network {

    /// <summary> Sends a heartbeat (optionally repeatedly every certain interval) to a web server. </summary>
    public abstract class Heartbeat {

        /// <summary> The max number of retries attempted for a heartbeat. </summary>
        public const int MAX_RETRIES = 3;
        
        /// <summary> List of all heartbeats to pump. </summary>
        public static List<Heartbeat> Heartbeats = new List<Heartbeat>() { new ClassiCubeBeat() };
        
        
        /// <summary> Gets the URL the heartbeat is sent to. </summary>
        public abstract string URL { get; }
        
        /// <summary> Gets whether this heartbeat periodically repeats beats. </summary>
        public virtual bool Persistent { get { return true; } }
        
        /// <summary> Initialises data for this heartbeat. </summary>
        public abstract void Init();
        
        /// <summary> Gets the data to be sent for a heartbeat. </summary>
        public abstract string GetHeartbeatData();
        
        /// <summary> Called when a request is about to be send to the web server. </summary>
        public abstract void OnRequest(HttpWebRequest request);
        
        /// <summary> Called when a response is received from the web server. </summary>
        public abstract void OnResponse(string response);

        
        /// <summary> Initialises all heartbeats. </summary>
        public static void InitHeartbeats() {
            if (Server.logbeat && !File.Exists("heartbeat.log")) {
                using (File.Create("heartbeat.log")) { }
            }

            canBeat = true;
            for (int i = 0; i < Beats.Length; i++) {
                Beats[i].Init();
                Pump(Beats[i]);
            }
        }

        /// <summary> Pumps the specified heartbeat. </summary>
        public static void Pump(Heartbeat beat) {
            byte[] data = Encoding.ASCII.GetBytes(beat.GetHeartbeatData());

            for (int i = 0; i < MAX_RETRIES; i++) {
                try {
                    HttpWebRequest req = WebRequest.Create(beat.URL) as HttpWebRequest;
                    req.Method = "POST";
                    req.ContentType = "application/x-www-form-urlencoded";
                    req.CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore);
                    req.Timeout = 15000;
                    beat.OnRequest(req);

                    req.ContentLength = data.Length;
                    using (Stream w = req.GetRequestStream()) {
                        w.Write(data, 0, data.Length);
                        if (Server.logbeat) Server.s.Log("Beat " + beat + " was sent");
                    }

                    using (StreamReader r = new StreamReader(req.GetResponse().GetResponseStream())) {
                        string response = r.ReadToEnd().Trim();
                        beat.OnResponse(response);

                        if (Server.logbeat) Server.s.Log("Beat: \"" + response + "\" was recieved");
                    }
                    return;
                } catch (Exception ex) {
                    // Make sure to dispose response to prevent resource leak on mono
                    if (ex is WebException) {
                        WebException webEx = (WebException)ex;
                        if (webEx.Response != null) webEx.Response.Close();                        
                    }
                    continue;
                }            
            }
            
            if (Server.logbeat) Server.s.Log("Beat: " + beat + " failed.");
        }
        
        
        static Timer timer;
        static bool canBeat = false;
        readonly static Heartbeat[] Beats = new Heartbeat[] { new ClassiCubeBeat() };

        static Heartbeat() {
            timer = new Timer(OnBeat, null, 30000, 30000);
        }

        static void OnBeat(object state) {
            if (!canBeat) return;
            for (int i = 0; i < Beats.Length; i++) {
                if (Beats[i].Persistent) Pump(Beats[i]);
            }
        }
    }
}
