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
    public sealed class CmdTake : MoneyCmd {
        public override string name { get { return "take"; } }
        public override string shortcut { get { return ""; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Admin; } }     
        public CmdTake() { }

        public override void Use(Player p, string message) {
            MoneyCmdData data;
            if (!ParseArgs(p, message, true, "take", out data)) return;
            
            int matches = 1;
            Player who = PlayerInfo.FindMatches(p, data.Name, out matches);
            if (matches > 1) return;
            if (p != null && p == who) { Player.Message(p, "%cYou can't take %3" + Server.moneys + "%c from yourself"); return; }
            
            string target = null;
            int money = 0;
            if (who == null) {
                target = Economy.FindMatches(p, data.Name, out money);
                if (target == null) return;
                Take(ref money, ref data);
                Economy.UpdateMoney(target, money);
            } else {
                target = who.name; money = who.money;
                Take(ref money, ref data);
                who.SetMoney(money);
            }
            MessageAll(p, "{0} %Stook &f{2} &3{3} %Sfrom {1}{4}", target, data);
            
            Economy.EcoStats stats = Economy.RetrieveStats(target);
            stats.Fine = Format(p, " by " + data.SourceRaw, data);
            Economy.UpdateStats(stats);
        }
        
        static void Take(ref int money, ref MoneyCmdData data) {
            if (data.All || money < data.Amount) {
                data.Amount = money;
                money = 0;
            } else {
                money -= data.Amount;
            }
        }
        
        public override void Help(Player p){
            Player.Message(p, "%T/take [player] [amount] <reason>");
            Player.Message(p, "%HTakes [amount] of &3" + Server.moneys + " %Sfrom [player]");
            Player.Message(p, "%T/take [player] all <reason>");
            Player.Message(p, "%HTakes all the &3" + Server.moneys + " %Sfrom [player]");
        }
    }
}
