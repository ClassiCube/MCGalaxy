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
using MCGalaxy.Eco;
using MCGalaxy.SQL;

namespace MCGalaxy.Commands {
    
    /// <summary> Economy Beta v1.0 QuantumHive </summary>
    public sealed class CmdBuy : Command {
        public override string name { get { return "buy"; } }
        public override string shortcut { get { return "purchase"; } }
        public override string type { get { return CommandTypes.Economy; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Guest; } }
        public override bool Enabled { get { return Economy.Enabled; } }
        
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

                default:
                    foreach (Item item in Economy.Items)
                        foreach (string alias in item.Aliases) 
                    {
                        if (parts[0].CaselessEquals(alias)) {
                            item.OnBuyCommand(this, p, parts); return;
                        }
                    }
                    Player.SendMessage(p, "%cThat wasn't a valid command addition!");
                    break;
            }
        }
        
        public override void Help(Player p) {
            Player.SendMessage(p, "%T/buy <title/color/tcolor/rank/map> [value] <map name>");
            Player.SendMessage(p, "%Hvalue is either [title/color/tcolor/map_preset]");
            Player.SendMessage(p, "%Hmap name is only used for %T/buy map%H.");
            Player.SendMessage(p, "%HUse %T/store <type> %Hto see the cost of an item.");
        }
    }
}
