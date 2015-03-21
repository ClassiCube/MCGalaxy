/*
    Written by Jack1312
  
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
using System;
namespace MCGalaxy.Commands
{
    public sealed class CmdRankMsg : Command
    {
        public override string name { get { return "rankmsg"; } }
        public override string shortcut { get { return "rm"; } }
        public override string type { get { return "other"; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.AdvBuilder; } }
        public CmdRankMsg() { }

        public override void Use(Player p, string message)
        {
            string[] command = message.ToLower().Split(' ');
            string msg1 = String.Empty;
            string msg2 = String.Empty;
            try
            {
                msg1 = command[0];
                msg2 = command[1];
            }
            catch
            { }
            if (msg1 == "")
            {
                Help(p);
                return;
            }
            if (msg2 == "")
            {
                Command.all.Find("rankmsg").Use(p, p.group.name + " " + msg1);
                return;
            }
            Group findgroup = Group.Find(msg1);
            if (findgroup == null)
            {
                Player.SendMessage(p, "Could not find group specified!");
                return;
            }
            foreach (Player pl in Player.players)
            {
                if (pl.group.name == findgroup.name)
                {
                    pl.SendMessage(p.color + p.name + ": " + Server.DefaultColor + (message.Replace(msg1, "").Trim()));
                }
            }
        }
        public override void Help(Player p)
        {
            Player.SendMessage(p, "/rankmsg [Rank] [Message] - Sends a message to the specified rank.");
            Player.SendMessage(p, "Note: If no message is given, player's rank is taken.");
        }
    }
}
