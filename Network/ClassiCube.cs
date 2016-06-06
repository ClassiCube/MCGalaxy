/*
    Copyright 2012 MCGalaxy
 
    Dual-licensed under the    Educational Community License, Version 2.0 and
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

namespace MCGalaxy {
    
    public sealed class ClassiCubeBeat : IBeat {
        
        public string URL { get { return "http://www.classicube.net/heartbeat.jsp"; } }

        public bool Persistance { get { return true; } }

        public string Prepare()  {
            string name = Server.name;
            Server.zombie.OnHeartbeat(ref name);
            Server.lava.OnHeartbeat(ref name);
            name = Colors.StripColours(name);
            
            return "&port=" + Server.port +
                "&max=" + Server.players +
                "&name=" + Heart.EncodeUrl(name) +
                "&public=" + Server.pub +
                "&version=7" +
                "&salt=" + Server.salt +
                "&users=" + PlayerInfo.Online.Count + "&software=MCGalaxy";
        }
        
        bool UrlSaid = false;
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
                    if (!UrlSaid) {
                        Server.s.Log("ClassiCube URL found: " + Server.URL);
                        UrlSaid = true;
                    }
                } else {
                    Server.URL = "Error while finding URL. Is the port open?";
                    Server.s.UpdateUrl(Server.URL);
                    Server.s.Log(Server.URL);
                }
            }
        }
    }
}
