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
// TODO: Finish native database backend
/*using System;
using System.Data;
using System.Runtime.InteropServices;
using System.Text;

namespace MCGalaxy.SQL.Native {

    unsafe sealed class NativeReader {
        
        int cols;
        public void ReadColumns(NativeCommand cmd, DataTable results) {
            results.Columns.Clear();
            cols = Interop.sqlite3_column_count(cmd.Statement);
            
            for (int i = 0; i < cols; i++) {
                IntPtr namePtr = Interop.sqlite3_column_name(cmd.Statement, i);
                string name = new String((sbyte*)namePtr);
                results.Columns.Add(new DataColumn(name));
            }
        }
        
        public void ReadRows(NativeCommand cmd, DataTable results) {
            while (true) {
                int code = Interop.sqlite3_step(cmd.Statement);
                if (code == Interop.RowReady) {
                    object[] values = new object[cols];
                    for (int i = 0; i < values.Length; i++)
                        values[i] = ParseValue(cmd.Statement, i);
                    
                    results.Rows.Add(values); continue;
                }
                if (code == Interop.Done) return;
                if (code > 0) throw new NativeException(code);
            }
        }
        
        unsafe object ParseValue(IntPtr stmt, int i) {
            int code = Interop.sqlite3_column_type(stmt, i);
            switch (code) {
                case 1: return Interop.sqlite3_column_int64(stmt, i);
                case 2: return Interop.sqlite3_column_double(stmt, i);
                case 3: return MakeString(stmt, i);
                case 4: return MakeBlob(stmt, i);
                case 5: return null;
            }
            throw new InvalidOperationException("Invalid type code: " + code);
        }
        
        string MakeString(IntPtr stmt, int i) {
            int count = Interop.sqlite3_column_bytes(stmt, i);
            if (count == 0) return "";
            
            byte* ptr = (byte*)Interop.sqlite3_column_text(stmt, i);        
            int charCount = Encoding.UTF8.GetCharCount(ptr, count);
            if (charCount == 0) return "";
            
            char* chars = stackalloc char[charCount];
            Encoding.UTF8.GetChars(ptr, count, chars, charCount);
            return new string(chars, 0, charCount);
        }
        
        byte[] MakeBlob(IntPtr stmt, int i) {
            int count = Interop.sqlite3_column_bytes(stmt, i);
            if (count == 0) return new byte[0];
            
            byte* ptr = (byte*)Interop.sqlite3_column_blob(stmt, i);
            byte[] dst = new byte[count];
            for (int j = 0; j < count; j++)
                dst[j] = ptr[j];
            return dst;
        }
    }
}*/