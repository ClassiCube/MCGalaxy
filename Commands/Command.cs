/*
	Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCGalaxy)

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
using System.Collections.Generic;
using MCGalaxy.Commands;
namespace MCGalaxy
{
	public abstract partial class Command
	{
		public abstract string name { get; }
		public abstract string shortcut { get; }
		public abstract string type { get; }
		public abstract bool museumUsable { get; }
		public abstract LevelPermission defaultRank { get; }
		public abstract void Use(Player p, string message);
		public abstract void Help(Player p);
		public bool isIntervalized;
		public int intervalInMinutes;
		public DateTime nextExecution;
		public Player intervalUsingPlayer;

		public static CommandList all = new CommandList();
		public static CommandList core = new CommandList();
		
		/// <summary>
		/// Add a command to the server
		/// </summary>
		/// <param name="command">The command to add</param>
		public void AddCommand(Command command)
		{
			all.Add(command);
		}
		
		protected static void RevertAndClearState(Player p, ushort x, ushort y, ushort z) {
			p.ClearBlockchange();
			byte b = p.level.GetTile(x, y, z);
			p.SendBlockchange(x, y, z, b);
		}
	}
	
	public sealed class CommandTypes {
		public const string Building = "build";
		public const string Chat = "chat";
		public const string Economy = "economy";
		public const string Games = "game";
		public const string Information = "information";
		public const string Moderation = "mod";
		public const string Other = "other";
		public const string World = "world";
	}
}
