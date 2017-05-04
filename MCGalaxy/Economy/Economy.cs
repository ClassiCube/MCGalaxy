/*
    Copyright 2011 MCForge
        
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
using System.Globalization;
using System.IO;
using MCGalaxy.Eco;

namespace MCGalaxy {
    public static partial class Economy {

        public static bool Enabled;

        public static void Load() {
            if (!File.Exists(Paths.EconomyPropsFile)) {
                Server.s.Log("Economy properties don't exist, creating");
                Save();
            }
            
            using (StreamReader r = new StreamReader(Paths.EconomyPropsFile)) {
                string line;
                while ((line = r.ReadLine()) != null) {
                    line = line.ToLower().Trim();
                    try {
                        ParseLine(line);
                    } catch (Exception ex) {
                        Server.ErrorLog(ex);
                    }
                }
            }
        }
        
        static void ParseLine(string line) {
            string[] args = line.Split(':');
            if (args[0].CaselessEq("enabled")) {
                Enabled = args[1].CaselessEq("true");
            } else if (args.Length >= 3) {
                Item item = GetItem(args[0]);
                if (item == null) return;
                
                if (args[1].CaselessEq("enabled")) {
                    item.Enabled = args[2].CaselessEq("true");
                } else if (args[1].CaselessEq("purchaserank")) {
                    item.PurchaseRank = (LevelPermission)int.Parse(args[2]);
                } else {
                    item.Parse(line, args);
                }
            }
        }

        public static void Save() {
            using (StreamWriter w = new StreamWriter(Paths.EconomyPropsFile, false)) {
                w.WriteLine("enabled:" + Enabled);
                foreach (Item item in Items) {
                    w.WriteLine();
                    item.Serialise(w);
                }
            }
        }
 
        
        public static List<Item> Items = new List<Item>() { new ColorItem(), new TitleColorItem(),
            new TitleItem(), new RankItem(), new LevelItem(), new LoginMessageItem(),
            new LogoutMessageItem(), new BlocksItem(), new QueueLevelItem(),
            new InfectMessageItem(), new NickItem(), new ReviveItem(),
            new HumanInvisibilityItem(), new ZombieInvisibilityItem() };
        
        /// <summary> Finds the item whose name or one of its aliases caselessly matches the input. </summary>
        public static Item GetItem(string name) {
            foreach (Item item in Items) {
                if (name.CaselessEq(item.Name)) return item;
                
                foreach (string alias in item.Aliases) {
                    if (name.CaselessEq(alias)) return item;
                }
            }
            return null;
        }
        
        /// <summary> Gets comma separated list of enabled items. </summary>
        public static string EnabledItemNames() {
            string items = Items.Join(x => x.Enabled ? x.ShopName : null);
            return items.Length == 0 ? "(no enabled items)" : items;
        }
        
        public static SimpleItem Color { get { return (SimpleItem)Items[0]; } }
        public static SimpleItem TitleColor { get { return (SimpleItem)Items[1]; } }
        public static SimpleItem Title { get { return (SimpleItem)Items[2]; } }
        public static RankItem Ranks { get { return (RankItem)Items[3]; } }
        public static LevelItem Levels { get { return (LevelItem)Items[4]; } }
        
        public static void MakePurchase(Player p, int cost, string item) {
            p.SetMoney(p.money - cost);
            Player.Message(p, "Your balance is now &f{0} &3{1}", p.money, Server.moneys);

            Economy.EcoStats stats = RetrieveStats(p.name);
            stats.TotalSpent += cost;
            stats.Purchase = item + "%3 for %f" + cost + " %3" + Server.moneys +
                " on %f" + DateTime.Now.ToString(CultureInfo.InvariantCulture);
            Economy.UpdateStats(stats);
        }
    }
}