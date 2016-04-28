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

namespace MCGalaxy.SQL {
    
    public abstract class BulkTransaction : IDisposable {
        protected IDbConnection connection;
        protected IDbTransaction transaction;

        public static BulkTransaction Create() {
            if (Server.useMySQL) return MySQLBulkTransaction.Create(MySQL.connString);
            else return SQLiteBulkTransaction.Create(SQLite.connString);
        }

        public abstract IDbCommand CreateCommand(string query);
        
        public abstract IDataParameter CreateParam(string paramName, DbType type);

        public void Commit() {
            try {
                transaction.Commit();
            } catch (Exception ex) {
                Server.ErrorLog(ex);
                Rollback();
            } finally {
                connection.Close();
            }
        }
        
        public bool Rollback() {
            try {
                transaction.Rollback();
                return true;
            } catch (Exception ex) {
                Server.ErrorLog(ex);
                return false;
            }
        }
        
        public void Dispose() {
            transaction.Dispose();
            connection.Dispose();
            transaction = null;
            connection = null;
        }
        
        public bool Execute(string query) {
            using (IDbCommand cmd = CreateCommand(query))
                return Execute(query, cmd);
        }
        
        public static bool Execute(string query, IDbCommand cmd) {
            try {
                cmd.ExecuteNonQuery();
            } catch (Exception e) {
                System.IO.File.AppendAllText("MySQL_error.log", DateTime.Now + " " + query + "\r\n");
                Server.ErrorLog(e);
                return false;
            }
            return true;
        }
    }
}
