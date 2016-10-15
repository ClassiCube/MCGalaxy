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
                case "public": SetPublic(p, args); break;
                case "private": SetPrivate(p, args); break;
                case "reset": DoReset(p, args); break;
                case "reload": DoReload(p, args); break;
                case "backup": DoBackup(p, args); break;
                case "restore": DoRestore(p, args); break;
                case "import": DoImport(p, args); break;
                default: Help(p); break;
            }
        }
        
        void SetPublic(Player p, string[] args) {
            Server.pub = true;
            Player.Message(p, "Server is now public!");
            Server.s.Log("Server is now public!");
        }
        
        void SetPrivate(Player p, string[] args) {
            Server.pub = false;
            Player.Message(p, "Server is now private!");
            Server.s.Log("Server is now private!");
        }
        
        void DoReset(Player p, string[] args) {
            if (!CheckPerms(p)) {
                Player.Message(p, "Only Console or the Server Owner can reset the server."); return;
            }

            Player.Message(p, "Backing up and deleting current property files.");
            foreach (string name in Directory.GetFiles("properties")) {
                File.Copy(name, name + ".bkp"); // create backup first
                File.Delete(name);
            }
            
            Player.Message(p, "Done!  Restoring defaults...");
            SetToDefault();
            DoReload(p, args);
        }
        
        void DoReload(Player p, string[] args) {
            if (!CheckPerms(p)) {
                Player.Message(p, "Only Console or the Server Owner can reload the server settings."); return;
            }
            Player.Message(p, "Reloading settings...");
            Server.LoadAllSettings();
            Player.Message(p, "Settings reloaded!  You may need to restart the server, however.");
        }
        
        void DoBackup(Player p, string[] args) {
            string type = args.Length == 1 ? "" : args[1].ToLower();
            if (type == "" || type == "all") {
                Player.Message(p, "Server backup (Everything) started. Please wait while backup finishes.");
                Save(true, true, p);
            } else if (type == "database" || type == "sql" || type == "db") {
                // Creates CREATE TABLE and INSERT statements for all tables and rows in the database
                Player.Message(p, "Server backup (Database) started. Please wait while backup finishes.");
                Save(false, true, p);
            } else if (type == "allbutdb" || type == "files" || type == "file") {
                // Saves all files and folders to a .zip
                Player.Message(p, "Server backup (Everything but Database) started. Please wait while backup finishes.");
                Save(true, false, p);
            } else if (type == "lite") {
                Player.Message(p, "Server backup (Everything but BlockDB tables and undo files) started. Please wait while backup finishes.");
                Save(true, true, p, true);
            } else if (type == "table") {
                if (args.Length == 2) { Player.Message(p, "You need to provide the table name to backup."); return; }
                if (!Formatter.ValidName(p, args[2], "table")) return;
                if (!Database.TableExists(args[2])) { Player.Message(p, "Table \"{0}\" does not exist.", args[2]); return; }
                
                Player.Message(p, "Backing up table {0} started. Please wait while backup finishes.", args[2]);
                using (StreamWriter sql = new StreamWriter(args[2] + ".sql"))
                    Backup.BackupTable(args[2], sql);
                Player.Message(p, "Finished backing up table {0}.", args[2]);
            } else {
                Help(p);
            }
        }
        
        static void DoRestore(Player p, string[] args) {
            if (!CheckPerms(p)) {
                Player.Message(p, "Only Console or the Server Owner can restore the server.");
                return;
            }
            Thread extract = new Thread(new ParameterizedThreadStart(Backup.ExtractPackage));
            extract.Name = "MCG_RestoreServer";
            extract.Start(p);
        }
        
        void DoImport(Player p, string[] args) {
            if (args.Length == 1) { Player.Message(p, "You need to provide the table name to import."); return; }
            if (!Formatter.ValidName(p, args[1], "table")) return;
            if (!File.Exists(args[1] + ".sql")) { Player.Message(p, "File \"{0}\".sql does not exist.", args[1]); return; }
            
            Player.Message(p, "Importing table {0} started. Please wait while import finishes.", args[1]);
            using (Stream fs = File.OpenRead(args[1] + ".sql"))
                Backup.ImportSql(fs);
            Player.Message(p, "Finished importing table {0}.", args[1]);
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
            Player.Message(p, "%T/server backup all/db/files/lite %H- Make a backup.");
            Player.Message(p, "  %Hall - Backups everything (default)");
            Player.Message(p, "  %Hdb - Only backups the database.");
            Player.Message(p, "  %Hfiles - Backups everything, except the database.");
            Player.Message(p, "  %Hlite - Backups everything, except BlockDB and undo files.");
            Player.Message(p, "%T/server backup table [name] %H- Backups that database table");
            Player.Message(p, "%T/server import [name] %H- Imports a backed up database table");
        }
    }
}
