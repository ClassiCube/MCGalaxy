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
using MCGalaxy.DB;
using MCGalaxy.Events;

namespace MCGalaxy.Commands.Moderation {    
    public sealed class CmdWarn : Command2 {        
        public override string name { get { return "Warn"; } }
        public override string type { get { return CommandTypes.Moderation; } }
        public override LevelPermission defaultRank { get { return LevelPermission.AdvBuilder; } }

        public override void Use(Player p, string message, CommandData data) {
            if (message.Length == 0) { Help(p); return; }
            string[] args = message.SplitSpaces(2);

            string reason = args.Length == 1 ? "you know why." : args[1];
            string target = ModActionCmd.FindName(p, "warn", "Warn", "", args[0], ref reason);
            if (target == null) return;
            
            reason = ModActionCmd.ExpandReason(p, reason);
            if (reason == null) return;

            Group group = ModActionCmd.CheckTarget(p, data, "warn", target);
            if (group == null) return;
                        
            ModAction action = new ModAction(target, p, ModActionType.Warned, reason);
            OnModActionEvent.Call(action);
        }
        
        public override void Help(Player p) {
            p.Message("&T/Warn [player] <reason>");
            p.Message("&HWarns a player. Players are kicked after 3 warnings.");
            p.Message("&HFor <reason>, @number can be used as a shortcut for that rule.");
        }
    }
}
