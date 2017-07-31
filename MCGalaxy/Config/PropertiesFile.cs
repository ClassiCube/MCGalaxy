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
        
        public static bool Read(string path, Action<string, string> processor,
                                char separator = '=', bool trimValue = true) {
            object obj = null;
            LineProcessor<object> del = (string key, string value, ref object state) => { processor(key, value); };
            return Read(path, ref obj, del, separator, trimValue);
        }
        
        public static bool Read<T>(string path, ref T state, LineProcessor<T> processor,
                                   char separator = '=', bool trimValue = true) {
            if (!File.Exists(path)) return false;
            
            using (StreamReader reader = new StreamReader(path)) {
                string line;
                while ((line = reader.ReadLine()) != null) {
                    int index = ParseLine(line, path, separator);
                    if (index == -1) continue;
                    
                    string key = line.Substring(0, index), value = line.Substring(index + 1);
                    if (trimValue) value = value.Trim();
                    
                    try {
                        processor(key.Trim(), value, ref state);
                    } catch (Exception ex) {
                        Logger.LogError(ex);
                        Logger.Log(LogType.Warning, "Line \"{0}\" in {1} caused an error", line, path);
                    }
                }
            }
            return true;
        }
        
        static int ParseLine(string line, string path, char separator) {
            if (line.Length == 0 || line[0] == '#') return -1;
            int index = line.IndexOf(separator);
            if (index == -1) {
                Logger.Log(LogType.Warning, "Line \"{0}\" in {1} is missing a value", line, path);
                return -1;
            }
            return index;
        }
    }
}
