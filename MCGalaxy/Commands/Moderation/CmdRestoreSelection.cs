/*
    Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCGalaxy)
    
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
using MCGalaxy.Drawing;
using MCGalaxy.Drawing.Ops;
using MCGalaxy.DB;
using MCGalaxy.Levels.IO;

namespace MCGalaxy.Commands.Moderation {    
    public sealed class CmdRestoreSelection : Command {        
        public override string name { get { return "rs"; } }
        public override string shortcut { get { return "restoreselection"; } }
        public override string type { get { return CommandTypes.Moderation; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }

        public override void Use(Player p, string message) {
            if (message == "") { Help(p); return; }
            if (!Formatter.ValidName(p, name, "level")) return;
            
            if (LevelInfo.ExistsBackup(p.level.name, message)) {
                p.SendMessage("Select two corners for restore.");
                p.MakeSelection(2, message, DoRestore);
            } else {
                Player.Message(p, "Backup " + message + " does not exist.");
            }
        }
        
        bool DoRestore(Player p, Vec3S32[] marks, object state, byte type, byte extType) {
            string path = LevelInfo.BackupPath(p.level.name, (string)state);
            Level source = IMapImporter.Formats[0].Read(path, "templevel", false);
            
            RestoreSelectionDrawOp op = new RestoreSelectionDrawOp();
            op.Source = source;
            if (DrawOpPerformer.Do(op, null, p, marks)) return false;
            
            // Not high enough draw limit
            source.Dispose();
            return false;
        }

        public override void Help(Player p) {
            Player.Message(p, "%T/restoreselection [backup name]");
            Player.Message(p, "%HRestores a previous backup of the current selection");
        }
    }
}
