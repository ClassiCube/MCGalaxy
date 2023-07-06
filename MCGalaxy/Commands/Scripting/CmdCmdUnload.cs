/*
    Copyright 2010 MCLawl Team - Written by Valek (Modified for use with MCForge)
 
    Dual-licensed under the Educational Community License, Version 2.0 and
    the GNU General Public License, Version 3 (the "Licenses"); you may
    not use this file except in compliance with the Licenses. You may
    obtain a copy of the Licenses at
    
    https://opensource.org/license/ecl-2-0/
    https://www.gnu.org/licenses/gpl-3.0.html
    
    Unless required by applicable law or agreed to in writing,
    software distributed under the Licenses are distributed on an "AS IS"
    BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express
    or implied. See the Licenses for the specific language governing
    permissions and limitations under the Licenses.
*/
using MCGalaxy.Scripting;

namespace MCGalaxy.Commands.Scripting 
{
    public sealed class CmdCmdUnload : Command2 
    {
        public override string name { get { return "CmdUnload"; } }
        public override string type { get { return CommandTypes.Other; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Owner; } }
        public override bool MessageBlockRestricted { get { return true; } }
        
        public override void Use(Player p, string cmdName, CommandData data) {
            if (cmdName.Length == 0) { Help(p); return; }
            
            string cmdArgs = "";
            Command.Search(ref cmdName, ref cmdArgs);
            Command cmd = Command.Find(cmdName);
            
            if (cmd == null) {
                p.Message("\"{0}\" is not a valid or loaded command.", cmdName); return;
            }           
            ScriptingOperations.UnloadCommand(p, cmd);
        }

        public override void Help(Player p) {
            p.Message("&T/CmdUnload [command]");
            p.Message("&HUnloads a command from the server.");
        }
    }
}
