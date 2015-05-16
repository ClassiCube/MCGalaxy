/*
	Copyright 2010 MCLawl Team - Written by Valek (Modified for use with MCGalaxy)
 
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
namespace MCGalaxy.Commands {
	public sealed class CmdDevs : Command {
		public override string name { get { return "devs"; } }
		public override string shortcut { get { return "dev"; } }
		public override string type { get { return "information"; } }
		public override bool museumUsable { get { return true; } }
		public override LevelPermission defaultRank { get { return LevelPermission.Banned; } }
		public CmdDevs() { }

		public override void Use(Player p, string message) {
			Player.SendMessage(p, "&9MCGalaxy Development Team: " + Server.DefaultColor + "[Add devs here]&e.");
		}

		public override void Help(Player p) {
			Player.SendMessage(p, "/devs - Displays the list of MCGalaxy developers.");
		}
	}
}