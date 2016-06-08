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
        readonly object locker = new object();
        
        public void Add(string p, string data) {
            lock (locker) {
                players.Add(p.ToLower());
                lines.Add(p.ToLower() + " " + data.ToLower());
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
        
        public string Find(string p) {
            lock (locker) {
                int idx = players.IndexOf(p.ToLower());
                if (idx == -1) return null;
                return lines[idx];
            }
        }
        
        public int Count { get {
                lock (locker)
                    return players.Count;
            } }
        
        public void Save() { Save(path, true); }
        
        public void Save(string path, bool console) {
            using (StreamWriter w = File.CreateText(path)) {
                lock (locker) {
                    foreach (string line in lines)
                        w.WriteLine(line);
                }
            }
            if (console)
                Server.s.Log("SAVED: " + path);
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
                    // Need to convert uppercase to lowercase, in case user added in entries.
                    bool anyUpper = false;
                    for (int i = 0; i < line.Length; i++) {
                        char c = line[i];
                        anyUpper |= (c >= 'A' && c <= 'Z');
                    }
                    
                    if (anyUpper) line = line.ToLower();
                    list.lines.Add(line);
                    
                    int space = line.IndexOf(' ');
                    string name = space >= 0 ? line.Substring(0, space) : line;
                    list.players.Add(line);
                }
            }
            return list;
        }
    }
}