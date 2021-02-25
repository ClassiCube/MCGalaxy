/*
    Copyright 2011 MCForge
        
    Dual-licensed under the Educational Community License, Version 2.0 and
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
using MCGalaxy.Events.EconomyEvents;

namespace MCGalaxy.Commands.Eco {
    public sealed class CmdPay : MoneyCmd {
        public override string name { get { return "Pay"; } }
        public override bool MessageBlockRestricted { get { return true; } }
        
        public override void Use(Player p, string message, CommandData data) {
            EcoTransaction trans;
            bool all = false;
            if (!ParseArgs(p, message, ref all, "pay", out trans)) return;
            
            // Player can use /pay messages to bypass a mute
            // TODO: Make MessageCmd.CanSpeak more generic so that can be used here instead
            if (trans.Reason != null && !p.CanSpeak()) {
                p.Message("&WCannot specify a payment reason, as you cannot currently speak");
                return;
            }
            
            int matches = 1;
            Player who = PlayerInfo.FindMatches(p, trans.TargetName, out matches);
            if (matches > 1) return;
            if (p == who) { p.Message("&WYou cannot pay yourself &3" + Server.Config.Currency); return; }
            int money, srcMoney = p.IsSuper ? int.MaxValue : p.money;
            
            if (who == null) {
                trans.TargetName = Economy.FindMatches(p, trans.TargetName, out money);
                if (trans.TargetName == null) return;
                
                if (!IsLegalPayment(p, srcMoney, money, trans.Amount)) return;
                money += trans.Amount;
                Economy.UpdateMoney(trans.TargetName, money);
            } else {
                trans.TargetName = who.name; 
                money = who.money;
                
                if (!IsLegalPayment(p, srcMoney, money, trans.Amount)) return;
                who.SetMoney(who.money + trans.Amount);
            }
            
            trans.TargetFormatted = p.FormatNick(trans.TargetName);
            trans.Type = EcoTransactionType.Payment;
            OnEcoTransactionEvent.Call(trans);
        }

        static bool IsLegalPayment(Player p, int payer, int receiver, int amount) {
            if (receiver + amount > 16777215) { 
                p.Message("&WPlayers cannot have over &f16777215 &3" + Server.Config.Currency); return false; 
            }
            if (payer < amount) { 
                p.Message("&WYou don't have enough &3" + Server.Config.Currency); return false; 
            }
            return true;
        }
        
        public override void Help(Player p) {
            p.Message("&T/Pay [player] [amount] <reason>");
            p.Message("&HPays [amount] &3{0} &Hto [player]", Server.Config.Currency);
        }
    }
}
