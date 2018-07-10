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
using MCGalaxy.Commands;
using MCGalaxy.DB;
using MCGalaxy.Games;

namespace MCGalaxy.Eco {
    
    public sealed class BlocksItem : SimpleItem {
        
        public BlocksItem() {
            Aliases = new string[] { "blocks", "bl", "b" };
            Price = 1;
            AllowsNoArgs = true;
        }
        
        public override string Name { get { return "10Blocks"; } }

        protected override void DoPurchase(Player p, string message, string[] args) {
            int count = 1;
            const string group = "Number of groups of 10 blocks";
            if (args.Length >= 2 && !CommandParser.GetInt(p, args[1], group, ref count, 0, 10)) return;
            
            if (p.money < Price * count) {
                p.Message("%WYou don't have enough &3{2} %Wto buy {1} {0}.",
                               Name, count * 10, ServerConfig.Currency); return;
            }
            
            ZSData data = ZSGame.Get(p);
            data.BlocksLeft += 10 * count;
            Economy.MakePurchase(p, Price * count, "%310Blocks: " + (10 * count));
        }
        
        protected internal override void OnStoreCommand(Player p) {
            p.Message("%T/Buy 10blocks [num]");
            p.Message("%HCosts &a{0} * [num] %H{1}", Price, ServerConfig.Currency);
            p.Message("Increases the blocks you are able to place by 10 * [num].");
        }
    }
    
    public sealed class QueueLevelItem : SimpleItem {
        
        public QueueLevelItem() {
            Aliases = new string[] { "queuelevel", "queuelvl", "queue" };
            Price = 150;
        }
        
        public override string Name { get { return "QueueLevel"; } }
        
        protected override void DoPurchase(Player p, string message, string[] args) {
            if (ZSGame.Instance.Picker.QueuedMap != null) {
                p.Message("Someone else has already queued a level."); return;
            }
            string map = Matcher.FindMaps(p, args[1]);
            if (map == null) return;
            
            UseCommand(p, "Queue", "level " + map);
            Economy.MakePurchase(p, Price, "%3QueueLevel: " + map);
        }
        
        protected internal override void OnStoreCommand(Player p) {
            p.Message("%T/Buy {0} [map]", Name);
            OutputItemInfo(p);
            p.Message("The map used for the next round of " +
                           "zombie survival will be the given map.");
        }
    }
    
    public sealed class InfectMessageItem : SimpleItem {
        
        public InfectMessageItem() {
            Aliases = new string[] { "infectmessage", "infectmsg" };
            Price = 150;
        }
        
        public override string Name { get { return "InfectMessage"; } }
        
        protected override void DoPurchase(Player p, string message, string[] args) {
            string text = message.SplitSpaces(2)[1]; // keep spaces this way
            bool hasAToken = false;
            for (int i = 0; i < text.Length; i++) {
                if (!CheckEscape(text, i, ref hasAToken)) {
                    p.Message("You can only use {0} and {1} for tokens in infect messages."); return;
                }
            }
            if (!hasAToken) {
                p.Message("You need to include a \"{0}\" (placeholder for zombie player) " +
                               "and/or a \"{1}\" (placeholder for human player) in the infect message."); return;
            }
            
            ZSData data = ZSGame.Get(p);
            if (data.InfectMessages == null) data.InfectMessages = new List<string>();
            data.InfectMessages.Add(text);
            
            ZSConfig.AppendPlayerInfectMessage(p.name, text);
            p.Message("&aAdded infect message: &f" + text);
            Economy.MakePurchase(p, Price, "%3InfectMessage: " + message);
        }
        
        static bool CheckEscape(string text, int i, ref bool hasAToken) {
            // Only {0} and {1} are allowed in infect messages for the Format() call
            if (text[i] != '{') return true;
            hasAToken = true;
            if ((i + 2) >= text.Length) return false;
            return (text[i + 1] == '0' || text[i + 1] == '1') && text[i + 2] == '}';
        }
    }
    
    public abstract class InvisibilityItem : SimpleItem {
        
        protected abstract int MaxPotions { get; }
        protected abstract int Duration { get; }
        protected abstract bool ForHumans { get; }
        
        protected internal override void OnBuyCommand(Player p, string message, string[] args) {
            if (p.money < Price) {
                p.Message("%WYou don't have enough &3{1} %Wto buy a {0}.", Name, ServerConfig.Currency); return;
            }
            if (!ZSGame.Instance.Running || !ZSGame.Instance.RoundInProgress) {
                p.Message("You can only buy an invisiblity potion " +
                               "when a round of zombie survival is in progress."); return;
            }
            
            ZSData data = ZSGame.Get(p);
            
            if (data.Invisible) { p.Message("You are already invisible."); return; }
            if (data.InvisibilityPotions >= MaxPotions) {
                p.Message("You cannot buy any more invisibility potions this round."); return;
            }
            if (ForHumans && data.Infected) {
                p.Message("Use %T/Buy zinvisibility %Sfor buying invisibility when you are a zombie."); return;
            }
            if (!ForHumans && !data.Infected) {
                p.Message("Use %T/Buy invisibility %Sfor buying invisibility when you are a human."); return;
            }
            
            DateTime end = ZSGame.Instance.RoundEnd;
            if (DateTime.UtcNow.AddSeconds(60) > end) {
                p.Message("You cannot buy an invisibility potion " +
                               "during the last minute of a round."); return;
            }
            
            data.Invisible = true;
            data.InvisibilityEnd = DateTime.UtcNow.AddSeconds(Duration);
            data.InvisibilityPotions++;
            int left = MaxPotions - data.InvisibilityPotions;
            
            p.Message("Lasts for &a{0} %Sseconds. You can buy &a{1} %Smore this round.", Duration, left);
            ZSGame.Instance.Map.Message(p.ColoredName + " %Svanished. &a*POOF*");
            Entities.GlobalDespawn(p, false, false);
            Economy.MakePurchase(p, Price, "%3Invisibility: " + Duration);
        }
        
        protected override void DoPurchase(Player p, string message, string[] args) { }
        
        protected internal override void OnStoreCommand(Player p) {
            p.Message("%T/Buy " + Name);
            OutputItemInfo(p);
            
            p.Message("Makes you invisible to {0} - %Wyou can still {1}",
                           ForHumans ? "zombies" : "humans",
                           ForHumans ? "be infected" : "infect humans");
            p.Message("Lasts for " + Duration + " seconds before you reappear.");
        }
    }
    
    public sealed class HumanInvisibilityItem : InvisibilityItem {
        
        public HumanInvisibilityItem() {
            Aliases = new string[] { "invisibility", "invisible", "invis" };
            Price = 3;
        }
        
        public override string Name { get { return "Invisibility"; } }
        protected override int MaxPotions { get { return ZSGame.Config.InvisibilityPotions; } }
        protected override int Duration { get { return ZSGame.Config.InvisibilityDuration; } }
        protected override bool ForHumans { get { return true; } }
    }
    
    public sealed class ZombieInvisibilityItem : InvisibilityItem {
        
        public ZombieInvisibilityItem() {
            Aliases = new string[] { "zinvisibility", "zinvisible", "zinvis" };
            Price = 3;
        }
        
        public override string Name { get { return "ZombieInvisibility"; } }
        protected override int MaxPotions { get { return ZSGame.Config.ZombieInvisibilityPotions; } }
        protected override int Duration { get { return ZSGame.Config.ZombieInvisibilityDuration; } }
        protected override bool ForHumans { get { return false; } }
    }
}
