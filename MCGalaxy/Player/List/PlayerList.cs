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
    
    /// <summary> Represents a list of player names. Case insensitive. Thread safe. </summary>
    public sealed class PlayerList {
        
        /// <summary> Path to the file that stores this list on disc. </summary>
        public string Path;
        
        List<string> names = new List<string>();        
        internal readonly object locker = new object();
        readonly object saveLocker = new object();
        
        public PlayerList() { }
        public PlayerList(string path) { Path = path; }
        
        /// <summary> Returns a copy of all names in the list. </summary>
        public List<string> All() {
            lock (locker)
                return new List<string>(names);
        }
        
        /// <summary> Returns the number of names in the list. </summary>
        public int Count { get { lock (locker) return names.Count; } }
        
        
        /// <summary> Adds the given name to the list. Does not check for duplicates. </summary>
        public void Add(string name) {
            lock (locker)
                names.Add(name);
        }

        /// <summary> Removes the given name to the list, 
        /// returning whether it was in the list to begin with. </summary>
        public bool Remove(string name) {
            lock (locker)
                return names.CaselessRemove(name);
        }
        
        /// <summary> Returns whether the given name is in the list. </summary>
        public bool Contains(string name) {
            lock (locker)
                return names.CaselessContains(name);
        }
        
        /// <summary> Adds the given name, if it is not already in the list. </summary>
        /// <returns> Whether the given name was added to the list. </returns>
        public bool AddIfNotExists(string name) {
            lock (locker) {
                int idx = names.CaselessIndexOf(name);
                if (idx >= 0) return false;
                
                names.Add(name);
            }
            return true;
        }
        
        /// <summary> Finds matches within this list for the given name. </summary>
        public string FindMatches(Player p, string name, string type, out int matches) {
            lock (locker) {
                return Matcher.Find<string>(p, name, out matches, names,
                                            null, n => n, type, 20);
            }
        }
        
        
        /// <summary> Saves the list of names to disc, also logging to console. </summary>
        public void Save() { Save(true); }
        
        /// <summary> Saves the list of names to disc, optionally logging to console. </summary>
        public void Save(bool log) {
            lock (saveLocker) {
                using (StreamWriter w = new StreamWriter(Path))
                    SaveEntries(w);
            }
            if (log) Server.s.Log("SAVED: " + Path, true);
        }
        
        void SaveEntries(StreamWriter w) {
            lock (locker) {
                foreach (string p in names)
                    w.WriteLine(p);
            }
        }
        
        /// <summary> Loads a list of names from disc. </summary>
        public static PlayerList Load(string file) {
            if (!Directory.Exists("ranks")) Directory.CreateDirectory("ranks");
            PlayerList list = new PlayerList(file);
            if (file.IndexOf('/') == -1) file = "ranks/" + file;
            list.Path = file;
            
            if (!File.Exists(list.Path)) {
                File.Create(list.Path).Close();
                Server.s.Log("CREATED NEW: " + list.Path);
                return list;
            }
            
            using (StreamReader r = new StreamReader(list.Path, Encoding.UTF8)) {
                string line = null;
                while ((line = r.ReadLine()) != null) {
                    list.names.Add(line);
                }
            }
            return list;
        }
    }
}