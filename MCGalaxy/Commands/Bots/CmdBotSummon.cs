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
using MCGalaxy.Bots;

namespace MCGalaxy.Commands.Bots {
    public sealed class CmdBotSummon : Command {
        public override string name { get { return "BotSummon"; } }
        public override string type { get { return CommandTypes.Moderation; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Admin; } }
        public override bool SuperUseable { get { return false; } }

        public override void Use(Player p, string message) {
            if (message.Length == 0) { Help(p); return; }
            if (!p.level.BuildAccess.CheckDetailed(p)) {
                Player.Message(p, "Hence, you cannot summon bots on this map.");
                return;
            }
            
            PlayerBot bot = Matcher.FindBotsInLevel(p, message);
            if (bot == null) return;
            
            bot.Pos = p.Pos; bot.SetYawPitch(p.Rot.RotY, p.Rot.HeadX);
            BotsFile.UpdateBot(bot);
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/BotSummon [name]");
            Player.Message(p, "%HSummons a bot to your position.");
        }
    }
}
