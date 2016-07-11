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
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Runtime.InteropServices;
using System.Security;
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
    
    internal unsafe sealed class NativeParameter : IDataParameter {        
        public DbType DbType { get { return type; } set { type = value; } }
        public ParameterDirection Direction { get; set; }
        public bool IsNullable { get { return false; } }
        public string ParameterName { get; set; }
        public string SourceColumn { get; set; }
        public DataRowVersion SourceVersion { get; set; }
        public object Value { get; set; }
        
        public DbType type;
        public int Index = -1;
        // Avoid boxing primitive types
        public int I32Value;
        public ushort U16Value;
        public byte U8Value;
        public bool BoolValue;
        public byte* StringPtr;
        public int StringCount;
        
        public void SetString(string value) {
            if ((value.Length + 1) > StringCount) {
                if (StringCount > 0) 
                    Marshal.FreeHGlobal((IntPtr)StringPtr);
                StringCount = value.Length + 1;
                StringPtr = (byte*)Marshal.AllocHGlobal(StringCount);
            }
            
            // Reset all bytes to 0, then replace them.
            for (int i = 0; i < StringCount; i++)
                StringPtr[i] = 0;
            for (int i = 0; i < value.Length; i++)
                StringPtr[i] = (byte)value[i];
        }
        
        public void Dispose() {
            if (StringCount != 0) return;
            Marshal.FreeHGlobal((IntPtr)StringPtr);
            StringCount = 0;
        }
    }
    
    unsafe static class Interop {
        static Encoding encoding = Encoding.UTF8;
        const CallingConvention conv = CallingConvention.Cdecl;
        
        public const int Busy = 5, Locked = 6;
        public const int RowReady = 100, Done = 101;
        
        public static byte[] MakeUTF8(string input) {
            int count = encoding.GetByteCount(input) + 1; // null terminator
            byte[] chars = new byte[count];
            encoding.GetBytes(input, 0, input.Length, chars, 0);
            return chars;
        }
        
        [DllImport("sqlite3", CallingConvention = conv), SuppressUnmanagedCodeSecurity]
        public static extern int sqlite3_open_v2(byte[] filename, out IntPtr db, int flags, IntPtr vfs);
        
        [DllImport("sqlite3", CallingConvention = conv), SuppressUnmanagedCodeSecurity]
        public static extern int sqlite3_close(IntPtr db);
        
        
        [DllImport("sqlite3", CallingConvention = conv), SuppressUnmanagedCodeSecurity]
        public static extern int sqlite3_bind_int(IntPtr stmt, int index, int value);
        
        [DllImport("sqlite3", CallingConvention = conv), SuppressUnmanagedCodeSecurity]
        public static extern int sqlite3_bind_parameter_index(IntPtr stmt, byte[] name);

        [DllImport("sqlite3", CallingConvention = conv), SuppressUnmanagedCodeSecurity]
        public static extern int sqlite3_bind_text(IntPtr stmt, int index, byte* text, int textLen, IntPtr reserved);
        
        [DllImport("sqlite3", CallingConvention = conv), SuppressUnmanagedCodeSecurity]
        public static extern int sqlite3_finalize(IntPtr stmt);
        
        [DllImport("sqlite3", CallingConvention = conv), SuppressUnmanagedCodeSecurity]
        public static extern int sqlite3_prepare_v2(IntPtr db, byte[] sql, int nBytes, out IntPtr stmt, out IntPtr sqlTail);
        
        [DllImport("sqlite3", CallingConvention = conv), SuppressUnmanagedCodeSecurity]
        public static extern int sqlite3_reset(IntPtr stmt);
        
        [DllImport("sqlite3", CallingConvention = conv), SuppressUnmanagedCodeSecurity]
        public static extern int sqlite3_step(IntPtr stmt);

        [DllImport("sqlite3", CallingConvention = conv), SuppressUnmanagedCodeSecurity]
        public static extern IntPtr sqlite3_next_stmt(IntPtr db, IntPtr stmt);
        

        [DllImport("sqlite3", CallingConvention = conv), SuppressUnmanagedCodeSecurity]
        public static extern int sqlite3_column_count(IntPtr stmt);

        [DllImport("sqlite3", CallingConvention = conv), SuppressUnmanagedCodeSecurity]
        public static extern int sqlite3_column_type(IntPtr stmt, int iCol);
        
        [DllImport("sqlite3", CallingConvention = conv), SuppressUnmanagedCodeSecurity] 
        public static extern IntPtr sqlite3_column_name(IntPtr stmt, int iCol);
        

        [DllImport("sqlite3", CallingConvention = conv), SuppressUnmanagedCodeSecurity]      
        public static extern double sqlite3_column_double(IntPtr stmt, int iCol);

        [DllImport("sqlite3", CallingConvention = conv), SuppressUnmanagedCodeSecurity]
        public static extern int sqlite3_column_int(IntPtr stmt, int iCol);

        [DllImport("sqlite3", CallingConvention = conv), SuppressUnmanagedCodeSecurity]
        public static extern long sqlite3_column_int64(IntPtr stmt, int iCol);

        [DllImport("sqlite3", CallingConvention = conv), SuppressUnmanagedCodeSecurity]
        public static extern IntPtr sqlite3_column_text(IntPtr stmt, int iCol);
        
        [DllImport("sqlite3", CallingConvention = conv), SuppressUnmanagedCodeSecurity]
        public static extern IntPtr sqlite3_column_blob(IntPtr stmt, int iCol);
        
        [DllImport("sqlite3", CallingConvention = conv), SuppressUnmanagedCodeSecurity]
        public static extern int sqlite3_column_bytes(IntPtr stmt, int iCol);        
    }
}