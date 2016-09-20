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
using System.Text;

namespace MCGalaxy {
    public sealed class PlayerExtList {   
        
        string path;
        List<string> players = new List<string>();
        List<string> lines = new List<string>();
        readonly object locker = new object(), saveLocker = new object();
        
        public void Add(string p, string data) {
        	p = p.ToLower();
            lock (locker) {
                players.Add(p); lines.Add(p + " " + data);
            }
        }
        
        public bool Remove(string p) {
            lock (locker) {
                int idx = players.IndexOf(p.ToLower());
                if (idx == -1) return false;
                
                players.RemoveAt(idx);
                lines.RemoveAt(idx);
                return true;
            }
        }
        
        public void AddOrReplace(string p, string data) {
        	p = p.ToLower();
            lock (locker) {
                int idx = players.IndexOf(p);
                if (idx == -1) {
                	players.Add(p); lines.Add(p + " " + data);
                } else {
                	lines[idx] = p + " " + data;
                }
            }
        }
        
        public string Find(string p) {
            lock (locker) {
                int idx = players.IndexOf(p.ToLower());
                return idx == -1 ? null : lines[idx];
            }
        }
        
        public int Count { get { lock (locker) return players.Count; } }
        
        
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
                foreach (string l in lines)
                    w.WriteLine(l);
            }
        }
        
        public static PlayerExtList Load(string path) {
            PlayerExtList list = new PlayerExtList();
            list.path = path;
            
            if (!File.Exists(path)) {
                File.Create(path).Close();
                Server.s.Log("CREATED NEW: " + path);
                return list;
            }
            
            using (StreamReader r = new StreamReader(path, Encoding.UTF8)) {
                string line = null;
                while ((line = r.ReadLine()) != null) {
                    list.lines.Add(line);
                    int space = line.IndexOf(' ');
                    string name = space >= 0 ? line.Substring(0, space) : line;
                    
                    // Need to convert uppercase to lowercase, in case user added in entries.
                    bool anyUpper = false;
                    for (int i = 0; i < name.Length; i++) {
                        char c = line[i];
                        anyUpper |= (c >= 'A' && c <= 'Z');
                    }
                    if (anyUpper) name = name.ToLower();
                    list.players.Add(name);
                }
            }
            return list;
        }
    }
}