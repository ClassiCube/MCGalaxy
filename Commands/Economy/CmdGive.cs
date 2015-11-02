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
using System;
using System.Globalization;
namespace MCGalaxy.Commands
{
    public sealed class CmdGive : Command
    {
        public override string name { get { return "give"; } }
        public override string shortcut { get { return "gib"; } }
        public override string type { get { return CommandTypes.Economy; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Admin; } }
        public CmdGive() { }

        public override void Use(Player p, string message)
        {
            if (message.IndexOf(' ') == -1) { Help(p); return; }
            if (message.Split(' ').Length != 2) { Help(p); return; }

            string user1 = "";
            string user2 = "";
            if (p == null) { user1 = "%f[ " + Server.DefaultColor + "Console%f]"; user2 = String.Format("{0}Console [&a{1}{0}]", Server.DefaultColor, Server.ZallState); }
            else { user1 = p.color + p.name; user2 = p.prefix + p.name; }

            int amountGiven;
            try { amountGiven = int.Parse(message.Split(' ')[1]); }
            catch { Player.SendMessage(p, "%cInvalid amount"); return; }
            if (amountGiven < 0) { Player.SendMessage(p, "%cCannot give negative %3" + Server.moneys); return; }

            Player who = Player.Find(message.Split(' ')[0]);
            Economy.EcoStats ecos;

            if (who == null)
            { //player is offline
                Player.OfflinePlayer off = Player.FindOffline(message.Split()[0]);
                if (off.name == "") { Player.SendMessage(p, "%cThe player %f" + message.Split()[0] + Server.DefaultColor + "(offline)%c does not exist or has never logged on to this server"); return; }

                ecos = Economy.RetrieveEcoStats(message.Split()[0]);
                if (ReachedMax(p, ecos.money, amountGiven)) return;
                ecos.money += amountGiven;
                ecos.salary = "%f" + amountGiven + "%3 " + Server.moneys + " by " + user1 + "%3 on %f" + DateTime.Now.ToString(CultureInfo.InvariantCulture);
                Economy.UpdateEcoStats(ecos);
                //Player.GlobalMessage("%f" + ecos.playerName + Server.DefaultColor + "(offline) was given %f" + amountGiven + " %3" + Server.moneys + Server.DefaultColor + " by " + user2);
                Player.GlobalMessage(user2 + Server.DefaultColor + " gave %f" + ecos.playerName + Server.DefaultColor + "(offline)" + " %f" + amountGiven + " %3" + Server.moneys);
                return;
            }

            if (who == p /*&& p.name != Server.server_owner*/)
            {
                Player.SendMessage(p, "%cYou can't give yourself %3" + Server.moneys);
                return;
            }//I think owners should be able to give themselves money, for testing reasons..
            //although questionable, because console could give money too
            /* else if (who == p && p.name == Server.server_owner) {
                if (ReachedMax(p, who.money, amountGiven)) return;
                p.money += amountGiven;
                ecos = Economy.RetrieveEcoStats(p.name);
                ecos.money = p.money;
                ecos.salary = "%f" + amountGiven + " %3 " + Server.moneys + " by " + p.color + p.name + "%3 on %f" + DateTime.Now.ToString(CultureInfo.InvariantCulture);
                Economy.UpdateEcoStats(ecos);
                Player.SendMessage(p, "You gave yourself %f" + amountGiven + " %3" + Server.moneys);
                return;
            }*/

            if (ReachedMax(p, who.money, amountGiven)) return;
            who.money += amountGiven;
            ecos = Economy.RetrieveEcoStats(who.name);
            ecos.money = who.money;
            ecos.salary = "%f" + amountGiven + "%3 " + Server.moneys + " by " + user1 + "%3 on %f" + DateTime.Now.ToString(CultureInfo.InvariantCulture);
            Economy.UpdateEcoStats(ecos);
            Player.GlobalMessage(user2 + Server.DefaultColor + " gave " + who.prefix + who.name + " %f" + amountGiven + " %3" + Server.moneys);
            //Player.GlobalMessage(who.color + who.prefix + who.name + Server.DefaultColor + " was given %f" + amountGiven + " %3" + Server.moneys + Server.DefaultColor + " by " + user2);
        }
        public override void Help(Player p)
        {
            Player.SendMessage(p, "%f/give [player] <amount>" + Server.DefaultColor + " - Gives [player] <amount> %3" + Server.moneys);
        }

        private bool ReachedMax(Player p, int current, int amount)
        {
            if (current + amount > 16777215)
            {
                Player.SendMessage(p, "%cPlayers cannot have over %316777215 %3" + Server.moneys);
                return true;
            }
            return false;
        }
    }
}
