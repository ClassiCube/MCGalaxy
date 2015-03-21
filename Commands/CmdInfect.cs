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
    /// <summary>
    /// This is the command /infect
    /// use /help infect in-game for more info
    /// </summary>
    public sealed class CmdInfect : Command
    {
        public override string name { get { return "infect"; } }
        public override string shortcut { get { return "i"; } }
        public override string type { get { return "game"; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        public CmdInfect() { }
        public override void Use(Player p, string message)
        {
            Player who = null;
            if (message == "") { who = p; message = p.name; } else { who = Player.Find(message); }
            if (who.infected)
            {
                p.SendMessage("Player cannot be infected!");
            }
            else
            {
                if (!who.referee)
                {
                    if (Server.zombie.GameInProgess())
                    {
                        Server.zombie.InfectPlayer(who);
                        Player.GlobalMessage(who.color + who.name + Server.DefaultColor + " just got Infected!");
                    }
                }
            }
        }
        public override void Help(Player p)
        {
            Player.SendMessage(p, "/infect [name] - infects [name]");
        }
    }
}