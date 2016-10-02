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
using System.IO;

namespace MCGalaxy.Eco {
    
    /// <summary> An abstract object that can be bought in the economy. (e.g. a rank, title, levels, etc) </summary>
    public abstract class Item {
        
        /// <summary> Simple name for this item. </summary>
        public abstract string Name { get; }
        
        /// <summary> The minimum permission/rank required to purchase this item. </summary>
        public LevelPermission PurchaseRank = LevelPermission.Guest;
        
        /// <summary> Simple name displayed in /shop, defaults to item name. </summary>
        public virtual string ShopName { get { return Name; } }
        
        /// <summary> Other common names for this item. </summary>
        public string[] Aliases;
        
        /// <summary> Whether this item can currently be bought in the economy. </summary>
        public bool Enabled;
        
        /// <summary> Reads the given property of this item from the economy.properties file. </summary>
        /// <remarks> args is line split by the : character. </remarks>
        public abstract void Parse(string line, string[] args);
        
        /// <summary> Writes the properties of this item to the economy.properties file. </summary>
        public abstract void Serialise(StreamWriter writer);
        

        internal void Setup(Player p, string[] args) {
            switch (args[1].ToLower()) {
                case "enable":
                    Player.Message(p, "%aThe {0} item is now enabled.", Name);
                    Enabled = true; break;
                case "disable":
                    Player.Message(p, "%aThe {0} item is now disabled.", Name);
                    Enabled = false; break;
                case "purchaserank":
                    if (args.Length == 2) { Player.Message(p, "You need to provide a rank name."); return; }
                    Group grp = Group.FindMatches(p, args[2]);
                    if (grp == null) return;
                    
                    PurchaseRank = grp.Permission;
                    Player.Message(p, "Min purchase rank for {0} item set to {1}%S.", Name, grp.ColoredName);
                    break;
                default:
                    OnSetupCommand(p, args); break;
            }
        }
        
        /// <summary> Called when the player does /buy [item name] &lt;value&gt; </summary>
        protected internal abstract void OnBuyCommand(Player p, string message, string[] args);
        
        /// <summary> Called when the player does /eco [item name] [option] &lt;value&gt; </summary>
        protected internal abstract void OnSetupCommand(Player p, string[] args);
        
        /// <summary> Called when the player does /eco help [item name] </summary>
        protected internal virtual void OnSetupCommandHelp(Player p) {
            Player.Message(p, "%T/eco {0} enable/disable", Name.ToLower());
            Player.Message(p, "%HEnables/disables purchasing this item.");
            Player.Message(p, "%T/eco {0} purchaserank [rank]", Name.ToLower());
            Player.Message(p, "%HSets the lowest rank which can purchase this item.");
        }

        /// <summary> Called when the player does /store </summary>
        protected internal abstract void OnStoreOverview(Player p);
        
        /// <summary> Called when the player does /store [item name] </summary>
        protected internal abstract void OnStoreCommand(Player p);
    }
    
    /// <summary> Simple item, in that it only has one cost value. </summary>
    public abstract class SimpleItem : Item {
        
        /// <summary> How much this item costs to purchase. </summary>
        public int Price = 100;
        
        /// <summary> Whether providing no arguments is allowed. </summary>
        protected bool AllowsNoArgs;
        
        public override void Parse(string line, string[] args) {
            if (args[1].CaselessEq("price"))
                Price = int.Parse(args[2]);
        }
        
        public override void Serialise(StreamWriter writer) {
            writer.WriteLine(Name + ":enabled:" + Enabled);
            writer.WriteLine(Name + ":price:" + Price);
            writer.WriteLine(Name + ":purchaserank:" + (int)PurchaseRank);
        }
        
        protected internal override void OnBuyCommand(Player p, string message, string[] args) {
            if (AllowsNoArgs && args.Length == 1) {
                DoPurchase(p, message, args); return;
            }
            // Must always provide an argument.
            if (args.Length < 2) { OnStoreCommand(p); return; }
            if (p.money < Price) {
                Player.Message(p, "%cYou don't have enough &3{1}&c to buy a {0}.", Name, Server.moneys); return;
            }
            DoPurchase(p, message, args);
        }
        
        protected abstract void DoPurchase(Player p, string message, string[] args);
        
        protected internal override void OnSetupCommand(Player p, string[] args) {
            if (args[1].CaselessEq("price")) {
                int cost;
                if (!int.TryParse(args[2], out cost)) {
                    Player.Message(p, "\"{0}\" is not a valid integer.", args[2]); return;
                }
                
                Player.Message(p, "Changed price of {0} item to &f{1} &3{2}", Name, cost, Server.moneys);
                Price = cost;
            } else {
                Player.Message(p, "Supported actions: enable, disable, price [cost]");
            }
        }
        
        protected internal override void OnSetupCommandHelp(Player p) {
            base.OnSetupCommandHelp(p);
            Player.Message(p, "%T/eco {0} price [amount]", Name.ToLower());
            Player.Message(p, "%HSets how many &3{0} %Hthis item costs.", Server.moneys);
        }
        
        protected internal override void OnStoreOverview(Player p) {
            if (p == null || p.Rank >= PurchaseRank) {
                Player.Message(p, "&6{0} %S- &a{1} %S{2}", Name, Price, Server.moneys);
            } else {
                Group grp = Group.findPerm(PurchaseRank);
                string grpName = grp == null ? ((int)PurchaseRank).ToString() : grp.ColoredName;
                Player.Message(p, "&6{0} %S({3}%S+) - &a{1} %S{2}", Name, Price, Server.moneys, grpName);
            }
        }
        
        protected internal override void OnStoreCommand(Player p) {
            Player.Message(p, "%T/buy {0} [value]", Name);
            OutputItemInfo(p);
        }
        
        protected void OutputItemInfo(Player p) {
            Player.Message(p, "%HCosts &a{0} {1} %Heach time the item is bought.", Price, Server.moneys);
            List<string> shortcuts = new List<string>();
            foreach (Alias a in Alias.aliases) {
                if (!a.Target.CaselessEq("buy") || a.Prefix == null) continue;
                
                // Find if there are any custom aliases for this item
                bool matchFound = false;
                foreach (string alias in Aliases) {
                    if (a.Prefix != alias) continue;
                    matchFound = true; break;
                }
                
                if (!matchFound) continue;
                shortcuts.Add("/" + a.Trigger);
            }
            
            if (shortcuts.Count == 0) return;
            Player.Message(p, "Shortcuts: {0}", shortcuts.Join());
        }
    }
}
