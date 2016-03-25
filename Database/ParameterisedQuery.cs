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

namespace MCGalaxy.SQL {

    public abstract class ParameterisedQuery {
        
		protected Dictionary<string, object> parameters = new Dictionary<string, object>();
		public void AddParam(string name, object param) {
			parameters.Add(name, param);
		}
        
		public void ClearParams() {
			parameters.Clear();
		}
        
        public abstract void Execute(string query, bool createDB = false);
        
        public abstract void Fill(string query, DataTable results);
        
        public static ParameterisedQuery Create() {
        	if (Server.useMySQL) return new MySQLParameterisedQuery();
        	else return new SQLiteParameterisedQuery();
        }
    }
}
