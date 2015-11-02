/*
	Copyright 2011 MCGalaxy
	
	Written by Valek / MCLawl team
		
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
    public sealed class CmdOHide : Command
    {
        public override string name { get { return "ohide"; } }
        public override string shortcut { get { return ""; } }
       public override string type { get { return CommandTypes.Moderation; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }

        public override void Use(Player p, string message)
        {
            if (message == "") { Help(p); return; }
            message = message.Split(' ')[0];
            Player who = Player.Find(message);
            if (who == null)
            {
                Player.SendMessage(p, "Could not find player.");
                return;
            }
            if (who == p)
            {
                Player.SendMessage(p, "On yourself?  Really?  Just use /hide.");
                return;
            }
            if (who.group.Permission >= p.group.Permission)
            {
                Player.SendMessage(p, "Cannot use this on someone of equal or greater rank.");
                return;
            }
            Command.all.Find("hide").Use(who, "");
            Player.SendMessage(p, "Used /hide on " + who.color + who.name + Server.DefaultColor + ".");
        }

        public override void Help(Player p)
        {
            Player.SendMessage(p, "/ohide <player> - Hides/unhides the player specified.");
            Player.SendMessage(p, "Only works on players of lower rank.");
        }
    }
}
