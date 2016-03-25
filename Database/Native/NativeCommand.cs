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

    sealed class NativeSQLiteCommand : IDbCommand {
        IntPtr statement;
        
        public IDbConnection Connection { get; set; }
        public IDbTransaction Transaction { get; set; }
        public string CommandText { get; set; }
        public int CommandTimeout { get; set; }
        public CommandType CommandType { get; set; }
        public IDataParameterCollection Parameters { get; private set; }
        public UpdateRowSource UpdatedRowSource { get; set; }
        
        public IDbDataParameter CreateParameter() { return null; }
        public void Cancel() { }
        public IDataReader ExecuteReader() { return ExecuteReader(CommandBehavior.Default); }
        public IDataReader ExecuteReader(CommandBehavior behavior) { return null; }
        public object ExecuteScalar() { return null; }
        
        public void Prepare() {
        }
        
        public int ExecuteNonQuery() {
            int code = sqlite3_step(statement);
            if (code > 0) throw new NativeException(code);
            code = sqlite3_reset(statement);
            if (code > 0) throw new NativeException(code);
            return 0;
        }
        
        public void Dispose() {
            int code = sqlite3_finalize(statement);
            if (code > 0) throw new NativeException(code);
        }
        
        [DllImport("sqlite3.dll")]
        static extern int sqlite3_finalize(IntPtr stmt);
        
        [DllImport("sqlite3.dll")]
        static extern int sqlite3_prepare_v2(IntPtr db, byte[] sql, int nBytes, out IntPtr stmt, out IntPtr sqlTail);
        
        [DllImport("sqlite3.dll")]
        static extern int sqlite3_reset(IntPtr stmt);
        
        [DllImport("sqlite3.dll")]
        static extern int sqlite3_step(IntPtr stmt);
    }
}