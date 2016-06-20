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
namespace MCGalaxy.Commands.Moderation {    
    public sealed class CmdWarn : Command {        
        public override string name { get { return "warn"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.Moderation; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Builder; } }

        public override void Use(Player p, string message) {
            if (message == "") { Help(p); return; }
            string[] args = message.SplitSpaces(2);
            Player who = PlayerInfo.FindMatches(p, args[0]);
            string reason = args.Length == 1 ? "you know why." : args[1];
            
            if (who == null) { WarnOffline(p, args); return; }
            if (who == p) { Player.Message(p, "you can't warn yourself"); return; }
            if (p != null && p.group.Permission <= who.group.Permission) {
                MessageTooHighRank(p, "warn", false); return;
            }           
            
            string warnedby = (p == null) ? "(console)" : p.ColoredName;
            Player.GlobalMessage(warnedby + " %ewarned " + who.ColoredName + " %ebecause:");
            Player.GlobalMessage("&c" + reason);
            Server.IRC.Say(warnedby + " %ewarned " + who.ColoredName + " %efor: %c" + reason);
            Server.s.Log(warnedby + " warned " + who.name);

            if (who.warn == 0) {
                Player.Message(who, "Do it again twice and you will get kicked!");
            } else if (who.warn == 1) {
                Player.Message(who, "Do it one more time and you will get kicked!");
            } else if (who.warn == 2) {
                Player.GlobalMessage(who.ColoredName + " %Swas warn-kicked by " + warnedby);
                who.Kick("KICKED BECAUSE " + reason + "");
            }
            
            who.warn++;
            if (args.Length == 1) Player.AddNote(who.name, p, "W");
            else Player.AddNote(who.name, p, "W", args[1]);
        }
        
        static void WarnOffline(Player p, string[] args) {
            if (!Server.LogNotes) { 
                Player.Message(p, "Notes logging must be enabled to warn offline players."); return;
            }
            Player.Message(p, "Searching PlayerDB..");
            string offName = PlayerInfo.FindOfflineNameMatches(p, args[0]);
            if (offName == null) return;
            
            if (args.Length == 1) Player.AddNote(offName, p, "W");
            else Player.AddNote(offName, p, "W", args[1]);
            string reason = args.Length > 1 ? " for: " + args[1] : "";
            Player.Message(p, "Warned {0}{1}.", offName, reason);
        }
        
        public override void Help(Player p) {
            Player.Message(p, "/warn <player> <reason> - Warns a player.");
            Player.Message(p, "Player will get kicked after 3 warnings.");
        }
    }
}
