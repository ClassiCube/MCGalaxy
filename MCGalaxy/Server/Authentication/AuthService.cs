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
using System.IO;
using MCGalaxy.Network;

namespace MCGalaxy.Authentication
{
    public sealed class AuthServiceConfig
    {
        public string URL;
        public string NameSuffix = "";
        public string SkinPrefix = "";
    }
    
    public class AuthService
    {
        /// <summary> List of all authentication services </summary>
        public static List<AuthService> Services = new List<AuthService>();        
        
        public Heartbeat Beat;
        public AuthServiceConfig Config;
        
        public virtual bool Authenticate(Player p, string mppass) {
            string calculated = Server.CalcMppass(p.truename, Beat.Salt);
            if (!mppass.CaselessEq(calculated)) return false;
            AuthServiceConfig cfg = Config;
            
            p.verifiedName = true;
            p.SkinName     = cfg.SkinPrefix + p.SkinName;
            
            p.name        += cfg.NameSuffix;
            p.truename    += cfg.NameSuffix;
            p.DisplayName += cfg.NameSuffix;
            return true;
        }
        
        
        static string lastUrls;
        /// <summary> Reloads list of authentication services from server config </summary>
        public static void ReloadDefault() {
            string urls = Server.Config.HeartbeatURL;
            
            // don't reload services unless absolutely have to
            if (urls != lastUrls) {
                lastUrls = urls;
                ReloadServices();
            }
            
            LoadConfig();
            foreach (AuthService service in Services)
            {
                service.Config = GetOrCreateConfig(service.Beat.URL);
            }
        }
        
        static void ReloadServices() {
            // TODO only reload default auth services, don't clear all
            foreach (AuthService service in Services) 
            {
                Heartbeat.Heartbeats.Remove(service.Beat);
            }
            Services.Clear();
            
            foreach (string url in lastUrls.SplitComma())
            {
                Heartbeat   beat = new ClassiCubeBeat() { URL  = url  };
                AuthService auth = new AuthService()    { Beat = beat };
                
                Services.Add(auth);
                Heartbeat.Register(beat);
            }
        }
        
        
        static List<AuthServiceConfig> configs = new List<AuthServiceConfig>();
        static AuthServiceConfig GetOrCreateConfig(string url)
        {
            foreach (AuthServiceConfig c in configs)
            {
                if (c.URL.CaselessEq(url)) return c;
            }
            
            AuthServiceConfig cfg = new AuthServiceConfig() { URL = url };
            configs.Add(cfg);
            
            try {
                SaveConfig();
            } catch (Exception ex) {
                Logger.LogError("Error saving authservices.properties", ex);
            }
            return cfg;
        }
        
        static void LoadConfig() {
            configs.Clear();
            
            AuthServiceConfig cur = null;
            PropertiesFile.Read(Paths.AuthServicesFile, ref cur, ParseProperty, '=', true);
            if (cur != null) configs.Add(cur);
        }
        
        static void ParseProperty(string key, string value, ref AuthServiceConfig cur) {
            if (key.CaselessEq("URL")) {
                if (cur != null) configs.Add(cur);

                cur = new AuthServiceConfig() { URL = value };
            } else if (key.CaselessEq("name-suffix")) {
                if (cur == null) return;
                cur.NameSuffix = value;
            } else if (key.CaselessEq("skin-prefix")) {
                if (cur == null) return;
                cur.SkinPrefix = value;
            }
        }
        
        static void SaveConfig() {
            using (StreamWriter w = new StreamWriter(Paths.AuthServicesFile)) {
                w.WriteLine("# Authentication services configuration");
                w.WriteLine("#   There is no reason to modify these configuration settings, unless the server has been configured");
                w.WriteLine("#    to send heartbeats to multiple authentication services (e.g. both ClassiCube.net and BetaCraft.pl)");
                w.WriteLine("#   DO NOT EDIT THIS FILE UNLESS YOU KNOW WHAT YOU ARE DOING");
                w.WriteLine();
                w.WriteLine("#URL = string");
                w.WriteLine("#   URL of the authentication service the following settings apply to");
                w.WriteLine("#   (this must be the same as one of the heartbeat URLs specified in server.properties)");
                w.WriteLine("#name-suffix = string");
                w.WriteLine("#   Characters that are appended to usernames of players that login through the authentication service");
                w.WriteLine("#   (used to prevent username collisions between authentication services that would otherwise occur)");
                w.WriteLine("#skin-prefix = string");
                w.WriteLine("#   Characters that are prefixed to skin name of players that login through the authentication service");
                w.WriteLine("#   (used to ensure players from other authentication services see the correct skin)");
                w.WriteLine();
                
                foreach (AuthServiceConfig c in configs) 
                {
                    w.WriteLine("URL = " + c.URL);
                    w.WriteLine("name-suffix = " + c.NameSuffix);
                    w.WriteLine("skin-prefix = " + c.SkinPrefix);
                    w.WriteLine();
                }
            }
        }
    }
}