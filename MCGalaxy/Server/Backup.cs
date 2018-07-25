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
using System.IO.Packaging;

namespace MCGalaxy {
    public static partial class Backup {

        const string zipPath = "MCGalaxy.zip", sqlPath = "SQL.sql", dbPath = "MCGalaxy.db";
        public class BackupArgs {
            public Player p;
            public bool Files, Database, Lite;
        }
        
        public static void CreatePackage(Player p, bool files, bool db, bool lite) {
            if (db) {
                Logger.Log(LogType.SystemActivity, "Backing up the database...");
                using (StreamWriter sql = new StreamWriter(sqlPath))
                    BackupDatabase(sql,lite);
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
                if (files) {
                    Logger.Log(LogType.SystemActivity, "Compressing files...");
                    SaveFiles(writer, filesList);
                }
                
                if (db) SaveDatabase(writer);
                
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
        
        static void SaveFiles(ZipWriter writer, List<string> paths) {
            foreach (string path in paths) {
                try {
                    using (Stream src = File.OpenRead(path)) {
                        writer.WriteEntry(src, path);
                    }
                } catch (Exception ex) {
                    Logger.LogError("Failed to backup file: " + path, ex);
                }
            }
        }
        
        static void SaveDatabase(ZipWriter writer) {
            Logger.Log(LogType.SystemActivity, "Compressing Database...");
            // TODO: gzip compress
            using (FileStream fs = File.OpenRead(sqlPath)) {
                writer.WriteEntry(fs, sqlPath);
            }
            Logger.Log(LogType.SystemActivity, "Database compressed");
        }

        public static void ExtractPackage(Player p) {
            int errors = 0;
            using (FileStream src = File.OpenRead(zipPath))
                using (ZipPackage zip = (ZipPackage)ZipPackage.Open(src))
            {
                PackagePartCollection parts = zip.GetParts();
                foreach (ZipPackagePart item in parts) {
                    ExtractItem(item, ref errors);
                    if (item.Uri.ToString().CaselessContains("sql.sql")) {
                        // If it's in there, they backed it up, meaning they want it restored
                        Backup.ReplaceDatabase(item.GetStream());
                    }
                }
            }
            
            // To make life easier, we reload settings now, to maker it less likely to need restart
            Server.LoadAllSettings();
            p.Message("Server restored" + (errors > 0 ? " with errors. May be a partial restore" : ""));
            p.Message("It is recommended that you restart the server, although this is not required.");
        }
        
        static void CopyStream(Stream source, Stream target) {
            const int bufSize = 0x1000;
            byte[] buf = new byte[bufSize];
            int bytesRead = 0;
            while ((bytesRead = source.Read(buf, 0, bufSize)) > 0)
                target.Write(buf, 0, bytesRead);
        }
        
        static void ExtractItem(ZipPackagePart item, ref int errors) {
            string entry = item.Uri.ToString();
            string file = "./" + Uri.UnescapeDataString(entry);
            
            using (Stream src = item.GetStream()) {
                try {
                    using (Stream dst = File.Create(file))
                        CopyStream(src, dst);
                } catch {
                    try {
                        Directory.CreateDirectory("./" + entry.Substring(0, entry.LastIndexOfAny("\\/".ToCharArray())));
                        using (Stream dst = File.Create(file))
                            CopyStream(src, dst);
                    } catch (IOException e) {
                        Logger.LogError(e);
                        Logger.Log(LogType.Warning, "Caught ignoreable Error. See log for more details. Will continue with rest of files.");
                        errors++;
                    }
                }
            }
        }
    }
}
