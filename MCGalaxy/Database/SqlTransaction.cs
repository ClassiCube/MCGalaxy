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
    
    public sealed class SqlTransaction : IDisposable {
        internal ISqlConnection conn;
        internal ISqlTransaction transaction;
        
        public SqlTransaction() {
            IDatabaseBackend db = Database.Backend;
            conn = db.CreateConnection();
            conn.Open();
            
            if (db.MultipleSchema) conn.ChangeDatabase(Server.Config.MySQLDatabaseName);
            transaction = conn.BeginTransaction();
        }

        public void Commit() {
            try {
                transaction.Commit();
            } catch (Exception ex) {
                Logger.LogError("Error committing SQL transaction", ex);
                Rollback();
            } finally {
                conn.Close();
            }
        }
        
        public bool Rollback() {
            try {
                transaction.Rollback();
                return true;
            } catch (Exception ex) {
                Logger.LogError("Error rolling back SQL transaction", ex);
                return false;
            }
        }
        
        public void Dispose() {
            transaction.Dispose();
            conn.Dispose();
            transaction = null;
            conn = null;
        }
        
        public bool Execute(string sql, params object[] args) {
            try {
                using (ISqlCommand cmd = conn.CreateCommand(sql)) {
        			cmd.Associate(transaction);
                    SqlQuery.FillParams(cmd, args);
                    cmd.ExecuteNonQuery();
                    return true;
                }
            } catch (Exception ex) {
                Logger.LogError("Error executing SQL transaction: " + sql, ex);
                return false;
            }
        }
    }
}
