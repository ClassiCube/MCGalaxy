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
    public sealed class CmdTake : MoneyCmd {
        public override string name { get { return "Take"; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Admin; } }     

        public override void Use(Player p, string message) {
            EcoTransaction data;
            bool all = true;
            if (!ParseArgs(p, message, ref all, "take", out data)) return;
            
            int matches = 1;
            Player who = PlayerInfo.FindMatches(p, data.TargetName, out matches);
            if (matches > 1) return;
            if (p != null && p == who) { Player.Message(p, "%cYou can't take %3" + ServerConfig.Currency + "%c from yourself"); return; }
            
            int money = 0;
            if (who == null) {
                data.TargetName = Economy.FindMatches(p, data.TargetName, out money);
                if (data.TargetName == null) return;
                
                Take(ref money, all, data);
                Economy.UpdateMoney(data.TargetName, money);
            } else {
                data.TargetName = who.name;
                money = who.money;
                
                Take(ref money, all, data);
                who.SetMoney(money);
            }
            
            data.TargetFormatted = PlayerInfo.GetColoredName(p, data.TargetName);
            data.Type = EcoTransactionType.Take;
            OnEcoTransactionEvent.Call(data);
        }
        
        static void Take(ref int money, bool all, EcoTransaction data) {
            if (all || money < data.Amount) {
                data.Amount = money;
                money = 0;
            } else {
                money -= data.Amount;
            }
        }
        
        public override void Help(Player p){
            Player.Message(p, "%T/Take [player] [amount] <reason>");
            Player.Message(p, "%HTakes [amount] of &3" + ServerConfig.Currency + " %Sfrom [player]");
            Player.Message(p, "%T/Take [player] all <reason>");
            Player.Message(p, "%HTakes all the &3" + ServerConfig.Currency + " %Sfrom [player]");
        }
    }
}
