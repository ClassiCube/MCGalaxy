/*
    Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCGalaxy)
    
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
using System.IO;
using System.Text;

namespace MCGalaxy.Commands.World {
    public sealed class CmdRestore : Command {        
        public override string name { get { return "Restore"; } }
        public override string type { get { return CommandTypes.World; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        public override bool MessageBlockRestricted { get { return true; } }
        
        public override void Use(Player p, string message) {
            if (message.Length == 0) { OutputBackups(p); return; }
            
            Level lvl;
            string[] args = message.SplitSpaces();
            if (args.Length >= 2) {
                lvl = Matcher.FindLevels(p, args[1]);
                if (lvl == null) return;
            } else {
                if (p != null && p.level != null) {
                    lvl = p.level;
                } else {
                    Player.Message(p, "You must provide a level name when using /restore from console.");
                    return;
                }
            }

            if (!LevelInfo.ValidateAction(p, lvl.name, "restore a backup of this level")) return;
            if (File.Exists(LevelInfo.BackupFilePath(lvl.name, args[0]))) {
                try {
                    DoRestore(lvl, args[0]);
                } catch (Exception ex) {
                    Logger.LogError(ex);
                    Logger.Log(LogType.Warning, "Restore failed");
                }
            } else { 
                Player.Message(p, "Backup " + args[0] + " does not exist."); 
            }
        }
        
        static void DoRestore(Level lvl, string backup) {
            lock (lvl.saveLock) {
                File.Copy(LevelInfo.BackupFilePath(lvl.name, backup), LevelInfo.MapPath(lvl.name), true);
                lvl.SaveChanges = false;
            }
            
            Level restore = Level.Load(lvl.name);
            if (restore != null) {
                LevelActions.Replace(lvl, restore);
            } else {
                Logger.Log(LogType.Warning, "Restore nulled");
                File.Copy(LevelInfo.MapPath(lvl.name) + ".backup", LevelInfo.MapPath(lvl.name), true);
            }
        }
        
        static void OutputBackups(Player p) {
            string backupPath = LevelInfo.BackupBasePath(p.level.name);
            if (!Directory.Exists(backupPath)) {
                Player.Message(p, p.level.ColoredName + " %Shas no backups yet."); return;
            }
            
            string[] dirs = Directory.GetDirectories(backupPath);
            Player.Message(p, p.level.ColoredName + " %Shas &b" + dirs.Length + " %Sbackups.");
            int count = 0;
            StringBuilder custom = new StringBuilder();
            
            foreach (string path in dirs) {
                string name = LevelInfo.BackupNameFrom(path);
                int num;
                if (int.TryParse(name, out num)) continue;
                
                count++;
                custom.Append(", " + name);
            }

            if (count == 0) return;
            Player.Message(p, "&b" + count + " %Sof these are custom-named restores:");
            Player.Message(p, custom.ToString(2, custom.Length - 2));
        }

        public override void Help(Player p) {
            Player.Message(p, "%T/Restore %H- lists all backups for the current map");
            Player.Message(p, "%T/Restore [number] [name]");
            Player.Message(p, "%HRestores a previous backup for the given map.");
            Player.Message(p, "%H  If [name] is not given, your current map is used.");
        }
    }
}
