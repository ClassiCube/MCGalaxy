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
using System.Data;
using System.IO;
using System.Net;
using MCGalaxy.SQL;

namespace MCGalaxy.Commands {
    
    public class CmdLocation : Command {
        
        public override string name { get { return "location"; } }
        public override string shortcut { get { return "lo"; } }
        public override string type { get { return CommandTypes.Moderation; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Admin; } }
        public CmdLocation() { }
        
        public override void Use(Player p, string message) {
            string ip = "";
            Player who = PlayerInfo.Find(message);
            if (who == null) {
                Player.Message(p, "&eNo online player \"" + message + "\", searching database..");
                OfflinePlayer target = PlayerInfo.FindOfflineMatches(p, message);              
                if (target == null) return;
                ip = target.ip;
            } else {
                ip = who.ip;
            }
            
            if (Player.IPInPrivateRange(ip)) {
                Player.Message(p, Colors.red + "Player has an internal IP, cannot trace"); return;
            }        
            string name = who != null ? who.name : message;
            Player.Message(p, "&aThe IP of &b" + name + " &ahas been traced to: &b" + GetIPLocation(ip));
        }
        
        static string GetIPLocation(string IP) {
            string city, country;
            WebRequest reqCity = WebRequest.Create("http://ipinfo.io/" + IP + "/city");
            WebRequest reqCountry = WebRequest.Create("http://ipinfo.io/" + IP + "/country");
            using (WebResponse resp = reqCity.GetResponse())
                using (StreamReader reader = new StreamReader(resp.GetResponseStream()))
            {
                city = reader.ReadToEnd().Replace("\n", "");
                if (city == "")
                    city = "Unknown";
            }
            
            using (WebResponse resp = reqCountry.GetResponse())
                using (StreamReader reader = new StreamReader(resp.GetResponseStream()))
            {
                country = reader.ReadToEnd().Replace("\n", "");
            }
            return city + "/" + country;
        }
        
        public override void Help(Player p) {
            Player.Message(p, "/location <name> - Tracks down the location of the IP associated with <name>.");
        }
    }
}
