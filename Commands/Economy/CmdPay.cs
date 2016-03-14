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
using System.Globalization;
namespace MCGalaxy.Commands
{
    public sealed class CmdPay : Command
    {
        public override string name { get { return "pay"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.Economy; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Banned; } }
        public CmdPay() { }

        public override void Use(Player p, string message)
        {
            string[] args = message.Split(' ');
            if (args.Length != 2) { Help(p); return; }
            Player who = PlayerInfo.Find(args[0]);
            Economy.EcoStats payer;
            Economy.EcoStats receiver;

            int amount;
            if (!int.TryParse(args[1], out amount)) {
                Player.SendMessage(p, "Amount must be an integer."); return;
            }
            if (amount < 0) { Player.SendMessage(p, "Cannot pay negative %3" + Server.moneys); return; }

            if (who == null)
            { //player is offline
                OfflinePlayer off = PlayerInfo.FindOffline(message.Split()[0]);
                if (off == null) { Player.SendMessage(p, "%cThe player %f" + message.Split()[0] + Server.DefaultColor + "(offline)%c does not exist or has never logged on to this server"); return; }

                payer = Economy.RetrieveEcoStats(p.name);
                receiver = Economy.RetrieveEcoStats(message.Split()[0]);
                if (!IsLegalPayment(p, payer.money, receiver.money, amount)) return;

                p.money -= amount;

                payer.money = p.money;
                receiver.money += amount;

                payer.payment = "%f" + amount + " %3" + Server.moneys + " to " + off.color + off.name + "%3 on %f" + DateTime.Now.ToString(CultureInfo.InvariantCulture);
                receiver.salary = "%f" + amount + " %3" + Server.moneys + " by " + p.color + p.name + "%3 on %f" + DateTime.Now.ToString(CultureInfo.InvariantCulture);

                Economy.UpdateEcoStats(payer);
                Economy.UpdateEcoStats(receiver);

                Player.GlobalMessage(p.FullName + " %Spaid %f" + off.color + off.name + "%S(offline) %f" + amount + " %3" + Server.moneys);
                return;
            }
            if (who == p) { Player.SendMessage(p, "%cYou can't pay yourself %3" + Server.moneys); return; }

            payer = Economy.RetrieveEcoStats(p.name);
            receiver = Economy.RetrieveEcoStats(who.name);
            if (!IsLegalPayment(p, payer.money, receiver.money, amount)) return;

            p.money -= amount;
            who.money += amount;

            payer.money = p.money;
            receiver.money = who.money;

            payer.payment = "%f" + amount + " %3" + Server.moneys + " to " + who.color + who.name + "%3 on %f" + DateTime.Now.ToString(CultureInfo.InvariantCulture);
            receiver.salary = "%f" + amount + " %3" + Server.moneys + " by " + p.color + p.name + "%3 on %f" + DateTime.Now.ToString(CultureInfo.InvariantCulture);

            Economy.UpdateEcoStats(payer);
            Economy.UpdateEcoStats(receiver);
            Player.GlobalMessage(p.FullName + " %Spaid " + who.FullName + " %f" + amount + " %3" + Server.moneys);
        }       

        bool IsLegalPayment(Player p, int payer, int receiver, int amount) {
            if (receiver + amount > 16777215) { Player.SendMessage(p, "%cPlayers cannot have over %f16777215 %3" + Server.moneys); return false; }
            if (payer - amount < 0) { Player.SendMessage(p, "%cYou don't have enough %3" + Server.moneys); return false; }
            return true;
        }
        
        public override void Help(Player p) {
            Player.SendMessage(p, "%f/pay [player] <amount> " + Server.DefaultColor + "- Pays <amount> " + Server.moneys + " to [player]");
        }
    }
}
