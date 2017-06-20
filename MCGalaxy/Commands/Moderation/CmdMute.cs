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
using System;
using System.IO;
using MCGalaxy.Events;

namespace MCGalaxy.Commands.Moderation {
    public sealed class CmdMute : Command {
        public override string name { get { return "mute"; } }
        public override string type { get { return CommandTypes.Moderation; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }

        public override void Use(Player p, string message) {
            if (message == "") { Help(p); return; }
            string[] args = message.SplitSpaces(3);
            
            Player who = PlayerInfo.FindMatches(p, args[0]);
            if (who == null) {
                if (Server.muted.Contains(args[0])) Unmute(p, args[0], args);
                return;
            }
            if (p != null && p == who) { Player.Message(p, "You cannot mute or unmute yourself."); return; }

            if (who.muted) {
                Unmute(p, who.name, args);
            } else {
                if (p != null && who.Rank >= p.Rank) { MessageTooHighRank(p, "mute", false); return; }
                if (args.Length < 2) { Help(p); return; }
                
                TimeSpan duration = TimeSpan.Zero;
                if (!CommandParser.GetTimespan(p, args[1], ref duration, "mute for", 's')) return;
                
                string reason = args.Length > 2 ? args[2] : "";
                reason = ModActionCmd.ExpandReason(p, reason);
                if (reason == null) return;
                
                ModAction action = new ModAction(who.name, p, ModActionType.Muted, reason, duration);
                OnModActionEvent.Call(action);
            }
        }
        
        static void Unmute(Player p, string name, string[] args) {
            string reason = args.Length > 1 ? args[1] : "";
            reason = ModActionCmd.ExpandReason(p, reason);
            if (reason == null) return;
            
            ModAction action = new ModAction(name, p, ModActionType.Unmuted, reason);
            OnModActionEvent.Call(action);
        }

        public override void Help(Player p) {
            Player.Message(p, "%T/mute [player] [timespan] <reason>");
            Player.Message(p, "%HMutes or unmutes that player.");
            Player.Message(p, "%HFor <reason>, @number can be used as a shortcut for that rule.");
        }
    }
}
