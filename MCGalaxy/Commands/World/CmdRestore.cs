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

namespace MCGalaxy.Commands.World {
    public sealed class CmdRestore : Command2 {        
        public override string name { get { return "Restore"; } }
        public override string type { get { return CommandTypes.World; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        public override bool MessageBlockRestricted { get { return true; } }
        
        public override void Use(Player p, string message, CommandData data) {
            if (message.Length == 0) { LevelInfo.OutputBackups(p, p.level.MapName); return; }
            
            Level lvl;
            string[] args = message.SplitSpaces();
            if (args.Length >= 2) {
                lvl = Matcher.FindLevels(p, args[1]);
                if (lvl == null) return;
            } else {
                if (p.IsSuper) {
                    SuperRequiresArgs(p, "level name"); return;
                }
                lvl = p.level;
            }

            if (!LevelInfo.Check(p, data.Rank, lvl, "restore a backup of this level")) return;
            string path = LevelInfo.BackupFilePath(lvl.name, args[0]);
            
            if (File.Exists(path)) {
                try {
                    DoRestore(lvl, args[0]);
                } catch (Exception ex) { 
                    Logger.LogError("Error restoring map", ex); 
                }
            } else {
                p.Message("Backup {0} does not exist.", args[0]); 
                LevelInfo.OutputBackups(p, lvl.name);
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

        public override void Help(Player p) {
            p.Message("&T/Restore &H- lists all backups for the current level");
            p.Message("&T/Restore [number] <level>");
            p.Message("&HRestores a previous backup for the given level.");
            p.Message("&H  If <level> is not given, the current level is used.");
        }
    }
}
