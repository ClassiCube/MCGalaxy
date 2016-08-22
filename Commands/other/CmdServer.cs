/*
    Copyright 2011 MCForge
        
    Dual-licensed under the    Educational Community License, Version 2.0 and
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

namespace MCGalaxy.Commands {
    public sealed class CmdServer : Command {
        public override string name { get { return "server"; } }
        public override string shortcut { get { return "serv"; } }
        public override string type { get { return CommandTypes.Moderation; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Admin; } }
        public CmdServer() { }

        public override void Use(Player p, string message) {
            string[] args = message.Split(' ');
            switch (args[0].ToLower()) {
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
                    string type = args.Length == 1 ? "" : args[1].ToLower();
                    if (type == "" || type == "all") {
                        // Backup Everything.
                        //   Create SQL statements for this.  The SQL will assume the settings for the current configuration are correct.
                        //   This means we use the currently defined port, database, user, password, and pooling.
                        // Also important to save everything to a .zip file (Though we can rename the extention.)
                        // When backing up, one option is to save all non-main program files.
                        //    This means all folders, and files in these folders.
                        Player.Message(p, "Server backup (Everything) started. Please wait while backup finishes.");
                        Save(true, true, p);
                    } else if (type == "database" || type == "sql" || type == "db") {
                        // Backup database only.
                        //   Create SQL statements for this.  The SQL will assume the settings for the current configuration are correct.
                        //   This means we use the currently defined port, database, user, password, and pooling.
                        // Also important to save everything to a .zip file (Though we can rename the extention.)
                        // When backing up, one option is to save all non-main program files.
                        //    This means all folders, and files in these folders.
                        Player.Message(p, "Server backup (Database) started. Please wait while backup finishes.");
                        Save(false, true, p);
                    } else if (type == "allbutdb" || type == "files" || type == "file") {
                        // Important to save everything to a .zip file (Though we can rename the extention.)
                        // When backing up, one option is to save all non-main program files.
                        //    This means all folders, and files in these folders.
                        Player.Message(p, "Server backup (Everything but Database) started. Please wait while backup finishes.");
                        Save(true, false, p);
                    } else if (type == "lite") {
                        Player.Message(p, "Server backup (Everything but BlockDB tables and undo files) started. Please wait while backup finishes.");
                        Save(true, true, p, true);
                    } else if (type == "table") {
                        if (args.Length == 2) { Player.Message(p, "You need to provide the table name to backup."); return; }
                        if (!ValidName(p, args[2], "table")) return;                        
                        if (!Database.TableExists(args[2])) { Player.Message(p, "Table \"{0}\" does not exist.", args[2]); return; }
                        
                        Player.Message(p, "Backing up table {0} started. Please wait while backup finishes.", args[2]);
                        using (StreamWriter sql = new StreamWriter(args[2] + ".sql"))
                            Backup.BackupTable(args[2], sql);
                        Player.Message(p, "Finished backing up table {0}.", args[2]);
                    } else {
                        Help(p);
                    }
                    break;
                case "restore":
                    if (!CheckPerms(p)) {
                        Player.Message(p, "Only Console or the Server Owner can restore the server.");
                        return;
                    }
                    Thread extract = new Thread(new ParameterizedThreadStart(Backup.ExtractPackage));
                    extract.Name = "MCG_RestoreServer";
                    extract.Start(p);
                    break;
                default:
                    if (message != "")
                        Player.Message(p, "/server " + message + " is not currently implemented.");
                    Help(p);
                    break;
            }
        }

        static bool CheckPerms(Player p) {
            if (p == null) return true;
            if (Server.server_owner.CaselessEq("Notch")) return false;
            return p.name.CaselessEq(Server.server_owner);
        }

        static void Save(bool withFiles, bool withDB, Player p, bool lite = false) {
            Thread worker = new Thread(Backup.CreatePackage);
            worker.Name = "MCG_SaveServer";
            
            Backup.BackupArgs args = new Backup.BackupArgs();
            args.p = p; args.Lite = lite;
            args.Files = withFiles; args.Database = withDB;
            worker.Start(args);
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

        public override void Help(Player p) {
            Player.Message(p, "%T/server reset %H- Reset everything to defaults. (Owner only)");
            Player.Message(p, "  &cWARNING: This will erase ALL properties. Use with caution. (Likely requires restart)");
            Player.Message(p, "%T/server reload %H- Reload the server files. (May require restart) (Owner only)");
            Player.Message(p, "%T/server public/private %H- Make the server public or private.");
            Player.Message(p, "%T/server restore %H- Restore the server from a backup.");
            Player.Message(p, "%T/server backup [all/db/files/lite] %H- Make a backup. (Default all)");
            Player.Message(p, "%HBackup options:");
            Player.Message(p, "  %Hall - Make a backup of the server and all SQL data.");
            Player.Message(p, "  %Hdb - Just backup the database.");
            Player.Message(p, "  %Hfiles - Backup everything BUT the database.");
            Player.Message(p, "  %Hlite - Backups everything, except BlockDB and undo files.");
            Player.Message(p, "%T/server backup table [name] %H- Backups that database table");
        }
    }
}
