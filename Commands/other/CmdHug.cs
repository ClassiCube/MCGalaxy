/*
	Copyright 2015 MCGalaxy
	
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

namespace MCGalaxy
{
    public class CmdHug : Command
    {
        public override string name { get { return "hug"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.Other; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Guest; } }
        public override void Use(Player p, string message)
        {
            Player who = Player.Find(message);
            if (who == null || who.hidden) { Player.SendMessage(p, "Could not find specified player!"); return; }
            else
            {
                string msg = String.Format("the {0}Console [&a{1}{0}]", Server.DefaultColor, Server.ZallState);
                string giver = (p == null) ? msg : p.color + p.DisplayName;
                Player.GlobalMessage(who.color + who.DisplayName + Server.DefaultColor + " was hugged by " + giver);
            }
        }
        public override void Help(Player p)
        {
            Player.SendMessage(p, "/hug <player> - hugs the given player");
        }
    }
}
