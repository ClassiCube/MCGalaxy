/*
    Copyright 2012 MCGalaxy
    
    Dual-licensed under the	Educational Community License, Version 2.0 and
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

        /// <summary>
        /// The max number of retries it runs for a beat
        /// </summary>
        public const int MAX_RETRIES = 3;

        /// <summary>
        /// Gets or sets a value indicating whether this instance can beat.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance can beat; otherwise, <c>false</c>.
        /// </value>
        public static bool CanBeat { get; set; }

        static Timer Timer;
        static object Lock = new object();


        private readonly static IBeat[] Beats = {

            //Keep in this order.
            new ClassiCubeBeat()
        };




        static Heart() {
            Thread t = new Thread(new ThreadStart(() => {
                Timer = new Timer(OnBeat, null,
#if DEBUG
                6000, 6000
#else
                45000, 45000
#endif
                );
            }));
        	t.Name = "MCG_Heartbeat";
        	t.Start();
        }

        private static void OnBeat(object state) {
            for ( int i = 0; i < Beats.Length; i++ ) {
                if ( Beats[i].Persistance )
                    Pump(Beats[i]);
            }
        }



        /// <summary>
        /// Inits this instance.
        /// </summary>
        public static void Init() {
            if ( Server.logbeat ) {
                if ( !File.Exists("heartbeat.log") ) {
                    using ( File.Create("heartbeat.log") ) { }
                }
            }
            
            CanBeat = true;
            
            for ( int i = 0; i < Beats.Length; i++ )
                Pump(Beats[i]);
        }

        /// <summary>
        /// Pumps the specified beat.
        /// </summary>
        /// <param name="beat">The beat.</param>
        /// <returns></returns>
        public static void Pump(IBeat beat) {
            
            if(!CanBeat)
                return;
            return;

            byte[] data = Encoding.ASCII.GetBytes(beat.Prepare());

            for ( int i = 0; i < MAX_RETRIES; i++ ) {
                try {
                    var request = WebRequest.Create(beat.URL) as HttpWebRequest;
                    request.Method = "POST";
                    request.ContentType = "application/x-www-form-urlencoded";
                    request.CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore);
                    request.Timeout = 15000;
                    request.ContentLength = data.Length;

                    using ( var writer = request.GetRequestStream() ) {
                        writer.Write(data, 0, data.Length);

                        if ( Server.logbeat )
                            Server.s.Log("Beat " + beat.ToString() + " was sent");
                    }

                    using ( var reader = new StreamReader(request.GetResponse().GetResponseStream()) ) {
                        string read = reader.ReadToEnd().Trim();
                        beat.OnResponse(read);

                        if ( Server.logbeat )
                            Server.s.Log("Beat: \"" + read + "\" was recieved");
                    }
                    return;
                }
                catch {
                    continue;
                }
            }

            if ( Server.logbeat )
                Server.s.Log("Beat: " + beat.ToString() + " failed.");
        }

        /// <summary>
        /// Encodes the URL.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>An encoded url</returns>
        public static string EncodeUrl(string input) {
            StringBuilder output = new StringBuilder();
            for ( int i = 0; i < input.Length; i++ ) {
                if ( ( input[i] >= '0' && input[i] <= '9' ) ||
                    ( input[i] >= 'a' && input[i] <= 'z' ) ||
                    ( input[i] >= 'A' && input[i] <= 'Z' ) ||
                    input[i] == '-' || input[i] == '_' || input[i] == '.' || input[i] == '~' ) {
                    output.Append(input[i]);
                }
                else if ( Array.IndexOf<char>(ReservedChars, input[i]) != -1 ) {
                    output.Append('%').Append(( (int)input[i] ).ToString("X"));
                }
            }
            return output.ToString();
        }

        public static readonly char[] ReservedChars = { ' ', '!', '*', '\'', '(', ')', ';', ':', '@', '&', '=', '+', '$', ',', '/', '?', '%', '#', '[', ']' };
    }

}
