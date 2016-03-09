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
using System;
namespace MCGalaxy.Commands
{
    public sealed class CmdMoney : Command
    {
        public override string name { get { return "money"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.Economy; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Banned; } }
        public override void Use(Player p, string message)
        {
            bool emptyMessage = message == "" || message == null || message == string.Empty;
            if (p != null && emptyMessage)
            {
                Player.SendMessage(p, "You currently have %f" + p.money + " %3" + Server.moneys);
            }
            else if (message.Split().Length == 1)
            {
                Player who = PlayerInfo.Find(message);
                if (who == null)
                { //player is offline
                    Economy.EcoStats ecos = Economy.RetrieveEcoStats(message);
                    Player.SendMessage(p, ecos.playerName + "(%foffline" + Server.DefaultColor + ") currently has %f" + ecos.money + " %3" + Server.moneys);
                    return;
                }
                //you can see everyone's stats with /eco stats [player]
                /*if (who.group.Permission >= p.group.Permission) {
                    Player.SendMessage(p, "%cCannot see the money of someone of equal or greater rank.");
                    return;
                }*/
                Player.SendMessage(p, who.color + who.name + Server.DefaultColor + " currently has %f" + who.money + " %3" + Server.moneys);
            }
            else if (p == null && emptyMessage)
            {
                Player.SendMessage(p, "%Console can't have %3" + Server.moneys);
            }
            else
            {
                Player.SendMessage(p, "%cInvalid parameters!");
                Help(p);
            }

        }

        public override void Help(Player p)
        {
            Player.SendMessage(p, "%f/money <player>" + Server.DefaultColor + " - Shows how much %3" + Server.moneys + Server.DefaultColor + " <player> has");
        }
    }
}
