/*
    Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCGalaxy)
    
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
namespace MCGalaxy.Commands.Building {   
    public sealed class CmdStatic : Command {      
        public override string name { get { return "static"; } }
        public override string shortcut { get { return "t"; } }
        public override string type { get { return CommandTypes.Building; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.AdvBuilder; } }
        public override CommandAlias[] Aliases {
            get { return new[] { new CommandAlias("zz", "cuboid") }; }
        }
        public CmdStatic() { }

        public override void Use(Player p, string message) {
            p.staticCommands = !p.staticCommands;
            p.ClearBlockchange();
            p.modeType = 0;

            Player.Message(p, "Static mode: &a" + p.staticCommands);
            if (message == "" || !p.staticCommands) return;

            string[] parts = message.Split(new char[] { ' ' }, 2);
            Command cmd = Command.all.Find(parts[0]);
            if (cmd == null) {
                Player.Message(p, "There is no command \"" + parts[0] + "\""); return;
            }
            
            if (!p.group.CanExecute(cmd)) {
                Player.Message(p, "Cannot use the \"" + parts[0] + "\" command."); return;
            }
            string args = parts.Length > 1 ? parts[1] : "";
            cmd.Use(p, args);
        }
        
        public override void Help(Player p) {
            Player.Message(p, "/static [command] - Makes every command a toggle.");
            Player.Message(p, "If [command] is given, then that command is used");
        }
    }
}
