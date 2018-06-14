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

namespace MCGalaxy {
    
    /// <summary> Represents a list of metadata about players. (such as rank info, ban info, notes). </summary>
    public sealed class PlayerMetaList {
        
        public readonly string file;
        readonly object locker;
        
        public PlayerMetaList(string file) {
            this.file = file;
            locker = new object();
        }
        
        public void EnsureExists() {
            if (!File.Exists(file))
                File.Create(file).Dispose();
        }

        /// <summary> Adds the given line to the end of the file. </summary>
        public void Append(string data) {
            lock (locker) {
                using (StreamWriter w = new StreamWriter(file, true))
                    w.WriteLine(data);
            }
        }
        
        
        /// <summary> Finds all lines which caselessly start with the given name. </summary>
        public IEnumerable<string> Find(string name) {
            if (!File.Exists(file)) yield break;
            name += " ";
            
            using (StreamReader r = new StreamReader(file)) {
                string line;
                while ((line = r.ReadLine()) != null) {
                    if (line.CaselessStarts(name)) yield return line;
                }
            }
            yield break;
        }

        public List<string> FindMatches(Player p, string name, string group) {
            int matches;
            return Matcher.FindMulti<string>(p, name, out matches, AllLines(),
                                             null, GetName, group);
        }
        
        IEnumerable<string> AllLines() {
            if (!File.Exists(file)) yield break;
            
            using (StreamReader r = new StreamReader(file)) {
                string line;
                while ((line = r.ReadLine()) != null) {
                    yield return line;
                }
            }
            yield break;
        }
        
        public static string GetName(string line) {
            int index = line.IndexOf(' ');
            return index == -1 ? line : line.Substring(0, index);
        }
    }
}
