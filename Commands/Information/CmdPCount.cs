/*
	Copyright 2010 MCLawl Team - Written by Valek (Modified for use with MCGalaxy)
 
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
using System.Data;
using MCGalaxy.SQL;
namespace MCGalaxy.Commands
{
    public sealed class CmdPCount : Command
    {
        public override string name { get { return "pcount"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.Information; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Banned; } }
        public CmdPCount() { }

        public override void Use(Player p, string message)
        {
            int bancount = Group.findPerm(LevelPermission.Banned).playerList.All().Count;

            DataTable count = Database.fillData("SELECT COUNT(id) FROM Players");
            Player.SendMessage(p, "A total of " + count.Rows[0]["COUNT(id)"] + " unique players have visited this server.");
            Player.SendMessage(p, "Of these players, " + bancount + " have been banned.");
            count.Dispose();

            int playerCount = 0;
            int hiddenCount = 0;
           
            foreach (Player pl in Player.players)
            {
                if (!pl.hidden || p == null || p.group.Permission > LevelPermission.AdvBuilder)
                {
                    playerCount++;
                    if (pl.hidden && pl.group.Permission <= p.group.Permission && (p == null || p.group.Permission > LevelPermission.AdvBuilder))
                    {
                        hiddenCount++;
                    }
                }
            }
            if (playerCount == 1)
            {
                if (hiddenCount == 0)
                {
                    Player.SendMessage(p, "There is 1 player currently online.");
                }
                else
                {
                    Player.SendMessage(p, "There is 1 player currently online (" + hiddenCount + " hidden).");
                }
            }
            else
            {
                if (hiddenCount == 0)
                {
                    Player.SendMessage(p, "There are " + playerCount + " players online.");
                }
                else
                {
                    Player.SendMessage(p, "There are " + playerCount + " players online (" + hiddenCount + " hidden).");
                }
            }
        }

        public override void Help(Player p)
        {
            Player.SendMessage(p, "/pcount - Displays the number of players online and total.");
        }
    }
}
