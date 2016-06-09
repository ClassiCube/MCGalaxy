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

namespace MCGalaxy.Commands {
    
    public sealed class CmdBalance : Command {
        
        public override string name { get { return "balance"; } }
        public override string shortcut { get { return "money"; } }
        public override string type { get { return CommandTypes.Economy; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Guest; } }
        public override CommandEnable Enabled { get { return CommandEnable.Economy; } }        
        
        public override void Use(Player p, string message) {
            Economy.EcoStats ecos;
            if (CheckSuper(p, message, "player name")) return;

            int matches = 1;
            Player who = message == "" ? p : PlayerInfo.FindOrShowMatches(p, message, out matches);
            if (matches > 1) return;
            if (matches == 0) {
                OfflinePlayer off = PlayerInfo.FindOfflineOrShowMatches(p, message);
                if (off == null) return;
                ecos = Economy.RetrieveEcoStats(off.name);
                Player.Message(p, "%3===Economy stats for: %f" + ecos.playerName + "%7(offline)%3===");
            } else {
                ecos = Economy.RetrieveEcoStats(who.name);
                Player.Message(p, "%3===Economy stats for: " + who.color + who.name + "%3===");
            }
            
            Player.Message(p, "Current balance: %f" + ecos.money + " %3" + Server.moneys);
            Player.Message(p, "Total spent: %f" + ecos.totalSpent + " %3" + Server.moneys);
            if (!(String.IsNullOrEmpty(ecos.purchase) || ecos.purchase == "%cNone"))
                Player.Message(p, "Last purchase: " + ecos.purchase);
            if (!(String.IsNullOrEmpty(ecos.payment) || ecos.payment == "%cNone"))
                Player.Message(p, "Last payment: " + ecos.payment);
            if (!(String.IsNullOrEmpty(ecos.salary) || ecos.salary == "%cNone"))
                Player.Message(p, "Last receipt: " + ecos.salary);
            if (!(String.IsNullOrEmpty(ecos.fine) || ecos.fine == "%cNone"))
                Player.Message(p, "Last fine: " + ecos.fine);
        }

        public override void Help(Player p) {
            Player.Message(p, "%T/balance <player>");
            Player.Message(p, "%HShows how much %3" + Server.moneys + " %H<player> has, " +
                               "plus their most recent transactions.");
            Player.Message(p, "%HIf <player> is not given, shows your own balance.");
        }
    }
}
