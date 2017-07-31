/*
    Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCGalaxy)
    
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
using System.Collections.Generic;

namespace MCGalaxy.Commands.Bots {
    public sealed class CmdBots : Command {
        public override string name { get { return "Bots"; } }
        public override string shortcut { get { return "BotList"; } }
        public override string type { get { return CommandTypes.Other; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Builder; } }

        public override void Use(Player p, string message) { 
            Level lvl = p == null ? null : p.level;
            string[] args = message.SplitSpaces(2);
            int ignored, offset = 0;
            
            if (args.Length == 2 || !(message.Length == 0 || args[0].CaselessEq("all") || int.TryParse(args[0], out ignored))) {
                lvl = Matcher.FindLevels(p, args[0]);
                offset = 1;
                if (lvl == null) return;
            }
            
            PlayerBot[] bots = PlayerBot.Bots.Items;
            List<PlayerBot> inScope = new List<PlayerBot>();
            foreach (PlayerBot bot in bots) {
                if (lvl != null && bot.level != lvl) continue;
                inScope.Add(bot);
            }
            
            string cmd = (lvl == null || lvl == p.level) ? "bots" : "bots " + lvl.name;
            string modifier = args.Length > offset ? args[offset] : "";
            
            string group = lvl == null ? "All bots:" : "Bots in " + lvl.ColoredName + ":";
            Player.Message(p, group);
            MultiPageOutput.Output(p, inScope, FormatBot, cmd, "Bots", modifier, false);
        }
        
        static string FormatBot(PlayerBot bot) {
            string desc = bot.DisplayName;
            if (bot.DisplayName != bot.name) desc += "%S(" + bot.name + ")";
            
            if (bot.AIName != "") desc += "[" + bot.AIName + "]";
            else if (bot.hunt) desc += "[Hunt]";
            if (bot.kill) desc += "-kill";
            return desc;
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/Bots");
            Player.Message(p, "%HShows a list of bots on your level, and their AIs and levels");
            Player.Message(p, "%T/Bots [level]");
            Player.Message(p, "%HShows bots on the given level");
        }
    }
}
