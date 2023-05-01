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
using MCGalaxy.SQL;

namespace MCGalaxy.DB 
{
    /// <summary> Stores per-player persistent data. </summary>
    public static class PlayerDB 
    {        
        static string LoginPath(string name)  { return "text/login/"  + name.ToLower() + ".txt"; }
        static string LogoutPath(string name) { return "text/logout/" + name.ToLower() + ".txt"; }

        const string NICK_PREFIX = "Nick = ";
        public static string LoadNick(string name) {
            string path = "players/" + name + "DB.txt";
            if (!File.Exists(path)) return null;

            foreach (string line in File.ReadAllLines(path)) 
            {
                if (!line.CaselessStarts(NICK_PREFIX)) continue;

                return line.Substring(NICK_PREFIX.Length).Trim();
            }
            return null;
        }

        public static void SetNick(string name, string nick) {
            EnsureDirectoriesExist();
            using (StreamWriter sw = new StreamWriter("players/" + name + "DB.txt", false))
                sw.WriteLine(NICK_PREFIX + nick);
        }
        
        
        public static string GetLoginMessage(string name) {
            string path = LoginPath(name);
            if (File.Exists(path)) return File.ReadAllText(path);
            
            // Filesystem is case sensitive (older files used correct casing of name)
            path = "text/login/" + name + ".txt";
            return File.Exists(path) ? File.ReadAllText(path) : "";
        }

        public static string GetLogoutMessage(string name) {
            string path = LogoutPath(name);
            if (File.Exists(path)) return File.ReadAllText(path);
            
            path = "text/logout/" + name + ".txt";
            return File.Exists(path) ? File.ReadAllText(path) : "";
        }
        
        static void SetMessage(string path, string msg) {
            EnsureDirectoriesExist();
            if (msg.Length > 0) {
                File.WriteAllText(path, msg);
            } else if (File.Exists(path)) {
                File.Delete(path);
            }
        }
        
        public static void SetLoginMessage(string name, string msg) {
            SetMessage(LoginPath(name), msg);
        }
        
        public static void SetLogoutMessage(string name, string msg) {
            SetMessage(LogoutPath(name), msg);
        }
        
        
        /// <summary> Returns the fields of the row whose Name field caselessly equals the given name </summary>
        public static PlayerData FindData(string name) {
            string suffix   = Database.Backend.CaselessWhereSuffix;
            PlayerData data = null;

            Database.ReadRows("Players", "*",
                                record => data = PlayerData.Parse(record),
                                "WHERE Name=@0" + suffix, name);
            return data;
        }

        /// <summary> Returns the Name field of the row whose Name field caselessly equals the given name </summary>
        public static string FindName(string name) {
            string suffix = Database.Backend.CaselessWhereSuffix;
            return Database.ReadString("Players", "Name", "WHERE Name=@0" + suffix, name);
        }
        
        /// <summary> Returns the IP field of the row whose Name field caselessly equals the given name </summary>
        public static string FindIP(string name) {
            string suffix = Database.Backend.CaselessWhereSuffix;
            return Database.ReadString("Players", "IP", "WHERE Name=@0" + suffix, name);
        }
        
        public static string FindOfflineIPMatches(Player p, string name, out string ip) {
            string[] match = PlayerDB.MatchValues(p, name, "Name,IP");
            ip   = match == null ? null : match[1];
            return match == null ? null : match[0];
        }
        
        
        public static void Update(string name, string column, string value) {
            Database.UpdateRows("Players", column + "=@1", "WHERE Name=@0", name, value);
        }
        
        public static string FindColor(Player p) {
            string raw = Database.ReadString("Players", "Color", "WHERE ID=@0", p.DatabaseID);
            if (raw == null) return "";
            return PlayerData.ParseColor(raw);
        }
        
        
        public static string MatchNames(Player p, string name) {
            List<string> names = new List<string>();
            MatchMulti(name, "Name",
                       record => names.Add(record.GetText(0)));
            
            int matches;
            return Matcher.Find(p, name, out matches, names,
                                null, n => n, "players", 20);
        }
        
        public static string[] MatchValues(Player p, string name, string columns) {
            List<string[]> name_values = new List<string[]>();
            MatchMulti(name, columns,
                       record => name_values.Add(Database.ParseFields(record)));

            int matches;
            return Matcher.Find(p, name, out matches, name_values,
                                null, n => n[0], "players", 20);
        }  
        
        public static PlayerData Match(Player p, string name) {
            List<PlayerData> stats = new List<PlayerData>();
            MatchMulti(name, "*", record => stats.Add(PlayerData.Parse(record)));
            
            int matches;
            return Matcher.Find(p, name, out matches, stats,
                                null, stat => stat.Name, "players", 20);
        }
        
        static void MatchMulti(string name, string columns, ReaderCallback callback) {
            string suffix = Database.Backend.CaselessLikeSuffix;
            Database.ReadRows("Players", columns, callback,
                              "WHERE Name LIKE @0 ESCAPE '#' LIMIT 101" + suffix,
                              "%" + name.Replace("_", "#_") + "%");
        }
        
        
        public static void EnsureDirectoriesExist() {
            if (!Directory.Exists("text/login"))
                Directory.CreateDirectory("text/login");
            if (!Directory.Exists("text/logout"))
            	Directory.CreateDirectory("text/logout");
            if (!Directory.Exists("players"))
                Directory.CreateDirectory("players");
        }
    }
}