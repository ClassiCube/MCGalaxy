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
namespace MCGalaxy.Commands.Scripting {
    public sealed class CmdCmdUnload : Command {
        public override string name { get { return "cmdunload"; } }
        public override string type { get { return CommandTypes.Other; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Nobody; } }
        public CmdCmdUnload() { }

        public override void Use(Player p, string message)
        {
            if (message == "") { Help(p); return; }
            string cmdName = message.SplitSpaces()[0];
            
            if (Command.core.Contains(cmdName)) {
                Player.Message(p, "/{0} is a core command, you cannot unload it.", cmdName); return;
            }
            
            Command cmd = Command.all.Find(cmdName);
            if (cmd == null) {
                Player.Message(p, "\"{0}\" is not a valid or loaded command.", cmdName); return;
            }
            
            Command.all.Remove(cmd);
            foreach (Group grp in Group.GroupList)
               grp.commands.Remove(cmd);
            Player.Message(p, "Command was successfully unloaded.");
        }

        public override void Help(Player p) {
            Player.Message(p, "%T/cmdunload [command]");
            Player.Message(p, "%HUnloads a command from the server.");
        }
    }
}
