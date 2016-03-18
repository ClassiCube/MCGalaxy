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

namespace MCGalaxy.Eco {
    
    public sealed class BlocksItem : SimpleItem {
        
        public BlocksItem() {
            Aliases = new[] { "blocks", "bl", "b" };
            Price = 1;
        }
        
        public override string Name { get { return "Blocks"; } }

        protected override void OnBuyCommand(Player p, string message, string[] args) {
            byte count = 1;
            if (args.Length >= 3 && !byte.TryParse(args[2], out count) || count == 0 || count > 10) {
                Player.SendMessage(p, "Number of groups of 10 blocks to buy must be an integer between 1 and 10."); return;
            }
            if (!p.EnoughMoney(Price * count)) {
                Player.SendMessage(p, "%cYou don't have enough %3" + Server.moneys + 
                                   "%c to buy " +  (count * 10) +  " " + Name + "."); return;
            }
            
            p.blocksStacked += 10 * count;
            MakePurchase(p, Price, "%3Blocks: " + (10 * count));
        }
    }
}
