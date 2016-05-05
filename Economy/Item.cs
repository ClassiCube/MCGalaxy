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
using System.Globalization;
using System.IO;

namespace MCGalaxy.Eco {
    
    /// <summary> An abstract object that can be bought in the economy. (e.g. a rank, title, levels, etc) </summary>
    public abstract class Item {
        
        /// <summary> Simple name for this item. </summary>
        public abstract string Name { get; }
        
        /// <summary> Other common names for this item. </summary>
        public string[] Aliases;
        
        /// <summary> Whether this item can currently be bought in the economy. </summary>
        public bool Enabled;
        
        /// <summary> Reads the given property of this item from the economy.properties file. </summary>
        /// <remarks> split is line split by the : character. </remarks>
        public abstract void Parse(string line, string[] split);
        
        /// <summary> Writes the properties of this item to the economy.properties file. </summary>
        public abstract void Serialise(StreamWriter writer);

        protected internal abstract void OnBuyCommand(Command cmd, Player p, 
                                                      string message, string[] args);

        protected internal abstract void OnSetupCommand(Player p, string[] args);

        protected internal abstract void OnStoreOverview(Player p);
        
        protected internal abstract void OnStoreCommand(Player p);
    }
    
    /// <summary> Simple item, in that it only has one cost value. </summary>
    public abstract class SimpleItem : Item {
        
        /// <summary> How much this item costs to purchase. </summary>
        public int Price = 100;
        
        protected bool NoArgsResetsItem;
        
        public override void Parse(string line, string[] split) {
            if (split.Length < 3) return;
            
            if (split[1].CaselessEq("enabled"))
                Enabled = split[2].CaselessEq("true");
            if (split[1].CaselessEq("price"))
                Price = int.Parse(split[2]);
        }
        
        public override void Serialise(StreamWriter writer) {
            writer.WriteLine(Name + ":enabled:" + Enabled);
            writer.WriteLine(Name + ":price:" + Price);
        }
        
        protected internal override void OnBuyCommand(Command cmd, Player p, 
                                                      string message, string[] args) {
            if (NoArgsResetsItem && args.Length == 1) {
                OnBuyCommand(p, message, args); return;
            }
            // Must always provide an argument.
            if (args.Length < 2) { cmd.Help(p); return; }
            if (p.money < Price) {
                Player.Message(p, "%cYou don't have enough %3" + Server.moneys + "%c to buy a " + Name + "."); return;
            }
            OnBuyCommand(p, message, args);
        }
        
        protected abstract void OnBuyCommand(Player p, string message, string[] args);
        
        protected internal override void OnSetupCommand(Player p, string[] args) {
            switch (args[1].ToLower()) {
                case "enable":
                    Player.Message(p, "%aThe " + Name + " item is now enabled.");
                    Enabled = true; break;
                case "disable":
                    Player.Message(p, "%aThe " + Name + " item it now enabled.");
                    Enabled = false; break;
                case "price":
                    int cost;
                    if (!int.TryParse(args[2], out cost)) {
                        Player.Message(p, "\"" + args[2] + "\" is not a valid integer."); return;
                    }
                    Player.Message(p, "%aSuccessfully changed the " + Name + " price to %f" + cost + " %3" + Server.moneys); 
                    Price = cost; break;
                default:
                    Player.Message(p, "Supported actions: enable, disable, price [cost]"); break;
            }
        }
        
        protected internal override void OnStoreOverview(Player p) {
            Player.Message(p, Name + " - costs %f" + Price + " %3" + Server.moneys);
        }
        
        protected internal override void OnStoreCommand(Player p) {
        	Player.Message(p, "Syntax: %T/buy " + Name + " [value]");
            Player.Message(p, "Costs %f" + Price + " %3" + Server.moneys + " %Seach time the item is bought.");
        }
    }
}
