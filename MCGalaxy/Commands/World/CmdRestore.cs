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
using System.IO;
using System.Text;

namespace MCGalaxy.Commands.World {
    public sealed class CmdRestore : Command {        
        public override string name { get { return "restore"; } }
        public override string type { get { return CommandTypes.World; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        public CmdRestore() { }

        public override void Use(Player p, string message) {
            if (message == "") { OutputBackups(p); return; }
            
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

            if (File.Exists(LevelInfo.BackupPath(lvl.name, args[0]))) {
                try {
                    DoRestore(lvl, args[0]);
                } catch {
                    Server.s.Log("Restore fail");
                }
            } else { 
                Player.Message(p, "Backup " + args[0] + " does not exist."); 
            }
        }
        
        static void DoRestore(Level lvl, string backup) {
            lock (lvl.saveLock) {
                File.Copy(LevelInfo.BackupPath(lvl.name, backup), LevelInfo.MapPath(lvl.name), true);
                lvl.saveLevel = false;
            }
            
            Level restore = Level.Load(lvl.name);
            if (restore != null) {
                LevelActions.Replace(lvl, restore);
            } else {
                Server.s.Log("Restore nulled");
                File.Copy(LevelInfo.MapPath(lvl.name) + ".backup", LevelInfo.MapPath(lvl.name), true);
            }
        }
        
        static void OutputBackups(Player p) {
            if (!Directory.Exists(Server.backupLocation + "/" + p.level.name)) {
                Player.Message(p, p.level.ColoredName + " %Shas no backups yet."); return;
            }
            
            string[] dirs = Directory.GetDirectories(Server.backupLocation + "/" + p.level.name);
            Player.Message(p, p.level.ColoredName + " %Shas &b" + dirs.Length + " %Sbackups.");
            int count = 0;
            StringBuilder custom = new StringBuilder();
            
            foreach (string s in dirs) {
                string name = s.Substring(s.LastIndexOf(Path.DirectorySeparatorChar) + 1);
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
            Player.Message(p, "%T/restore %H- lists all backups for the current map");
            Player.Message(p, "%T/restore [number] [name]");
            Player.Message(p, "%HRestores a previous backup for the given map.");
            Player.Message(p, "%H  If [name] is not given, your current map is used.");
        }
    }
}
