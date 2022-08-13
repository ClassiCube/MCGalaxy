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
using System.IO;

namespace MCGalaxy.SQL 
{
    public sealed class TableDumper 
    {
        bool gottenRows;
        string table, insertCols;
        internal StreamWriter sql;
        string[] colNames;
        
        public void DumpTable(StreamWriter sql, string table) {
            gottenRows = false;
            this.sql   = sql;
            this.table = table;
            Database.ReadRows(table, "*", DumpRow);
            
            if (!gottenRows) {
                sql.WriteLine("-- No data in table `{0}`!", table);
                sql.WriteLine();
            }
        }
        
        void MakeInsertFormat(ISqlRecord record) {
            sql.WriteLine("--");
            sql.WriteLine("-- Dumping data for table `{0}`", table);
            sql.WriteLine("--");
            sql.WriteLine();

            colNames = new string[record.FieldCount];
            for (int i = 0; i < record.FieldCount; i++) 
            {
                colNames[i] = record.GetName(i);
            }
            insertCols = FormatInsertColumns(table);
            gottenRows = true;
        }
        
        void DumpRow(ISqlRecord record) {
            if (!gottenRows) MakeInsertFormat(record);
            sql.WriteLine(insertCols);

            //The values themselves can be integers or strings, or null
            for (int col = 0; col < colNames.Length; col++) 
            {
                sql.Write(record.DumpValue(col));
                sql.Write((col < colNames.Length - 1 ? ", " : ");"));
            }
            
            sql.WriteLine();
        }
        
        string FormatInsertColumns(string name) {
            string sql    = "INSERT INTO `" + name + "` (`";
            string[] cols = colNames;

            for (int i = 0; i < cols.Length; i++) 
            {
                sql += cols[i] + "`";
                if (i < cols.Length - 1) sql += ", `";
                else sql += ") VALUES (";
            }
            return sql;
        }
    }
}