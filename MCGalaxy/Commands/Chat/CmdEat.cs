/*
    Copyright 2015 MCGalaxy
    
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

namespace MCGalaxy.Commands.Chatting {  
    public sealed class CmdEat : Command2 {
        public override string name { get { return "Eat"; } }
        public override string type { get { return CommandTypes.Chat; } }
        
        // Custom command, so can still be used even when economy is disabled
        public override void Use(Player p, string message, CommandData data) {
            Item item = Economy.GetItem("Snack");
            item.OnPurchase(p, message);
        }
     
        public override void Help(Player p) {
            SimpleItem item = (SimpleItem)Economy.GetItem("Snack");
            p.Message("&T/Eat &H- Eats a random snack.");
            
            if (item.Price == 0) return;
            p.Message("&HCosts {0} &3{1} &Heach time", item.Price, Server.Config.Currency);
        }
    }
}
