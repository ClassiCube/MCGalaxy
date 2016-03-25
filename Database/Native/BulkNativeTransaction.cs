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
using System.Runtime.InteropServices;

namespace MCGalaxy.SQL.Native {

    public partial class BulkNativeTransaction : BulkDatabaseTransaction {

        private BulkNativeTransaction(string connString) {
            Init(connString);
        }

        void Init(string connString) {
            connection = new NativeConnection();
            connection.ConnectionString = SQLite.connString;
            connection.Open();
            transaction = connection.BeginTransaction();
        }

        public static BulkDatabaseTransaction Create(string connString) {
            try {
                return new BulkNativeTransaction(connString);
            } catch (Exception ex) {
                Server.ErrorLog(ex);
                return null;
            }
        }

        public override bool Execute(string query) {
            throw new NotImplementedException();
        }
        
        public override IDbCommand CreateCommand(string query) {
            IDbCommand cmd = new NativeSQLiteCommand();
            cmd.CommandText = query;
            cmd.Connection = connection;
            cmd.Transaction = transaction;
            return cmd;
        }
        
        public override IDataParameter CreateParam(string paramName, DbType type) {
            IDataParameter param = new NativeParameter();
            param.ParameterName = paramName;
            param.DbType = type;
            return param;
        }
        
        [DllImport("sqlite3.dll")]
        static extern int sqlite3_open_v2(byte[] filename, out IntPtr db, int flags, IntPtr vfs);
        
        [DllImport("sqlite3.dll")]
        static extern int sqlite3_close_v2(IntPtr db);
    }
}