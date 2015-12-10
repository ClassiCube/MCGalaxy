using System;
using MCGalaxy;

namespace MCGalaxy.Commands
{
	public class CmdNick : Command
	{
		public override string name { get { return "nick"; } }
		public override string shortcut { get { return "nickname"; } }
		public override string type { get { return CommandTypes.Other; } }
		public override bool museumUsable { get { return true; } }
		public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
		public CmdNick() { }

		public override void Use(Player p, string message)
		{
			if (message == "") { Help(p); return; }

			int pos = message.IndexOf(' ');
			Player who = Player.Find(message.Split(' ')[0]);
			if (who == null) { Player.SendMessage(p, "Could not find player."); return; }
			if (p != null && who.group.Permission > p.group.Permission)
			{
				Player.SendMessage(p, "Cannot change the nick of someone of greater rank");
				return;
			}
			string query;
			string newName = "";
			if (message.Split(' ').Length > 1) newName = message.Substring(pos + 1);
			else
			{
				who.DisplayName = who.name;
				Player.GlobalChat(who, who.color + who.prefix + who.DisplayName + "&g has reverted their nick to their original name.", false);
				Player.GlobalDie(p, false);
				Player.GlobalSpawn(p, p.pos[0], p.pos[1], p.pos[2], p.rot[0], p.rot[1], false);
				return;
			}

			if (newName.Length > 60) { Player.SendMessage(p, "Nick must be under 60 letters."); return; }

			if (newName != "") Player.GlobalChat(who, who.color + who.DisplayName + "&g has changed their nick to " + newName + "&g.", false);
			who.DisplayName = newName;
			Player.GlobalDie(who, false);
			Player.GlobalSpawn(who, who.pos[0], who.pos[1], who.pos[2], who.rot[0], who.rot[1], false);
            PlayerDB.Save(who);
		}
		public override void Help(Player p)
		{
			Player.SendMessage(p, "/nick <player> [newName] - Gives <player> the nick of [newName].");
			Player.SendMessage(p, "If no [newName] is given, the player's nick is reverted to their original name.");
		}
	}
}

