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
using System.Data;
using System.IO;

namespace MCGalaxy.SQL {
    public static partial class Database {
        
		public static IDatabaseBackend Backend;
        
        public static bool TableExists(string table) {
		    return Backend.TableExists(table);
        }
        
        [Obsolete("Use Execute() method instead.")]
        public static void executeQuery(string queryString, bool createDB = false) {
        	ParameterisedQuery query = Backend.GetStaticParameterised();
            Execute(query, queryString, createDB, null);
        }
        
        public static void Execute(string queryString) {
            ParameterisedQuery query = Backend.GetStaticParameterised();
            Execute(query, queryString, false, null);
        }
        
        public static void Execute(string queryString, params object[] args) {
            ParameterisedQuery query = Backend.CreateParameterised();
            Execute(query, queryString, false, args);
        }
        
        [Obsolete("Use Fill() method instead.")]
        public static DataTable fillData(string queryString, bool skipError = false) {
            ParameterisedQuery query = Backend.GetStaticParameterised();
            return Fill(query, queryString, null);
        }
        
        public static DataTable Fill(string queryString) {
            ParameterisedQuery query = Backend.GetStaticParameterised();
            return Fill(query, queryString, null);
        }
        
        public static DataTable Fill(string queryString, params object[] args) {
        	ParameterisedQuery query = Backend.CreateParameterised();
            return Fill(query, queryString, args);
        }


        static void Execute(ParameterisedQuery query, string queryString, 
                            bool createDB, params object[] args) {
            BindParams(query, args);
            string connString = Backend.ConnectionString;
            Exception e = null;
            
            for (int i = 0; i < 10; i++) {
                try {
                    query.Execute(queryString, connString, createDB);
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
        
        static DataTable Fill(ParameterisedQuery query, string queryString, 
                              params object[] args) {
            BindParams(query, args);
            string connString = Backend.ConnectionString;
            Exception e = null;
            
            using (DataTable results = new DataTable("toReturn")) {
                for (int i = 0; i < 10; i++) {
                    try {
                        query.Fill(queryString, connString, results);
                        query.ClearParams();
                        return results;
                    } catch (Exception ex) {
                        e = ex; // try yet again
                    }
                }
                
                File.AppendAllText("MySQL_error.log", DateTime.Now + " " + queryString + "\r\n");
                Server.ErrorLog(e);
                query.ClearParams();
                return results;
            }
        }

        static readonly object idsLock = new object();
        static string[] ids = null;
        static void BindParams(ParameterisedQuery query, object[] args) {
            if (args == null || args.Length == 0) return;
            
            // Avoid allocation overhead from string concat every query by caching
            string[] names = null;
            lock (idsLock) {
                names = ids;
                if (ids == null || args.Length > ids.Length) {
                    ids = new string[args.Length];
                    for (int i = 0; i < args.Length; i++)
                        ids[i] = "@" + i;
                    names = ids;
                }
            }
            for (int i = 0; i < args.Length; i++)
                query.AddParam(names[i], args[i]);
        }
    }
}