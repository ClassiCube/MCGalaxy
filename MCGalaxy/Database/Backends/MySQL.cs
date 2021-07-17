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
using MySql.Data.MySqlClient;

namespace MCGalaxy.SQL {

    public sealed class MySQLBackend : IDatabaseBackend 
    {
        public static IDatabaseBackend Instance = new MySQLBackend();
        public MySQLBackend() {
            // MySQL uses case insensitive collation by default
            CaselessWhereSuffix = "";
            CaselessLikeSuffix  = "";
        }
        
        public override bool EnforcesTextLength { get { return true; } }
        public override bool MultipleSchema { get { return true; } }     
        
        internal override ISqlConnection CreateConnection() {
            const string format = "Data Source={0};Port={1};User ID={2};Password={3};Pooling={4};Treat Tiny As Boolean=false;";
            string str = string.Format(format, Server.Config.MySQLHost, Server.Config.MySQLPort,
                                       Server.Config.MySQLUsername, Server.Config.MySQLPassword, Server.Config.DatabasePooling);
            
            MySqlConnection conn = new MySqlConnection(str);
            return new MySQLConnection(conn);
        }
        
        public override void CreateDatabase() {
            string sql = "CREATE DATABASE if not exists `" + Server.Config.MySQLDatabaseName + "`";
            Database.Do(sql, true, null, null);
        }
        
        public override string RawGetDateTime(ISqlRecord record, int col) {
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
        

        static object IterateExists(ISqlRecord record, object arg) { return ""; }
        public override bool TableExists(string table) {
            return Database.Iterate("SHOW TABLES LIKE '" + table + "'",
                                    null, IterateExists) != null;
        }
        
        public override List<string> AllTables() {
            return GetStrings("SHOW TABLES");
        }
        
        public override List<string> ColumnNames(string table) {
            Database.ValidateName(table);
            return GetStrings("DESCRIBE `" + table + "`");
        }
        
        public override string RenameTableSql(string srcTable, string dstTable) {
            return "RENAME TABLE `" + srcTable + "` TO `" + dstTable + "`";
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
        
        public override string AddColumnSql(string table, ColumnDesc col, string colAfter) {
            string sql = "ALTER TABLE `" + table + "` ADD COLUMN " + col.Column + " " + col.FormatType();
            if (colAfter.Length > 0) sql += " AFTER " + colAfter;
            return sql;
        }
        
        public override string AddOrReplaceRowSql(string table, string columns, object[] args) {
            return InsertSql("REPLACE INTO", table, columns, args);
        }
    }
    
    sealed class MySQLConnection : ISqlConnection 
    {
        readonly MySqlConnection conn;        
        public MySQLConnection(MySqlConnection conn) { this.conn = conn; }
        
        public ISqlTransaction BeginTransaction() {
            MySqlTransaction trn = conn.BeginTransaction();
            return new MySQLTransaction(trn);
        }
        
        public ISqlCommand CreateCommand(string sql) {
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            return new MySQLCommand(cmd);
        }
        
        public void Open() { conn.Open(); }
        public void ChangeDatabase(string name) { conn.ChangeDatabase(name); }
        public void Close() { conn.Close(); }
        public void Dispose() { conn.Dispose(); }
    }
    
    sealed class MySQLCommand : ISqlCommand
    {        
        readonly MySqlCommand cmd;
        public MySQLCommand(MySqlCommand cmd) { this.cmd = cmd; }
        
        public void ClearParameters() { 
            cmd.Parameters.Clear(); 
        }
        public void AddParameter(string name, object value) {
            cmd.Parameters.AddWithValue(name, value);
        }
        
        public void Dispose() { cmd.Dispose(); }
        public void Prepare() { cmd.Prepare(); }
        public int ExecuteNonQuery() { return cmd.ExecuteNonQuery(); }
        
        public ISqlReader ExecuteReader() { 
            MySqlDataReader rdr = cmd.ExecuteReader();
            return new MySQLReader(rdr);
        }
    }

    sealed class MySQLTransaction : ISqlTransaction
    {
        readonly MySqlTransaction trn;
        public MySQLTransaction(MySqlTransaction trn) { this.trn = trn; }        
        
        public void Commit()   { trn.Commit(); }
        public void Rollback() { trn.Rollback(); }
        public void Dispose()  { trn.Dispose(); }
    }
    
    sealed class MySQLReader : ISqlReader
    {
        readonly MySqlDataReader rdr;
        public MySQLReader(MySqlDataReader rdr) { this.rdr = rdr; }
        
        public int RowsAffected { get { return rdr.RecordsAffected; } }
        public void Close()   { rdr.Close(); }
        public void Dispose() { rdr.Dispose(); }
        public bool Read()    { return rdr.Read(); }
        
        
        public int FieldCount { get  { return rdr.FieldCount; } }
        public string GetName(int i) { return rdr.GetName(i); }
        public Type GetFieldType(int i) { return rdr.GetFieldType(i); }
        public int GetOrdinal(string name) { return rdr.GetOrdinal(name); }
        
        public object this[int i] { get { return rdr[i]; } }
        public object this[string name] { get { return rdr[name]; } }
        public object GetValue(int i)   { return rdr.GetValue(i); }
        
        public bool GetBoolean(int i)  { return rdr.GetBoolean(i); }
        public byte[] GetBytes(int i)  { return null; }
        public int GetInt32(int i)     { return rdr.GetInt32(i); }
        public long GetInt64(int i)    { return rdr.GetInt64(i); }
        public double GetDouble(int i) { return rdr.GetDouble(i); }
        public string GetString(int i) { return rdr.GetString(i); }
        public DateTime GetDateTime(int i) { return rdr.GetDateTime(i); }
        public bool IsDBNull(int i)    { return rdr.IsDBNull(i); }
    }
}
