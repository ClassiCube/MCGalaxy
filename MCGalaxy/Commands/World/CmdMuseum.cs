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
using System.Threading;
using MCGalaxy.Levels.IO;

namespace MCGalaxy.Commands.World {
    public sealed class CmdMuseum : Command2 {
        public override string name { get { return "Museum"; } }
        public override string type { get { return CommandTypes.World; } }
        public override bool SuperUseable { get { return false; } }

        public override void Use(Player p, string message, CommandData data) {
            string[] args = message.SplitSpaces();
            string path = args.Length == 1 ? LevelInfo.MapPath(args[0]) : LevelInfo.BackupFilePath(args[0], args[1]);
            if (!File.Exists(path)) {
                p.Message("Level or backup could not be found."); return;
            }
            
            string name = null;
            if (args.Length == 1) {
                name = "&cMuseum &S(" + args[0] + ")";
            } else {
                name = "&cMuseum &S(" + args[0] + " " + args[1] + ")";
            }
            
            if (p.level.name.CaselessEq(name)) {
                p.Message("You are already in this museum."); return;
            }
            if (Interlocked.CompareExchange(ref p.LoadingMuseum, 1, 0) == 1) {
                p.Message("You are already loading a museum level."); return;
            }
            
            try {
                JoinMuseum(p, name, args[0].ToLower(), path);
            } finally {
                Interlocked.Exchange(ref p.LoadingMuseum, 0);
            }
        }
        
        static void JoinMuseum(Player p, string name, string mapName, string path) {
            Level lvl = IMapImporter.Formats[0].Read(path, name, false);
            lvl.MapName = mapName;
            SetLevelProps(lvl);
            Level.LoadMetadata(lvl);
            
            if (!PlayerActions.ChangeMap(p, lvl)) return;
        }
        
        static void SetLevelProps(Level lvl) {
            lvl.SetPhysics(0);
            lvl.backedup = true;
            lvl.BuildAccess.Min = LevelPermission.Nobody;
            lvl.IsMuseum = true;
        }
        
        public override void Help(Player p) {
            p.Message("&T/Museum [level] [backup]");
            p.Message("&HTeleports you to a backup of the given level.");
        }
    }
}
