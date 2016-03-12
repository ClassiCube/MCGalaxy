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
using System.Linq;

namespace MCGalaxy.Games {

    public sealed class Team {
        
        public string Color;
        public string Name;
        public string Founder;
        public List<string> Members = new List<string>();
        
        public static Dictionary<string, Team> TeamsList = new Dictionary<string, Team>();
        
        public static void SaveList() {
            using (StreamWriter w = new StreamWriter("extra/teams.txt")) {
                foreach (var pair in TeamsList) {
                    w.WriteLine("Name=" + pair.Value.Name);
                    w.WriteLine("Color=" + pair.Value.Color);
                    w.WriteLine("Founder=" + pair.Value.Founder);
                    string list = String.Join(",", TeamsList.ToArray());
                    w.WriteLine("Members=" + list);
                    w.WriteLine("");
                }
            }
        }
        
        public static void LoadList() {
            if (!File.Exists("extra/teams.txt")) return;
            Team team = new Team();
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
                case "founder":
                    team.Founder = value; break;
                case "members":
                    team.Members = new List<string>(value.Split(',')); break;
            }
        }
    }
}
