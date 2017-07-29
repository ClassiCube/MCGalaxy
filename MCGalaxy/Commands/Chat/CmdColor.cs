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

namespace MCGalaxy.Commands.Chatting {    
    public class CmdColor : EntityPropertyCmd {
        public override string name { get { return "color"; } }
        public override string type { get { return CommandTypes.Chat; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        public override CommandPerm[] ExtraPerms {
            get { return new[] { new CommandPerm(LevelPermission.Operator, "+ can change the color of others"),
                    new CommandPerm(LevelPermission.Operator, "+ can change the color of bots") }; }
        }
        public override CommandAlias[] Aliases {
            get { return new[] { new CommandAlias("colour"), new CommandAlias("xcolor", "-own") }; }
        }        
        public override void Use(Player p, string message) { UseBotOrPlayer(p, message, "color"); }

        protected override void SetBotData(Player p, PlayerBot bot, string colName) {
            if (p != null && !bot.level.BuildAccess.CheckDetailed(p)) {
                Player.Message(p, "Hence, you cannot change the color of that bot.");
                return;
            }
            
            string color = colName == "" ? "&1" : Colors.Parse(colName);
            if (color == "") { Player.Message(p, "There is no color \"" + colName + "\"."); return; }
            Chat.MessageLevel(bot.level, "Bot " + bot.ColoredName + "'s %Scolor was set to " 
                              + color + Colors.Name(color));
            bot.color = color;
            
            bot.GlobalDespawn();
            bot.GlobalSpawn();
            BotsFile.UpdateBot(bot);
        }
        
        protected override void SetPlayerData(Player p, Player who, string colName) {
            string color = "";
            if (colName == "") {
                Chat.MessageGlobal(who, who.ColoredName + " %Shad their color removed.", false);
                who.color = who.group.Color;
            } else {
                color = Colors.Parse(colName);
                if (color == "") { Player.Message(p, "There is no color \"" + colName + "\"."); return; }
                else if (color == who.color) { Player.Message(p, who.DisplayName + " %Salready has that color."); return; }
                
                Chat.MessageGlobal(who, who.ColoredName + " %Shad their color changed to " + color + Colors.Name(color) + "%S.", false);
                who.color = color;
            }
            
            Entities.GlobalRespawn(who);
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
