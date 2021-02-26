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
    public sealed class CmdStatic : Command2 {      
        public override string name { get { return "Static"; } }
        public override string shortcut { get { return "t"; } }
        public override string type { get { return CommandTypes.Building; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.AdvBuilder; } }
        public override bool SuperUseable { get { return false; } }
        public override CommandAlias[] Aliases {
            get { return new[] { new CommandAlias("zz", "cuboid") }; }
        }

        public override void Use(Player p, string message, CommandData data) {
            p.staticCommands = !p.staticCommands;
            p.ClearBlockchange();

            p.Message("Static mode: &a" + p.staticCommands);
            if (message.Length == 0 || !p.staticCommands) return;
            data.Context = CommandContext.Static;

            string[] parts = message.SplitSpaces(2);
            string cmd = parts[0], args = parts.Length > 1 ? parts[1] : "";
            p.HandleCommand(cmd, args, data);
        }
        
        public override void Help(Player p) {
            p.Message("&T/Static [command]");
            p.Message("&HMakes every command a toggle.");
            p.Message("&HIf [command] is given, then that command is used");
        }
    }
}
