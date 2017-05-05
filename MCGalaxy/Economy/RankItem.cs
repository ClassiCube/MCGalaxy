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
using MCGalaxy.Commands;

namespace MCGalaxy.Eco {
    
    public sealed class RankItem : Item {
        
        public RankItem() {
            Aliases = new string[] { "rank", "ranks", "rankup" };
        }
        
        public override string Name { get { return "Rank"; } }
        
        public override string ShopName { get { return "Rankup"; } }
        
        public LevelPermission MaxRank = LevelPermission.AdvBuilder;
        
        public List<Rank> RanksList = new List<Rank>();
        public class Rank {
            public Group group;
            public int price = 1000;
        }
        
        public override void Parse(string line, string[] args) {
            if (args[1].CaselessEq("price")) {
                Rank rnk = FindRank(args[2]);
                if (rnk == null) {
                    rnk = new Rank();
                    rnk.group = Group.Find(args[2]);
                    if (rnk.group == null) return;
                    
                    RanksList.Add(rnk);
                }
                rnk.price = int.Parse(args[3]);
            } else if (args[1] == "maxrank") {
                MaxRank = LevelPermission.AdvBuilder;
                LevelPermission perm = Group.ParsePermOrName(args[2]);
                if (perm != LevelPermission.Null) MaxRank = perm;
            }
        }
        
        public override void Serialise(StreamWriter writer) {
            writer.WriteLine("rank:enabled:" + Enabled);
            writer.WriteLine("rank:purchaserank:" + (int)PurchaseRank);
            
            writer.WriteLine("rank:maxrank:" + (int)MaxRank);
            foreach (Rank rnk in RanksList) {
                writer.WriteLine("rank:price:" + rnk.group.name + ":" + rnk.price);
                if (rnk.group.Permission >= MaxRank) break;
            }
        }
        
        protected internal override void OnBuyCommand(Player p, string message, string[] args) {
            if (args.Length >= 2) {
                Player.Message(p, "%cYou cannot provide a rank name, use %a/buy rank %cto buy the NEXT rank."); return;
            }
            if (p.Rank >= MaxRank) {
                Player.Message(p, "%cYou cannot buy anymore ranks, as you are at or past the max buyable rank of {0}",
                               Group.GetColoredName(MaxRank));
                return;
            }
            if (p.money < NextRank(p).price) {
                Player.Message(p, "%cYou don't have enough %3" + Server.moneys + "%c to buy the next rank"); return;
            }
            
            Command.all.Find("setrank").Use(null, "+up " + p.name);
            Player.Message(p, "You bought the rank " + p.group.ColoredName);
            Economy.MakePurchase(p, FindRank(p.group.name).price, "%3Rank: " + p.group.ColoredName);
        }
        
        protected internal override void OnSetupCommand(Player p, string[] args) {
            switch (args[1].ToLower()) {
                case "price":
                    Rank rnk = FindRank(args[2]);
                    if (rnk == null) {
                        Player.Message(p, "%cThat wasn't a rank or it's past the max rank (max rank is: {0}%c)",
                                       Group.GetColoredName(MaxRank));
                        return;
                    }
                    
                    int cost = 0;
                    if (!CommandParser.GetInt(p, args[3], "Price", ref cost, 0)) return;
                    
                    Player.Message(p, "%aSuccesfully changed the rank price for {0} to: &f{1} &3{2}", rnk.group.ColoredName, cost, Server.moneys);
                    rnk.price = cost; break;

                case "maxrank":
                case "max":
                case "maximum":
                case "maximumrank":
                    Group grp = Matcher.FindRanks(p, args[2]);
                    if (grp == null) return;
                    if (p != null && p.Rank < grp.Permission) { Player.Message(p, "%cCannot set maxrank to a rank higher than yours."); return; }
                    
                    MaxRank = grp.Permission;
                    Player.Message(p, "%aSuccessfully set max rank to: " + grp.ColoredName);
                    UpdatePrices();
                    break;
                default:
                    OnSetupCommandHelp(p); break;
            }
        }
        
        protected internal override void OnSetupCommandHelp(Player p) {
            base.OnSetupCommandHelp(p);
            Player.Message(p, "%T/eco rank price [rank] [amount]");
            Player.Message(p, "%HSets how many &3{0} %Hthat rank costs.", Server.moneys);
            Player.Message(p, "%T/eco rank maxrank [rank]");
            Player.Message(p, "%HSets the maximum rank that can be bought.", Server.moneys);
        }

        protected internal override void OnStoreOverview(Player p) {
            if (p == null || p.Rank >= MaxRank) {
                Player.Message(p, "&6Rankup %S- &calready at max rank."); return;
            }
            
            Rank rnk = NextRank(p);
            if (rnk == null) {
                Player.Message(p, "&6Rankup %S- &cno further ranks to buy.");
            } else {
                Player.Message(p, "&6Rankup to {0} %S- &a{1} %S{2}", rnk.group.ColoredName, rnk.price, Server.moneys);
            }
        }
        
        protected internal override void OnStoreCommand(Player p) {
            Player.Message(p, "%T/buy rankup");
            Player.Message(p, "%fThe highest buyable rank is: {0}", Group.GetColoredName(MaxRank));
            Player.Message(p, "%cYou can only buy ranks one at a time, in sequential order.");
            
            foreach (Rank rnk in RanksList) {
                Player.Message(p, "&6{0} %S- &a{1} %S{2}", rnk.group.ColoredName, rnk.price, Server.moneys);
                if (rnk.group.Permission >= MaxRank) break;
            }
        }
        
        public Rank FindRank(string name) {
            foreach (Rank rank in RanksList) {
                if (rank.group.name != null && rank.group.name.CaselessEq(name))
                    return rank;
            }
            return null;
        }

        public Rank NextRank(Player p) {
            int grpIndex = Group.GroupList.IndexOf(p.group);
            for (int i = grpIndex + 1; i < Group.GroupList.Count; i++) {
                Rank rank = FindRank(Group.GroupList[i].name);
                if (rank != null) return rank;
            }
            return null;
        }
        
        public void UpdatePrices() {
            int lastPrice = 0;
            foreach (Group group in Group.GroupList) {
                if (group.Permission > MaxRank) break;
                if (group.Permission <= Group.standard.Permission) continue;
                
                Rank rank = FindRank(group.name);
                if (rank == null) {
                    rank = new Rank();
                    rank.group = group;
                    
                    if (lastPrice == 0) { rank.price = 1000; }
                    else { rank.price = lastPrice + 250; }
                    RanksList.Add(rank);
                } else {
                    lastPrice = rank.price;
                }
            }
        }
    }
}
