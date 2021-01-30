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

namespace MCGalaxy.Commands.Eco { 
    public sealed class CmdBalance : Command2 {        
        public override string name { get { return "Balance"; } }
        public override string shortcut { get { return "Money"; } }
        public override string type { get { return CommandTypes.Economy; } }
        public override CommandEnable Enabled { get { return CommandEnable.Economy; } }
        
        public override void Use(Player p, string message, CommandData data) {
            if (CheckSuper(p, message, "player name")) return;
            if (message.Length == 0) message = p.name;
            if (!Formatter.ValidName(p, message, "player")) return;
            
            int matches = 1;
            Player who  = PlayerInfo.FindMatches(p, message, out matches);
            if (matches > 1) return;
            
            string target = null;
            int money = 0;
            if (matches == 0) {
                target = Economy.FindMatches(p, message, out money);
                if (target == null) return;
            } else {
                target = who.name; money = who.money;
            }

            string targetName = p.FormatNick(target);
            p.Message("Economy stats for {0}&S:", targetName);
            p.Message(" Current balance: &f{0} &3{1}", money, Server.Config.Currency);
            
            Economy.EcoStats ecos = Economy.RetrieveStats(target);
            p.Message(" Total spent: &f" + ecos.TotalSpent + " &3" + Server.Config.Currency);
            Output(p, ecos.Purchase, "purchase");
            Output(p, ecos.Payment, "payment");
            Output(p, ecos.Salary, "receipt");
            Output(p, ecos.Fine, "fine");
        }
        
        const string dateFormat = "MM'/'dd'/'yyyy HH:mm:ss";
        static void Output(Player p, string value, string type) {
            if (value == null) return;
            
            if (!AdjustRelative(ref value, " on %f")) {
                AdjustRelative(ref value, " - Date: %f"); // old date format for purchases
            }
            p.Message(" Last {0}: {1}", type, value);
        }
        
        static bool AdjustRelative(ref string value, string dateStart) {
            int index = value.IndexOf(dateStart);
            if (index == -1) return false;
            
            string prefix = value.Substring(0, index);
            index += dateStart.Length; // skip over the date start bit
            
            const int dateLength = 19;
            string date = value.Substring(index, dateLength);

            index += dateLength;
            string suffix = "";
            if (index < value.Length)
                suffix = value.Substring(index);
            
            DateTime time;
            if (!DateTime.TryParseExact(date, dateFormat, null, 0, out time)) return false;
            
            TimeSpan delta = DateTime.Now - time;
            value = prefix + " &f" + delta.Shorten() + " ago" + suffix;
            return true;
        }

        public override void Help(Player p) {
            p.Message("&T/Balance [player]");
            p.Message("&HShows how much &3" + Server.Config.Currency + " &H[player] has, " +
                           "plus their most recent transactions.");
            p.Message("&HIf [player] is not given, shows your own balance.");
        }
    }
}
