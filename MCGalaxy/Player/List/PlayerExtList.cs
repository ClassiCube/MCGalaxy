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
    
    /// <summary> Represents a list of player names and simple associated data. Case insensitive. Thread safe. </summary>
    public class PlayerExtList {
        public char Separator = ' ';
        public string Path;
        
        List<string> names = new List<string>(), lines = new List<string>();
        internal readonly object locker = new object();
        readonly object saveLocker = new object();

        public List<string> AllNames() {
            lock (locker) return new List<string>(names);
        }
        
        /// <summary> Returns a copy of all lines (name + separator + data) in the list. </summary>
        public List<string> AllLines() {
            lock (locker) return new List<string>(lines);
        }
        
        /// <summary> Returns number of names that are in this list. </summary>
        public int Count { get { lock (locker) return names.Count; } }
        
        /// <summary> Sets the data associated with the given name. </summary>
        public void Update(string name, string data) {
            lock (locker) {
                int idx = names.CaselessIndexOf(name);
                if (idx == -1) {
                    names.Add(name); lines.Add(name + Separator + data);
                } else {
                    lines[idx] = name + Separator + data;
                }
            }
        }
        
        /// <summary> Returns whether the given name was removed from this list. </summary>
        public bool Remove(string name) {
            lock (locker) {
                int idx = names.CaselessIndexOf(name);
                if (idx == -1) return false;
                
                names.RemoveAt(idx);
                lines.RemoveAt(idx);
                return true;
            }
        }

        /// <summary> Returns whether the given name is in this list. </summary>
        public bool Contains(string name) {
            lock (locker) return names.CaselessContains(name);
        }

        /// <summary> Retrieves the data associated with the given name. </summary>
        /// <remarks> Returns null if there is no data associated. </remarks>
        public string FindData(string name) {
            lock (locker) {
                int idx = names.CaselessIndexOf(name);
                if (idx == -1) return null;
                
                string line = lines[idx];
                idx = line.IndexOf(Separator);
                return idx == -1 ? null : line.Substring(idx + 1);
            }
        }

        
        [Obsolete("Use Update instead")]
        public void Add(string name, string data) { Update(name, data); }

        [Obsolete("Use Update instead")]        
        public void AddOrReplace(string name, string data) { Update(name, data); }
        
        
        public void Save() { Save(true); }
        public void Save(bool log) {
            lock (saveLocker) {
                using (StreamWriter w = new StreamWriter(Path))
                    SaveEntries(w);
            }
            if (log) Logger.Log(LogType.BackgroundActivity, "SAVED: " + Path);
        }
        
        void SaveEntries(StreamWriter w) {
            lock (locker) {
                foreach (string line in lines) w.WriteLine(line);
            }
        }
        
        public static PlayerExtList Load(string path, char separator = ' ') {
            PlayerExtList list = new PlayerExtList();
            list.Path = path;
            list.Separator = separator;
            
            if (!File.Exists(path)) {
                File.Create(path).Close();
                Logger.Log(LogType.SystemActivity, "CREATED NEW: " + path);
                return list;
            }
            
            using (StreamReader r = new StreamReader(path, Encoding.UTF8)) {
                string line = null;
                while ((line = r.ReadLine()) != null) {
                    list.lines.Add(line);
                    int sepIndex = line.IndexOf(separator);
                    string name  = sepIndex >= 0 ? line.Substring(0, sepIndex) : line;
                    list.names.Add(name);
                }
            }
            return list;
        }
    }
}