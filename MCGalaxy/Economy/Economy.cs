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
using System.Data;
using System.Globalization;
using System.IO;
using System.Text;
using MCGalaxy.Eco;
using MCGalaxy.SQL;

namespace MCGalaxy {
    public static class Economy {

        public static bool Enabled;
        const string propertiesFile = "properties/economy.properties";
        
        static ColumnDesc[] createEconomy = {
            new ColumnDesc("player", ColumnType.VarChar, 20, priKey: true),
            new ColumnDesc("money", ColumnType.Int32),
            new ColumnDesc("total", ColumnType.Integer, notNull: true, def: "0"),
            new ColumnDesc("purchase", ColumnType.VarChar, 255, notNull: true, def: "'%cNone'"),
            new ColumnDesc("payment", ColumnType.VarChar, 255, notNull: true, def: "'%cNone'"),
            new ColumnDesc("salary", ColumnType.VarChar, 255, notNull: true, def: "'%cNone'"),
            new ColumnDesc("fine", ColumnType.VarChar, 255, notNull: true, def: "'%cNone'"),
        };

        public struct EcoStats {
            public string Player, Purchase, Payment, Salary, Fine;
            public int TotalSpent;
            
            public EcoStats(int tot, string player, string pur,
                            string pay, string sal, string fin) {
                TotalSpent = tot;
                Player = player;
                Purchase = pur;
                Payment = pay;
                Salary = sal;
                Fine = fin;
            }
        }

        public static void LoadDatabase() {
            Database.Backend.CreateTable("Economy", createEconomy);
            using (DataTable eco = Database.Backend.GetRows("Economy", "*"))
                foreach (DataRow row in eco.Rows)
            {
                int money = PlayerData.ParseInt(row["money"].ToString());
                if (money == 0) continue;
                
                EcoStats stats;
                stats.Player = row["player"].ToString();
                stats.Payment = row["payment"].ToString();
                stats.Purchase = row["purchase"].ToString();
                stats.Salary = row["salary"].ToString();
                stats.Fine = row["fine"].ToString();
                stats.TotalSpent = PlayerData.ParseInt(row["total"].ToString());
                
                UpdateMoney(stats.Player, money);
                UpdateStats(stats);
            }
        }

        public static void Load() {
            if (!File.Exists(propertiesFile)) {
                Server.s.Log("Economy properties don't exist, creating");
                Save();
            }
            
            using (StreamReader r = new StreamReader(propertiesFile)) {
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
                
                if (args[1].CaselessEq("enabled"))
                    item.Enabled = args[2].CaselessEq("true");
                else if (args[1].CaselessEq("purchaserank"))
                    item.PurchaseRank = (LevelPermission)int.Parse(args[2]);
                else
                    item.Parse(line, args);
            }
        }

        public static void Save() {
            using (StreamWriter w = new StreamWriter(propertiesFile, false)) {
                w.WriteLine("enabled:" + Enabled);
                foreach (Item item in Items) {
                    w.WriteLine();
                    item.Serialise(w);
                }
            }
        }
        
        public static void UpdateStats(EcoStats stats) {
            Database.Backend.AddOrReplaceRow("Economy", "player, money, total, purchase, payment, salary, fine",
                                             stats.Player, 0, stats.TotalSpent, stats.Purchase,
                                             stats.Payment, stats.Salary, stats.Fine);
        }

        public static EcoStats RetrieveStats(string name) {
            EcoStats stats = default(EcoStats);
            stats.Player = name;
            
            using (DataTable eco = Database.Backend.GetRows("Economy", "*", "WHERE player=@0", name)) {
                if (eco.Rows.Count > 0) {
                    stats.TotalSpent = int.Parse(eco.Rows[0]["total"].ToString());
                    stats.Purchase = eco.Rows[0]["purchase"].ToString();
                    stats.Payment = eco.Rows[0]["payment"].ToString();
                    stats.Salary = eco.Rows[0]["salary"].ToString();
                    stats.Fine = eco.Rows[0]["fine"].ToString();
                } else {
                    stats.Purchase = "%cNone";
                    stats.Payment = "%cNone";
                    stats.Salary = "%cNone";
                    stats.Fine = "%cNone";
                }
            }
            return stats;
        }
        
        public static string FindMatches(Player p, string name, out int money) {
            DataRow row = PlayerInfo.QueryMulti(p, name, "Name, Money");
            money = row == null ? 0 : PlayerData.ParseInt(row["Money"].ToString());
            return row == null ? null : row["Name"].ToString();
        }
        
        public static void UpdateMoney(string name, int money) {
            Database.Backend.UpdateRows("Players", "Money = @1",
                                        "WHERE Name = @0", name, money);
        }
        
        public static List<Item> Items = new List<Item>() { new ColorItem(), new TitleColorItem(),
            new TitleItem(), new RankItem(), new LevelItem(), new LoginMessageItem(),
            new LogoutMessageItem(), new BlocksItem(), new QueueLevelItem(),
            new InfectMessageItem(), new NickItem(), new ReviveItem(),
            new HumanInvisibilityItem(), new ZombieInvisibilityItem() };
        
        public static Item GetItem(string name) {
            foreach (Item item in Items) {
                if (name.CaselessEq(item.Name)) return item;
                
                foreach (string alias in item.Aliases) {
                    if (name.CaselessEq(alias)) return item;
                }
            }
            return null;
        }
        
        public static string GetItemNames() {
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