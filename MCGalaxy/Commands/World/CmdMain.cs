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
    public sealed class CmdMain : Command2 {      
        public override string name { get { return "Main"; } }
        public override string shortcut { get { return "h"; } }
        public override string type { get { return CommandTypes.World; } }
        public override CommandPerm[] ExtraPerms {
            get { return new[] { new CommandPerm(LevelPermission.Admin, "can change the main level") }; }
        }
        public override CommandAlias[] Aliases {
            get { return new[] { new CommandAlias("WMain"), new CommandAlias("WorldMain") }; }
        }

        public override void Use(Player p, string message, CommandData data) {
            if (message.Length == 0) {
                if (p.IsSuper) {
                    p.Message("Main level is {0}", Server.mainLevel.ColoredName);
                } else if (p.level == Server.mainLevel) {
                    PlayerActions.Respawn(p);
                } else {
                    PlayerActions.ChangeMap(p, Server.mainLevel);
                }
            } else {
                if (!CheckExtraPerm(p, data, 1)) return;
                if (!Formatter.ValidMapName(p, message)) return;
                if (!LevelInfo.Check(p, data.Rank, Server.mainLevel, "set main to another map")) return;
                
                string map = Matcher.FindMaps(p, message);
                if (map == null) return;
                if (!LevelInfo.Check(p, data.Rank, map, "set main to this map")) return;
                
                Server.SetMainLevel(map);
                SrvProperties.Save();
                p.Message("Set main level to {0}", Server.mainLevel.ColoredName);
            }
        }
        
        public override void Help(Player p) {
            p.Message("&T/Main");
            p.Message("&HSends you to the main level.");
            p.Message("&T/Main [level]");
            p.Message("&HSets the main level to that level.");
        }
    }
}
