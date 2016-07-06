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
        
        public static bool TableExists(string tableName) {
            ParameterisedQuery query = ParameterisedQuery.Create();
            query.AddParam("@Name", tableName);
            query.AddParam("@DB", Server.MySQLDatabaseName);
            
            string syntax = Server.useMySQL ? MySQL.TableExists : SQLite.TableExists;
            using (DataTable table = fillData(query, syntax))
                return table.Rows.Count > 0;
        }
    }
	
	public static class MySQL { //: Database //Extending for future improvement (Making it object oriented later)
        
        static string connStringFormat = "Data Source={0};Port={1};User ID={2};Password={3};Pooling={4}";
        public static string connString { get { return String.Format(connStringFormat, Server.MySQLHost, Server.MySQLPort, Server.MySQLUsername, Server.MySQLPassword, Server.DatabasePooling); } }
        internal static ParameterisedQuery query = new MySQLParameterisedQuery();

        public static void AddParams(string name, object param) { query.AddParam(name, param); }
        
        public static void ClearParams() { query.ClearParams(); }
        
        public const string TableExists = "SELECT * FROM information_schema.tables WHERE table_schema = @DB AND table_name = @Name";
    }
	
	public static class SQLite {
        
        static string connStringFormat = "Data Source =" + Server.apppath + "/MCGalaxy.db; Version =3; Pooling ={0}; Max Pool Size =300;";
        public static string connString { get { return String.Format(connStringFormat, Server.DatabasePooling); } }       
        internal static ParameterisedQuery query = new SQLiteParameterisedQuery();

        public static void AddParams(string name, object param) { query.AddParam(name, param); }
        
        public static void ClearParams() { query.ClearParams(); }
        
        public const string TableExists = "SELECT name FROM sqlite_master WHERE type='table' AND name=@Name";
    }
}