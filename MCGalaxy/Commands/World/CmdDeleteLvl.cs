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
using System.IO;
using MCGalaxy.SQL;

namespace MCGalaxy.Commands.World {
    public sealed class CmdDeleteLvl : Command2 {
        public override string name { get { return "DeleteLvl"; } }
        public override string type { get { return CommandTypes.World; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Admin; } }
        public override CommandAlias[] Aliases {
            get { return new[] { new CommandAlias("WDelete"), new CommandAlias("WorldDelete"), new CommandAlias("WRemove") }; }
        }
        public override bool MessageBlockRestricted { get { return true; } }
        
        public override void Use(Player p, string message, CommandData data) {
            if (message.Length == 0 || message.SplitSpaces().Length > 1) { Help(p); return; }
            if (!Formatter.ValidName(p, message, "level")) return;
            string map = Matcher.FindMaps(p, message);
            if (map == null) return;

            if (map.CaselessEq(ServerConfig.MainLevel)) { p.Message("Cannot delete the main level."); return; }
            if (!LevelInfo.ValidateAction(p, data, map, "delete this level")) return;
            
            p.Message("Created backup.");
            if (LevelActions.Delete(map)) return;
            p.Message(LevelActions.DeleteFailedMessage);
        }
        
        public override void Help(Player p) {
            p.Message("%T/DeleteLvl [map]");
            p.Message("%HCompletely deletes [map] (portals, MBs, everything)");
            p.Message("%HA backup of the map will be placed in the levels/deleted folder");
        }
    }
}
