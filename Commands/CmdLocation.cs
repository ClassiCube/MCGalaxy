/*
	Copyright 2015 MCGalaxy team

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
using MCGalaxy.SQL;
using System.Data;
namespace MCGalaxy.Commands
{
    public class CmdLocation : Command
    {
        public override string name { get { return "location"; } }
        public override string shortcut { get { return "lo"; } }
        public override string type { get { return "moderation"; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Admin; } }
        public CmdLocation() { }
        public override void Use(Player p, string message)
        {
            Player who = null;
            string searchip = "";
            who = Player.Find(message);
            if (who == null)
            {
                Player.SendMessage(p, c.red + "Could not find player " + message + " ...searching in database");
                Database.AddParams("@Name", message);
                DataTable playerDb = Database.fillData("SELECT * FROM Players WHERE Name=@Name");
                if (playerDb.Rows.Count == 0)
                {
                    Player.SendMessage(p, c.red + "Could not find player at ALL");
                    return;
                }
                else
                    searchip = (string)playerDb.Rows[0]["IP"];
            }
            else
                searchip = who.ip;
            if (Player.IPInPrivateRange(searchip))
            {
                Player.SendMessage(p, c.red + "Player has an internal IP, cannot trace");
                return;
            }
            Player.SendMessage(p, c.lime + "The IP of " + c.aqua + message + c.lime + " has been traced to: " + c.aqua + Player.GetIPLocation(searchip));
        }
        public override void Help(Player p)
        {
            Player.SendMessage(p, "/location <name> - Tracks down the location of the IP associated with <name>.");
        }
    }
    
}
