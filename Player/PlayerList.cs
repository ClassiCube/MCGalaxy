/*
    Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCGalaxy)
    
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
using System.IO;
using System.Collections.Generic;

namespace MCGalaxy {
    
    public sealed class PlayerList {
        
        public Group group;
        List<string> players = new List<string>();
        readonly object locker = new object();
        public PlayerList() { }
        
        public void Add(string p) {
            lock (locker)
                players.Add(p.ToLower());
        }
        
        public bool Remove(string p) {
            lock (locker)
                return players.Remove(p.ToLower());
        }
        
        public bool Contains(string p) {
            lock (locker)
                return players.Contains(p.ToLower());
        }
        
        public List<string> All() {
            lock (locker)
                return new List<string>(players);
        }
        
        public void Save() { Save(group.fileName, true); }
        
        public void Save(string path, bool console) {
            using (StreamWriter w = File.CreateText("ranks/" + path)) {
                lock (locker) {
                    foreach (string p in players)
                        w.WriteLine(p);
                }
            }
            if (console)
                Server.s.Log("SAVED: " + path);
        }
        
        public static PlayerList Load(string path, Group groupName) {
            if (!Directory.Exists("ranks"))
                Directory.CreateDirectory("ranks");
            path = "ranks/" + path;
            PlayerList list = new PlayerList();
            list.group = groupName;
            
            if (File.Exists(path)) {
                foreach (string line in File.ReadAllLines(path)) { list.Add(line); }
            } else {
                File.Create(path).Close();
                Server.s.Log("CREATED NEW: " + path);
            }
            return list;
        }
    }
}