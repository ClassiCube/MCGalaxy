/*
    Copyright 2011 MCForge
        
    Dual-licensed under the Educational Community License, Version 2.0 and
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
using System.IO;
using System.Threading;
using MCGalaxy.Levels.IO;

namespace MCGalaxy.Commands.World {
    public sealed class CmdMuseum : Command2 {
        public override string name { get { return "Museum"; } }
        public override string type { get { return CommandTypes.World; } }
        public override bool SuperUseable { get { return false; } }

        const string CURRENT_FLAG = "*current";

        public override void Use(Player p, string message, CommandData data) {
            if (message.Length == 0) { LevelOperations.OutputBackups(p, p.level); return; }

            string[] args = message.ToLower().SplitSpaces();
            string mapArg = args.Length > 1 ? args[0] : p.level.MapName;
            string backupArg = args.Length > 1 ? args[1] : args[0];

            string path;
            if (backupArg == CURRENT_FLAG) {
                path = LevelInfo.MapPath(mapArg);
                if (!LevelInfo.MapExists(mapArg)) {
                    if (Directory.Exists(LevelInfo.BackupBasePath(mapArg))) {
                        p.Message("&WLevel \"{0}\" does not currently exist, &Showever:", mapArg);
                        LevelOperations.OutputBackups(p, mapArg, LevelInfo.GetConfig(mapArg));
                    } else {
                        p.Message("&WLevel \"{0}\" does not exist and no backups could be found.", mapArg);
                    }
                    return;
                }
            } else {
                if (!LevelInfo.GetBackupPath(p, mapArg, backupArg, out path)) return;
            }

            string formattedMuseumName;
            if (backupArg == CURRENT_FLAG) {
                formattedMuseumName = "&cMuseum &S(" + mapArg + ")";
            } else {
                formattedMuseumName = "&cMuseum &S(" + mapArg + " " + backupArg + ")";
            }
            
            if (p.level.name.CaselessEq(formattedMuseumName)) {
                p.Message("You are already in this museum."); return;
            }
            if (Interlocked.CompareExchange(ref p.LoadingMuseum, 1, 0) == 1) {
                p.Message("You are already loading a museum level."); return;
            }
            
            try {
                Level lvl = LevelActions.LoadMuseum(p, formattedMuseumName, mapArg, path);
                PlayerActions.ChangeMap(p, lvl);
            } finally {
                Interlocked.Exchange(ref p.LoadingMuseum, 0);
            }
        }
        
        public override void Help(Player p) {
            p.Message("&T/Museum <level> [backup]");
            p.Message("&HVisits the [backup] of <level>");
            p.Message("&T/Museum <level> {0}", LevelInfo.LATEST_MUSEUM_FLAG);
            p.Message("&HVisits the latest backup of <level>");
            p.Message("&T/Museum <level> {0}", CURRENT_FLAG);
            p.Message("&HVisits <level> as it is currently stored on disk.");
            p.Message("&HIf <level> is not given, the current level is used.");
        }
    }
}
