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
        
        public override void Use(Player p, string message) {
            EcoTransaction data;
            bool all = false;
            if (!ParseArgs(p, message, ref all, "pay", out data)) return;
            
            int matches = 1;
            Player who = PlayerInfo.FindMatches(p, data.TargetName, out matches);
            if (matches > 1) return;
            if (p != null && p == who) { Player.Message(p, "You cannot pay yourself &3" + ServerConfig.Currency); return; }
            int money, srcMoney = Player.IsSuper(p) ? int.MaxValue : p.money;
            
            if (who == null) {
                data.TargetName = Economy.FindMatches(p, data.TargetName, out money);
                if (data.TargetName == null) return;
                
                if (!IsLegalPayment(p, srcMoney, money, data.Amount)) return;
                money += data.Amount;
                Economy.UpdateMoney(data.TargetName, money);
            } else {
                data.TargetName = who.name; 
                money = who.money;
                
                if (!IsLegalPayment(p, srcMoney, money, data.Amount)) return;
                who.SetMoney(who.money + data.Amount);
            }
            
            data.TargetFormatted = PlayerInfo.GetColoredName(p, data.TargetName);
            data.Type = EcoTransactionType.Payment;
            OnEcoTransactionEvent.Call(data);
        }

        static bool IsLegalPayment(Player p, int payer, int receiver, int amount) {
            if (receiver + amount > 16777215) { 
                Player.Message(p, "&cPlayers cannot have over &f16777215 &3" + ServerConfig.Currency); return false; 
            }
            if (payer < amount) { 
                Player.Message(p, "&cYou don't have enough &3" + ServerConfig.Currency); return false; 
            }
            return true;
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/Pay [player] [amount] <reason>");
            Player.Message(p, "%HPays [amount] &3{0} %Hto [player]", ServerConfig.Currency);
        }
    }
}
