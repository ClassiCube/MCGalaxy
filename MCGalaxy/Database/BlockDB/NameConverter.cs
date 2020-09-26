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
using System.Data;
using MCGalaxy.SQL;

namespace MCGalaxy.DB {
    
    /// <summary> Converts names to integer ids and back </summary>
    public static class NameConverter {
        
        // NOTE: this restriction is due to BlockDBCacheEntry
        public const int MaxPlayerID = 0x00FFFFFF;
        
        /// <summary> Returns the name associated with the given ID, or ID#[id] if not found </summary>
        public static string FindName(int id) {
            // Only returns non-null if id > MaxPlayerID - invalid.Count
            string name = Server.invalidIds.GetAt(MaxPlayerID - id);
            if (name != null) return name;
            
            name = Database.ReadString("Players", "Name", "WHERE ID=@0", id);
            return name != null ? name : "ID#" + id;
        }
        
        static object ListIds(IDataRecord record, object arg) {
            ((List<int>)arg).Add(record.GetInt32(0));
            return arg;
        }
        
        /// <summary> Finds all the IDs associated with the given name. </summary>
        public static int[] FindIds(string name) {
            List<int> ids = new List<int>();
            
            int i = Server.invalidIds.IndexOf(name);
            if (i >= 0) ids.Add(MaxPlayerID - i);
            
            Database.ReadRows("Players", "ID", ids, ListIds, "WHERE Name=@0", name);
            return ids.ToArray();
        }
        
        /// <summary> Returns a non-database ID for the given name </summary>
        public static int InvalidNameID(string name) {
            bool added = Server.invalidIds.Add(name);
            if (added) Server.invalidIds.Save();
            
            int index = Server.invalidIds.IndexOf(name);
            return MaxPlayerID - index;
        }
    }
}