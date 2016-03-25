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
using System.Text;

namespace MCGalaxy.SQL.Native {
    
    sealed class NativeTransaction : IDbTransaction {
        public IDbConnection Connection { get; set; }
        public IsolationLevel IsolationLevel { get { return IsolationLevel.Unspecified; } }
        
        public NativeTransaction(IDbConnection connection) {
            Connection = connection;
            DoCommand("BEGIN");
        }
        
        public void Commit() { DoCommand("COMMIT"); }
        public void Rollback() { DoCommand("ROLLBACK"); }
        public void Dispose() { }
        
        void DoCommand(string query) {
            using (IDbCommand cmd = Connection.CreateCommand()) {
                cmd.CommandText = query;
                cmd.Prepare();
                cmd.ExecuteNonQuery();
            }
        }
    }
    
    sealed class NativeParameter : IDataParameter {	    
		public DbType DbType { get { return type; } set { type = value; } }
        public ParameterDirection Direction { get; set; }
        public bool IsNullable { get { return false; } }
        public string ParameterName { get; set; }
        public string SourceColumn { get; set; }
        public DataRowVersion SourceVersion { get; set; }
        public object Value { get; set; }
        
        public DbType type;
        public int Index = -1;
    }
    
    sealed class NativeParamsList : List<IDataParameter>, IDataParameterCollection {        

        public object this[string parameterName] {
            get { return this[IndexOf(parameterName)]; }
            set { this[IndexOf(parameterName)] = (IDbDataParameter)value; }
        }
        
        public bool Contains(string parameterName) {
            return IndexOf(parameterName) >= 0;
        }
        
        public int IndexOf(string parameterName) {
            for (int i = 0; i < Count; i++) {
                if (this[i].ParameterName == parameterName) return i;
            }
            return -1;
        }

        public void RemoveAt(string parameterName) {
            int index = IndexOf(parameterName);
            if (index >= 0) RemoveAt(index);
        }
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