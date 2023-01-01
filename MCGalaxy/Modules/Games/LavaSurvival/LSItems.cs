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

namespace MCGalaxy.Modules.Games.LS 
{    
    sealed class LifeItem : SimpleItem 
    {    
        public LifeItem() {
            Aliases = new string[] { "life", "lifes", "live", "lives" };
            Enabled = true;
            Price   = 20;
        }
        
        public override string Name { get { return "Life"; } }

        protected internal override void OnPurchase(Player p, string args) {
            if (!LSGame.Instance.RoundInProgress) {
                p.Message("You can only buy a life " +
                          "when a round of lava survival is in progress."); return;
            }
        	
        	// TODO: Don't allow buying when Config.MaxLives == 0
        	
        	if (!CheckPrice(p)) return;
        	
        	LSGame.Get(p).TimesDied--;
        	// TODO: announce lifes left
            Economy.MakePurchase(p, Price, "%3Life:");
        }
    }
}
