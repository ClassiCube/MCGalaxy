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

    public sealed class Team {
        
        public string Color, Name, Owner;
        public List<string> Members = new List<string>();
        
        public Team() { }
        public Team(string name, string owner) {
            Name = name;
            Owner = owner;
            Members.Add(owner);
        }
        
        public void Message(Player source, string message) {
            message = "&9- to team - λNICK: &f" + message;
            if (!source.CheckCanSpeak("send teamchat")) return;
            
            Chat.MessageChat(ChatScope.All, source, message, this,
                             (pl, arg) => pl.Game.Team == arg);
        }
        
        public void Action(Player source, string message) {
            message = "Team - λNICK &S" + message;
            Chat.MessageFrom(ChatScope.All, source, message, this,
                             (pl, arg) => pl.Game.Team == arg);
        }
        
        public bool Remove(string name) {
            return Members.CaselessRemove(name);
        }
                
        public void DeleteIfEmpty() {
            if (Members.Count > 0) return;
            Teams.Remove(this);
        }        
        
        public void UpdatePrefix() {
            foreach (string name in Members) {
                Player p = PlayerInfo.FindExact(name);
                if (p != null) p.SetPrefix();
            }
        }
        
        
        public static List<Team> Teams = new List<Team>();
        static readonly object ioLock = new object();
        
        public static Team TeamIn(Player p) {
            foreach (Team team in Teams) {
                List<string> members = team.Members;
                if (members.CaselessContains(p.name)) return team;
            }
            return null;
        }
        
        public static Team Find(string name) {
            name = Colors.Strip(name);
        	
            foreach (Team team in Teams) {
                string teamName = Colors.Strip(team.Name);
                if (name.CaselessEq(teamName)) return team;
            }
            return null;
        }
        
        public static void Add(Team team) {
            Team old = Find(team.Name);
            if (old != null) Teams.Remove(old);
            Teams.Add(team);
        }
        
        public static void SaveList() {
            lock (ioLock)
                using (StreamWriter w = new StreamWriter("extra/teams.txt"))
                    foreach (Team team in Teams)
            {
                w.WriteLine("Name=" + team.Name);
                w.WriteLine("Color=" + team.Color);
                w.WriteLine("Owner=" + team.Owner);
                string list = team.Members.Join(",");
                w.WriteLine("Members=" + list);
                w.WriteLine("");
            }
        }
        
        public static void LoadList() {
            if (!File.Exists("extra/teams.txt")) return;
            Team tmp = new Team();            
            
            lock (ioLock) {
                Teams.Clear();
                PropertiesFile.Read("extra/teams.txt", ref tmp, LineProcessor, '=');       
                if (tmp.Name != null) Add(tmp);
            }
        }
        
        static void LineProcessor(string key, string value, ref Team tmp) {
            switch (key.ToLower()) {
                case "name":
                    if (tmp.Name != null) Add(tmp);
                    tmp = new Team();
                    tmp.Name = value;
                    break;
                case "color":
                    tmp.Color = value; break;
                case "owner":
                    tmp.Owner = value; break;
                case "members":
                    tmp.Members = new List<string>(value.SplitComma()); break;
            }
        }
    }
}
