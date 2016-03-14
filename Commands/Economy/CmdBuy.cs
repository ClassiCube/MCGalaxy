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
using System.Globalization;
using System.Threading;
using MCGalaxy.SQL;

namespace MCGalaxy.Commands {
    
    /// <summary> Economy Beta v1.0 QuantumHive </summary>
    public sealed class CmdBuy : Command {
        public override string name { get { return "buy"; } }
        public override string shortcut { get { return "purchase"; } }
        public override string type { get { return CommandTypes.Economy; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Guest; } }
        public override bool Enabled { get { return Economy.Settings.Enabled; } }
        
        public override void Use(Player p, string message) {
            if (p == null) { MessageInGameOnly(p); return; }
            string[] parts = message.Split(' ');

            Economy.EcoStats ecos = Economy.RetrieveEcoStats(p.name);
            switch (parts[0].ToLower()) {
                case "map":
                case "level":
                case "maps":
                case "levels":
                    if (parts.Length < 2) { Help(p); return; }
                    Economy.Settings.Level lvl = Economy.FindLevel(parts[1]);
                    if (lvl == null) { Player.SendMessage(p, "%cThat isn't a level preset"); return; }
                    
                    if (!p.EnoughMoney(lvl.price)) {
                        Player.SendMessage(p, "%cYou don't have enough %3" + Server.moneys + "%c to buy that map"); return;
                    }
                    int old = p.money;
                    int oldTS = ecos.totalSpent;
                    string oldP = ecos.purchase;
                    if (parts.Length < 3) { Help(p); return; }
                    string name = parts[2];
                    
                    try {
                        Command.all.Find("newlvl").Use(null, p.name + "_" + name + " " + lvl.x + " " + lvl.y + " " + lvl.z + " " + lvl.type);
                        Player.SendMessage(p, "%aCreating level: '%f" + p.name + "_" + name + "%a' . . .");
                        p.money = p.money - lvl.price;
                        ecos.money = p.money;
                        ecos.totalSpent += lvl.price;
                        ecos.purchase = "%3Map: %f" + lvl.name + "%3 - Price: %f"  + lvl.price + " %3" + Server.moneys +
                            " - Date: %f" + DateTime.Now.ToString(CultureInfo.InvariantCulture);
                        Economy.UpdateEcoStats(ecos);
                        
                        Command.all.Find("load").Use(null, p.name + "_" + name);
                        Thread.Sleep(250);
                        Level level = LevelInfo.Find(p.name + "_" + name);
                        if (level.permissionbuild > p.group.Permission) { level.permissionbuild = p.group.Permission; }
                        if (level.permissionvisit > p.group.Permission) { level.permissionvisit = p.group.Permission; }
                        Command.all.Find("goto").Use(p, p.name + "_" + name);

                        Player.SendMessage(p, "%aSuccessfully created your map: '%f" + p.name + "_" + name + "%a'");
                        Player.SendMessage(p, "%aYour balance is now %f" + p.money + " %3" + Server.moneys);
                        try {
                            //safe against SQL injections, but will be replaced soon by a new feature
                            //DB
                            Database.executeQuery("INSERT INTO `Zone" + level.name + "` (SmallX, SmallY, SmallZ, BigX, BigY, BigZ, Owner) parts[1]S " +
                                                  "(0,0,0," + (level.Width - 1) + "," + (level.Height - 1) + "," + (level.Length - 1) + ",'" + p.name + "')");
                            //DB
                            Player.SendMessage(p, "%aZoning Succesful");
                        } catch { Player.SendMessage(p, "%cZoning Failed"); }
                    } catch {
                        Player.SendMessage(p, "%cSomething went wrong, Money restored");
                        if (old != p.money) {
                            p.money = old; ecos.money = old; ecos.totalSpent = oldTS; ecos.purchase = oldP;
                            Economy.UpdateEcoStats(ecos);
                        }
                    } break;

                case "colors":
                case "color":
                case "colours":
                case "colour":
                    if (parts.Length < 2) { Help(p); return; }
                    if (!p.EnoughMoney(Economy.Settings.ColorPrice)) {
                        Player.SendMessage(p, "%cYou don't have enough %3" + Server.moneys + "%c to buy a color");
                        return;
                    }
                    if (!parts[1].StartsWith("&") || !parts[1].StartsWith("%")) {
                        parts[1] = Colors.Parse(parts[1]);
                        if (parts[1] == "") {
                            Player.SendMessage(p, "%cThat wasn't a color");
                            return;
                        }
                    }
                    if (parts[1] == p.color) {
                        Player.SendMessage(p, "%cYou already have a " + parts[1] + Colors.Name(parts[1]) + "%c color"); return;
                    }
                    
                    Command.all.Find("color").Use(null, p.name + " " + Colors.Name(parts[1]));
                    p.money = p.money - Economy.Settings.ColorPrice;
                    ecos.money = p.money;
                    ecos.totalSpent += Economy.Settings.ColorPrice;
                    ecos.purchase = "%3Color: " + parts[1] + Colors.Name(parts[1]) + "%3 - Price: %f" + Economy.Settings.ColorPrice + " %3" + Server.moneys + " - Date: %f" + DateTime.Now.ToString(CultureInfo.InvariantCulture);
                    Economy.UpdateEcoStats(ecos);
                    Player.SendMessage(p, "%aYour color has been successfully changed to " + parts[1] + Colors.Name(parts[1]));
                    Player.SendMessage(p, "%aYour balance is now %f" + p.money.ToString() + " %3" + Server.moneys);
                    break;

                case "tcolor":
                case "tcolors":
                case "titlecolor":
                case "titlecolors":
                case "tc":
                    if (parts.Length < 2) { Help(p); return; }
                    if (!p.EnoughMoney(Economy.Settings.TColorPrice)) {
                        Player.SendMessage(p, "%cYou don't have enough %3" + Server.moneys + "%c to buy a titlecolor");
                        return;
                    }
                    if (!parts[1].StartsWith("&") || !parts[1].StartsWith("%")) {
                        parts[1] = Colors.Parse(parts[1]);
                        if (parts[1] == "") {
                            Player.SendMessage(p, "%cThat wasn't a color");
                            return;
                        }
                    }
                    if (parts[1] == p.titlecolor) {
                        Player.SendMessage(p, "%cYou already have a " + parts[1] + Colors.Name(parts[1]) + "%c titlecolor"); return;
                    }
                    
                    Command.all.Find("tcolor").Use(null, p.name + " " + Colors.Name(parts[1]));
                    p.money = p.money - Economy.Settings.TColorPrice;
                    ecos.money = p.money;
                    ecos.totalSpent += Economy.Settings.TColorPrice;
                    ecos.purchase = "%3Titlecolor: " + parts[1] + Colors.Name(parts[1]) + "%3 - Price: %f" + Economy.Settings.TColorPrice + " %3" + Server.moneys + " - Date: %f" + DateTime.Now.ToString(CultureInfo.InvariantCulture);
                    Economy.UpdateEcoStats(ecos);
                    Player.SendMessage(p, "%aYour titlecolor has been successfully changed to " + parts[1] + Colors.Name(parts[1]));
                    Player.SendMessage(p, "%aYour balance is now %f" + p.money + " %3" + Server.moneys);
                    break;

                case "titles":
                case "title":
                    if (parts.Length < 2) { Help(p); return; }
                    if (!p.EnoughMoney(Economy.Settings.TitlePrice)) {
                        Player.SendMessage(p, "%cYou don't have enough %3" + Server.moneys + "%c to buy a title"); return;
                    }
                    if (parts[1] == p.title) {
                        Player.SendMessage(p, "%cYou already have that title"); return;
                    }
                    if (parts[1].Length > 17) {
                        Player.SendMessage(p, "%cTitles cannot be longer than 17 characters"); return;
                    }
                    var regex = new System.Text.RegularExpressions.Regex(@"^[a-zA-Z0-9-_\\.]*$");
                    if (!regex.IsMatch(parts[1])) {
                        Player.SendMessage(p, "%cInvalid title! Titles may only contain alphanumeric characters and .-_");
                        return;
                    }
                    bool free = parts[1] == "";
                    Command.all.Find("title").Use(null, p.name + " " + parts[1]);
                    if (!free) {
                        p.money = p.money - Economy.Settings.TitlePrice;
                        ecos.money = p.money;
                        ecos.totalSpent += Economy.Settings.TitlePrice;
                        ecos.purchase = "%3Title: %f" + parts[1] + "%3 - Price: %f" + Economy.Settings.TitlePrice + " %3" + Server.moneys + " - Date: %f" + DateTime.Now.ToString(CultureInfo.InvariantCulture);
                        Economy.UpdateEcoStats(ecos);
                        Player.SendMessage(p, "%aYour title has been successfully changed to [" + p.titlecolor + parts[1] + "%a]");
                    } else {
                        Player.SendMessage(p, "%aYour title has been successfully removed for free");
                    }
                    Player.SendMessage(p, "%aYour balance is now %f" + p.money + " %3" + Server.moneys);
                    break;

                case "ranks":
                case "rank":
                    if (parts.Length >= 2) {
                        Player.SendMessage(p, "%cYou cannot provide a rank name, use %a/eco buy rank %cto buy the NEXT rank."); return;
                    }

                    LevelPermission maxrank = Group.Find(Economy.Settings.MaxRank).Permission;
                    if (p.group.Permission >= maxrank) {
                        Player.SendMessage(p, "%cYou cannot buy anymore ranks, because you passed the max buyable rank: " + Group.Find(Economy.Settings.MaxRank).color + Economy.Settings.MaxRank);
                        return;
                    }
                    
                    if (!p.EnoughMoney(Economy.NextRank(p).price)) {
                        Player.SendMessage(p, "%cYou don't have enough %3" + Server.moneys + "%c to buy the next rank"); return;
                    }
                    Command.all.Find("promote").Use(null, p.name);
                    p.money = p.money - Economy.FindRank(p.group.name).price;
                    ecos.money = p.money;
                    ecos.totalSpent += Economy.FindRank(p.group.name).price;
                    ecos.purchase = "%3Rank: " + p.group.color + p.group.name + "%3 - Price: %f" + Economy.FindRank(p.group.name).price + " %3" + Server.moneys + " - Date: %f" + DateTime.Now.ToString(CultureInfo.InvariantCulture);
                    Economy.UpdateEcoStats(ecos);
                    Player.SendMessage(p, "%aYou've successfully bought the rank " + p.group.color + p.group.name);
                    Player.SendMessage(p, "%aYour balance is now %f" + p.money + " %3" + Server.moneys);
                    break;

                default:
                    Player.SendMessage(p, "%cThat wasn't a valid command addition!");
                    break;
            }
        }
        
        public override void Help(Player p) {
            Player.SendMessage(p, "%T/buy <title/color/tcolor/rank/map> [parts[1]] <map name>");
            Player.SendMessage(p, "%Hparts[1] is either [title/color/tcolor/map_preset]");
            Player.SendMessage(p, "%Hmap name is only used for %T/buy map%H.");
        }
    }
}
