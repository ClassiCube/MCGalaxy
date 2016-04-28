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
        	return new NativeTransaction(this);
        }
        
        public void Close() {
            int code = Interop.sqlite3_close(DB);
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
        
        string path;
        public void Open() {
        	ParseConnectionString();
            byte[] filename = Interop.MakeUTF8(path);
            int code = Interop.sqlite3_open_v2(filename, out DB, 0x2, IntPtr.Zero);
            if (code > 0) throw new NativeException(code);
        }
        
        void ParseConnectionString() {
        	string[] args = ConnectionString.Split(';');
            foreach (string kvp in args) {
            	int sepIndex = kvp.IndexOf('=');
            	if (sepIndex < 0) continue;
            	string key = kvp.Substring(0, sepIndex).Trim();
            	string value = kvp.Substring(sepIndex + 1).Trim();

            	if (key == "Data Source") path = value;
            }
        }        
    }
}