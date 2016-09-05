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
            if (CheckSuper(p, message, "player name")) return;
            if (!Formatter.ValidName(p, message, "player")) return;
            int matches = 1;
            Player who = message == "" ? p : PlayerInfo.FindMatches(p, message, out matches);
            if (matches > 1) return;
            
            string target = null;
            int money = 0;
            if (matches == 0) {
                target = Economy.FindMatches(p, message, out money);
                if (target == null) return;
            } else {
                target = who.name; money = who.money;
            }

            string targetName = PlayerInfo.GetColoredName(p, target);
            Player.Message(p, "Economy stats for {0}%S:", targetName);
            Player.Message(p, " Current balance: &f{0} &3{1}", money, Server.moneys);
            
            Economy.EcoStats ecos = Economy.RetrieveStats(target);
            Player.Message(p, " Total spent: &f" + ecos.TotalSpent + " &3" + Server.moneys);
            Output(p, ecos.Purchase, "purchase");
            Output(p, ecos.Payment, "payment");
            Output(p, ecos.Salary, "receipt");
            Output(p, ecos.Fine, "fine");
        }
        
        const string dateFormat = "MM'/'dd'/'yyyy hh:mm:ss";
        static void Output(Player p, string value, string type) {
            if (String.IsNullOrEmpty(value) || value == "%cNone") return;
            
            int timeIndex = value.IndexOf(" on %f");
            if (timeIndex == -1) {
                Player.Message(p, " Last {0}: {1}", type, value); return;
            }            
            string msg = value.Substring(0, timeIndex);
            string date = value.Substring(timeIndex + 6);
            
            // Attempt to show relative time
            DateTime time;
            if (DateTime.TryParseExact(date, dateFormat, null, 0, out time)) {
                TimeSpan delta = DateTime.Now - time;
                value = msg + " %f" + delta.Shorten() + " ago";
            }
            Player.Message(p, " Last {0}: {1}", type, value);
        }

        public override void Help(Player p) {
            Player.Message(p, "%T/balance [player]");
            Player.Message(p, "%HShows how much %3" + Server.moneys + " %H<player> has, " +
                           "plus their most recent transactions.");
            Player.Message(p, "%HIf [player] is not given, shows your own balance.");
        }
    }
}
