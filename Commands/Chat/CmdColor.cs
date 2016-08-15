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
using MCGalaxy.SQL;

namespace MCGalaxy.Commands {    
    public class CmdColor : Command {        
        public override string name { get { return "color"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.Chat; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        public override CommandPerm[] ExtraPerms {
            get { return new[] { new CommandPerm(LevelPermission.Operator, "+ can change the color of others") }; }
        }
        public override CommandAlias[] Aliases {
            get { return new[] { new CommandAlias("colour"), new CommandAlias("xcolor", "-own") }; }
        }
        
        public override void Use(Player p, string message) {
            if (message == "") { Help(p); return; }
            string[] args = message.Split(' ');
            if (args[0].CaselessEq("-own")) {
                if (Player.IsSuper(p)) { SuperRequiresArgs(p, "player name"); return; }
                args[0] = p.name;
            }
            
            Player who = null;
            PlayerBot bot = null;
            bool isBot = message.CaselessStarts("bot ");
            if (isBot) bot = PlayerBot.FindMatchesPreferLevel(p, args[1]);
            else who = PlayerInfo.FindMatches(p, args[0]);
            if (bot == null && who == null) return;
            
            if (p != null && who != null && who.Rank > p.Rank) {
                MessageTooHighRank(p, "change the color of", true); return;
            }
            if ((isBot || who != p) && !CheckExtraPerm(p)) { MessageNeedExtra(p, "change the color of others."); return; }
            if (isBot) SetBotColor(p, bot, args);
            else SetColor(p, who, args);
        }

        static void SetBotColor(Player p, PlayerBot bot, string[] args) {
            string color = args.Length == 2 ? "&1" : Colors.Parse(args[2]);
            if (color == "") { Player.Message(p, "There is no color \"" + args[2] + "\"."); return; }
            Chat.MessageAll("Bot {0}'s %Scolor was changed to {1}{2}", bot.DisplayName, color, Colors.Name(color));
             
            bot.color = color;
            bot.GlobalDespawn();
            bot.GlobalSpawn();
            BotsFile.UpdateBot(bot);
        }
        
        static void SetColor(Player p, Player who, string[] args) {
            if (args.Length == 1) {
                Player.SendChatFrom(who, who.ColoredName + " %Shad their color removed.", false);
                who.color = who.group.color;
                Database.Execute("UPDATE Players SET color = '' WHERE name = @0", who.name);
            } else {
                string color = Colors.Parse(args[1]);
                if (color == "") { Player.Message(p, "There is no color \"" + args[1] + "\"."); return; }
                else if (color == who.color) { Player.Message(p, who.DisplayName + " already has that color."); return; }
                Player.SendChatFrom(who, who.ColoredName + " %Shad their color changed to " + color + Colors.Name(color) + "%S.", false);
                who.color = color;
                Database.Execute("UPDATE Players SET color = @1 WHERE name = @0", who.name, color);
            }
            Entities.GlobalDespawn(who, true);
            Entities.GlobalSpawn(who, true);
            who.SetPrefix();
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/color [player] [color]");
            Player.Message(p, "%HSets the nick color of that player");
            Player.Message(p, "%HIf no [color] is given, reverts to player's rank color.");
            Player.Message(p, "%H/color bot [bot] [color]");
            Player.Message(p, "%TSets the name color of that bot.");
            Player.Message(p, "%HTo see a list of all colors, use /help colors.");
        }
    }
}
