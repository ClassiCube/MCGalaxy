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
using System;
using MCGalaxy;

namespace MCGalaxy.Commands {	
	public class CmdNick : Command {
		
		public override string name { get { return "nick"; } }
		public override string shortcut { get { return "nickname"; } }
		public override string type { get { return CommandTypes.Other; } }
		public override bool museumUsable { get { return true; } }
		public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        public override CommandPerm[] AdditionalPerms {
            get { return new[] { new CommandPerm(LevelPermission.Operator, "+ can change the nick of other players") }; }
        }
        public override CommandAlias[] Aliases {
            get { return new[] { new CommandAlias("xnick", "-own") }; }
        }

		public override void Use(Player p, string message) {
			if (message == "") { Help(p); return; }
			string[] args = message.SplitSpaces(2);
            if (args[0].CaselessEq("-own")) {
                if (Player.IsSuper(p)) { SuperRequiresArgs(p, "player name"); return; }
                args[0] = p.name;
            }
			
			Player who = PlayerInfo.FindMatches(p, args[0]);
			if (who == null) return;
			if (p != null && who.group.Permission > p.group.Permission) {
                MessageTooHighRank(p, "change the nick of", true); return;
			}
			if (who != p && !CheckExtraPerm(p)) { MessageNeedPerms(p, "can change the nick of other players."); return; }

			string newName = args.Length > 1 ? args[1] : "";
			if (newName == "") {
				who.DisplayName = who.truename;
				Player.SendChatFrom(who, who.FullName + "%S reverted their nick to their original name.", false);
			} else {
				if (newName.Length >= 30) { Player.Message(p, "Nick must be under 30 letters."); return; }				
				Player.SendChatFrom(who, who.FullName + "%S changed their nick to " + who.color + newName + "%S.", false);
				who.DisplayName = newName;
			}
			
			Entities.GlobalDespawn(who, false);
			Entities.GlobalSpawn(who, false);
            PlayerDB.Save(who);
		}
		
		public override void Help(Player p) {
			Player.Message(p, "%T/nick <player> [nick]");
            Player.Message(p, "%HSets the nick of <player>");
			Player.Message(p, "%HIf no [nick] is given, reverts player's nick to their original name.");
		}
	}
}

