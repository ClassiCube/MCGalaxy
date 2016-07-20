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
        public override string type { get { return CommandTypes.Chat; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Admin; } }
        public override CommandPerm[] ExtraPerms {
            get { return new[] { new CommandPerm(LevelPermission.Admin, "+ can change the title of others") }; }
        }
        public override CommandAlias[] Aliases {
            get { return new[] { new CommandAlias("xtitle", "-own") }; }
        }

        public override void Use(Player p, string message) {
            if (message == "") { Help(p); return; }
            string[] args = message.SplitSpaces(2);
            if (args[0].CaselessEq("-own")) {
                if (Player.IsSuper(p)) { SuperRequiresArgs(p, "player name"); return; }
                args[0] = p.name;
            }
            
            Player who = PlayerInfo.FindMatches(p, args[0]);
            if (who == null) return;
            if (p != null && who.Rank > p.Rank) {
                MessageTooHighRank(p, "change the title of", true); return;
            }
            if (who != p && !CheckExtraPerm(p)) { MessageNeedExtra(p, "can change the title of others."); return; }
            SetTitle(p, who, args);
        }
        
        static void SetTitle(Player p, Player who, string[] args) {
            string title = args.Length > 1 ? args[1] : "";
            ParameterisedQuery query = ParameterisedQuery.Create();
            if (title != "")
                title = title.Replace("[", "").Replace("]", "");
            if (title.Length >= 20) { Player.Message(p, "Title must be under 20 letters."); return; }

            if (title == "") {
                Player.SendChatFrom(who, who.FullName + " %Shad their title removed.", false);
                query.AddParam("@Name", who.name);
                Database.executeQuery(query, "UPDATE Players SET Title = '' WHERE Name = @Name");
            } else {
                Player.SendChatFrom(who, who.FullName + " %Swas given the title of &b[" + title + "%b]", false);
                query.AddParam("@Title", title);
                query.AddParam("@Name", who.name);
                Database.executeQuery(query, "UPDATE Players SET Title = @Title WHERE Name = @Name");
            }        
            who.title = title;
            who.SetPrefix();            
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/title <player> [title]");
            Player.Message(p, "%HSets the title of <player>");
            Player.Message(p, "%HIf no [title] is given, removes player's title.");
        }
    }
}
