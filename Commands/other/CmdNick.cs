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
		public CmdNick() { }
		static char[] trimChars = { ' ' };

		public override void Use(Player p, string message) {
			if (message == "") { Help(p); return; }
			string[] parts = message.Split(trimChars, 2);
			
			Player who = PlayerInfo.Find(parts[0]);
			if (who == null) { Player.SendMessage(p, "Could not find player."); return; }
			if (p != null && who.group.Permission > p.group.Permission) {
				Player.SendMessage(p, "Cannot change the nick of someone of greater rank"); return;
			}

			string newName = parts.Length > 1 ? parts[1] : "";
			if (newName == "") {
				who.DisplayName = who.name;
				Player.SendChatFrom(who, who.FullName + "%S has reverted their nick to their original name.", false);
			} else {
				if (newName.Length >= 30) { Player.SendMessage(p, "Nick must be under 30 letters."); return; }				
				Player.SendChatFrom(who, who.FullName + "%S has changed their nick to " + who.color + newName + "%S.", false);
				who.DisplayName = newName;
			}
			
			Player.GlobalDespawn(who, false);
			Player.GlobalSpawn(who, who.pos[0], who.pos[1], who.pos[2], who.rot[0], who.rot[1], false);
            PlayerDB.Save(who);
		}
		
		public override void Help(Player p) {
			Player.SendMessage(p, "/nick <player> [newName] - Gives <player> the nick of [newName].");
			Player.SendMessage(p, "If no [newName] is given, the player's nick is reverted to their original name.");
		}
	}
	
	public class CmdXNick : Command {
		
        public override string name { get { return "xnick"; } }
        public override string shortcut { get { return "xnickname"; } }
        public override string type { get { return CommandTypes.Other; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        public CmdXNick() { }

        public override void Use(Player p, string message) {
            if (message != "") message = " " + message;
        	Command.all.Find("nick").Use(p, p.name + message);
        }
        
        public override void Help(Player p) {
            Player.SendMessage(p, "/xnick [newName] - Gives you the nick of [newName].");
            Player.SendMessage(p, "If no [newName] is given, your nick is reverted to your original name.");
        }
    }
}

