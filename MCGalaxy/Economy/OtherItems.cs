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
using MCGalaxy.Util;

namespace MCGalaxy.Eco {
    
    public sealed class SnackItem : SimpleItem {
        
        public SnackItem() {
            Aliases = new string[] { "snack" };
            Price = 0;
        }
        
        public override string Name { get { return "Snack"; } }
        
        protected internal override void OnPurchase(Player p, string args) {
            if (DateTime.UtcNow < p.NextEat) {
                p.Message("You're still full - you need to wait at least " +
                          "10 seconds between snacks."); return;
            }          

            if (!CheckPrice(p)) return;
            TextFile eatFile = TextFile.Files["Eat"];
            eatFile.EnsureExists();
            
            string[] actions = eatFile.GetText();
            string action = "ate some food";
            if (actions.Length > 0)
                action = actions[new Random().Next(actions.Length)];
            
            if (!p.CheckCanSpeak("eat a snack")) return;
            Chat.MessageFrom(p, "λNICK &S" + action, null);
            p.CheckForMessageSpam();
            
            p.NextEat = DateTime.UtcNow.AddSeconds(10);
            // intentionally not using Economy.MakePurchase here
            p.SetMoney(p.money - Price);
        }
    }
}
