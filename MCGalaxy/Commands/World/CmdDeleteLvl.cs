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
            string map = Matcher.FindMaps(p, message);
            LevelConfig cfg;
            
            if (map == null) return;            
            if (!LevelInfo.Check(p, data.Rank, map, "delete this map",out cfg)) return;

            if (!LevelActions.Delete(p, map)) return;
            Chat.MessageGlobal("Level {0} &Swas deleted", cfg.Color + map);
        }
        
        public override void Help(Player p) {
            p.Message("&T/DeleteLvl [level]");
            p.Message("&HCompletely deletes [level] (portals, MBs, everything)");
            p.Message("&HA backup of the level is made in the levels/deleted folder");
        }
    }
}
