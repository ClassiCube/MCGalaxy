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
using System.Text;
using MySql.Data.MySqlClient;

namespace MCGalaxy.SQL {

    public sealed class MySQLBackend : IDatabaseBackend {

        public static IDatabaseBackend Instance = new MySQLBackend();
        static ParameterisedQuery queryInstance = new MySQLParameterisedQuery();
        
        static string connFormat = "Data Source={0};Port={1};User ID={2};Password={3};Pooling={4};Treat Tiny As Boolean=false;";
        public override string ConnectionString {
            get { return String.Format(connFormat, Server.MySQLHost, Server.MySQLPort,
                                       Server.MySQLUsername, Server.MySQLPassword, Server.DatabasePooling); }
        }
        public override bool EnforcesTextLength { get { return true; } }
        
        public override void CreateDatabase() {
            ParameterisedQuery query = GetStaticParameterised();
            Database.Execute(query, "CREATE DATABASE if not exists `" + Server.MySQLDatabaseName + "`", true);
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
        
        
        public override bool TableExists(string table) {
            const string syntax = "SELECT * FROM information_schema.tables WHERE table_name = @0 AND table_schema = @1";
            using (DataTable results = Database.Fill(syntax, table, Server.MySQLDatabaseName)) {
                return results.Rows.Count > 0;
            }
        }
        
        public override void RenameTable(string srcTable, string dstTable) {
            string syntax = "RENAME TABLE `" + srcTable + "` TO `" + dstTable + "`";
            Database.Execute(syntax);
        }
        
        public override void ClearTable(string table) {
            string syntax = "TRUNCATE TABLE `" + table + "`";
            Database.Execute(syntax);
        }        
        
        public override void AddColumn(string table, string column, 
                                       string colType, string colAfter) {
            string syntax = "ALTER TABLE `" + table + "` ADD COLUMN " 
                + column + " " + colType;
            if (colAfter != "") syntax += " AFTER " + colAfter;
            Database.Execute(syntax);
        }
        
        public override void CreateTable(string table, ColumnParams[] columns) {
            StringBuilder sql = new StringBuilder();
            sql.AppendLine("CREATE TABLE if not exists `" + table + "` (");
            string priKey = null;
            
            for (int i = 0; i < columns.Length; i++) {
                ColumnParams col = columns[i];
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
            sql.AppendLine(");");
            Database.Execute(sql.ToString());
        }
    }
}
