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
    public sealed class CmdImpersonate : Command
	{
		public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
		public override void Help(Player p) { Player.SendMessage(p, "/impersonate <player> <message> - Sends a message as if it came from <player>"); }
		public override bool museumUsable { get { return true; } }
		public override string name { get { return "impersonate"; } }
		public override string shortcut { get { return "imp"; } }
		public override string type { get { return CommandTypes.Other; } }
		public void SendIt(Player p, string message, Player player)
		{
			if (message.Split(' ').Length > 1)
			{
				if (player != null)
				{
					message = message.Substring(message.IndexOf(' ') + 1);
					//Player.GlobalMessage(player.color + player.voicestring + player.color + player.prefix + player.name + ": &f" + message);
					Player.SendChatFrom(player, message);
				}
				else
				{
					Player.SendMessage(p, "You can't impersonate '" + message.Split(' ')[0] + ",' because that person is not online!");
					//string playerName = message.Split(' ')[0];
					//message = message.Substring(message.IndexOf(' ') + 1);
					//Player.GlobalMessage(playerName + ": &f" + message);
				}
			}
			else { Player.SendMessage(p, "No message was given."); }
		}
		public override void Use(Player p, string message)
		{
			if ((message == "")) { this.Help(p); }
			else
			{
				Player player = PlayerInfo.Find(message.Split(' ')[0]);
				if (player != null)
				{
					if (p == null) { this.SendIt(p, message, player); }
					else
					{
						if (player == p) { this.SendIt(p, message, player); }
						else
						{
							if (p.group.Permission > player.group.Permission) { this.SendIt(p, message, player); }
							else { Player.SendMessage(p, "You cannot impersonate a player of equal or greater rank."); }
						}
					}
				}
				else
				{
					if (p != null)
					{
						if (p.group.Permission >= LevelPermission.Admin)
						{
							if (Group.findPlayerGroup(message.Split(' ')[0]).Permission < p.group.Permission) { this.SendIt(p, message, null); }
							else { Player.SendMessage(p, "You cannot impersonate a player of equal or greater rank."); }
						}
						else { Player.SendMessage(p, "You are not allowed to impersonate offline players"); }
					}
					else { this.SendIt(p, message, null); }
				}
			}
		}
	}
}
