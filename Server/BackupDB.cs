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
    public static partial class Backup {
        
        public static void BackupDatabase(StreamWriter sql, bool lite) {
            //We technically know all tables in the DB...  But since this is MySQL, we can also get them all with a MySQL command
            //So we show the tables, and store the result.
            //Also output information data (Same format as phpMyAdmin's dump)

            //Important note:  This does NOT account for foreign keys, BLOB's etc.  It only works for what we actually put in the db.

            sql.WriteLine("-- MCGalaxy SQL Database Dump");
            sql.WriteLine("-- version 1.5");
            sql.WriteLine("-- http://mcgalaxy.ml");
            sql.WriteLine("--");
            sql.WriteLine("-- Host: {0}", Server.MySQLHost);
            sql.WriteLine("-- Generation Time: {0:d} at {0:HH:mm:ss}", DateTime.Now, DateTime.Now);
            sql.WriteLine("-- MCGalaxy Version: {0}", Server.Version);
            sql.WriteLine();
            sql.WriteLine();

            List<string> sqlTables = GetTables();
            foreach (string name in sqlTables) {
                if (lite && name.CaselessStarts("Block")) continue;
                BackupTable(name, sql);
            }
        }
        
        public static void BackupTable(string tableName, StreamWriter sql) {
            //For each table, we iterate through all rows, (and save them)
            sql.WriteLine();
            sql.WriteLine("-- --------------------------------------------------------");
            sql.WriteLine("-- Table structure for table `{0}`", tableName);
            sql.WriteLine();
            List<string[]> schema = WriteTableSchema(tableName, sql);
            
            using (DataTable data = Database.Fill("SELECT * FROM `" + tableName + "`")) {
                if (data.Rows.Count == 0) {
                    sql.WriteLine("-- No data in table `{0}`!", tableName);
                    sql.WriteLine();
                    return;
                }
                
                sql.WriteLine("--");
                sql.WriteLine("-- Dumping data for table `{0}`", tableName);
                sql.WriteLine("--");
                sql.WriteLine();
                List<DataColumn> allCols = new List<DataColumn>();
                foreach (DataColumn col in data.Columns)
                    allCols.Add(col);
                
                foreach (DataRow row in data.Rows) { //We rely on the correct datatype being given here.
                    sql.WriteLine();
                    sql.Write("INSERT INTO `{0}` (`", tableName);
                    foreach (string[] rParams in schema) {
                        sql.Write(rParams[0]);
                        sql.Write((schema[schema.Count - 1].Equals(rParams) ? "`) VALUES" : "`, `"));
                    }

                    sql.WriteLine();
                    sql.Write("(");
                    for (int col = 0; col < data.Columns.Count; col++) {
                        //The values themselves can be integers or strings, or null
                        Type type = allCols[col].DataType;
                        if (row.IsNull(col)) {
                            sql.Write("NULL");
                        } else if (type == typeof(DateTime)) { // special format
                        	sql.Write("'{0:yyyy-MM-dd HH:mm:ss.ffff}'", (DateTime)row[col]);
                        } else if (type == typeof(bool)) {
                        	sql.Write((bool)row[col] ? "1" : "0");
                        } else if (type == typeof(string)) { // Requires ''
                        	string value = row[col].ToString();
                        	if (value.IndexOf(' ') >= 0)
                        	    value = value.Replace("'", "''");
                        	sql.Write("'{0}'", value);
                        } else {
                        	sql.Write(row[col]); // We assume all other data is left as-is
                            //This includes numbers, blobs, etc.  (As well as objects, but we don't save them into the database)
                        }
                        sql.Write((col < row.ItemArray.Length - 1 ? ", " : ");"));
                    }
                }
                sql.WriteLine();
            }
        }
        
        static List<string[]> WriteTableSchema(string tableName, StreamWriter sql) {
            List<string[]> tableSchema = new List<string[]>();
            if (Server.useMySQL) {
                string[] rowParams;
                string pri;
                sql.WriteLine("CREATE TABLE IF NOT EXISTS `{0}` (", tableName);
                using (DataTable schema = Database.Fill("DESCRIBE `" + tableName + "`")) {
                    rowParams = new string[schema.Columns.Count];
                    pri = "";
                    foreach (DataRow row in schema.Rows) {
                        //Save the info contained to file
                        List<string> tmp = new List<string>();
                        for (int col = 0; col < schema.Columns.Count; col++)
                            tmp.Add(row[col].ToString());
                        
                        rowParams = tmp.ToArray();
                        rowParams[2] = (rowParams[2].ToLower().Equals("no") ? "NOT " : "DEFAULT ") + "NULL";
                        pri += (rowParams[3].ToLower().Equals("pri") ? rowParams[0] + ";" : "");
                        sql.WriteLine("`{0}` {1} {2}" + (rowParams[5].Equals("") ? "" : " {5}") + (pri.Equals("") && row == schema.Rows[schema.Rows.Count - 1] ? "" : ","), rowParams);
                        tableSchema.Add(rowParams);
                    }
                }
                
                if (pri != "") {
                    string[] tmp = pri.Substring(0, pri.Length - 1).Split(';');
                    sql.Write("PRIMARY KEY (`");
                    foreach (string prim in tmp) {
                        sql.Write(prim);
                        sql.Write("`" + (tmp[tmp.Length - 1].Equals(prim) ? ")" : ", `"));
                    }
                }
                sql.WriteLine(");");
            } else {
                using (DataTable tableSQL = Database.Fill("SELECT sql FROM" +
                                                          "   (SELECT * FROM sqlite_master UNION ALL" +
                                                          "    SELECT * FROM sqlite_temp_master)" +
                                                          "WHERE tbl_name LIKE '" + tableName + "'" +
                                                          "  AND type!='meta' AND sql NOT NULL AND name NOT LIKE 'sqlite_%'" +
                                                          "ORDER BY substr(type,2,1), name"))
                {
                    //just print out the data in the table.
                    foreach (DataRow row in tableSQL.Rows) {
                    	string tableSQLString = row[0].ToString();
                        sql.WriteLine(tableSQLString.Replace(" " + tableName, " `" + tableName + "`").Replace("CREATE TABLE `" + tableName + "`", "CREATE TABLE IF NOT EXISTS `" + tableName + "`") + ";");
                        //We parse this ourselves to find the actual types.
                        tableSchema = getSchema(tableSQLString);
                    }
                }
            }
            sql.WriteLine();
            return tableSchema;
        }

        static List<string[]> getSchema(string tableSQLString) {
            // All SQL for creating tables looks like "CREATE TABLE [IF NOT EXISTS] <TableName> (<ColumnDef>[, ... [, PRIMARY KEY (<ColumnName>[, ...])]])
            // <ColumnDef> = <name> <type> [[NOT|DEFAULT] NULL] [PRIMARY KEY] [AUTO_INCREMENT]
            List<string[]> schema = new List<string[]>();
            int foundStart = tableSQLString.IndexOf("(") + 1;
            int foundLength = tableSQLString.LastIndexOf(")") - foundStart;
            tableSQLString = tableSQLString.Substring(foundStart, foundLength);
            
            // Now we have everything inside the parenthisies.
            string[] column = tableSQLString.Split(',');
            foreach (string col in column) {
                if (!col.ToUpper().StartsWith("PRIMARY KEY")) {
                    bool autoInc = col.IndexOf("AUTOINCREMENT") >= 0 || col.IndexOf("AUTO_INCREMENT") >= 0;
                    string[] split = col.TrimStart('\n', '\r', '\t').Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                    //Just to make it the same as the MySQL schema.
                    schema.Add(new string[] { split[0].Trim('`'), split[1].Trim('\t', '`'),
                                   ( split.Length > 2 ? (split[2].Trim('\t', '`').ToUpper() == "NOT" ? "NOT NULL" : "DEFAULT NULL") : ""),
                                   ( split.Length > 2 ? (split[split.Length - 2].Trim('\t', '`').ToUpper() == "PRIMARY" && split[split.Length - 1].Trim('\t', '`').ToUpper() == "KEY" ? "PRI" : "") : ""),
                                   "NULL", autoInc ? "AUTO_INCREMENT" : ""});
                }
            }
            return schema;
        }

        static List<string> GetTables() {
            List<string> tableNames = new List<string>();
            string syntax = Server.useMySQL ? "SHOW TABLES" : "SELECT * FROM sqlite_master WHERE type='table' AND name NOT LIKE 'sqlite_%'";
            using (DataTable tables = Database.Fill(syntax)) {
                foreach (DataRow row in tables.Rows) {
            		string tableName = row[Server.useMySQL ? 0 : 1].ToString();
                    tableNames.Add(tableName);
                }
            }
            return tableNames;
        }
        
        internal static void fillDatabase(Stream stream) {
            //Backup
            using (FileStream backup = File.Create("backup.sql"))
                BackupDatabase(new StreamWriter(backup), false);
            
            //Delete old
            List<string> tables = GetTables();
            foreach (string name in tables)
                Database.Execute("DROP TABLE `" + name + "`");

            // Import data
            using (StreamReader reader = new StreamReader(stream))
                using (BulkTransaction helper = BulkTransaction.Create())
            {
                List<string> buffer = new List<string>();
                while (!reader.EndOfStream) {
                    string cmd = NextStatement(reader, buffer);
                    if (cmd == null || cmd.Length == 0) continue;
                    
                    int index = cmd.ToUpper().IndexOf("CREATE TABLE");
                    if (index > -1) ParseCreate(ref cmd, index);
                    
                    //Run the command in the transaction.
                    helper.Execute(cmd.Replace(" unsigned", "").Replace(" UNSIGNED", ""));
                }
                helper.Commit();
            }
            
            //Not sure if order matters.
            //AUTO_INCREMENT is changed to AUTOINCREMENT for MySQL -> SQLite
            //AUTOINCREMENT is changed to AUTO_INCREMENT for SQLite -> MySQL
            // All we should have in the script file is CREATE TABLE and INSERT INTO commands.
            //executeQuery(sb.ToString().Replace(" unsigned", "").Replace(" UNSIGNED", ""));
        }
        
        static string NextStatement(StreamReader reader, List<string> buffer) {
            buffer.Clear();
            string line = null;
            
            while ((line = reader.ReadLine()) != null) {
                if (line.StartsWith("--")) continue; // comment
                line = line.Trim();
                if (line.Length == 0) continue; // whitespace
                
                buffer.Add(line);
                if (line.EndsWith(";")) break;
            }
            return buffer.Join("");
        }
        
        static void ParseCreate(ref string cmd, int index) {
            cmd = cmd.Remove(0, index);
            //Do we have a primary key?
            try {
                if (Server.useMySQL) {
                    int priIndex = cmd.ToUpper().IndexOf(" PRIMARY KEY AUTOINCREMENT");
                    int priCount = " PRIMARY KEY AUTOINCREMENT".Length;
                    int attIdx = cmd.Substring(0, cmd.Substring(0, priIndex - 1).LastIndexOfAny("` \n".ToCharArray())).LastIndexOfAny("` \n".ToCharArray()) + 1;
                    int attIdxEnd = cmd.IndexOfAny("` \n".ToCharArray(), attIdx) - 1;
                    string attName = cmd.Substring(attIdx, attIdxEnd - attIdx + 1).Trim(' ', '\n', '`', '\r');
                    
                    //For speed, we just delete this, and add it to the attribute name, then delete the auto_increment clause.
                    cmd = cmd.Remove(priIndex, priCount);
                    cmd = cmd.Insert(cmd.LastIndexOf(")"), ", PRIMARY KEY (`" + attName + "`)");
                    cmd = cmd.Insert(cmd.IndexOf(',', priIndex), " AUTO_INCREMENT");
                } else {
                    int priIndex = cmd.ToUpper().IndexOf(",\r\nPRIMARY KEY");
                    int priIndexEnd = cmd.ToUpper().IndexOf(')', priIndex);
                    int attIdx = cmd.IndexOf("(", priIndex) + 1;
                    int attIdxEnd = priIndexEnd - 1;
                    string attName = cmd.Substring(attIdx, attIdxEnd - attIdx + 1);
                    cmd = cmd.Remove(priIndex, priIndexEnd - priIndex + 1);
                    int start = cmd.IndexOf(attName) + attName.Length;
                    int end = cmd.IndexOf(',');
                    cmd = cmd.Remove(start, end - start);
                    cmd = cmd.Insert(cmd.IndexOf(attName) + attName.Length, " INTEGER PRIMARY KEY AUTOINCREMENT");
                    cmd = cmd.Replace(" auto_increment", "").Replace(" AUTO_INCREMENT", "").Replace("True", "1").Replace("False", "0");
                }
            }
            catch (ArgumentOutOfRangeException) { } // If we don't, just ignore it.
        }
    }
}