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
    public class CmdColor : EntityPropertyCmd {
        public override string name { get { return "color"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.Chat; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        public override CommandPerm[] ExtraPerms {
            get { return new[] { new CommandPerm(LevelPermission.Operator, "+ can change the color of others") }; }
        }
        public override CommandAlias[] Aliases {
            get { return new[] { new CommandAlias("colour"), new CommandAlias("xcolor", "-own") }; }
        }        
        public override void Use(Player p, string message) { UseBotOrPlayer(p, message, "color"); }

        protected override void SetBotData(Player p, PlayerBot bot, string[] args) {
            string color = args.Length > 2 ? Colors.Parse(args[2]) : "&1";
            if (color == "") { Player.Message(p, "There is no color \"" + args[2] + "\"."); return; }
            Chat.MessageLevel(bot.level, "Bot " + bot.ColoredName + "'s %Scolor was set to " 
                              + color + Colors.Name(color));            
            bot.color = color;
            
            bot.GlobalDespawn();
            bot.GlobalSpawn();
            BotsFile.UpdateBot(bot);
        }
        
        protected override void SetPlayerData(Player p, Player who, string[] args) {
            string color = "";
            if (args.Length == 1) {
                Player.SendChatFrom(who, who.ColoredName + " %Shad their color removed.", false);
                who.color = who.group.color;
            } else {
                color = Colors.Parse(args[1]);
                if (color == "") { Player.Message(p, "There is no color \"" + args[1] + "\"."); return; }
                else if (color == who.color) { Player.Message(p, who.DisplayName + " %Salready has that color."); return; }
                Player.SendChatFrom(who, who.ColoredName + " %Shad their color changed to " + color + Colors.Name(color) + "%S.", false);
                who.color = color;
            }
            
            Entities.GlobalDespawn(who, true);
            Entities.GlobalSpawn(who, true);
            who.SetPrefix();
            Database.Backend.UpdateRows("Players", "color = @1", "WHERE Name = @0", who.name, color);
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/color [player] [color]");
            Player.Message(p, "%HSets the nick color of that player");
            Player.Message(p, "%H  If [color] is not given, reverts to player's rank color.");
            Player.Message(p, "%H/color bot [bot] [color]");
            Player.Message(p, "%TSets the name color of that bot.");
            Player.Message(p, "%HTo see a list of all colors, use /help colors.");
        }
    }
}
