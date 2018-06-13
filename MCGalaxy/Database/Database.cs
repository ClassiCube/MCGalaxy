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
    public static class Database {
        
        public static IDatabaseBackend Backend;
        
        /// <summary> Returns whether the given table exists in the database. </summary>
        public static bool TableExists(string table) { return Backend.TableExists(table); }

        
        /// <summary> Executes an SQL command that does not return any results. </summary>
        public static void Execute(string sql) {
            ParameterisedQuery query = Backend.GetStaticParameterised();
            Execute(query, sql, false, null);
        }

        /// <summary> Executes an SQL command that does not return any results. </summary>
        public static void Execute(string sql, params object[] args) {
            ParameterisedQuery query = Backend.CreateParameterised();
            Execute(query, sql, false, args);
        }

        /// <summary> Executes an SQL query, invoking callback function on each returned row. </summary>
        public static void Iterate(string sql, ReaderCallback callback, params object[] args) {
            ParameterisedQuery query = Backend.CreateParameterised();
            DoDatabaseCall(query, sql, false, null, callback, args);
        }
        
        /// <summary> Executes an SQL query, returning all read rows into a DataTable. </summary>
        public static DataTable Fill(string sql) {
            ParameterisedQuery query = Backend.GetStaticParameterised();
            return Fill(query, sql, null);
        }

        /// <summary> Executes an SQL query, returning all read rows into a DataTable. </summary>        
        public static DataTable Fill(string sql, params object[] args) {
            ParameterisedQuery query = Backend.CreateParameterised();
            return Fill(query, sql, args);
        }


        internal static void Execute(ParameterisedQuery query, string sql, bool createDB, params object[] args) {
            DoDatabaseCall(query, sql, createDB, null, null, args);
        }
        
        internal static DataTable Fill(ParameterisedQuery query, string sql, params object[] args) {
            using (DataTable results = new DataTable("toReturn")) {
                DoDatabaseCall(query, sql, false, results, null, args);
                return results;
            }
        }
        
        static void DoDatabaseCall(ParameterisedQuery query, string sql, bool createDB, 
                                   DataTable results, ReaderCallback callback, params object[] args) {
            query.parameters = args;
            string connString = Backend.ConnectionString;
            Exception e = null;
            
            for (int i = 0; i < 10; i++) {
                try {
                    if (callback != null) {
                        query.Iterate(sql, connString, callback);
                    } else if (results == null) {
                        query.Execute(sql, connString, createDB);
                    } else {
                        query.Fill(sql, connString, results);
                    }
                    
                    query.parameters = null;
                    return;
                } catch (Exception ex) {
                    e = ex; // try yet again
                }
            }
            
            query.parameters = null;
            File.AppendAllText("MySQL_error.log", DateTime.Now + " " + sql + "\r\n");
            Logger.LogError(e);
        }

        
        volatile static string[] ids;
        internal static string[] GetParamNames(int count) {
            // Avoid allocation overhead from string concat every query by caching
            string[] names = ids;
            if (names == null || count > names.Length) {
                names = new string[count];
                for (int i = 0; i < names.Length; i++) { names[i] = "@" + i; }
                ids = names;
            }
            return names;
        }
    }
}