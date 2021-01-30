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

namespace MCGalaxy.Commands.Eco {
    public sealed class CmdBuy : Command2 {
        public override string name { get { return "Buy"; } }
        public override string shortcut { get { return "Purchase"; } }
        public override string type { get { return CommandTypes.Economy; } }
        public override CommandEnable Enabled { get { return CommandEnable.Economy; } }
        public override bool SuperUseable { get { return false; } }
        
        public override void Use(Player p, string message, CommandData data) {
            string[] parts = message.SplitSpaces(2);
            Item item = Economy.GetItem(parts[0]);
            if (item == null) { Help(p); return; }

            if (!item.Enabled) {
                p.Message("&WThe {0} item is not currently buyable.", item.Name); return;
            }
            if (data.Rank < item.PurchaseRank) {
                Formatter.MessageNeedMinPerm(p, "+ can purchase a " + item.Name, item.PurchaseRank); return;
            }
            item.OnPurchase(p, parts.Length == 1 ? "" : parts[1]);
        }
        
        public override void Help(Player p) {
            p.Message("&T/Buy [item] [value] <map name>");
            p.Message("&Hmap name is only used for &T/Buy map&H.");
            p.Message("&HUse &T/Store [item] &Hto see more information for an item.");
            p.Message("&H  Available items: &S" + Economy.EnabledItemNames());
        }
    }
}
