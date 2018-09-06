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
using MCGalaxy.Config;
using MCGalaxy.Network;

namespace MCGalaxy.Commands.Moderation {
    public class CmdLocation : Command2 {
        public override string name { get { return "Location"; } }
        public override string shortcut { get { return "GeoIP"; } }
        public override string type { get { return CommandTypes.Moderation; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Admin; } }
        
        class GeoInfo {
            [ConfigString] public string region;
            [ConfigString] public string country;
        }
        static ConfigElement[] elems;
        
        public override void Use(Player p, string message, CommandData data) {
            if (message.Length == 0) {
                if (p.IsSuper) { SuperRequiresArgs(p, "player name or IP"); return; }
                message = p.name;
            }
            
            string name, ip = ModActionCmd.FindIP(p, message, "Location", out name);
            if (ip == null) return;
            
            if (HttpUtil.IsPrivateIP(ip)) {
                p.Message("%WPlayer has an internal IP, cannot trace"); return;
            }

            JsonContext ctx = new JsonContext();
            using (WebClient client = HttpUtil.CreateWebClient()) {
                ctx.Val = client.DownloadString("http://ipinfo.io/" + ip + "/geo");
            }
            
            JsonObject obj = (JsonObject)Json.ParseStream(ctx);
            GeoInfo info = new GeoInfo();
            if (obj == null || !ctx.Success) {
                p.Message("%WError parsing GeoIP info"); return;
            }
            
            if (elems == null) elems = ConfigElement.GetAll(typeof(GeoInfo));
            obj.Deserialise(elems, info);
            
            string target = name == null ? ip : "of " + PlayerInfo.GetColoredName(p, name);
            p.Message("The IP {0} %Shas been traced to: &b{1}%S/&b{2}", 
                      target, info.region, info.country);
        }
        
        public override void Help(Player p) {
            p.Message("%T/Location [name/IP]");
            p.Message("%HTracks down location of the given IP, or IP player is on.");
        }
    }
}
