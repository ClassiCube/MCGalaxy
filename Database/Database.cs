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
using System.Data;
using System.IO;
using System.Linq;
using System.Text;

namespace MCGalaxy.SQL {
    
    public static partial class Database {
        
        [Obsolete("Use a ParameterisedQuery instance instead, which is threadsafe.")]
        public static void AddParams(string name, object param) {
            if (Server.useMySQL) MySQL.AddParams(name, param);
            else SQLite.AddParams(name, param);
        }

        [Obsolete("Use a ParameterisedQuery instance instead, which is threadsafe.")]
        public static void executeQuery(string queryString, bool createDB = false) {
            if (Server.useMySQL) executeQuery(MySQL.query, queryString, createDB);
            else executeQuery(SQLite.query, queryString, createDB);
        }

        public static void executeQuery(ParameterisedQuery query, string queryString, bool createDB = false) {
            Exception e = null;
            for (int i = 0; i < 10; i++) {
                try {
                    query.Execute(queryString, createDB);
                    query.ClearParams();
                    return;
                } catch (Exception ex) {
                    e = ex; // try yet again
                }
            }
            
            File.AppendAllText("MySQL_error.log", DateTime.Now + " " + queryString + "\r\n");
            Server.ErrorLog(e);
            query.ClearParams();
        }
        
        [Obsolete("Use a ParameterisedQuery instance instead, which is threadsafe.")]
        public static DataTable fillData(string queryString, bool skipError = false) {
            if (Server.useMySQL) return fillData(MySQL.query, queryString, skipError);
            else return fillData(SQLite.query, queryString, skipError);
        }

        public static DataTable fillData(ParameterisedQuery query, string queryString, bool skipError = false) {
            using (DataTable results = new DataTable("toReturn")) {
                Exception e = null;
                for (int i = 0; i < 10; i++) {
                    try {
                        query.Fill(queryString, results);
                        query.ClearParams();
                        return results;
                    } catch (Exception ex) {
                        e = ex; // try yet again
                    }
                }
                
                if (skipError) return results;
                File.AppendAllText("MySQL_error.log", DateTime.Now + " " + queryString + "\r\n");
                Server.ErrorLog(e);
                query.ClearParams();
                return results;
            }
        }
    }
}