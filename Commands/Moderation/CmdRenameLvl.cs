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
using System.IO;
using MCGalaxy.SQL;

namespace MCGalaxy.Commands {
    public sealed class CmdRenameLvl : Command {
        public override string name { get { return "renamelvl"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.Moderation; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Admin; } }
        public CmdRenameLvl() { }

        public override void Use(Player p, string message) {
            if (message == "" || message.IndexOf(' ') == -1) { Help(p); return; }
            string[] args = message.Split(' ');
            Level lvl = LevelInfo.FindOrShowMatches(p, args[0]);
            if (lvl == null) return;
            string newName = args[1];
            if (!Player.ValidName(newName)) {
                Player.Message(p, "\"" + newName + "\" is not a valid level name."); return;
            }
            
            if (LevelInfo.ExistsOffline(newName)) { Player.Message(p, "Level already exists."); return; }
            if (lvl == Server.mainLevel) { Player.Message(p, "Cannot rename the main level."); return; }
            lvl.Unload();

            File.Move(LevelInfo.LevelPath(lvl.name), LevelInfo.LevelPath(newName));
            try {
                File.Move(LevelInfo.LevelPath(lvl.name) + ".backup", LevelInfo.LevelPath(newName) + ".backup");
            } catch {
            }
            
            try {
                File.Move("levels/level properties/" + lvl.name + ".properties", "levels/level properties/" + newName + ".properties");
            } catch {
            }
            
            try {
                File.Move("levels/level properties/" + lvl.name, "levels/level properties/" + newName + ".properties");
            } catch {
            }
            
            try {
                if (File.Exists("blockdefs/lvl_" + lvl.name + ".json"))
                    File.Move("blockdefs/lvl_" + lvl.name + ".json", "blockdefs/lvl_" + newName + ".json");
            } catch {
            }

            //Move and rename backups
            try {
                MoveBackups(lvl.name, newName);
            } catch {
            }

            //safe against SQL injections because foundLevel is being checked and,
            //newName is being split and partly checked on illegal characters reserved for Windows.
            if (Server.useMySQL)
                Database.executeQuery(String.Format("RENAME TABLE `Block{0}` TO `Block{1}`, " +
                                                    "`Portals{0}` TO `Portals{1}`, " +
                                                    "`Messages{0}` TO `Messages{1}`, " +
                                                    "`Zone{0}` TO `Zone{1}`", lvl.name.ToLower(), newName.ToLower()));
            else {
                using (BulkTransaction helper = SQLiteBulkTransaction.Create()) { // ensures that it's either all work, or none work.
                    helper.Execute(String.Format("ALTER TABLE `Block{0}` RENAME TO `Block{1}`", lvl.name.ToLower(), newName.ToLower()));
                    helper.Execute(String.Format("ALTER TABLE `Portals{0}` RENAME TO `Portals{1}`", lvl.name.ToLower(), newName.ToLower()));
                    helper.Execute(String.Format("ALTER TABLE `Messages{0}` RENAME TO `Messages{1}`", lvl.name.ToLower(), newName.ToLower()));
                    helper.Execute(String.Format("ALTER TABLE `Zone{0}` RENAME TO `Zone{1}`", lvl.name.ToLower(), newName.ToLower()));
                    helper.Commit();
                }
            }
            try { Command.all.Find("load").Use(p, newName); }
            catch { }
            Player.GlobalMessage("Renamed " + lvl.name + " to " + newName);
        }

        static bool DirectoryEmpty(string dir) {
            if (!Directory.Exists(dir))  return true;
            if (Directory.GetDirectories(dir).Length > 0) return false;
            if (Directory.GetFiles(dir).Length > 0) return false;
            return true;
        }
        
        static void MoveBackups(string oldName, string newName) {
            for (int i = 1; ; i++) {
                string oldDir = LevelInfo.BackupPath(oldName, i.ToString());
                string newDir = LevelInfo.BackupPath(newName, i.ToString());

                if (File.Exists(oldDir + oldName + ".lvl")) {
                    Directory.CreateDirectory(newDir);
                    File.Move(oldDir + oldName + ".lvl", newDir + newName + ".lvl");
                    if (DirectoryEmpty(oldDir)) Directory.Delete(oldDir);
                } else {
                    if (DirectoryEmpty(Server.backupLocation + "/" + oldName + "/"))
                        Directory.Delete(Server.backupLocation + "/" + oldName + "/");
                    break;
                }
            }
        }
        
        public override void Help(Player p) {
            Player.Message(p, "/renamelvl <level> <new name> - Renames <level> to <new name>");
            Player.Message(p, "Portals going to <level> will be lost");
        }
    }
}
