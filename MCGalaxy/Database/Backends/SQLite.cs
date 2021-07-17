﻿/*
    Copyright 2015 MCGalaxy
    
    Dual-licensed under the Educational Community License, Version 2.0 and
    the GNU General Public License, Version 3 (the "Licenses"); you may
    not use this file except in compliance with the Licenses. You may
    obtain a copy of the Licenses at
    
    http://www.osedu.org/licenses/ECL-2.0
    http://www.gnu.org/licenses/gpl-3.0.html
    
    Unless required by applicable law or agreed to in writing,
    software distributed under the Licenses are distributed on an "AS IS"
    BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express
    or implied. See the Licenses for the specific language governing
    permissions and limitations under the Licenses.
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MCGalaxy.SQL {

    public sealed class SQLiteBackend : IDatabaseBackend 
    {
        public static IDatabaseBackend Instance = new SQLiteBackend();
        public SQLiteBackend() {
            CaselessWhereSuffix = " COLLATE NOCASE";
            CaselessLikeSuffix  = " COLLATE NOCASE";
        }
        
        public override bool EnforcesTextLength { get { return false; } }
        public override bool MultipleSchema { get { return false; } }
        
        internal override ISqlConnection CreateConnection() {
            return new MCGSQLiteConnection();
        }        
        
        public override void CreateDatabase() { }
        
        public override string RawGetDateTime(ISqlRecord record, int col) {
            return record.GetString(col); // reader.GetDateTime is extremely slow so avoid it
        }
        
        public override bool TableExists(string table) {
            return Database.CountRows("sqlite_master",
                                      "WHERE type='table' AND name=@0", table) > 0;
        }
        
        public override List<string> AllTables() {
            const string sql = "SELECT name from sqlite_master WHERE type='table'";
            List<string> tables = GetStrings(sql);
            
            // exclude sqlite built-in database tables
            for (int i = tables.Count - 1; i >= 0; i--) {
                if (tables[i].StartsWith("sqlite_")) tables.RemoveAt(i);
            }
            return tables;
        }
        
        static object IterateColumnNames(ISqlRecord record, object arg) {
            List<string> columns = (List<string>)arg;
            columns.Add(record.GetText("name"));
            return arg;
        }
        
        public override List<string> ColumnNames(string table) {
            Database.ValidateName(table);
            List<string> columns = new List<string>();
            
            Database.Iterate("PRAGMA table_info(`" + table + "`)",
                             columns, IterateColumnNames, null);
            return columns;
        }
        
        public override string RenameTableSql(string srcTable, string dstTable) {
            return "ALTER TABLE `" + srcTable + "` RENAME TO `" + dstTable + "`";
        }
        
        protected override void CreateTableColumns(StringBuilder sql, ColumnDesc[] columns) {
            string priKey = null;
            for (int i = 0; i < columns.Length; i++) {
                ColumnDesc col = columns[i];
                sql.Append(col.Column).Append(' ');
                sql.Append(col.FormatType());
                
                // When the primary key isn't autoincrement, we use the same form as mysql
                // Otherwise we have to use sqlite's 'PRIMARY KEY AUTO_INCREMENT' form
                if (col.PrimaryKey) {
                    if (!col.AutoIncrement) priKey = col.Column;
                    else sql.Append(" PRIMARY KEY");
                }
                if (col.AutoIncrement) sql.Append(" AUTOINCREMENT");
                if (col.NotNull) sql.Append(" NOT NULL");
                
                if (i < columns.Length - 1) {
                    sql.Append(',');
                } else if (priKey != null) {
                    sql.Append(", PRIMARY KEY(").Append(priKey).Append(") ");
                }
                sql.AppendLine();
            }
        }
        
        public override void PrintSchema(string table, TextWriter w) {
            string sql = "SELECT sql from sqlite_master WHERE tbl_name = @0 AND type = 'table'";
            List<string> all = GetStrings(sql + CaselessWhereSuffix, table);
            
            for (int i = 0; i < all.Count; i++) {
                sql = all[i].Replace(" " + table, " `" + table + "`");
                sql = sql.Replace("CREATE TABLE `" + table + "`", "CREATE TABLE IF NOT EXISTS `" + table + "`");
                w.WriteLine(sql + ";");
            }
        }
        
        public override string AddColumnSql(string table, ColumnDesc col, string colAfter) {
            return "ALTER TABLE `" + table + "` ADD COLUMN " + col.Column + " " + col.FormatType();
        }
        
        public override string AddOrReplaceRowSql(string table, string columns, object[] args) {
            return InsertSql("INSERT OR REPLACE INTO", table, columns, args);
        }
    }
    
    public sealed class MCGSQLiteConnection : SQLiteConnection 
    {
        protected override bool ConnectionPooling { get { return Server.Config.DatabasePooling; } }
        protected override string DBPath { get { return "MCGalaxy.db"; } }
    }
}
