/*
    Copyright 2011 MCForge
        
    Dual-licensed under the    Educational Community License, Version 2.0 and
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
        public override CommandEnable Enabled { get { return CommandEnable.Economy; } }        
        public CmdPay() { }

        public override void Use(Player p, string message) {
            string[] args = message.Split(' ');
            if (args.Length != 2) { Help(p); return; }
            int amount;
            if (!int.TryParse(args[1], out amount)) { Player.SendMessage(p, "Amount must be an integer."); return; }
            if (amount < 0) { Player.SendMessage(p, "Cannot pay negative %3" + Server.moneys); return; }
            
            int matches = 1;
            Player who = PlayerInfo.FindOrShowMatches(p, args[0], out matches);
            if (matches > 1) return;
            if (p != null && p == who) { Player.SendMessage(p, "You cannot pay yourself %3" + Server.moneys); return; }
            string target = null;
            Economy.EcoStats payer, receiver;
            
            if (who == null) {
                OfflinePlayer off = PlayerInfo.FindOffline(args[0]);
                if (off == null) { Player.SendMessage(p, "The player \"&a" + args[0] + "%S\" was not found at all."); return; }
                payer = Economy.RetrieveEcoStats(p.name);
                receiver = Economy.RetrieveEcoStats(args[0]);
                if (!IsLegalPayment(p, payer.money, receiver.money, amount)) return;
                
                target = off.color + off.name;
                Player.GlobalMessage(p.FullName + " %Spaid %f" + off.color + off.name + "%S(offline) %f" + amount + " %3" + Server.moneys);
            } else {
                payer = Economy.RetrieveEcoStats(p.name);
                receiver = Economy.RetrieveEcoStats(who.name);
                if (!IsLegalPayment(p, payer.money, receiver.money, amount)) return;
                
                receiver.money = who.money;
                who.money += amount;
                who.OnMoneyChanged();
                target = who.color + who.name;
                Player.GlobalMessage(p.FullName + " %Spaid " + who.FullName + " %f" + amount + " %3" + Server.moneys);
            }
            
            payer.payment = "%f" + amount + " %3" + Server.moneys + " to " + target + "%3 on %f" + DateTime.Now.ToString(CultureInfo.InvariantCulture);                
            receiver.salary = "%f" + amount + " %3" + Server.moneys + " by " + p.color + p.name + "%3 on %f" + DateTime.Now.ToString(CultureInfo.InvariantCulture);
            receiver.money += amount;
            p.money -= amount;
            p.OnMoneyChanged();
            payer.money = p.money;
            Economy.UpdateEcoStats(payer);
            Economy.UpdateEcoStats(receiver);            
        }

        bool IsLegalPayment(Player p, int payer, int receiver, int amount) {
            if (receiver + amount > 16777215) { Player.SendMessage(p, "%cPlayers cannot have over %f16777215 %3" + Server.moneys); return false; }
            if (payer - amount < 0) { Player.SendMessage(p, "%cYou don't have enough %3" + Server.moneys); return false; }
            return true;
        }
        
        public override void Help(Player p) {
            Player.SendMessage(p, "%T/pay [player] [amount] ");
            Player.SendMessage(p, "%HPays <amount> %3" + Server.moneys + " %Hto [player]");
        }
    }
}
