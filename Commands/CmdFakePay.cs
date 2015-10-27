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
    public sealed class CmdFakePay : Command
    {
        public override string name { get { return "fakepay"; } }
        public override string shortcut { get { return "fpay"; } }
        public override string type { get { return "other"; } }
        public override bool museumUsable { get { return true; } }
        public override void Help(Player p)
        {
            Player.SendMessage(p, "/fakepay <name> <amount> - Sends a fake give change message.");
        }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        public override void Use(Player p, string message)
        {
            if (message == "") { Help(p); return; }
			
			var split = message.Split(' ');
			
            Player who = Player.Find(split[0]);
			if (who == null)
            {
                Player.SendMessage(p, Server.DefaultColor + "Player not found!");
                return;
            }
			
			int amount = 0;
			try
			{
	            amount = int.Parse(split[1]);
			}
			catch/* (Exception ex)*/
			{
				Player.SendMessage(p, "How much do you want to fakepay them?");
				return;
			}
			
            if (amount < 0)
            {
                Player.SendMessage(p, Server.DefaultColor + "You can't fakepay a negative amount.");
                return;
            }
			
            if (amount >= 16777215)
            {
                Player.SendMessage(p, "Whoa, that's too much money. Try less than 16777215.");
                return;
            }

            Player.GlobalMessage(who.color + who.prefix + who.DisplayName + Server.DefaultColor + " was given " + amount + " " + Server.moneys);

        }
    }
}

