/*
    Copyright 2010 MCLawl Team - Written by Valek (Modified for use with MCGalaxy)
 
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
using MCGalaxy.SQL;
namespace MCGalaxy.Commands {    
    public class CmdTColor : Command {
        
        public override string name { get { return "tcolor"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.Chat; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Admin; } }
        public override CommandPerm[] ExtraPerms {
            get { return new[] { new CommandPerm(LevelPermission.Operator, "+ can change the title color of others") }; }
        }
        public override CommandAlias[] Aliases {
            get { return new[] { new CommandAlias("tcolour"), new CommandAlias("xtcolor", "-own") }; }
        }

        public override void Use(Player p, string message) {
            if (message == "") { Help(p); return; }
            string[] args = message.Split(' ');
            if (args[0].CaselessEq("-own")) {
                if (Player.IsSuper(p)) { SuperRequiresArgs(p, "player name"); return; }
                args[0] = p.name;
            }
            
            Player who = PlayerInfo.FindMatches(p, args[0]);
            if (who == null) return;
            if (p != null && who.Rank > p.Rank) {
                MessageTooHighRank(p, "change the title color of", true); return;
            }
            if (who != p && !CheckExtraPerm(p)) { MessageNeedExtra(p, "can change the title color of others."); return; }
            SetTColor(p, who, args);
        }
        
        static void SetTColor(Player p, Player who, string[] args) {
            if (args.Length == 1) {                
                Player.SendChatFrom(who, who.ColoredName + " %Shad their title color removed.", false);
                who.titlecolor = "";
                Database.Execute("UPDATE Players SET title_color = '' WHERE Name = @0", who.name);
            } else  {
                string color = Colors.Parse(args[1]);
                if (color == "") { Player.Message(p, "There is no color \"" + args[1] + "\"."); return; }
                else if (color == who.titlecolor) { Player.Message(p, who.DisplayName + " already has that title color."); return; }
                Player.SendChatFrom(who, who.ColoredName + " %Shad their title color changed to " + color + Colors.Name(color) + "%S.", false);
                who.titlecolor = color;
                Database.Execute("UPDATE Players SET title_color = @1 WHERE Name = @0", who.name, color);                
            }
            who.SetPrefix();        	
        }

        public override void Help(Player p) {
            Player.Message(p, "%T/tcolor <player> [color]");
            Player.Message(p, "%HSets the title color of [player]");
            Player.Message(p, "%HIf no [color] is specified, title color is removed.");
            Player.Message(p, "%HTo see a list of all colors, use /help colors.");
        }
    }
}
