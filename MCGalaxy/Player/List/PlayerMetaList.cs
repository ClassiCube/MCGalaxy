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
    /// <remarks> Unlike other player lists, this list is NOT kept in memory. </remarks>
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

        public void Append(string data) {
            lock (locker) {
                using (StreamWriter w = new StreamWriter(file, true))
                    w.WriteLine(data);
            }
        }
        
        public List<string> FindAllExact(string name) {
            List<string> entries = new List<string>();
            if (!File.Exists(file)) return entries;
            name += " ";
            
            using (StreamReader r = new StreamReader(file)) {
                string line;
                while ((line = r.ReadLine()) != null) {
                    if (line.CaselessStarts(name)) entries.Add(line);
                }
            }
            return entries;
        }
    }
}
