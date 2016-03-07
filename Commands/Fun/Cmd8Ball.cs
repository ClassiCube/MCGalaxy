/*
    Copyright 2015 MCGalaxy
        
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
	
    public sealed class Cmd8Ball : Command {

        public override string name { get { return "8ball"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.Other; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Banned; } }

        public override void Use(Player p, string message) {
            if (String.IsNullOrEmpty(message)) { Help(p); return; }
            if (p.joker || p.muted) { Player.SendMessage(p, "Cannot use /me while muted or jokered."); return; }
            Random random = new Random();
            string[] messages = { "Not likely." , "Very likely." , "Impossible!" , "Probably." , "Ask again later." , "No." , "Maybe." };
            Chat.GlobalChatLevel(p, p.color + "*" + Colors.StripColours(p.DisplayName) + p.color + "asked the quesiton " + message + " ", false);
            Chat.GlobalChatLevel(p, p.color + "*" + Colors.StripColours(p.DisplayName) + p.color + "and the awnser was " + [random.Next(messages.Length)], false);
        }

        public override void Help(Player p) {
            Player.SendMessage(p, "%T/8ball (message)");
            Player.SendMessage(p, "%HGives you a meaningless response to a question.");
        }
    }
}
