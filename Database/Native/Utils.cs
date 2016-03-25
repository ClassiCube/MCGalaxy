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
using System.Text;

namespace MCGalaxy.SQL.Native {

    sealed class NativeConnection : IDbConnection {
        public string ConnectionString { get; set; }
        public int ConnectionTimeout { get { return 0; } }
        public string Database { get { return ""; } }
        public ConnectionState State { get { return ConnectionState.Open; } }
        
        public IDbTransaction BeginTransaction() { return BeginTransaction(IsolationLevel.Unspecified); }
        public IDbTransaction BeginTransaction(IsolationLevel il) { return null; }
        public void Close() { }
        public void ChangeDatabase(string databaseName) { }
        public IDbCommand CreateCommand() { return null; }
        public void Open() { }
        public void Dispose() { }
    }
    
    sealed class NativeTransaction : IDbTransaction {
        public IDbConnection Connection { get; set; }
        public IsolationLevel IsolationLevel { get { return IsolationLevel.Unspecified; } }
        public void Commit() { }
        public void Rollback() { }
        public void Dispose() { }
    }
    
    sealed class NativeParameter : IDataParameter {
        public DbType DbType { get; set; }
        public ParameterDirection Direction { get; set; }
        public bool IsNullable { get { return false; } }
        public string ParameterName { get; set; }
        public string SourceColumn { get; set; }
        public DataRowVersion SourceVersion { get; set; }
        public object Value { get; set; }
    }
    
    static class NativeUtils {
        static Encoding encoding = Encoding.UTF8;
        
        public static byte[] MakeUTF8(string input) {
            int count = encoding.GetByteCount(input) + 1; // null terminator
            byte[] chars = new byte[count];
            encoding.GetBytes(input, 0, input.Length, chars, 0);
            return chars;
        }
    }
}