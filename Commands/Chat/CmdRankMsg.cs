/*
    Written by Jack1312
  
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
using System;

namespace MCGalaxy.Commands {	
    public sealed class CmdRankMsg : Command {		
        public override string name { get { return "rankmsg"; } }
        public override string shortcut { get { return "rm"; } }
        public override string type { get { return CommandTypes.Chat; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.AdvBuilder; } }

        public override void Use(Player p, string message) {
            if (message == "") { Help(p); return; }
            string[] args = message.SplitSpaces(2);
            string rank = args.Length == 1 ? p.group.name : args[0];
            string text = args[args.Length - 1];            
            Group grp = Group.FindMatches(p, rank);
            if (grp == null) return;
            
            Player[] players = PlayerInfo.Online.Items;
            string toSend = p.color + p.name + ": %S" + text.Trim();
            foreach (Player pl in players) {
                if (pl.group.name == grp.name) pl.SendMessage(toSend);
            }
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/rankmsg [Rank] [Message]");
            Player.Message(p, "%HSends a message to the specified rank.");
            Player.Message(p, "%HNote: If no [rank] is given, player's rank is taken.");
        }
    }
}
