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

namespace MCGalaxy.Commands {
    
    public class CmdSearch : Command {
        
        public override string name { get { return "search"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.Information; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Builder; } }
        static char[] trimChars = { ' ' };
        const StringComparison comp = StringComparison.OrdinalIgnoreCase;

        public override void Use(Player p, string message) {
            string[] args = message.Split(trimChars, 2);
            if (args.Length < 2) { Help(p); return; }
            args[0] = args[0].ToLower();
            string keyword = args[1];
            
            if (args[0] == "cmd" || args[0] == "cmds" || args[0] == "command" || args[0] == "commands") {
                SearchCommands(p, keyword);
            } else if (args[0] == "block" || args[0] == "blocks") {
                SearchBlocks(p, keyword);
            } else if (args[0] == "rank" || args[0] == "ranks") {
                SearchRanks(p, keyword);
            } else if (args[0] == "user" || args[0] == "users" || args[0] == "player" || args[0] == "players") {
                SearchPlayers(p, keyword);
            } else if (args[0] == "loaded") {
                SearchLoaded(p, keyword);
            } else if (args[0] == "level" || args[0] == "levels") {
                SearchUnloaded(p, keyword);
            } else {
                Help(p);
            }
        }
        
        static void SearchCommands(Player p, string keyword) {
            if (keyword.Length <= 2) {
                Player.SendMessage(p, "You need to enter at least three characters to search for."); return;
            }
            string[] keywords = keyword.Split(' ');
            string[] found = keywords.Length == 1 ?
                CommandKeywords.Find(keyword) : CommandKeywords.Find(keywords);
            if (found == null) {
                Player.SendMessage(p, "No commands found matching keyword(s): '" + keyword + "'"); return;
            }
            
            StringBuilder cmds = new StringBuilder();
            bool mode = true;
            Player.SendMessage(p, "&bCommands found: ");
            foreach (string cmd in found) {
                cmds.Append(mode ? "%S, &9" : "%S, &2").Append(cmd);
                mode = !mode;
            }
            Player.SendMessage(p, cmds.ToString(4, cmds.Length - 4));
        }
        
        static void SearchBlocks(Player p, string keyword) {
            StringBuilder blocks = new StringBuilder();
            bool mode = true;
            for (byte id = 0; id < 255; id++) {
                string name = Block.Name(id);
                if (name.ToLower() != "unknown" && name.Contains(keyword)) {
                    blocks.Append(mode ? "%S, &9" : "%S, &2").Append(name);
                    mode = !mode;
                }
            }
            if (blocks.Length == 0) { Player.SendMessage(p, "No blocks found containing &b" + keyword); return; }
            Player.SendMessage(p, blocks.ToString(4, blocks.Length - 4));
        }
        
        static void SearchRanks(Player p, string keyword) {
            StringBuilder ranks = new StringBuilder();
            foreach (Group g in Group.GroupList) {
                if (g.name.IndexOf(keyword, comp) >= 0)
                    ranks.Append(", ").Append(g.color).Append(g.name);
            }
            if (ranks.Length == 0) { Player.SendMessage(p, "No ranks found containing &b" + keyword); return; }
            Player.SendMessage(p, ranks.ToString(2, ranks.Length - 2));
        }
        
        static void SearchPlayers(Player p, string keyword) {
            StringBuilder players = new StringBuilder();
            Player[] online = PlayerInfo.Online;
            foreach (Player who in online) {
                if (who.name.IndexOf(keyword, comp) >= 0 && Player.CanSee(p, who))
                    players.Append(", ").Append(who.color).Append(who.name);
            }
            if (players.Length == 0) { Player.SendMessage(p, "No usernames found containing &b" + keyword); return; }
            Player.SendMessage(p, players.ToString(2, players.Length - 2));
        }
        
        static void SearchLoaded(Player p, string keyword) {
            StringBuilder levels = new StringBuilder();
            foreach (Level level in Server.levels) {
                if (level.name.IndexOf(keyword, comp) >= 0)
                    levels.Append(", ").Append(level.name);
            }
            if (levels.Length == 0) { Player.SendMessage(p, "No loaded levels found containing &b" + keyword); return; }
            Player.SendMessage(p, levels.ToString(2, levels.Length - 2));
        }
        
        static void SearchUnloaded(Player p, string keyword) {
            StringBuilder searched = new StringBuilder();
            DirectoryInfo di = new DirectoryInfo("levels/");
            FileInfo[] fi = di.GetFiles("*.lvl");

            foreach (FileInfo file in fi) {
                string level = file.Name.Replace(".lvl", "");
                if (level.IndexOf(keyword, comp) >= 0)
                    searched.Append(", ").Append(level);
            }

            if (searched.Length == 0) { Player.SendMessage(p, "No levels found containing &b" + keyword); return; }
            Player.SendMessage(p, searched.ToString(2, searched.Length - 2));
        }
        
        public override void Help(Player p) {
            Player.SendMessage(p, "&b/search &2commands &a<keywords[more]> &e- finds commands with those keywords");
            Player.SendMessage(p, "&b/search &2blocks &a<keyword> &e- finds blocks with that keyword");
            Player.SendMessage(p, "&b/search &2ranks &a<keyword> &e- finds blocks with that keyword");
            Player.SendMessage(p, "&b/search &2players &a<keyword> &e- find players with that keyword");
            Player.SendMessage(p, "&b/search &2loaded &a<keyword> &e- finds loaded levels with that keyword");
            Player.SendMessage(p, "&b/search &2levels &a<keyword> &e- find all levels with that keyword");
        }
    }
}
