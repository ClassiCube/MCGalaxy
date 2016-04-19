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
namespace MCGalaxy.Commands {
    
    public sealed class CmdWarn : Command {
        
        public override string name { get { return "warn"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.Moderation; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Builder; } }
        static char[] trimChars = { ' ' };

        public override void Use(Player p, string message) {
            if (message == "") { Help(p); return; }
            string[] args = message.Split(trimChars, 2);
            Player who = PlayerInfo.FindOrShowMatches(p, args[0]);
            
            if (who == null) return;
            if (who == p) { Player.SendMessage(p, "you can't warn yourself"); return; }
            if (p != null && p.group.Permission <= who.group.Permission) {
                MessageTooHighRank(p, "warn", false); return;
            }
            
            string reason = args.Length == 1 ? "you know why." : args[1];
            string warnedby = (p == null) ? "<CONSOLE>" : p.color + p.DisplayName;
            Player.GlobalMessage(warnedby + " %ewarned " + who.color + who.DisplayName + " %ebecause:");
            Player.GlobalMessage("&c" + reason);
            Server.IRC.Say(warnedby + " %ewarned " + who.color + who.DisplayName + " %efor: %c" + reason);
            Server.s.Log(warnedby + " warned " + who.name);

            if (who.warn == 0) {
                Player.SendMessage(who, "Do it again twice and you will get kicked!");
            } else if (who.warn == 1) {
                Player.SendMessage(who, "Do it one more time and you will get kicked!");
            } else if (who.warn == 2) {
                Player.GlobalMessage(who.color + who.DisplayName + " %Swas warn-kicked by " + warnedby);
                who.Kick("KICKED BECAUSE " + reason + "");
            }
            
            who.warn++;
            if (args.Length == 1) Player.AddNote(who.name, p, "W");
            else Player.AddNote(who.name, p, "W", args[1]);
        }
        
        public override void Help(Player p) {
            Player.SendMessage(p, "/warn <player> <reason> - Warns a player.");
            Player.SendMessage(p, "Player will get kicked after 3 warnings.");
        }
    }
}
