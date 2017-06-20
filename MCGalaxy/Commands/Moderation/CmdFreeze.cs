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
using System;

namespace MCGalaxy.Commands.Moderation {
    public sealed class CmdFreeze : Command {
        public override string name { get { return "freeze"; } }
        public override string shortcut { get { return "fz"; } }
        public override string type { get { return CommandTypes.Moderation; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }

        public override void Use(Player p, string message) {
            if (message == "") { Help(p); return; }
            string[] args = message.SplitSpaces(3);
            
            Player who = PlayerInfo.FindMatches(p, args[0]);
            if (who == null) return;            
            if (p != null && p == who) { Player.Message(p, "Cannot freeze yourself."); return; }
            if (p != null && who.Rank >= p.Rank) { MessageTooHighRank(p, "freeze", false); return; }
            
            if (who.frozen) {
                string reason = args.Length > 1 ? args[1] : "";
                reason = ModActionCmd.ExpandReason(p, reason);
                if (reason == null) return;
                
                ModAction action = new ModAction(who.name, p, ModActionType.Unfrozen, reason);
                OnModActionEvent.Call(action);
            } else {
                if (args.Length < 2) { Help(p); return; }
                TimeSpan duration = TimeSpan.Zero;
                if (!CommandParser.GetTimespan(p, args[1], ref duration, "freeze for", 'm')) return;
                
                string reason = args.Length > 2 ? args[2] : "";
                reason = ModActionCmd.ExpandReason(p, reason);
                if (reason == null) return;
                
                ModAction action = new ModAction(who.name, p, ModActionType.Frozen, reason, duration);
                OnModActionEvent.Call(action);
            }
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/freeze [name] [timespan] <reason>");
            Player.Message(p, "%HStops [name] from moving for [timespan] time, or until manually unfrozen.");
            Player.Message(p, "%HFor <reason>, @number can be used as a shortcut for that rule.");
        }
    }
}
