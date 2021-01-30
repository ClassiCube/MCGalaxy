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
    public sealed class CmdStore : Command2 {
        public override string name { get { return "Store"; } }
        public override string shortcut { get { return "Shop"; } }
        public override string type { get { return CommandTypes.Economy; } }
        public override CommandEnable Enabled { get { return CommandEnable.Economy; } }
        public override CommandAlias[] Aliases {
            get { return new[] { new CommandAlias("Item") }; }
        }

        public override void Use(Player p, string message, CommandData data) {
            if (message.Length == 0) {
                foreach (Item item in Economy.Items) {
                    if (!item.Enabled) continue;
                    item.OnStoreOverview(p);
                }
                p.Message("&HUse &T/Store [item] &Hto see more information about that item.");
            } else {
                Item item = Economy.GetItem(message);
                if (item == null) { Help(p); return; }                

                if (!item.Enabled) {
                    p.Message("&WThe " + item.ShopName + " item is not currently buyable."); return;
                }
                item.OnStoreCommand(p);
            }
        }
        
        public override void Help(Player p) {
            p.Message("&T/Store [item]");
            p.Message("&HViews information about the specific item, such as its cost.");
            p.Message("&T/Store");
            p.Message("&HViews information about all enabled items.");
            p.Message("&H  Available items: &S" + Economy.EnabledItemNames());
        }
    }
}
