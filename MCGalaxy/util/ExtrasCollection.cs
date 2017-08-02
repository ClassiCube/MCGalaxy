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

        /// <summary> Returns the value associated with the given key as an object, or null if no value exists for this key </summary>
        public object Get(string key) {
            object value;
            TryGetValue(key, out value);
            return value;
        }

        /// <summary> Returns the value associated with the given key, or false if no mapping of the desired type exists for the given key. </summary>
        public bool GetBoolean(string key) { return GetBoolean(key, false); }

        /// <summary> Returns the value associated with the given key, or defaultValue if no mapping of the desired type exists for the given key. </summary>
        public bool GetBoolean(string key, bool defaultValue) {
            object value;
            if (TryGetValue(key, out value)) {
                try { return Convert.ToBoolean(value); }
                catch (Exception) { }
            }
            return defaultValue;
        }

        /// <summary> Returns the value associated with the given key, or 0 if no mapping of the desired type exists for the given key. </summary>
        public int GetInt(string key) { return GetInt(key, 0); }

        /// <summary> Returns the value associated with the given key, or defaultValue if no mapping of the desired type exists for the given key. </summary>
        public int GetInt(string key, int defaultValue) {
            object value;
            if (TryGetValue(key, out value)) {
                try { return Convert.ToInt32(value); }
                catch (Exception) { }
            }
            return defaultValue;
        }

        /// <summary> Returns the value associated with the given key, or null if no mapping of the desired type exists for the given key 
        /// or a null value is explicitly associated with the key. </summary>
        public string GetString(string key) { return GetString(key, null); }

        /// <summary> Returns the value associated with the given key, or defaultValue if no mapping of the desired type exists for the given key. </summary>
        public string GetString(string key, string defaultValue) {
            object value;
            if (TryGetValue(key, out value)) {
                try { return Convert.ToString(value); }
                catch (Exception) { }
            }
            return defaultValue;
        }

        /// <summary> Inserts a boolean value into the ExtrasCollection </summary>
        public void PutBoolean(string key, bool value) { this[key] = value; }

        /// <summary> Inserts an integer value into the ExtrasCollection </summary>
        public void PutInt(string key, int value) { this[key] = value; }

        /// <summary> Inserts a string value into the ExtrasCollection </summary>
        public void PutString(string key, string value) { this[key] = value; }

        /// <summary> Returns true if there is no data stored </summary>
        public bool IsEmpty() { return Count == 0; }
    }
}