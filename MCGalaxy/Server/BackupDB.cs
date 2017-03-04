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

            sql.WriteLine("-- {0} SQL Database Dump", Server.SoftwareName);
            sql.WriteLine("-- Host: {0}", Server.MySQLHost);
            sql.WriteLine("-- Generation Time: {0:d} at {0:HH:mm:ss}", DateTime.Now);
            sql.WriteLine("-- {0} version: {1}", Server.SoftwareName, Server.VersionString);
            sql.WriteLine();
            sql.WriteLine();

            List<string> sqlTables = Database.Backend.AllTables();
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
            WriteTableSchema(tableName, sql);
            
            TableDumper dumper = new TableDumper();
            dumper.DumpTable(sql, tableName);
            dumper.sql = null;
        }
        
        static void WriteTableSchema(string tableName, StreamWriter sql) {
            if (Server.useMySQL) {
                string pri = "";
                sql.WriteLine("CREATE TABLE IF NOT EXISTS `{0}` (", tableName);
                
                using (DataTable schema = Database.Fill("DESCRIBE `" + tableName + "`")) {
                    string[] rowParams = new string[schema.Columns.Count];
                    foreach (DataRow row in schema.Rows) {
                        //Save the info contained to file
                        List<string> tmp = new List<string>();
                        for (int col = 0; col < schema.Columns.Count; col++)
                            tmp.Add(row[col].ToString());
                        
                        rowParams = tmp.ToArray();
                        rowParams[2] = (rowParams[2].CaselessEq("no") ? "NOT " : "DEFAULT ") + "NULL";
                        pri += (rowParams[3].CaselessEq("pri") ? rowParams[0] + ";" : "");
                        sql.WriteLine("`{0}` {1} {2}" + (rowParams[5].Equals("") ? "" : " {5}") + (pri.Equals("") && row == schema.Rows[schema.Rows.Count - 1] ? "" : ","), rowParams);
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
                using (DataTable tableSQL = Database.Fill("SELECT sql FROM sqlite_master WHERE tbl_name LIKE @0" +
                                                          " AND type = 'table' AND name NOT LIKE 'sqlite_%'" +
                                                          " ORDER BY substr(type,2,1), name", tableName))
                {
                    //just print out the data in the table.
                    foreach (DataRow row in tableSQL.Rows) {
                        string sqlSyntax = row[0].ToString();
                        sqlSyntax = sqlSyntax.Replace(" " + tableName, " `" + tableName + "`");
                        sqlSyntax = sqlSyntax.Replace("CREATE TABLE `" + tableName + "`", "CREATE TABLE IF NOT EXISTS `" + tableName + "`");
                        sql.WriteLine(sqlSyntax + ";");
                    }
                }
            }
            sql.WriteLine();
        }
        
        internal static void ReplaceDatabase(Stream sql) {
            using (FileStream backup = File.Create("backup.sql"))
                BackupDatabase(new StreamWriter(backup), false); // backup

            List<string> tables = Database.Backend.AllTables();
            foreach (string table in tables)
                Database.Backend.DeleteTable(table); // drop all tables
            
            ImportSql(sql);
        }
        
        internal static void ImportSql(Stream sql) {
            // Import data (we only have CREATE TABLE and INSERT INTO statements)
            using (StreamReader reader = new StreamReader(sql))
                using (BulkTransaction helper = BulkTransaction.Create())
            {
                List<string> buffer = new List<string>();
                while (!reader.EndOfStream) {
                    string cmd = NextStatement(reader, buffer);
                    if (cmd == null || cmd.Length == 0) continue;
                    
                    int index = cmd.ToUpper().IndexOf("CREATE TABLE");
                    if (index > -1) ParseCreate(ref cmd, index);
                    
                    //Run the command in the transaction.
                    helper.Execute(cmd);
                }
                helper.Commit();
            }
        }
        
        static string NextStatement(StreamReader reader, List<string> buffer) {
            buffer.Clear();
            string line = null;
            
            while ((line = reader.ReadLine()) != null) {
                if (line.StartsWith("--")) continue; // comment
                line = line.Trim();
                if (line.Length == 0) continue; // whitespace
                
                buffer.Add(line);
                if (line[line.Length - 1] == ';') break;
            }
            return buffer.Join("");
        }
        
        static void ParseCreate(ref string cmd, int index) {
            cmd = cmd.Remove(0, index);
            cmd = cmd.Replace(" unsigned", " UNSIGNED");
            if (!Server.useMySQL) return;
            
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
    }
}