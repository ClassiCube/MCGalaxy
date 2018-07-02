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
using System.Data;

namespace MCGalaxy.SQL {
    
    /// <summary> Executes an SQL command or query, that takes named parameters/arguments. </summary>
    public static class SqlQuery {

        /// <summary> Executes an SQL command that does not return any results. </summary>
        public static void Execute(string sql, object[] parameters, bool createDB) {
        	IDatabaseBackend db = Database.Backend;
        	using (IDbConnection conn = db.CreateConnection()) {
                conn.Open();
                if (!createDB && db.MultipleSchema)
                    conn.ChangeDatabase(ServerConfig.MySQLDatabaseName);
                
                using (IDbCommand cmd = db.CreateCommand(sql, conn)) {
                    FillParams(cmd, parameters);
                    cmd.ExecuteNonQuery();
                }
                conn.Close();
            }
        }

        /// <summary> Excecutes an SQL query, invoking a callback on the returned rows one by one. </summary>        
        public static object Iterate(string sql, object[] parameters, object arg, ReaderCallback callback) {
        	IDatabaseBackend db = Database.Backend;
            using (IDbConnection conn = db.CreateConnection()) {
                conn.Open();
                if (db.MultipleSchema)
                    conn.ChangeDatabase(ServerConfig.MySQLDatabaseName);
                
                using (IDbCommand cmd = db.CreateCommand(sql, conn)) {
                    FillParams(cmd, parameters);
                    using (IDataReader reader = cmd.ExecuteReader()) {
                        while (reader.Read()) { arg = callback(reader, arg); }
                    }
                }
                conn.Close();
            }
            return arg;
        }
        
        
        internal static void FillParams(IDbCommand cmd, object[] parameters) {
            if (parameters == null || parameters.Length == 0) return;
            IDatabaseBackend db = Database.Backend;
            
            string[] names = GetNames(parameters.Length);
            for (int i = 0; i < parameters.Length; i++) {
                IDbDataParameter arg = db.CreateParameter();
                arg.ParameterName = names[i];
                arg.Value = parameters[i];
                cmd.Parameters.Add(arg);
            }
        }
        
        volatile static string[] ids;
        internal static string[] GetNames(int count) {
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
