/*
Copyright 2011-2014 MCGalaxy
Dual-licensed under the Educational Community License, Version 2.0 and
the GNU General Public License, Version 3 (the "Licenses"); you may
not use this file except in compliance with the Licenses. You may
obtain a copy of the Licenses at
https://opensource.org/license/ecl-2-0/
https://www.gnu.org/licenses/gpl-3.0.html
Unless required by applicable law or agreed to in writing,
software distributed under the Licenses are distributed on an "AS IS"
BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express
or implied. See the Licenses for the specific language governing
permissions and limitations under the Licenses.
 */
using System;
using System.Collections.Generic;
using MCGalaxy.Blocks;
using MCGalaxy.SQL;
using BlockID = System.UInt16;

namespace MCGalaxy.Commands.Info 
{
    public class CmdSearch : Command2 
    {
        public override string name { get { return "Search"; } }
        public override string type { get { return CommandTypes.Information; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Builder; } }
        public override bool UseableWhenFrozen { get { return true; } }
        
        public override void Use(Player p, string message, CommandData data) {
            string[] args = message.SplitSpaces(3);
            if (args.Length < 2) { Help(p); return; }
            
            string list     = args[0].ToLower();
            string keyword  = args[1];
            string modifier = args.Length > 2 ? args[2] : "";
            
            if (list == "block" || list == "blocks") {
                SearchBlocks(p, keyword, modifier);
            } else if (list == "rank" || list == "ranks") {
                SearchRanks(p, keyword, modifier);
            } else if (list == "command" || list == "commands") {
                SearchCommands(p, keyword, modifier);
            } else if (list == "player" || list == "players") {
                SearchPlayers(p, keyword, modifier);
            } else if (list == "online") {
                SearchOnline(p, keyword, modifier);
            }  else if (list == "loaded") {
                SearchLoaded(p, keyword, modifier);
            } else if (list == "level" || list == "levels" || list == "maps") {
                SearchMaps(p, keyword, modifier);
            } else {
                Help(p);
            }
        }
        
        
        static void SearchBlocks(Player p, string keyword, string modifier) {
            List<BlockID> blocks = new List<BlockID>();
            for (int b = 0; b < Block.SUPPORTED_COUNT; b++) 
            {
                BlockID block = (BlockID)b;
                if (Block.ExistsFor(p, block)) blocks.Add(block);
            }

            List<string> blockNames = Wildcard.Filter(blocks, keyword, 
                                                      b => Block.GetName(p, b), null,
                                                      b => Block.GetColoredName(p, b));
            OutputList(p, keyword, "search blocks", "blocks", modifier, blockNames);
        }
        
        static void SearchCommands(Player p, string keyword, string modifier) {
            List<string> commands  = Wildcard.Filter(Command.allCmds, keyword, cmd => cmd.name,
                                                     null, Command.GetColoredName);
            List<string> shortcuts = Wildcard.Filter(Command.allCmds, keyword, cmd => cmd.shortcut,
                                                     cmd => !String.IsNullOrEmpty(cmd.shortcut), 
                                                     Command.GetColoredName);
            
            // Match both names and shortcuts
            foreach (string shortcutCmd in shortcuts) 
            {
                if (commands.CaselessContains(shortcutCmd)) continue;
                commands.Add(shortcutCmd);
            }
            
            OutputList(p, keyword, "search commands", "commands", modifier, commands);
        }
        
        static void SearchRanks(Player p, string keyword, string modifier) {
            List<string> ranks = Wildcard.Filter(Group.GroupList, keyword, grp => grp.Name,
                                                null, grp => grp.ColoredName);
            OutputList(p, keyword, "search ranks", "ranks", modifier, ranks);
        }
        
        static void SearchOnline(Player p, string keyword, string modifier) {
            Player[] online = PlayerInfo.Online.Items;
            List<string> players = Wildcard.Filter(online, keyword, pl => pl.name,
                                                  pl => p.CanSee(pl), pl => pl.ColoredName);
            OutputList(p, keyword, "search online", "players", modifier, players);
        }
        
        static void SearchLoaded(Player p, string keyword, string modifier) {
            Level[] loaded = LevelInfo.Loaded.Items;
            List<string> levels = Wildcard.Filter(loaded, keyword, level => level.name);
            OutputList(p, keyword, "search loaded", "loaded levels", modifier, levels);
        }
        
        static void SearchMaps(Player p, string keyword, string modifier) {
            string[] allMaps = LevelInfo.AllMapNames();
            List<string> maps = Wildcard.Filter(allMaps, keyword, map => map);
            maps.Sort(new AlphanumComparator());
            OutputList(p, keyword, "search levels", "maps", modifier, maps);
        }
        
        static void OutputList(Player p, string keyword, string cmd, string type, string modifier, List<string> items) {
            if (items.Count == 0) {
                p.Message("No {0} found containing \"{1}\"", type, keyword);
            } else {
                Paginator.Output(p, items, item => item, cmd + " " + keyword, type, modifier);
            }
        }
        
        
        static void SearchPlayers(Player p, string keyword, string modifier) {
            List<string> names = new List<string>();
            string suffix = Database.Backend.CaselessLikeSuffix;
            
            // TODO supporting more than 100 matches somehow
            Database.ReadRows("Players", "Name", r => names.Add(r.GetText(0)),
                              "WHERE Name LIKE @0 ESCAPE '#' LIMIT 100" + suffix,
                              Wildcard.ToSQLFilter(keyword));
            
            OutputList(p, keyword, "search players", "players", modifier, names);
        }
        
        
        public override void Help(Player p) {
            p.Message("&T/Search [list] [keyword]");
            p.Message("&HFinds entries in a list that match the given keyword");
            p.Message("&H  keyword can also include wildcard characters:");
            p.Message("&H    * - placeholder for zero or more characters");
            p.Message("&H    ? - placeholder for exactly one character");
            p.Message("&HLists: &fblocks/commands/ranks/players/online/loaded/maps");
        }
    }
}
