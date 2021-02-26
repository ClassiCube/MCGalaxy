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
namespace MCGalaxy.Commands.World {
    public sealed class CmdReload : Command2 {
        public override string name { get { return "Reload"; } }
        public override string shortcut { get { return "Reveal"; } }
        public override string type { get { return CommandTypes.World; } }
        public override bool museumUsable { get { return false; } }
        public override CommandAlias[] Aliases {
            get { return new [] { new CommandAlias("ReJoin"), new CommandAlias("rd"),
                    new CommandAlias("WFlush"), new CommandAlias("WorldFlush") }; }
        }
        public override CommandPerm[] ExtraPerms {
            get { return new[] { new CommandPerm(LevelPermission.Operator, "can reload for all players") }; }
        }

        public override void Use(Player p, string message, CommandData data) {
            if (CheckSuper(p, message, "player or level name")) return;
            if (message.Length == 0) message = p.name;
            string[] parts = message.SplitSpaces();
            
            if (!parts[0].CaselessEq("all")) {
                PlayerActions.ReloadMap(p);
                p.Message("&bMap reloaded");
            } else {
                Level lvl = p.level;
                if (parts.Length == 2) {
                    lvl = Matcher.FindLevels(p, parts[1]);
                    if (lvl == null) return;
                } else if (p.IsSuper) {
                    SuperRequiresArgs(name + " all", p, "level name"); return;
                }
                
                if (!CheckExtraPerm(p, data, 1)) return;
                LevelActions.ReloadAll(lvl, p, true);
            }
            Server.DoGC();
        }
        
        public override void Help(Player p) {
            p.Message("&T/Reload &H- Reloads the map you are in, just for you.");
            p.Message("&T/Reload all &H- Reloads for all players in map you are in.");
            p.Message("&T/Reload all [level] &H- Reloads for all players in [level]");
        }
    }
}
