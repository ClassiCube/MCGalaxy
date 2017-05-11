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
using MCGalaxy.Eco;

namespace MCGalaxy.Commands.Eco {
    public sealed class CmdPay : MoneyCmd {
        public override string name { get { return "pay"; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Guest; } }
        public CmdPay() { }

        public override void Use(Player p, string message) {
            MoneyCmdData data;
            if (!ParseArgs(p, message, false, "pay", out data)) return;
            
            int matches = 1;
            Player who = PlayerInfo.FindMatches(p, data.Name, out matches);
            if (matches > 1) return;
            if (p != null && p == who) { Player.Message(p, "You cannot pay yourself %3" + Server.moneys); return; }
            string target = null;
            int money, srcMoney = Player.IsSuper(p) ? int.MaxValue : p.money;
            
            if (who == null) {
                target = Economy.FindMatches(p, data.Name, out money);
                if (target == null) return;
                
                if (!IsLegalPayment(p, srcMoney, money, data.Amount)) return;
                money += data.Amount;
                Economy.UpdateMoney(target, money);
            } else {
                target = who.name; money = who.money;
                if (!IsLegalPayment(p, srcMoney, money, data.Amount)) return;
                who.SetMoney(who.money + data.Amount);
            }
            if (!Player.IsSuper(p)) p.SetMoney(p.money - data.Amount);
            MessageAll(p, "{0} %Spaid {1} &f{2} &3{3}{4}", target, data);

            Economy.EcoStats stats = Economy.RetrieveStats(target);
            stats.Salary = Format(p, " by " + data.SourceRaw, data);
            Economy.UpdateStats(stats);
            
            if (Player.IsSuper(p)) return;
            stats = Economy.RetrieveStats(p.name);
            string targetName = PlayerInfo.GetColoredName(p, target);
            stats.Payment = Format(p, " to " + targetName, data);
            Economy.UpdateStats(stats);
        }

        static bool IsLegalPayment(Player p, int payer, int receiver, int amount) {
            if (receiver + amount > 16777215) { 
                Player.Message(p, "%cPlayers cannot have over %f16777215 %3" + Server.moneys); return false; 
            }
            if (payer < amount) { 
                Player.Message(p, "%cYou don't have enough %3" + Server.moneys); return false; 
            }
            return true;
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/pay [player] [amount] <reason>");
            Player.Message(p, "%HPays [amount] &3{0} %Hto [player]", Server.moneys);
        }
    }
}
