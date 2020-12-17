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
    public sealed class CmdMute : Command2 {
        public override string name { get { return "Mute"; } }
        public override string type { get { return CommandTypes.Moderation; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }

        public override void Use(Player p, string message, CommandData data) {
            if (message.Length == 0) { Help(p); return; }
            string[] args = message.SplitSpaces(2);
            
            string target = PlayerInfo.FindMatchesPreferOnline(p, args[0]);
            if (target == null) return;
            
            if (Server.muted.Contains(target)) {
                DoUnmute(p, target, args);
            } else {            
                Group group = ModActionCmd.CheckTarget(p, data, "mute", target);
                if (group == null) return;
                
                // unmute has second argument as reason, mute has third argument instead
                DoMute(p, target, message.SplitSpaces(3));
            }
        }
        
        void DoMute(Player p, string target, string[] args) {
            TimeSpan duration = Server.Config.ChatSpamMuteTime;
            if (args.Length > 1) {
                if (!CommandParser.GetTimespan(p, args[1], ref duration, "mute for", "s")) return;
            }
            
            string reason = args.Length > 2 ? args[2] : "";
            reason = ModActionCmd.ExpandReason(p, reason);
            if (reason == null) return;
            
            ModAction action = new ModAction(target, p, ModActionType.Muted, reason, duration);
            OnModActionEvent.Call(action);
        }
        
        void DoUnmute(Player p, string target, string[] args) {
            string reason = args.Length > 1 ? args[1] : "";
            reason = ModActionCmd.ExpandReason(p, reason);
            if (reason == null) return;
            if (p.name == target) { p.Message("You cannot unmute yourself."); return; }
            
            ModAction action = new ModAction(target, p, ModActionType.Unmuted, reason);
            OnModActionEvent.Call(action);
        }

        public override void Help(Player p) {
            p.Message("%T/Mute [player] <timespan> <reason>");
            p.Message("%HMutes player for <timespan>, or unmutes that player.");
            p.Message("%H If <timespan> is not given, mutes for auto spam mute timespan");
            p.Message("%HFor <reason>, @number can be used as a shortcut for that rule.");
        }
    }
}
