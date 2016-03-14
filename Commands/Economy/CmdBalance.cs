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
        
        public override void Use(Player p, string message) {
            Economy.EcoStats ecos;
            if (p == null && message == "") {
                Player.SendMessage(p, "You must provide a name when using the command from console."); return;
            }

            if (message != "") {
                Player who = PlayerInfo.Find(message);
                if (who == null) {
                    ecos = Economy.RetrieveEcoStats(message);
                    Player.SendMessage(p, "%3===Economy stats for: %f" + ecos.playerName + "%7(offline)%3===");
                } else {
                    ecos = Economy.RetrieveEcoStats(who.name);
                    Player.SendMessage(p, "%3===Economy stats for: " + who.color + who.name + "%3===");
                }
            } else {
                ecos = Economy.RetrieveEcoStats(p.name);
                Player.SendMessage(p, "%3===Economy stats for: " + p.color + p.name + "%3===");
            }
            
            Player.SendMessage(p, "Current balance: %f" + ecos.money + " %3" + Server.moneys);
            Player.SendMessage(p, "Total spent: %f" + ecos.totalSpent + " %3" + Server.moneys);
            if (!String.IsNullOrEmpty(ecos.purchase))
                Player.SendMessage(p, "Last purchase: " + ecos.purchase);
            if (!String.IsNullOrEmpty(ecos.payment))
                Player.SendMessage(p, "Last payment: " + ecos.payment);
            if (!String.IsNullOrEmpty(ecos.salary))
                Player.SendMessage(p, "Last receipt: " + ecos.salary);
            if (!String.IsNullOrEmpty(ecos.fine))
                Player.SendMessage(p, "Last fine: " + ecos.fine);
        }

        public override void Help(Player p) {
            Player.SendMessage(p, "%T/balance <player>");
            Player.SendMessage(p, "%HShows how much %3" + Server.moneys + " %H<player> has, " +
                               "plus their most recent transactions.");
            Player.SendMessage(p, "%HIf <player> is not given, shows how much %3" + Server.moneys + " %Hyou have.");
        }
    }
}
