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
    public sealed class PlayersFile {
        
        public readonly string file;
        readonly object locker;
        
        public PlayersFile(string file) {
            this.file = file;
            locker = new object();
        }
        
        public void EnsureExists() {
            if (!File.Exists(file)) 
                File.Create(file).Dispose();
        }
        
        /// <summary> Finds all lines which caselessly start with the given name. </summary>
        public IEnumerable<string> Find(string name) {
            if (!File.Exists(file)) yield break;
            
            using (StreamReader r = new StreamReader(file)) {
                string line;
                while ((line = r.ReadLine()) != null) {
                    if (line.CaselessStarts(name)) yield return line;
                }
            }
            yield break;
        }
        
        /// <summary> Deletes all lines which start with the given value. </summary>
        public void DeleteStartsWith(string value) {
            if (!File.Exists(file)) return;
            List<string> lines = new List<string>();
            using (StreamReader r = new StreamReader(file)) {
                string line;
                while ((line = r.ReadLine()) != null) {
                    if (line.StartsWith(value)) continue;
                    lines.Add(line);
                }
            }
            WriteLines(lines);
        }
        
        /// <summary> Deletes all lines which contain the given value. </summary>
        public void DeleteContains(string value) {
            if (!File.Exists(file)) return;
            List<string> lines = new List<string>();
            using (StreamReader r = new StreamReader(file)) {
                string line;
                while ((line = r.ReadLine()) != null) {
                    if (line.Contains(value)) continue;
                    lines.Add(line);
                }
            }
            WriteLines(lines);
        }
        
        void WriteLines(List<string> lines) {
            lock (locker) {
                using (StreamWriter w = new StreamWriter(file, false)) {
                    foreach (string line in lines)
                        w.WriteLine(line);
                }
            }
        }
        
        /// <summary> Adds the given line to the end of the file. </summary>
        public void Append(string data) {
            string line = CP437Writer.ConvertToUnicode(data);
            lock (locker) {
                using (StreamWriter w = new StreamWriter(file, true))
                    w.WriteLine(line);
            }
        }
    }
}
