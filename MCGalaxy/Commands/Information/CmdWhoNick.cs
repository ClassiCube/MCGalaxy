/*
    Copyright 2011 MCForge
        
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
using System.Data;
using MCGalaxy.SQL;

namespace MCGalaxy.Commands.Info {
    public sealed class CmdWhoNick : Command2 {
        public override string name { get { return "WhoNick"; } }
        public override string shortcut { get { return "RealName"; } }
        public override string type { get { return CommandTypes.Information; } }
        public override bool UseableWhenFrozen { get { return true; } }
        
        public override void Use(Player p, string message, CommandData data) {
            if (message.Length == 0) { Help(p); return; }
            string[] args = message.SplitSpaces(2);
            if (args.Length > 1 && args[0].CaselessEq("bot")) {
                ForBot(p, args[1]);
                return;
            }
            ForPlayer(p, message);
        }
        
        static void ForPlayer(Player p, string message) {
            Player nick = FindNick(p, message);
            if (nick == null) return;
            p.Message("The player nicknamed {0} &Sis named {1}", nick.DisplayName, nick.name);
        }
        static Player FindNick(Player p, string nick) {
            nick = Colors.Strip(nick);
            Player[] players = PlayerInfo.Online.Items;
            int matches;
            return Matcher.Find(p, nick, out matches, players, pl => p.CanSee(pl),
                                pl => Colors.Strip(pl.DisplayName), "online player nicks");
        }
        
        static void ForBot(Player p, string message) {
            PlayerBot bot = FindBotNick(p, message);
            if (bot == null) return;
            p.Message("The bot nicknamed {0} &Sis named {1}", bot.DisplayName, bot.name);
        }
        static PlayerBot FindBotNick(Player p, string nick) {
            nick = Colors.Strip(nick);
            PlayerBot[] bots = p.level.Bots.Items;
            int matches;
            return Matcher.Find(p, nick, out matches, bots, pl => true,
                                pl => Colors.Strip(pl.DisplayName), "bot nicknames");
        }
        
        public override void Help(Player p) {
            p.Message("&T/WhoNick [nickname]");
            p.Message("&HDisplays the player's real username");
            p.Message("&T/WhoNick bot [nickname]");
            p.Message("&HDisplays the bot's real name");
        }
    }
}
