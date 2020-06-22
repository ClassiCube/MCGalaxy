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
using System.IO;
using MCGalaxy.Events.EconomyEvents;

namespace MCGalaxy.Eco {
    
    public static partial class Economy {

        public static bool Enabled;

        public static void Load() {
            if (!File.Exists(Paths.EconomyPropsFile)) {
                Logger.Log(LogType.SystemActivity, "Economy properties don't exist, creating");
                Save();
            }
            
            using (StreamReader r = new StreamReader(Paths.EconomyPropsFile)) {
                string line;
                while ((line = r.ReadLine()) != null) {
                    line = line.Trim();
                    try {
                        ParseLine(line);
                    } catch (Exception ex) {
                        Logger.LogError(ex);
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

        static readonly object saveLock = new object();
        public static void Save() {
            try {
                lock (saveLock) SaveCore();
            } catch (Exception e) {
                Logger.LogError("Error saving " + Paths.EconomyPropsFile, e);
            }
        }
        
        static void SaveCore() {
            using (StreamWriter w = new StreamWriter(Paths.EconomyPropsFile, false)) {
                w.WriteLine("enabled:" + Enabled);
                foreach (Item item in Items) {
                    w.WriteLine();
                    w.WriteLine(item.Name + ":enabled:" + item.Enabled);
                    w.WriteLine(item.Name + ":purchaserank:" + (int)item.PurchaseRank);
                    item.Serialise(w);
                }
            }
        }
 
        
        public static List<Item> Items = new List<Item>() { new ColorItem(), new TitleColorItem(),
            new TitleItem(), new RankItem(), new LevelItem(), new LoginMessageItem(),
            new LogoutMessageItem(), new BlocksItem(), new QueueLevelItem(),
            new InfectMessageItem(), new NickItem(), new ReviveItem(),
            new InvisibilityItem(), new SnackItem() };
        
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
        
        public static RankItem Ranks { get { return (RankItem)Items[3]; } }
        public static LevelItem Levels { get { return (LevelItem)Items[4]; } }
        
        public static void MakePurchase(Player p, int cost, string item) {
            p.SetMoney(p.money - cost);
            EcoTransaction transaction = new EcoTransaction();
            transaction.TargetName = p.name;
            transaction.TargetFormatted = p.ColoredName;
            transaction.Amount = cost;
            transaction.Type = EcoTransactionType.Purchase;
            transaction.ItemDescription = item;
            OnEcoTransactionEvent.Call(transaction);
        }
    }
}