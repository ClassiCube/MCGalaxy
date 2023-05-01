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

        public override void OnPurchase(Player p, string args) {
            if (!LSGame.Instance.RoundInProgress) {
                p.Message("You can only buy a life " +
                          "when a round of lava survival is in progress."); return;
            }
            
            // TODO: Don't allow buying when Config.MaxLives == 0        
            if (!CheckPrice(p)) return;
            
            LSGame.Instance.AddLives(p, 1, false);
            Economy.MakePurchase(p, Price, "%3Life:");
        }
    }
    
    sealed class SpongesItem : SimpleItem 
    {
        public SpongesItem() {
            Aliases = new string[] { "sponge", "sponges" };
            Enabled = true;
            Price   = 2;
        }
        
        public override string Name { get { return "Sponges"; } }

        public override void OnPurchase(Player p, string args) {
            if (!LSGame.Instance.RoundInProgress) {
                p.Message("You can only buy sponges " +
                          "when a round of lava survival is in progress."); return;
            }   
            if (!CheckPrice(p)) return;
            
            // TODO avoid code duplication
            LSData data = LSGame.Get(p);
            data.SpongesLeft += 10;
            p.Message("Sponges left: &4{0}", data.SpongesLeft);
            Economy.MakePurchase(p, Price, "%3Sponges:");
        }
    }
    
    sealed class WaterItem : SimpleItem 
    {
        public WaterItem() {
            Aliases = new string[] { "water", "waters" };
            Enabled = true;
            Price   = 2;
        }
        
        public override string Name { get { return "Water"; } }

        public override void OnPurchase(Player p, string args) {
            if (!LSGame.Instance.RoundInProgress) {
                p.Message("You can only buy water " +
                          "when a round of lava survival is in progress."); return;
            }   
            if (!CheckPrice(p)) return;
            
            // TODO avoid code duplication
            LSData data = LSGame.Get(p);
            data.WaterLeft += 10;
            p.Message("Water blocks left: &4{0}", data.WaterLeft);
            Economy.MakePurchase(p, Price, "%3Water:");
        }
    }
}
