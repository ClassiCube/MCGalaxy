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
using MCGalaxy.DB;

namespace MCGalaxy.Commands.Chatting {    
    public class CmdColor : EntityPropertyCmd {
        public override string name { get { return "Color"; } }
        public override string type { get { return CommandTypes.Chat; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        public override CommandPerm[] ExtraPerms {
            get { return new[] { new CommandPerm(LevelPermission.Operator, "can change the color of others"),
                    new CommandPerm(LevelPermission.Operator, "can change the color of bots") }; }
        }
        public override CommandAlias[] Aliases {
            get { return new[] { new CommandAlias("Colour"), new CommandAlias("XColor", "-own") }; }
        }        
        public override void Use(Player p, string message, CommandData data) { 
            UseBotOrPlayer(p, data, message, "color"); 
        }

        protected override void SetBotData(Player p, PlayerBot bot, string colName) {
            string color = colName.Length == 0 ? "&1" : Matcher.FindColor(p, colName);
            if (color == null) return;
            
            p.Message("You changed the color of bot " + bot.ColoredName + 
                      " %Sto " + color + Colors.Name(color));
            bot.color = color;
            
            bot.GlobalDespawn();
            bot.GlobalSpawn();
            BotsFile.Save(p.level);
        }
        
        protected override void SetPlayerData(Player p, Player target, string colName) {
            string col = "";
            if (colName.Length == 0) {
                Chat.MessageFrom(target, "λNICK %Shad their color removed");
                target.UpdateColor(target.group.Color);
            } else {
                col = Matcher.FindColor(p, colName);
                if (col == null) return;
                if (col == target.color) { p.Message("{0} %Salready has that color.", p.FormatNick(target)); return; }
                
                Chat.MessageFrom(target, "λNICK %Shad their color changed to " + col + Colors.Name(col));
                target.UpdateColor(col);
            }
            PlayerDB.Update(target.name, PlayerData.ColumnColor, col);
        }
        
        public override void Help(Player p) {
            p.Message("%T/Color [player] [color]");
            p.Message("%HSets the nick color of that player");
            p.Message("%H  If [color] is not given, reverts to player's rank color.");
            p.Message("%T/Color bot [bot] [color]");
            p.Message("%HSets the name color of that bot.");
            p.Message("%HTo see a list of all colors, use /Help colors.");
        }
    }
}
