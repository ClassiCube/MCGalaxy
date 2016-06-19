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
using System.IO;

namespace MCGalaxy {
    
	public delegate void LineProcessor<T>(string key, string value, ref T state);
	
    /// <summary> Handles text files that have multiple key-value lines in the format 'key=value'. 
    /// Also supports # for commented lines. </summary>
    public static class PropertiesFile {
        
        public static bool Read(string path, Action<string, string> processor, char separator = '=') {
            if (!File.Exists(path)) return false;
            
            using (StreamReader reader = new CP437Reader(path)) {
                string line;
                while ((line = reader.ReadLine()) != null) {
                    if (line == "" || line[0] == '#') continue;
                    int index = line.IndexOf(separator);
                    
                    string key = index < 0 ? line : line.Substring(0, index);
                    string value = index < 0 ? "" : line.Substring(index + 1);
                    processor(key.Trim(), value.Trim());
                }
            }
            return true;
        }
    	
    	public static bool Read<T>(string path, ref T state, LineProcessor<T> processor, char separator = '=') {
            if (!File.Exists(path)) return false;
            
            using (StreamReader reader = new CP437Reader(path)) {
                string line;
                while ((line = reader.ReadLine()) != null) {
                    if (line == "" || line[0] == '#') continue;
                    int index = line.IndexOf(separator);
                    
                    string key = index < 0 ? line : line.Substring(0, index);
                    string value = index < 0 ? "" : line.Substring(index + 1);
                    processor(key.Trim(), value.Trim(), ref state);
                }
            }
            return true;
        }
    }
}
