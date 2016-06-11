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
using MCGalaxy.SQL;
namespace MCGalaxy.Commands {
    
    public class CmdTitle : Command {
        
        public override string name { get { return "title"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.Other; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Admin; } }
        public override CommandPerm[] AdditionalPerms {
            get { return new[] { new CommandPerm(LevelPermission.Admin, "+ can change the title of other players") }; }
        }
        public override CommandAlias[] Aliases {
            get { return new[] { new CommandAlias("xtitle", "-own") }; }
        }
        static char[] trimChars = { ' ' };

        public override void Use(Player p, string message) {
            if (message == "") { Help(p); return; }
            string[] args = message.Split(trimChars, 2);
            if (args[0].CaselessEq("-own")) {
                if (Player.IsSuper(p)) { SuperRequiresArgs(p, "player name"); return; }
                args[0] = p.name;
            }
            
            Player who = PlayerInfo.FindMatches(p, args[0]);
            if (who == null) return;
            if (p != null && who.group.Permission > p.group.Permission) {
                MessageTooHighRank(p, "change the title of", true); return;
            }
            if (who != p && !CheckExtraPerm(p)) { MessageNeedPerms(p, "can change the title of other players."); return; }
            
            string newTitle = args.Length > 1 ? args[1] : "";
            ParameterisedQuery query = ParameterisedQuery.Create();
            if (newTitle != "")
                newTitle = newTitle.Replace("[", "").Replace("]", "");
            if (newTitle.Length >= 20) { Player.Message(p, "Title must be under 20 letters."); return; }

            if (newTitle == "") {
                Player.SendChatFrom(who, who.FullName + " %Shad their title removed.", false);
                query.AddParam("@Name", who.name);
                Database.executeQuery(query, "UPDATE Players SET Title = '' WHERE Name = @Name");
            } else {
                Player.SendChatFrom(who, who.FullName + " %Swas given the title of &b[" + newTitle + "%b]", false);
                query.AddParam("@Title", newTitle);
                query.AddParam("@Name", who.name);
                Database.executeQuery(query, "UPDATE Players SET Title = @Title WHERE Name = @Name");
            }        
            who.title = newTitle;
            who.SetPrefix();
        }
        
        public override void Help(Player p) {
            Player.Message(p, "/title <player> [title] - Sets the title of <player>");
            Player.Message(p, "If no [title] is given, removes player's title.");
        }
    }
}
