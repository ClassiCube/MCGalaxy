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
using System.Collections.Generic;
using MCGalaxy.Scripting;

namespace MCGalaxy.Commands.Scripting {
    public sealed class CmdCmdLoad : Command2 {
        public override string name { get { return "CmdLoad"; } }
        public override string type { get { return CommandTypes.Other; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Nobody; } }
        public override bool MessageBlockRestricted { get { return true; } }
        
        public override void Use(Player p, string cmdName, CommandData data) {
            if (cmdName.Length == 0) { Help(p); return; }
            if (!Formatter.CheckFilenameOnly(p, cmdName)) return;
            string path = IScripting.CommandPath(cmdName);
            string error;
            List<Command> cmds = IScripting.LoadCommands(path, out error);
            
            if (error != null) { 
                p.Message(error);
            } else {
                p.Message("Successfully loaded {0}", cmds.Join(c => "/" + c.name));
            }
        }

        public override void Help(Player p) {
            p.Message("&T/CmdLoad [command name]");
            p.Message("&HLoads a compiled command into the server for use.");
            p.Message("&H  Loads both C# and Visual Basic compiled commands.");
        }
    }
}
