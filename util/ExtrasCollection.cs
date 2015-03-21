/*
	Copyright 2011 MCGalaxy
		
	Dual-licensed under the	Educational Community License, Version 2.0 and
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
//
using System;
using System.Collections.Generic;
using System.Linq;

namespace MCGalaxy
{
    /// <summary>
    /// You can use this class to store extra information for the server
    /// For Example:
    /// This is useful if you want to store the value "lives" for the player object to 
    /// </summary>
    public sealed class ExtrasCollection : IDisposable
    {
        private Dictionary<string, object> _objects = new Dictionary<string, object>();

        /// <summary>
        /// An array of keys in the collection
        /// </summary>
        public String[] Keys
        {
            get
            {
                return _objects.Keys.ToArray();
            }
        }

        /// <summary>
        /// Total number of key/value pairs
        /// </summary>
        public int Count
        {
            get
            {
                return _objects.Count;
            }
        }

        #region Getters

        /// <summary>
        /// Returns the value associated with the given key as an object, or null if no value exists for this key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public object Get(String key)
        {
            return _objects.ContainsKey(key) ? _objects[key] : null;
        }

        /// <summary>
        /// Returns the value associated with the given key, or false if no mapping of the desired type exists for the given key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool GetBoolean(String key)
        {
            return GetBoolean(key, false);
        }

        /// <summary>
        /// Returns the value associated with the given key, or defaultValue if no mapping of the desired type exists for the given key.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public bool GetBoolean(String key, bool defaultValue)
        {
            if (_objects.ContainsKey(key))
            {
                try { return Convert.ToBoolean(_objects[key]); }
                catch (Exception) { }
            }
            
            return defaultValue;
        }

        /// <summary>
        /// Returns the value associated with the given key, or 0 if no mapping of the desired type exists for the given key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public int GetInt(String key)
        {
            return GetInt(key, 0);
        }

        /// <summary>
        /// Returns the value associated with the given key, or defaultValue if no mapping of the desired type exists for the given key.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public int GetInt(String key, int defaultValue)
        {
            if (_objects.ContainsKey(key))
            {
                try { return Convert.ToInt32(_objects[key]); }
                catch (Exception) { }
            }

            return defaultValue;
        }

        /// <summary>
        /// Returns the value associated with the given key, or null if no mapping of the desired type exists for the given key or a null value is explicitly associated with the key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string GetString(String key)
        {
            return GetString(key, null);
        }

        /// <summary>
        /// Returns the value associated with the given key, or defaultValue if no mapping of the desired type exists for the given key.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public string GetString(String key, String defaultValue)
        {
            if (_objects.ContainsKey(key))
            {
                try { return Convert.ToString(_objects[key]); }
                catch (Exception) { }
            }

            return defaultValue;
        }

        #endregion

        #region Setters

        /// <summary>
        /// Inserts a boolean value into the ExtrasCollection
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void PutBoolean(String key, bool value)
        {
            _objects.Add(key, value);
        }

        /// <summary>
        /// Inserts an integer value into the ExtrasCollection
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void PutInt(String key, int value)
        {
            _objects.Add(key, value);
        }

        /// <summary>
        /// Inserts a string value into the ExtrasCollection
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void PutString(String key, string value)
        {
            _objects.Add(key, value);
        }

        #endregion

        /// <summary>
        /// Returns true if the given key is contained in the mapping of this ExtrasCollection.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool ContainsKey(String key)
        {
            return _objects.ContainsKey(key);
        }

        /// <summary>
        /// Removes any entry with the given key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool Remove(String key)
        {
            return _objects.Remove(key);
        }

        /// <summary>
        /// Returns true if there is no data stored
        /// </summary>
        /// <returns></returns>
        public bool IsEmpty()
        {
            return _objects.Count == 0;
        }

        /// <summary>
        /// Removes all stored data
        /// </summary>
        public void Clear()
        {
            _objects.Clear();
        }


        public void Dispose()
        {
            this.Clear();
        }
    }
}
