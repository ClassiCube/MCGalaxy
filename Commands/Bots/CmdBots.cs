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
using System.Text;

namespace MCGalaxy.Commands {
    public sealed class CmdBots : Command {
        public override string name { get { return "bots"; } }
        public override string shortcut { get { return "botlist"; } }
        public override string type { get { return CommandTypes.Other; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Builder; } }
        public CmdBots() { }

        public override void Use(Player p, string message) {
            StringBuilder text = new StringBuilder();
            PlayerBot[] bots = PlayerBot.Bots.Items;
            
            foreach (PlayerBot bot in bots) {
                text.Append(", ").Append(bot.name)
                	.Append("(").Append(bot.level.name).Append(")");
                                                              
                if (bot.AIName != "") text.Append("[").Append(bot.AIName).Append("]");
                else if (bot.hunt) text.Append("[Hunt]");
                if (bot.kill) text.Append("-kill");
            }

            if (text.Length > 0) 
                Player.Message(p, "&1Bots: %S" + text.ToString(2, text.Length - 2));
            else 
                Player.Message(p, "No bots are alive.");
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/bots %H- Shows a list of bots, their AIs and levels");
        }
    }
}
