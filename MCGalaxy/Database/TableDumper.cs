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
using System.IO;

namespace MCGalaxy.SQL {
    public sealed class TableDumper {
        
        bool gottenRows;
        string table, insertCols;
        internal StreamWriter sql;
        Type[] colTypes;
        
        public void DumpTable(StreamWriter sql, string table) {
            gottenRows = false;
            this.sql = sql;
            this.table = table;
            Database.ReadRows(table, "*", null, DumpRow);
            
            if (!gottenRows) {
                sql.WriteLine("-- No data in table `{0}`!", table);
                sql.WriteLine();
            }
        }
        
        void MakeInsertFormat(IDataRecord record) {
            sql.WriteLine("--");
            sql.WriteLine("-- Dumping data for table `{0}`", table);
            sql.WriteLine("--");
            sql.WriteLine();

            string[] colNames = new string[record.FieldCount];
            colTypes = new Type[record.FieldCount];
            for (int i = 0; i < record.FieldCount; i++) {
                colNames[i] = record.GetName(i);
                colTypes[i] = record.GetFieldType(i);
            }
            insertCols = FormatInsertColumns(colNames, table);
            gottenRows = true;
        }
        
        object DumpRow(IDataRecord record, object arg) {
            if (!gottenRows) MakeInsertFormat(record);
            sql.WriteLine(insertCols);

            //The values themselves can be integers or strings, or null
            for (int col = 0; col < colTypes.Length; col++) {
                if (record.IsDBNull(col)) {
                    sql.Write("NULL");
                } else if (colTypes[col] == typeof(string)) {
                    string value = record.GetString(col);
                    if (value.IndexOf('\'') >= 0) // escape '
                        value = value.Replace("'", "''");
                    sql.Write("'" + value + "'");
                } else if (colTypes[col] == typeof(DateTime)) {
                    string date = Database.Backend.RawGetDateTime(record, col);
                    sql.Write("'" + date + "'");
                } else {
                    long value = record.GetInt64(col); // TODO: try to use GetInt32 where possible
                    sql.Write(value);
                }
                sql.Write((col < colTypes.Length - 1 ? ", " : ");"));
            }
            
            sql.WriteLine();
            return arg;
        }
        
        static string FormatInsertColumns(string[] cols, string name) {
            string sql = "INSERT INTO `" + name + "` (`";
            for (int i = 0; i < cols.Length; i++) {
                sql += cols[i] + "`";
                if (i < cols.Length - 1) sql += ", `";
                else sql += ") VALUES (";
            }
            return sql;
        }
    }
}