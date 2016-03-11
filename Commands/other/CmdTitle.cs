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
    
    public sealed class CmdTitle : Command {
        
        public override string name { get { return "title"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.Other; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Admin; } }
        public CmdTitle() { }
        static char[] trimChars = { ' ' };

        public override void Use(Player p, string message) {
            if (message == "") { Help(p); return; }
            string[] parts = message.Split(trimChars, 2);

            Player who = PlayerInfo.FindOrShowMatches(p, parts[0]);
            if (who == null) return;
            if (p != null && who.group.Permission > p.group.Permission) {
                Player.SendMessage(p, "Cannot change the title of someone of greater rank"); return;
            }
            
            string newTitle = parts.Length > 1 ? parts[1] : "";
            if (newTitle != "")
                newTitle = newTitle.Replace("[", "").Replace("]", "");
            if (newTitle.Length > 17) { Player.SendMessage(p, "Title must be under 17 letters."); return; }

            if (newTitle == "") {
                Player.SendChatFrom(who, who.color + who.prefix + who.name + " %Shad their title removed.", false);
                Database.AddParams("@Name", who.name);
                Database.executeQuery("UPDATE Players SET Title = '' WHERE Name = @Name");
            } else {
                Player.SendChatFrom(who, who.color + who.name + " %Swas given the title of &b[" + newTitle + "%b]", false);
                Database.AddParams("@Title", newTitle);
                Database.AddParams("@Name", who.name);
                Database.executeQuery("UPDATE Players SET Title = @Title WHERE Name = @Name");
            }        
            who.title = newTitle;
            who.SetPrefix();
        }
        
        public override void Help(Player p) {
            Player.SendMessage(p, "/title <player> [title] - Gives <player> the [title].");
            Player.SendMessage(p, "If no [title] is given, the player's title is removed.");
        }
    }
    
    public class CmdXTitle : Command {
        
        public override string name { get { return "xtitle"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.Other; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Admin; } }
        public CmdXTitle() { }

        public override void Use(Player p, string message) {
            if (message != "") message = " " + message;
            Command.all.Find("title").Use(p, p.name + message);
        }
        
        public override void Help(Player p) {
            Player.SendMessage(p, "/xtitle [title] - Gives you the [title].");
            Player.SendMessage(p, "If no [title] is given, your title is removed.");
        }
    }
}
