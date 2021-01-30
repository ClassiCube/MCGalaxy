/*
    Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCGalaxy)
    
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
using System;
using MCGalaxy.Events;

namespace MCGalaxy.Commands.Moderation {
    public sealed class CmdKick : Command2 {
        public override string name { get { return "Kick"; } }
        public override string shortcut { get { return "k"; } }
        public override string type { get { return CommandTypes.Moderation; } }
        public override LevelPermission defaultRank { get { return LevelPermission.AdvBuilder; } }
        
        public override void Use(Player p, string message, CommandData data) {
            if (message.Length == 0) { Help(p); return; }
            string[] args = message.SplitSpaces(2);
            
            Player who = PlayerInfo.FindMatches(p, args[0]);
            if (who == null) return;
            string kickMsg = "by " + p.truename, reason = null;
            
            if (args.Length > 1) {
                reason = ModActionCmd.ExpandReason(p, args[1]);
                if (message == null) return;
                kickMsg += "&f: " + reason; 
            }

            if (p == who) { p.Message("You cannot kick yourself."); return; }
            if (who.Rank >= data.Rank) {
                Chat.MessageFrom(p, "λNICK &Stried to kick " + who.ColoredName + " &Sbut failed.");
                return;
            }
            
            ModAction action = new ModAction(who.name, p, ModActionType.Kicked, reason);
            OnModActionEvent.Call(action);
            who.Kick(kickMsg, "Kicked " + kickMsg);
        }
        
        public override void Help(Player p) {
            p.Message("&T/Kick [player] <reason>");
            p.Message("&HKicks a player.");
            p.Message("&HFor <reason>, @number can be used as a shortcut for that rule.");
        }
    }
}
