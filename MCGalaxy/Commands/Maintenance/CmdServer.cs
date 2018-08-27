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
    public sealed class CmdServer : Command2 {
        public override string name { get { return "Server"; } }
        public override string shortcut { get { return "Serv"; } }
        public override string type { get { return CommandTypes.Moderation; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Admin; } }

        public override void Use(Player p, string message, CommandData data) {
            string[] args = message.SplitSpaces();
            switch (args[0].ToLower()) {
                    case "public": SetPublic(p, args); break;
                    case "private": SetPrivate(p, args); break;
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
            p.Message("Server is now public!");
            Logger.Log(LogType.SystemActivity, "Server is now public!");
        }
        
        void SetPrivate(Player p, string[] args) {
            ServerConfig.Public = false;
            p.Message("Server is now private!");
            Logger.Log(LogType.SystemActivity, "Server is now private!");
        }
        
        void DoReload(Player p, string[] args) {
            p.Message("Reloading settings...");
            Server.LoadAllSettings();
            Server.LoadPlayerLists();
            p.Message("Settings reloaded! You may need to restart the server, however.");
        }
        
        void DoBackup(Player p, string[] args) {
            string type  = args.Length > 1 ? args[1] : "";
            string value = args.Length > 2 ? args[2] : "";
            
            if (type.CaselessEq("table")) {
                if (value.Length == 0) { p.Message("You need to provide the name of the table to backup."); return; }
                if (!Formatter.ValidName(p, value, "table")) return;
                if (!Database.TableExists(value)) { p.Message("Table \"{0}\" does not exist.", value); return; }
                
                p.Message("Start backing up table {0}. Please wait while backup finishes.", value);
                using (StreamWriter sql = new StreamWriter(value + ".sql")) {
                    Backup.BackupTable(value, sql);
                }
                p.Message("Finished backing up table {0}.", value);
                return;
            } 
            
            bool compress = true;
            if (value.Length > 0 && !CommandParser.GetBool(p, value, ref compress)) return;
            
            if (type.Length == 0 || type.CaselessEq("all")) {
                p.Message("Server backup started. Please wait while backup finishes.");
                Backup.Perform(p, true, true, false, compress);
            } else if (type.CaselessEq("database") || type.CaselessEq("db")) {
                p.Message("Database backup started. Please wait while backup finishes.");
                Backup.Perform(p, false, true, false, compress);
            } else if (type.CaselessEq("files") || type.CaselessEq("file")) {
                p.Message("All files backup started. Please wait while backup finishes.");
                Backup.Perform(p, true, false, false, compress);
            } else if (type.CaselessEq("lite")) {
                p.Message("Server backup (except BlockDB) started. Please wait while backup finishes.");
                Backup.Perform(p, true, true, true, compress);
            } else {
                Help(p);
            }
        }
        
        static void DoRestore(Player p, string[] args) {
            if (!CheckPerms(p)) {
                p.Message("Only Console or the Server Owner can restore the server.");
                return;
            }
            Backup.Extract(p);
        }
        
        void DoImport(Player p, string[] args) {
            if (args.Length == 1) { p.Message("You need to provide the table name to import."); return; }
            if (!Formatter.ValidName(p, args[1], "table")) return;
            if (!File.Exists(args[1] + ".sql")) { p.Message("File \"{0}\".sql does not exist.", args[1]); return; }
            
            p.Message("Importing table {0} started. Please wait while import finishes.", args[1]);
            using (Stream fs = File.OpenRead(args[1] + ".sql"))
                Backup.ImportSql(fs);
            p.Message("Finished importing table {0}.", args[1]);
        }
        
        void DoBlockDBUpgrade(Player p, string[] args) {
            if (args.Length == 1 || !args[1].CaselessEq("confirm")) {
                p.Message("This will export all the BlockDB tables in the database to more efficient .cbdb files.");
                p.Message("Note: This is only useful if you have updated from older {0} versions", Server.SoftwareName);
                p.MessageLines(DBUpgrader.CompactMessages);
                p.Message("Type %T/Server upgradeblockdb confirm %Sto begin");
            } else if (DBUpgrader.Upgrading) {
                p.Message("BlockDB upgrade is already in progress.");
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
            if (p.IsConsole) return true;
            if (ServerConfig.OwnerName.CaselessEq("Notch")) return false;
            return p.name.CaselessEq(ServerConfig.OwnerName);
        }
        
        public override void Help(Player p, string message) {
            if (message.CaselessEq("backup")) {
                p.Message("%T/Server backup [mode] <compress>");
                p.Message("%HMode can be one of the following:");
                p.Message("  &fall %H- Backups everything (default)");
                p.Message("  &fdb %H- Only backups the database");
                p.Message("  &ffiles %H- Backups everything, except the database");
                p.Message("  &flite %H- Backups everything, except BlockDB files");
                p.Message("%H<compress> - Whether to compress the backup (default yes)");
            } else {
                base.Help(p, message);
            }
        }

        public override void Help(Player p) {
            p.Message("%T/Server reload %H- Reloads the server files");
            p.Message("%T/Server public/private %H- Makes the server public or private");
            p.Message("%T/Server restore %H- Restores the server from a backup");
            p.Message("%T/Server backup %H- Make a backup. See %T/help server backup");
            p.Message("%T/Server backup table [name] %H- Backups that database table");
            p.Message("%T/Server import [name] %H- Imports a backed up database table");
            p.Message("%T/Server upgradeblockdb %H- Dumps BlockDB tables from database");
            p.Message("%HOnly useful when upgrading from a very old {0} version", Server.SoftwareName);
        }
    }
}
