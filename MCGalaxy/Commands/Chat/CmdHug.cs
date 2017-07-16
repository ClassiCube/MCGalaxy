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
        public override string name { get { return "hug"; } }
        public override CommandPerm[] ExtraPerms {
            get { return new[] { new CommandPerm(LevelPermission.Operator, "+ can death hug") }; }
        }
        
        public override void Use(Player p, string message) {
            if (message == "") { Help(p); return; }
            string[] args = message.SplitSpaces();
            Player who = PlayerInfo.FindMatches(p, args[0]);
            if (who == null) return;
            if (p != null && who.name == p.name) {
                Player.Message(p, "You cannot hug yourself, silly!"); return;
            }
            
            string hugType = null;
            if (args.Length > 1) {
                args[1] = args[1].ToLower();
                if (args[1] == "loving" || args[1] == "creepy" || args[1] == "friendly" || args[1] == "deadly")
                    hugType = args[1];
            }
            if (hugType == null) { TryMessageAction(p, args[0], "{0} %Shugged {1}.", false); return; }
            
            if (hugType == "deadly") {
                if (!CheckExtraPerm(p)) { MessageNeedExtra(p, 1); return; }
                if (p != null && who.Rank > p.Rank) {
                    MessageTooHighRank(p, "&cdeath-hug%S", true); return;
                }
                who.HandleDeath((ExtBlock)Block.Stone, " died from a %cdeadly hug.");
            }
            TryMessageAction(p, args[0], "{0} %Sgave {1} %Sa " + hugType + " hug.", false); return;
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/hug [player] <type>");
            Player.Message(p, "%HValid types are: &floving, friendly, creepy and deadly.");
            Player.Message(p, "%HSpecifying no type or a non-existent type results in a normal hug.");
        }
    }
}
