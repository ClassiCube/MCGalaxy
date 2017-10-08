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
            sql.WriteLine("-- Host: {0}", ServerConfig.MySQLHost);
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
            Database.Backend.PrintSchema(tableName, sql);
            
            TableDumper dumper = new TableDumper();
            dumper.DumpTable(sql, tableName);
            dumper.sql = null;
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
            
            using (StreamReader reader = new StreamReader(sql)) {
                ImportBulk(reader);
            }
        }
        
        static void ImportBulk(StreamReader reader) {
            BulkTransaction helper = null;
            List<string> buffer = new List<string>();
            
            try {
                helper = Database.Backend.CreateBulk();
                while (!reader.EndOfStream) {
                    string cmd = NextStatement(reader, buffer);
                    if (cmd == null || cmd.Length == 0) continue;
                    
                    int index = cmd.ToUpper().IndexOf("CREATE TABLE");
                    if (index > -1) {
                        cmd = cmd.Remove(0, index);
                        cmd = cmd.Replace(" unsigned", " UNSIGNED");
                        Database.Backend.ParseCreate(ref cmd);
                    }
                    
                    //Run the command in the transaction.
                    if (helper.Execute(cmd)) continue;

                    // Something went wrong.. commit what we've imported so far.
                    // We need to recreate connection otherwise every helper.Execute fails
                    helper.Commit();
                    helper.Dispose();
                    helper = Database.Backend.CreateBulk();
                }
                helper.Commit();
            } finally {
                if (helper != null) helper.Dispose();
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
    }
}