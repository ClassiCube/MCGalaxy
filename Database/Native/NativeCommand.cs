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
using System.Runtime.InteropServices;

namespace MCGalaxy.SQL.Native {

    unsafe sealed class NativeCommand : IDbCommand {
        public IntPtr Statement;
        NativeParamsList args = new NativeParamsList();

        public IDbConnection Connection { get; set; }
        public IDbTransaction Transaction { get; set; }
        public string CommandText { get; set; }
        public int CommandTimeout { get; set; }
        public CommandType CommandType { get; set; }
        public IDataParameterCollection Parameters { get { return args; } }
        public UpdateRowSource UpdatedRowSource { get; set; }
        public IDbDataParameter CreateParameter() { return null; }
        public void Cancel() { }
        public IDataReader ExecuteReader() { return ExecuteReader(CommandBehavior.Default); }
        public IDataReader ExecuteReader(CommandBehavior behavior) { return null; }
        public object ExecuteScalar() { return null; }
        
        public void Prepare() {
            byte[] sql = NativeUtils.MakeUTF8(CommandText);
            IntPtr db = ((NativeConnection)Connection).DB;
            IntPtr tail;
            int code = NativeUtils.sqlite3_prepare_v2(db, sql, sql.Length, out Statement, out tail);
            if (code > 0) throw new NativeException(code);
        }
        
        public int ExecuteNonQuery() {
            foreach (IDataParameter param in args)
                BindParam(param);
            
            int code = NativeUtils.sqlite3_step(Statement);
            if (code > 0 && code != 101) throw new NativeException(code);
            code = NativeUtils.sqlite3_reset(Statement);
            if (code > 0) throw new NativeException(code);
            return 0;
        }
        
        public void Dispose() {
            int code = NativeUtils.sqlite3_finalize(Statement);
            if (code > 0) throw new NativeException(code);
        }
        
        void BindParam(IDataParameter param) {
            NativeParameter nParam = (NativeParameter)param;
            if (nParam.Index == -1) BindIndex(nParam);
            
            int code = 0;
            switch (nParam.type) {
                case DbType.AnsiStringFixedLength:
                    code = NativeUtils.sqlite3_bind_text(Statement, nParam.Index, nParam.StringPtr, 
                                             nParam.StringCount - 1, IntPtr.Zero);
                    break;
                case DbType.UInt16:
                    code = NativeUtils.sqlite3_bind_int(Statement, nParam.Index, nParam.U16Value);
                    break;                    
                case DbType.Byte:
                    code = NativeUtils.sqlite3_bind_int(Statement, nParam.Index, nParam.U8Value);
                    break;
                case DbType.Boolean:
                    code = NativeUtils.sqlite3_bind_int(Statement, nParam.Index, nParam.BoolValue ? 1 : 0);
                    break;
            }
            if (code > 0) throw new NativeException(code);
        }
        
        void BindIndex(NativeParameter nParam) {
            byte[] name = NativeUtils.MakeUTF8(nParam.ParameterName);
            nParam.Index = NativeUtils.sqlite3_bind_parameter_index(Statement, name);
        }
    }
}