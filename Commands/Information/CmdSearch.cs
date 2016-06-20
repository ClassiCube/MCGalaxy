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
        const StringComparison comp = StringComparison.OrdinalIgnoreCase;

        public override void Use(Player p, string message) {
            string[] args = message.SplitSpaces(2);
            if (args.Length < 2) { Help(p); return; }
            args[0] = args[0].ToLower();
            string keyword = args[1];
            
            if (args[0] == "block" || args[0] == "blocks") {
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
            if (blocks.Length == 0) { Player.Message(p, "No blocks found containing &b" + keyword); return; }
            Player.Message(p, blocks.ToString(4, blocks.Length - 4));
        }
        
        static void SearchRanks(Player p, string keyword) {
            StringBuilder ranks = new StringBuilder();
            foreach (Group g in Group.GroupList) {
                if (g.name.IndexOf(keyword, comp) >= 0)
                    ranks.Append(", ").Append(g.color).Append(g.name);
            }
            if (ranks.Length == 0) { Player.Message(p, "No ranks found containing &b" + keyword); return; }
            Player.Message(p, ranks.ToString(2, ranks.Length - 2));
        }
        
        static void SearchPlayers(Player p, string keyword) {
            StringBuilder players = new StringBuilder();
            Player[] online = PlayerInfo.Online.Items;
            foreach (Player who in online) {
                if (who.name.IndexOf(keyword, comp) >= 0 && Entities.CanSee(p, who))
                    players.Append(", ").Append(who.color).Append(who.name);
            }
            if (players.Length == 0) { Player.Message(p, "No usernames found containing &b" + keyword); return; }
            Player.Message(p, players.ToString(2, players.Length - 2));
        }
        
        static void SearchLoaded(Player p, string keyword) {
            StringBuilder levels = new StringBuilder();
            Level[] loaded = LevelInfo.Loaded.Items;
            foreach (Level level in loaded) {
                if (level.name.IndexOf(keyword, comp) >= 0)
                    levels.Append(", ").Append(level.name);
            }
            if (levels.Length == 0) { Player.Message(p, "No loaded levels found containing &b" + keyword); return; }
            Player.Message(p, levels.ToString(2, levels.Length - 2));
        }
        
        static void SearchUnloaded(Player p, string keyword) {
            List<string> matches = new List<string>();
            string[] files = Directory.GetFiles("levels", "*.lvl");
            foreach (string file in files) {
                string level = Path.GetFileNameWithoutExtension(file);
                if (level.IndexOf(keyword, comp) >= 0)
                matches.Add(level);
            }
            
            if (matches.Count == 0) 
            Player.Message(p, "No levels found containing &b" + keyword);
            else
            Player.Message(p, matches.Concatenate(", "));
        }
        
        public override void Help(Player p) {
            Player.Message(p, "&b/search &2blocks &a<keyword> &e- finds blocks with that keyword");
            Player.Message(p, "&b/search &2ranks &a<keyword> &e- finds blocks with that keyword");
            Player.Message(p, "&b/search &2players &a<keyword> &e- find players with that keyword");
            Player.Message(p, "&b/search &2loaded &a<keyword> &e- finds loaded levels with that keyword");
            Player.Message(p, "&b/search &2levels &a<keyword> &e- find all levels with that keyword");
        }
    }
}
