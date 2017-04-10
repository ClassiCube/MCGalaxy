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
using System;
namespace MCGalaxy.Commands {
    public sealed class CmdBotRemove : Command {
        public override string name { get { return "botremove"; } }
        public override string type { get { return CommandTypes.Moderation; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Admin; } }
        public CmdBotRemove() { }

        public override void Use(Player p, string message) {
            if (message == "") { Help(p); return; }
            if (Player.IsSuper(p)) { MessageInGameOnly(p); return; }            
            if (!p.level.BuildAccess.CheckDetailed(p)) {
                Player.Message(p, "Hence, you cannot change remove bots from this map.");
                return;
            }
            
            if (message.CaselessEq("all")) {
                PlayerBot.RemoveAllFromLevel(p.level); return;
            } else {
                PlayerBot bot = Matcher.FindBotsInLevel(p, message);
                if (bot == null) return;
                
                PlayerBot.Remove(bot);
                Player.Message(p, "Removed bot {0}%S", bot.ColoredName);
            }
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/botremove [name]");
            Player.Message(p, "%HRemove a bot on the same level as you");
            Player.Message(p, "%HIf [name] is \"all\", all bots on your map are removed");
        }
    }
}
