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
using MCGalaxy.Games;
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
            if (CheckSuper(p, message, "level name")) return;
            IGame game = IGame.GameOn(p.level);
            
            if (message.Length == 0) {
                if (game != null) {
                    p.Message("You cannot use &T/Reload &Swhile a game is running");
                } else if (!Hacks.CanUseNoclip(p)) {
                    p.Message("You cannot use &T/Reload &Son this level");
                } else {
                    PlayerActions.ReloadMap(p);
                    p.Message("&bMap reloaded");
                }
                return;
            } 
            
            if (!CheckExtraPerm(p, data, 1)) return;
            Level lvl = p.level;
            
            if (!message.CaselessEq("all")) {
                lvl = Matcher.FindLevels(p, message);
                if (lvl == null) return;
            }
            LevelActions.ReloadAll(lvl, p, true);
        }
        
        public override void Help(Player p) {
            p.Message("&T/Reload &H- Reloads the map you are in, just for you.");
            p.Message("&T/Reload all &H- Reloads for all players in map you are in.");
            p.Message("&T/Reload [level] &H- Reloads for all players in [level]");
        }
    }
}
