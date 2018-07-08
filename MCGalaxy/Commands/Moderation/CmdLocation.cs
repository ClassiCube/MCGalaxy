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
using System.Net;
using MCGalaxy.Network;

namespace MCGalaxy.Commands.Moderation {
    public class CmdLocation : Command {
        public override string name { get { return "Location"; } }
        public override string shortcut { get { return "lo"; } }
        public override string type { get { return CommandTypes.Moderation; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Admin; } }
        
        public override void Use(Player p, string message) {
            if (message.Length == 0) {
                if (Player.IsSuper(p)) { SuperRequiresArgs(p, "player name"); return; }
                message = p.name;
            }
            
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
            
            if (HttpUtil.IsPrivateIP(ip)) {
                Player.Message(p, "%WPlayer has an internal IP, cannot trace"); return;
            }

            string country = null;
            using (WebClient client = HttpUtil.CreateWebClient()) {
                country = client.DownloadString("http://ipinfo.io/" + ip + "/country");
                country = country.Replace("\n", "");
            }
            Player.Message(p, "The IP of {0} %Shas been traced to: &b" + country, PlayerInfo.GetColoredName(p, target));
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/Location [name]");
            Player.Message(p, "%HTracks down the country of the IP associated with [name].");
        }
    }
}
