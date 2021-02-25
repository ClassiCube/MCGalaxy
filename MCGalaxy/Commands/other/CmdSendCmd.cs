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
    
    public sealed class CmdSendCmd : Command2 {        
        public override string name { get { return "SendCmd"; } }
        public override string type { get { return CommandTypes.Other; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Nobody; } }
        
        public override void Use(Player p, string message, CommandData data) {
            string[] args = message.SplitSpaces(3);
            Player target = PlayerInfo.FindMatches(p, args[0]);
            if (target == null) return;
            
            if (!CheckRank(p, data, target, "send commands for", true)) return;
            if (args.Length == 1) { p.Message("No command name given."); return; }
            
            string cmdName = args[1], cmdArgs = args.Length > 2 ? args[2] : "";
            Command.Search(ref cmdName, ref cmdArgs);
            
            Command cmd = Command.Find(cmdName);
            if (cmd == null) {
                p.Message("Unknown command \"" + cmdName + "\"."); return;
            }
            
            data.Context = CommandContext.SendCmd;
            data.Rank = p.Rank;
            cmd.Use(target, cmdArgs, data);
        }

        public override void Help(Player p) {
            p.Message("&T/SendCmd [player] [command] <arguments>");
            p.Message("&HMake another user use a command. (e.g &T/SendCmd bob tp bob2&H)");
            p.Message("  &WNote [player] uses the command as if they had your rank");
        }
    }
}