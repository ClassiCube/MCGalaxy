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
    public sealed class CmdGive : MoneyCmd {
        public override string name { get { return "Give"; } }
        public override string shortcut { get { return "Gib"; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Admin; } }

        public override void Use(Player p, string message, CommandData data) {
            EcoTransaction trans;
            bool all = false;
            if (!ParseArgs(p, message, ref all, "give", out trans)) return;
            
            int matches = 1;
            Player who = PlayerInfo.FindMatches(p, trans.TargetName, out matches);
            if (matches > 1) return;
            int money = 0;
            
            if (who == null) {
                trans.TargetName = Economy.FindMatches(p, trans.TargetName, out money);
                if (trans.TargetName == null) return;
                
                if (ReachedMax(p, money, trans.Amount)) return;
                money += trans.Amount;
                Economy.UpdateMoney(trans.TargetName, money);
            } else {
                trans.TargetName = who.name; 
                money = who.money;
                
                if (ReachedMax(p, money, trans.Amount)) return;
                who.SetMoney(who.money + trans.Amount);
            }
            
            trans.TargetFormatted = p.FormatNick(trans.TargetName);
            trans.Type = EcoTransactionType.Give;
            OnEcoTransactionEvent.Call(trans);
        }
        
        static bool ReachedMax(Player p, int current, int amount) {
            if (current + amount > 16777215) {
                p.Message("&WPlayers cannot have over &316,777,215 &3" + Server.Config.Currency); return true;
            }
            return false;
        }
        
        public override void Help(Player p) {
            p.Message("&T/Give [player] [amount] <reason>");
            p.Message("&HGives [player] [amount] &3" + Server.Config.Currency);
        }
    }
}
