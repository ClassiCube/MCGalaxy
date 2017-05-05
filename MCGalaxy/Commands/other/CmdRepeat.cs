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
namespace MCGalaxy.Commands.Misc {
    
    public sealed class CmdRepeat : Command {
        
        public override string name { get { return "repeat"; } }
        public override string type { get { return CommandTypes.Other; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Guest; } }
        public CmdRepeat() { }

        public override void Use(Player p, string message) {
            if (p.lastCMD == "") { Player.Message(p, "No commands used yet."); return; }

            Player.Message(p, "Repeating &b/" + p.lastCMD);
            int argsIndex = p.lastCMD.IndexOf(' ');
            string cmdName = argsIndex == -1 ? p.lastCMD : p.lastCMD.Substring(0, argsIndex);
            string cmdMsg = argsIndex == -1 ? "" : p.lastCMD.Substring(argsIndex + 1);
            
            Command cmd = Command.all.Find(cmdName);
            if (cmd == null) { Player.Message(p, "Unknown command \"" + cmdName + "\"."); }
            if (p != null && !p.group.CanExecute(cmd)) { cmd.MessageCannotUse(p); return; }
            cmd.Use(p, cmdMsg);
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/repeat");
            Player.Message(p, "%HRepeats the last used command");
        }
    }
}
