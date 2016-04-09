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
                    Player.SendMessage(p, "Server is now public!");
                    Server.s.Log("Server is now public!");
                    break;
                case "private":
                    Server.pub = false;
                    Player.SendMessage(p, "Server is now private!");
                    Server.s.Log("Server is now private!");
                    break;
                case "reset":  //made so ONLY the owner or console can use this command.
                    if (p != null && !Server.server_owner.ToLower().Equals(p.name.ToLower()) || Server.server_owner.Equals("Notch"))
                    {
                        Player.SendMessage(p, "Sorry. You must be the Server Owner or Console to reset the server.");
                        return;
                    }
                    //restting to default properties is dangerous... but recoverable.
                    //We save the old files to <name>.bkp, then delete them.
                    //Files needed:
                    //  Property files
                    //    Group
                    //    Server
                    //    Rank
                    //    Command
                    Player.SendMessage(p, "Backing up and deleting current property files.");
                    foreach (string name in Directory.GetFiles("properties"))
                    {
                        File.Copy(name, name + ".bkp"); // create backup first.
                        File.Delete(name);
                    }
                    Player.SendMessage(p, "Done!  Restoring defaults...");
                    //We set he defaults here, then go to reload the settings.
                    setToDefault();
                    goto case "reload";
                case "reload":  // For security, only the owner and Console can use this.
                    if (p != null && !Server.server_owner.ToLower().Equals(p.name.ToLower()) || Server.server_owner.Equals("Notch"))
                    {
                        p.SendMessage("Sorry.  You must be the Server Owner or Console to reload the server settings.");
                        return;
                    }
                    Player.SendMessage(p, "Reloading settings...");
                    Server.LoadAllSettings();
                    Player.SendMessage(p, "Settings reloaded!  You may need to restart the server, however.");
                    break;
                case "backup":
                case "backup all":
                    // Backup Everything.
                    //   Create SQL statements for this.  The SQL will assume the settings for the current configuration are correct.
                    //   This means we use the currently defined port, database, user, password, and pooling.
                    // Also important to save everything to a .zip file (Though we can rename the extention.)
                    // When backing up, one option is to save all non-main program files.
                    //    This means all folders, and files in these folders.
                    Player.SendMessage(p, "Server backup (Everything): Started.\n\tPlease wait while backup finishes.");
                    Save(true, p);
                    break;
                case "backup db":
                    // Backup database only.
                    //   Create SQL statements for this.  The SQL will assume the settings for the current configuration are correct.
                    //   This means we use the currently defined port, database, user, password, and pooling.
                    // Also important to save everything to a .zip file (Though we can rename the extention.)
                    // When backing up, one option is to save all non-main program files.
                    //    This means all folders, and files in these folders.
                    Player.SendMessage(p, "Server backup (Database): Started.\n\tPlease wait while backup finishes.");
                    Save(false, true, p);
                    break;
                case "backup allbutdb":
                    // Important to save everything to a .zip file (Though we can rename the extention.)
                    // When backing up, one option is to save all non-main program files.
                    //    This means all folders, and files in these folders.
                    Player.SendMessage(p, "Server backup (Everything but Database): Started.\n\tPlease wait while backup finishes.");
                    Save(false, p);
                    break;
                case "restore":
                    if (p != null && !Server.server_owner.ToLower().Equals(p.name.ToLower()) || Server.server_owner.Equals("Notch"))
                    {
                        p.SendMessage("Sorry.  You must be the defined Server Owner or Console to restore the server.");
                        return;
                    }
                    Thread extract = new Thread(new ParameterizedThreadStart(ExtractPackage));
                    extract.Name = "MCG_RestoreServer";
                    extract.Start(p);
                    break;
                default:
                    Player.SendMessage(p, "/server " + message + " is not currently implemented.");
                    goto case "";
                //case "help":
                //    Help(p);
                //    break;
            }
        }

        private void Save(bool withDB, Player p)
        {
            Save(true, withDB, p);
        }

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

        private void setToDefault()
        { // could do this elsewhere, but will worry about that later.
            //Other
            Server.higherranktp = true;
            Server.agreetorulesonentry = false;

            Server.tempBans = new List<Server.TempBan>();

            Server.afkset = new List<string>();
            Server.ircafkset = new List<string>();
            Server.messages = new List<string>();

            Server.restartcountdown = "";
            Server.selectedrevision = "";

            Server.chatmod = false;

            //Global VoteKick In Progress Flag
            Server.voteKickInProgress = false;
            Server.voteKickVotesNeeded = 0;

            //Zombie
            Server.zombie.ResetState();
            Server.zombie.StartImmediately = false;
            Server.zombie.noRespawn = true;
            Server.zombie.noPillaring = true;
            Server.zombie.ZombieName = "";
            Server.zombie.ChangeLevels = true;
            Server.zombie.LevelList.Clear();
            Server.zombie.SetMainLevel = false;
            //Settings
            #region Server Settings
            Server.salt = "";

            Server.name = "[MCGalaxy] Default";
            Server.motd = "Welcome!";
            Server.players = 12;
            //for the limiting no. of guests:
            Server.maxGuests = 10;

            Server.port = 25565;
            Server.pub = true;
            Server.verify = true;
            Server.worldChat = true;
            //            Server.guestGoto = false; //Never used

            Server.autorestart = false;
            Server.restarttime = DateTime.Now;

            //Spam Prevention
            Server.checkspam = false;
            Server.spamcounter = 8;
            Server.mutespamtime = 60;
            Server.spamcountreset = 5;

            Server.ZallState = "Alive";

            Server.level = "main";
            Server.errlog = "error.log";

            //Server.console = false;
            Server.reportBack = true;

            Server.irc = false;
            Server.ircColorsEnable = false;
            //            Server.safemode = false;
            Server.ircPort = 6667;
            Server.ircNick = "ForgeBot";
            Server.ircServer = "irc.esper.net";
            Server.ircChannel = "#changethis";
            Server.ircOpChannel = "#changethistoo";
            Server.ircIdentify = false;
            Server.ircPassword = "";
            Server.verifyadmins = true;
            Server.verifyadminsrank = LevelPermission.Operator;

            Server.restartOnError = true;

            Server.Overload = 1500;
            Server.rpLimit = 500;
            Server.rpNormLimit = 10000;

            Server.backupLocation = System.Windows.Forms.Application.StartupPath + "/levels/backups";
            Server.backupInterval = 300;
            Server.blockInterval = 60;

            Server.physicsRestart = true;
            Server.deathcount = true;
            Server.AutoLoad = false;
            Server.physUndo = 20000;
            Server.totalUndo = 200;
            Server.parseSmiley = true;
            Server.useWhitelist = false;
            Server.forceCuboid = false;
            Server.profanityFilter = false;
            Server.repeatMessage = false;
            Server.DrawReloadLimit = 10000;
            Server.MapGenLimit = 30 * 1000 * 1000;
            Server.MapGenLimitAdmin = 225 * 1000 * 1000;

            Server.checkUpdates = true;

            Server.useMySQL = false;
            Server.MySQLHost = "127.0.0.1";
            Server.MySQLPort = "3306";
            Server.MySQLUsername = "root";
            Server.MySQLPassword = "password";
            Server.MySQLDatabaseName = "MCZallDB";
            Server.DatabasePooling = true;

            Server.DefaultColor = "&e";
            Server.IRCColour = "&5";
            Server.GlobalChatColor = "&6";
            Server.HelpSyntaxColor = "&a";
            Server.HelpDescriptionColor = "&e";
            Server.UseGlobalChat = true;

            Server.afkminutes = 10;
            Server.afkkick = 45;
            //Server.RemotePort = 1337;

            Server.defaultRank = "guest";

            Server.dollarNames = true;
            Server.unsafe_plugin = true;
            Server.cheapMessage = true;
            Server.cheapMessageGiven = " is now being cheap and being immortal";
            Server.defaultBanMessage = "You're banned!";
            Server.shutdownMessage = "Server shutdown. Rejoin in 10 seconds.";
            Server.defaultPromoteMessage = "&6Congratulations for working hard and getting &2PROMOTED!";
            Server.defaultDemoteMessage = "&4DEMOTED! &6We're sorry for your loss. Good luck on your future endeavors! &1:'(";
            Server.moneys = "moneys";
            Server.opchatperm = LevelPermission.Operator;
            Server.adminchatperm = LevelPermission.Admin;
            Server.logbeat = false;
            Server.adminsjoinsilent = false;
            //Server.mono = false;
            Server.server_owner = "Notch";

            Server.flipHead = false;

            Server.shuttingDown = false;
            Server.restarting = false;

            //hackrank stuff
            Server.hackrank_kick = true;
            Server.hackrank_kick_time = 5; //seconds, it converts it to milliseconds in the command.

            Server.showEmptyRanks = false;

            #endregion
        }

        public override void Help(Player p)
        {
            Player.SendMessage(p, "/server <reset|restart|reload|update|shutdown|public|private|backup|restore> - All server commands.");
            Player.SendMessage(p, "/server <reset>    - Reset everything to defaults. (Owner only)  WARNING: This will erase ALL properties.  Use with caution. (Likely requires restart)");
            Player.SendMessage(p, "/server <restart>  - Restart the server.");
            Player.SendMessage(p, "/server <reload>   - Reload the server files. (May require restart) (Owner only)");
            Player.SendMessage(p, "/server <update>   - Update the server");
            Player.SendMessage(p, "/server <shutdown> - Shutdown the server");
            Player.SendMessage(p, "/server <public>   - Make the server public. (Start listening for new connections.)");
            Player.SendMessage(p, "/server <private>  - Make the server private. (Stop listening for new connections.)");
            Player.SendMessage(p, "/server <restore>  - Restore the server from a backup.");
            Player.SendMessage(p, "/server <backup> [all/db/allbutdb] - Make a backup. (Default is all)");
            Player.SendMessage(p, "Backup options:");
            Player.SendMessage(p, "all      - Make a backup of the server and all SQL data. (Default)");
            Player.SendMessage(p, "db       - Just backup the database.");
            Player.SendMessage(p, "allbutdb - Backup everything BUT the database.");
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
            Player.SendMessage(p, "Server backup (" + (withFiles ? "Everything" + (withDB ? "" : " but Database") : "Database") + "): Complete!");
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
            Player.SendMessage((Player)p, "Server restored" + (errors > 0 ? " with errors.  May be a partial restore" : "") + ".  Restart is reccommended, though not required.");
        }
    }
}
