/*
    Copyright 2015 MCGalaxy
    
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

namespace MCGalaxy.Commands.Chatting {    
    public class CmdHug : MessageCmd {
        public override string name { get { return "Hug"; } }
        public override CommandPerm[] ExtraPerms {
            get { return new[] { new CommandPerm(LevelPermission.Operator, "can death hug") }; }
        }
        
        public override void Use(Player p, string message, CommandData data) {
            if (message.Length == 0) { Help(p); return; }
            string[] args = message.SplitSpaces();
            string hugType = null;
            
            if (args.Length > 1) {
                args[1] = args[1].ToLower();
                if (args[1] == "loving" || args[1] == "creepy" || args[1] == "friendly" || args[1] == "deadly")
                    hugType = args[1];
            }
            if (hugType == null) { TryMessageAction(p, args[0], "λNICK &Shugged λTARGET", false); return; }
            
            TryMessageAction(p, args[0], "λNICK &Sgave λTARGET &Sa " + hugType + " hug", false);
            if (hugType == "deadly") {
                if (!CheckExtraPerm(p, data, 1)) return;
                Player target = PlayerInfo.FindMatches(p, args[0]);
                if (target == null) return;
            
                if (!CheckRank(p, data, target, "&cdeath-hug&S", true)) return;
                target.HandleDeath(Block.Stone, "@p &Sdied from a &cdeadly hug.");
            } 
        }
        
        public override void Help(Player p) {
            p.Message("&T/Hug [player] <type>");
            p.Message("&HValid types are: &floving, friendly, creepy and deadly.");
            p.Message("&HSpecifying no type or a non-existent type results in a normal hug.");
        }
    }
}
