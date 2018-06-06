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
using System.Data.Common;

namespace MCGalaxy.SQL {

    /// <summary> Callback function invoked on a row returned from an SQL query. </summary>
    public delegate void ReaderCallback(IDataReader reader);
    
    /// <summary> Represents an SQL command or query, that takes named parameters/arguments. </summary>
    public abstract class ParameterisedQuery {
        
        internal object[] parameters;
        protected abstract bool MultipleSchema { get; }
        
        protected abstract IDbConnection CreateConnection(string connString);        
        protected abstract IDbCommand CreateCommand(string query, IDbConnection conn);        
        protected abstract DbDataAdapter CreateDataAdapter(string query, IDbConnection conn);        
        protected abstract IDbDataParameter CreateParameter();
        
        
        /// <summary> Executes an SQL command that does not return any results. </summary>
        public void Execute(string query, string connString, bool createDB = false) {
            using (IDbConnection conn = CreateConnection(connString)) {
                conn.Open();
                if (!createDB && MultipleSchema)
                    conn.ChangeDatabase(ServerConfig.MySQLDatabaseName);
                
                using (IDbCommand cmd = CreateCommand(query, conn)) {
                    FillParams(cmd);
                    cmd.ExecuteNonQuery();
                }
                conn.Close();
            }
        }

        /// <summary> Excecutes an SQL query, returning all rows into a single DataTable. </summary>
        public void Fill(string query, string connString, DataTable toReturn) {
            using (IDbConnection conn = CreateConnection(connString)) {
                conn.Open();
                if (MultipleSchema)
                    conn.ChangeDatabase(ServerConfig.MySQLDatabaseName);
                
                using (DbDataAdapter da = CreateDataAdapter(query, conn)) {
                    FillParams(da.SelectCommand);
                    da.Fill(toReturn);
                    da.SelectCommand.Dispose();
                }
                conn.Close();
            }
        }

        /// <summary> Excecutes an SQL query, invoking a callback on the returned rows one by one. </summary>        
        public void ExecuteReader(string query, string connString, ReaderCallback callback) {
            using (IDbConnection conn = CreateConnection(connString)) {
                conn.Open();
                if (MultipleSchema)
                    conn.ChangeDatabase(ServerConfig.MySQLDatabaseName);
                
                using (IDbCommand cmd = CreateCommand(query, conn)) {
                    FillParams(cmd);
                    using (IDataReader reader = cmd.ExecuteReader()) {
                        while (reader.Read()) { callback(reader); }
                    }
                }
                conn.Close();
            }
        }
        
        void FillParams(IDbCommand cmd) {
            object[] args = parameters;
            if (args == null || args.Length == 0) return;
            
            string[] names = Database.GetParamNames(args.Length);
            for (int i = 0; i < args.Length; i++) {
                IDbDataParameter dbParam = CreateParameter();
                dbParam.ParameterName = names[i];
                dbParam.Value = args[i];
                cmd.Parameters.Add(dbParam);
            }
        }
    }
}
