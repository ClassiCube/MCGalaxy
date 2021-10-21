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
using MCGalaxy.Config;
using MCGalaxy.Network;

namespace MCGalaxy.Authentication
{
    public sealed class AuthServiceConfig
    {
        public string URL;
        [ConfigString("name-suffix", null, "", true)]
        public string NameSuffix = "";
        [ConfigString("skin-prefix", null, "", true)]
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
        
        static ConfigElement[] cfg;
        static void LoadConfig() {
            AuthServiceConfig cur = null;
            if (cfg == null) cfg = ConfigElement.GetAll(typeof(AuthServiceConfig));
            
            configs.Clear();
            PropertiesFile.Read(Paths.AuthServicesFile, ref cur, ParseProperty, '=', true);
            if (cur != null) configs.Add(cur);
        }
        
        static void ParseProperty(string key, string value, ref AuthServiceConfig cur) {
            if (key.CaselessEq("URL")) {
                if (cur != null) configs.Add(cur);

                cur = new AuthServiceConfig() { URL = value };
            } else if (cur != null) {
                ConfigElement.Parse(cfg, cur, key, value);
            }
        }
        
        static void SaveConfig() {
            using (StreamWriter w = new StreamWriter(Paths.AuthServicesFile)) {
                w.WriteLine("# Authentication services configuration");
                // TODO: this description needs rewriting when I'm not so tired
                w.WriteLine("#   This file allows you to configure settings for players logging in through authentication services");
                w.WriteLine("#   This only makes sense for servers that are sending heartbeats to multiple authentication services (e.g. ClassiCube and BetaCraft)");
                w.WriteLine("#   DO NOT EDIT THIS FILE UNLESS YOU KNOW WHAT YOU ARE DOING");
                w.WriteLine();
                w.WriteLine("#URL = string");
                w.WriteLine("#   URL of authentication service the following configuration is for");
                w.WriteLine("#NameSuffix = string");
                w.WriteLine("#   Characters always added to end of usernames that login through the authentication service");
                w.WriteLine("#SkinPrefix = string");
                w.WriteLine();
                
                foreach (AuthServiceConfig c in configs) 
                {
                    w.WriteLine("URL = " + c.URL);
                    ConfigElement.SerialiseElements(cfg, w, c);
                    w.WriteLine();
                }
            }
        }
    }
}