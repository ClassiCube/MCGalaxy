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
    
	public class CmdTitle : EntityPropertyCmd {        
        public override string name { get { return "title"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.Chat; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Admin; } }
        public override CommandPerm[] ExtraPerms {
            get { return new[] { new CommandPerm(LevelPermission.Admin, "+ can change the title of others") }; }
        }
        public override CommandAlias[] Aliases {
            get { return new[] { new CommandAlias("xtitle", "-own") }; }
        }

        public override void Use(Player p, string message) { UsePlayer(p, message, "title"); }
        
        protected override void SetPlayerData(Player p, Player who, string[] args) {
            string title = args.Length > 1 ? args[1] : "";
            title = title.Replace("[", "").Replace("]", "");
            if (title.Length >= 20) { Player.Message(p, "Title must be under 20 letters."); return; }

            if (title == "") {
                Player.SendChatFrom(who, who.FullName + " %Shad their title removed.", false);
                Database.Execute("UPDATE Players SET Title = '' WHERE Name = @0", who.name);
            } else {
                Player.SendChatFrom(who, who.FullName + " %Swas given the title of &b[" + title + "&b]", false);
                Database.Execute("UPDATE Players SET Title = @1 WHERE Name = @0", who.name, title);
            }        
            who.title = title;
            who.SetPrefix();
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/title [player] [title]");
            Player.Message(p, "%HSets the title of [player]");
            Player.Message(p, "%H  If [title] is not given, removes [player]'s title.");
        }
    }
}
