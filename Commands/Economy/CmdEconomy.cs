/*
    Copyright 2011 MCForge
    
    Dual-licensed under the    Educational Community License, Version 2.0 and
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
	
    /// <summary> Economy Beta v1.0 QuantumHive  </summary>
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
            string[] raw = message.Split(' ');
            string[] args = { "", "", "", "", "", "", "", "", "" };
            for (int i = 0; i < raw.Length; i++)
                args[i] = i < 2 ? raw[i].ToLower() : raw[i];

            switch (args[0]) {
                case "setup":
                    HandleSetup(p, message, args); break;
                case "stats":
                case "balance":
                case "amount":
                    HandleStats(p, message, args); break;
                case "help":
                    HandleHelp(p, message, args); break;
                default:
                    Help(p); break;
            }
        }
        
        void HandleSetup(Player p, string message, string[] args) {
            if (p != null && (int)p.group.Permission < CommandOtherPerms.GetPerm(this)) {
                Player.SendMessage(p, "%cYou are not allowed to use %f/eco setup"); return;
            }
            
            switch (args[1]) {
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
                    Economy.Settings.Level lvl = Economy.FindLevel(args[3]);
                    switch (args[2]) {
                        case "new":
                        case "create":
                        case "add":
                            if (Economy.FindLevel(args[3]) != null) { Player.SendMessage(p, "%cThat preset level already exists"); break; } else {
                                Economy.Settings.Level level = new Economy.Settings.Level();
                                level.name = args[3];
                                if (isGood(args[4]) && isGood(args[5]) && isGood(args[6])) {
                                    level.x = args[4]; level.y = args[5]; level.z = args[6];
                                } else { Player.SendMessage(p, "%cDimension must be a power of 2"); break; }
                                
                                if (!MapGen.IsRecognisedFormat(args[7])) {
                                    MapGen.PrintValidFormats(p); return;
                                }
                                level.type = args[7].ToLower();
                                try {
                                    level.price = int.Parse(args[8]);
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
                            if (lvl == null) { Player.SendMessage(p, "%cThat preset level doesn't exist"); return; }
                            Economy.Settings.LevelsList.Remove(lvl);
                            Player.SendMessage(p, "%aSuccessfully removed preset: %f" + lvl.name);
                            break;

                        case "edit":
                        case "change":
                            if (lvl == null) { Player.SendMessage(p, "%cThat preset level doesn't exist"); return; }
                            
                            switch (args[4]) {
                                case "name":
                                case "title":
                                    lvl.name = args[5];
                                    Player.SendMessage(p, "%aSuccessfully changed preset name to %f" + lvl.name);
                                    break;

                                case "x":
                                    if (isGood(args[5])) {
                                        lvl.x = args[5];
                                        Player.SendMessage(p, "%aSuccessfully changed preset x size to %f" + lvl.x);
                                    } else { Player.SendMessage(p, "%cDimension was wrong, it must be a power of 2"); break; }
                                    break;

                                case "y":
                                    if (isGood(args[5])) {
                                        lvl.y = args[5];
                                        Player.SendMessage(p, "%aSuccessfully changed preset y size to %f" + lvl.y);
                                    } else { Player.SendMessage(p, "%cDimension was wrong, it must be a power of 2"); break; }
                                    break;

                                case "z":
                                    if (isGood(args[5])) {
                                        lvl.z = args[5];
                                        Player.SendMessage(p, "%aSuccessfully changed preset z size to %f" + lvl.z);
                                    } else { Player.SendMessage(p, "%cDimension was wrong, it must be a power of 2"); break; }
                                    break;

                                case "type":
                                    if (MapGen.IsRecognisedFormat(args[5])) {
                                        lvl.type = args[5].ToLower();
                                    } else {
                                        MapGen.PrintValidFormats(p); return;
                                    }
                                    Player.SendMessage(p, "%aSuccessfully changed preset type to %f" + lvl.type);
                                    break;

                                case "price":
                                    int newPrice = 0;
                                    if (!int.TryParse(args[5], out newPrice)) {
                                        Player.SendMessage(p, "%cInvalid amount of %3" + Server.moneys); return;
                                    }
                                    if (newPrice < 0) { 
                                        Player.SendMessage(p, "%cAmount of %3" + Server.moneys + "%c cannot be negative"); return; 
                                    }
                                    lvl.price = newPrice;
                                    Player.SendMessage(p, "%aSuccessfully changed preset price to %f" + lvl.price + " %3" + Server.moneys);
                                    break;

                                default:
                                    Player.SendMessage(p, "%cThat wasn't a valid command addition!");
                                    break;
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
                    switch (args[2]) {
                        case "enable":
                            if (Economy.Settings.Titles) { Player.SendMessage(p, "%cTitles are already enabled for the economy system"); break; } else { Economy.Settings.Titles = true; Player.SendMessage(p, "%aTitles are now enabled for the economy system"); break; }

                        case "disable":
                            if (Economy.Settings.Titles == false) { Player.SendMessage(p, "%cTitles are already disabled for the economy system"); break; } else { Economy.Settings.Titles = false; Player.SendMessage(p, "%aTitles are now disabled for the economy system"); break; }

                        case "price":
                            try {
                                Economy.Settings.TitlePrice = int.Parse(args[3]);
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
                    switch (args[2]) {
                        case "enable":
                            if (Economy.Settings.Colors) { Player.SendMessage(p, "%cColors are already enabled for the economy system"); break; } else { Economy.Settings.Colors = true; Player.SendMessage(p, "%aColors are now enabled for the economy system"); break; }

                        case "disable":
                            if (Economy.Settings.Colors == false) { Player.SendMessage(p, "%cColors are already disabled for the economy system"); break; } else { Economy.Settings.Colors = false; Player.SendMessage(p, "%aColors are now disabled for the economy system"); break; }

                        case "price":
                            try {
                                Economy.Settings.ColorPrice = int.Parse(args[3]);
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
                    switch (args[2]) {
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
                                Economy.Settings.TColorPrice = int.Parse(args[3]);
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
                    switch (args[2]) {
                        case "enable":
                            if (Economy.Settings.Ranks) { Player.SendMessage(p, "%cRanks are already enabled for the economy system"); break; } else { Economy.Settings.Ranks = true; Player.SendMessage(p, "%aRanks are now enabled for the economy system"); break; }

                        case "disable":
                            if (Economy.Settings.Ranks == false) { Player.SendMessage(p, "%cRanks are already disabled for the economy system"); break; } else { Economy.Settings.Ranks = false; Player.SendMessage(p, "%aRanks are now disabled for the economy system"); break; }

                        case "price":
                            Economy.Settings.Rank rnk = Economy.FindRank(args[3]);
                            if (rnk == null) { Player.SendMessage(p, "%cThat wasn't a rank or it's past the max rank (maxrank is: " + Group.Find(Economy.Settings.MaxRank).color + Economy.Settings.MaxRank + "%c)"); break; } else {
                                try {
                                    rnk.price = int.Parse(args[4]);
                                } catch { Player.SendMessage(p, "%cInvalid price input: that wasn't a number!"); return; }
                                Player.SendMessage(p, "%aSuccesfully changed the rank price for " + rnk.group.color + rnk.group.name + " to: %f" + rnk.price + " %3" + Server.moneys);
                                break;
                            }

                        case "maxrank":
                        case "max":
                        case "maximum":
                        case "maximumrank":
                            Group grp = Group.Find(args[3]);
                            if (grp == null) { Player.SendMessage(p, "%cThat wasn't a rank!"); } else {
                                if (p.group.Permission < grp.Permission) { Player.SendMessage(p, "%cCan't set a maxrank that is higher than yours!"); } else {
                                    Economy.Settings.MaxRank = args[3].ToLower(); Player.SendMessage(p, "%aSuccessfully set max rank to: " + Group.Find(Economy.Settings.MaxRank).color + Economy.Settings.MaxRank);
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
                    if (Economy.Settings.Enabled) { Player.SendMessage(p, "%cThe economy system is already enabled"); return; }
                    else { Economy.Settings.Enabled = true; Player.SendMessage(p, "%aThe economy system is now enabled"); return; }

                case "disable":
                    if (!Economy.Settings.Enabled) { Player.SendMessage(p, "%cThe economy system is already disabled"); return; }
                    else { Economy.Settings.Enabled = false; Player.SendMessage(p, "%aThe economy system is now disabled"); return; }

                default:
                    if (args[1] == "") { SetupHelp(p); return; }
                    Player.SendMessage(p, "%cThat wasn't a valid command addition!");
                    return;
            }
            Economy.Save();
        }
        
        void HandleStats(Player p, string message, string[] args) {
            Economy.EcoStats ecostats;
            if (p == null && args[1] == "") {
                Player.SendMessage(p, "You must provide a name when using the command from console."); return;
            }

            if (args[1] != "") {
                Player who = PlayerInfo.Find(args[1]);
                if (who == null) {
                    ecostats = Economy.RetrieveEcoStats(args[1]);
                    Player.SendMessage(p, "%3===Economy stats for: %f" + ecostats.playerName + "%7(offline)%3===");
                } else {
                    ecostats = Economy.RetrieveEcoStats(who.name);
                    Player.SendMessage(p, "%3===Economy stats for: " + who.color + who.name + "%3===");
                }
            } else {
                ecostats = Economy.RetrieveEcoStats(p.name);
                Player.SendMessage(p, "%3===Economy stats for: " + p.color + p.name + "%3===");
            }
            
            Player.SendMessage(p, "Balance: %f" + ecostats.money + " %3" + Server.moneys);
            Player.SendMessage(p, "Total spent: %f" + ecostats.totalSpent + " %3" + Server.moneys);
            Player.SendMessage(p, "Recent purchase: " + ecostats.purchase);
            Player.SendMessage(p, "Recent payment: " + ecostats.payment);
            Player.SendMessage(p, "Recent receivement: " + ecostats.salary);
            Player.SendMessage(p, "Recent fine: " + ecostats.fine);
        }
        
        void HandleHelp(Player p, string message, string[] args) {
            switch (args[1]) {
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
                        SetupHelp(p); return;
                    } else { Player.SendMessage(p, "%cYou are not allowed to use %f/eco help setup"); return; }

                default:
                    Player.SendMessage(p, "%cThat wasn't a valid command addition, sending you to help");
                    Help(p); return;
            }
        }
        
        public override void Help(Player p) {
            Player.SendMessage(p, "%3===Welcome to the Economy Help Menu===");
            Player.SendMessage(p, "%f/eco stats [%aplayer%f] %e- view ecostats about yourself or [player]");
            Player.SendMessage(p, "%f/eco info <title/color/tcolor/rank/map> %e- view information about buying the specific feature");
            if ((int)p.group.Permission >= CommandOtherPerms.GetPerm(this)) {
                Player.SendMessage(p, "%f/eco setup <type> %e- to setup economy");
                Player.SendMessage(p, "%f/eco help <buy/stats/info/setup> %e- get more specific help");
            } else { Player.SendMessage(p, "%f/eco help <buy/stats/info> %e- get more specific help"); }
        }

        void SetupHelp(Player p) {
            Player.SendMessage(p, "%3===Economy Setup Help Menu===");
            if (p == null || p.name == Server.server_owner) {
                Player.SendMessage(p, "%4/eco setup apply %e- applies the changes made to 'economy.properties'");
            }
            Player.SendMessage(p, "%4/eco setup [%aenable%4/%cdisable%4] %e- to enable/disable the economy system");
            Player.SendMessage(p, "%4/eco setup [title/color/tcolor/rank/map] [%aenable%4/%cdisable%4] %e- to enable/disable that feature");
            Player.SendMessage(p, "%4/eco setup [title/color/tcolor] [%3price%4] %e- to setup the prices for these features");
            Player.SendMessage(p, "%4/eco setup rank price [%frank%4] [%3price%4] %e- to set the price for that rank");
            Player.SendMessage(p, "%4/eco setup rank maxrank [%frank%4] %e- to set the max buyable rank");
            Player.SendMessage(p, "%4/eco setup map new [%fname%4] [%fx%4] [%fy%4] [%fz%4] [%ftype%4] [%3price%4] %e- to setup a map preset");
            Player.SendMessage(p, "%4/eco setup map delete [%fname%4] %e- to delete a map");
            Player.SendMessage(p, "%4/eco setup map edit [%fname%4] [name/x/y/z/type/price] [%fvalue%4] %e- to edit a map preset");
        }

        public static bool isGood(string value) {
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
