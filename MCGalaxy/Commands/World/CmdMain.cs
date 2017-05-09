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
namespace MCGalaxy.Commands.World {
    public sealed class CmdMain : Command {
        
        public override string name { get { return "main"; } }
        public override string shortcut { get { return "h"; } }
        public override string type { get { return CommandTypes.World; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Banned; } }
        public override CommandPerm[] ExtraPerms {
            get { return new[] { new CommandPerm(LevelPermission.Admin, "+ can change the main level") }; }
        }
        public override CommandAlias[] Aliases {
            get { return new[] { new CommandAlias("wmain", "worldmain") }; }
        }

        public override void Use(Player p, string message)
        {
            if (message == "") {
                if (Player.IsSuper(p)) {
                    Player.Message(p, "Main level is {0}", Server.mainLevel.ColoredName);
                } else if (p.level.name == Server.mainLevel.name) {
                    Command.all.Find("spawn").Use(p, "");
                } else {
                    PlayerActions.ChangeMap(p, Server.mainLevel);
                }
            } else {
                if (!CheckExtraPerm(p)) { MessageNeedExtra(p, 1); return; }
                if (!Formatter.ValidName(p, message, "level")) return;
                
                string map = Matcher.FindMaps(p, message);
                if (map == null) return;
                Server.SetMainLevel(map);
                SrvProperties.Save();
                Player.Message(p, "Set main level to {0}", Server.mainLevel.ColoredName);
            }
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/main");
            Player.Message(p, "%HSends you to the main level.");
            Player.Message(p, "%T/main [level]");
            Player.Message(p, "%HSets the main level to that level.");
        }
    }
}
