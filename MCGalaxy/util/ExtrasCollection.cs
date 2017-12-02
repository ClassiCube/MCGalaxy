/*
    Copyright 2011 MCForge
        
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

namespace MCGalaxy {
    
    /// <summary> You can use this class to store extra information for/about the player/level/server.
    /// For example: This is useful if you want to store the value "lives" for a player. </summary>
    public sealed class ExtrasCollection : Dictionary<string, object> {

        public object Get(string key) {
            object value;
            TryGetValue(key, out value);
            return value;
        }

        public bool GetBoolean(string key) { return GetBoolean(key, false); }
        public bool GetBoolean(string key, bool defaultValue) {
            object value;
            if (TryGetValue(key, out value)) {
                try { return Convert.ToBoolean(value); }
                catch (Exception) { }
            }
            return defaultValue;
        }

        public int GetInt(string key) { return GetInt(key, 0); }
        public int GetInt(string key, int defaultValue) {
            object value;
            if (TryGetValue(key, out value)) {
                try { return Convert.ToInt32(value); }
                catch (Exception) { }
            }
            return defaultValue;
        }

        public string GetString(string key) { return GetString(key, null); }
        public string GetString(string key, string defaultValue) {
            object value;
            if (TryGetValue(key, out value)) {
                try { return Convert.ToString(value); }
                catch (Exception) { }
            }
            return defaultValue;
        }

        public void PutBoolean(string key, bool value) { this[key] = value; }
        public void PutInt(string key, int value) { this[key] = value; }
        public void PutString(string key, string value) { this[key] = value; }
        public bool IsEmpty() { return Count == 0; }
    }
}