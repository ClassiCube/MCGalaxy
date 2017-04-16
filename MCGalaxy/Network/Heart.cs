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
using System.Net.Cache;
using System.Text;
using System.Threading;

namespace MCGalaxy {

    public static class Heart {

        /// <summary> The max number of retries it runs for a beat </summary>
        public const int MAX_RETRIES = 3;

        /// <summary> Gets or sets a value indicating whether this instance can beat. </summary>
        /// <value> <c>true</c> if this instance can beat; otherwise, <c>false</c>. </value>
        public static bool CanBeat { get; set; }

        static Timer timer;

        readonly static IBeat[] Beats = new IBeat[] {         
            new ClassiCubeBeat()
        }; //Keep these in order

        static Heart() {
            timer = new Timer(OnBeat, null, 30000, 30000);
        }

        static void OnBeat(object state) {
            for (int i = 0; i < Beats.Length; i++) {
                if (Beats[i].Persistance) Pump(Beats[i]);
            }
        }

        /// <summary> Inits this instance. </summary>
        public static void Init() {
            if (Server.logbeat && !File.Exists("heartbeat.log")) {
                using (File.Create("heartbeat.log")) { }
            }
            
            CanBeat = true;            
            for (int i = 0; i < Beats.Length; i++) {
                Beats[i].Init();
                Pump(Beats[i]);
            }
        }

        /// <summary> Pumps the specified beat. </summary>
        public static void Pump(IBeat beat) {
            if (!CanBeat) return;
            byte[] data = Encoding.ASCII.GetBytes(beat.PrepareBeat());

            for (int i = 0; i < MAX_RETRIES; i++) {
                try {
                    HttpWebRequest req = WebRequest.Create(beat.URL) as HttpWebRequest;
                    req.Method = "POST";
                    req.ContentType = "application/x-www-form-urlencoded";
                    req.CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore);
                    req.Timeout = 15000;
                    req.ContentLength = data.Length;
                    beat.OnRequest(req);

                    using (Stream w = req.GetRequestStream()) {
                        w.Write(data, 0, data.Length);
                        if (Server.logbeat) Server.s.Log("Beat " + beat + " was sent");
                    }

                    using (StreamReader r = new StreamReader(req.GetResponse().GetResponseStream())) {
                        string read = r.ReadToEnd().Trim();
                        beat.OnResponse(read);

                        if (Server.logbeat) Server.s.Log("Beat: \"" + read + "\" was recieved");
                    }
                    return;
                } catch {
                    continue;
                }
            }

            if (Server.logbeat) Server.s.Log("Beat: " + beat + " failed.");
        }
    }
}
