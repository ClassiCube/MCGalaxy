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
	public sealed class CmdZz : Command
	{
        public override string name { get { return "zz"; } }
		public override string shortcut { get { return ""; } }
        public override string type { get { return "mod"; } }
		public override bool museumUsable { get { return false; } }
		public override LevelPermission defaultRank { get { return LevelPermission.Builder; } }
		public override void Use(Player p, string message)
		{
            if ((p.group.CanExecute(Command.all.Find("cuboid"))) && (p.group.CanExecute(Command.all.Find("static"))))
            {
                if ((!p.staticCommands == true) && (!p.megaBoid == true))
                {
                    Command.all.Find("static").Use(p, "");
                    Command.all.Find("cuboid").Use(p, message);
                    Player.SendMessage(p, p.color + p.DisplayName + Server.DefaultColor + ", to stop this, use /zz again");
                }
                else 
                {
                    p.ClearBlockchange();
                    p.staticCommands = false;
                    Player.SendMessage(p, "/zz has ended.");
                }
            }
            else { Player.SendMessage(p, "Sorry, your rank cannot use one of the commands this uses!"); }
               
		}
		public override void Help(Player p)
		{
			Player.SendMessage(p, "/zz - Like cuboid but in static mode automatically.");
		}
	}
}