/*
Copyright 2011-2014 MCGalaxy
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
using System.Text;

namespace MCGalaxy.Commands.Info {
    public class CmdSearch : Command {        
        public override string name { get { return "Search"; } }
        public override string type { get { return CommandTypes.Information; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Builder; } }

        public override void Use(Player p, string message) {
            string[] args = message.SplitSpaces(3);
            if (args.Length < 2) { Help(p); return; }
            args[0] = args[0].ToLower();
            string keyword = args[1];
            string modifier = args.Length > 2 ? args[2] : "";
            
            if (args[0] == "block" || args[0] == "blocks") {
                SearchBlocks(p, keyword, modifier);
            } else if (args[0] == "rank" || args[0] == "ranks") {
                SearchRanks(p, keyword, modifier);
            } else if (args[0] == "command" || args[0] == "commands" || args[0] == "cmd" || args[0] == "cmds") {
                SearchCommands(p, keyword, modifier);
            } else if (args[0] == "user" || args[0] == "users" || args[0] == "player" || args[0] == "players") {
                SearchPlayers(p, keyword, modifier);
            } else if (args[0] == "loaded") {
                SearchLoaded(p, keyword, modifier);
            } else if (args[0] == "level" || args[0] == "levels") {
                SearchUnloaded(p, keyword, modifier);
            } else {
                Help(p);
            }
        }
        
        static void SearchBlocks(Player p, string keyword, string modifier) {
            List<string> blocks = new List<string>();
            for (byte id = 0; id < Block.Invalid; id++) {
                string name = Block.Name(id);
                if (name.CaselessContains(keyword) && !name.CaselessEq("unknown"))
                    blocks.Add(name);
            }
            
            OutputList(p, keyword, "search blocks", "blocks", 
                       modifier, blocks, CmdBlocks.FormatBlockName);
        }
        
        static void SearchCommands(Player p, string keyword, string modifier) {
            List<Command> cmds = new List<Command>();
            foreach (Command cmd in Command.all.commands) {
                if (cmd.name.CaselessContains(keyword)) {
                    cmds.Add(cmd); continue;
                }
                
                if (String.IsNullOrEmpty(cmd.shortcut)) continue;
                if (cmd.shortcut.CaselessContains(keyword))
                    cmds.Add(cmd);
            }    
            
            OutputList(p, keyword, "search commands", "commands", 
                       modifier, cmds, (cmd) => CmdHelp.GetColor(cmd) + cmd.name);
        }
        
        static void SearchRanks(Player p, string keyword, string modifier) {
            List<string> ranks = new List<string>();
            foreach (Group g in Group.GroupList) {
                if (g.Name.CaselessContains(keyword)) {
                    ranks.Add(g.ColoredName);
                }
            }
            
            OutputList(p, keyword, "search ranks", "ranks", 
                       modifier, ranks, (name) => name);
        }
        
        static void SearchPlayers(Player p, string keyword, string modifier) {
            List<string> players = new List<string>();
            Player[] online = PlayerInfo.Online.Items;
            foreach (Player who in online) {
                if (who.name.CaselessContains(keyword) && Entities.CanSee(p, who))
                    players.Add(who.ColoredName);
            }
            
            OutputList(p, keyword, "search players", "players", 
                       modifier, players, (name) => name);
        }
        
        static void SearchLoaded(Player p, string keyword, string modifier) {
            List<string> levels = new List<string>();
            Level[] loaded = LevelInfo.Loaded.Items;
            foreach (Level level in loaded) {
                if (level.name.CaselessContains(level.name)) 
                    levels.Add(level.name);
            }
            
            OutputList(p, keyword, "search loaded", "loaded levels", 
                       modifier, levels, (level) => level);
        }
        
        static void SearchUnloaded(Player p, string keyword, string modifier) {
            List<string> maps = new List<string>();
            string[] files = LevelInfo.AllMapFiles();
            foreach (string file in files) {
                string map = Path.GetFileNameWithoutExtension(file);
                if (map.CaselessContains(keyword)) maps.Add(map);
            }

            OutputList(p, keyword, "search levels", "maps", 
                       modifier, maps, (map) => map);
        }
        
        static void OutputList<T>(Player p, string keyword, string cmd, string type, string modifier,
                                  List<T> items, StringFormatter<T> formatter) {
            if (items.Count == 0) {
                Player.Message(p, "No {0} found containing \"{1}\"", type, keyword);
            } else {
                MultiPageOutput.Output(p, items, formatter, cmd + " " + keyword, type, modifier, false);
            }
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/Search blocks [keyword] %H- finds blocks with that keyword");
            Player.Message(p, "%T/Search commands [keyword] %H- finds commands with that keyword");
            Player.Message(p, "%T/Search ranks [keyword] %H- finds ranks with that keyword");
            Player.Message(p, "%T/Search players [keyword] %H- find players with that keyword");
            Player.Message(p, "%T/Search loaded [keyword] %H- finds loaded levels with that keyword");
            Player.Message(p, "%T/Search levels [keyword] %H- find all levels with that keyword");
        }
    }
}
