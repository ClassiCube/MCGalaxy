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
using System.IO;
namespace MCGalaxy.Commands
{
    public sealed class CmdBanlist : Command
    {
        public override string name { get { return "banlist"; } }
        public override string shortcut { get { return "bl"; } }
       public override string type { get { return CommandTypes.Moderation; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Banned; } }
        public CmdBanlist() { }

        public override void Use(Player p, string message)
        {
            string endresult = "";
            foreach (string line in File.ReadAllLines("ranks/banned.txt"))
            {
                if (Ban.IsBanned(line))
                {
                    endresult = endresult + "&a" + line + Server.DefaultColor + ", ";
                }
                else
                {
                    endresult = endresult + "&c" + line + Server.DefaultColor + ", ";
                }
            }
            if (endresult == "")
            {
                Player.SendMessage(p, "There are no players banned");
            }
            else
            {
                endresult = endresult.Remove(endresult.Length - 2, 2);
                endresult = "&9Banned players: " + Server.DefaultColor + endresult + Server.DefaultColor + ".";
                Player.SendMessage(p, endresult);
            }
        }
        public override void Help(Player p)
        {
            Player.SendMessage(p, "/banlist - shows who's banned on server");
        }
    }
}
