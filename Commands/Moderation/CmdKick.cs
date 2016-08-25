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
using System;

namespace MCGalaxy.Commands.Moderation {    
    public sealed class CmdKick : ModActionCmd {       
        public override string name { get { return "kick"; } }
        public override string shortcut { get { return "k"; } }
        public override LevelPermission defaultRank { get { return LevelPermission.AdvBuilder; } }
        
        public override void Use(Player p, string message) {
            if (message == "") { Help(p); return; }
            string[] args = message.SplitSpaces(2);
            
            Player who = PlayerInfo.FindMatches(p, args[0]);
            if (who == null) return;
            if (args.Length > 1) message = args[1];
            else if (p == null) message = "You were kicked by the console.";
            else message = "You were kicked by " + p.DisplayName + ".";
            
            message = GetReason(p, message);
            if (message == null) return;

            if (p != null && p == who) { Player.Message(p, "You cannot kick yourself."); return; }
            if (p != null && who.Rank >= p.Rank) {
                Player.SendChatFrom(p, p.ColoredName + "%S tried to kick " 
                                    + who.ColoredName + "%S but failed.", false);
                return;
            }
            
            who.Kick(message);
            if (args.Length == 1) Player.AddNote(who.name, p, "K");
            else Player.AddNote(who.name, p, "K", message);
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/kick [player] <reason>");
            Player.Message(p, "%HKicks a player.");
            Player.Message(p, "%HFor <reason>, @number can be used as a shortcut for that rule.");
        }
    }
}
