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
using MCGalaxy.SQL;
namespace MCGalaxy.Commands {    
    public class CmdColor : Command {
		
        public override string name { get { return "color"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.Chat; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        public override CommandPerm[] AdditionalPerms {
            get { return new[] { new CommandPerm(LevelPermission.Operator, "+ can change the color of other players") }; }
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
            
            Player who = PlayerInfo.FindMatches(p, args[0]);
            if (who == null) return;
            if (p != null && who.group.Permission > p.group.Permission) {
                MessageTooHighRank(p, "change the color of", true); return;
            }
            if (who != p && !CheckExtraPerm(p)) { MessageNeedPerms(p, "can change the color of other players."); return; }
            
            ParameterisedQuery query = ParameterisedQuery.Create();
            if (args.Length == 1) {
                Player.SendChatFrom(who, who.ColoredName + " %Shad their color removed.", false);
                who.color = who.group.color;
                
                query.AddParam("@Name", who.name);
                Database.executeQuery(query, "UPDATE Players SET color = '' WHERE name = @Name");
            } else {
                string color = Colors.Parse(args[1]);
                if (color == "") { Player.Message(p, "There is no color \"" + args[1] + "\"."); return; }
                else if (color == who.color) { Player.Message(p, p.DisplayName + " already has that color."); return; }
                Player.SendChatFrom(who, who.ColoredName + " %Shad their color changed to " + color + Colors.Name(color) + "%S.", false);
                who.color = color;
                
                query.AddParam("@Color", Colors.Name(color));
                query.AddParam("@Name", who.name);
                Database.executeQuery(query, "UPDATE Players SET color = @Color WHERE name = @Name");
            }
            Entities.GlobalDespawn(who, false);
            Entities.GlobalSpawn(who, false);
            who.SetPrefix();
        }
        
        public override void Help(Player p) {
            Player.Message(p, "/color <player> [color] - Sets the nick color of <player>");
            Player.Message(p, "If no [color] is given, reverts to player's rank color.");
            Player.Message(p, "To see a list of all colors, use /help colors.");
        }
    }
}
