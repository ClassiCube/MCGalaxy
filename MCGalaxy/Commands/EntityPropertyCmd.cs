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
        
        protected void UseBotOrOnline(Player p, CommandData data, string message, string type) {
            if (message.CaselessStarts("bot ")) {
                UseBot(p,    data, message, type);
            } else {
                UseOnline(p, data, message, type);
            }
        }
        
        protected void UseBotOrPlayer(Player p, CommandData data, string message, string type) {
            if (message.CaselessStarts("bot ")) {
                UseBot(p,    data, message, type);
            } else {
                UsePlayer(p, data, message, type);
            }
        }
		
        void UseBot(Player p, CommandData data, string message, string type) {
            string[] args = message.SplitSpaces(3);
            PlayerBot bot = Matcher.FindBots(p, args[1]);
            
            if (bot == null) return;
            if (!CheckExtraPerm(p, data, 2)) return;
            
            if (!LevelInfo.Check(p, data.Rank, p.level, "change the " + type + " of that bot")) return;
            if (!bot.EditableBy(p, "change the " + type + " of")) { return; }
            SetBotData(p, bot, args.Length > 2 ? args[2] : "");
        }
        
        protected void UseOnline(Player p, CommandData data, string message, string type) {
            if (message.Length == 0) { Help(p); return; }
            string[] args = message.SplitSpaces(2);
            string name   = CheckOwn(p, args[0], "player name");
            if (name == null) return;
            
            Player who = PlayerInfo.FindMatches(p, name);
            if (who == null) return;
            
            if (p != who && !CheckExtraPerm(p, data, 1)) return;
            if (!CheckRank(p, data, who, "change the " + type + " of", true)) return;
            SetOnlineData(p, who, args.Length > 1 ? args[1] : "");
        }
        
        protected void UsePlayer(Player p, CommandData data, string message, string type) {
            if (message.Length == 0) { Help(p); return; }
            string[] args = message.SplitSpaces(2);
            string target = CheckOwn(p, args[0], "player name");
            if (target == null) return;
            
            target = PlayerInfo.FindMatchesPreferOnline(p, target);
            if (target == null) return;            
            if (p.name != target && !CheckExtraPerm(p, data, 1)) return;
            
            LevelPermission rank = Group.GroupIn(target).Permission;
            if (!CheckRank(p, data, target, rank, "change the " + type + " of", true)) return;
            SetPlayerData(p, target, args.Length > 1 ? args[1] : "");
        }

        protected virtual void SetBotData(Player p, PlayerBot bot,    string args) { }      
        protected virtual void SetOnlineData(Player p, Player who,    string args) { }       
        protected virtual void SetPlayerData(Player p, string target, string args) { }
        
        protected void MessageFrom(string target, Player who, string message) {
            if (who == null) {
                string nick = Player.Console.FormatNick(target);
                Chat.MessageGlobal(nick + " &S" + message);
            } else {
                Chat.MessageFrom(who, "λNICK &S" + message);
            }
        }
    }
}
