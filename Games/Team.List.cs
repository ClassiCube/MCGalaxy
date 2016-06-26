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

namespace MCGalaxy.Games {

    public sealed partial class Team {
        
        public static Dictionary<string, Team> TeamsList = new Dictionary<string, Team>();
        static readonly object ioLock = new object();
        
        public static Team FindTeam(Player p) {
            foreach (var team in TeamsList) {
                List<string> members = team.Value.Members;
                foreach (string member in members) {
                    if (p.name.CaselessEq(member))
                        return team.Value;
                }
            }
            return null;
        }
        
        public static Team FindTeam(string name) {
        	foreach (var team in TeamsList) {
        		if (name.CaselessEq(team.Key))
                    return team.Value;
            }
            return null;
        }
        
        public static void SaveList() {
            lock (ioLock)
                using (CP437Writer w = new CP437Writer("extra/teams.txt"))
                    foreach (var pair in TeamsList)
            {
                w.WriteLine("Name=" + pair.Value.Name);
                w.WriteLine("Color=" + pair.Value.Color);
                w.WriteLine("Owner=" + pair.Value.Owner);
                string list = String.Join(",", pair.Value.Members);
                w.WriteLine("Members=" + list);
                w.WriteLine("");
            }
        }
        
        public static void LoadList() {
            if (!File.Exists("extra/teams.txt")) return;
            Team team = new Team();
            lock (ioLock)
                PropertiesFile.Read("extra/teams.txt", ref team, LineProcessor, '=');
            if (team.Name != null) TeamsList[team.Name] = team;
        }
        
        static void LineProcessor(string key, string value, ref Team team) {
            switch (key.ToLower()) {
                case "name":
                    if (team.Name != null) TeamsList[team.Name] = team;
                    team = new Team();
                    team.Name = value;
                    break;
                case "color":
                    team.Color = value; break;
                case "owner":
                    team.Owner = value; break;
                case "members":
                    team.Members = new List<string>(value.Split(',')); break;
            }
        }
    }
}
