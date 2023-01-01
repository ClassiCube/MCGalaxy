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

namespace MCGalaxy.Eco 
{    
    public static partial class Economy 
    {
        public static bool Enabled;
        static Dictionary<string, List<string>> itemCfg = new Dictionary<string, List<string>>();
        
        static List<string> GetConfig(string item) {
            List<string> cfg;
            if (itemCfg.TryGetValue(item, out cfg)) return cfg;
            
            cfg = new List<string>();
            itemCfg[item] = cfg;
            return cfg;
        }
        

        public static void Load() {
            if (!File.Exists(Paths.EconomyPropsFile)) {
                Logger.Log(LogType.SystemActivity, "Economy properties don't exist, creating");
                Save();
            }
            
            using (StreamReader r = new StreamReader(Paths.EconomyPropsFile)) {
                string line;
                while ((line = r.ReadLine()) != null) 
                {
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
            string name, value;
            line.Separate(':', out name, out value);
            if (value.Length == 0) return;
            
            if (name.CaselessEq("enabled")) {
                Enabled = value.CaselessEq("true"); return;
            } 
            
            Item item = GetItem(name);
            name = item != null ? item.Name : name;
            
            GetConfig(name).Add(value);            
            if (item != null) item.LoadConfig(value);
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
                
                foreach (Item item in Items) 
                {
                    List<string> cfg = GetConfig(item.Name);
                    cfg.Clear();
                    item.SaveConfig(cfg);
                }
                
                foreach (var kvp in itemCfg)
                {
                    w.WriteLine();
                    foreach (string prop in kvp.Value)
                    {
                        w.WriteLine(kvp.Key + ":" + prop);
                    }
                }
            }
        }
 
        
        public static List<Item> Items = new List<Item>() { 
            new ColorItem(), new TitleColorItem(), new TitleItem(), 
            new RankItem(), new LevelItem(), new LoginMessageItem(),
            new LogoutMessageItem(), new NickItem(), new SnackItem() 
        };
        
        public static void RegisterItem(Item item) {
            List<string> cfg = GetConfig(item.Name);
            
            foreach (string line in cfg)
            {
                item.LoadConfig(line);
            }
            Items.Add(item);
        }
        
        /// <summary> Finds the item whose name or one of its aliases caselessly matches the input. </summary>
        public static Item GetItem(string name) {
            foreach (Item item in Items) 
            {
                if (name.CaselessEq(item.Name)) return item;
                
                foreach (string alias in item.Aliases) 
                {
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
        
        public static RankItem Ranks   { get { return (RankItem)Items[3]; } }
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