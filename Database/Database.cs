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
        
        [Obsolete("Use a ParameterisedQuery instance instead, which is threadsafe.")]
        public static void AddParams(string name, object param) {
            if (Server.useMySQL) MySQL.AddParams(name, param);
            else SQLite.AddParams(name, param);
        }
        
        public static bool TableExists(string table) {
            string syntax = Server.useMySQL ? MySQL.TableExists : SQLite.TableExists;
            using (DataTable results = Fill(syntax, table, Server.MySQLDatabaseName))
                return results.Rows.Count > 0;
        }
        
        [Obsolete("Use Execute() method instead.")]
        public static void executeQuery(string queryString, bool createDB = false) {
            ParameterisedQuery query = Server.useMySQL ? MySQL.query : SQLite.query;
            Execute(query, queryString, createDB, null);
        }

        [Obsolete("Use Execute() method instead.")]
        public static void executeQuery(ParameterisedQuery query, string queryString, bool createDB = false) {
            Execute(query, queryString, createDB, null);
        }
        
        public static void Execute(string queryString) {
            ParameterisedQuery query = Server.useMySQL ? MySQL.query : SQLite.query;
            Execute(query, queryString, false, null);
        }
        
        public static void Execute(string queryString, params object[] args) {
            ParameterisedQuery query = ParameterisedQuery.Create();
            Execute(query, queryString, false, args);
        }
        
        [Obsolete("Use Fill() method instead.")]
        public static DataTable fillData(string queryString, bool skipError = false) {
            ParameterisedQuery query = Server.useMySQL ? MySQL.query : SQLite.query;
            return Fill(query, queryString, null);
        }
        
        [Obsolete("Use Fill() method instead.")]
        public static DataTable fillData(ParameterisedQuery query, string queryString, bool skipError = false) {
            return Fill(query, queryString, null);
        }
        
        public static DataTable Fill(string queryString) {
            ParameterisedQuery query = Server.useMySQL ? MySQL.query : SQLite.query;
            return Fill(query, queryString, null);
        }
        
        public static DataTable Fill(string queryString, params object[] args) {
            ParameterisedQuery query = ParameterisedQuery.Create();
            return Fill(query, queryString, args);
        }


        static void Execute(ParameterisedQuery query, string queryString, bool createDB, params object[] args) {
            BindParams(query, args);
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
        
        static DataTable Fill(ParameterisedQuery query, string queryString, params object[] args) {
            BindParams(query, args);
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
    
    public static class MySQL { //: Database //Extending for future improvement (Making it object oriented later)
        
        static string connStringFormat = "Data Source={0};Port={1};User ID={2};Password={3};Pooling={4}";
        public static string connString { get { return String.Format(connStringFormat, Server.MySQLHost, Server.MySQLPort, Server.MySQLUsername, Server.MySQLPassword, Server.DatabasePooling); } }
        internal static ParameterisedQuery query = new MySQLParameterisedQuery();

        public static void AddParams(string name, object param) { query.AddParam(name, param); }
        
        public static void ClearParams() { query.ClearParams(); }
        
        public const string TableExists = "SELECT * FROM information_schema.tables WHERE table_schema = @1 AND table_name = @0";
    }
    
    public static class SQLite {
        
        static string connStringFormat = "Data Source =" + Server.apppath + "/MCGalaxy.db; Version =3; Pooling ={0}; Max Pool Size =300;";
        public static string connString { get { return String.Format(connStringFormat, Server.DatabasePooling); } }
        internal static ParameterisedQuery query = new SQLiteParameterisedQuery();

        public static void AddParams(string name, object param) { query.AddParam(name, param); }
        
        public static void ClearParams() { query.ClearParams(); }
        
        public const string TableExists = "SELECT name FROM sqlite_master WHERE type='table' AND name=@0";
    }
}