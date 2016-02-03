using System.Text.RegularExpressions;
/*
	Copyright 2011 MCGalaxy
	
	Dual-licensed under the	Educational Community License, Version 2.0 and
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
using MCGalaxy.SQL;
namespace MCGalaxy.Commands {
	
	public sealed class CmdClearBlockChanges : Command {
		
		public override string name { get { return "clearblockchanges"; } }
		public override string shortcut { get { return "cbc"; } }
		public override string type { get { return CommandTypes.World; } }
		public override bool museumUsable { get { return false; } }
		public override LevelPermission defaultRank { get { return LevelPermission.Admin; } }
		public CmdClearBlockChanges() { }

		public override void Use(Player p, string message) {
			if (p == null && message == "") {
				Player.SendMessage(p, "You must provide a map name when running the command from console."); return;
			}
			Level lvl = p == null ? null : p.level;
			if (message != "") {
				lvl = LevelInfo.Find(message.ToLower());
				if (lvl == null) {
					Player.SendMessage(p, "Could not find the entered level."); return; 
				}
			}
			
			if ( Server.useMySQL ) 
				Database.executeQuery("TRUNCATE TABLE `Block" + lvl.name + "`"); 
			else 
				Database.executeQuery("DELETE FROM `Block" + lvl.name + "`");
			Player.SendMessage(p, "Cleared &cALL" + Server.DefaultColor + " recorded block changes for: &d" + lvl.name);
		}
		
		public override void Help(Player p) {
			Player.SendMessage(p, "%T/clearblockchanges [map]");
			Player.SendMessage(p, "%HClears the block changes stored in /about for [map]");
			Player.SendMessage(p, "%H  If no map name is given, clears block changes for the current map.");
			Player.SendMessage(p, "%CUse with great caution!");
		}
	}
}
