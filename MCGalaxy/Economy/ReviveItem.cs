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
        
        protected internal override void OnBuyCommand(Player p, string message, string[] args) {
            if (p.money < Price) {
                Player.Message(p, "&cYou don't have enough &3" + Server.moneys + "&c to buy a " + Name + "."); return;
            }
            if (!p.Game.Infected) {
                Player.Message(p, "You are already a human."); return;
            }
            if (!Server.zombie.Running || !Server.zombie.RoundInProgress) {
                Player.Message(p, "You can only buy an revive potion " +
                                   "when a round of zombie survival is in progress."); return;
            }
            
            DateTime end = Server.zombie.RoundEnd;
            if (DateTime.UtcNow.AddSeconds(ZombieGameProps.ReviveNoTime) > end) {
                Player.Message(p, ZombieGameProps.ReviveNoTimeMessage); return;
            }
            int count = Server.zombie.Infected.Count;
            if (count < ZombieGameProps.ReviveFewZombies) {
                Player.Message(p, ZombieGameProps.ReviveFewZombiesMessage); return;
            }
            if (p.Game.RevivesUsed >= ZombieGameProps.ReviveTimes) {
                Player.Message(p, "You cannot buy any more revive potions."); return;
            }
            if (p.Game.TimeInfected.AddSeconds(ZombieGameProps.ReviveTooSlow) < DateTime.UtcNow) {
                Server.zombie.CurLevel.ChatLevel(p.ColoredName + " %S" + ZombieGameProps.ReviveTooSlowMessage); return;
            }
            
            int chance = new Random().Next(0, 101);
            if (chance <= ZombieGameProps.ReviveChance) {
                Server.zombie.DisinfectPlayer(p);
                p.Game.RevivesUsed++;
                Server.zombie.CurLevel.ChatLevel(p.ColoredName + " %S" + ZombieGameProps.ReviveSuccessMessage);
            } else {
                Server.zombie.CurLevel.ChatLevel(p.ColoredName + " %S" + ZombieGameProps.ReviveFailureMessage);
            }
            Economy.MakePurchase(p, Price, "%3Revive:");
        }
        
        protected override void DoPurchase(Player p, string message, string[] args) { }
        
        protected internal override void OnStoreCommand(Player p) {
            base.OnStoreCommand(p);
            int time = ZombieGameProps.ReviveNoTime, expiry = ZombieGameProps.ReviveTooSlow;
            int potions = ZombieGameProps.ReviveTimes;
            Player.Message(p, "%T/buy " + Name);
            Player.Message(p, "%HCannot be used in the last &a" + time + " %Hseconds of a round.");
            Player.Message(p, "%HCan only be used within &a" + expiry + " %Hseconds after being infected.");
            Player.Message(p, "%HYou can only buy &a" + potions + " %Hrevive potions per round.");
        }
    }
}
