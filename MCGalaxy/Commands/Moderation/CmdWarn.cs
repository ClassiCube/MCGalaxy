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
using MCGalaxy.Events;

namespace MCGalaxy.Commands.Moderation {    
    public sealed class CmdWarn : Command {        
        public override string name { get { return "warn"; } }
        public override string type { get { return CommandTypes.Moderation; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.AdvBuilder; } }

        public override void Use(Player p, string message) {
            if (message == "") { Help(p); return; }
            string[] args = message.SplitSpaces(2);
            Player who = PlayerInfo.FindMatches(p, args[0]);
            
            string reason = args.Length == 1 ? "you know why." : args[1];
            reason = ModActionCmd.ExpandReason(p, reason);
            if (reason == null) return;
            
            if (who == null) { WarnOffline(p, args, reason); return; }
            if (who == p) { Player.Message(p, "you can't warn yourself"); return; }
            if (p != null && p.Rank <= who.Rank) {
                MessageTooHighRank(p, "warn", false); return;
            }           
                        
            ModAction action = new ModAction(who.name, p, ModActionType.Warned, reason);
            OnModActionEvent.Call(action);
        }
        
        static void WarnOffline(Player p, string[] args, string reason) {
            Player.Message(p, "Searching PlayerDB..");
            string offName = PlayerInfo.FindOfflineNameMatches(p, args[0]);
            if (offName == null) return;
      
            ModAction action = new ModAction(offName, p, ModActionType.Warned, reason);
            OnModActionEvent.Call(action);
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/warn [player] <reason>");
            Player.Message(p, "%HWarns a player. Players are kicked after 3 warnings.");
            Player.Message(p, "%HFor <reason>, @number can be used as a shortcut for that rule.");
        }
    }
}
