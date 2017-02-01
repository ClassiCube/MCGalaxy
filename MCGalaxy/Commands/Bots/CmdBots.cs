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

namespace MCGalaxy.Commands {
    public sealed class CmdBots : Command {
        public override string name { get { return "bots"; } }
        public override string shortcut { get { return "botlist"; } }
        public override string type { get { return CommandTypes.Other; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Builder; } }
        public CmdBots() { }

        public override void Use(Player p, string message) {
            PlayerBot[] bots = PlayerBot.Bots.Items;         
            Level lvl = null;
            string[] args = message.SplitSpaces(2);
            
            if (!(message == "" || args[0].CaselessEq("all"))) {
                lvl = LevelInfo.FindMatches(p, args[0]);
                if (lvl == null) return;
            }
            
            List<PlayerBot> inScope = new List<PlayerBot>();
            foreach (PlayerBot bot in bots) {
                if (lvl != null && bot.level != lvl) continue;
                inScope.Add(bot);
            }
            
            string cmd = lvl == null ? "bots all" : "bots " + lvl.name;
            string modifier = args.Length > 1 ? args[1] : "";
            MultiPageOutput.Output(p, inScope, FormatBot, cmd, "bots", modifier, false);
        }
        
        static string FormatBot(PlayerBot bot, int index) {
            string desc = bot.name + "(" + bot.level.name + ")";
            
            if (bot.AIName != "") desc += "[" + bot.AIName + "]";
            else if (bot.hunt) desc += "[Hunt]";
            if (bot.kill) desc += "-kill";
            return desc;
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/bots %H- Shows a list of bots, their AIs and levels");
            Player.Message(p, "%T/bots [level] %H- Only shows bots on the given level");
        }
    }
}
