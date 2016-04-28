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

    unsafe sealed class NativeReader {
        
        public void ReadColumns(NativeCommand cmd, DataTable results) {
            int cols = Interop.sqlite3_column_count(cmd.Statement);
            for (int i = 0; i < cols; i++) {
                IntPtr namePtr = Interop.sqlite3_column_name(cmd.Statement, i);
                string name = new String((sbyte*)namePtr);
                int code = Interop.sqlite3_column_type(cmd.Statement, i);
                
                Type type = typeof(object);
                if (code == 1) type = typeof(long);
                else if (code == 2) type = typeof(double);
                else if (code == 3) type = typeof(string);
                else if (code == 4) type = typeof(byte[]);
                results.Columns.Add(new DataColumn(name, type));
            }
        }
        
        public void ReadRows(NativeCommand cmd, DataTable results) {
            
        }
    }
}