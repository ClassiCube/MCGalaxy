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
    
    public sealed class BlocksItem : SimpleItem {
        
        public BlocksItem() {
            Aliases = new[] { "blocks", "bl", "b" };
            Price = 1;
        }
        
        public override string Name { get { return "10Blocks"; } }

        protected override void OnBuyCommand(Player p, string message, string[] args) {
            byte count = 1;
            if (args.Length >= 3 && !byte.TryParse(args[2], out count) || count == 0 || count > 10) {
                Player.Message(p, "Number of groups of 10 blocks to buy must be an integer between 1 and 10."); return;
            }
            if (p.money < Price * count) {
                Player.Message(p, "&cYou don't have enough &3{2} &cto buy {1} {0}.",
                               Name, count * 10, Server.moneys); return;
            }
            
            p.Game.BlocksLeft += 10 * count;
            Economy.MakePurchase(p, Price * count, "%310Blocks: " + (10 * count));
        }
        
        protected internal override void OnStoreCommand(Player p) {
            Player.Message(p, "Syntax: %T/buy 10blocks [num]");
            Player.Message(p, "Increases the blocks you are able to place by 10 * [num].");
            Player.Message(p, "Costs &f{0} * [num] &3{1}", Price, Server.moneys);
        }
    }
    
    public sealed class QueueLevelItem : SimpleItem {
        
        public QueueLevelItem() {
            Aliases = new[] { "queuelevel", "queuelvl", "queue" };
            Price = 150;
        }
        
        public override string Name { get { return "QueueLevel"; } }
        
        protected override void OnBuyCommand(Player p, string message, string[] args) {
            if (Server.zombie.QueuedLevel != null) {
                Player.Message(p, "Someone else has already queued a level."); return;
            }
            string map = LevelInfo.FindMapMatches(p, message);
            if (map == null) return;
            
            Command.all.Find("queue").Use(p, "level " + map);
            Economy.MakePurchase(p, Price, "%3QueueLevel: " + map);
        }
    }
    
    public sealed class InfectMessageItem : SimpleItem {
        
        public InfectMessageItem() {
            Aliases = new[] { "infectmessage", "infectmsg" };
            Price = 150;
        }
        
        public override string Name { get { return "InfectMessage"; } }
        
        protected override void OnBuyCommand(Player p, string message, string[] args) {
            string text = message.SplitSpaces(2)[1]; // keep spaces this way
            bool hasAToken = false;
            for (int i = 0; i < text.Length; i++) {
                if (!CheckEscape(text, i, ref hasAToken)) {
                    Player.Message(p, "You can only use {0} and {1} for tokens in infect messages."); return;
                }
            }
            if (!hasAToken) {
                Player.Message(p, "You need to include a \"{0}\" (placeholder for zombie player) " +
                               "and/or a \"{1}\" (placeholder for human player) in the infect message."); return;
            }
            
            PlayerDB.AppendInfectMessage(p.name, text);
            if (p.Game.InfectMessages == null) p.Game.InfectMessages = new List<string>();
            p.Game.InfectMessages.Add(text);
            Player.Message(p, "%aAdded infect message: &f" + text);
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
        
        protected internal override void OnBuyCommand(Command cmd, Player p,
                                                      string message, string[] args) {
            if (p.money < Price) {
                Player.Message(p, "%cYou don't have enough &3{1} &c to buy a {0}.", Name, Server.moneys); return;
            }
            if (p.Game.Invisible) { Player.Message(p, "You are already invisible."); return; }
            if (p.Game.InvisibilityPotions >= MaxPotions) {
                Player.Message(p, "You cannot buy any more invisibility potions this round."); return;
            }
            if (ForHumans && p.Game.Infected) {
                Player.Message(p, "Use %T/buy zinvisibility %Sfor buying invisibility when you are a zombie."); return;
            }
            if (!ForHumans && !p.Game.Infected) {
                Player.Message(p, "Use %T/buy invisibility %Sfor buying invisibility when you are a human."); return;
            }
            
            if (!Server.zombie.Running || !Server.zombie.RoundInProgress) {
                Player.Message(p, "You can only buy an invisiblity potion " +
                               "when a round of zombie survival is in progress."); return;
            }
            
            DateTime end = Server.zombie.RoundEnd;
            if (DateTime.UtcNow.AddSeconds(60) > end) {
                Player.Message(p, "You cannot buy an invisibility potion " +
                               "during the last minute of a round."); return;
            }
            p.Game.Invisible = true;
            p.Game.InvisibilityEnd = DateTime.UtcNow.AddSeconds(Duration);
            p.Game.InvisibilityPotions++;
            int left = MaxPotions - p.Game.InvisibilityPotions;
            
            Player.Message(p, "Lasts for &a{0} %Sseconds. You can buy &a{1} %Smore this round.", Duration, left);
            Server.zombie.CurLevel.ChatLevel(p.ColoredName + " %Svanished. &a*POOF*");
            Entities.GlobalDespawn(p, false);
            Economy.MakePurchase(p, Price, "%3Invisibility: " + Duration);
        }
        
        protected override void OnBuyCommand(Player p, string message, string[] args) { }
        
        protected internal override void OnStoreCommand(Player p) {
            base.OnStoreCommand(p);
            Player.Message(p, "%HLasts for " + Duration + " seconds before you reappear.");
        }
    }
    
    public sealed class HumanInvisibilityItem : InvisibilityItem {
        
        public HumanInvisibilityItem() {
            Aliases = new[] { "invisibility", "invisible", "invis" };
            Price = 3;
        }
        
        public override string Name { get { return "Invisibility"; } }
        protected override int MaxPotions { get { return ZombieGame.InvisibilityPotions; } }
        protected override int Duration { get { return ZombieGame.InvisibilityDuration; } }
        protected override bool ForHumans { get { return true; } }
    }
    
    public sealed class ZombieInvisibilityItem : InvisibilityItem {
        
        public ZombieInvisibilityItem() {
            Aliases = new[] { "zinvisibility", "zinvisible", "zinvis" };
            Price = 3;
        }
        
        public override string Name { get { return "ZombieInvisibility"; } }
        protected override int MaxPotions { get { return ZombieGame.ZombieInvisibilityPotions; } }
        protected override int Duration { get { return ZombieGame.ZombieInvisibilityDuration; } }
        protected override bool ForHumans { get { return false; } }
    }
}
