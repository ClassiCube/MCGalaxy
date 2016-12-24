/*
    Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCGalaxy)
    
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
using System.Text;

namespace MCGalaxy {
    public sealed class PlayerList {
        string path;
        List<string> players = new List<string>();
        readonly object locker = new object(), saveLocker = new object();
        
        public PlayerList() { }
        public PlayerList(string path) { this.path = path; }
        
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
        
        public int Count { get { lock (locker) return players.Count; } }
        
        /// <summary> Adds or replaces the given name. </summary>
        /// <returns> Whether the given player name was added to the list. </returns>
        public bool AddIfNotExists(string p) {
            p = p.ToLower();
            lock (locker) {
                int idx = players.IndexOf(p);
                if (idx >= 0) return false;
                
                players.Add(p);
            }
            return true;
        }
        
        public string FindMatches(Player p, string name, string type, out int matches) {
            lock (locker) {
                return Utils.FindMatches<string>(p, name, out matches, players,
                                                 n => true, n => n, type, 20);
            }
        }
        
        
        public void Save() { Save(true); }
        public void Save(bool console) {
            lock (saveLocker) {
                using (StreamWriter w = new StreamWriter(path))
                    SaveEntries(w);
            }
            if (console) Server.s.Log("SAVED: " + path, true);
        }
        
        void SaveEntries(StreamWriter w) {
            lock (locker) {
                foreach (string p in players)
                    w.WriteLine(p);
            }
        }
        
        public static PlayerList Load(string file) {
            if (!Directory.Exists("ranks")) Directory.CreateDirectory("ranks");
            PlayerList list = new PlayerList(file);
            if (file.IndexOf('/') == -1) file = "ranks/" + file;
            list.path = file;
            
            if (!File.Exists(list.path)) {
                File.Create(list.path).Close();
                Server.s.Log("CREATED NEW: " + list.path);
                return list;
            }
            
            using (StreamReader r = new StreamReader(list.path, Encoding.UTF8)) {
                string line = null;
                while ((line = r.ReadLine()) != null) {
                    // Need to convert uppercase to lowercase, in case user added in entries.
                    bool anyUpper = false;
                    for (int i = 0; i < line.Length; i++) {
                        char c = line[i];
                        anyUpper |= (c >= 'A' && c <= 'Z');
                    }
                    
                    if (anyUpper) line = line.ToLower();
                    list.players.Add(line);
                }
            }
            return list;
        }
        
        [Obsolete("Group parameter is completely ignored.")]
        public static PlayerList Load(string path, Group grp) { return Load(path); }
    }
}