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

namespace MCGalaxy.Commands {
    public sealed class CmdPay : MoneyCmd {
        public override string name { get { return "pay"; } }
        public override string shortcut { get { return ""; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Banned; } }    
        public CmdPay() { }

        public override void Use(Player p, string message) {
            MoneyCmdData data;
            if (!ParseArgs(p, message, false, "pay", out data)) return;
            
            int matches = 1;
            Player who = PlayerInfo.FindMatches(p, data.Name, out matches);
            if (matches > 1) return;
            if (p != null && p == who) { Player.Message(p, "You cannot pay yourself %3" + Server.moneys); return; }
            string target = null;
            int money;
            
            if (who == null) {
                target = Economy.FindMatches(p, data.Name, out money);
                if (target == null) return;
                
                if (!IsLegalPayment(p, p.money, money, data.Amount)) return;
                money += data.Amount;
                Economy.UpdateMoney(target, money);
            } else {
                target = who.name; money = who.money;
                if (!IsLegalPayment(p, p.money, money, data.Amount)) return;
                who.SetMoney(who.money + data.Amount);
            }

            p.SetMoney(p.money - data.Amount);
            string targetName = PlayerInfo.GetColoredName(p, target);
            Chat.MessageAll("{0} %Spaid {1} &f{2} &3{3}",
                            data.Source, targetName, data.Amount, Server.moneys);
            
            Economy.EcoStats stats = Economy.RetrieveStats(p.name);
            stats.Payment = "%f" + data.Amount + " %3" + Server.moneys + " to " 
                + target + "%3 on %f" + DateTime.Now.ToString(CultureInfo.InvariantCulture);
            Economy.UpdateStats(stats);

            stats = Economy.RetrieveStats(target);            
            stats.Salary = "%f" + data.Amount + " %3" + Server.moneys + " by " 
                + p.color + p.name + "%3 on %f" + DateTime.Now.ToString(CultureInfo.InvariantCulture);
            Economy.UpdateStats(stats);
        }

        bool IsLegalPayment(Player p, int payer, int receiver, int amount) {
            if (receiver + amount > 16777215) { Player.Message(p, "%cPlayers cannot have over %f16777215 %3" + Server.moneys); return false; }
            if (payer - amount < 0) { Player.Message(p, "%cYou don't have enough %3" + Server.moneys); return false; }
            return true;
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/pay [player] [amount] ");
            Player.Message(p, "%HPays [amount] &3" + Server.moneys + " %Hto [player]");
        }
    }
}
