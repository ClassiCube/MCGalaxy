/*
    Copyright 2015 MCGalaxy
    
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

namespace MCGalaxy.Commands {
    public abstract class EntityPropertyCmd : Command2 {
        
        protected void UseBotOrPlayer(Player p, CommandData data, string message, string type) {
            if (message.Length == 0) { Help(p); return; }
            bool isBot = message.CaselessStarts("bot ");
            string[] args = message.SplitSpaces(isBot ? 3 : 2);
            if (!CheckOwn(p, args, "player or bot name")) return;
            
            Player who = null;
            PlayerBot bot = null;
            if (isBot) bot = Matcher.FindBots(p, args[1]);
            else who = PlayerInfo.FindMatches(p, args[0]);
            if (bot == null && who == null) return;

            if (isBot) {
                if (!CheckExtraPerm(p, data, 2)) return;
                
                if (!LevelInfo.Check(p, data.Rank, p.level, "change the " + type + " of that bot")) return;
                if (!bot.EditableBy(p, "change the " + type + " of")) { return; }
                SetBotData(p, bot, args.Length > 2 ? args[2] : "");
            } else {
                if (p != who && !CheckExtraPerm(p, data, 1)) return;
                
                if (!CheckRank(p, data, who, "change the " + type + " of", true)) return;
                SetPlayerData(p, who, args.Length > 1 ? args[1] : "");
            }
        }
        
        protected void UsePlayer(Player p, CommandData data, string message, string type) {
            if (message.Length == 0) { Help(p); return; }
            string[] args = message.SplitSpaces(2);
            if (!CheckOwn(p, args, "player name")) return;
            
            Player who = PlayerInfo.FindMatches(p, args[0]);
            if (who == null) return;
            
            if (!CheckRank(p, data, who, "change the " + type + " of", true)) return;
            if (p != who && !CheckExtraPerm(p, data, 1)) return;
            SetPlayerData(p, who, args.Length > 1 ? args[1] : "");
        }
        
        bool CheckOwn(Player p, string[] args, string type) {
            if (args[0].CaselessEq("-own")) {
                if (p.IsSuper) { SuperRequiresArgs(p, type); return false; }
                args[0] = p.name;
            }
            return true;
        }

        protected virtual void SetBotData(Player p, PlayerBot bot, string args) { }
        
        protected virtual void SetPlayerData(Player p, Player who, string args) { }
    }
}
