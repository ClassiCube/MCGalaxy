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
        
        protected internal abstract void OnStoreCommand(Player p);
        
        protected static void MakePurchase(Player p, int cost, string item) {
            Economy.EcoStats ecos = Economy.RetrieveEcoStats(p.name);
            p.money -= cost;
            p.OnMoneyChanged();
            ecos.money = p.money;
            ecos.totalSpent += cost;
            ecos.purchase = item + "%3 - Price: %f" + cost + " %3" + Server.moneys +
                " - Date: %f" + DateTime.Now.ToString(CultureInfo.InvariantCulture);
            Economy.UpdateEcoStats(ecos);
            Player.SendMessage(p, "%aYour balance is now %f" + p.money + " %3" + Server.moneys);
        }
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
                Player.SendMessage(p, "%cYou don't have enough %3" + Server.moneys + "%c to buy a " + Name + "."); return;
            }
			OnBuyCommand(p, message, args);
		}
		
		protected abstract void OnBuyCommand(Player p, string message, string[] args);
        
        protected internal override void OnSetupCommand(Player p, string[] args) {
			switch (args[1].ToLower()) {
                case "enable":
                    Player.SendMessage(p, "%a" + Name + "s are now enabled for the economy system.");
                    Enabled = true; break;
                case "disable":
                    Player.SendMessage(p, "%a" + Name + "s are now disabled for the economy system.");
                    Enabled = false; break;
                case "price":
                    int cost;
                    if (!int.TryParse(args[2], out cost)) {
                        Player.SendMessage(p, "\"" + args[2] + "\" is not a valid integer."); return;
                    }
                    Player.SendMessage(p, "%aSuccessfully changed the " + Name + " price to %f" + cost + " %3" + Server.moneys); 
                    Price = cost; break;
                default:
                    Player.SendMessage(p, "Supported actions: enable, disable, price [cost]"); break;
            }
        }
        
        protected internal override void OnStoreCommand(Player p) {
            Player.SendMessage(p, Name + "s cost %f" + Price + " %3" + Server.moneys + " %Seach");
        }
    }
}
