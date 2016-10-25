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
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using System.Text;

namespace MCGalaxy.SQL {

    public sealed class SQLiteBackend : IDatabaseBackend {

        public static IDatabaseBackend Instance = new SQLiteBackend();
        static ParameterisedQuery queryInstance = new SQLiteParameterisedQuery();
        
        static string connFormat = "Data Source =" + Server.apppath + "/MCGalaxy.db; Version =3; Pooling ={0}; Max Pool Size =300;";        
        public override string ConnectionString {
            get { return String.Format(connFormat, Server.DatabasePooling); }
        }
        public override bool EnforcesTextLength { get { return false; } }
        
        public override void CreateDatabase() { }
        
        public override BulkTransaction CreateBulk() {
            return new SQLiteBulkTransaction(ConnectionString);
        }
        
        public override ParameterisedQuery CreateParameterised() {
            return new SQLiteParameterisedQuery();
        }
        
        internal override ParameterisedQuery GetStaticParameterised() {
            return queryInstance;
        }
        
        
        public override bool TableExists(string table) {
            using (DataTable results = GetRows("sqlite_master", "name",
                                               "WHERE type='table' AND name=@0", table)) {
                return results.Rows.Count > 0;
            }
        }
        
        public override void RenameTable(string srcTable, string dstTable) {
            string syntax = "ALTER TABLE `" + srcTable + "` RENAME TO `" + dstTable + "`";
            Database.Execute(syntax);
        }
        
        public override void ClearTable(string table) {
            string syntax = "DELETE FROM `" + table + "`";
            Database.Execute(syntax);
        }
        
        public override void AddColumn(string table, string column, 
                                       string colType, string colAfter) {
            string syntax = "ALTER TABLE `" + table + "` ADD COLUMN " 
                + column + " " + colType;
            Database.Execute(syntax);
        }
        
        protected override void CreateTableColumns(StringBuilder sql, ColumnDesc[] columns) {
            string priKey = null;            
            for (int i = 0; i < columns.Length; i++) {
                ColumnDesc col = columns[i];
                if (col.Type == ColumnType.Bool) {
                    sql.Append(col.Column).Append(' ').Append("TINYINT");
                } else {
                    sql.Append(col.Column).Append(' ').Append(col.FormatType());
                }
                
                // When the primary key isn't autoincrement, we use the same form as mysql
                // Otherwise we have to use sqlite's 'PRIMARY KEY AUTO_INCREMENT' form
                if (col.PrimaryKey) {
                    if (!col.AutoIncrement) priKey = col.Column;
                    else sql.Append(" PRIMARY KEY");
                }
                if (col.AutoIncrement) sql.Append(" AUTOINCREMENT");
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
        
        public override void AddOrReplaceRow(string table, string columns, params object[] args) {
            DoInsert("INSERT OR REPLACE INTO", table, columns, args);
        }
    }
    
    
     public sealed class SQLiteBulkTransaction : BulkTransaction {

        public SQLiteBulkTransaction(string connString) { 
            connection = new SQLiteConnection(connString);
            connection.Open();
            transaction = connection.BeginTransaction();
        }
        
        public override IDbCommand CreateCommand(string query) {
            return new SQLiteCommand(query, (SQLiteConnection)connection, (SQLiteTransaction)transaction);
        }
        
        public override IDataParameter CreateParam(string paramName, DbType type) {
            return new SQLiteParameter(paramName, type);
        }
    }
    
    public sealed class SQLiteParameterisedQuery : ParameterisedQuery {        
        protected override bool MultipleSchema { get { return false; } }
        
        protected override IDbConnection CreateConnection(string connString) {
            return new SQLiteConnection(connString);
        }
        
        protected override IDbCommand CreateCommand(string query, IDbConnection conn) {
            return new SQLiteCommand(query, (SQLiteConnection)conn);
        }
        
        protected override DbDataAdapter CreateDataAdapter(string query, IDbConnection conn) {
            return new SQLiteDataAdapter(query, (SQLiteConnection)conn);
        }
        
        protected override IDbDataParameter CreateParameter() {
            return new SQLiteParameter();
        }
    }
}
