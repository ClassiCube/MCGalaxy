/*
    Copyright 2015 MCGalaxy
    
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
using MCGalaxy.Games;

namespace MCGalaxy.Commands.Info 
{
    public sealed class CmdWhere : Command2 
    {
        public override string name { get { return "Where"; } }
        public override string type { get { return CommandTypes.Information; } }
        
        public override void Use(Player p, string message, CommandData data) {
            Entity target;
            string targetName;
            if (message.CaselessStarts("bot ")) {
                string botName = message.SplitSpaces(2)[1];
                target = Matcher.FindBots(p, botName);
                if (target == null) return;
                targetName = "Bot " + ((PlayerBot)target).DisplayName;
            } else {
                if (message.Length == 0) message = p.name;
                target = PlayerInfo.FindMatches(p, message);
                if (target == null) return;
                if (IGame.GameOn(target.Level) != null && !(p.IsSuper || p.Game.Referee)) {
                    p.Message("You can only use /where on people in games when you are in referee mode."); return;
                }
                targetName = p.FormatNick((Player)target);
            }

            target.DisplayPosition(p, p.FormatNick(targetName));
        }

        public override void Help(Player p) {
            p.Message("&T/Where [name]");
            p.Message("&HDisplays level, position, and orientation of that player.");
            p.Message("&T/Where bot [name]");
            p.Message("&HDisplays level, position, and orientation of that bot.");
        }
    }
}
