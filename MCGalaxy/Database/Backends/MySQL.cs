/*
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
using System.Data;
using System.IO;
using System.Text;
using MySql.Data.MySqlClient;

namespace MCGalaxy.SQL {

    public sealed class MySQLBackend : IDatabaseBackend {
        public static IDatabaseBackend Instance = new MySQLBackend();
        public MySQLBackend() {
            CaselessWhereSuffix = " COLLATE utf8_general_ci";
            CaselessLikeSuffix = "";
        }
        
        public override bool EnforcesTextLength { get { return true; } }
        public override bool MultipleSchema { get { return true; } }     
        
        internal override IDbConnection CreateConnection() {
            const string format = "Data Source={0};Port={1};User ID={2};Password={3};Pooling={4};Treat Tiny As Boolean=false;";
            string str = string.Format(format, ServerConfig.MySQLHost, ServerConfig.MySQLPort,
                                       ServerConfig.MySQLUsername, ServerConfig.MySQLPassword, ServerConfig.DatabasePooling);
            return new MySqlConnection(str);
        }
        
        internal override IDbCommand CreateCommand(string sql, IDbConnection conn) {
            return new MySqlCommand(sql, (MySqlConnection)conn);
        }
        
        internal override IDbDataParameter CreateParameter() {
            return new MySqlParameter();
        }

        
        public override void CreateDatabase() {
            string sql = "CREATE DATABASE if not exists `" + ServerConfig.MySQLDatabaseName + "`";
            Database.Do(sql, true, null, null);
        }
        
        public override string RawGetDateTime(IDataRecord record, int col) {
            DateTime date = record.GetDateTime(col);
            return date.ToString(Database.DateFormat);
        }
        
        protected internal override void ParseCreate(ref string cmd) {
            // MySQL does not support the format used by the SQLite backend for the primary key
            const string priKey = " PRIMARY KEY AUTOINCREMENT";
            int priIndex = cmd.ToUpper().IndexOf(priKey);
            if (priIndex == -1) return;
            
            // Find the name of this column
            char[] sepChars = new char[] { '\t', ' ' }; // chars that separate part of a column definition
            char[] startChars = new char[] { '`', '(', ' ', ',', '\t' }; // chars that can start a column definition
            string before = cmd.Substring(0, priIndex);
            before = before.Substring(0, before.LastIndexOfAny(sepChars)); // get rid of column type
            int nameStart = before.LastIndexOfAny(startChars) + 1;
            string name = before.Substring(nameStart);
            
            // Replace the 'PRIMARY KEY AUTOINCREMENT' with just 'AUTO_INCREMENT';
            cmd = cmd.Remove(priIndex, priKey.Length);
            cmd = cmd.Insert(priIndex, " AUTO_INCREMENT");
            // Insert 'PRIMARY KEY' at end of columns definition
            cmd = cmd.Insert(cmd.LastIndexOf(")"), ", PRIMARY KEY (`" + name + "`)");
        }
        

        static object IterateExists(IDataRecord record, object arg) { return ""; }
        public override bool TableExists(string table) {
            ValidateTable(table);
            return Database.Iterate("SHOW TABLES LIKE '" + table + "'",
                                    null, IterateExists) != null;
        }
        
        public override List<string> AllTables() {
            return Database.GetStrings("SHOW TABLES");
        }
        
        public override List<string> ColumnNames(string table) {
            ValidateTable(table);
            return Database.GetStrings("DESCRIBE `" + table + "`");
        }
        
        public override void RenameTable(string srcTable, string dstTable) {
            ValidateTable(srcTable);
            ValidateTable(dstTable);
            string sql = "RENAME TABLE `" + srcTable + "` TO `" + dstTable + "`";
            Database.Execute(sql, null);
        }
        
        protected override void CreateTableColumns(StringBuilder sql, ColumnDesc[] columns) {
            string priKey = null;
            for (int i = 0; i < columns.Length; i++) {
                ColumnDesc col = columns[i];
                sql.Append(col.Column).Append(' ').Append(col.FormatType());
                
                if (col.PrimaryKey) priKey = col.Column;
                if (col.AutoIncrement) sql.Append(" AUTO_INCREMENT");
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
            w.WriteLine("CREATE TABLE IF NOT EXISTS `{0}` (", table);
            List<string[]> fields = new List<string[]>();
            Database.Iterate("DESCRIBE `" + table + "`", fields, Database.ReadFields);
            
            const int i_name = 0, i_type = 1, i_null = 2, i_key = 3, i_def = 4, i_extra = 5;
            string pri = "";
            
            for (int i = 0; i < fields.Count; i++) {
                string[] field = fields[i];
                if (field[i_key].CaselessEq("pri")) pri = field[i_name];
                
                string meta = field[i_null].CaselessEq("no") ? "NOT NULL" : "DEFAULT NULL";
                if (field[i_def].Length > 0)   meta += " DEFAULT '" + field[i_def] + "'";
                if (field[i_extra].Length > 0) meta += " " + field[i_extra];

                string suffix = pri.Length == 0 && (i == fields.Count - 1) ? "" : ",";
                w.WriteLine("`{0}` {1} {2}{3}", field[i_name], field[i_type], meta, suffix);
            }
            
            if (pri.Length > 0) w.Write("PRIMARY KEY (`{0}`)", pri);
            w.WriteLine(");");
        }
        
        public override void AddColumn(string table, ColumnDesc col, string colAfter) {
            ValidateTable(table);
            string sql = "ALTER TABLE `" + table + "` ADD COLUMN " + col.Column + " " + col.FormatType();
            if (colAfter.Length > 0) sql += " AFTER " + colAfter;
            Database.Execute(sql, null);
        }
        
        public override void AddOrReplaceRow(string table, string columns, params object[] args) {
            ValidateTable(table);
            DoInsert("REPLACE INTO", table, columns, args);
        }
    }
}
