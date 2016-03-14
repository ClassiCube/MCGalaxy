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
	
    /// <summary> Economy Beta v1.0 QuantumHive  </summary>
    public sealed class CmdStore : Command {
        public override string name { get { return "store"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.Economy; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Guest; } }
        public override bool Enabled { get { return Economy.Settings.Enabled; } }

        public override void Use(Player p, string message) {
            if (!Economy.Settings.Enabled) {
                Player.SendMessage(p, "%cThe %3Economy System %cis currently disabled!"); return;
            }
        	
            switch (message) {
                case "map":
                case "level":
                case "maps":
                case "levels":
                    if (!Economy.Settings.Levels) { Player.SendMessage(p, "%cMaps are not enabled for the economy system"); return; }
                    Player.SendMessage(p, "%3===Economy info: Maps===");
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
                    if (!Economy.Settings.Titles ) { Player.SendMessage(p, "%cTitles are not enabled for the economy system"); return; }
                    Player.SendMessage(p, "%3===Economy info: Titles===");
                    Player.SendMessage(p, "Titles cost %f" + Economy.Settings.TitlePrice + " %3" + Server.moneys + " %Seach");
                    return;

                case "tcolor":
                case "tcolors":
                case "titlecolor":
                case "titlecolors":
                case "tc":
                    if (!Economy.Settings.TColors) { Player.SendMessage(p, "%cTitlecolors are not enabled for the economy system"); return; }
                    Player.SendMessage(p, "%3===Economy info: Titlecolors===");
                    Player.SendMessage(p, "Titlecolors cost %f" + Economy.Settings.TColorPrice + " %3" + Server.moneys + " %Seach");
                    return;

                case "colors":
                case "color":
                case "colours":
                case "colour":
                    if (!Economy.Settings.Colors) { Player.SendMessage(p, "%cColors are not enabled for the economy system"); return; }
                    Player.SendMessage(p, "%3===Economy info: Colors===");
                    Player.SendMessage(p, "Colors cost %f" + Economy.Settings.ColorPrice + " %3" + Server.moneys + " %Seach");
                    return;

                case "ranks":
                case "rank":
                    if (!Economy.Settings.Ranks) { Player.SendMessage(p, "%cRanks are not enabled for the economy system"); return; }
                    Player.SendMessage(p, "%3===Economy info: Ranks===");
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
        }
        
        public override void Help(Player p) {
            Player.SendMessage(p, "%T/store <title/color/tcolor/rank/map>");
            Player.SendMessage(p, "%HViews information about buying the specific feature, such as prices.");
        }
    }
}
