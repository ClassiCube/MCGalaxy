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
using MCGalaxy.DB;
using MCGalaxy.SQL;

namespace MCGalaxy.Commands.Maintenance {
    public sealed class CmdServer : Command {
        public override string name { get { return "server"; } }
        public override string shortcut { get { return "serv"; } }
        public override string type { get { return CommandTypes.Moderation; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Admin; } }

        public override void Use(Player p, string message) {
            string[] args = message.SplitSpaces();
            switch (args[0].ToLower()) {
                case "public": SetPublic(p, args); break;
                case "private": SetPrivate(p, args); break;
                case "reset": DoReset(p, args); break;
                case "reload": DoReload(p, args); break;
                case "backup": DoBackup(p, args); break;
                case "restore": DoRestore(p, args); break;
                case "import": DoImport(p, args); break;
                case "upgradeblockdb": DoBlockDBUpgrade(p, args); break;
                default: Help(p); break;
            }
        }
        
        void SetPublic(Player p, string[] args) {
            ServerConfig.Public = true;
            Player.Message(p, "Server is now public!");
            Logger.Log(LogType.SystemActivity, "Server is now public!");
        }
        
        void SetPrivate(Player p, string[] args) {
            ServerConfig.Public = false;
            Player.Message(p, "Server is now private!");
            Logger.Log(LogType.SystemActivity, "Server is now private!");
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
                Player.Message(p, "Server backup started. Please wait while backup finishes.");
                Backup.CreatePackage(p, true, true, false);
            } else if (type == "database" || type == "sql" || type == "db") {
                // Creates CREATE TABLE and INSERT statements for all tables and rows in the database
                Player.Message(p, "Database backup started. Please wait while backup finishes.");
                Backup.CreatePackage(p, false, true, false);
            } else if (type == "allbutdb" || type == "files" || type == "file") {
                // Saves all files and folders to a .zip
                Player.Message(p, "All files backup started. Please wait while backup finishes.");
                Backup.CreatePackage(p, true, false, false);
            } else if (type == "lite") {
                Player.Message(p, "Server backup (except BlockDB and undo data) started. Please wait while backup finishes.");
                Backup.CreatePackage(p, true, true, true);
            } else if (type == "litedb") {
                Player.Message(p, "Database backup (except BlockDB tables) started. Please wait while backup finishes.");
                Backup.CreatePackage(p, false, true, true);
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
            Backup.ExtractPackage(p);
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
        
        void DoBlockDBUpgrade(Player p, string[] args) {
            if (args.Length == 1 || !args[1].CaselessEq("confirm")) {
                Player.Message(p, "This will export all the BlockDB tables in the database to more efficient .cbdb files.");
                Player.Message(p, "Note: This is only useful if you have updated from older {0} versions", Server.SoftwareName);
                Player.MessageLines(p, DBUpgrader.CompactMessages);
                Player.Message(p, "Type %T/server upgradeblockdb confirm %Sto begin");
            } else if (DBUpgrader.Upgrading) {
                Player.Message(p, "BlockDB upgrade is already in progress.");
            } else {
                try {
                    DBUpgrader.Lock();
                    DBUpgrader.Upgrade();
                } finally {
                    DBUpgrader.Unlock();
                }
            }
        }
        

        static bool CheckPerms(Player p) {
            if (p == null) return true;
            if (ServerConfig.OwnerName.CaselessEq("Notch")) return false;
            return p.name.CaselessEq(ServerConfig.OwnerName);
        }

        void SetToDefault() {
            foreach (var elem in Server.serverConfig)
                elem.Field.SetValue(null, elem.Attrib.DefaultValue);
            
            Server.messages = new List<string>();
            Server.chatmod = false;

            Server.voteKickInProgress = false;
            Server.voteKickVotesNeeded = 0;

            Server.zombie.End();
            SrvProperties.GenerateSalt();

            ServerConfig.RestartTime = DateTime.Now;
            ServerConfig.MainLevel = "main";

            ServerConfig.BackupDirectory = Path.Combine(Utils.FolderPath, "levels/backups");
            ServerConfig.BlockDBSaveInterval = 60;

            ServerConfig.unsafe_plugin = true;
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
            Player.Message(p, "%T/server backup all/db/files/lite/litedb %H- Make a backup.");
            Player.Message(p, "  %Hall - Backups everything (default)");
            Player.Message(p, "  %Hdb - Only backups the database.");
            Player.Message(p, "  %Hfiles - Backups everything, except the database.");
            Player.Message(p, "  %Hlite - Backups everything, except BlockDB and undo files.");
            Player.Message(p, "  %Hlitedb - Backups database, except BlockDB tables.");
            Player.Message(p, "%T/server backup table [name] %H- Backups that database table");
            Player.Message(p, "%T/server import [name] %H- Imports a backed up database table");
            Player.Message(p, "%T/server upgradeblockdb %H- Dumps BlockDB tables from database");
        }
    }
}
