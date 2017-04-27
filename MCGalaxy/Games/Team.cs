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
        
        public string Color;
        public string Name;
        public string Owner;
        public List<string> Members = new List<string>();
        
        public Team() { }
        
        public Team(string name, string owner) {
            Name = name;
            Owner = owner;
            Members.Add(owner);
        }
        
        public void Chat(Player source, string message) {
            string toSend = "&9- to team - " + source.ColoredName + ": &f" + message;
            foreach (string name in Members) {
                Player p = PlayerInfo.FindExact(name);
                if (p == null) continue;
                p.SendMessage(toSend);
            }
        }
        
        public void Action(Player source, string message) {
            string toSend = "Team - " + source.ColoredName + " %S" + message;
            foreach (string name in Members) {
                Player p = PlayerInfo.FindExact(name);
                if (p == null) continue;
                p.SendMessage(toSend);
            }
        }
        
        public bool Remove(string name) {
            for (int i = 0; i < Members.Count; i++) {
                if (!name.CaselessEq(Members[i])) continue;
                Members.RemoveAt(i); return true;
            }
            return false;
        }        
                
        public void RemoveIfEmpty() {
            if (Members.Count > 0) return;
            TeamsList.Remove(Name);
        }        
        
        public void UpdatePrefix() {
            foreach (string name in Members) {
                Player p = PlayerInfo.FindExact(name);
                if (p != null) p.SetPrefix();
            }
        }
    }
}
