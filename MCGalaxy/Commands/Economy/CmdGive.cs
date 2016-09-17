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
using System.Globalization;
namespace MCGalaxy.Commands {
    public sealed class CmdGive : MoneyCmd {
        public override string name { get { return "give"; } }
        public override string shortcut { get { return "gib"; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Admin; } }       
        public CmdGive() { }

        public override void Use(Player p, string message) {
            MoneyCmdData data;
            if (!ParseArgs(p, message, false, "give", out data)) return;
            
            int matches = 1;
            Player who = PlayerInfo.FindMatches(p, data.Name, out matches);
            if (matches > 1) return;
            if (p != null && p == who) { Player.Message(p, "You cannot give yourself %3" + Server.moneys); return; }

            string target = null;
            int money = 0;
            if (who == null) {
                target = Economy.FindMatches(p, data.Name, out money);
                if (target == null) return;
                
                if (ReachedMax(p, money, data.Amount)) return;
                money += data.Amount;
                Economy.UpdateMoney(target, money);
            } else {
            	target = who.name; money = who.money;
                if (ReachedMax(p, money, data.Amount)) return;
                who.SetMoney(who.money + data.Amount);
            }
            MessageAll(p, "{0} %Sgave {1} &f{2} &3{3}{4}", target, data);
            
            Economy.EcoStats stats = Economy.RetrieveStats(target);
            stats.Salary = Format(p, " by " + data.SourceRaw, data);
            Economy.UpdateStats(stats);
        }
        
        static bool ReachedMax(Player p, int current, int amount) {
            if (current + amount > 16777215) {
                Player.Message(p, "%cPlayers cannot have over %316,777,215 %3" + Server.moneys); return true;
            }
            return false;
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/give [player] [amount] <reason>");
            Player.Message(p, "%HGives [player] [amount] %3" + Server.moneys);
        }
    }
}
