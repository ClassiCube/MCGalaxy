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
        
        public static void CreatePackage(object p) {
            BackupArgs args = (BackupArgs)p;
            try {
                CreatePackageCore(args);
            } catch (Exception ex) {
                Server.ErrorLog(ex);
                Server.s.Log("Error while trying to perform backup.");
                Player.Message(args.p, "Error while trying to perform backup.");
            }
        }
        
        static void CreatePackageCore(BackupArgs args) {
            if (args.Database)  {
                Server.s.Log("Saving DB...");
                using (StreamWriter sql = new StreamWriter("SQL.sql"))
                    BackupDatabase(sql, args.Lite);
                Server.s.Log("Saved DB to SQL.sql");
            }

            Server.s.Log("Creating package...");
            using (ZipPackage package = (ZipPackage)ZipPackage.Open(path, FileMode.Create)) {
                if (args.Files) {
                    Server.s.Log("Collecting Directory structure...");
                    string dir = Directory.GetCurrentDirectory() + "\\";
                    List<Uri> uris = GetAllFiles(new DirectoryInfo("./"), new Uri(dir), args.Lite);
                    Server.s.Log("Structure complete");

                    Server.s.Log("Saving data...");
                    SaveFiles(package, uris);
                }
                
                if (args.Database)
                    SaveDatabase(package);
                Server.s.Log("Data saved!");
            }
            Player.Message(args.p, "Server backup (" + (args.Files ? "Everything" + (args.Database ? "" : " but Database") : "Database") + "): Complete!");
            Server.s.Log("Server backed up!");
        }

        static List<Uri> GetAllFiles(DirectoryInfo dir, Uri baseUri, bool lite) {
            List<Uri> list = new List<Uri>();
            foreach (FileSystemInfo entry in dir.GetFileSystemInfos()) {
                if (entry is FileInfo) {
                    string path = ((FileInfo)entry).FullName;
                    if (lite && path.Contains("extra/undo/")) continue;
                    if (lite && path.Contains("extra/undoPrevious")) continue;
                    
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
                    using (Stream stream = new FileStream("./" + file, FileMode.Open, FileAccess.Read))
                        CopyStream(stream, part.GetStream());
                } catch (Exception ex) {
                    Server.s.Log("Failed to save file: " + file);
                    Server.ErrorLog(ex);
                }
            }
        }
        
        static void SaveDatabase(ZipPackage package) {
            Server.s.Log("Compressing Database...");
            Uri uri = new Uri("/SQL.sql", UriKind.Relative);
            
            PackagePart part = package.CreatePart(uri, "", CompressionOption.Normal);
            CopyStream(File.OpenRead("SQL.sql"), part.GetStream());
            Server.s.Log("Database compressed.");
        }

        static void CopyStream(Stream source, Stream target) {
            const int bufSize = 0x1000;
            byte[] buf = new byte[bufSize];
            int bytesRead = 0;
            while ((bytesRead = source.Read(buf, 0, bufSize)) > 0)
                target.Write(buf, 0, bytesRead);
        }

        public static void ExtractPackage(object p) {
            int errors = 0;
            using (ZipPackage zip = (ZipPackage)ZipPackage.Open(File.OpenRead(path)))  {
                PackagePartCollection pc = zip.GetParts();
                foreach (ZipPackagePart item in pc) {
                    try {
                        CopyStream(item.GetStream(), File.Create("./" + Uri.UnescapeDataString(item.Uri.ToString())));
                    } catch {
                        try {
                            Directory.CreateDirectory("./" + item.Uri.ToString().Substring(0, item.Uri.ToString().LastIndexOfAny("\\/".ToCharArray())));
                            CopyStream(item.GetStream(), File.Create("./" + Uri.UnescapeDataString(item.Uri.ToString())));
                        } catch (IOException e) {
                            Server.ErrorLog(e);
                            Server.s.Log("Caught ignoreable Error. See log for more details. Will continue with rest of files.");
                            errors++;
                        }
                    }
                    
                    // To make life easier, we reload settings now, to maker it less likely to need restart
                    Command.all.Find("server").Use(null, "reload"); //Reload, as console
                    if (item.Uri.ToString().ToLower().Contains("sql.sql"))
                    { // If it's in there, they backed it up, meaning they want it restored
                        Backup.fillDatabase(item.GetStream());
                    }
                }
            }
            Player.Message((Player)p, "Server restored" + (errors > 0 ? " with errors.  May be a partial restore" : "") + ".  Restart is reccommended, though not required.");
        }
    }
}
