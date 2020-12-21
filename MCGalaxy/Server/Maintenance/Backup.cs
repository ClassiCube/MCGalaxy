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
using System.IO;
using MCGalaxy.SQL;

namespace MCGalaxy {
    /// <summary> Utility methods for backing up and restoring a server. </summary>
    public static class Backup {
        const string zipPath = "MCGalaxy.zip", sqlPath = "SQL.sql", dbPath = "MCGalaxy.db";
        
        public static void Perform(Player p, bool files, bool db, bool lite, bool compress) {
            if (db) {
                Logger.Log(LogType.SystemActivity, "Backing up the database...");
                using (StreamWriter sql = new StreamWriter(sqlPath))
                    BackupDatabase(sql);
                Logger.Log(LogType.SystemActivity, "Backed up the database to " + sqlPath);
            }
            
            List<string> filesList = null;
            if (files) {
                Logger.Log(LogType.SystemActivity, "Determining which files to backup...");
                filesList = GetAllFiles(lite);
                Logger.Log(LogType.SystemActivity, "Finished determining included files");
            }

            Logger.Log(LogType.SystemActivity, "Creating compressed backup...");
            using (Stream stream = File.Create(zipPath)) {
                ZipWriter writer = new ZipWriter(stream);
                if (files) SaveFiles(writer, filesList, compress);
                if (db) SaveDatabase(writer, compress);
                
                writer.FinishEntries();
                writer.WriteFooter();
                Logger.Log(LogType.SystemActivity, "Compressed all data!");
            }
            p.Message("Backup of (" + (files ? "everything" + (db ? "" : " but database") : "database") + ") complete!");
            Logger.Log(LogType.SystemActivity, "Server backed up!");
        }
        
        static List<string> GetAllFiles(bool lite) {
            string[] all = Directory.GetFiles("./", "*", SearchOption.AllDirectories);
            List<string> paths = new List<string>();
            
            for (int i = 0; i < all.Length; i++) {
                string path = all[i];
                // convert to zip entry form
                path = path.Replace('\\', '/').Replace("./", "");
                
                if (lite && path.Contains("extra/undo/"))         continue;
                if (lite && path.Contains("extra/undoPrevious/")) continue;
                if (lite && path.Contains("levels/prev"))         continue;
                if (lite && path.Contains("levels/backups/"))     continue;
                if (lite && path.Contains("blockdb/"))            continue;
                
                //if (path.Contains(zipPath)) continue;
                //if (path.Contains(sqlPath)) continue;
                //if (path.Contains(dbPath))  continue;
                // ignore files in root folder
                if (path.IndexOf('/') == -1) continue;
                paths.Add(path);
            }
            return paths;
        }
        
        static void SaveFiles(ZipWriter writer, List<string> paths, bool compress) {
            Logger.Log(LogType.SystemActivity, "Compressing {0} files...", paths.Count);
            for (int i = 0; i < paths.Count; i++) {
                string path = paths[i];
                bool compressThis = compress && !path.CaselessContains(".lvl");
                
                try {
                    using (Stream src = File.OpenRead(path)) {
                        writer.WriteEntry(src, path, compressThis);
                    }
                } catch (Exception ex) {
                    Logger.LogError("Failed to backup file: " + path, ex);
                }
                
                if (i == 0 || (i % 100) != 0) continue;
                Logger.Log(LogType.SystemActivity, "Backed up {0}/{1} files", i, paths.Count);
            }
        }
        
        static void SaveDatabase(ZipWriter writer, bool compress) {
            Logger.Log(LogType.SystemActivity, "Compressing Database...");
            using (Stream src = File.OpenRead(sqlPath)) {
                writer.WriteEntry(src, sqlPath, compress);
            }
            Logger.Log(LogType.SystemActivity, "Database compressed");
        }

        public static void Extract(Player p) {
            int errors = 0;
            using (FileStream src = File.OpenRead(zipPath)) {
                ZipReader reader = new ZipReader(src);
                reader.FindFooter();
                int entries = reader.FindEntries();
                p.Message("Restoring {0} files", entries);
                
                for (int i = 0; i < entries; i++) {
                    string path = ExtractItem(reader, i, ref errors);                    
                    if (i > 0 && (i % 100) == 0) {
                        Logger.Log(LogType.SystemActivity, "Restored {0}/{1} files", i, entries);
                    }
                    
                    if (!path.CaselessEq(sqlPath)) continue;                
                    // If DB is in there, they backed it up, meaning they want it restored
                    using (Stream part = reader.GetEntry(i, out path)) {
                        ReplaceDatabase(part);
                    }
                }
            }
            
            // To make life easier, we reload settings now, to make it less likely to need restart
            Server.LoadAllSettings();
            p.Message("Server restored" + (errors > 0 ? " with errors. May be a partial restore" : ""));
            p.Message("It is recommended that you restart the server, although this is not required.");
        }
        
        static void Extract(Stream src, string path) {
            byte[] buf = new byte[4096];
            int read = 0;
            
            using (Stream dst = File.Create(path)) {
                while ((read = src.Read(buf, 0, buf.Length)) > 0)
                    dst.Write(buf, 0, read);
            }
        }
        
        static string ExtractItem(ZipReader reader, int i, ref int errors) {
            string path;
            using (Stream part = reader.GetEntry(i, out path)) {
                // old server backup used to URI encode files
                path = Uri.UnescapeDataString(path);
                
                try {
                    Extract(part, path); return path;
                } catch {
                    try {
                        string dir = Path.GetDirectoryName(path);
                        Directory.CreateDirectory(dir);
                        Extract(part, path); return path;
                    } catch (IOException e) {
                        Logger.LogError(e);
                        Logger.Log(LogType.Warning, "%WError extracting {0}, continuing with rest.", path);
                        errors++;
                        return "";
                    }
                }
            }
        }
        
        
        static void BackupDatabase(StreamWriter sql) {
            // NOTE: This does NOT account for foreign keys, BLOBs etc. It only works for what we actually put in the DB.
            sql.WriteLine("-- {0} SQL database dump", Server.SoftwareNameVersioned);
            sql.WriteLine("-- Host: {0}", Server.Config.MySQLHost);
            sql.WriteLine("-- Generated on {0:d} at {0:HH:mm:ss}", DateTime.Now);
            sql.WriteLine();
            sql.WriteLine();

            List<string> tables = Database.Backend.AllTables();
            foreach (string name in tables) {
                BackupTable(name, sql);
            }
        }
        
        public static void BackupTable(string tableName, StreamWriter sql) {
            sql.WriteLine();
            sql.WriteLine("-- --------------------------------------------------------");
            sql.WriteLine("-- Table structure for table `{0}`", tableName);
            sql.WriteLine();
            Database.Backend.PrintSchema(tableName, sql);
            
            TableDumper dumper = new TableDumper();
            dumper.DumpTable(sql, tableName);
            dumper.sql = null;
        }
        
        static void ReplaceDatabase(Stream sql) {
            using (Stream backup = File.Create("backup.sql"))
                BackupDatabase(new StreamWriter(backup));

            List<string> tables = Database.Backend.AllTables();
            foreach (string table in tables) {
                Database.DeleteTable(table);
            }
            ImportSql(sql);
        }
        
        internal static void ImportSql(Stream sql) {
            // Import data (we only have CREATE TABLE and INSERT INTO statements)
            using (StreamReader reader = new StreamReader(sql)) {
                ImportBulk(reader);
            }
        }
        
        static void ImportBulk(StreamReader reader) {
            SqlTransaction bulk = null;
            List<string> buffer = new List<string>();
            
            try {
                bulk = new SqlTransaction();
                while (!reader.EndOfStream) {
                    string cmd = NextStatement(reader, buffer);
                    if (cmd == null || cmd.Length == 0) continue;
                    
                    int index = cmd.ToUpper().IndexOf("CREATE TABLE");
                    if (index >= 0) {
                        cmd = cmd.Remove(0, index);
                        cmd = cmd.Replace(" unsigned", " UNSIGNED");
                        Database.Backend.ParseCreate(ref cmd);
                    }
                    
                    //Run the command in the transaction.
                    if (bulk.Execute(cmd, null)) continue;

                    // Something went wrong.. commit what we've imported so far.
                    // We need to recreate connection otherwise every helper.Execute fails
                    bulk.Commit();
                    bulk.Dispose();
                    bulk = new SqlTransaction();
                }
                bulk.Commit();
            } finally {
                if (bulk != null) bulk.Dispose();
            }
        }
        
        static string NextStatement(StreamReader r, List<string> buffer) {
            buffer.Clear();
            string line = null;
            
            while ((line = r.ReadLine()) != null) {
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
