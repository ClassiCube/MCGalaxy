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
using System.Threading;
using System.Text;

namespace MCGalaxy.Commands {
    public sealed class Cmd8Ball : Command {
		private static bool canUse = true;
        public override string name { get { return "8ball"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.Chat; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Guest; } }
        public Cmd8Ball() { }
        string[] messages = { "Not likely." , "Very likely." , "Impossible!" , "No." , "Yes." , "Definitely!" , "Do some more thinking." };
        
        public override void Use(Player p, string message) {
        	if (Player.IsSuper(p)) { MessageInGameOnly(p); return; }
			if (Server.chatmod || p.muted) {
                Player.SendMessage(p, "Cannot use 8-ball while muted.");
                return;
            }	
			if (canUse == false) {Player.SendMessage(p, "Must wait 10 seconds from the last time the command was used to use it again!"); return;}
            if (message == "") { Help(p); return; }
			canUse = false;	
           
            StringBuilder builder = new StringBuilder(message.Length);
            foreach (char c in message) {
                if (ValidChar(c)) builder.Append(c);
            }
           
            string final = builder.ToString();
            Random random = new Random(final.ToLower().GetHashCode());
            Player.GlobalMessage(p.color + p.DisplayName + "%s asked the %b8-Ball: %f" + message);
            Thread.Sleep(2000);
            Player.GlobalMessage("The %b8-Ball %ssays:%f " + messages[random.Next(messages.Length)]);
			Thread.Sleep(10000);
			canUse = true;
        }
       
        bool ValidChar(char c) {
            return (c >= '0' && c <= '9') || (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z');
        }

        public override void Help(Player p) {
            Player.Message(p, "%T/8ball [yes or no question]");
            Player.Message(p, "%HGet an answer from the all-knowing 8-Ball!");
        }
    }
}
