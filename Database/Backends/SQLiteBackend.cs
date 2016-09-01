/*
    Copyright 2011 MCForge
    
    Dual-licensed under the Educational Community License, Version 2.0 and
    the GNU General Public License, Version 3 (the "Licenses"); you may
    not use this file except in compliance with the Licenses. You may
    obtain a copy of the Licenses at
    
    http://www.osedu.org/licenses/ECL-2.0
    http://www.gnu.org/licenses/gpl-3.0.html
    
    Unless required by applicable law or agreed to in writing,
    software distributed under the Licenses are distributed on an "AS IS"
    BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express
    or implied. See the Licenses for the specific language governing
    permissions and limitations under the Licenses.
 */
using System;
using System.Data;
using System.Data.SQLite;

namespace MCGalaxy.SQL {

	public abstract class SQLiteBackend : IDatabaseBackend {

        static string connStringFormat = "Data Source =" + Server.apppath + "/MCGalaxy.db; Version =3; Pooling ={0}; Max Pool Size =300;";        
        public override string ConnectionString {
            get { return String.Format(connStringFormat, Server.DatabasePooling); }
        }
        
        public override BulkTransaction CreateBulk() {
            return new SQLiteBulkTransaction(ConnectionString);
        }
        
        public override ParameterisedQuery CreateParamterised() {
            return new SQLiteParameterisedQuery();
        }
    }
    
     public sealed class SQLiteBulkTransaction : BulkTransaction {

        public SQLiteBulkTransaction(string connString) { 
            connection = new SQLiteConnection(connString);
            connection.Open();
            transaction = connection.BeginTransaction();
        }
        
        public override IDbCommand CreateCommand(string query) {
            return new SQLiteCommand(query, (SQLiteConnection)connection, (SQLiteTransaction)transaction);
        }
        
        public override IDataParameter CreateParam(string paramName, DbType type) {
            return new SQLiteParameter(paramName, type);
        }
    }
    
    public sealed class SQLiteParameterisedQuery : ParameterisedQuery {

        public override void Execute(string query, bool createDB = false) {
            using (var conn = new SQLiteConnection(SQLite.connString)) {
                conn.Open();
                using (SQLiteCommand cmd = new SQLiteCommand(query, conn)) {
                    foreach (var param in parameters)
                        cmd.Parameters.AddWithValue(param.Key, param.Value);
                    cmd.ExecuteNonQuery();
                }
                conn.Close();
            }
        }

        public override void Fill(string query, DataTable results) {
            using (var conn = new SQLiteConnection(SQLite.connString)) {
                conn.Open();
    
                using (SQLiteDataAdapter da = new SQLiteDataAdapter(query, conn)) {
                    foreach (var param in parameters)
                        da.SelectCommand.Parameters.AddWithValue(param.Key, param.Value);
                    da.Fill(results);
                    da.SelectCommand.Dispose();
                }
                conn.Close();
            }
        }
    }
}
