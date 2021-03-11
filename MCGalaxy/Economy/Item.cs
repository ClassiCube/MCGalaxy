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
using MCGalaxy.Commands;

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
        
        
        /// <summary> Called when the player does /buy [item name] &lt;value&gt; </summary>
        protected internal abstract void OnPurchase(Player p, string args);
        
        /// <summary> Called when the player does /eco [item name] [option] &lt;value&gt; </summary>
        protected internal abstract void OnSetup(Player p, string[] args);
        
        /// <summary> Called when the player does /eco help [item name] </summary>
        protected internal virtual void OnSetupHelp(Player p) {
            p.Message("&T/Eco {0} enable/disable", Name.ToLower());
            p.Message("&HEnables/disables purchasing this item.");
            p.Message("&T/Eco {0} purchaserank [rank]", Name.ToLower());
            p.Message("&HSets the lowest rank which can purchase this item.");
        }

        /// <summary> Called when the player does /store </summary>
        protected internal abstract void OnStoreOverview(Player p);
        
        /// <summary> Called when the player does /store [item name] </summary>
        protected internal abstract void OnStoreCommand(Player p);
        

        internal void Setup(Player p, string[] args) {
            string cmd = args[1];
            if (cmd.CaselessEq("enable")) {
                p.Message("&aThe {0} item is now enabled.", Name);
                Enabled = true;
            } else if (cmd.CaselessEq("disable")) {
                p.Message("&aThe {0} item is now disabled.", Name);
                Enabled = false;
            } else if (cmd.CaselessStarts("purchaserank")) {
                if (args.Length == 2) { p.Message("You need to provide a rank name."); return; }
                Group grp = Matcher.FindRanks(p, args[2]);
                if (grp == null) return;
                
                PurchaseRank = grp.Permission;
                p.Message("Min purchase rank for {0} item set to {1}&S.", Name, grp.ColoredName);
            } else {
                OnSetup(p, args);
            }
        }
        
        protected static void UseCommand(Player p, string cmd, string args) {
            CommandData data = default(CommandData);
            data.Rank    = LevelPermission.Nobody;
            data.Context = CommandContext.Purchase;
            Command.Find(cmd).Use(p, args, data);
        }
        
        protected static bool CheckPrice(Player p, int price, string item) {
            if (p.money < price) {
                p.Message("&WYou don't have enough &3{1} &Wto buy {0}.", item, Server.Config.Currency); 
                return false;
            }
            return true;
        }
    }
    
    /// <summary> Simple item, in that it only has one cost value. </summary>
    public abstract class SimpleItem : Item {
        
        /// <summary> How much this item costs to purchase. </summary>
        public int Price = 100;
        
        public override void Parse(string line, string[] args) {
            if (args[1].CaselessEq("price"))
                Price = int.Parse(args[2]);
        }
        
        public override void Serialise(StreamWriter writer) {
            writer.WriteLine(Name + ":price:" + Price);
        }
        
        protected bool CheckPrice(Player p) { return CheckPrice(p, Price, "a " + Name); }
        
        protected internal override void OnSetup(Player p, string[] args) {
            if (args[1].CaselessEq("price")) {
                int cost = 0;
                if (!CommandParser.GetInt(p, args[2], "Price", ref cost)) return;
                
                p.Message("Changed price of {0} item to &f{1} &3{2}", Name, cost, Server.Config.Currency);
                Price = cost;
            } else {
                p.Message("Supported actions: enable, disable, price [cost]");
            }
        }
        
        protected internal override void OnSetupHelp(Player p) {
            base.OnSetupHelp(p);
            p.Message("&T/Eco {0} price [amount]", Name.ToLower());
            p.Message("&HSets how many &3{0} &Hthis item costs.", Server.Config.Currency);
        }
        
        protected internal override void OnStoreOverview(Player p) {
            if (p.Rank >= PurchaseRank) {
                p.Message("&6{0} &S- &a{1} &S{2}", Name, Price, Server.Config.Currency);
            } else {
                string grpName = Group.GetColoredName(PurchaseRank);
                p.Message("&6{0} &S({3}&S+) - &a{1} &S{2}", Name, Price, Server.Config.Currency, grpName);
            }
        }
        
        protected internal override void OnStoreCommand(Player p) {
            p.Message("&T/Buy {0} [value]", Name);
            OutputItemInfo(p);
        }
        
        protected void OutputItemInfo(Player p) {
            p.Message("&HCosts &a{0} {1} &Heach time the item is bought.", Price, Server.Config.Currency);
            List<string> shortcuts = new List<string>();
            foreach (Alias a in Alias.aliases) {
                if (!a.Target.CaselessEq("buy") || a.Format == null) continue;
                
                // Find if there are any custom aliases for this item
                bool matchFound = false;
                foreach (string alias in Aliases) {
                    if (!a.Format.CaselessEq(alias)) continue;
                    matchFound = true; break;
                }
                
                if (!matchFound) continue;
                shortcuts.Add("/" + a.Trigger);
            }
            
            if (shortcuts.Count == 0) return;
            p.Message("Shortcuts: &T{0}", shortcuts.Join());
        }
    }
}
