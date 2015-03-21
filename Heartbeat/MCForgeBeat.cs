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
using System.Collections.Generic;
using System.Linq;
namespace MCGalaxy
{
    internal sealed class MCGalaxyBeat : IBeat
    {
        public string URL { get { return ServerSettings.HeartbeatAnnounce; } }

        public bool Persistance
        {
            get { return true; }
        }

        public string Prepare()
        {

            string Parameters = "name=" + Heart.EncodeUrl(Server.name) +
                                                     "&users=" + Player.players.Count +
                                                     "&max=" + Server.players +
                                                     "&port=" + Server.port +
                                                     "&version=" + Server.Version +
                                                     "&gcname=" + Heart.EncodeUrl(Server.UseGlobalChat ? Server.GlobalChatNick : "[Disabled]") +
                                                     "&public=" + (Server.pub ? "1" : "0") +
                                                     "&motd=" + Heart.EncodeUrl(Server.motd);

            if (Server.levels != null && Server.levels.Count > 0)
            {
                IEnumerable<string> worlds = from l in Server.levels select l.name;
                Parameters += "&worlds=" + String.Join(", ", worlds.ToArray());
            }
            Parameters += "&hash=" + Server.Hash;

            return Parameters;
        }

        public void OnResponse(string line)
        {
            //Do nothing
        }

    }
}
