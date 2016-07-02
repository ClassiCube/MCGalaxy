/*
    Copyright 2012 MCForge
 
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
using Newtonsoft.Json;

namespace MCGalaxy {
    
    public sealed class ClassiCubeBeat : IBeat {
        
        public string URL { get { return "http://www.classicube.net/heartbeat.jsp"; } }

        public bool Persistance { get { return true; } }

        public string Prepare()  {
            string name = Server.name;
            Server.zombie.OnHeartbeat(ref name);
            Server.lava.OnHeartbeat(ref name);
            name = Colors.StripColors(name);
            
            return "&port=" + Server.port +
                "&max=" + Server.players +
                "&name=" + Heart.EncodeUrl(name) +
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
        
        class Response {
        	public string[][] errors;
        	public string response;
        	public string status;
        }
    }
}
