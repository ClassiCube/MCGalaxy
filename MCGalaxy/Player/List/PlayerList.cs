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
    public class PlayerList {
        public string Path;
        
        List<string> names = new List<string>();        
        internal readonly object locker = new object();
        readonly object saveLocker = new object();
        
        public PlayerList() { }
        
        /// <summary> Returns a copy of all names in the list. </summary>
        public List<string> All() {
            lock (locker) return new List<string>(names);
        }

        /// <summary> Returns number of names that are in this list. </summary>
        public int Count { get { lock (locker) return names.Count; } }

        /// <summary> Returns whether the given name was actually added to this list. </summary>
        public bool Add(string name) {
            lock (locker) {
                int idx = names.CaselessIndexOf(name);
                if (idx >= 0) return false;
                
                names.Add(name);
            }
            return true;
        }

        /// <summary> Returns whether the given name was removed from this list. </summary>
        public bool Remove(string name) {
            lock (locker) return names.CaselessRemove(name);
        }
        
        /// <summary> Returns whether the given name is in this list. </summary>
        public bool Contains(string name) {
            lock (locker) return names.CaselessContains(name);
        }
        
        /// <summary> Removes all names from this list. </summary>
        public void Clear() {
            lock (locker) names.Clear();
        }
        

        [Obsolete("Use Add instead")]
        public bool AddUnique(string name) { return Add(name); }
        
        internal int IndexOf(string name) {
            lock (locker) return names.CaselessIndexOf(name);
        }
        
        internal string GetAt(int index) {
            lock (locker) {
                if (index < 0 || index >= names.Count) return null;
                return names[index];
            }
        }
        
        
        /// <summary> Finds matches within this list for the given name. </summary>
        public string FindMatches(Player p, string name, string type, out int matches) {
            lock (locker) {
                return Matcher.Find(p, name, out matches, names,
                                    null, n => n, type, 20);
            }
        }
        
        /// <summary> Outputs list of players using MultiPageOutput.Output. </summary>
        /// <remarks> Names are formatted using PlayerInfo.GetColoredName(). </remarks>
        public void Output(Player p, string group, string listCmd, string modifier) {
            List<string> list = All();
            if (list.Count == 0) {
                p.Message("There are no {0}.", group);
            } else {
                p.Message("{0}:", group.Capitalize());
                MultiPageOutput.Output(p, list,
                                       (name) => p.FormatNick(name),
                                       listCmd, "players", modifier, false);
            }
        }
        
        /// <summary> Outputs list of players using MultiPageOutput.Output. </summary>
        /// <remarks> Names are not formatted at all. </remarks>
        public void OutputPlain(Player p, string group, string listCmd, string modifier) {
            List<string> list = All();
            if (list.Count == 0) {
                p.Message("There are no {0}.", group);
            } else {
                p.Message("{0}:", group.Capitalize());
                MultiPageOutput.Output(p, list, (name) => name,
                                       listCmd, "players", modifier, false);
            }
        }
        

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
                foreach (string p in names) w.WriteLine(p);
            }
        }
        
        public static PlayerList Load(string path) {
            PlayerList list = new PlayerList();
            list.Path = path;
            
            if (!File.Exists(path)) {
                File.Create(path).Close();
                Logger.Log(LogType.SystemActivity, "CREATED NEW: " + path);
                return list;
            }
            
            using (StreamReader r = new StreamReader(path, Encoding.UTF8)) {
                string line = null;
                while ((line = r.ReadLine()) != null) {
                    list.names.Add(line);
                }
            }
            return list;
        }
    }
}