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

namespace MCGalaxy.SQL {
    
    /// <summary> Simple abstraction for a database management system. </summary>
    public abstract class IDatabaseBackend {
        
        /// <summary> Whether this backend enforces the character length in VARCHAR columns. </summary>
        public abstract bool EnforcesTextLength { get; }        
        /// <summary> Whether this backend supports multiple database schemas. </summary>
        public abstract bool MultipleSchema { get; }
        
        internal abstract IDbConnection CreateConnection();
        internal abstract IDbCommand CreateCommand(string sql, IDbConnection conn);
        internal abstract IDbDataParameter CreateParameter();
        
        /// <summary> Suffix required after a WHERE clause for caseless string comparison. </summary>
        public string CaselessWhereSuffix { get; protected set; }
        /// <summary> Suffix required after a LIKE clause for caseless string comparison. </summary>        
        public string CaselessLikeSuffix { get; protected set; }

        
        /// <summary> Creates the schema for this database (if required). </summary>
        public abstract void CreateDatabase();
        
        public abstract string RawGetDateTime(IDataRecord record, int col);
        
        protected internal virtual void ParseCreate(ref string cmd) { }
        
        protected static List<string> GetStrings(string sql, params object[] args) {
            List<string> values = new List<string>();
            Database.Iterate(sql, values, Database.ReadList, args);
            return values;
        }
        
        
        // == Higher level table management functions ==
        
        /// <summary> Returns whether a table (case sensitive) exists by that name. </summary>
        public abstract bool TableExists(string table);

        /// <summary> Returns a list of all tables in this database. </summary>
        public abstract List<string> AllTables();
        
        /// <summary> Returns a list of the column names in the given table. </summary>
        public abstract List<string> ColumnNames(string table);
        
        /// <summary> Returns SQL for renaming the source table to the given name. </summary>
        public abstract string RenameTableSql(string srcTable, string dstTable);
        
        /// <summary> Returns SQL for creating a new table (unless it already exists). </summary>
        public virtual string CreateTableSql(string table, ColumnDesc[] columns) {
            StringBuilder sql = new StringBuilder();
            sql.AppendLine("CREATE TABLE if not exists `" + table + "` (");
            CreateTableColumns(sql, columns);
            sql.AppendLine(");");
            return sql.ToString();
        }
        
        protected abstract void CreateTableColumns(StringBuilder sql, ColumnDesc[] columns);
        
        /// <summary> Returns SQL for completely removing the given table. </summary>
        public virtual string DeleteTableSql(string table) {
            return "DROP TABLE `" + table + "`";
        }
        
        /// <summary> Prints/dumps the table schema of the given table. </summary>
        public abstract void PrintSchema(string table, TextWriter w);
        
        /// <summary> Returns SQL for adding a new coloumn to the given table. </summary>
        /// <remarks> Note colAfter is only a hint - some database backends ignore this. </remarks>
        public abstract string AddColumnSql(string table, ColumnDesc col, string colAfter);

        
        // == Higher level functions ==

        /// <summary> Returns SQL for copying all the rows from the source table into the destination table. </summary>
        public virtual string CopyAllRowsSql(string srcTable, string dstTable) {
            return "INSERT INTO `" + dstTable + "` SELECT * FROM `" + srcTable + "`";
        }
        
        /// <summary> Returns SQL for reading rows from the given table. </summary>
        public virtual string ReadRowsSql(string table, string columns, string modifier) {
            string sql = "SELECT " + columns + " FROM `" + table + "`";
            if (modifier.Length > 0) sql += " " + modifier;
            return sql;
        }
        
        /// <summary> Returns SQL for updating rows for the given table. </summary>
        public virtual string UpdateRowsSql(string table, string columns, string modifier) {
            string sql = "UPDATE `" + table + "` SET " + columns;
            if (modifier.Length > 0) sql += " " + modifier;
            return sql;
        }
        
        /// <summary> Returns SQL for deleting rows for the given table. </summary>
        public virtual string DeleteRowsSql(string table, string modifier) {
            string sql = "DELETE FROM `" + table + "`";
            if (modifier.Length > 0) sql += " " + modifier;
            return sql;
        }

        /// <summary> Returns SQL for adding a row to the given table. </summary>
        public virtual string AddRowSql(string table, string columns, object[] args) {
            return InsertSql("INSERT INTO", table, columns, args);
        }
        
        /// <summary> Returns SQL for adding or replacing a row (same primary key) in the given table. </summary>
        public abstract string AddOrReplaceRowSql(string table, string columns, object[] args);
      
        
        protected string InsertSql(string cmd, string table, string columns, object[] args) {
            StringBuilder sql = new StringBuilder(cmd);
            sql.Append(" `").Append(table).Append("` ");
            sql.Append('(').Append(columns).Append(')');
            
            string[] names = SqlQuery.GetNames(args.Length);
            sql.Append(" VALUES (");
            for (int i = 0; i < args.Length; i++) {
                sql.Append(names[i]);
                if (i < args.Length - 1) sql.Append(", ");
                else sql.Append(")");
            }
            return sql.ToString();
        }
    }
}
