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
namespace MCGalaxy.Commands
{
	public sealed class CmdGlobalCLS : Command
	{
		public override string name { get { return "globalcls"; } }
		public override string shortcut { get { return "gcls"; } }
		public override string type { get { return CommandTypes.Chat; } }
		public override bool museumUsable { get { return true; } }
		public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }

		public override void Use(Player p, string message)
		{
			int i = 0;
			for (i = 0; i < 20; i++)
			{
				Player.players.ForEach(delegate(Player p1) { BlankMessage(p1); });
			}
			Player.GlobalMessage("%4Global Chat Cleared.");
		}
		//Yes this does work
		//Trust me...I'm a doctor
		public void BlankMessage(Player p)
		{
			byte[] buffer = new byte[65];
			Player.StringFormat(" ", 64).CopyTo(buffer, 1);
			p.SendRaw(13, buffer);
			buffer = null;
		}
		public override void Help(Player p)
		{
			Player.SendMessage(p, "/globalcls - Clears the chat for all users.");
		}
	}
}
