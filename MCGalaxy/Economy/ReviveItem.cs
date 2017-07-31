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
            Aliases = new string[] { "revive", "rev" };
            Price = 7;
        }
        
        public override string Name { get { return "Revive"; } }
        
        protected internal override void OnBuyCommand(Player p, string message, string[] args) {
            if (p.money < Price) {
                Player.Message(p, "&cYou don't have enough &3" + ServerConfig.Currency + "&c to buy a " + Name + "."); return;
            }
            if (!p.Game.Infected) {
                Player.Message(p, "You are already a human."); return;
            }
            if (!Server.zombie.Running || !Server.zombie.RoundInProgress) {
                Player.Message(p, "You can only buy an revive potion " +
                                   "when a round of zombie survival is in progress."); return;
            }
            
            DateTime end = Server.zombie.RoundEnd;
            if (DateTime.UtcNow.AddSeconds(ZSConfig.ReviveNoTime) > end) {
                Player.Message(p, ZSConfig.ReviveNoTimeMessage); return;
            }
            int count = Server.zombie.Infected.Count;
            if (count < ZSConfig.ReviveFewZombies) {
                Player.Message(p, ZSConfig.ReviveFewZombiesMessage); return;
            }
            if (p.Game.RevivesUsed >= ZSConfig.ReviveTimes) {
                Player.Message(p, "You cannot buy any more revive potions."); return;
            }
            if (p.Game.TimeInfected.AddSeconds(ZSConfig.ReviveTooSlow) < DateTime.UtcNow) {
                Player.Message(p, "&cYou can only revive within the first {0} seconds after you were infected.",
                               ZSConfig.ReviveTooSlow); return;
            }
            
            int chance = new Random().Next(1, 101);
            if (chance <= ZSConfig.ReviveChance) {
                Server.zombie.DisinfectPlayer(p);
                Server.zombie.CurLevel.ChatLevel(p.ColoredName + " %S" + ZSConfig.ReviveSuccessMessage);
            } else {
                Server.zombie.CurLevel.ChatLevel(p.ColoredName + " %S" + ZSConfig.ReviveFailureMessage);
            }
            Economy.MakePurchase(p, Price, "%3Revive:");
            p.Game.RevivesUsed++;
        }
        
        protected override void DoPurchase(Player p, string message, string[] args) { }
        
        protected internal override void OnStoreCommand(Player p) {
            int time = ZSConfig.ReviveNoTime, expiry = ZSConfig.ReviveTooSlow;
            int potions = ZSConfig.ReviveTimes;
            Player.Message(p, "%T/Buy " + Name);
            OutputItemInfo(p);
            
            Player.Message(p, "Lets you rejoin the humans - &cnot guaranteed to always work");
            Player.Message(p, "  Cannot be used in the last &a" + time + " %Sseconds of a round.");
            Player.Message(p, "  Can only be used within &a" + expiry + " %Sseconds after being infected.");
            Player.Message(p, "  Can only buy &a" + potions + " %Srevive potions per round.");
        }
    }
}
