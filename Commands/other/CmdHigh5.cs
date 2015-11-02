/*
	Copyright 2011 MCGalaxy
	
	Written by GamezGalaxy (hypereddie10)
	
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
    public sealed class CmdHigh5 : Command
    {
        public override string name { get { return "high5"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.Other; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Guest; } }
        public CmdHigh5() { }
        public override void Use(Player p, string message)
        {
            if (message == "")
            {
                Help(p);
				return;
            }
            Player player1 = Player.Find(message);
            if (player1 == null || player1.hidden)
            {
                Player.SendMessage(p, "Could not find player specified.");
				return;
            }

			string giver = (p == null) ? "The Console" : p.DisplayName;
			string color = (p == null) ? "" : p.color;
            Player.SendMessage(player1, giver + " just highfived you");
            Player.GlobalMessage(color + giver + " " + Server.DefaultColor + "just highfived " + player1.color + player1.DisplayName);
        }

        // This one controls what happens when you use /help [commandname].
        public override void Help(Player p)
        {
            Player.SendMessage(p, "/high5 <player> - High five someone :D");
        }
    }
}
