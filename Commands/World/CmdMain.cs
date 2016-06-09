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
namespace MCGalaxy.Commands {
    public sealed class CmdMain : Command {
        
        public override string name { get { return "main"; } }
        public override string shortcut { get { return "h"; } }
        public override string type { get { return CommandTypes.World; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Banned; } }
        public override CommandPerm[] AdditionalPerms {
            get { return new[] { new CommandPerm(LevelPermission.Admin, "+ can change the main level") }; }
        }
        public CmdMain() { }

        public override void Use(Player p, string message) {
            if (message == "") {
                if (p.level.name == Server.mainLevel.name) {
                    Player.Message(p, "You are already on the server's main level."); return;
                }
                PlayerActions.ChangeMap(p, Server.mainLevel.name);
            } else {
                if (!CheckExtraPerm(p)) { MessageNeedPerms(p, "change the main level"); return; }
                if (!Player.ValidName(message)) {
                    Player.Message(p, "\"{0}\" is not a valid level name.", message); return;
                }
                
                string map = LevelInfo.FindMapOrShowMatches(p, message);
                if (map == null) return;
                
                Server.level = map;
                Server.mainLevel = Level.Load(map);
                Server.mainLevel.unload = false;
                SrvProperties.Save();
                Player.Message(p, "Set main level to \"{0}\"", map);
            }
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/main %H- Sends you to the main level.");
            Player.Message(p, "%T/main [level]%H- Sets the main level to that level.");
        }
    }
}
