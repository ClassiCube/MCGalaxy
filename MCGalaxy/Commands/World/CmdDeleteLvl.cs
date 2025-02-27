/*
    Copyright 2011 MCForge
    
    Dual-licensed under the    Educational Community License, Version 2.0 and
    the GNU General Public License, Version 3 (the "Licenses"); you may
    not use this file except in compliance with the Licenses. You may
    obtain a copy of the Licenses at
    
    https://opensource.org/license/ecl-2-0/
    https://www.gnu.org/licenses/gpl-3.0.html
    
    Unless required by applicable law or agreed to in writing,
    software distributed under the Licenses are distributed on an "AS IS"
    BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express
    or implied. See the Licenses for the specific language governing
    permissions and limitations under the Licenses.
 */
using System;

namespace MCGalaxy.Commands.World {
    public sealed class CmdDeleteLvl : Command2 {
        public override string name { get { return "DeleteLvl"; } }
        public override string type { get { return CommandTypes.World; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Admin; } }

        public const string BACKUP_FLAG = "*backup";
        public readonly static CommandAlias BackupAlias = new CommandAlias("DeleteBackup", BACKUP_FLAG);

        public override CommandAlias[] Aliases {
            get { return new[] {
                    new CommandAlias("WDelete"), new CommandAlias("WorldDelete"), new CommandAlias("WRemove"),
                    BackupAlias
                }; }
        }

        public override CommandPerm[] ExtraPerms {
            get { return new[] { new CommandPerm(LevelPermission.Owner, "can delete backups of levels") }; }
        }

        public override bool MessageBlockRestricted { get { return true; } }
        
        public override void Use(Player p, string message, CommandData data) {
            if (message.Length == 0) { Help(p); return; }

            string[] words = message.SplitSpaces(2);
            if (words[0].CaselessEq(BACKUP_FLAG)) {
                if (!CheckExtraPerm(p, data, 1)) return;
                UseBackup(p, words.Length >= 2 ? words[1] : "", false);
                return;
            }

            if (words.Length > 1) { Help(p); return; }
            string map = Matcher.FindMaps(p, message);
            LevelConfig cfg;
            
            if (map == null) return;            
            if (!LevelInfo.Check(p, data.Rank, map, "delete this map",out cfg)) return;

            if (!LevelActions.Delete(p, map)) return;
            Chat.MessageGlobal("Level {0} &Swas deleted", cfg.Color + map);
        }
        /// <summary>
        /// os changes which confirmation text is displayed
        /// </summary>
        public static void UseBackup(Player p, string message, bool os) {
            if (message.Length == 0) { HelpBackup(p); return; }
            string[] words = message.SplitSpaces();
            if (words.Length < 2) {
                //Cannot be seen with os since two args guaranteed provided
                p.Message("You must provide a level name and the backup to delete.");
                p.Message("A backup is usually a number, but may also be named.");
                p.Message("See &T/help restore &7to display backups.");
                return;
            }
            bool confirmed = words.Length == 3 && words[2].CaselessEq("confirm");

            string map = words[0].ToLower();
            string backup = words[1].ToLower();
            if (!confirmed) {
                p.Message("You are about to &Wpermanently delete&S backup \"{0}\" from level \"{1}\"",
                    backup, map);

                if (os) {
                    p.Message("If you are sure, type &T/os delete {0} {1} confirm", BACKUP_FLAG, backup);
                } else {
                    // Don't use message, since they could have typed /deletebackup earth 1 derp
                    // and it should not tell you to type "[...] derp confirm"
                    p.Message("If you are sure, type &T/{0} {1} {2} confirm", BackupAlias.Trigger, map, backup);
                }
                return;
            }

            LevelActions.DeleteBackup(p, map, backup);
        }
        
        public override void Help(Player p) {
            p.Message("&T/DeleteLvl [level]");
            p.Message("&HCompletely deletes [level] (portals, MBs, everything)");
            p.Message("&HA backup of the level is made in the levels/deleted folder");
            HelpBackup(p);
        }
        public static void HelpBackup(Player p) {
            p.Message("&T/DeleteLvl {0} [level] [backup]", BACKUP_FLAG);
            p.Message("&H-Permanently- deletes [backup] of [level].");
        }

    }
}
