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
            string[] args = message.SplitSpaces(2);
            
            string reason = args.Length > 1 ? args[1] : "";
            reason = ModActionCmd.ExpandReason(p, reason);
            if (reason == null) return;
            
            Player who = PlayerInfo.FindMatches(p, args[0]);
            if (who == null) {
                if (Server.muted.Contains(args[0])) {
                    ModAction action = new ModAction(args[0], p, ModActionType.Unmuted, reason);
                    OnModActionEvent.Call(action);
                }
                return;
            }
            if (p != null && p == who) { Player.Message(p, "You cannot mute or unmute yourself."); return; }

            if (who.muted) {
                ModAction action = new ModAction(who.name, p, ModActionType.Unmuted, reason);
                OnModActionEvent.Call(action);
            } else  {
                if (p != null && who.Rank >= p.Rank) { 
                    MessageTooHighRank(p, "mute", false); return;
                }
                
                ModAction action = new ModAction(who.name, p, ModActionType.Muted, reason);
                OnModActionEvent.Call(action);
            }
        }

        public override void Help(Player p) {
            Player.Message(p, "%T/mute [player] <reason>");
            Player.Message(p, "%HMutes or unmutes that player.");
            Player.Message(p, "%HFor <reason>, @number can be used as a shortcut for that rule.");
        }
    }
}
