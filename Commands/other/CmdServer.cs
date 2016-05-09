/*
	Copyright 2011 MCForge
		
	Dual-licensed under the	Educational Community License, Version 2.0 and
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
using System.Threading;
using MCGalaxy.SQL;
namespace MCGalaxy.Commands
{
    public sealed class CmdServer : Command
    {
        public override string name { get { return "server"; } }
        public override string shortcut { get { return "serv"; } }
        public override string type { get { return CommandTypes.Other; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Admin; } }
        public CmdServer() { }

        public override void Use(Player p, string message)
        {
            switch (message)
            {
                case "": // To prevent '/server' from causing an error message
                    Help(p);
                    break;
                case "restart":
                case "update":
                case "shutdown":
                    Command.all.Find(message).Use(p, ""); //Will use other options later.
                    break;
                case "public":
                    Server.pub = true;
                    Player.Message(p, "Server is now public!");
                    Server.s.Log("Server is now public!");
                    break;
                case "private":
                    Server.pub = false;
                    Player.Message(p, "Server is now private!");
                    Server.s.Log("Server is now private!");
                    break;
                case "reset":  //made so ONLY the owner or console can use this command.
                    if (!CheckPerms(p)) {
                        Player.Message(p, "Only Console or the Server Owner can reset the server."); return;
                    }
                    //restting to default properties is dangerous... but recoverable.
                    //We save the old files to <name>.bkp, then delete them.
                    //Files needed:
                    //  Property files
                    //    Group
                    //    Server
                    //    Rank
                    //    Command
                    Player.Message(p, "Backing up and deleting current property files.");
                    foreach (string name in Directory.GetFiles("properties"))
                    {
                        File.Copy(name, name + ".bkp"); // create backup first.
                        File.Delete(name);
                    }
                    Player.Message(p, "Done!  Restoring defaults...");
                    //We set he defaults here, then go to reload the settings.
                    SetToDefault();
                    goto case "reload";
                case "reload":  // For security, only the owner and Console can use this.
                    if (!CheckPerms(p)) {
                        Player.Message(p, "Only Console or the Server Owner can reload the server settings."); return;
                    }
                    Player.Message(p, "Reloading settings...");
                    Server.LoadAllSettings();
                    Player.Message(p, "Settings reloaded!  You may need to restart the server, however.");
                    break;
                case "backup":
                case "backup all":
                    // Backup Everything.
                    //   Create SQL statements for this.  The SQL will assume the settings for the current configuration are correct.
                    //   This means we use the currently defined port, database, user, password, and pooling.
                    // Also important to save everything to a .zip file (Though we can rename the extention.)
                    // When backing up, one option is to save all non-main program files.
                    //    This means all folders, and files in these folders.
                    Player.Message(p, "Server backup (Everything): Started.\n\tPlease wait while backup finishes.");
                    Save(true, p);
                    break;
                case "backup db":
                    // Backup database only.
                    //   Create SQL statements for this.  The SQL will assume the settings for the current configuration are correct.
                    //   This means we use the currently defined port, database, user, password, and pooling.
                    // Also important to save everything to a .zip file (Though we can rename the extention.)
                    // When backing up, one option is to save all non-main program files.
                    //    This means all folders, and files in these folders.
                    Player.Message(p, "Server backup (Database): Started.\n\tPlease wait while backup finishes.");
                    Save(false, true, p);
                    break;
                case "backup allbutdb":
                    // Important to save everything to a .zip file (Though we can rename the extention.)
                    // When backing up, one option is to save all non-main program files.
                    //    This means all folders, and files in these folders.
                    Player.Message(p, "Server backup (Everything but Database): Started.\n\tPlease wait while backup finishes.");
                    Save(false, p);
                    break;
                case "restore":
                    if (!CheckPerms(p)) {
                        Player.Message(p, "Only Console or the Server Owner can restore the server.");
                        return;
                    }
                    Thread extract = new Thread(new ParameterizedThreadStart(ExtractPackage));
                    extract.Name = "MCG_RestoreServer";
                    extract.Start(p);
                    break;
                default:
                    Player.Message(p, "/server " + message + " is not currently implemented.");
                    goto case "";
                //case "help":
                //    Help(p);
                //    break;
            }
        }

        static bool CheckPerms(Player p) {
            if (p == null) return true;
            if (Server.server_owner.CaselessEq("Notch")) return false;
            return p.name.CaselessEq(Server.server_owner);
        }
        
        private void Save(bool withDB, Player p) { Save(true, withDB, p); }

        private void Save(bool withFiles, bool withDB, Player p)
        {
            ParameterizedThreadStart pts = new ParameterizedThreadStart(CreatePackage);
            Thread doWork = new Thread(new ParameterizedThreadStart(CreatePackage));
            doWork.Name = "MCG_SaveServer";
            List<object> param = new List<object>();
            param.Add("MCGalaxy.zip");
            param.Add(withFiles);
            param.Add(withDB);
            param.Add(p);
            doWork.Start(param);
        }

        void SetToDefault() {
        	foreach (var elem in Server.serverConfig)
        		elem.Field.SetValue(null, elem.Attrib.DefaultValue);

            Server.tempBans = new List<Server.TempBan>();
            Server.ircafkset = new List<string>();
            Server.messages = new List<string>();
            Server.chatmod = false;

            Server.voteKickInProgress = false;
            Server.voteKickVotesNeeded = 0;

            Server.zombie.ResetState();
            Server.salt = "";

            Server.restarttime = DateTime.Now;
            Server.level = "main";

            Server.backupLocation = System.Windows.Forms.Application.StartupPath + "/levels/backups";
            Server.blockInterval = 60;

            Server.unsafe_plugin = true;
            Server.flipHead = false;
            Server.shuttingDown = false;
            Server.restarting = false;
        }

        public override void Help(Player p)
        {
            Player.Message(p, "/server <reset|restart|reload|update|shutdown|public|private|backup|restore> - All server commands.");
            Player.Message(p, "/server <reset>    - Reset everything to defaults. (Owner only)  WARNING: This will erase ALL properties.  Use with caution. (Likely requires restart)");
            Player.Message(p, "/server <restart>  - Restart the server.");
            Player.Message(p, "/server <reload>   - Reload the server files. (May require restart) (Owner only)");
            Player.Message(p, "/server <update>   - Update the server");
            Player.Message(p, "/server <shutdown> - Shutdown the server");
            Player.Message(p, "/server <public>   - Make the server public. (Start listening for new connections.)");
            Player.Message(p, "/server <private>  - Make the server private. (Stop listening for new connections.)");
            Player.Message(p, "/server <restore>  - Restore the server from a backup.");
            Player.Message(p, "/server <backup> [all/db/allbutdb] - Make a backup. (Default is all)");
            Player.Message(p, "Backup options:");
            Player.Message(p, "all      - Make a backup of the server and all SQL data. (Default)");
            Player.Message(p, "db       - Just backup the database.");
            Player.Message(p, "allbutdb - Backup everything BUT the database.");
        }

        private static void CreatePackage(object par)
        {
            List<object> param = (List<object>)par;
            CreatePackage((string)param[0], (bool)param[1], (bool)param[2], (Player)param[3]);
        }

        //  -------------------------- CreatePackage --------------------------
        /// <summary>
        ///   Creates a package zip file containing specified
        ///   content and resource files.</summary>
        private static void CreatePackage(string packagePath, bool withFiles, bool withDB, Player p)
        {

            // Create the Package
            if (withDB)
            {
                Server.s.Log("Saving DB...");
                SaveDatabase("SQL.sql");
                Server.s.Log("Saved DB to SQL.sql");
            }

            Server.s.Log("Creating package...");
            using (ZipPackage package = (ZipPackage)ZipPackage.Open(packagePath, FileMode.Create))
            {

                if (withFiles)
                {
                    Server.s.Log("Collecting Directory structure...");
                    string currDir = Directory.GetCurrentDirectory() + "\\";
                    List<Uri> partURIs = GetAllFiles(new DirectoryInfo("./"), new Uri(currDir));
                    Server.s.Log("Structure complete");

                    Server.s.Log("Saving data...");
                    foreach (Uri loc in partURIs)
                    {
                        if (!Uri.UnescapeDataString(loc.ToString()).Contains(packagePath))
                        {

                            // Add the part to the Package

                            ZipPackagePart packagePart =
                                (ZipPackagePart)package.CreatePart(loc, "");

                            // Copy the data to the Document Part
                            using (FileStream fileStream = new FileStream(
                                    "./" + Uri.UnescapeDataString(loc.ToString()), FileMode.Open, FileAccess.Read))
                            {
                                CopyStream(fileStream, packagePart.GetStream());
                            }// end:using(fileStream) - Close and dispose fileStream.
                        }
                    }// end:foreach(Uri loc)
                }
                if (withDB)
                { // If we don't want to back up database, we don't do this part.
                    Server.s.Log("Compressing Database...");
                    ZipPackagePart packagePart =
                                (ZipPackagePart)package.CreatePart(new Uri("/SQL.sql", UriKind.Relative), "", CompressionOption.Normal);
                    CopyStream(File.OpenRead("SQL.sql"), packagePart.GetStream());
                    Server.s.Log("Database compressed.");
                }// end:if(withFiles)
                Server.s.Log("Data saved!");
            }// end:using (Package package) - Close and dispose package.
            Player.Message(p, "Server backup (" + (withFiles ? "Everything" + (withDB ? "" : " but Database") : "Database") + "): Complete!");
            Server.s.Log("Server backed up!");
        }// end:CreatePackage()

        private static void SaveDatabase(string filename)
        {
            using (StreamWriter sql = new StreamWriter(File.Create(filename)))
            {
                Database.CopyDatabase(sql);
            }
        }

        private static List<Uri> GetAllFiles(DirectoryInfo currDir, Uri baseUri)
        {
            List<Uri> uriList = new List<Uri>();
            foreach (FileSystemInfo entry in currDir.GetFileSystemInfos())
            {
                if (entry is FileInfo)
                {
                    // Make a relative URI
                    Uri fullURI = new Uri(((FileInfo)entry).FullName);
                    Uri relURI = baseUri.MakeRelativeUri(fullURI);
                    if (relURI.ToString().IndexOfAny("/\\".ToCharArray()) > 0)
                    {
                        uriList.Add(PackUriHelper.CreatePartUri(relURI));
                    }
                }
                else
                {
                    uriList.AddRange(GetAllFiles((DirectoryInfo)entry, baseUri));
                }
            }
            return uriList;
        }


        //  --------------------------- CopyStream ---------------------------
        /// <summary>
        ///   Copies data from a source stream to a target stream.</summary>
        /// <param name="source">
        ///   The source stream to copy from.</param>
        /// <param name="target">
        ///   The destination stream to copy to.</param>
        private static void CopyStream(Stream source, Stream target)
        {
            const int bufSize = 0x1000;
            byte[] buf = new byte[bufSize];
            int bytesRead = 0;
            while ((bytesRead = source.Read(buf, 0, bufSize)) > 0)
                target.Write(buf, 0, bytesRead);
        }// end:CopyStream()

        private static void ExtractPackage(object p)
        {
            int errors = 0;
            using (ZipPackage zip = (ZipPackage)ZipPackage.Open(File.OpenRead("MCGalaxy.zip")))
            {
                PackagePartCollection pc = zip.GetParts();
                foreach (ZipPackagePart item in pc)
                {
                    try
                    {
                        CopyStream(item.GetStream(), File.Create("./" + Uri.UnescapeDataString(item.Uri.ToString())));
                    }
                    catch
                    {
                        try
                        {
                            Directory.CreateDirectory("./" + item.Uri.ToString().Substring(0, item.Uri.ToString().LastIndexOfAny("\\/".ToCharArray())));
                            CopyStream(item.GetStream(), File.Create("./" + Uri.UnescapeDataString(item.Uri.ToString())));
                        }
                        catch (IOException e)
                        {
                            Server.ErrorLog(e);
                            Server.s.Log("Caught ignoreable Error.  See log for more details.  Will continue with rest of files.");
                            errors++;
                        }
                    }
                    // To make life easier, we reload settings now, to maker it less likely to need restart
                    Command.all.Find("server").Use(null, "reload"); //Reload, as console
                    if (item.Uri.ToString().ToLower().Contains("sql.sql"))
                    { // If it's in there, they backed it up, meaning they want it restored
                        Database.fillDatabase(item.GetStream());
                    }
                }
            }
            Player.Message((Player)p, "Server restored" + (errors > 0 ? " with errors.  May be a partial restore" : "") + ".  Restart is reccommended, though not required.");
        }
    }
}
