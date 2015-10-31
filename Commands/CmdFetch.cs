/*
	Copyright 2011 MCGalaxy
	
	Author: SebbiUltimate
	
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
using System.Threading;
namespace MCGalaxy.Commands
{
    /// <summary>
    /// This is the command /fetch
    /// use /help fetch in-game for more info
    /// </summary>
	public sealed class CmdFetch : Command
	{
		public override string name { get { return "fetch"; } }
		public override string shortcut { get { return "fb"; } }
		public override string type { get { return "mod"; } }
		public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
		public override void Use(Player p, string message)
		{
            if (p == null)
            {
                Player.SendMessage(p, "Console cannot use this command. Try using /move instead.");
                return;
            }

            Player who = Player.Find(message);
            if (who == null || who.hidden)
            {
                Player.SendMessage(p, "Could not find player.");
                return;
            }

            if (p.group.Permission <= who.group.Permission)
            {
                Player.SendMessage(p, "You cannot fetch a player of equal or greater rank!");
                return;
            }

            if (p.level != who.level)
            {
                Player.SendMessage(p, who.DisplayName + " is in a different Level. Forcefetching has started!");
                Level where = p.level;
                Command.all.Find("goto").Use(who, where.name);
                Thread.Sleep(1000);
                // Sleep for a bit while they load
                while (who.Loading) { Thread.Sleep(250); }
            }

            unchecked
            {
                who.SendPos((byte)-1, p.pos[0], p.pos[1], p.pos[2], p.rot[0], 0);
            }
		}
		public override void Help(Player p)
		{
			Player.SendMessage(p, "/fetch <player> - Fetches Player forced!");
            Player.SendMessage(p, "Moves Player to your Level first");
		}
	}
}