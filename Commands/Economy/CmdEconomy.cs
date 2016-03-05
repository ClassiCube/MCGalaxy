/*
	Copyright 2011 MCGalaxy
	
	Dual-licensed under the	Educational Community License, Version 2.0 and
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
    /// <summary>
    /// Economy Beta v1.0 QuantumHive
    /// </summary>
    public sealed class CmdEconomy : Command {
        public override string name { get { return "economy"; } }
        public override string shortcut { get { return "eco"; } }
        public override string type { get { return CommandTypes.Economy; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Guest; } }
        public override CommandPerm[] AdditionalPerms {
            get { return new[] { new CommandPerm(LevelPermission.Operator, "The lowest rank that can setup the economy") }; }
        }        
        
        public override void Use(Player p, string message) {
            string[] command = message.Trim().Split(' ');
            string par0 = String.Empty;
            string par1 = String.Empty;
            string par2 = String.Empty;
            string par3 = String.Empty;
            string par4 = String.Empty;
            string par5 = String.Empty;
            string par6 = String.Empty;
            string par7 = String.Empty;
            string par8 = String.Empty;
            try {
                par0 = command[0].ToLower();
                par1 = command[1].ToLower();
                par2 = command[2];
                par3 = command[3];
                par4 = command[4];
                par5 = command[5];
                par6 = command[6];
                par7 = command[7];
                par8 = command[8];
            } catch { }
            string ecoColor = "%3";
            switch (par0) {
                case "setup":
                    if ((int)p.group.Permission >= CommandOtherPerms.GetPerm(this)) {
                        switch (par1) {
                            case "apply":
                                if (p == null || p.name == Server.server_owner) {
                                    Economy.Load();
                                    Player.SendMessage(p, "%aApplied changes");
                                } else {
                                    Player.SendMessage(p, "%cThis command is only usable by the server owner: %6" + Server.server_owner);
                                }
                                return;
                            case "maps":
                            case "levels":
                            case "map":
                            case "level":
                                Economy.Settings.Level lvl = Economy.FindLevel(par3);
                                switch (par2) {
                                    case "new":
                                    case "create":
                                    case "add":
                                        if (Economy.FindLevel(par3) != null) { Player.SendMessage(p, "%cThat preset level already exists"); break; } else {
                                            Economy.Settings.Level level = new Economy.Settings.Level();
                                            level.name = par3;
                                            if (isGood(par4) && isGood(par5) && isGood(par6)) { level.x = par4; level.y = par5; level.z = par6; } else { Player.SendMessage(p, "%cDimension must be  a power of 2"); break; }
                                            switch (par7.ToLower()) {
                                                case "flat":
                                                case "pixel":
                                                case "island":
                                                case "mountains":
                                                case "ocean":
                                                case "forest":
                                                case "desert":
                                                case "space":
                                                    level.type = par7.ToLower();
                                                    break;

                                                default:
                                                    Player.SendMessage(p, "%cValid types are: island, mountains, forest, ocean, flat, pixel, desert, space");
                                                    break;
                                            }
                                            try {
                                                level.price = int.Parse(par8);
                                            } catch { Player.SendMessage(p, "%cInvalid price input: that wasn't a number!"); return; }
                                            Economy.Settings.LevelsList.Add(level);
                                            Player.SendMessage(p, "%aSuccessfully added the map preset with the following specs:");
                                            Player.SendMessage(p, "Map Preset Name: %f" + level.name);
                                            Player.SendMessage(p, "x:" + level.x + ", y:" + level.y + ", z:" + level.z);
                                            Player.SendMessage(p, "Map Type: %f" + level.type);
                                            Player.SendMessage(p, "Map Price: %f" + level.price + " %3" + Server.moneys);
                                            break;
                                        }

                                    case "delete":
                                    case "remove":
                                        if (lvl == null) { Player.SendMessage(p, "%cThat preset level doesn't exist"); break; } else { Economy.Settings.LevelsList.Remove(lvl); Player.SendMessage(p, "%aSuccessfully removed preset: %f" + lvl.name); break; }

                                    case "edit":
                                    case "change":
                                        if (lvl == null) { Player.SendMessage(p, "%cThat preset level doesn't exist"); break; } else {
                                            switch (par4) {
                                                case "name":
                                                case "title":
                                                    Economy.Settings.LevelsList.Remove(lvl);
                                                    lvl.name = par5;
                                                    Economy.Settings.LevelsList.Add(lvl);
                                                    Player.SendMessage(p, "%aSuccessfully changed preset name to %f" + lvl.name);
                                                    break;

                                                case "x":
                                                    if (isGood(par5)) {
                                                        Economy.Settings.LevelsList.Remove(lvl);
                                                        lvl.x = par5;
                                                        Economy.Settings.LevelsList.Add(lvl);
                                                        Player.SendMessage(p, "%aSuccessfully changed preset x size to %f" + lvl.x);
                                                    } else { Player.SendMessage(p, "%cDimension was wrong, it must be a power of 2"); break; }
                                                    break;

                                                case "y":
                                                    if (isGood(par5)) {
                                                        Economy.Settings.LevelsList.Remove(lvl);
                                                        lvl.y = par5;
                                                        Economy.Settings.LevelsList.Add(lvl);
                                                        Player.SendMessage(p, "%aSuccessfully changed preset y size to %f" + lvl.y);
                                                    } else { Player.SendMessage(p, "%cDimension was wrong, it must be a power of 2"); break; }
                                                    break;

                                                case "z":
                                                    if (isGood(par5)) {
                                                        Economy.Settings.LevelsList.Remove(lvl);
                                                        lvl.z = par5;
                                                        Economy.Settings.LevelsList.Add(lvl);
                                                        Player.SendMessage(p, "%aSuccessfully changed preset z size to %f" + lvl.z);
                                                    } else { Player.SendMessage(p, "%cDimension was wrong, it must be a power of 2"); break; }
                                                    break;

                                                case "type":
                                                    Economy.Settings.LevelsList.Remove(lvl);
                                                    switch (par5.ToLower()) {
                                                        case "flat":
                                                        case "pixel":
                                                        case "island":
                                                        case "mountains":
                                                        case "ocean":
                                                        case "forest":
                                                        case "desert":
                                                        case "space":
                                                            lvl.type = par5.ToLower();
                                                            break;

                                                        default:
                                                            Player.SendMessage(p, "%cValid types are: island, mountains, forest, ocean, flat, pixel, desert, space");
                                                            Economy.Settings.LevelsList.Add(lvl);
                                                            return;
                                                    }
                                                    Economy.Settings.LevelsList.Add(lvl);
                                                    Player.SendMessage(p, "%aSuccessfully changed preset type to %f" + lvl.type);
                                                    break;

                                                /*case "dimensions":
                                                case "sizes":
                                                case "dimension":
                                                case "size":
                                                    Economy.Settings.LevelsList.Remove(lvl);
                                                    if (isGood(par4)) { lvl.x = par4; }
                                                    if (isGood(par5)) { lvl.y = par5; }
                                                    if (isGood(par6)) { lvl.z = par6; } else { Player.SendMessage(p, "A Dimension was wrong, it must a power of 2"); Economy.Settings.LevelsList.Add(lvl); break; }
                                                    Economy.Settings.LevelsList.Add(lvl);
                                                    Player.SendMessage(p, "Changed preset name");
                                                    break;*/

                                                case "price":
                                                    Economy.Settings.LevelsList.Remove(lvl);
                                                    int old = lvl.price;
                                                    try {
                                                        lvl.price = int.Parse(par5);
                                                    } catch {
                                                        Economy.Settings.LevelsList.Add(lvl);
                                                        Player.SendMessage(p, "%cInvalid amount of %3" + Server.moneys);
                                                        return;
                                                    }
                                                    if (lvl.price < 0) { Player.SendMessage(p, "%cAmount of %3" + Server.moneys + "%c cannot be negative"); lvl.price = old; Economy.Settings.LevelsList.Add(lvl); return; }
                                                    Economy.Settings.LevelsList.Add(lvl);
                                                    Player.SendMessage(p, "%aSuccessfully changed preset price to %f" + lvl.price + " %3" + Server.moneys);
                                                    break;

                                                default:
                                                    Player.SendMessage(p, "%cThat wasn't a valid command addition!");
                                                    break;
                                            }
                                        }
                                        break;

                                    case "enable":
                                        if (Economy.Settings.Levels) { Player.SendMessage(p, "%cMaps are already enabled for the economy system"); break; } else { Economy.Settings.Levels = true; Player.SendMessage(p, "%aMaps are now enabled for the economy system"); break; }

                                    case "disable":
                                        if (Economy.Settings.Levels == false) { Player.SendMessage(p, "%cMaps are already disabled for the economy system"); break; } else { Economy.Settings.Levels = false; Player.SendMessage(p, "%aMaps are now disabled for the economy system"); break; }

                                    default:
                                        Player.SendMessage(p, "%cThat wasn't a valid command addition!");
                                        break;
                                }
                                break;

                            case "titles":
                            case "title":
                                switch (par2) {
                                    case "enable":
                                        if (Economy.Settings.Titles) { Player.SendMessage(p, "%cTitles are already enabled for the economy system"); break; } else { Economy.Settings.Titles = true; Player.SendMessage(p, "%aTitles are now enabled for the economy system"); break; }

                                    case "disable":
                                        if (Economy.Settings.Titles == false) { Player.SendMessage(p, "%cTitles are already disabled for the economy system"); break; } else { Economy.Settings.Titles = false; Player.SendMessage(p, "%aTitles are now disabled for the economy system"); break; }

                                    case "price":
                                        try {
                                            Economy.Settings.TitlePrice = int.Parse(par3);
                                        } catch { Player.SendMessage(p, "%cInvalid price input: that wasn't a number!"); return; }
                                        Player.SendMessage(p, "%aSuccessfully changed the title price to: %f"  + Economy.Settings.TitlePrice + " %3" + Server.moneys);
                                        break;

                                    default:
                                        Player.SendMessage(p, "%cThat wasn't a valid command addition!");
                                        break;
                                }
                                break;

                            case "colors":
                            case "colours":
                            case "color":
                            case "colour":
                                switch (par2) {
                                    case "enable":
                                        if (Economy.Settings.Colors) { Player.SendMessage(p, "%cColors are already enabled for the economy system"); break; } else { Economy.Settings.Colors = true; Player.SendMessage(p, "%aColors are now enabled for the economy system"); break; }

                                    case "disable":
                                        if (Economy.Settings.Colors == false) { Player.SendMessage(p, "%cColors are already disabled for the economy system"); break; } else { Economy.Settings.Colors = false; Player.SendMessage(p, "%aColors are now disabled for the economy system"); break; }

                                    case "price":
                                        try {
                                            Economy.Settings.ColorPrice = int.Parse(par3);
                                        } catch { Player.SendMessage(p, "%cInvalid price input: that wasn't a number!"); return; }
                                        Player.SendMessage(p, "Successfully changed the color price to %f" + Economy.Settings.ColorPrice + " %3" + Server.moneys);
                                        break;

                                    default:
                                        Player.SendMessage(p, "%cThat wasn't a valid command addition!");
                                        break;
                                }
                                break;
                            case "tcolor":
                            case "tcolors":
                            case "titlecolor":
                            case "titlecolors":
                            case "tc":
                                switch (par2) {
                                    case "enable":
                                        if (Economy.Settings.TColors) Player.SendMessage(p, "%cTitleColors are already enabled for the economy system");
                                        else { Economy.Settings.TColors = true; Player.SendMessage(p, "%aTitleColors are now enabled for the economy system"); }
                                        break;
                                    case "disable":
                                        if (Economy.Settings.TColors == false) Player.SendMessage(p, "%cTitleColors are already disabled for the economy system");
                                        else { Economy.Settings.TColors = false; Player.SendMessage(p, "%aTitleColors are now disabled for the economy system"); }
                                        break;
                                    case "price":
                                        try {
                                            Economy.Settings.TColorPrice = int.Parse(par3);
                                        } catch { Player.SendMessage(p, "%cInvalid price input: that wasn't a number!"); return; }
                                        Player.SendMessage(p, "%aSuccessfully changed the titlecolor price to %f" + Economy.Settings.TColorPrice + " %3" + Server.moneys);
                                        break;
                                    default:
                                        Player.SendMessage(p, "%cThat wasn't a valid command addition!");
                                        break;
                                }
                                break;
                            case "ranks":
                            case "rank":
                                switch (par2) {
                                    case "enable":
                                        if (Economy.Settings.Ranks) { Player.SendMessage(p, "%cRanks are already enabled for the economy system"); break; } else { Economy.Settings.Ranks = true; Player.SendMessage(p, "%aRanks are now enabled for the economy system"); break; }

                                    case "disable":
                                        if (Economy.Settings.Ranks == false) { Player.SendMessage(p, "%cRanks are already disabled for the economy system"); break; } else { Economy.Settings.Ranks = false; Player.SendMessage(p, "%aRanks are now disabled for the economy system"); break; }

                                    case "price":
                                        Economy.Settings.Rank rnk = Economy.FindRank(par3);
                                        if (rnk == null) { Player.SendMessage(p, "%cThat wasn't a rank or it's past the max rank (maxrank is: " + Group.Find(Economy.Settings.MaxRank).color + Economy.Settings.MaxRank + "%c)"); break; } else {
                                            try {
                                                rnk.price = int.Parse(par4);
                                            } catch { Player.SendMessage(p, "%cInvalid price input: that wasn't a number!"); return; }
                                            Player.SendMessage(p, "%aSuccesfully changed the rank price for " + rnk.group.color + rnk.group.name + " to: %f" + rnk.price + " %3" + Server.moneys);
                                            break;
                                        }

                                    case "maxrank":
                                    case "max":
                                    case "maximum":
                                    case "maximumrank":
                                        Group grp = Group.Find(par3);
                                        if (grp == null) { Player.SendMessage(p, "%cThat wasn't a rank!"); } else {
                                            if (p.group.Permission < grp.Permission) { Player.SendMessage(p, "%cCan't set a maxrank that is higher than yours!"); } else {
                                                Economy.Settings.MaxRank = par3.ToLower(); Player.SendMessage(p, "%aSuccessfully set max rank to: " + Group.Find(Economy.Settings.MaxRank).color + Economy.Settings.MaxRank);
                                                int lasttrueprice = 0;
                                                foreach (Group group in Group.GroupList) {
                                                    if (group.Permission > grp.Permission) { break; }
                                                    if (!(group.Permission <= Group.Find(Server.defaultRank).Permission)) {
                                                        Economy.Settings.Rank rank = new Economy.Settings.Rank();
                                                        rank = Economy.FindRank(group.name);
                                                        if (rank == null) {
                                                            rank = new Economy.Settings.Rank();
                                                            rank.group = group;
                                                            if (lasttrueprice == 0) { rank.price = 1000; } else { rank.price = lasttrueprice + 250; }
                                                            Economy.Settings.RanksList.Add(rank);
                                                        } else { lasttrueprice = rank.price; }
                                                    }
                                                }
                                            }
                                        }
                                        break;
                                    default:
                                        Player.SendMessage(p, "%cThat wasn't a valid command addition!");
                                        break;
                                }
                                break;

                            case "enable":
                                if (Economy.Settings.Enabled) { Player.SendMessage(p, "%cThe economy system is already enabled"); return; } else { Economy.Settings.Enabled = true; Player.SendMessage(p, "%aThe economy system is now enabled"); return; }

                            case "disable":
                                if (Economy.Settings.Enabled == false) { Player.SendMessage(p, "%cThe economy system is already disabled"); return; } else { Economy.Settings.Enabled = false; Player.SendMessage(p, "%aThe economy system is now disabled"); return; }

                            default:
                                if (par1 == null || par1 == "") {
                                    SetupHelp(p);
                                    return;
                                }
                                Player.SendMessage(p, "%cThat wasn't a valid command addition!");
                                return;
                        }
                        Economy.Save();
                        return;
                    } else { Player.SendMessage(p, "%cYou are not allowed to use %f/eco setup"); return; }



                case "buy":
                    if (p == null) { Player.SendMessage(p, "%cConsole cannot buy any items"); return; }
                    Economy.EcoStats ecos = Economy.RetrieveEcoStats(p.name);
                    switch (par1) {
                        case "map":
                        case "level":
                        case "maps":
                        case "levels":
                            Economy.Settings.Level lvl = Economy.FindLevel(par2);
                            if (lvl == null) { Player.SendMessage(p, "%cThat isn't a level preset"); return; } else {
                                if (!p.EnoughMoney(lvl.price)) {
                                    Player.SendMessage(p, "%cYou don't have enough %3" + Server.moneys + "%c to buy that map");
                                    return;
                                } else {
                                    if (par3 == null) { Player.SendMessage(p, "%cYou didn't specify a name for your level"); return; } else {
                                        int old = p.money;
                                        int oldTS = ecos.totalSpent;
                                        string oldP = ecos.purchase;
                                        try {
                                            Command.all.Find("newlvl").Use(null, p.name + "_" + par3 + " " + lvl.x + " " + lvl.y + " " + lvl.z + " " + lvl.type);
                                            Player.SendMessage(p, "%aCreating level: '%f" + p.name + "_" + par3 + "%a' . . .");
                                            p.money = p.money - lvl.price;
                                            ecos.money = p.money;
                                            ecos.totalSpent += lvl.price;
                                            ecos.purchase = "%3Map: %f" + lvl.name + "%3 - Price: %f"  + lvl.price + " %3" + Server.moneys + " - Date: %f" + ecoColor + DateTime.Now.ToString(CultureInfo.InvariantCulture);
                                            Economy.UpdateEcoStats(ecos);
                                            Command.all.Find("load").Use(null, p.name + "_" + par3);
                                            Thread.Sleep(250);
                                            Level level = LevelInfo.Find(p.name + "_" + par3);
                                            if (level.permissionbuild > p.group.Permission) { level.permissionbuild = p.group.Permission; }
                                            if (level.permissionvisit > p.group.Permission) { level.permissionvisit = p.group.Permission; }
                                            Command.all.Find("goto").Use(p, p.name + "_" + par3);

                                            Player.SendMessage(p, "%aSuccessfully created your map: '%f" + p.name + "_" + par3 + "%a'");
                                            Player.SendMessage(p, "%aYour balance is now %f" + p.money.ToString() + " %3" + Server.moneys);
                                            try {
                                                //safe against SQL injections, but will be replaced soon by a new feature
                                                //DB
                                                Database.executeQuery("INSERT INTO `Zone" + level.name + "` (SmallX, SmallY, SmallZ, BigX, BigY, BigZ, Owner) VALUES " +
                                                                      "(0,0,0," + (level.Width - 1) + "," + (level.Height - 1) + "," + (level.Length - 1) + ",'" + p.name + "')");
                                                //DB
                                                Player.SendMessage(p, "%aZoning Succesful");
                                                return;
                                            } catch { Player.SendMessage(p, "%cZoning Failed"); return; }
                                        } catch { Player.SendMessage(p, "%cSomething went wrong, Money restored"); if (old != p.money) { p.money = old; ecos.money = old; ecos.totalSpent = oldTS; ecos.purchase = oldP; Economy.UpdateEcoStats(ecos); } return; }
                                    }
                                }
                            }

                        case "colors":
                        case "color":
                        case "colours":
                        case "colour":
                            if (p.EnoughMoney(Economy.Settings.ColorPrice) == false) {
                                Player.SendMessage(p, "%cYou don't have enough %3" + Server.moneys + "%c to buy a color");
                                return;
                            }
                            if (!par2.StartsWith("&") || !par2.StartsWith("%")) {
                            	par2 = Colors.Parse(par2);
                            	if (par2 == "") {
                            		Player.SendMessage(p, "%cThat wasn't a color");
                            		return;
                                }
                            }
                            if (par2 == p.color) {
                                Player.SendMessage(p, "%cYou already have a " + par2 + Colors.Name(par2) + "%c color");
                                return;
                            } else {
                                Command.all.Find("color").Use(null, p.name + " " + Colors.Name(par2));
                                p.money = p.money - Economy.Settings.ColorPrice;
                                ecos.money = p.money;
                                ecos.totalSpent += Economy.Settings.ColorPrice;
                                ecos.purchase = "%3Color: " + par2 + Colors.Name(par2) + "%3 - Price: %f" + Economy.Settings.ColorPrice + " %3" + Server.moneys + " - Date: %f" + DateTime.Now.ToString(CultureInfo.InvariantCulture);
                                Economy.UpdateEcoStats(ecos);
                                Player.SendMessage(p, "%aYour color has been successfully changed to " + par2 + Colors.Name(par2));
                                Player.SendMessage(p, "%aYour balance is now %f" + p.money.ToString() + " %3" + Server.moneys);
                                return;
                            }

                        case "tcolor":
                        case "tcolors":
                        case "titlecolor":
                        case "titlecolors":
                        case "tc":
                            if (!p.EnoughMoney(Economy.Settings.TColorPrice)) {
                                Player.SendMessage(p, "%cYou don't have enough %3" + Server.moneys + "%c to buy a titlecolor");
                                return;
                            }
                            if (!par2.StartsWith("&") || !par2.StartsWith("%")) {
                                par2 = Colors.Parse(par2);
                            	if (par2 == "") {
                            		Player.SendMessage(p, "%cThat wasn't a color");
                            		return;
                                }
                            }
                            if (par2 == p.titlecolor) {
                                Player.SendMessage(p, "%cYou already have a " + par2 + Colors.Name(par2) + "%c titlecolor");
                                return;
                            } else {
                                Command.all.Find("tcolor").Use(null, p.name + " " + Colors.Name(par2));
                                p.money = p.money - Economy.Settings.TColorPrice;
                                ecos.money = p.money;
                                ecos.totalSpent += Economy.Settings.TColorPrice;
                                ecos.purchase = "%3Titlecolor: " + par2 + Colors.Name(par2) + "%3 - Price: %f" + Economy.Settings.TColorPrice + " %3" + Server.moneys + " - Date: %f" + DateTime.Now.ToString(CultureInfo.InvariantCulture);
                                Economy.UpdateEcoStats(ecos);
                                Player.SendMessage(p, "%aYour titlecolor has been successfully changed to " + par2 + Colors.Name(par2));
                                Player.SendMessage(p, "%aYour balance is now %f" + p.money + " %3" + Server.moneys);
                                return;
                            }

                        case "titles":
                        case "title":
                            if (p.EnoughMoney(Economy.Settings.TitlePrice) == false) {
                                Player.SendMessage(p, "%cYou don't have enough %3" + Server.moneys + "%c to buy a title");
                                return;
                            }
                            if (par3 != string.Empty) {
                                Player.SendMessage(p, "%cYour title cannot contain any spaces");
                                return;
                            }
                            if (par2 == p.title) {
                                Player.SendMessage(p, "%cYou already have that title");
                                return;
                            }
                            if (par2.Length > 17) {
                                Player.SendMessage(p, "%cTitles cannot be longer than 17 characters");
                                return;
                            }
                            var regex = new System.Text.RegularExpressions.Regex(@"^[a-zA-Z0-9-_\\.]*$");
                            if (!regex.IsMatch(par2)) {
                                Player.SendMessage(p, "%cInvalid title! Titles may only contain alphanumeric characters and .-_");
                                return;
                            }
                            bool free = false;
                            if (par2 == null || par2 == string.Empty || par2 == "") {
                                par2 = ""; //just an extra check to make sure it's good
                                free = true;
                            }
                            Command.all.Find("title").Use(null, p.name + " " + par2);
                            if (!free) {
                                p.money = p.money - Economy.Settings.TitlePrice;
                                ecos.money = p.money;
                                ecos.totalSpent += Economy.Settings.TitlePrice;
                                ecos.purchase = "%3Title: %f" + par2 + "%3 - Price: %f" + Economy.Settings.TitlePrice + " %3" + Server.moneys + " - Date: %f" + DateTime.Now.ToString(CultureInfo.InvariantCulture);
                                Economy.UpdateEcoStats(ecos);
                                Player.SendMessage(p, "%aYour title has been successfully changed to [" + p.titlecolor + par2 + "%a]");
                            } else { Player.SendMessage(p, "%aYour title has been successfully removed for free"); }
                            Player.SendMessage(p, "%aYour balance is now %f" + p.money + " %3" + Server.moneys);
                            return;

                        case "ranks":
                        case "rank":
                            if (par2 != "" && par2 != null && !string.IsNullOrEmpty(par2) && par2 != string.Empty) {
                                Player.SendMessage(p, "%cYou cannot provide a rank name, use %a/eco buy rank %cto buy the NEXT rank.");
                                return;
                            }

                            LevelPermission maxrank = Group.Find(Economy.Settings.MaxRank).Permission;
                            if (p.group.Permission == maxrank || p.group.Permission >= maxrank) {
                                Player.SendMessage(p, "%cYou cannot buy anymore ranks, because you passed the max buyable rank: " + Group.Find(Economy.Settings.MaxRank).color + Economy.Settings.MaxRank);
                                return;
                            } else {
                                if (!p.EnoughMoney(Economy.NextRank(p).price)) {
                                    Player.SendMessage(p, "%cYou don't have enough %3" + Server.moneys + "%c to buy the next rank");
                                    return;
                                }
                                Command.all.Find("promote").Use(null, p.name);
                                p.money = p.money - Economy.FindRank(p.group.name).price;
                                ecos.money = p.money;
                                ecos.totalSpent += Economy.FindRank(p.group.name).price;
                                ecos.purchase = "%3Rank: " + p.group.color + p.group.name + "%3 - Price: %f" + Economy.FindRank(p.group.name).price + " %3" + Server.moneys + " - Date: %f" + DateTime.Now.ToString(CultureInfo.InvariantCulture);
                                Economy.UpdateEcoStats(ecos);
                                Player.SendMessage(p, "%aYou've successfully bought the rank " + p.group.color + p.group.name);
                                Player.SendMessage(p, "%aYour balance is now %f" + p.money + " %3" + Server.moneys);
                                return;
                            }

                        default:
                            Player.SendMessage(p, "%cThat wasn't a valid command addition!");
                            return;
                    }

                case "stats":
                case "balance":
                case "amount":
                    Economy.EcoStats ecostats;
                    if (par1 != string.Empty && par1 != null && par1 != "") {
                        Player who = PlayerInfo.Find(par1); //is player online?
                        if (who == null) { //player is offline
                            ecostats = Economy.RetrieveEcoStats(par1);
                            Player.SendMessage(p, "%3===Economy stats for: %f" + ecostats.playerName + "%7(offline)%3===");
                        } else { //player is online
                            ecostats = Economy.RetrieveEcoStats(who.name);
                            Player.SendMessage(p, "%3===Economy stats for: " + who.color + who.name + "%3===");
                        }
                    } else if (p != null) { //this player
                        ecostats = Economy.RetrieveEcoStats(p.name);
                        Player.SendMessage(p, "%3===Economy stats for: " + p.color + p.name + "%3===");
                    } else { Player.SendMessage(p, "%cConsole cannot contain any eco stats"); return; }
                    Player.SendMessage(p, "Balance: %f" + ecostats.money + " %3" + Server.moneys);
                    Player.SendMessage(p, "Total spent: %f" + ecostats.totalSpent + " %3" + Server.moneys);
                    Player.SendMessage(p, "Recent purchase: " + ecostats.purchase);
                    Player.SendMessage(p, "Recent payment: " + ecostats.payment);
                    Player.SendMessage(p, "Recent receivement: " + ecostats.salary);
                    Player.SendMessage(p, "Recent fine: " + ecostats.fine);
                    return;
                case "info":
                case "about":
                    if (Economy.Settings.Enabled) {
                        switch (par1) {
                            case "map":
                            case "level":
                            case "maps":
                            case "levels":
                                if (Economy.Settings.Levels == false) { Player.SendMessage(p, "%cMaps are not enabled for the economy system"); return; }
                                Player.SendMessage(p, ecoColor + "%3===Economy info: Maps===");
                                Player.SendMessage(p, "%aAvailable maps to buy:");
                                if (Economy.Settings.LevelsList.Count == 0)
                                    Player.SendMessage(p, "%8-None-");
                                else
                                    foreach (Economy.Settings.Level lvl in Economy.Settings.LevelsList) {
                                        Player.SendMessage(p, lvl.name + " (" + lvl.x + "," + lvl.y + "," + lvl.z + ") " + lvl.type + ": %f" + lvl.price + " %3" + Server.moneys);
                                    }
                                return;

                            case "title":
                            case "titles":
                                if (Economy.Settings.Titles == false) { Player.SendMessage(p, "%cTitles are not enabled for the economy system"); return; }
                                Player.SendMessage(p, ecoColor + "%3===Economy info: Titles===");
                                Player.SendMessage(p, "Titles cost %f" + Economy.Settings.TitlePrice + " %3" + Server.moneys + Server.DefaultColor + " each");
                                return;

                            case "tcolor":
                            case "tcolors":
                            case "titlecolor":
                            case "titlecolors":
                            case "tc":
                                if (!Economy.Settings.TColors) { Player.SendMessage(p, "%cTitlecolors are not enabled for the economy system"); return; }
                                Player.SendMessage(p, ecoColor + "%3===Economy info: Titlecolors===");
                                Player.SendMessage(p, "Titlecolors cost %f" + Economy.Settings.TColorPrice + " %3" + Server.moneys + Server.DefaultColor + " each");
                                return;

                            case "colors":
                            case "color":
                            case "colours":
                            case "colour":
                                if (Economy.Settings.Colors == false) { Player.SendMessage(p, "%cColors are not enabled for the economy system"); return; }
                                Player.SendMessage(p, ecoColor + "%3===Economy info: Colors===");
                                Player.SendMessage(p, "Colors cost %f" + Economy.Settings.ColorPrice + " %3" + Server.moneys + Server.DefaultColor + " each");
                                return;

                            case "ranks":
                            case "rank":
                                if (Economy.Settings.Ranks == false) { Player.SendMessage(p, "%cRanks are not enabled for the economy system"); return; }
                                Player.SendMessage(p, ecoColor + "%3===Economy info: Ranks===");
                                Player.SendMessage(p, "%fThe maximum buyable rank is: " + Group.Find(Economy.Settings.MaxRank).color + Economy.Settings.MaxRank);
                                Player.SendMessage(p, "%cRanks purchased will be bought in order.");
                                Player.SendMessage(p, "%fRanks cost:");
                                foreach (Economy.Settings.Rank rnk in Economy.Settings.RanksList) {
                                    Player.SendMessage(p, rnk.group.color + rnk.group.name + ": %f" + rnk.price + " %3" + Server.moneys);
                                    if (rnk.group.name == Economy.Settings.MaxRank.ToLower())
                                        break;
                                }
                                return;

                            default:
                                Player.SendMessage(p, "%cThat wasn't a valid command addition!");
                                return;
                        }
                    } else { Player.SendMessage(p, "%cThe %3Economy System %cis currently disabled!"); return; }

                case "help":
                    switch (par1) {
                        case "":
                            Help(p);
                            return;

                        case "buy":
                            Player.SendMessage(p, "%3===Economy Help: Buy===");
                            Player.SendMessage(p, "Buying titles: %f/eco buy title [title_name]");
                            Player.SendMessage(p, "Buying colors: %f/eco buy color [color]");
                            Player.SendMessage(p, "Buying titlecolors: %f/eco buy tcolor [color]");
                            Player.SendMessage(p, "Buying ranks: %f/eco buy the NEXT rank");
                            Player.SendMessage(p, "%7Check out the ranks and their prices with: %f/eco info rank");
                            Player.SendMessage(p, "Buy your own maps: %f/eco buy map [map_preset_name] [custom_map_name]");
                            Player.SendMessage(p, "%7Check out the map presets with: %f/eco info map");
                            return;

                        case "stats":
                        case "balance":
                        case "amount":
                            Player.SendMessage(p, "%3===Economy Help: Stats===");
                            Player.SendMessage(p, "Check your stats: %f/eco stats");
                            Player.SendMessage(p, "Check the stats of a player: %f/eco stats [player_name]");
                            return;

                        case "info":
                        case "about":
                            Player.SendMessage(p, "%3===Economy Help: Info===");
                            Player.SendMessage(p, "To get info and prices about features: %f/eco info [color/title/titlecolor/rank/map]");
                            return;

                        case "setup":
                            if ((int)p.group.Permission >= CommandOtherPerms.GetPerm(this)) {
                                SetupHelp(p);
                                return;
                            } else { Player.SendMessage(p, "%cYou are not allowed to use %f/eco help setup"); return; }

                        default:
                            Player.SendMessage(p, "%cThat wasn't a valid command addition, sending you to help");
                            Help(p);
                            return;
                    }

                default:
                    //Player.SendMessage(p, "%4That wasn't a valid command addition, Sending you to help:");
                    Help(p);
                    return;
            }
        }
        public override void Help(Player p) {
            string defaultcolor = Group.findPerm(defaultRank).color;
            string othercolor = Group.findPermInt(CommandOtherPerms.GetPerm(this)).color;
            Player.SendMessage(p, "%3===Welcome to the Economy Help Menu===");
            Player.SendMessage(p, defaultcolor + "%f/eco buy <title/color/tcolor/rank/map> [%atitle/color/tcolor/map_preset%f] [%acustom_map_name%f] %e- to buy one of these features");
            Player.SendMessage(p, defaultcolor + "%f/eco stats [%aplayer%f] %e- view ecostats about yourself or [player]");
            Player.SendMessage(p, defaultcolor + "%f/eco info <title/color/tcolor/rank/map> %e- view information about buying the specific feature");
            if ((int)p.group.Permission >= CommandOtherPerms.GetPerm(this)) {
                Player.SendMessage(p, othercolor + "%f/eco setup <type> %e- to setup economy");
                Player.SendMessage(p, othercolor + "%f/eco help <buy/stats/info/setup> %e- get more specific help");
            } else { Player.SendMessage(p, defaultcolor + "%f/eco help <buy/stats/info> %e- get more specific help"); }
        }

        public void SetupHelp(Player p) {
            Player.SendMessage(p, "%3===Economy Setup Help Menu===");
            if (p !=null && p.name == Server.server_owner) {
                Player.SendMessage(p, "%4/eco setup apply %e- applies the changes made to 'economy.properties'");
            } else if (p == null) { Player.SendMessage(p, "%4/eco setup apply %e- applies the changes made to 'economy.properties'"); }
            Player.SendMessage(p, "%4/eco setup [%aenable%4/%cdisable%4] %e- to enable/disable the economy system");
            Player.SendMessage(p, "%4/eco setup [title/color/tcolor/rank/map] [%aenable%4/%cdisable%4] %e- to enable/disable that feature");
            Player.SendMessage(p, "%4/eco setup [title/color/tcolor] [%3price%4] %e- to setup the prices for these features");
            Player.SendMessage(p, "%4/eco setup rank price [%frank%4] [%3price%4] %e- to set the price for that rank");
            Player.SendMessage(p, "%4/eco setup rank maxrank [%frank%4] %e- to set the max buyable rank");
            Player.SendMessage(p, "%4/eco setup map new [%fname%4] [%fx%4] [%fy%4] [%fz%4] [%ftype%4] [%3price%4] %e- to setup a map preset");
            Player.SendMessage(p, "%4/eco setup map delete [%fname%4] %e- to delete a map");
            Player.SendMessage(p, "%4/eco setup map edit [%fname%4] [name/x/y/z/type/price] [%fvalue%4] %e- to edit a map preset");
        }

        public bool isGood(string value) {
            ushort uvalue = ushort.Parse(value);
            switch (uvalue) {
                case 2:
                case 4:
                case 8:
                case 16:
                case 32:
                case 64:
                case 128:
                case 256:
                case 512:
                case 1024:
                case 2048:
                case 4096:
                case 8192:
                    return true;
            }

            return false;
        }
    }
}
