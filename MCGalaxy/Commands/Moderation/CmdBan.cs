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

namespace MCGalaxy.Commands.Moderation {    
    public sealed class CmdBan : Command {
        public override string name { get { return "ban"; } }
        public override string type { get { return CommandTypes.Moderation; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        public CmdBan() { }

        public override void Use(Player p, string message)
        {
            if (message == "") { Help(p); return; }
            
            bool stealth = message[0] == '#';
            if (stealth) {
                message = message.Remove(0, 1);
                Server.s.Log("Stealth ban Attempted by " + (p == null ? "Console" : p.ColoredName));
            }
            string[] args = message.SplitSpaces(2);
            string reason = args.Length > 1 ? args[1] : "";
            
            string target = ModActionCmd.FindName(p, "ban", "ban", "", args[0], ref reason);
            if (target == null) return;
            Player who = PlayerInfo.FindExact(target);
            
            if (reason == "") reason = Server.defaultBanMessage;
            if (reason == "-") reason = "&c-";
            reason = ModActionCmd.ExpandReason(p, reason);
            
            if (reason == null) return;
            Group group = who == null ? Group.findPlayerGroup(args[0]) : who.group;
            if (!CheckPerms(target, group, p)) return;

            string banReason = reason == "-" ? "" : " (" + reason + ")";
            string banner = p == null ? "(console)" : p.ColoredName;
            string banMsg = null;
            if (who == null) {
                banMsg = target + " &f(offline) %Swas &8banned %Sby " + banner + "%S." + banReason;
                Chat.MessageGlobal(banMsg);
            } else {
                if (stealth) {
                    banMsg = who.ColoredName + " %Swas STEALTH &8banned %Sby " + banner + "%S." + banReason;
                    Chat.MessageOps(banMsg);
                } else {
                    banMsg = who.ColoredName + " %Swas &8banned %Sby " + banner + "%S." + banReason;
                    Chat.MessageGlobal(banMsg);
                }
                who.color = "";
            }
            
            Ban.DeleteBan(target);
            Ban.BanPlayer(p, target, reason, stealth, group.name);
            ModActionCmd.ChangeRank(target, group, Group.BannedRank, who);
            
            if (args.Length == 1) Player.AddNote(target, p, "B");
            else Player.AddNote(target, p, "B", reason);
            Server.IRC.Say(banMsg);
            Server.s.Log("BANNED: " + target.ToLower() + " by " + banner);
        }
        
        bool CheckPerms(string name, Group group, Player p) {
            if (group.Permission == LevelPermission.Banned) {
                Player.Message(p, name + " is already banned."); return false;
            }
            if (p != null && group.Permission >= p.Rank) {
                MessageTooHighRank(p, "ban", false); return false;
            }
            return true;
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/ban [player] <reason>");
            Player.Message(p, "%HBans a player without kicking them.");
            Player.Message(p, "%HAdd # before name to stealth ban.");
            Player.Message(p, "%HFor <reason>, @number can be used as a shortcut for that rule.");
        }
    }
}
