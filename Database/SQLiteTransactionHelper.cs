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
using System.Data.Common;
using System.Data.SQLite;

namespace MCGalaxy.SQL {

    public sealed class SQLiteTransactionHelper : DatabaseTransactionHelper {

        private SQLiteTransactionHelper() {
            Init(SQLite.connString);
        }

        private SQLiteTransactionHelper(string connString) {
            Init(connString);
        }

        void Init(string connString) {
            connection = new SQLiteConnection(connString);
            connection.Open();
            //connection.ChangeDatabase(Server.MySQLDatabaseName);
            transaction = connection.BeginTransaction();
        }

        public static DatabaseTransactionHelper Create() {
            return Create(SQLite.connString);
        }

        public static DatabaseTransactionHelper Create(string connString) {
            try {
                return new SQLiteTransactionHelper(connString);
            } catch (Exception ex) {
                Server.ErrorLog(ex);
                return null;
            }
        }

        public override bool Execute(string query) {
            try {
                using (SQLiteCommand cmd = new SQLiteCommand(
                    query, (SQLiteConnection)connection, (SQLiteTransaction)transaction)) {
                    cmd.ExecuteNonQuery();
                }
            } catch (Exception e) {
                System.IO.File.AppendAllText("MySQL_error.log", DateTime.Now + " " + query + "\r\n");
                Server.ErrorLog(e);
                return false;
            }
            return true;
        }
		
        public override IDbCommand CreateCommand(string query) {
            return new SQLiteCommand(query, (SQLiteConnection)connection, (SQLiteTransaction)transaction);
		}
		
		public override DbParameter CreateParam(string paramName, DbType type) {
			return new SQLiteParameter(paramName, type);
		}
    }
}