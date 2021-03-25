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
using System.IO;
using MCGalaxy.Scripting;

namespace MCGalaxy.Commands.Scripting {
    public sealed class CmdCmdLoad : Command2 {
        public override string name { get { return "CmdLoad"; } }
        public override string type { get { return CommandTypes.Other; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Nobody; } }
        public override bool MessageBlockRestricted { get { return true; } }
        
        public override void Use(Player p, string cmdName, CommandData data) {
            if (!Formatter.ValidName(p, cmdName, "command")) return;
            if (Command.Find(cmdName) != null) {
                p.Message("That command is already loaded!"); return;
            }
            
            string path = IScripting.CommandPath(cmdName);
            if (!File.Exists(path)) {
                p.Message("File &9{0} &Snot found.", path); return;
            }
            
            string error = IScripting.LoadCommands(path);
            if (error != null) { p.Message("&W" + error); return; }
            p.Message("Command was successfully loaded.");
        }

        public override void Help(Player p) {
            p.Message("&T/CmdLoad [command name]");
            p.Message("&HLoads a compiled command into the server for use.");
            p.Message("&H  Loads both C# and Visual Basic compiled commands.");
        }
    }
}
