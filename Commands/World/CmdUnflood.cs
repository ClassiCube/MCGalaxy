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
using System;
namespace MCGalaxy.Commands
{
	public sealed class CmdUnflood : Command
	{
		public override string name { get { return "unflood"; } }
		public override string shortcut { get { return ""; } }
		public override string type { get { return CommandTypes.World; } }
		public override bool museumUsable { get { return false; } }
		public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
		
		public override void Use(Player p, string message) {
			if (p == null) { MessageInGameOnly(p); return; }
			if (message == "") { Help(p); return; }
			
			if (message.ToLower() != "all" && Block.Byte(message) == Block.Zero) {
				Player.SendMessage(p, "There is no block \"" + message + "\"."); return;
			}
			
			int phys = p.level.physics;
			Command.all.Find("physics").Use(p, "0");

			Command cmd = Command.all.Find("replaceall");
			if (message.ToLower() == "all") {
				cmd.Use(p, "lavafall air");
				cmd.Use(p, "waterfall air");
				cmd.Use(p, "lava_fast air");
				cmd.Use(p, "active_lava air");
				cmd.Use(p, "active_water air");
				cmd.Use(p, "active_hot_lava air");
				cmd.Use(p, "active_cold_water air");
				cmd.Use(p, "fast_hot_lava air");
				cmd.Use(p, "magma air");
			} else {
				cmd.Use(p, message + " air");
			}

			Command.all.Find("physics").Use(p, phys.ToString());
			Player.GlobalMessage("Unflooded!");
		}
		
		public override void Help(Player p) {
			Player.SendMessage(p, "%T/unflood [liquid]");
			Player.SendMessage(p, "%HUnfloods the map you are currently in of [liquid].");
			Player.SendMessage(p, "%H  If [liquid] is \"all\", unfloods the map of all liquids.");
		}
	}
}
