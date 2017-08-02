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

        const string path = "MCGalaxy.zip";
        public class BackupArgs {
            public Player p;
            public bool Files, Database, Lite;
        }
        
        public static void CreatePackage(Player p, bool files, bool db, bool lite) {
            if (db) {
                Logger.Log(LogType.SystemActivity, "Backing up the database...");
                using (StreamWriter sql = new StreamWriter("SQL.sql"))
                    BackupDatabase(sql,lite);
                Logger.Log(LogType.SystemActivity, "Backed up the database to SQL.sql");
            }
            
            List<Uri> filesList = null;
            if (files) {
                Logger.Log(LogType.SystemActivity, "Determining which files to backup...");
                string dir = Directory.GetCurrentDirectory() + "\\";
                filesList = GetAllFiles(new DirectoryInfo("./"), new Uri(dir), lite);
                Logger.Log(LogType.SystemActivity, "Finished determining included files");
            }

            Logger.Log(LogType.SystemActivity, "Creating compressed backup...");
            using (ZipPackage package = (ZipPackage)ZipPackage.Open(path, FileMode.Create)) {
                if (files) {
                    Logger.Log(LogType.SystemActivity, "Compressing files...");
                    SaveFiles(package, filesList);
                }
                
                if (db) SaveDatabase(package);
                Logger.Log(LogType.SystemActivity, "Compressed all data!");
            }
            Player.Message(p, "Backup of (" + (files ? "everything" + (db ? "" : " but database") : "database") + ") complete!");
            Logger.Log(LogType.SystemActivity, "Server backed up!");
        }

        const string undo1 = "extra/undo/", undo2 = @"extra\undo\";
        const string prev1 = "extra/undoPrevious/", prev2 = @"extra\undoPrevious\";
        const string levelBackup1 = "levels/backups/", levelBackup2 = @"levels\backups\";
        const string levelPrev1 = "levels/prev/", levelPrev2 = @"levels\prev\";
        const string blockDB1 = "blockdb/", blockDB2 = @"blockdb\";
        
        static List<Uri> GetAllFiles(DirectoryInfo dir, Uri baseUri, bool lite) {
            List<Uri> list = new List<Uri>();
            foreach (FileSystemInfo entry in dir.GetFileSystemInfos()) {
                if (entry is FileInfo) {
                    string path = ((FileInfo)entry).FullName;
                    if (lite && (path.Contains(undo1) || path.Contains(undo2))) continue;
                    if (lite && (path.Contains(prev1) || path.Contains(prev2))) continue;
                    if (lite && (path.Contains(levelBackup1) || path.Contains(levelBackup2))) continue;
                    if (lite && (path.Contains(levelPrev1) || path.Contains(levelPrev2))) continue;
                    if (lite && (path.Contains(blockDB1) || path.Contains(blockDB2))) continue;
                    
                    // Make a relative URI
                    Uri uri = baseUri.MakeRelativeUri(new Uri(path));
                    if (uri.ToString().IndexOfAny("/\\".ToCharArray()) > 0)
                        list.Add(PackUriHelper.CreatePartUri(uri));
                } else {
                    list.AddRange(GetAllFiles((DirectoryInfo)entry, baseUri, lite));
                }
            }
            return list;
        }
        
        static void SaveFiles(ZipPackage package, List<Uri> partURIs) {
            foreach (Uri loc in partURIs) {
                string file = Uri.UnescapeDataString(loc.ToString());
                if (file.Contains(path)) continue;
                
                try {
                    PackagePart part = package.CreatePart(loc, "");
                    using (Stream src = new FileStream("./" + file, FileMode.Open, FileAccess.Read))
                        CopyStream(src, part.GetStream());
                } catch (Exception ex) {
                    Logger.Log(LogType.Warning, "Failed to save file: " + file);
                    Logger.LogError(ex);
                }
            }
        }
        
        static void SaveDatabase(ZipPackage package) {
            Logger.Log(LogType.SystemActivity, "Compressing Database...");
            Uri uri = new Uri("/SQL.sql", UriKind.Relative);
            
            PackagePart part = package.CreatePart(uri, "", CompressionOption.Normal);
            CopyStream(File.OpenRead("SQL.sql"), part.GetStream());
            Logger.Log(LogType.SystemActivity, "Database compressed");
        }

        static void CopyStream(Stream source, Stream target) {
            const int bufSize = 0x1000;
            byte[] buf = new byte[bufSize];
            int bytesRead = 0;
            while ((bytesRead = source.Read(buf, 0, bufSize)) > 0)
                target.Write(buf, 0, bytesRead);
        }

        public static void ExtractPackage(Player p) {
            int errors = 0;
            using (FileStream src = File.OpenRead(path))
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
            Command.all.FindByName("Server").Use(null, "reload"); // Reload, as console
            Player.Message(p, "Server restored" + (errors > 0 ? " with errors.  May be a partial restore" : "") + ".  Restart is reccommended, though not required.");
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
