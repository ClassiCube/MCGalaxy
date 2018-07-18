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
using System.Collections.Generic;

namespace MCGalaxy.Commands.World {
    public sealed class CmdRenameLvl : Command2 {
        public override string name { get { return "RenameLvl"; } }
        public override string type { get { return CommandTypes.World; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Admin; } }
        public override CommandAlias[] Aliases {
            get { return new[] { new CommandAlias("WRename"), new CommandAlias("WorldRename") }; }
        }
        public override bool MessageBlockRestricted { get { return true; } }
        
        public override void Use(Player p, string message, CommandData data) {
            string[] args = message.SplitSpaces();
            if (args.Length != 2) { Help(p); return; }
            
            Level lvl = Matcher.FindLevels(p, args[0]);
            if (lvl == null) return;
            string newMap = args[1].ToLower();
            if (!Formatter.ValidName(p, newMap, "level")) return;
            
            if (LevelInfo.MapExists(newMap)) { p.Message("Level already exists."); return; }
            if (lvl == Server.mainLevel) { p.Message("Cannot rename the main level."); return; }
            if (!LevelInfo.ValidateAction(p, data, lvl, "rename this level")) return;
            
            List<Player> players = lvl.getPlayers();
            lvl.Unload();
            
            LevelActions.Rename(lvl.name, newMap);
            LevelActions.Load(p, newMap, true);
            Chat.MessageGlobal("Renamed {0} to {1}", lvl.name, newMap);
            // Move all the old players to the renamed map
            foreach (Player pl in players)
                PlayerActions.ChangeMap(pl, newMap);
        }
        
        public override void Help(Player p) {
            p.Message("%T/RenameLvl [level] [new name]");
            p.Message("%HRenames [level] to [new name]");
            p.Message("%HNote: Portals going to [level] will no longer work.");
        }
    }
}
