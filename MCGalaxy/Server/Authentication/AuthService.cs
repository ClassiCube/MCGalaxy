/*
    Copyright 2015 MCGalaxy
    
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
using MCGalaxy.Network;

namespace MCGalaxy.Authentication
{
    public class AuthService
    {
        /// <summary> List of all authentication services </summary>
        public static List<AuthService> Services = new List<AuthService>();
        
        
        public Heartbeat Beat;
        public string NameSuffix    = "";
        public string DisplaySuffix = "";
        public string SkinPrefix    = "";
        
        public virtual bool Authenticate(Player p, string mppass) {
            string calculated = Server.CalcMppass(p.truename, Beat.Salt);
            if (!mppass.CaselessEq(calculated)) return false;
            
            p.verifiedName = true;
            p.name += NameSuffix;
            p.SkinName = SkinPrefix + p.SkinName;
            
            p.truename    += DisplaySuffix;
            p.DisplayName += DisplaySuffix;
            return true;
        }
        
        
        public static void Register(AuthService service) {
            Services.Add(service);
            Heartbeat.Register(service.Beat);
        }
        
        static string lastUrls;
        /// <summary> Reloads list of authentication services from server config </summary>
        public static void ReloadDefault() {
            string urls = Server.Config.HeartbeatURL;
            // don't reload services unless absolutely have to
            if (urls == lastUrls) return;
            lastUrls    = urls;
            
            // TODO only reload default heartbeats
            foreach (AuthService service in Services) {
                Heartbeat.Heartbeats.Remove(service.Beat);
            }
            Services.Clear();
            
            foreach (string url in urls.SplitComma())
            {
                Heartbeat   beat = new ClassiCubeBeat() { URL  = url  };
                AuthService auth = new AuthService()    { Beat = beat };
                Register(auth);
            }
        }
    }
}