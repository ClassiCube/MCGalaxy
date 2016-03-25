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
using System.Text;

namespace MCGalaxy.SQL.Native {

    sealed class NativeConnection : IDbConnection {
        public IntPtr DB;
        
        public string ConnectionString { get; set; }
        public int ConnectionTimeout { get { return 0; } }
        public string Database { get { return ""; } }
        public ConnectionState State { get { return ConnectionState.Open; } }       
        public IDbTransaction BeginTransaction() { return BeginTransaction(IsolationLevel.Unspecified); }
        public void ChangeDatabase(string databaseName) { }
        
        public IDbTransaction BeginTransaction(IsolationLevel il) { 
            return null;
        }
        
        public void Close() {
            int code = sqlite3_close_v2(DB);
            DB = IntPtr.Zero;
            if (code > 0) throw new NativeException(code);
        }
        
        public IDbCommand CreateCommand() { 
            IDbCommand cmd = new NativeCommand();
            cmd.Connection = this;
            return cmd;
        }
        
        public void Dispose() {
            if (DB == IntPtr.Zero) return;
            Close();
        }
        
        public void Open() {
            string[] args = ConnectionString.Split(';');
            byte[] filename = NativeUtils.MakeUTF8(args[0]);
            int code = sqlite3_open_v2(filename, out DB, 0, IntPtr.Zero);
            if (code > 0) throw new NativeException(code);
        }
        
        [DllImport("sqlite3.dll")]
        static extern int sqlite3_open_v2(byte[] filename, out IntPtr db, int flags, IntPtr vfs);
        
        [DllImport("sqlite3.dll")]
        static extern int sqlite3_close_v2(IntPtr db);
    }
}