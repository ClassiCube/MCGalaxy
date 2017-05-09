/*
    Copyright 2011 MCForge
    
    Written by SebbiUltimate
        
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
    
    public sealed class CmdSendCmd : Command {
        
        public override string name { get { return "sendcmd"; } }
        public override string type { get { return CommandTypes.Other; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Nobody; } }
        
        public override void Use(Player p, string message)
        {
            string[] parts = message.SplitSpaces(3);
            Player target = PlayerInfo.FindMatches(p, parts[0]);
            if (target == null) return;        
            if (p != null && p.Rank < target.group.Permission) {
                MessageTooHighRank(p, "send commands for", true); return;
            }
            if (parts.Length == 1) {
                Player.Message(p, "No command name given."); return;
            }
            
            string name = parts[1], args = parts.Length > 2 ? parts[2] : "";
            Command cmd = Command.all.Find(name);
            if (cmd == null) {
                Player.Message(p, "Unknown command \"" + name + "\"."); return;
            }
            cmd.Use(target, args);
        }

        public override void Help(Player p) {
            Player.Message(p, "%T/sendcmd [player] [command] <arguments>");
            Player.Message(p, "%HMake another user use a command. (e.g /sendcmd bob tp bob2)");
        }
    }
}