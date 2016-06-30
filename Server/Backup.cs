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
            if (args.Database)  {
                Server.s.Log("Saving DB...");
                using (StreamWriter sql = new StreamWriter("SQL.sql"))
                    BackupDatabase(sql, args.Lite);
                Server.s.Log("Saved DB to SQL.sql");
            }

            Server.s.Log("Creating package...");
            using (ZipPackage package = (ZipPackage)ZipPackage.Open(path, FileMode.Create))
            {
                if (args.Files) {
                    Server.s.Log("Collecting Directory structure...");
                    string currDir = Directory.GetCurrentDirectory() + "\\";
                    List<Uri> partURIs = GetAllFiles(new DirectoryInfo("./"), new Uri(currDir));
                    Server.s.Log("Structure complete");

                    Server.s.Log("Saving data...");
                    foreach (Uri loc in partURIs) {
                    	string file = Uri.UnescapeDataString(loc.ToString());
                    	if (args.Lite && file.Contains("extra/undo/")) continue;
                    	if (args.Lite && file.Contains("extra/undoPrevious")) continue;
                    	
                        if (!file.Contains(path)) {
                            // Add the part to the Package
                            ZipPackagePart packagePart = (ZipPackagePart)package.CreatePart(loc, "");

                            // Copy the data to the Document Part
                            using (FileStream stream = new FileStream("./" + file, FileMode.Open, FileAccess.Read))
                                CopyStream(stream, packagePart.GetStream());
                        }
                    }// end:foreach(Uri loc)
                }
                if (args.Database) { // If we don't want to back up database, we don't do this part.
                    Server.s.Log("Compressing Database...");
                    ZipPackagePart packagePart =
                                (ZipPackagePart)package.CreatePart(new Uri("/SQL.sql", UriKind.Relative), "", CompressionOption.Normal);
                    CopyStream(File.OpenRead("SQL.sql"), packagePart.GetStream());
                    Server.s.Log("Database compressed.");
                }// end:if(withFiles)
                Server.s.Log("Data saved!");
            }// end:using (Package package) - Close and dispose package.
            Player.Message(args.p, "Server backup (" + (args.Files ? "Everything" + (args.Database ? "" : " but Database") : "Database") + "): Complete!");
            Server.s.Log("Server backed up!");
        }// end:CreatePackage()

        static List<Uri> GetAllFiles(DirectoryInfo currDir, Uri baseUri) {
            List<Uri> uriList = new List<Uri>();
            foreach (FileSystemInfo entry in currDir.GetFileSystemInfos()) {
                if (entry is FileInfo) {
                    // Make a relative URI
                    Uri fullURI = new Uri(((FileInfo)entry).FullName);
                    Uri relURI = baseUri.MakeRelativeUri(fullURI);
                    if (relURI.ToString().IndexOfAny("/\\".ToCharArray()) > 0) {
                        uriList.Add(PackUriHelper.CreatePartUri(relURI));
                    }
                } else {
                    uriList.AddRange(GetAllFiles((DirectoryInfo)entry, baseUri));
                }
            }
            return uriList;
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
