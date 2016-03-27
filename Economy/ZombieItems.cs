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

namespace MCGalaxy.Eco {
    
    public sealed class BlocksItem : SimpleItem {
        
        public BlocksItem() {
            Aliases = new[] { "blocks", "bl", "b" };
            Price = 1;
        }
        
        public override string Name { get { return "Blocks"; } }

        protected override void OnBuyCommand(Player p, string message, string[] args) {
            byte count = 1;
            if (args.Length >= 3 && !byte.TryParse(args[2], out count) || count == 0 || count > 10) {
                Player.SendMessage(p, "Number of groups of 10 blocks to buy must be an integer between 1 and 10."); return;
            }
            if (p.money < Price * count) {
                Player.SendMessage(p, "%cYou don't have enough %3" + Server.moneys + 
                                   "%c to buy " +  (count * 10) +  " " + Name + "."); return;
            }
            
            p.blockCount += 10 * count;
            MakePurchase(p, Price * count, "%3Blocks: " + (10 * count));
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
                Player.SendMessage(p, "Someone else has already queued a level."); return;
            }
            if (!LevelInfo.ExistsOffline(message)) {
                Player.SendMessage(p, "Given level does not exist."); return;
            }
            
            Command.all.Find("queue").Use(p, "level " + message);
            MakePurchase(p, Price, "%3QueueLevel: " + message);
        }
    }
    
    public sealed class InfectMessageItem : SimpleItem {
        
        public InfectMessageItem() {
            Aliases = new[] { "infectmessage", "infectmsg" };
            Price = 150;
        }
        
        public override string Name { get { return "InfectMessage"; } }
        
        static char[] trimChars = { ' ' };
        protected override void OnBuyCommand(Player p, string message, string[] args) {
            string text = message.Split(trimChars, 2)[1]; // keep spaces this way
            bool hasAToken = false;
            for (int i = 0; i < text.Length; i++) {
                if (!CheckEscape(text, i, ref hasAToken)) { 
                    Player.SendMessage(p, "You can only use {0} and {1} for tokens in infect messages."); return; 
                }
            }
            if (!hasAToken) {
                Player.SendMessage(p, "You need to include a \"{0}\" (placeholder for zombie player) " +
                                   "and/or a \"{1}\" (placeholder for human player) in the infect message."); return;
            }
            
            PlayerDB.AppendInfectMessage(p.name, text);
            if (p.infectMessages == null) p.infectMessages = new List<string>();
            p.infectMessages.Add(text);
            Player.SendMessage(p, "%aAdded infect message: %f" + text);
            MakePurchase(p, Price, "%3InfectMessage: " + message);
        }
        
        static bool CheckEscape(string text, int i, ref bool hasAToken) {
            // Only {0} and {1} are allowed in infect messages for the Format() call
            if (text[i] != '{') return true;
            hasAToken = true;
            if ((i + 2) >= text.Length) return false;
            return (text[i + 1] == '0' || text[i + 1] == '1') && text[i + 2] == '}';
        }
    }
}
