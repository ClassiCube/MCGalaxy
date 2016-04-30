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
using System.Collections.Generic;
using MCGalaxy.Games;

namespace MCGalaxy.Eco {
    
    public sealed class ReviveItem : SimpleItem {
        
        public ReviveItem() {
            Aliases = new[] { "revive", "rev" };
            Price = 7;
        }
        
        public override string Name { get { return "Revive"; } }
        
        protected internal override void OnBuyCommand(Command cmd, Player p, 
                                             string message, string[] args) {
            if (p.money < Price) {
                Player.SendMessage(p, "%cYou don't have enough %3" + Server.moneys + "%c to buy a " + Name + "."); return;
            }
            if (!p.Game.Infected) {
                Player.SendMessage(p, "You are already a human."); return;
            }
            if (!Server.zombie.Running || !Server.zombie.RoundInProgress) {
                Player.SendMessage(p, "You can only buy an revive potion " +
                                   "when a round of zombie survival is in progress."); return;
            }
            
            DateTime end = Server.zombie.RoundEnd;
            if (DateTime.UtcNow.AddSeconds(ZombieGame.ReviveNoTime) > end) {
                Player.SendMessage(p, ZombieGame.ReviveNoTimeMessage); return;
            }
            int count = Server.zombie.Infected.Count;
            if (count < ZombieGame.ReviveFewZombies) {
                Player.SendMessage(p, ZombieGame.ReviveFewZombiesMessage); return;
            }
            
            // TODO: finish this
            int chance = new Random().Next(0, 101);
            if (chance <= ZombieGame.ReviveChance) {
                Server.zombie.DisinfectPlayer(p);
            } else {
                
            }
            Economy.MakePurchase(p, Price, "%3Revive:");
        }
        
        protected override void OnBuyCommand(Player p, string message, string[] args) { }
        
        protected internal override void OnStoreCommand(Player p) {
            base.OnStoreCommand(p);
            int time = ZombieGame.ReviveNoTime, expiry = ZombieGame.ReviveTooSlow;
            Player.SendMessage(p, "Syntax: %T/buy " + Name);
            Player.SendMessage(p, "%HCannot be used in the last &a" + time + " %Hseconds of a round.");
            Player.SendMessage(p, "%HCan only be used within &a" + expiry + " %Hseconds after being infected.");
        }
    }
}
