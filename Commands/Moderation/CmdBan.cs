/*
    Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCGalaxy)
    
    Dual-licensed under the    Educational Community License, Version 2.0 and
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

namespace MCGalaxy.Commands {
    
    public sealed class CmdBan : Command {
        
        public override string name { get { return "ban"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.Moderation; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        public CmdBan() { }
        static char[] trimChars = { ' ' };

        public override void Use(Player p, string message) {
            if (message == "") { Help(p); return; }
            
            bool stealth = false, totalBan = false;
            if (message[0] == '#') {
                message = message.Remove(0, 1).Trim();
                stealth = true;
                Server.s.Log("Stealth ban Attempted by " + (p == null ? "Console" : p.FullName));
            } else if (message[0] == '@') {
                message = message.Remove(0, 1).Trim();
                totalBan = true;
                Server.s.Log("Total ban Attempted by " + (p == null ? "Console" : p.FullName));
            }
            
            string[] args = message.Split(trimChars, 2);
            string reason = args.Length > 1 ? args[1] : "-";
            string banReason = reason == "-" ? "" : " (" + reason + ")";
            if (reason == "-") reason = "&c-";    
            reason = reason.Replace(" ", "%20");
            Player who = Player.Find(args[0]);
            
            string target = who == null ? args[0] : who.name;
            if (!Player.ValidName(target)) {
                Player.SendMessage(p, "Invalid name \"" + target + "\"."); return;
            }
            Group group = who == null ? Group.findPlayerGroup(args[0]) : who.group;
            if (!CheckPerms(target, group, p)) return;
            
            string banner = p == null ? "(console)" : p.FullName;
            string banMsg = null;
            if (who == null) {
            	banMsg = target + " &f(offline)" + " %Swas &8banned" + Server.DefaultColor + " by " + banner + "%S." + banReason;
            	Player.GlobalMessage(banMsg);
            } else {
            	if (stealth) {
            		banMsg = who.FullName + " %Swas STEALTH &8banned" + Server.DefaultColor + " by " + banner + "%S." + banReason;
                    Chat.GlobalMessageOps(banMsg);
            	} else {
            		banMsg = who.FullName + " %Swas &8banned" + Server.DefaultColor + " by " + banner + "%S." + banReason;
            		Player.GlobalMessage(banMsg);
            	}
                
                who.group = Group.findPerm(LevelPermission.Banned);
                who.color = who.group.color;
                Player.GlobalDespawn(who, false);
                Player.GlobalSpawn(who, who.pos[0], who.pos[1], who.pos[2], who.rot[0], who.rot[1], false);
            }
            
            string oldgroup = group.name.ToString();
            Group.findPerm(LevelPermission.Banned).playerList.Add(target);
            Ban.Banplayer(p, target.ToLower(), reason, stealth, oldgroup);
            Group.findPerm(LevelPermission.Banned).playerList.Save();
            
            Server.IRC.Say(banMsg);
            Server.s.Log("BANNED: " + target.ToLower() + " by " + banner);
            if (totalBan) {
                Command.all.Find("undo").Use(p, target + " 0");
                Command.all.Find("banip").Use(p, "@ " + target);
            }
        }
        
        bool CheckPerms(string name, Group group, Player p) {
            if ((int)group.Permission >= CommandOtherPerms.GetPerm(this)) {
                string highest = Group.findPermInt(CommandOtherPerms.GetPerm(this)).name;
                Player.SendMessage(p, "You can't ban players ranked " + highest + " or higher!"); return false;
            }
            if (group.Permission == LevelPermission.Banned) {
                Player.SendMessage(p, name + " is already banned."); return false;
            }
            if (p != null && group.Permission >= p.group.Permission) {
                Player.SendMessage(p, "You cannot ban a person ranked equal or higher than you."); return false;
            }
            
            group.playerList.Remove(name);
            group.playerList.Save();
            return true;
        }
        
        public override void Help(Player p) {
            Player.SendMessage(p, "/ban <player> [reason] - Bans a player without kicking them.");
            Player.SendMessage(p, "Add # before name to stealth ban.");
            Player.SendMessage(p, "Add @ before name to total ban.");
        }
    }
}
