/*
    Copyright 2015 MCGalaxy
    
    Dual-licensed under the Educational Community License, Version 2.0 and
    the GNU General Public License, Version 3 (the "Licenses"); you may
    not use this file except in compliance with the Licenses. You may
    obtain a copy of the Licenses at
    
    http://www.osedu.org/licenses/ECL-2.0
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

namespace MCGalaxy.DB {
    
    /// <summary> Stores per-player persistent data. </summary>
    public static class PlayerDB {
        
        public static string LoginPath(string name) {
            return "text/login/" + name.ToLower() + ".txt";
        }
        
        public static string LogoutPath(string name) {
            return "text/logout/" + name.ToLower() + ".txt";
        }
        
        public static string InfectPath(string name) {
            return "text/infect/" + name.ToLower() + ".txt";
        }

        
        static char[] trimChars = new char[] {'='};
        public static bool Load( Player p ) {
            if (!File.Exists("players/" + p.name + "DB.txt")) return false;
            
            string[] lines = File.ReadAllLines( "players/" + p.name + "DB.txt");
            foreach (string line in lines) {
                if (line.Length == 0 || line[0] == '#') continue;
                string[] parts = line.Split(trimChars, 2);
                if (parts.Length < 2) continue;             
                string key = parts[0].Trim(), value = parts[1].Trim();

                if (key.CaselessEq("nick"))
                    p.DisplayName = value;
            }
            p.SetPrefix();
            return true;
        }

        public static void Save(Player p) {
            using (StreamWriter sw = new StreamWriter("players/" + p.name + "DB.txt", false))
                sw.WriteLine("Nick = " + p.DisplayName);
        }
        
        
        /// <summary> Retrieves the login message set for the given player. </summary>
        public static string GetLoginMessage(Player p) {
            if (!Directory.Exists("text/login"))
                Directory.CreateDirectory("text/login");
            
            string path = LoginPath(p.name);
            if (File.Exists(path)) return File.ReadAllText(path); 
            // Unix is case sensitive (older files used correct casing of name)
            path = "text/login/" + p.name + ".txt";
            return File.Exists(path) ? File.ReadAllText(path) : "connected";
        }

        /// <summary> Retrieves the logout message set for the given player. </summary>
        public static string GetLogoutMessage(Player p) {
            if (p.name == null) return "disconnected";
            if (!Directory.Exists("text/logout"))
                Directory.CreateDirectory("text/logout");
            
            string path = LogoutPath(p.name);
            if (File.Exists(path)) return File.ReadAllText(path); 
            
            path = "text/logout/" + p.name + ".txt";
            return File.Exists(path) ? File.ReadAllText(path) : "disconnected";
        }

        /// <summary> Retrieves the ZS infect messages list for the given player. </summary>
        public static List<string> GetInfectMessages(Player p) {
            if (p.name == null || !Directory.Exists("text/infect")) return null;
            string path = InfectPath(p.name);
            
            if (!File.Exists(path)) return null;
            string[] lines = File.ReadAllLines(path);
            return new List<string>(lines);
        }
        
        
        /// <summary> Sets the login message for the given player. </summary>         
        public static void SetLoginMessage(string name, string loginMsg) {
            File.WriteAllText(LoginPath(name), loginMsg);
        }

        /// <summary> Sets the logout message for the given player. </summary>        
        public static void SetLogoutMessage(string name, string logoutMsg) {
            File.WriteAllText(LogoutPath(name), logoutMsg);
        }
        
        public static void AppendInfectMessage(string name, string infectMsg) {
            if (!Directory.Exists("text/infect"))
                Directory.CreateDirectory("text/infect");
            
            string path = InfectPath(name);
            using (StreamWriter w = new StreamWriter(path, true))
                w.WriteLine(infectMsg);
        }
    }
}