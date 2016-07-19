/*
    Copyright 2010 MCLawl Team - Written by Valek (Modified for use with MCGalaxy)
 
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
    public sealed class CmdCmdLoad : Command {
        public override string name { get { return "cmdload"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.Other; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Nobody; } }

        public override void Use(Player p, string message) {
        	string[] args = message.Split(' ');
            if (Command.all.Contains(args[0])) {
                Player.Message(p, "That command is already loaded!"); return;
            }
            
        	string error = Scripting.Load("Cmd" + args[0]);
        	if (error != null) { Player.Message(p, error); return; }
        	GrpCommands.fillRanks();
        	Player.Message(p, "Command was successfully loaded.");
        }

        public override void Help(Player p) {
            Player.Message(p, "%T/cmdload <command>");
            Player.Message(p, "%HLoads a compiled command into the server for use.");
            Player.Message(p, "%HThis method load both C# and Visual Basic compiled commands.");
        }
    }
}
