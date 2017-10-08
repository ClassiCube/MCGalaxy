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
using System.Data.Common;
using System.IO;
using System.Text;
using MySql.Data.MySqlClient;

namespace MCGalaxy.SQL {

    public sealed class MySQLBackend : IDatabaseBackend {

        public static IDatabaseBackend Instance = new MySQLBackend();
        static ParameterisedQuery queryInstance = new MySQLParameterisedQuery();
        
        static string connFormat = "Data Source={0};Port={1};User ID={2};Password={3};Pooling={4};Treat Tiny As Boolean=false;";
        public override string ConnectionString {
            get { return String.Format(connFormat, ServerConfig.MySQLHost, ServerConfig.MySQLPort,
                                       ServerConfig.MySQLUsername, ServerConfig.MySQLPassword, ServerConfig.DatabasePooling); }
        }
        public override bool EnforcesTextLength { get { return true; } }

        public MySQLBackend() {
            CaselessWhereSuffix = " COLLATE utf8_general_ci";
            CaselessLikeSuffix = "";
        }

        
        public override void CreateDatabase() {
            ParameterisedQuery query = GetStaticParameterised();
            Database.Execute(query, "CREATE DATABASE if not exists `" + ServerConfig.MySQLDatabaseName + "`", true);
        }
        
        public override BulkTransaction CreateBulk() {
            return new MySQLBulkTransaction(ConnectionString);
        }
        
        public override ParameterisedQuery CreateParameterised() {
            return new MySQLParameterisedQuery();
        }
        
        internal override ParameterisedQuery GetStaticParameterised() {
            return queryInstance;
        }       
                
        public override string FastGetDateTime(IDataReader reader, int col) {
            DateTime date = reader.GetDateTime(col);
            return date.ToString("yyyy-MM-dd HH:mm:ss");
        }
        
        internal override void ParseCreate(ref string cmd) {
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
        
        
        public override bool TableExists(string table) {
            ValidateTable(table);
            const string syntax = "SELECT * FROM information_schema.tables WHERE table_name = @0 AND table_schema = @1";
            using (DataTable results = Database.Fill(syntax, table, ServerConfig.MySQLDatabaseName)) {
                return results.Rows.Count > 0;
            }
        }
        
        public override List<string> AllTables() {
            using (DataTable results = Database.Fill("SHOW TABLES")) {
                List<string> tables = new List<string>(results.Rows.Count);
                foreach (DataRow row in results.Rows) {
                    tables.Add(row[0].ToString());
                }
                return tables;
            }
        }
        
        public override List<string> ColumnNames(string table) {
            ValidateTable(table);
            using (DataTable results = Database.Fill("DESCRIBE `" + table + "`")) {
                List<string> columns = new List<string>(results.Rows.Count);
                foreach (DataRow row in results.Rows) {
                    columns.Add(row["Field"].ToString());
                }
                return columns;
            }
        }
        
        public override void RenameTable(string srcTable, string dstTable) {
            ValidateTable(srcTable);
            ValidateTable(dstTable);
            string syntax = "RENAME TABLE `" + srcTable + "` TO `" + dstTable + "`";
            Database.Execute(syntax);
        }
        
        public override void ClearTable(string table) {
            ValidateTable(table);
            string syntax = "TRUNCATE TABLE `" + table + "`";
            Database.Execute(syntax);
        }        
        
        protected override void CreateTableColumns(StringBuilder sql, ColumnDesc[] columns) {
            string priKey = null;
            for (int i = 0; i < columns.Length; i++) {
                ColumnDesc col = columns[i];
                sql.Append(col.Column).Append(' ').Append(col.FormatType());
                
                if (col.PrimaryKey) priKey = col.Column;
                if (col.AutoIncrement) sql.Append(" AUTO_INCREMENT");
                if (col.NotNull) sql.Append(" NOT NULL");
                if (col.DefaultValue != null)
                    sql.Append(" DEFAULT ").Append(col.DefaultValue);
                
                if (i < columns.Length - 1) {
                    sql.Append(',');
                } else if (priKey != null) {
                    sql.Append(", PRIMARY KEY(").Append(priKey).Append(") ");
                }
                sql.AppendLine();
            }
        }        
        
        public override void PrintSchema(string table, TextWriter w) {
            string pri = "";
            w.WriteLine("CREATE TABLE IF NOT EXISTS `{0}` (", table);
            
            using (DataTable schema = Database.Fill("DESCRIBE `" + table + "`")) {
                string[] data = new string[schema.Columns.Count];
                foreach (DataRow row in schema.Rows) {
                    for (int col = 0; col < schema.Columns.Count; col++) {
                        data[col] = row[col].ToString();
                    }
                    
                    if (data[3].CaselessEq("pri")) pri = data[0];
                    string value = data[2].CaselessEq("no") ? "NOT NULL" : "DEFAULT NULL";
                    if (data[4].Length > 0) value += " DEFAULT '" + data[4] + "'";
                    if (data[5].Length > 0) value += " " + data[5];

                    string suffix = pri.Length == 0 && row == schema.Rows[schema.Rows.Count - 1] ? "" : ",";
                    w.WriteLine("`{0}` {1} {2}{3}", row[0], row[1], value, suffix);
                }
            }
            
            if (pri.Length > 0) w.Write("PRIMARY KEY (`{0}`)", pri);
            w.WriteLine(");");
        }
                
        public override void AddColumn(string table, ColumnDesc col, string colAfter) {
            ValidateTable(table);
            string syntax = "ALTER TABLE `" + table + "` ADD COLUMN " 
                + col.Column + " " + col.FormatType();
            if (colAfter.Length > 0) syntax += " AFTER " + colAfter;
            Database.Execute(syntax);
        }
        
        public override void AddOrReplaceRow(string table, string columns, params object[] args) {
            ValidateTable(table);
            DoInsert("REPLACE INTO", table, columns, args);
        }
    }
    
    
    public sealed class MySQLBulkTransaction : BulkTransaction {

        public MySQLBulkTransaction(string connString) {
            connection = new MySqlConnection(connString);
            connection.Open();
            connection.ChangeDatabase(ServerConfig.MySQLDatabaseName);

            transaction = connection.BeginTransaction();
        }

        public override IDbCommand CreateCommand(string query) {
            return new MySqlCommand(query, (MySqlConnection)connection, (MySqlTransaction)transaction);
        }
        
        public override IDataParameter CreateParam(string paramName, DbType type) {
            MySqlParameter arg = new MySqlParameter(paramName, null);
            arg.DbType = type;
            return arg;
        }
    }

    public sealed class MySQLParameterisedQuery : ParameterisedQuery {
        protected override bool MultipleSchema { get { return true; } }
        
        protected override IDbConnection CreateConnection(string connString) {
            return new MySqlConnection(connString);
        }
        
        protected override IDbCommand CreateCommand(string query, IDbConnection conn) {
            return new MySqlCommand(query, (MySqlConnection)conn);
        }
        
        protected override DbDataAdapter CreateDataAdapter(string query, IDbConnection conn) {
            return new MySqlDataAdapter(query, (MySqlConnection)conn);
        }
        
        protected override IDbDataParameter CreateParameter() {
            return new MySqlParameter();
        }
    }    
}
