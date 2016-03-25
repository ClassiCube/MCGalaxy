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

    sealed class NativeException : Exception {
        public readonly int ErrorCode;
        
        public NativeException(int errorCode) {
            ErrorCode = errorCode;
        }
               
        public override string Message { get { return ToString(); } }

        public override string ToString() {
            byte primaryCode = (byte)ErrorCode;
            string desc = errors[primaryCode] ?? "SQL error";
            return desc + " (" + ErrorCode + ")";
        }
        
        static string[] errors = new string[256];
        static NativeException() {
            errors[0] = "Successful result";
            errors[1] = "SQL error or missing database";
            errors[2] = "Internal logic error in SQLite";
            errors[3] = "Access permission denied";
            errors[4] = "Callback routine requested an abort";
            errors[5] = "The database file is locked";
            errors[6] = "A table in the database is locked";
            errors[7] = "A malloc() failed";
            errors[8] = "Attempt to write a readonly database";
            errors[9] = "Operation terminated by sqlite3_interrupt()";
            errors[10] = "Some kind of disk I/O error occurred";
            errors[11] = "The database disk image is malformed";
            errors[12] = "Unknown opcode in sqlite3_file_control()";
            errors[13] = "Insertion failed because database is full";
            errors[14] = "Unable to open the database file";
            errors[15] = "Database lock protocol error";
            errors[16] = "Database is empty";
            errors[17] = "The database schema changed";
            errors[18] = "String or BLOB exceeds size limit";
            errors[19] = "Abort due to constraint violation";
            errors[20] = "Data type mismatch";
            errors[21] = "Library used incorrectly";
            errors[22] = "Uses OS features not supported on host";
            errors[23] = "Authorization denied";
            errors[24] = "Auxiliary database format error";
            errors[25] = "2nd parameter to sqlite3_bind out of range";
            errors[26] = "File opened that is not a database file";
            errors[27] = "Notifications from sqlite3_log()";
            errors[28] = "Warnings from sqlite3_log()";
            errors[100] = "sqlite3_step() has another row ready";
            errors[101] = "sqlite3_step() has finished executing";
        }
    }
}