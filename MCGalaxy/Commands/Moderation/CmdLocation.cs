/*
    Copyright 2015 MCGalaxy team

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
using System.Net;

namespace MCGalaxy.Commands.Moderation {
    public class CmdLocation : Command {        
        public override string name { get { return "location"; } }
        public override string shortcut { get { return "lo"; } }
        public override string type { get { return CommandTypes.Moderation; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Admin; } }
        
        public override void Use(Player p, string message) {
            string ip = "";
            Player match = PlayerInfo.FindMatches(p, message);
            string target = message;
            
            if (match == null) {
                Player.Message(p, "Searching PlayerDB for \"{0}\"..", message);
                target = PlayerInfo.FindOfflineIPMatches(p, message, out ip);
                if (target == null) return;
            } else {
                target = match.name; ip = match.ip;
            }
            
            if (Player.IPInPrivateRange(ip)) {
                Player.Message(p, Colors.red + "Player has an internal IP, cannot trace"); return;
            }

            string country = new WebClient().DownloadString("http://ipinfo.io/" + ip + "/country");
            country = country.Replace("\n", "");
            Player.Message(p, "The IP of &a" + target + " %Shas been traced to: &b" + country);
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/location [name]");
            Player.Message(p, "%HTracks down the country of the IP associated with [name].");
        }
    }
}
