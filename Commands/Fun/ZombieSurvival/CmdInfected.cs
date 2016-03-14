/*
	Copyright 2011 MCForge
	
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
    public sealed class CmdInfected : Command
    {
        public override string name { get { return "infected"; } }
        public override string shortcut { get { return "dead"; } }
        public override string type { get { return CommandTypes.Games; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Banned; } }
 		public override bool Enabled { get { return Server.ZombieModeOn; } }       
        public CmdInfected() { }
        
        public override void Use(Player p, string message)
        {
            if (ZombieGame.infectd.Count == 0)
            {
                Player.SendMessage(p, "No one is infected");
            }
            else
            {
                Player.SendMessage(p, "Players who are " + Colors.red + "infected " + Server.DefaultColor + "are:");
                string playerstring = "";
                ZombieGame.infectd.ForEach(delegate(Player player)
                {
                    playerstring = playerstring + Colors.red + player.DisplayName + Server.DefaultColor + ", ";
                });
                Player.SendMessage(p, playerstring);
            }
        }
        public override void Help(Player p)
        {
            Player.SendMessage(p, "/infected - shows who is infected");
        }
    }
}
