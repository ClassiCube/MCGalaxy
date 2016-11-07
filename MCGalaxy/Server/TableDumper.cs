/*
    Copyright 2011 MCForge
        
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
using System.IO;
using MCGalaxy.SQL;

namespace MCGalaxy {
    public sealed class TableDumper {
        
        bool gottenRows;
        string table, insertCols;
        internal StreamWriter sql;
        bool[] rowTypes;
        
        public void DumpTable(StreamWriter sql, string table) {
            gottenRows = false;
            this.sql = sql;
            this.table = table;
            Database.ExecuteReader("SELECT * FROM `" + table + "`", DumpRow);
            
            if (!gottenRows) {
                sql.WriteLine("-- No data in table `{0}`!", table);
                sql.WriteLine();
            }
        }
        
        void MakeInsertFormat(IDataReader reader) {
            sql.WriteLine("--");
            sql.WriteLine("-- Dumping data for table `{0}`", table);
            sql.WriteLine("--");
            sql.WriteLine();

            string[] colNames = new string[reader.FieldCount];
            rowTypes = new bool[reader.FieldCount];
            for (int i = 0; i < reader.FieldCount; i++) {
                colNames[i] = reader.GetName(i);
                Type type = reader.GetFieldType(i);
                rowTypes[i] = type == typeof(string) || type == typeof(DateTime);
            }
            insertCols = FormatInsertColumns(colNames, table);
            gottenRows = true;
        }
        
        void DumpRow(IDataReader reader) {
            if (!gottenRows) MakeInsertFormat(reader);
            sql.WriteLine();
            sql.WriteLine(insertCols);
            // TODO: MySQL and SQLite have different date formats :/
            // TODO: Try to use GetInt32 when possible

            for (int col = 0; col < rowTypes.Length; col++) {
                //The values themselves can be integers or strings, or null
                if (reader.IsDBNull(col)) {
                    sql.Write("NULL");
                } else if (rowTypes[col]) {
                    string value = reader.GetString(col);
                    if (value.IndexOf('\'') >= 0) // escape '
                        value = value.Replace("'", "''");
                    sql.Write("'" + value + "'");
                } else {
                    long value = reader.GetInt64(col);
                    sql.Write(value);
                }
                sql.Write((col < rowTypes.Length - 1 ? ", " : ");"));
            }
            sql.WriteLine();
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